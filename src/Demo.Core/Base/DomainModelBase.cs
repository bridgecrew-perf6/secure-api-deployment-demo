namespace Demo.Core.Base
{
    public abstract class DomainModelBase<TId>
    {

        public TId Id { get; }

        public TrackingState TrackingState { get; private set; }

        protected void SetState(TrackingState newState)
        {
            if (TrackingState.CanChangeTo(newState))
            {
                TrackingState = newState;
            }
        }

        protected DomainModelBase(TId id, TrackingState initialState = null)
        {
            Id = id;
            TrackingState = initialState ?? TrackingState.Pristine;
        }


    }
}