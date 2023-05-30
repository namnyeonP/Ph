using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using Microsoft.Win32;
using MiniScheduler;
using Peak.Can.Basic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections; //네임스페이스 추가

namespace Renault_BT6.Forms
{
    enum SparePatsCntHIPOTIndex
    {
        RlResultCpIndex,
        LtResultCpIndex,
    }

    public partial class FormPartsCountHipotSetting : Form
    {
        MainWindow mw;

        private int _nRealData_ResultHp;
        public int nRealData_ResultHp { get { return _nRealData_ResultHp; } set { _nRealData_ResultHp = value; } }

        private int _nLimitData_ResultHp;
        public int nLimitData_ResultHp { get { return _nLimitData_ResultHp; } set { _nLimitData_ResultHp = value; } }

        private const int nMax = 4;
        public int nEnumIndex = 0;
        int[] nLimitResult = new int[2];
        int[] nResetResult = new int[2];


        public FormPartsCountHipotSetting(MainWindow mw)
        {
            this.mw = mw;

            InitializeComponent();
            this.Text = "SparePartsCountSetting " + this.mw.position;
            ShowHideButton(0);
            LoadSparePatsCountInfo();
            ShowResetData();
            LoadLimitDataEdit();
            LoadAllDataEdit();
        }
        public FormPartsCountHipotSetting(MainWindow mw, string strStart)
        {
            this.mw = mw;
            if (strStart == "START")
            {
                LoadSparePatsCountInfo();
                LoadAllDataEdit();
            }
            else
            {
                LoadAllDataEdit();
            }
        }

        private void btnRtHipotPin_Hipot_Click(object sender, EventArgs e)
        {
            ShowHideButton(1);
            nEnumIndex = (int)SparePatsCntHIPOTIndex.RlResultCpIndex;
            SeleteButtonColor((int)SparePatsCntHIPOTIndex.RlResultCpIndex);
        }

        private void btnLtHipotPin_Hipot_Click(object sender, EventArgs e)
        {
            ShowHideButton(2);
        }

        private void btnLtApply_Hipot_Click(object sender, EventArgs e)
        {
            SaveSparePatsCountInfo("LIMIT");
        }

        private void btnRtReset_Hipot_Click(object sender, EventArgs e)
        {
            //수정한 사람 누군지 로그 남길려고 
            string strPerson;
            //그렇게 중요하지 않음...
            string strReason;
            //Reset 인덱스 구분
            int nResetIndex = nEnumIndex;
            StreamWriter writer;

            string year = System.DateTime.Now.ToString("yyyy");
            string month = System.DateTime.Now.ToString("MM");
            string day = System.DateTime.Now.ToString("dd");
            string hour = System.DateTime.Now.ToString("hh");
            string minute = System.DateTime.Now.ToString("mm");
            string second = System.DateTime.Now.ToString("ss");
            string strWriteResult = null;

            //로그 저장, 년,월 까지만 저장하면 될 듯
            strPerson = txRtPerson_Hipot.Text;
            strReason = txRtReason_Hipot.Text;

            if (strPerson == "")
            {
                System.Windows.MessageBox.Show("Please Person Name Check!");
                return;
            }

            try
            {
                string strPath = "C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\Log\\" + year + "_" + month + ".txt";
                writer = File.AppendText(strPath);         //Text File이 저장될 위치(파일명) 

                //221017 ... file name -> 2022.10 / ex)  2022.10.17 09:01 남년이가 HI-POT PIN 항목을 Reset 하였습니다.
                // 로그 저장하는 부분...
                switch (nResetIndex)
                {
                    case (int)SparePatsCntHIPOTIndex.RlResultCpIndex:
                        strWriteResult = string.Format("{0}.{1}.{2} {3}:{4} 'Name:{5}' The Hipot Pin Item has been reset(Reason:{6})"
                            , year, month, day, hour, minute, strPerson, strReason);
                        break;
                    default:
                        break;
                }

                writer.WriteLine(strWriteResult);    //저장될 string
                writer.Close();

                //버튼 색깔 유지
                DefaultButtonColor();

                //
                nResetResult[nResetIndex] = 0;
                SaveSparePatsCountInfo("RESET");
                LoadSparePatsCountInfo();
            }
            catch (Exception) { }
        }

        private void btnRtCancel_Hipot_Click(object sender, EventArgs e)
        {
            ShowHideButton(0);
            DefaultButtonColor();
        }

        private void btnPCSLogOpen_Hipot_Click(object sender, EventArgs e)
        {
            //this.mw.position
            string @FILE_PATH_PCS = @"C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\Log";
            System.Diagnostics.Process.Start("C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\Log\\", @FILE_PATH_PCS);
        }

        private void btExit_Hipot_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //검사 한번 할때 전부 카운트 하는 부분
        public void CountRealData()
        {
            nRealData_ResultHp++;

            nResetResult[0] = nRealData_ResultHp;

            SaveSparePatsCountInfo("RESET_CNT");
        }


        //reset, Limit Button disable, able
        public void ShowHideButton(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    //reset
                    lbRtPerson_Hipot.Visible = false;
                    lbRtReason_Hipot.Visible = false;
                    txRtPerson_Hipot.Visible = false;
                    txRtReason_Hipot.Visible = false;
                    btnRtReset_Hipot.Visible = false;
                    btnRtCancel_Hipot.Visible = false;
                    //limit
                    btnLtApply_Hipot.Visible = true;

                    break;
                case 1:
                    //reset
                    lbRtPerson_Hipot.Visible = true;
                    lbRtReason_Hipot.Visible = true;
                    txRtPerson_Hipot.Visible = true;
                    txRtReason_Hipot.Visible = true;
                    btnRtReset_Hipot.Visible = true;
                    btnRtCancel_Hipot.Visible = true;
                    break;
                case 2:
                    //reset
                    lbRtPerson_Hipot.Visible = false;
                    lbRtReason_Hipot.Visible = false;
                    txRtPerson_Hipot.Visible = false;
                    txRtReason_Hipot.Visible = false;
                    btnRtReset_Hipot.Visible = false;
                    btnRtCancel_Hipot.Visible = false;
                    break;
                default:
                    //reset
                    lbRtPerson_Hipot.Visible = false;
                    lbRtReason_Hipot.Visible = false;
                    txRtPerson_Hipot.Visible = false;
                    txRtReason_Hipot.Visible = false;
                    btnRtReset_Hipot.Visible = false;
                    btnRtCancel_Hipot.Visible = false;
                    //limit
                    btnLtApply_Hipot.Visible = true;
                    break;
            }
        }

        private void DefaultButtonColor()
        {
            btnRtHipotPin_Hipot.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
        }

        public void LoadSparePatsCountInfo()
        {
            FileInfo fileInfo = new FileInfo("C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\SparePatsCountInfo.csv");
            bool bIndexCheck = false;
            string strIndexCheck = "";

            if (!fileInfo.Exists)// 파일이 있는지 체크
            {
                //파일이 없을 경우 "SparePatsCountInfo.csv"
                System.Windows.MessageBox.Show("Not File 'SparePatsCountInfo.csv'");
            }

            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                using (FileStream readData = new FileStream("C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\SparePatsCountInfo.csv",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(readData, Encoding.UTF8, false))
                    {
                        string strLineValue = null;    // 한번씩 읽어올 문자열
                        string[] values = null;        // 문자열을 나눔

                        while ((strLineValue = sr.ReadLine()) != null)
                        {
                            values = strLineValue.Split(',');    // ,로 Split을해 데이터를 나눈다.

                            if (values[0] == "End" || values[0] == "") return;

                            if (values[0] == "HIPOT") strIndexCheck = values[0];

                            //엑셀이 공백일 경우 0으로 디폴트
                            if (values[values.Length - 1] == "" && values.Length == 2)
                            {
                                values[values.Length - 1] = "0";
                            }

                            if (strIndexCheck == "HIPOT")
                            {
                                if (values[0] == "Name" && values[1] == "Limit") bIndexCheck = true;

                                if (bIndexCheck == false)
                                {
                                    switch (values[0])
                                    {
                                        case "txLtHipotpin": nRealData_ResultHp = int.Parse(values[1]); break;
                                    }
                                }
                                else
                                {
                                    switch (values[0])
                                    {
                                        case "txLtHipotpin": nLimitData_ResultHp = int.Parse(values[1]); break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        public void ShowResetData()
        {
            lbRtLvConnector_Hipot.Text = string.Format("{0} / {1}", nRealData_ResultHp, nLimitData_ResultHp);
        }
        private void LoadLimitDataEdit()
        {
            txLtLvConnector_Hipot.Text = string.Format("{0} ", nLimitData_ResultHp);
        }
        private void LoadAllDataEdit()
        {
            try
            {
                nLimitResult[0] = nLimitData_ResultHp;
                nResetResult[0] = nRealData_ResultHp;
            }
            catch (Exception) { }
        }
        private void SeleteButtonColor(int nIndex)
        {
            DefaultButtonColor();

            switch (nIndex)
            {
                case (int)SparePatsCntHIPOTIndex.RlResultCpIndex:
                    btnRtHipotPin_Hipot.BackColor = System.Drawing.Color.FromArgb(215, 215, 215);
                    break;
            }
        }

        public void SaveSparePatsCountInfo(string strItem)
        {
            //this.mw.position
            List<string> ctrlItems = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool bIndexCheck = false;
            string strIndexCheck = "";

            if ("LIMIT" == strItem) if (!ResultLimitDataEdit()) return;

            try
            {
                string strLineValue = null;    // 한번씩 읽어올 문자열
                string[] values = new string[nMax + 2];

                string strLineValueTemp = null;    // 한번씩 읽어올 문자열

                using (FileStream readData = new FileStream("C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\SparePatsCountInfo.csv",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(readData, Encoding.UTF8))
                    {
                        int nIndexReal = 0, nIndexLimit = 0;
                        while ((strLineValue = sr.ReadLine()) != null)
                        {
                            values = strLineValue.Split(',');    // ,로 Split을해 데이터를 나눈다.

                            //
                            if (values[0] == "End" || values[0] == "") break;

                            //HIPOT일 경우
                            if (values[0] == "HIPOT") strIndexCheck = values[0];
                            if (values[0] == "DCIR") strIndexCheck = values[0];

                            if (strIndexCheck == "HIPOT")
                            {
                                if (values[0] == "HIPOT")
                                {
                                    strLineValueTemp += strLineValue + "\n";
                                    continue;
                                }

                                if (values[0] == "Name" && values[1] == "Limit") bIndexCheck = true;

                                if (bIndexCheck == false)
                                {
                                    if (values[0] == "Name") { strLineValueTemp += strLineValue + "\n"; }
                                    else { strLineValueTemp += values[0] + "," + nResetResult[nIndexReal++] + "\n"; }

                                }
                                else
                                {
                                    if (values[0] == "Name") { strLineValueTemp += strLineValue + "\n"; }
                                    else { strLineValueTemp += values[0] + "," + nLimitResult[nIndexLimit++] + "\n"; }

                                }
                            }
                            else
                            {
                                strLineValueTemp += strLineValue + "\n";
                            }
                        }

                        //저장된 제어항목을 파일에 저장
                        using (StreamWriter sw = new StreamWriter("C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\SparePatsCountInfo.csv", false))
                        {
                            sw.WriteLine(strLineValueTemp);
                            sw.Write(sb.ToString());
                        }
                    }
                }
            }
            catch (Exception) { }

            //
            if ("RESET_CNT" != strItem)
            {
                LoadSparePatsCountInfo();
                ShowResetData();
                this.mw.SetPartsCountData();
            }
        }
        private bool ResultLimitDataEdit()
        {
            try
            {
                nLimitData_ResultHp = nLimitResult[0] = int.Parse(txLtLvConnector_Hipot.Text);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
