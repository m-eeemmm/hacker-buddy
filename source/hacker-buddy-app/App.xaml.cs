using System.IO;
using System.Reflection;
using System.Windows;

namespace hacker_buddy_app
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Vibes;
        internal static string GptApiKey = "";
        protected override void OnStartup(StartupEventArgs e)
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            if (e.Args.Length > 0)
            {
                Vibes = e.Args[0];
            }

            GptApiKey = File.ReadAllText("gptSecret");
            
            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the exception
            // ...

            // Show an error message to the user
            MessageBox.Show("An error has occurred and the application needs to close. Error messagee: " + e.ToString() + "\r\n" + e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Mark the exception as handled
            e.Handled = true;
        }
    }
}
