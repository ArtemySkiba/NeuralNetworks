namespace NeuralNetworks
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    internal partial class MainWindowView
    {

        public MainWindowView(MainWindowVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

    }
}
