// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;

namespace Edanoue.HybridGraph
{
    public interface IGraphItem : IDisposable
    {
        /// <summary>
        /// Connect the node with trigger.
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="nextNode"></param>
        internal void Connect(int trigger, IGraphItem nextNode);

        /// <summary>
        /// Connect the node with condition.
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="nextNode"></param>
        internal void Connect(Func<bool> trigger, IGraphItem nextNode);

        internal void OnInitializedInternal(object blackboard, IGraphBox parent);

        internal void WrappedOnEnter();

        internal void WrappedOnExecute();

        internal void WrappedOnExit(IGraphItem nextNode);

        internal IGraphNode GetEntryNode();
    }
}