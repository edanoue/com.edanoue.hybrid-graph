// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;

namespace Edanoue.HybridGraph
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TBlackboard"></typeparam>
    public abstract class LeafState<TBlackboard> : IGraphNode
    {
        /// <summary>
        /// <para>内部用のTransition Table (Trigger 用)</para>
        /// </summary>
        private readonly Dictionary<int, IGraphNode> _transitionTable = new();

        /// <summary>
        /// <para>Transition Table (Condition 用)</para>
        /// <remarks>Trigger とは異なり同一 Node に対しての複数 Condition を許可しないため Key, Value を逆に</remarks>
        /// </summary>
        private readonly Dictionary<IGraphNode, Func<bool>> _transitionTableWithCondition = new();

        private CancellationTokenSource? _onExitCts;

        private IGraphBox? _parent;

        /// <summary>
        /// Gets the blackboard.
        /// </summary>
        protected TBlackboard Blackboard = default!;

        /// <summary>
        /// Gets the cancellation token raised when the State is exited.
        /// </summary>
        [Obsolete]
        protected CancellationToken CancellationTokenOnExit
        {
            get
            {
                if (_onExitCts is not null)
                {
                    return _onExitCts.Token;
                }

                throw new InvalidOperationException("CancellationTokenOnExit is available when OnEnter or OnStay.");
            }
        }

        void IGraphItem.Connect(int trigger, IGraphItem nextNode)
        {
            if (_transitionTable.ContainsKey(trigger))
            {
                throw new ArgumentException($"Already registered trigger: {trigger}");
            }

            _transitionTable.Add(trigger, nextNode.GetEntryNode());
        }

        void IGraphItem.Connect(Func<bool> condition, IGraphItem nextNode)
        {
            var entryNode = nextNode.GetEntryNode();
            if (_transitionTableWithCondition.ContainsKey(entryNode))
            {
                throw new ArgumentException($"Already registered condition to {nextNode}");
            }

            _transitionTableWithCondition.Add(entryNode, condition);
        }

        void IGraphItem.OnInitializedInternal(object blackboard, IGraphBox parent)
        {
            Blackboard = (TBlackboard)blackboard;
            _parent = parent;
            OnInitialize();
        }

        void IGraphItem.WrappedOnEnter()
        {
            _parent?.WrappedOnEnter();
            _onExitCts = new CancellationTokenSource(); // ToDo: 必要なときだけ生成するように!
            OnEnter();
        }

        void IGraphItem.WrappedOnExecute()
        {
            _parent?.WrappedOnExecute();
            OnExecute();
        }

        void IGraphItem.WrappedOnExit(IGraphItem nextNode)
        {
            // CancellationTokenをキャンセルする
            _onExitCts?.Cancel();
            _onExitCts?.Dispose();
            _onExitCts = null;

            OnExit();
            _parent?.WrappedOnExit(nextNode);
        }

        IGraphNode IGraphItem.GetEntryNode()
        {
            return this;
        }

        bool IGraphNode.TryGetNextNode(int trigger, out IGraphNode nextNode)
        {
            return _transitionTable.TryGetValue(trigger, out nextNode);
        }

        bool IGraphNode.TryGetNextNodeWithCondition(out IGraphNode nextNode)
        {
            foreach (var pair in _transitionTableWithCondition)
            {
                // Check condition
                if (pair.Value())
                {
                    nextNode = pair.Key;
                    return true;
                }
            }

            nextNode = default!;
            return false;
        }

        public void Dispose()
        {
            // CancellationTokenをキャンセルする
            _onExitCts?.Cancel();
            _onExitCts?.Dispose();

            OnDestroy();
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnExecute()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnDestroy()
        {
        }
    }
}