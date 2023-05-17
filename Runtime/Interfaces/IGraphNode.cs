// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
namespace Edanoue.HybridGraph
{
    public interface IGraphNode : IGraphItem
    {
        internal bool TryGetNextNode(int trigger, out IGraphNode nextNode);
    }
}