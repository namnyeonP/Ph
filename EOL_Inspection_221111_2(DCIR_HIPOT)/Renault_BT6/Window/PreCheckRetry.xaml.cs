using System;
using System.Collections.Generic;
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

namespace Renault_BT6.윈도우
{
    /// <summary>
    /// PreCheckRetry.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PreCheckRetry : Window
    {
        private List<BitmapImage> mListImgSource = null;

        private MainWindow mMainHndler = null;

        public PreCheckRetry(MainWindow mw)
        {
            InitializeComponent();

            InitMember(mw);
        }

        private void InitMember(MainWindow mw)
        {
            mMainHndler = mw;

            mListImgSource = new List<BitmapImage>();

            try
            {
                this.AddImgSource(System.Windows.Forms.Application.StartupPath + "\\_Images\\PreCheck_1.PNG");

                this.AddImgSource(System.Windows.Forms.Application.StartupPath + "\\_Images\\PreCheck_2.PNG");
            }
            catch (Exception ec)
            {
                mListImgSource.Clear();

                mMainHndler.LogState(LogType.Fail, ec./*ToString()*/Message);
            }
        }

        private void AddImgSource(string srcPath)
        {
            if(mListImgSource != null)
            {
                BitmapImage srcImg = new BitmapImage(new Uri(srcPath, UriKind.RelativeOrAbsolute));

                mListImgSource.Add(srcImg);
            }
        }

        public void SetImg(int nSrcIndex) // 테스트 이후 인덱스 디파인 해놓을것
        {
            if(mListImgSource != null && (nSrcIndex >= 0 && nSrcIndex < mListImgSource.Count))
            {
                this.guideImg.Source = mListImgSource[nSrcIndex];
            }

        }
    }
}
