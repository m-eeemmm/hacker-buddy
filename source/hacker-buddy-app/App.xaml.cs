using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using hacker_buddy_app.Pipe;
using hacker_buddy_console.Pipe;

namespace hacker_buddy_app
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Vibes;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if(e.Args.Length > 0)
            {
                Vibes = e.Args[0]; 
            }


            PiperPiped.Instance.VibeRefreshed += InstanceOnVibeRefreshed;
        }

        private void InstanceOnVibeRefreshed(object? sender, VibePackage e)
        {
            Vibes = e.Vibe;
        }
    }
}
