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
        /// 内部用のTransition Table
        /// 遷移先を辞書形式で保存している
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly Dictionary<int, IGraphNode> _transitionTable = new();

        private CancellationTokenSource? _onExitCts;

        private IGraphBox? _parent;

        /// <summary>
        /// Get the blackboard.
        /// </summary>
        protected TBlackboard Blackboard = default!;

        /// <summary>
        /// Get the cancellation token raised when the State is exited.
        /// </summary>
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

        void IGraphItem.OnInitializedInternal(object blackboard, IGraphBox parent)
        {
            Blackboard = (TBlackboard)blackboard;
            _parent = parent;
            OnInitialize();
        }

        void IGraphItem.WrappedOnEnter()
        {
            _parent?.WrappedOnEnter();
            _onExitCts = new CancellationTokenSource();
            OnEnter();
        }

        void IGraphItem.WrappedOnExecute()
        {
            OnStay();
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

        protected virtual void OnStay()
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