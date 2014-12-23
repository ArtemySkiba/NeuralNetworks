﻿using System;
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

        #region Ctor

        public MainWindowVM()
        {
            neurals = new ObservableCollection<Neural>();
        }

        #endregion

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
                //if (n != value)
                {
                    n = value;
                    OnPropertyChanged("FillCommandEnable");
                    updateGrid();
                }
            }
        }

        public static int T { get; set; }

        public static bool NeedRandom { get; set; }

        private void updateGrid()
        {
            Random r = new Random();
            neurals.Clear();
            for (int i = 0; i < N; i++)
            {
                if (NeedRandom)
                {
                    neurals.Add(new Neural { W = r.Next(1, T + 1) });
                }
                else
                {
                    neurals.Add(new Neural { Number = i + 1 });
                }
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

        public string Result { get; set; }

        #endregion

        #region Commands

        private ICommand fillCommand;
        public ICommand FillCommand
        {
            get
            {
                return fillCommand = fillCommand ?? new Command(getSamples);
            }
        }

        #endregion

        #region Method

        private void getSamples()
        {
            Result = string.Empty;
            List<Neural> firstMGroup = neurals.Where(ne => ne.W == T).ToList();
            List<Neural> secondMGroup = neurals.Where(ne => ne.W > (double)T / 2 && ne.W < T).ToList();
            List<Neural> thirdMGroup = neurals.Where(ne => ne.W == (double)T / 2).ToList();
            List<Neural> fourthMGroup = neurals.Where(ne => ne.W < (double)T / 2).ToList();

            List<Neural> firstMGroupStar = neurals.Where(ne => ne.W == T - 1).ToList();
            List<Neural> secondMGroupStar = neurals.Where(ne => ne.W > (double)(T - 1) / 2 && ne.W < T - 1).ToList();
            List<Neural> thirdMGroupStar = neurals.Where(ne => ne.W == (double)(T - 1) / 2).ToList();
            List<Neural> fourthMGroupStar = neurals.Where(ne => ne.W < (double)(T - 1) / 2).ToList();

            List<List<Neural>> result = new List<List<Neural>>();
            bool usedAllFourth = false;

            //1 group
            firstMGroup.ForEach(m => result.Add(new List<Neural> { m }));

            //2 group
            int fourthCounter = 0;
            if (fourthMGroup.Count == 0)
            {
                secondMGroup.ForEach(m => result.Add(new List<Neural> { m }));
            }
            else
            {
                foreach (var m in secondMGroup)
                {
                    var temp = new List<Neural> { m };

                    if (fourthCounter < fourthMGroup.Count)
                    {
                        temp.Add(fourthMGroup[fourthCounter++]);
                    }
                    else
                    {
                        usedAllFourth = true;
                        fourthCounter = 0;
                        temp.Add(fourthMGroup[fourthCounter++]);
                    }
                    result.Add(temp);
                }
            }

            //3 group
            if (thirdMGroup.Count % 2 == 0)
            {
                for (int i = 0; i < thirdMGroup.Count; i += 2)
                {
                    result.Add(new List<Neural> { thirdMGroup[i], thirdMGroup[i + 1] });
                }
            }
            else
            {
                for (int i = 0; i < thirdMGroup.Count - 1; i += 2)
                {
                    result.Add(new List<Neural> { thirdMGroup[i], thirdMGroup[i + 1] });
                }
                var temp = new List<Neural> { thirdMGroup[thirdMGroup.Count - 1] };
                if (!usedAllFourth && fourthCounter < fourthMGroup.Count)
                {
                    temp.Add(fourthMGroup[fourthCounter]);
                }
                else
                {
                    if (fourthMGroup.Any())
                    {
                        temp.Add(fourthMGroup.FirstOrDefault(f => f.W + thirdMGroup[thirdMGroup.Count-1].W == T));
                    }
                }
                result.Add(temp);
            }

            //4 group
            if (!usedAllFourth && fourthCounter < fourthMGroup.Count)
            {
                for (int i = fourthCounter; i < fourthMGroup.Count; i++)
                {
                    int res = fourthMGroup[i].W;
                    if (res == T)
                    {
                        result.Add(new List<Neural> { fourthMGroup[i] });
                        break;
                    }
                    else
                    {
                        if (++fourthCounter == fourthMGroup.Count)
                        {
                            fourthCounter = 0;
                        }
                        res += fourthMGroup[fourthCounter].W;
                        if (res == T)
                        {
                            result.Add(new List<Neural> {fourthMGroup[i], fourthMGroup[fourthCounter]});
                            break;
                        }
                        else
                        {
                            List<Neural> tempNeurals = new List<Neural>();
                            foreach (var resN in result)
                            {
                                tempNeurals.AddRange(resN);
                            }
                            foreach (var tempNeural in tempNeurals)
                            {
                                int a = res + tempNeural.W;
                                if (a == T)
                                {
                                    result.Add(new List<Neural> { fourthMGroup[i], fourthMGroup[fourthCounter], tempNeural });
                                    break;
                                }
                            }
                        }
                    }
                }
            }





            //STAR
            List<List<Neural>> result2 = new List<List<Neural>>();
            usedAllFourth = false;

            //1 group
            firstMGroupStar.ForEach(m => result2.Add(new List<Neural> { m }));

            //2 group
            fourthCounter = 0;
            if (fourthMGroupStar.Count == 0)
            {
                secondMGroupStar.ForEach(m => result2.Add(new List<Neural> { m }));
            }
            else
            {
                foreach (var m in secondMGroupStar)
                {
                    var temp = new List<Neural> { m };

                    if (fourthCounter < fourthMGroupStar.Count)
                    {
                        temp.Add(fourthMGroupStar[fourthCounter++]);
                    }
                    else
                    {
                        fourthCounter = 0;
                        temp.Add(fourthMGroupStar[fourthCounter++]);
                        usedAllFourth = true;
                    }
                    result2.Add(temp);
                }
            }

            //3 group
            if (thirdMGroupStar.Count % 2 == 0)
            {
                for (int i = 0; i < thirdMGroupStar.Count; i += 2)
                {
                    result2.Add(new List<Neural> { thirdMGroupStar[i], thirdMGroupStar[i + 1] });
                }
            }
            else
            {
                for (int i = 0; i < thirdMGroupStar.Count - 1; i += 2)
                {
                    result2.Add(new List<Neural> { thirdMGroupStar[i], thirdMGroupStar[i + 1] });
                }
                var temp = new List<Neural> { thirdMGroupStar[thirdMGroupStar.Count - 1] };
                if (!usedAllFourth && fourthCounter < fourthMGroupStar.Count)
                {
                    temp.Add(fourthMGroupStar[fourthCounter]);
                }
                else
                {
                    if (fourthMGroupStar.Any())
                    {
                        temp.Add(fourthMGroupStar.FirstOrDefault(f => f.W + thirdMGroupStar[thirdMGroupStar.Count - 1].W == T));
                    }
                }
                result2.Add(temp);
            }

            //4 group
            if (!usedAllFourth && fourthCounter < fourthMGroupStar.Count)
            {
                for (int i = fourthCounter; i < fourthMGroupStar.Count; i++)
                {
                    int res = fourthMGroupStar[i].W;
                    if (res == T)
                    {
                        result.Add(new List<Neural> { fourthMGroupStar[i] });
                        break;
                    }
                    else
                    {
                        if (++fourthCounter == fourthMGroupStar.Count)
                        {
                            fourthCounter = 0;
                        }
                        res += fourthMGroupStar[fourthCounter].W;
                        if (res == T)
                        {
                            result.Add(new List<Neural> { fourthMGroupStar[i], fourthMGroupStar[fourthCounter] });
                            break;
                        }
                        else
                        {
                            List<Neural> tempNeurals = new List<Neural>();
                            foreach (var resN in result2)
                            {
                                tempNeurals.AddRange(resN);
                            }
                            foreach (var tempNeural in tempNeurals)
                            {
                                int a = res + tempNeural.W;
                                if (a == T)
                                {
                                    result.Add(new List<Neural> { fourthMGroupStar[i], fourthMGroupStar[fourthCounter], tempNeural });
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //checkForTable(result, result2);

            //checkForCapacity(secondMGroup, fourthMGroup, firstMGroup.Count, thirdMGroup.Count, result);
            //checkForCapacity2(secondMGroupStar, fourthMGroupStar, firstMGroupStar.Count, thirdMGroupStar.Count, result2);

            //string s = string.Empty;
            //foreach (var res in result)
            //{
            //    int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
            //    s += (temp == T) + Environment.NewLine;
            //}
            ////MessageBox.Show(s);
            //Result += s + Environment.NewLine;

            //s = string.Empty;
            //foreach (var res in result2)
            //{
            //    int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
            //    s += (temp == T - 1) + Environment.NewLine;
            //}
            ////MessageBox.Show(s);
            //Result += s + Environment.NewLine;

            foreach (var res in result)
            {
                foreach (var r in res)
                {
                    Result += "W"+r.Number + " ";
                }
                Result += Environment.NewLine;
            }

            foreach (var res in result2)
            {
                foreach (var r in res)
                {
                    Result += "W" + r.Number + " ";
                }
                Result += Environment.NewLine;
            }

            OnPropertyChanged("Result");
        }

        private void checkForCapacity(List<Neural> secondMGroup, List<Neural> fourthMGroup, int k, int q, List<List<Neural>> result)
        {
            int sum = 0;
            foreach (var second in secondMGroup)
            {
                sum += second.W;
            }
            sum = secondMGroup.Count * T - sum;
            bool check = true;
            foreach (var fourth in fourthMGroup)
            {
                if (!(fourth.W <= sum))
                {
                    check = false;
                }
            }

            if (check)
            {
                Result += "Рассчётное количество 1 = " + (k + secondMGroup.Count + Math.Ceiling((double)q / 2)) + Environment.NewLine;
            }
            else
            {
                int temp = 0;
                foreach (var r in result)
                {
                    temp += r.Sum(rr => rr.W);
                }
                Result += "Рассчётное количество 1 = " + (Math.Ceiling((double)temp / T)) + Environment.NewLine;
            }
        }

        private void checkForCapacity2(List<Neural> secondMGroup, List<Neural> fourthMGroup, int k, int q, List<List<Neural>> result)
        {
            int sum = 0;
            foreach (var second in secondMGroup)
            {
                sum += second.W;
            }
            sum = (secondMGroup.Count * T - 1) - sum;
            bool check = true;
            foreach (var fourth in fourthMGroup)
            {
                if (!(fourth.W <= sum))
                {
                    check = false;
                }
            }

            if (check)
            {
                Result += "Рассчётное количество 2 = " + (k + secondMGroup.Count + Math.Ceiling((double)q / 2)) + Environment.NewLine;
            }
            else
            {
                int temp = 0;
                foreach (var r in result)
                {
                    temp += r.Sum(rr => rr.W);
                }
                Result += "Рассчётное количество 2 = " + (Math.Ceiling((double)temp / (T - 1))) + Environment.NewLine;
            }
        }

        private void checkForTable(List<List<Neural>> result1, List<List<Neural>> result2)
        {
            foreach (var res in result1)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                //MessageBox.Show("1 type of error = " + (temp >= T));
                Result += "1 type of error = " + (temp >= T) + Environment.NewLine;
                if (temp >= T)
                {
                    break;
                }
            }

            bool needBrake = false;
            foreach (var res in result1)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                foreach (var r in res)
                {
                    if (temp <= T + r.W && temp >= T)
                    {
                        //MessageBox.Show("2 type of error = " + (temp <= T + r.W && temp >= T));
                        Result += "2 type of error = " + (temp <= T + r.W && temp >= T) + Environment.NewLine;
                        needBrake = true;
                        break;
                    }
                }
                if (needBrake)
                {
                    break;
                }
            }

            foreach (var res in result1)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                if (temp == T)
                {
                    //MessageBox.Show("3 type of error = " + (temp == T));
                    Result += "3 type of error = " + (temp == T) + Environment.NewLine;
                    break;
                }
            }

            foreach (var res in result1)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                if (temp == T)
                {
                    // MessageBox.Show("4 type of error = " + (temp == T));
                    Result += "4 type of error = " + (temp == T) + Environment.NewLine;
                    break;
                }
            }



            foreach (var res in result2)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                if (temp < T)
                {
                    //  MessageBox.Show("5 type of error = " + (temp < T));
                    Result += "5 type of error = " + (temp < T) + Environment.NewLine;
                    break;
                }
            }


            needBrake = false;
            foreach (var res in result2)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                foreach (var r in res)
                {
                    if (temp <= T - 1 && temp >= T - r.W)
                    {
                        //           MessageBox.Show("6 type of error = " + (temp <= T - 1 && temp >= T - r.W));
                        Result += "6 type of error = " + (temp <= T - 1 && temp >= T - r.W) + Environment.NewLine;
                        needBrake = true;
                        break;
                    }
                }
                if (needBrake)
                {
                    break;
                }
            }

            foreach (var res in result2)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                if (temp < T)
                {
                    //      MessageBox.Show("7 type of error = " + (temp < T));
                    Result += "7 type of error = " + (temp < T) + Environment.NewLine;
                    break;
                }
            }

            foreach (var res in result2)
            {
                int temp = res.Sum(r => r.W * Convert.ToInt32(r.X));
                if (temp < T)
                {
                    //    MessageBox.Show("8 type of error = " + (temp < T));
                    Result += "8 type of error = " + (temp < T) + Environment.NewLine;
                    break;
                }
            }

            Result += "Количество наборов 1 = " + result1.Count + Environment.NewLine;
            Result += "Количество наборов 2 = " + result2.Count + Environment.NewLine;

        }

        #endregion

    }
}
