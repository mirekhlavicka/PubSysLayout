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

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
