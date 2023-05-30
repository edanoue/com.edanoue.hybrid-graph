// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;
using System.Collections.Generic;

namespace Edanoue.HybridGraph
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TBlackboard"></typeparam>
    public abstract class LeafState<TBlackboard> : IGraphNode
    {
        /// <summary>
        /// <para>Transition Table (Condition 用)</para>
        /// <remarks>Trigger とは異なり同一 Node に対しての複数 Condition を許可しないため Key, Value を逆に</remarks>
        /// </summary>
        private readonly Dictionary<IGraphNode, Func<bool>> _transitionTableWithCondition = new();

        /// <summary>
        /// <para>内部用のTransition Table (Trigger 用)</para>
        /// </summary>
        private readonly Dictionary<int, IGraphNode> _transitionTableWithTrigger = new();

        private IGraphBox? _parent;

        /// <summary>
        /// Gets the blackboard.
        /// </summary>
        protected TBlackboard Blackboard = default!;


        void IGraphItem.Connect(int trigger, IGraphItem nextNode)
        {
            if (_transitionTableWithTrigger.ContainsKey(trigger))
            {
                throw new ArgumentException($"Already registered trigger: {trigger}");
            }

            _transitionTableWithTrigger.Add(trigger, nextNode.GetEntryNode());
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
            OnEnter();
        }

        void IGraphItem.WrappedOnExecute()
        {
            _parent?.WrappedOnExecute();
            OnExecute();
        }

        void IGraphItem.WrappedOnExit(IGraphItem nextNode)
        {
            OnExit();
            _parent?.WrappedOnExit(nextNode);
        }

        IGraphNode IGraphItem.GetEntryNode()
        {
            return this;
        }

        bool IGraphNode.TryGetNextNode(int trigger, out IGraphNode nextNode)
        {
            return _transitionTableWithTrigger.TryGetValue(trigger, out nextNode);
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
            OnDestroy();
        }

        /// <summary>
        /// Called when setup HFSM.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Called when entered this state each times.
        /// </summary>
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