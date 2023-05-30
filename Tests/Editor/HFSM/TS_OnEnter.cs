// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using Edanoue.HybridGraph;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace HFSM
{
    public class TS_OnEnter
    {
        [Test]
        [Category("Normal")]
        public void 初期StateのOnEnter()
        {
            // Graph を作成しただけでは OnEnter は呼ばれない
            var bb = new BlackboardMockA();
            var graph = EdaGraph.Create<StateMachineMockA>(bb);
            Assert.That(bb._fooEnterCount, Is.EqualTo(0));
            Assert.That(bb._barEnterCount, Is.EqualTo(0));

            // Graph を実行すると初期 State に設定された OnEnter が呼ばれる
            graph.Execute();
            Assert.That(bb._fooEnterCount, Is.EqualTo(1));
            Assert.That(bb._barEnterCount, Is.EqualTo(0));
        }

        [Test]
        [Category("Normal")]
        public void Transitionが絡んだOnEnter()
        {
            var bb = new BlackboardMockA();
            var graph = EdaGraph.Create<StateMachineMockA>(bb);
            graph.Execute();

            // HFSM にトリガーを送付, この時点では State のコールバックは呼ばれない
            graph.SendTrigger(0);
            Assert.That(bb._fooEnterCount, Is.EqualTo(1));
            Assert.That(bb._barEnterCount, Is.EqualTo(0));

            // HFSM を更新, Transition が発生してコールバックが呼ばれる
            graph.Execute();
            Assert.That(bb._fooEnterCount, Is.EqualTo(1));
            Assert.That(bb._barEnterCount, Is.EqualTo(1));
        }

        [Test]
        [Category("Normal")]
        public void GroupStateのOnEnterが呼ばれる()
        {
            var bb = new BlackboardMockB();
            var graph = EdaGraph.Create<StateMachineMockB>(bb);

            // Graph を実行すると初期 State に設定された OnEnter が呼ばれる
            graph.Execute();

            // GroupState -> LeafState の順で呼ばれる
            Assert.That(bb._groupStateEnterCount, Is.EqualTo(1));
            Assert.That(bb._fooStateEnterCount, Is.EqualTo(1));
        }

        [Test]
        [Category("Normal")]
        public void GroupStateのOnEnterは子孫のLeafStateの遷移では呼ばれない()
        {
            var bb = new BlackboardMockB();
            var graph = EdaGraph.Create<StateMachineMockB>(bb);

            // State: Foo
            graph.Execute();

            // Transition を発生させる
            graph.SendTrigger(0);
            graph.Execute();

            // Group の OnEnter は呼ばれず, Leaf のみが呼ばれている
            Assert.That(bb._groupStateEnterCount, Is.EqualTo(1));
            Assert.That(bb._fooStateEnterCount, Is.EqualTo(1));
            Assert.That(bb._barStateEnterCount, Is.EqualTo(1));
        }

        #region Mock Classes

        private class BlackboardMockA
        {
            public int _barEnterCount;
            public int _fooEnterCount;
        }

        private class StateMachineMockA : StateMachine<BlackboardMockA>
        {
            protected override void OnSetupStates(IStateBuilder builder)
            {
                builder.AddTransition<StateFoo, StateBar>(0);
                builder.SetInitialState<StateFoo>();
            }

            private class StateFoo : LeafState<BlackboardMockA>
            {
                protected override void OnEnter()
                {
                    Blackboard._fooEnterCount++;
                }
            }

            private class StateBar : LeafState<BlackboardMockA>
            {
                protected override void OnEnter()
                {
                    Blackboard._barEnterCount++;
                }
            }
        }

        private class BlackboardMockB
        {
            public int _barStateEnterCount;
            public int _fooStateEnterCount;
            public int _groupStateEnterCount;
        }

        private class StateMachineMockB : StateMachine<BlackboardMockB>
        {
            protected override void OnSetupStates(IStateBuilder builder)
            {
                builder.AddTransition<StateFoo, StateBar>(0);
                builder.SetInitialState<StateFoo>();
            }

            protected override void OnEnter()
            {
                Blackboard._groupStateEnterCount++;
            }

            private class StateFoo : LeafState<BlackboardMockB>
            {
                protected override void OnEnter()
                {
                    if (Blackboard._groupStateEnterCount >= 1)
                    {
                        Blackboard._fooStateEnterCount++;
                    }
                }
            }

            private class StateBar : LeafState<BlackboardMockB>
            {
                protected override void OnEnter()
                {
                    Blackboard._barStateEnterCount++;
                }
            }
        }

        #endregion
    }
}