// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using Edanoue.HybridGraph;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace HFSM
{
    public class TS_TransitionWithCondition
    {
        [Test]
        [Category("Normal")]
        public void Conditionを満たすと遷移が発生する()
        {
            // 起動まで
            var bb = new BlackboardMockA();
            var graph = EdaGraph.Create<StateMachineMockA>(bb);
            graph.Execute();

            // Condition が false なので Transition は発生しない
            graph.Execute();
            Assert.That(bb._fooEnterCount, Is.EqualTo(1));
            Assert.That(bb._fooExecuteCount, Is.EqualTo(1));
            Assert.That(bb._barEnterCount, Is.EqualTo(0));

            // Condition が true なので Transition が発生する
            // Execute が呼ばれるまでは遷移はしていない
            bb._fooToBar = true;
            Assert.That(bb._fooEnterCount, Is.EqualTo(1));
            Assert.That(bb._barEnterCount, Is.EqualTo(0));

            // Graph の更新, ここで Condition による Transition が発生する
            graph.Execute();
            Assert.That(bb._fooEnterCount, Is.EqualTo(1));
            Assert.That(bb._fooExecuteCount, Is.EqualTo(1));
            Assert.That(bb._fooExitCount, Is.EqualTo(1));
            Assert.That(bb._barEnterCount, Is.EqualTo(1));
            Assert.That(bb._barExecuteCount, Is.EqualTo(0));
            Assert.That(bb._barExitCount, Is.EqualTo(0));
        }

        [Test]
        [Category("Normal")]
        public void Conditionを最初から満たしている場合は遷移が発生する()
        {
            // State 侵入時にすでに Condition が満たされている Node に付いてのテスト

            // 起動まで
            var bb = new BlackboardMockA();
            var graph = EdaGraph.Create<StateMachineMockA>(bb);

            // Graph 実行時に Foo -> Bar への遷移条件が既に満たされている
            bb._fooToBar = true;

            // Foo は一切コールバックが呼ばれず, Bar に Enter していることを確認する
            graph.Execute();
            Assert.That(bb._fooEnterCount, Is.EqualTo(0));
            Assert.That(bb._fooExecuteCount, Is.EqualTo(0));
            Assert.That(bb._fooExitCount, Is.EqualTo(0));
            Assert.That(bb._barEnterCount, Is.EqualTo(1));
            Assert.That(bb._barExecuteCount, Is.EqualTo(0));
            Assert.That(bb._barExitCount, Is.EqualTo(0));
        }

        private class BlackboardMockA
        {
            public int  _barEnterCount;
            public int  _barExecuteCount;
            public int  _barExitCount;
            public int  _fooEnterCount;
            public int  _fooExecuteCount;
            public int  _fooExitCount;
            public bool _fooToBar;
        }

        private class StateMachineMockA : StateMachine<BlackboardMockA>
        {
            protected override void OnSetupStates(IStateBuilder builder)
            {
                builder.AddTransition<StateFoo, StateBar>(() => Blackboard._fooToBar);
                builder.SetInitialState<StateFoo>();
            }

            private class StateFoo : LeafState<BlackboardMockA>
            {
                protected override void OnEnter()
                {
                    Blackboard._fooEnterCount++;
                }

                protected override void OnExecute()
                {
                    Blackboard._fooExecuteCount++;
                }

                protected override void OnExit()
                {
                    Blackboard._fooExitCount++;
                }
            }

            private class StateBar : LeafState<BlackboardMockA>
            {
                protected override void OnEnter()
                {
                    Blackboard._barEnterCount++;
                }

                protected override void OnExecute()
                {
                    Blackboard._barExecuteCount++;
                }

                protected override void OnExit()
                {
                    Blackboard._barExitCount++;
                }
            }
        }
    }
}