﻿using System;
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
    enum SparePatsCntIndex
    {
        RlResultLcIndex,
        RlResultCpIndex,
        RlResultMc1Index,
        RlResultMc2Index,
        RlResultMc3Index,
        LtResultLcIndex,
        LtResultCpIndex,
        LtResultMc1Index,
        LtResultMc2Index,
        LtResultMc3Index
    }

    public partial class FormPartsCountSetting : Form
    {
        MainWindow mw;

        private int _nRealData_ResultLc;
        private int _nRealData_ResultCp;
        private int _nRealData_ResultMc1;
        private int _nRealData_ResultMc2;
        private int _nRealData_ResultMc3;

        public int nRealData_ResultLc { get { return _nRealData_ResultLc; } set { _nRealData_ResultLc = value; } }
        public int nRealData_ResultCp { get { return _nRealData_ResultCp; } set { _nRealData_ResultCp = value; } }
        public int nRealData_ResultMc1 { get { return _nRealData_ResultMc1; } set { _nRealData_ResultMc1 = value; } }
        public int nRealData_ResultMc2 { get { return _nRealData_ResultMc2; } set { _nRealData_ResultMc2 = value; } }
        public int nRealData_ResultMc3 { get { return _nRealData_ResultMc3; } set { _nRealData_ResultMc3 = value; } }

        private int _nLimitData_ResultLc;
        private int _nLimitData_ResultCp;
        private int _nLimitData_ResultMc1;
        private int _nLimitData_ResultMc2;
        private int _nLimitData_ResultMc3;

        public int nLimitData_ResultLc { get { return _nLimitData_ResultLc; } set { _nLimitData_ResultLc = value; } }
        public int nLimitData_ResultCp { get { return _nLimitData_ResultCp; } set { _nLimitData_ResultCp = value; } }
        public int nLimitData_ResultMc1 { get { return _nLimitData_ResultMc1; } set { _nLimitData_ResultMc1 = value; } }
        public int nLimitData_ResultMc2 { get { return _nLimitData_ResultMc2; } set { _nLimitData_ResultMc2 = value; } }
        public int nLimitData_ResultMc3 { get { return _nLimitData_ResultMc3; } set { _nLimitData_ResultMc3 = value; } }


        private const int nMax = 12;
        public int nEnumIndex = 0;
        int[] nLimitResult = new int[5];
        int[] nResetResult = new int[5];

        //// string 형
        //string[] strName_Result = new string[nMax];

        //// 인덱서 정의. string 파라미터 사용
        //public string this[int index]
        //{
        //    get
        //    {
        //        if (index < 0 || index >= nMax)
        //        {
        //            throw new IndexOutOfRangeException();
        //        }
        //        else
        //        {
        //            return strName_Result[index];
        //        }
        //    }
        //    set
        //    {
        //        if (!(index < 0 || index >= nMax))
        //        {
        //            strName_Result[index] = value;
        //        }
        //    }
        //}

        public FormPartsCountSetting()
        {

        }

        public FormPartsCountSetting(MainWindow mw)
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
        public FormPartsCountSetting(MainWindow mw, string strStart)
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


        //Reset 창 관련 부분
        #region
        private void btnRtLvConnector_Click(object sender, EventArgs e)
        {
            ShowHideButton(1);
            nEnumIndex = (int)SparePatsCntIndex.RlResultLcIndex;
            SeleteButtonColor((int)SparePatsCntIndex.RlResultLcIndex);
        }

        private void btnRtCurrentProbe_Click(object sender, EventArgs e)
        {
            ShowHideButton(1);
            nEnumIndex = (int)SparePatsCntIndex.RlResultCpIndex;
            SeleteButtonColor((int)SparePatsCntIndex.RlResultCpIndex);
        }

        private void btnRtMc1_Click(object sender, EventArgs e)
        {
            ShowHideButton(1);
            nEnumIndex = (int)SparePatsCntIndex.RlResultMc1Index;
            SeleteButtonColor((int)SparePatsCntIndex.RlResultMc1Index);
        }

        private void btnRtMc2_Click(object sender, EventArgs e)
        {
            ShowHideButton(1);
            nEnumIndex = (int)SparePatsCntIndex.RlResultMc2Index;
            SeleteButtonColor((int)SparePatsCntIndex.RlResultMc2Index);
        }

        private void btnRtMc3_Click(object sender, EventArgs e)
        {
            ShowHideButton(1);
            nEnumIndex = (int)SparePatsCntIndex.RlResultMc3Index;
            SeleteButtonColor((int)SparePatsCntIndex.RlResultMc3Index);
        }

        private void btnRtReset_Click(object sender, EventArgs e)
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
            strPerson = txRtPerson.Text;
            strReason = txRtReason.Text;

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
                    case (int)SparePatsCntIndex.RlResultLcIndex:
                        strWriteResult = string.Format("{0}.{1}.{2} {3}:{4} 'Name:{5}' The Lv Connector Item has been reset(Reason:{6})"
                            , year, month, day, hour, minute, strPerson, strReason);
                        break;
                    case (int)SparePatsCntIndex.RlResultCpIndex:
                        strWriteResult = string.Format("{0}.{1}.{2} {3}:{4} 'Name:{5}' The Current Probe Item has been reset(Reason:{6})"
                            , year, month, day, hour, minute, strPerson, strReason);
                        break;
                    case (int)SparePatsCntIndex.RlResultMc1Index:
                        strWriteResult = string.Format("{0}.{1}.{2} {3}:{4} 'Name:{5}' The Mc1 Item has been reset(Reason:{6})"
                            , year, month, day, hour, minute, strPerson, strReason);
                        break;
                    case (int)SparePatsCntIndex.RlResultMc2Index:
                        strWriteResult = string.Format("{0}.{1}.{2} {3}:{4} 'Name:{5}' The Mc2 Item has been reset(Reason:{6})"
                            , year, month, day, hour, minute, strPerson, strReason);
                        break;
                    case (int)SparePatsCntIndex.RlResultMc3Index:
                        strWriteResult = string.Format("{0}.{1}.{2} {3}:{4} 'Name:{5}' The Mc3 Item has been reset(Reason:{6})"
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

        private void DefaultButtonColor()
        {
            btnRtLvConnector.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            btnRtCurrentProbe.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            btnRtMc1.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            btnRtMc2.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
            btnRtMc3.BackColor = System.Drawing.Color.FromArgb(128, 128, 128);
        }

        private void SeleteButtonColor(int nIndex)
        {
            DefaultButtonColor();

            switch (nIndex)
            {
                case (int)SparePatsCntIndex.RlResultLcIndex:
                    btnRtLvConnector.BackColor = System.Drawing.Color.FromArgb(215, 215, 215);
                    break;
                case (int)SparePatsCntIndex.RlResultCpIndex:
                    btnRtCurrentProbe.BackColor = System.Drawing.Color.FromArgb(215, 215, 215);
                    break;
                case (int)SparePatsCntIndex.RlResultMc1Index:
                    btnRtMc1.BackColor = System.Drawing.Color.FromArgb(215, 215, 215);
                    break;
                case (int)SparePatsCntIndex.RlResultMc2Index:
                    btnRtMc2.BackColor = System.Drawing.Color.FromArgb(215, 215, 215);
                    break;
                case (int)SparePatsCntIndex.RlResultMc3Index:
                    btnRtMc3.BackColor = System.Drawing.Color.FromArgb(215, 215, 215);
                    break;
            }
        }
        //Reset 취소 버튼
        private void btnRtCancel_Click(object sender, EventArgs e)
        {
            ShowHideButton(0);
            DefaultButtonColor();
        }
        #endregion

        //Limit 창 관련 부분
        #region
        private void btnLtLvConnector_Click(object sender, EventArgs e)
        {
            ShowHideButton(2);
        }

        private void btnLtCurrentProbe_Click(object sender, EventArgs e)
        {
            ShowHideButton(2);
        }

        private void btnLtMc1_Click(object sender, EventArgs e)
        {
            ShowHideButton(2);
        }

        private void btnLtMc2_Click(object sender, EventArgs e)
        {
            ShowHideButton(2);
        }

        private void btnLtMc3_Click(object sender, EventArgs e)
        {
            ShowHideButton(2);
        }

        private bool ResultLimitDataEdit()
        {
            try
            {
                nLimitData_ResultLc = nLimitResult[0] = int.Parse(txLtLvConnector.Text);
                nLimitData_ResultCp = nLimitResult[1] = int.Parse(txLtCurrentProbe.Text);
                nLimitData_ResultMc1 = nLimitResult[2] = int.Parse(txLtMc1.Text);
                nLimitData_ResultMc2 = nLimitResult[3] = int.Parse(txLtMc2.Text);
                nLimitData_ResultMc3 = nLimitResult[4] = int.Parse(txLtMc3.Text);
            }
            catch (Exception)
            {
                //System.Windows.MessageBox.Show("NO NO NO NO NO NO");
                return false;
            }
            return true;
        }

        private void LoadLimitDataEdit()
        {
            txLtLvConnector.Text = string.Format("{0} ", nLimitData_ResultLc);
            txLtCurrentProbe.Text = string.Format("{0}", nLimitData_ResultCp);
            txLtMc1.Text = string.Format("{0} ", nLimitData_ResultMc1);
            txLtMc2.Text = string.Format("{0} ", nLimitData_ResultMc2);
            txLtMc3.Text = string.Format("{0}", nLimitData_ResultMc3);
        }

        private void LoadAllDataEdit()
        {
            try
            {
                nLimitResult[0] = nLimitData_ResultLc;
                nLimitResult[1] = nLimitData_ResultCp;
                nLimitResult[2] = nLimitData_ResultMc1;
                nLimitResult[3] = nLimitData_ResultMc2;
                nLimitResult[4] = nLimitData_ResultMc3;

                nResetResult[0] = nRealData_ResultLc;
                nResetResult[1] = nRealData_ResultCp;
                nResetResult[2] = nRealData_ResultMc1;
                nResetResult[3] = nRealData_ResultMc2;
                nResetResult[4] = nRealData_ResultMc3;
            }
            catch (Exception) { }
        }

        //리미트 저장하는 부분
        private void btnLtApply_Click(object sender, EventArgs e)
        {
            SaveSparePatsCountInfo("LIMIT");
        }

        #endregion

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

                            if (values[0] == "End") return;

                            if (values[0] == "DCIR") strIndexCheck = values[0];

                            //엑셀이 공백일 경우 0으로 디폴트
                            if (values[values.Length - 1] == "" && values.Length == 2)
                            {
                                values[values.Length - 1] = "0";
                            }

                            if (strIndexCheck == "DCIR")
                            {
                                if (values[0] == "Name" && values[1] == "Limit") bIndexCheck = true;

                                if (bIndexCheck == false)
                                {
                                    switch (values[0])
                                    {
                                        case "txLtLvConnector": nRealData_ResultLc = int.Parse(values[1]); break;
                                        case "txLtCurrentProbe": nRealData_ResultCp = int.Parse(values[1]); break;
                                        case "txLtMc1": nRealData_ResultMc1 = int.Parse(values[1]); break;
                                        case "txLtMc2": nRealData_ResultMc2 = int.Parse(values[1]); break;
                                        case "txLtMc3": nRealData_ResultMc3 = int.Parse(values[1]); break;
                                    }
                                }
                                else
                                {
                                    switch (values[0])
                                    {
                                        case "txLtLvConnector": nLimitData_ResultLc = int.Parse(values[1]); break;
                                        case "txLtCurrentProbe": nLimitData_ResultCp = int.Parse(values[1]); break;
                                        case "txLtMc1": nLimitData_ResultMc1 = int.Parse(values[1]); break;
                                        case "txLtMc2": nLimitData_ResultMc2 = int.Parse(values[1]); break;
                                        case "txLtMc3": nLimitData_ResultMc3 = int.Parse(values[1]); break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
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

                            if (values[0] == "DCIR") strIndexCheck = values[0];

                            if (strIndexCheck == "DCIR")
                            {
                                if (values[0] == "DCIR")
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

        //검사 한번 할때 전부 카운트 하는 부분
        public void CountRealData()
        {
            nRealData_ResultLc++;
            nRealData_ResultCp++;
            nRealData_ResultMc1++;
            nRealData_ResultMc2++;
            nRealData_ResultMc3++;

            nResetResult[0] = nRealData_ResultLc;
            nResetResult[1] = nRealData_ResultCp;
            nResetResult[2] = nRealData_ResultMc1;
            nResetResult[3] = nRealData_ResultMc2;
            nResetResult[4] = nRealData_ResultMc3;

            SaveSparePatsCountInfo("RESET_CNT");
        }

        //Reset data show
        public void ShowResetData()
        {
            lbRtLvConnector.Text = string.Format("{0} / {1}", nRealData_ResultLc, nLimitData_ResultLc);
            lbRtCurrentProbe.Text = string.Format("{0} / {1}", nRealData_ResultCp, nLimitData_ResultCp);
            lbRtMc1.Text = string.Format("{0} / {1}", nRealData_ResultMc1, nLimitData_ResultMc1);
            lbRtMc2.Text = string.Format("{0} / {1}", nRealData_ResultMc2, nLimitData_ResultMc2);
            lbRtMc3.Text = string.Format("{0} / {1}", nRealData_ResultMc3, nLimitData_ResultMc3);
        }

        //reset, Limit Button disable, able
        public void ShowHideButton(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    //reset
                    lbRtPerson.Visible = false;
                    lbRtReason.Visible = false;
                    txRtPerson.Visible = false;
                    txRtReason.Visible = false;
                    btnRtReset.Visible = false;
                    btnRtCancel.Visible = false;
                    //limit
                    btnLtApply.Visible = true;
                    break;
                case 1:
                    //reset
                    lbRtPerson.Visible = true;
                    lbRtReason.Visible = true;
                    txRtPerson.Visible = true;
                    txRtReason.Visible = true;
                    btnRtReset.Visible = true;
                    btnRtCancel.Visible = true;
                    break;
                case 2:
                    //reset
                    lbRtPerson.Visible = false;
                    lbRtReason.Visible = false;
                    txRtPerson.Visible = false;
                    txRtReason.Visible = false;
                    btnRtReset.Visible = false;
                    btnRtCancel.Visible = false;
                    break;
                default:
                    //reset
                    lbRtPerson.Visible = false;
                    lbRtReason.Visible = false;
                    txRtPerson.Visible = false;
                    txRtReason.Visible = false;
                    btnRtReset.Visible = false;
                    btnRtCancel.Visible = false;
                    //limit
                    btnLtApply.Visible = true;

                    break;
            }
        }

        private void btnPCSLogOpen_Click(object sender, EventArgs e)
        {
            //this.mw.position
            string @FILE_PATH_PCS = @"C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\Log";
            System.Diagnostics.Process.Start("C:\\EOL_SPAREPATSCOUNT_INFO\\" + this.mw.position + "\\Log\\", @FILE_PATH_PCS);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
