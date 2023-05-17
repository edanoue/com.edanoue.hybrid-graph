// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
namespace Edanoue.HybridGraph
{
    public interface IStateBuilder
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetInitialState<T>()
            where T : class, IGraphItem, new();

        /// <summary>
        /// </summary>
        /// <param name="trigger"></param>
        /// <typeparam name="TPrev"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        public void AddTransition<TPrev, TNext>(int trigger)
            where TPrev : class, IGraphItem, new()
            where TNext : class, IGraphItem, new();
    }
}