namespace Core.FSM
{
    public sealed class StateMachine
    {
        private IState _current;
        public IState Current => _current;

        public void Change(IState next)
        {
            if (_current == next) return;
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        public void Update() => _current?.Update();
    }
}