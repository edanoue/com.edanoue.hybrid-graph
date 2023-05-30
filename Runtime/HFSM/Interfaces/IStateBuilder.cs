// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;

namespace Edanoue.HybridGraph
{
    public interface IStateBuilder
    {
        /// <summary>
        /// Set the initial state.
        /// </summary>
        /// <typeparam name="T">Initial state type</typeparam>
        public void SetInitialState<T>()
            where T : class, IGraphItem, new();

        /// <summary>
        /// Set the transition between states with trigger.
        /// </summary>
        /// <param name="trigger"></param>
        /// <typeparam name="TPrev">Previous state type</typeparam>
        /// <typeparam name="TNext">Next state type</typeparam>
        public void AddTransition<TPrev, TNext>(int trigger)
            where TPrev : class, IGraphItem, new()
            where TNext : class, IGraphItem, new();

        /// <summary>
        /// Set the transition between states with condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <typeparam name="TPrev">Previous state type</typeparam>
        /// <typeparam name="TNext">Next state type</typeparam>
        public void AddTransition<TPrev, TNext>(Func<bool> condition)
            where TPrev : class, IGraphItem, new()
            where TNext : class, IGraphItem, new();
    }
}