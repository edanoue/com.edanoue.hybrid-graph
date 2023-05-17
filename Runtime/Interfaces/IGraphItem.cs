// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;

namespace Edanoue.HybridGraph
{
    public interface IGraphItem : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="nextNode"></param>
        internal void Connect(int trigger, IGraphItem nextNode);

        internal void OnInitializedInternal(object blackboard, IGraphBox parent);

        internal void WrappedOnEnter();

        internal void WrappedOnExecute();

        internal void WrappedOnExit(IGraphItem nextNode);

        internal IGraphNode GetEntryNode();
    }
}