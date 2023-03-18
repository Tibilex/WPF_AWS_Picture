using System.Windows;
using AWS_Picture.ViewModels;

namespace AWS_Picture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AmazonClientViewModel _amazonClientViewModel = new AmazonClientViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _amazonClientViewModel;
        }
    }
}
