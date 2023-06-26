// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace Edanoue.HybridGraph
{
    public class EdaGraph : IDisposable
    {
        private readonly IGraphEntryNode _container;
        private          IGraphNode      _currentNode;
        private          bool            _disposed;
        private          bool            _isEntered;
        private          IGraphNode?     _nextNode;

        private EdaGraph(IGraphEntryNode container, object blackboard)
        {
            _container = container;
            _currentNode = container.InitializeAndGetEntryNode(blackboard);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EdaGraph));
            }

            _container.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="blackboard"></param>
        /// <typeparam name="T"></typeparam>
        public static EdaGraph Create<T>(object blackboard)
            where T : class, IGraphEntryNode, new()
        {
            // Initialize container (StateMachine or BehaviourTree)
            var container = new T();

            // Run container
            return new EdaGraph(container, blackboard);
        }

        /// <summary>
        /// </summary>
        public void Execute()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EdaGraph));
            }

            var currentNodeEnterSkippedWithCondition = false;

            if (!_isEntered)
            {
                _isEntered = true;
                if (!UpdateNextNodeWithCondition())
                {
                    _currentNode.WrappedOnEnter();
                }
                else
                {
                    currentNodeEnterSkippedWithCondition = true;
                }

                if (_nextNode is null)
                {
                    return;
                }
            }

            if (_nextNode is null)
            {
                if (!UpdateNextNodeWithCondition())
                {
                    // 現在のStateのUpdate関数を呼ぶ
                    _currentNode.WrappedOnExecute();
                }
            }

            // 次の遷移先が代入されていたら, ステートを切り替える
            while (_nextNode is not null)
            {
                // 以前のステートを終了する
                if (!currentNodeEnterSkippedWithCondition)
                {
                    _currentNode.WrappedOnExit(_nextNode);
                }

                // ステートの切り替え処理
                _currentNode = _nextNode;
                _nextNode = null;
                currentNodeEnterSkippedWithCondition = false;

                // 次のステートを開始する
                if (!UpdateNextNodeWithCondition())
                {
                    _currentNode.WrappedOnEnter();
                }
                else
                {
                    currentNodeEnterSkippedWithCondition = true;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool SendTrigger(int trigger)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EdaGraph));
            }

            if (_currentNode.TryGetNextNode(trigger, out var nextNode))
            {
                _nextNode = nextNode;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 現在の Node の Condition を確認, 遷移可能なら遷移を行う
        /// </summary>
        /// <returns>Condition により NextNode が更新されたら true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool UpdateNextNodeWithCondition()
        {
            if (!_currentNode.TryGetNextNodeWithCondition(out var nextNode))
            {
                return false;
            }

            _nextNode = nextNode;
            return true;
        }
    }
}