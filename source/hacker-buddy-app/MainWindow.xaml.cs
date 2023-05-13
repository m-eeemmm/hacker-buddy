using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using hacker_buddy_app.Pipe;
using hacker_buddy_console.Pipe;

namespace hacker_buddy_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _vibes;
        private string _gptSays;
        private string _glippsySays;

        public MainWindow()
        {
            InitializeComponent();
            Vibes = App.Vibes;
            this.DataContext = this;

            PiperPiped.Instance.VibeRefreshed += InstanceOnVibeRefreshed;
        }

        public string Vibes
        {
            get => _vibes;
            set
            {
                if (value == _vibes) return;
                _vibes = value;
                OnPropertyChanged();
            }
        }

        public string ClippsySays
        {
            get => _glippsySays;
            set
            {
                if (value == _glippsySays) return;
                _glippsySays = value;
                OnPropertyChanged();
            }
        }

        public string GptSays
        {
            get => _gptSays;
            set
            {
                if (value == _gptSays) return;
                _gptSays = value;
                OnPropertyChanged();
            }
        }


        private void InstanceOnVibeRefreshed(object? sender, VibePackage e)
        {
            Vibes = e.Vibe;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
