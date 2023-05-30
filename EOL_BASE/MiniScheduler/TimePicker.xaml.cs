using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MiniScheduler
{
    /// <summary>
    /// TimePicker.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimePicker : UserControl, INotifyPropertyChanged
    {
        #region Private Member variable

        private bool _adHours;
        private bool _addMinutes;
        private ObservableCollection<string> _amPmTypes
    = new ObservableCollection<string>();
        private string _displayAmPm;

        int day;
        int hour;
        int minute;
        int second;

        #endregion

        #region Constructors

        public TimePicker()
        {
            InitializeComponent();
            day = hour = minute = second = 0;

            this.DataContext = this;
        }

        #endregion

        #region Public Properties

        public string DisplayDays
        {
            get
            {
                return day.ToString("00");
                //return hours.ToString();
            }
            set
            {
                if (int.Parse(value) < 0)
                {
                    value = "0";
                }
                Int32.TryParse(value, out day);

                OnPropertyChanged("DisplayDays");
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
                OnPropertyChanged("DisplayTimeSeconds");
            }
        }

        public string DisplayTimeHours
        {
            get
            {
                return hour.ToString("00");
                //return hours.ToString();
            }
            set
            {
                if (value == "24")
                {
                    DisplayDays = (int.Parse(DisplayDays) + 1).ToString();
                    value = "0";
                }

                if (int.Parse(value) < 0)
                {
                    value = "0";
                }

                Int32.TryParse(value, out hour);

                OnPropertyChanged("DisplayDays");
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
                OnPropertyChanged("DisplayTimeSeconds");
            }
        }

        public string DisplayTimeMinutes
        {
            get { return minute.ToString("00"); }
            set
            {
                if (value == "60")
                {
                    DisplayTimeHours = (int.Parse(DisplayTimeHours) + 1).ToString();
                    value = "0";
                }

                if (int.Parse(value) < 0)
                {
                    value = "0";
                }

                Int32.TryParse(value, out minute);

                OnPropertyChanged("DisplayDays");
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
                OnPropertyChanged("DisplayTimeSeconds");
            }
        }

        public string DisplayTimeSeconds
        {
            get { return second.ToString("00"); }
            set
            {
                if (value == "60")
                {
                    DisplayTimeMinutes = (int.Parse(DisplayTimeMinutes) + 1).ToString();
                    value = "0";
                }

                if (int.Parse(value) < 0)
                {
                    value = "0";
                }


                Int32.TryParse(value, out second);


                OnPropertyChanged("DisplayDays");
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
                OnPropertyChanged("DisplayTimeSeconds");
            }
        }


        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SelectedTimeProperty
    = DependencyProperty.Register(
          "SelectedTime", typeof(string), typeof(TimePicker),
    new PropertyMetadata(default(string)));

        public string SelectedTime
        {
            get
            {
                return ((DateTime)GetValue(SelectedTimeProperty))
                    .ToLocalTime().ToString("t");
            }
            set { SetValue(SelectedTimeProperty, value); }
        }

        #endregion

        #region Methods

        private void MinutesUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            var m = int.Parse(DisplayTimeMinutes);
            DisplayTimeMinutes = (m + 1).ToString();
        }

        private void MinutesDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            var m = int.Parse(DisplayTimeMinutes);
            DisplayTimeMinutes = (m - 1).ToString();
        }

        private void HourUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            var m = int.Parse(DisplayTimeHours);
            DisplayTimeHours = (m + 1).ToString();
        }

        private void HourDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            var m = int.Parse(DisplayTimeHours);
            DisplayTimeHours = (m - 1).ToString();
        }

        private void DayUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            var m = int.Parse(DisplayDays);
            DisplayDays = (m + 1).ToString();
        }

        private void DayDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            var m = int.Parse(DisplayDays);
            DisplayDays = (m - 1).ToString();
        }


        private void SecondsUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (selectedText)
            {
                case 0:
                    {
                        var m = int.Parse(DisplayDays);
                        DisplayDays = (m + 1).ToString();
                    } break;
                case 1:
                    {
                        var m = int.Parse(DisplayTimeHours);
                        DisplayTimeHours = (m + 1).ToString();
                    } break;
                case 2:
                    {
                        var m = int.Parse(DisplayTimeMinutes);
                        DisplayTimeMinutes = (m + 1).ToString();
                    } break;
                case 3:
                    {
                        var m = int.Parse(DisplayTimeSeconds);
                        DisplayTimeSeconds = (m + 1).ToString();
                    } break;
            }
        }

        private void SecondsDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (selectedText)
            {
                case 0:
                    {
                        var m = int.Parse(DisplayDays);
                        DisplayDays = (m - 1).ToString();
                    } break;
                case 1:
                    {
                        var m = int.Parse(DisplayTimeHours);
                        DisplayTimeHours = (m - 1).ToString();
                    } break;
                case 2:
                    {
                        var m = int.Parse(DisplayTimeMinutes);
                        DisplayTimeMinutes = (m - 1).ToString();
                    } break;
                case 3:
                    {
                        var m = int.Parse(DisplayTimeSeconds);
                        DisplayTimeSeconds = (m - 1).ToString();
                    } break;
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


        /// <summary>
        /// 0 = day / 1 = hour / 2 = minute / 3 = second
        /// </summary>
        int selectedText = 3;
        private void AddHoursTextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedText = 1;
        }

        private void AddDaysTextBox_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedText = 0;
        }

        private void AddMinutesTextBox_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedText = 2;
        }

        private void AddSecondsTextBox_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedText = 3;
        }

        private void AddDaysTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {//날짜는 세자리까지 받도록
            if ((sender as TextBox).Text == "")
                return;

            int p = 0;
            if (!int.TryParse((sender as TextBox).Text, out p))
            {
                (sender as TextBox).Text = "00";
            }


            if ((sender as TextBox).Text.Length > 3)
            {
                (sender as TextBox).Text = "999";
            }

            if (int.Parse((sender as TextBox).Text) > 999)
            {
                (sender as TextBox).Text = "999";
            }

            day = int.Parse((sender as TextBox).Text);
        }

        private void Common_TextChanged(object sender, TextChangedEventArgs e)
        {//초
            if ((sender as TextBox).Text == "")
                return;

            int p = 0;
            if (!int.TryParse((sender as TextBox).Text, out p))
            {
                (sender as TextBox).Text = "00";
            }


            if ((sender as TextBox).Text.Length > 2)
            {
                (sender as TextBox).Text = "59";
            }

            if (int.Parse((sender as TextBox).Text) > 59)
            {
                (sender as TextBox).Text = "59";
            }

            second = int.Parse((sender as TextBox).Text);
        }

        private void hour_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
                return;

            int p = 0;
            if (!int.TryParse((sender as TextBox).Text, out p))
            {
                (sender as TextBox).Text = "00";
            }


            if ((sender as TextBox).Text.Length > 2)
            {
                (sender as TextBox).Text = "23";
            }

            if (int.Parse((sender as TextBox).Text) > 23)
            {
                (sender as TextBox).Text = "23";
            }

            hour = int.Parse((sender as TextBox).Text);
        }

        private void minutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
                return;

            int p = 0;
            if (!int.TryParse((sender as TextBox).Text, out p))
            {
                (sender as TextBox).Text = "00";
            }


            if ((sender as TextBox).Text.Length > 2)
            {
                (sender as TextBox).Text = "59";
            }

            if (int.Parse((sender as TextBox).Text) > 59)
            {
                (sender as TextBox).Text = "59";
            }

            minute = int.Parse((sender as TextBox).Text);
        }


    }

}