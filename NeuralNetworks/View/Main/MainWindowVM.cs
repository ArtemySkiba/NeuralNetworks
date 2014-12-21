using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NeuralNetworks.MVVM;

namespace NeuralNetworks
{
    class MainWindowVM : NotifyPropertyChanged
    {

        public MainWindowVM()
        {
            neurals = new ObservableCollection<Neural>();
        }

        #region Properties

        public Visibility FillCommandEnable
        {
            get { return n > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        private int n;
        public int N
        {
            get { return n; }
            set
            {
                if (n != value)
                {
                    n = value;
                    OnPropertyChanged("FillCommandEnable");
                    updateGrid();
                }
            }
        }

        private void updateGrid()
        {
            neurals.Clear();
            for (int i = 0; i < N; i++)
            {
                neurals.Add(new Neural());
            }
            OnPropertyChanged("Neurals");
        }

        private ObservableCollection<Neural> neurals;
        public ObservableCollection<Neural> Neurals
        {
            get
            {
                return neurals;
            }
        }

        #endregion

        #region Commands

        private ICommand fillCommand;
        public ICommand FillCommand
        {
            get
            {
                return fillCommand = fillCommand ?? new Command(() =>
                {
                    int a = 0;
                    var b = neurals;
                });
            }
        }

        #endregion

    }
}
