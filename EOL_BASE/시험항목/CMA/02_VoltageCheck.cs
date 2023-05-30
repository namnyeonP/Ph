using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        List<double> CellVoltageList;
        int cellVoltCnt = 12;
        List<double> tempList;
        double tempCnt = 2;

        public bool ModuleVoltage(TestItem ti)
        {
            isProcessingUI(ti);
                       
            if (keysight == null)
            {
                ti.Value_ = "NotConnected";
                return JudgementTestItem(ti);
            }

            //var str = "0AA";
            //switch (localTypes)
            //{
            //    case 2: str = "0AA"; break;
            //    case 0: str = "0BB"; break;
            //}

            //cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            //LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            //Thread.Sleep(500);

            //sensing regist
            keysight.rtstring = "";
            cellVoltCnt = 0;
            //var measString = "101,102,103,104,105,106,107,108,117";//,109,110,111,112,113,114,115,116,117"; 
            var measString = "101,102,103,104,106,107,108,109,117";

            keysight.MeasTemp(measString);

            int rec = keysight.sp.BytesToRead;

            int cnt = 0;
            while (rec < 145)
            {
                Thread.Sleep(100);
                rec = keysight.sp.BytesToRead;
                cnt += 100;
                //not received data
                if (cnt == 5000)
                {
                    keysight.MeasTemp(measString);

                    rec = keysight.sp.BytesToRead;

                    cnt = 0;
                    while (rec < 145)
                    {
                        Thread.Sleep(100);
                        rec = keysight.sp.BytesToRead;
                        cnt += 100;
                        if (cnt == 5000)
                        {
                            ti.Value_ = _DEVICE_NOT_READY;
                            return JudgementTestItem(ti);
                        }
                    }
                    break;
                }
            }
            //받은 후에 데이터 파싱
            byte[] bt = new byte[rec];
            keysight.sp.Read(bt, 0, rec);

            keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

            
            //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

            var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var resList = new List<double>();
            string resetr = "";
            foreach (var item in vArr)
            {
                double dv = 0;
                if (double.TryParse(item, out dv))
                {
                    resList.Add(dv);
                    resetr += dv.ToString() + "&";
                }
            }
            LogState(LogType.RESPONSE, "KeysightDMM RES:" + resetr);

            keysight.rtstring = "";
            keysight.MeasVolt(measString);

            rec = keysight.sp.BytesToRead;

            cnt = 0;
            while (rec < 145)//33
            {
                Thread.Sleep(100);
                rec = keysight.sp.BytesToRead;
                cnt += 100;
                //not received data
                if (cnt == 5000)
                {
                    keysight.MeasVolt(measString);

                    rec = keysight.sp.BytesToRead;

                    cnt = 0;
                    while (rec < 145)//33
                    {
                        Thread.Sleep(100);
                        rec = keysight.sp.BytesToRead;
                        cnt += 100;
                        if (cnt == 5000)
                        {
                            ti.Value_ = _DEVICE_NOT_READY;
                            return JudgementTestItem(ti);
                        }
                    }
                    break;
                }
            }
            //받은 후에 데이터 파싱
            bt = new byte[rec];
            keysight.sp.Read(bt, 0, rec);

            keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

            //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

            vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            CellVoltageList = new List<double>();
            string voltstr = "";
            foreach (var item in vArr)
            {
                double dv=0;
                if (double.TryParse(item, out dv))
                {                    
                    CellVoltageList.Add(dv);
                    voltstr += dv.ToString() + ",";
                }
            }
            LogState(LogType.RESPONSE, "KeysightDMM Volt:" + voltstr);

            for (int i = 0; i < resList.Count - 1; i++)
            {
                if (resList[i] > 1000000)
                {
                    CellVoltageList[i] = 0;
                }
            }

            ti.Value_ = CellVoltageList.Last();

            CellVoltageList.Remove(CellVoltageList.Last());
            
            //if (cycler.cycler1voltage < 10)
            //{
            //    ti.Value_ = _DEVICE_NOT_READY;
            //}
            //else
            //{
            //    ti.Value_ = cycler.cycler1voltage;
            //}

            //cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });

            // 모듈전압 찍고 바로 초기 전압으로 입력
            SetOcvCellVoltage(CellVoltageList);

            return JudgementTestItem(ti);
        }

        private void SetOcvCellVoltage(List<double> CellVoltageList)
        {
            //ocvDetailList[0].CellVolt_1 = double.Parse(CellVoltageList[0].ToString("N4"));
            //ocvDetailList[0].CellVolt_2 = double.Parse(CellVoltageList[1].ToString("N4"));
            //ocvDetailList[0].CellVolt_3 = double.Parse(CellVoltageList[2].ToString("N4"));
            //ocvDetailList[0].CellVolt_4 = double.Parse(CellVoltageList[3].ToString("N4"));
            //ocvDetailList[0].CellVolt_5 = double.Parse(CellVoltageList[4].ToString("N4"));
            //ocvDetailList[0].CellVolt_6 = double.Parse(CellVoltageList[5].ToString("N4"));
            //ocvDetailList[0].CellVolt_7 = double.Parse(CellVoltageList[6].ToString("N4"));
            //ocvDetailList[0].CellVolt_8 = double.Parse(CellVoltageList[7].ToString("N4"));
        }

        public bool CellVoltage_High(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList.Max();
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage_Low(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList.Min();
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage_Dev(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                // 2018.11.02 jeonhj's comment
                // Deviation unit is mV(1V == 100mV)
                ti.Value_ = (CellVoltageList.Max()- CellVoltageList.Min()) * 100;
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage1(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[0];
            }
               
            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage2(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[1];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage3(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[2];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage4(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[3];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage5(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[4];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage6(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[5];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage7(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[6];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage8(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[7];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage9(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[8];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage10(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[9];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage11(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[10];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage12(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[11];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage13(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[12];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage14(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[13];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltageDev(TestItem ti)
        {
            isProcessingUI(ti);

            if (CellVoltageList != null && CellVoltageList.Count>2)
            {
                CellVoltageList.Remove(CellVoltageList.Max());
                ti.Value_ = (CellVoltageList.Max() - CellVoltageList.Min()) * 100;
            }

            return JudgementTestItem(ti);
        }
    }
}