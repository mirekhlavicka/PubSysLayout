namespace PubSysLayout.Client
{
    public class CurrentDB
    {
        private string current = null;

        public string Current
        {
            get => current;
            set
            {
                current = value;
                NotifyStateChanged();
            }
        }

        public event Action CurrentChanged;

        private void NotifyStateChanged() => CurrentChanged?.Invoke();
    }
}
