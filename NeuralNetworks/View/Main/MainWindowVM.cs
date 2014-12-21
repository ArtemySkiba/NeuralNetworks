using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public static int T { get; set; }

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
                    getFirstGroupSamples();

                });
            }
        }

        private void getFirstGroupSamples()
        {
            List<Neural> firstMGroup = neurals.Where(ne => ne.W == T).ToList();
            List<Neural> secondMGroup = neurals.Where(ne => ne.W > T / 2 && ne.W < T).ToList();
            List<Neural> thirdMGroup = neurals.Where(ne => ne.W == T / 2).ToList();
            List<Neural> fourthMGroup = neurals.Where(ne => ne.W < T / 2).ToList();

            //MessageBox.Show(string.Format("{0} {1} {2} {3}", firstMGroup.Count, secondMGroup.Count, thirdMGroup.Count,
            //    fourthMGroup.Count));

            List<List<Neural>> result = new List<List<Neural>>();
            bool usedAllFourth = false;
            firstMGroup.ForEach(m => result.Add(new List<Neural> { m }));

            int fourthCount = fourthMGroup.Count;
            if (fourthCount == 0)
            {
                secondMGroup.ForEach(m => result.Add(new List<Neural> { m }));
            }
            else
            {
                foreach (var m in secondMGroup)
                {
                    fourthCount = 0;
                    var temp = new List<Neural> { m, fourthMGroup[fourthCount++] };
                    if (fourthCount == fourthMGroup.Count)
                    {
                        usedAllFourth = true;
                        fourthCount = 0;
                    }
                    result.Add(temp);
                }
            }

            if (thirdMGroup.Count % 2 == 0)
            {
                for (int i = 0; i < thirdMGroup.Count - 1; i++)
                {
                    result.Add(new List<Neural> { thirdMGroup[i], thirdMGroup[i + 1] });
                }
            }
            else
            {
                for (int i = 0; i < thirdMGroup.Count - 2; i++)
                {
                    result.Add(new List<Neural> { thirdMGroup[i], thirdMGroup[i + 1] });
                }
                var temp = new List<Neural> {thirdMGroup[thirdMGroup.Count - 1]};
                if (fourthMGroup.Count > secondMGroup.Count)
                {
                    temp.Add(fourthMGroup[secondMGroup.Count]);
                }
                result.Add(temp);
            }

            if (!usedAllFourth)
            {
                for (int i = fourthCount; i < fourthMGroup.Count; i++)
                {
                    result.Add(new List<Neural>{fourthMGroup[i]});
                }
            }

            foreach (var res in result)
            {
                int temp = 0;
                foreach (var r in res)
                {
                    temp += r.W*Convert.ToInt32(r.X);
                }
                MessageBox.Show((temp == T).ToString());
            }

        }

        #endregion

    }
}
