using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// SelfDiagnosisWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelfDiagnosisWindow : Window
    {
        TestItem testitem;
        MainWindow mw;

        public static string guideStr1 = "하단 버튼의 이미지를 순서대로 점검하세요.";
        public static string guideStr2 = "Check the images on the bottom button in order.";
        public static string guideStr3 = "Sprawdź kolejno obrazy na dolnym przycisku.";

        public bool isContinue = false;
        public SelfDiagnosisWindow(MainWindow mw, TestItem ti)
        {
            InitializeComponent();

            this.mw = mw;
            this.testitem = ti;

            this.Loaded += SelfDiagnosisWindow_Loaded;
            this.Closed += SelfDiagnosisWindow_Closed;
        }

        void SelfDiagnosisWindow_Closed(object sender, EventArgs e)
        {
            isContinue = false;
            this.Hide();
        }

        void SelfDiagnosisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mw.LogState(LogType.Info, "SelfDiagnosisWindow_Loaded Start");

            this.mainTab.Items.Clear();
            this.testitemTb.Text = testitem.btname.ToString() + " NG" + "\n"
                + guideStr1 + "\n"
                + guideStr2 + "\n"
                + guideStr3 + "\n";

            this.valueTb.Text = testitem.Value_.ToString();
            this.maxTb.Text = testitem.Max.ToString();
            this.minTb.Text = testitem.Min.ToString();


            DirectoryInfo di = new DirectoryInfo(testitem.selfDiagDirectory);
            if (!di.Exists)
            {
                MessageBox.Show("Folder is EMPTY!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //di.Create();
            }

            mw.LogState(LogType.Info, "Image Copy Start");

            foreach (var folder in di.GetDirectories())
            {
                var innerDi = new DirectoryInfo(folder.FullName);

                TabItem ti = new TabItem();
                ti.Header = innerDi.Name;
                var image = new Image();
                if (innerDi.GetFiles().Length >0)
                {
                    image.Source = new BitmapImage(new Uri(innerDi.GetFiles()[0].FullName));
                    ti.Content = image;
                }

                this.mainTab.Items.Add(ti);

            }

            mw.LogState(LogType.Info, "Image Copy End");


            //foreach (var test in testitem.selfDiagList)
            //{
            //    TabItem ti = new TabItem();
            //    ti.Header = test.Header;
            //    var image = new Image();
            //    image.Source = new BitmapImage(new Uri(test.FileAddress));
            //    ti.Content = image;

            //    this.mainTab.Items.Add(ti);
            //}

            mw.LogState(LogType.Info, "SelfDiagnosisWindow_Loaded End");
        }

        private void NextBtClicked(object sender, RoutedEventArgs e)
        {
            isContinue = true;
            this.Hide();
        }

        private void EndBtClicked(object sender, RoutedEventArgs e)
        {
            isContinue = false;
            this.Hide();
        }
    }
}
