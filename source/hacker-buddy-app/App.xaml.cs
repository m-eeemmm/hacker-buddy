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
            if (e.Args.Length > 0)
            {
                Vibes = e.Args[0];
            }

            string path = Path.Combine(Assembly.GetExecutingAssembly().Location, "gptSecret");

            GptApiKey = File.ReadAllText(path);


            base.OnStartup(e);
        }
    }
}
