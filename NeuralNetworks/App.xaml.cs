using System.Windows;

namespace NeuralNetworks
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindowVM vm = new MainWindowVM();
            MainWindowView view = new MainWindowView(vm);
            view.Show();
        }
    }
}
