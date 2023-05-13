using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

            Task.Run(() =>
            {
                Thread.Sleep(10000);
                App.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
            });
        }
    }
}
