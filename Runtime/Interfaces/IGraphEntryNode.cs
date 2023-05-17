// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;

namespace Edanoue.HybridGraph
{
    public interface IGraphEntryNode : IDisposable
    {
        /// <summary>
        /// HybridGraph により実行されるエントリーポイント
        /// </summary>
        /// <param name="blackboard"></param>
        internal IGraphNode Run(object blackboard);
    }
}