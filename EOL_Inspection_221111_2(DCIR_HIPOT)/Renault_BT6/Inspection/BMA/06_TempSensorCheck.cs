using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Renault_BT6
{

    public partial class MainWindow
    {
        double temp1delta;
        double temp2delta;
        double temp3delta;
        double temp4delta;
        double temp5delta;
        double temp6delta;
        double temp7delta;
        double temp8delta;
        double temp9delta;
        double temp10delta;
        double temp11delta;
        double temp12delta;

        bool GetToDID_singleData(byte DID_byte1, byte DID_byte2, string expectRtn, bool iscontinueMsg, out List<string> array,
            byte firstbyte = 0x03, byte secondbyte = 0x22,
            byte fifthbyte = 0x00, byte sixthbyte = 0x00, byte seventhbyte = 0x00, byte eighthbyte = 0x00)
        {
            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { firstbyte, secondbyte, DID_byte1, DID_byte2, fifthbyte, sixthbyte, seventhbyte, eighthbyte });
            LogState(LogType.Info, "Send 735 :" + firstbyte.ToString("X2") + 
                " " + secondbyte.ToString("X2") + 
                " " + DID_byte1.ToString("X2") + 
                " " + DID_byte2.ToString("X2") +
                " " + fifthbyte.ToString("X2") +
                " " + sixthbyte.ToString("X2") +
                " " + seventhbyte.ToString("X2") +
                " " + eighthbyte.ToString("X2"));

            if (iscontinueMsg)
            {
                Thread.Sleep(70);
            }
            else
            {
                Thread.Sleep(100);
            }

            if (Hybrid_Instru_CAN._635List.Count > 0 && Hybrid_Instru_CAN._635List[0].Contains(expectRtn))
            {
                if (iscontinueMsg)
                {
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Info, "Recv 635 :" + fe009c);
                    }
                    Thread.Sleep(30);
                    Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    LogState(LogType.Info, "Send 735 :30 00 00 00 00 00 00 00");
                    Thread.Sleep(300);
                }

                var rList = new List<string>();
                lock (_635_obj)
                {
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Info, "Recv 635 :" + fe009c);
                        var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < arr.Length; i++)
                        {
                            rList.Add(arr[i]);
                        }
                    }
                    array = rList;
                }
                return true;
                //ti.Value_ = rList[4] + rList[5] + rList[6] + rList[7] + GetHexToStr(rList[8] + rList[9] + rList[10]);
            }
            else
            {
                LogState(LogType.Fail, "NOT Received Data");
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recv 635 :" + fe009c);
                }
                var rList = new List<string>();
                for (int i = 0; i < 200; i++)
                {
                    rList.Add("0");
                }
                array = rList;
                return false;
            }
        }

        bool GetToDID_singleData2(byte DID_byte1, byte DID_byte2, string expectRtn, bool iscontinueMsg, out List<string> array,
            byte firstbyte = 0x03, byte secondbyte = 0x22,
            byte fifthbyte = 0x00, byte sixthbyte = 0x00, byte seventhbyte = 0x00, byte eighthbyte = 0x00)
        {
            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { firstbyte, secondbyte, DID_byte1, DID_byte2, fifthbyte, sixthbyte, seventhbyte, eighthbyte });
            LogState(LogType.Info, "Send 735 { 0x" + firstbyte.ToString("X2") +
                ", 0x" + secondbyte.ToString("X2") +
                ", 0x" + DID_byte1.ToString("X2") +
                ", 0x" + DID_byte2.ToString("X2") +
                ", 0x" + fifthbyte.ToString("X2") +
                ", 0x" + sixthbyte.ToString("X2") +
                ", 0x" + seventhbyte.ToString("X2") +
                ", 0x" + eighthbyte.ToString("X2"));

            if (iscontinueMsg)
            {
                Thread.Sleep(70);
            }
            else
            {
                Thread.Sleep(100);
            }

            if (Hybrid_Instru_CAN._635List.Count > 1 && Hybrid_Instru_CAN._635List[1].Contains(expectRtn))
            {
                if (iscontinueMsg)
                {
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recv 635 :" + fe009c);
                    }
                    Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    LogState(LogType.Info, "Send 735 :30 00 00 00 00 00 00 00");

                    Thread.Sleep(200);
                }

                var rList = new List<string>();
                lock (_635_obj)
                {
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recv 635 :" + fe009c);
                        var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < arr.Length; i++)
                        {
                            rList.Add(arr[i]);
                        }
                    }
                    array = rList;
                }
                return true;
                //ti.Value_ = rList[4] + rList[5] + rList[6] + rList[7] + GetHexToStr(rList[8] + rList[9] + rList[10]);
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recv 635 :" + fe009c);
                }
                array = null;
                return false;
            }
        }

        object _635_obj = new object();
        
        /*
        public bool TempSensorEach(TestItem ti)
        {
            isProcessingUI(ti);
            temp1delta = 0.0;
            temp2delta = 0.0;
            temp3delta = 0.0;
            temp4delta = 0.0;
            temp5delta = 0.0;
            temp6delta = 0.0;
            temp7delta = 0.0;
            temp8delta = 0.0;
            temp9delta = 0.0;
            temp10delta=0.0;
            temp11delta=0.0;
            temp12delta = 0.0;
            var rList = new List<string>();
            GetToDID_singleData(0x49, 0x09, "05 62", false, out rList);
            temp1delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0A, "05 62", false, out rList);
            temp2delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0B, "05 62", false, out rList);
            temp3delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0C, "05 62", false, out rList);
            temp4delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0D, "05 62", false, out rList);
            temp5delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0E, "05 62", false, out rList);
            temp6delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0F, "05 62", false, out rList);
            temp7delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x10, "05 62", false, out rList);
            temp8delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x11, "05 62", false, out rList);
            temp9delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x12, "05 62", false, out rList);
            temp10delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x13, "05 62", false, out rList);
            temp11delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x14, "05 62", false, out rList);
            temp12delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;


            List<double> tempList = new List<double>();
            tempList.Add(temp1delta);
            tempList.Add(temp2delta);
            tempList.Add(temp3delta);
            tempList.Add(temp4delta);
            tempList.Add(temp5delta);
            tempList.Add(temp6delta);
            tempList.Add(temp7delta);
            tempList.Add(temp8delta);
            tempList.Add(temp9delta);
            tempList.Add(temp10delta);
            tempList.Add(temp11delta);
            tempList.Add(temp12delta);

            tempViewList.Clear();
            for (int ii = 0; ii < tempList.Count; ii++)
            {
                tempViewList.Add(new doubleValue() { Name = "Temp" + (ii + 1).ToString(), Value_ = tempList[ii] });
            }

            var aTemp = temps.tempStr;
            LogState(LogType.Info,"Max Temp:"+tempList.Max()+"/Min Temp:"+tempList.Min()+"/Ambint Temp:"+aTemp);
            ti.Value_ = tempList.Max() - tempList.Min();

            var maxV = double.Parse(ti.Max.ToString());
            int i = 1;
            foreach (var item in tempList)
            {
                LogState(LogType.Info, "Temp"+i.ToString()+":"+item);
                var dev = Math.Abs(aTemp-item);
                if (dev > maxV)
                {
                    LogState(LogType.NG, "Ambint Temp:" + aTemp + "/Temp" + i.ToString() + ":" + item + "/Deviation:" + dev);
                    ti.Value_ = dev;
                }
                i++;
            }


            return JudgementTestItem(ti);
        }


        public bool TempSensor1Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp1delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor2Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp2delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor3Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp3delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor4Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp4delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor5Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp5delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor6Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp6delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor7Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp7delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor8Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp8delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor9Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp9delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor10Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp10delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor11Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp11delta;
            return JudgementTestItem(ti);
        }


        public bool TempSensor12Delta(TestItem ti)
        {
            isProcessingUI(ti);
            var localTemp = temps.tempStr;
            LogState(LogType.Info, "Ambient Temp:" + localTemp);
            ti.Value_ = localTemp - temp12delta;
            return JudgementTestItem(ti);
        }
        */

    }
}