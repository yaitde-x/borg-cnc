namespace Borg.Machine
{
    public class RS274StateBuilder
    {
        private readonly RS274State _state;

        public RS274StateBuilder()
            : this(RS274State.Default)
        {

        }

        public RS274StateBuilder(RS274State state)
        {
            _state = state;
        }

        public RS274StateBuilder WithX(double x)
        {
            _state.X = x;
            return this;
        }
        public RS274StateBuilder WithY(double y)
        {
            _state.Y = y;
            return this;
        }
        public RS274StateBuilder WithZ(double z)
        {
            _state.Z = z;
            return this;
        }

        public RS274StateBuilder WithFeed(double feed)
        {
            _state.Feed = feed;
            return this;
        }

        public RS274StateBuilder WithSpeed(double speed)
        {
            _state.Speed = speed;
            return this;
        }

        public RS274StateBuilder WithRunState(RunState runState)
        {
            _state.State = runState;
            return this;
        }

        public RS274StateBuilder WithModal(string modal, string value)
        {
            _state.SetModal(modal, value);
            return this;
        }

        public RS274StateBuilder WithExistingState(RS274State state)
        {
            return this
                    .WithX(state.X)
                    .WithY(state.Y)
                    .WithZ(state.Z)
                    .WithSpeed(state.Speed)
                    .WithFeed(state.Feed)
                    .WithRunState(state.State);
        }

        public RS274State Build()
        {
            return _state;
        }
    }
}