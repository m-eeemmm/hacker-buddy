using System.Windows;
namespace hacker_buddy_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Vibes = App.Vibes;
            this.DataContext = this;
        }


        public string Vibes { get; set; }
    }
}
