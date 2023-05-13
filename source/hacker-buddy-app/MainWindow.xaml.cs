using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using ChatGPT.Net;
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
        private Guid _chatId;

        private DateTime _lastGptUpdate = DateTime.MinValue;
        private TimeSpan _gptUpdateThreashold = TimeSpan.FromMinutes(1);
        private ChatGpt _bot = null;

        public MainWindow()
        {
            InitializeComponent();
            Vibes = App.Vibes;
            this.DataContext = this;

            PiperPiped.Instance.VibeRefreshed += InstanceOnVibeRefreshed;

            _bot = new ChatGpt(App.GptApiKey);
        }

        public string Vibes
        {
            get => _vibes;
            set
            {
                if (value == _vibes) return;
                _vibes = value;
                UpdateChatGPTAsync(_vibes);
                OnPropertyChanged();
            }
        }

        private async Task UpdateChatGPTAsync(string vibe)
        {
            if (DateTime.Now - _lastGptUpdate < _gptUpdateThreashold)
                return;

            _lastGptUpdate = DateTime.Now;

            ClippsySays = $"Hallo mein freund, ich habe bemerkt, dass es dir {Vibes} geht. Zur aufmunterung, habe ich meinen kleinen Buddy angeschrieben!";
            var answer = await _bot.Ask($"Ich bin Softwareenwickler und heute fühle ich mich {vibe}! Sag mir in zwei sätzen, was du mir empfiehlst!?", _chatId.ToString());
            GptSays = answer;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var answer = await _bot.Ask(User);
            //GptSays = answer;
        }
    }
}
