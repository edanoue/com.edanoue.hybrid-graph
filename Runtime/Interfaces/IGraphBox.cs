// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
namespace Edanoue.HybridGraph
{
    public interface IGraphBox : IGraphItem
    {
        internal bool IsDescendantNode(IGraphItem node);
    }
}