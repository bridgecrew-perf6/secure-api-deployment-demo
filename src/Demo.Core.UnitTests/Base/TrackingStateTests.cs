using System.Linq;
using Demo.Core.Base;
using NUnit.Framework;

namespace Demo.Core.UnitTests.Base
{

    [TestFixture]
    public class TrackingStateTests
    {

        [TestCase(TrackingStateName.Pristine, TrackingStateName.New)]
        [TestCase(TrackingStateName.Modified, TrackingStateName.New)]
        [TestCase(TrackingStateName.Deleted, TrackingStateName.New)]
        [TestCase(TrackingStateName.New, TrackingStateName.New)]
        [TestCase(TrackingStateName.Touched, TrackingStateName.New)]
        public void WhenTryingToSetStateNew_StateChangeIsIgnored(string initialStateText, string newStateText)
        {
            var initialState = TrackingState.All.FirstOrDefault(states => states.Status == initialStateText);
            var newState = TrackingState.All.FirstOrDefault(states => states.Status == newStateText);

            var canChange = initialState.CanChangeTo(newState);
            Assert.IsFalse(canChange);
        }
    }
}