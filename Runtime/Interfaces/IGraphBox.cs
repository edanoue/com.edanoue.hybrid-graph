// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
namespace Edanoue.HybridGraph
{
    internal interface IGraphBox : IGraphItem
    {
        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="includeDescendants"></param>
        /// <returns></returns>
        public bool HasNode(IGraphItem node, bool includeDescendants = true);
    }
}