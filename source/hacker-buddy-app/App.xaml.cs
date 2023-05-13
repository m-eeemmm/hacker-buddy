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
        }
    }
}
