using System;
using System.Collections.Generic;
using System.Data;
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
using CSVLibraryAK;
using CSVLibraryAK.Resources.Constants;
using EOL_BASE.클래스;

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// CSVEditWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CSVEditWindow : Window
    {
        string strPath = "";
        MainWindow mw;
        public CSVEditWindow(MainWindow mw, string path)
        {
            InitializeComponent();

            this.strPath = path;

            this.Loaded += CSVEditWindow_Loaded;

            insertBCRBt.Visibility = Visibility.Collapsed;

            this.grdLoad.SelectedCellsChanged += onlyEOLList_SelectedCellsChanged;

            if(path.Contains("eollist.csv"))
            {
                this.Width = System.Windows.SystemParameters.PrimaryScreenWidth - 200;
            }
            this.mw = mw;
        }

        /// <summary>
        /// master BCR
        /// </summary>
        /// <param name="mw"></param>
        /// <param name="path"></param>
        /// <param name="desc1"></param>
        /// <param name="desc2"></param>
        public CSVEditWindow(MainWindow mw, string path,string desc1,string desc2)
        {
            InitializeComponent();

            this.strPath = path;

            this.Loaded += CSVEditWindow_Loaded;

            row1.Height = new GridLength(30);
            this.desc1.Content = desc1.ToString();
            row2.Height = new GridLength(30);
            this.desc2.Content = desc2.ToString();

            insertBCRBt.Visibility = Visibility.Visible;
            this.grdLoad.SelectedCellsChanged += GrdLoad_SelectedCellsChanged;
            this.mw = mw;
        }


        private void onlyEOLList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (grdLoad.CurrentColumn.DisplayIndex == 15)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.SelectedPath = @"C:\EOL_SELF_DIAG\";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    datatable.Rows[grdLoad.SelectedIndex][grdLoad.CurrentColumn.DisplayIndex] = dialog.SelectedPath;
                }
            }
        }

        private void GrdLoad_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (grdLoad.SelectedCells.Count == 0)
                return;

            if (this.desc1.Content.ToString() == "")
                return;

            var item1Arr = (grdLoad.SelectedCells[0].Item as DataRowView).Row.ItemArray;

            modelType = item1Arr[0].ToString();
            mode = item1Arr[1].ToString();
            prodId = item1Arr[2].ToString();
            bcr = item1Arr[3].ToString();
            insertBCRBt.Content = "Insert [" + bcr.ToString() + "]";
        }

        string modelType = "";
        string mode = "";
        string prodId = "";
        string bcr = "";


        /// <summary>
        /// Data table property.
        /// </summary>
        private DataTable dataTableObj = new DataTable();
        DataTable datatable = new DataTable();


        private void CSVEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pathLb.Content = strPath;
            // Import CSV file.
            datatable = CSVLibraryAK.CSVLibraryAK.Import(strPath, true);

            // Verification.
            if (datatable.Rows.Count <= 0)
            {
                // Message.
                MessageBox.Show("Your file is either corrupt or does not contain any data. Make sure that you are using valid CSV file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Info.
                return;
            }

            // Load Csv to datagrid.
            this.grdLoad.ItemsSource = datatable.DefaultView;

            // Settings.
            this.dataTableObj = datatable;
        }
    

        private void GrdInfo_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                // Setting.
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.Write(ex);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verification.
                if (this.dataTableObj.Rows.Count <= 0)
                {
                    // Message.
                    MessageBox.Show("There is no data available to export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Info.
                    return;
                }

                if (this.strPath.Contains("Master_BCR"))
                {
                    int nCount = 0;
                    for (int i = 3; i < dataTableObj.Rows.Count; i++)
                    {
                        for (int j = (i + 1); j < (dataTableObj.Rows.Count); j++)
                        {
                            if (dataTableObj.Rows[i][3].ToString() == dataTableObj.Rows[j][3].ToString())
                            {
                                // Message.
                                MessageBox.Show("Please Remove Duplicate Barcode.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                // Info.
                                return;
                            }
                        }
                    }
                }

                // Export to CSV file.
                CSVLibraryAK.CSVLibraryAK.Export(strPath, this.dataTableObj);

                MessageBox.Show("Success to save file.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            if (bcr == "")
                return;

            try
            {
                // Verification.
                if (this.dataTableObj.Rows.Count <= 0)
                {
                    // Message.
                    MessageBox.Show("There is no data available to export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Info.
                    return;
                }

                mw.Barcode = mw.lotTb.Text = mw.viewModel.LotId = bcr;

                mw.prodTb.Text = mw.viewModel.ProdId = prodId;

                if (mw.mBCRs.Exists(x => x.BCR == bcr))
                {
                    (mw.mBCRs.First(x => x.BCR == bcr) as CustomBCR).ProdID = prodId;
                    (mw.mBCRs.First(x => x.BCR == bcr) as CustomBCR).ModelType = int.Parse(modelType);
                    (mw.mBCRs.First(x => x.BCR == bcr) as CustomBCR).Mode = int.Parse(mode);
                }
                else
                {
                    var cbcr = new CustomBCR();
                    cbcr.BCR = bcr;
                    cbcr.ProdID = prodId;
                    cbcr.ModelType = int.Parse(modelType);
                    cbcr.Mode = int.Parse(mode);
                    mw.mBCRs.Add(cbcr);
                }

                //var dd = this.dataTableObj.Select();
                //// Export to CSV file.
                //CSVLibraryAK.CSVLibraryAK.Export(strPath, this.dataTableObj);

                //MessageBox.Show("Success to save file.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Info.
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
