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
                FTP = null;
                NotifyStateChanged();
            }
        }

        public event Action CurrentChanged;

        private void NotifyStateChanged() => CurrentChanged?.Invoke();
        public string FTP { get; set; } = null;
    }    
}
