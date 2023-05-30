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
        string ascii1;
        string ascii2;
        string ascii3;
        string ascii4;

        public string GetHexToStr(string hexString)
        {
            string ascii = string.Empty;

            for (int i = 0; i < hexString.Length; i += 2)
            {
                String hs = string.Empty;

                hs = hexString.Substring(i, 2);
                uint decval = System.Convert.ToUInt32(hs, 16);
                char character = System.Convert.ToChar(decval);
                ascii += character;

            }
            return ascii;
        }

        public bool SWVersion(TestItem ti)
        {
            isProcessingUI(ti);

            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xFD, 0x00, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xFD, 0x00, 0x00, 0x00, 0x00, 0x00}");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("07 62"))
            {
                LogState(LogType.Info, "Recved data - " + Hybrid_Instru_CAN._635List[0]);
                var strArr0 = Hybrid_Instru_CAN._635List[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                ti.Value_ = Convert.ToInt32(strArr0[4], 16).ToString() + Convert.ToInt32(strArr0[5], 16).ToString() + Convert.ToInt32(strArr0[6], 16).ToString() + Convert.ToInt32(strArr0[7], 16).ToString();
                //ti.Value_ = strArr0[4].ToString() + " " + strArr0[5].ToString() + " " + strArr0[6].ToString() + " " + strArr0[7].ToString();

                //pack_pre_fuse = Convert.ToInt64(strArr0[4].ToString() + strArr0[5], 16) * 0.1;
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
            }
            return JudgementTestItem(ti);
        }


        public bool SWVersionPN1(TestItem ti)
        {
            isProcessingUI(ti);

            ascii1 = ascii2 = ascii3 = ascii4 = "";

            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xF1, 0x2E, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xF1, 0x2E, 0x00, 0x00, 0x00, 0x00}");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 20 62"))
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                }
                Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 735 { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}");

                Thread.Sleep(100);
                var rList = new List<string>();
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                    var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < arr.Length; i++)
                    {
                        rList.Add(arr[i]);
                    }
                }


                ascii1 = rList[5] + rList[6] + rList[7] + rList[8] + GetHexToStr(rList[9] + rList[10] + rList[11]);
                ascii2 = rList[12] + rList[13] + rList[14] + rList[15] + GetHexToStr(rList[16] + rList[17] + rList[18]);
                ascii3 = rList[19] + rList[20] + rList[21] + rList[22] + GetHexToStr(rList[23] + rList[24] + rList[25]);
                ascii4 = rList[26] + rList[27] + rList[28] + rList[29] + GetHexToStr(rList[30] + rList[31] + rList[32]);

                ti.Value_ = ascii1;
                //pack_pre_fuse = Convert.ToInt64(strArr0[4].ToString() + strArr0[5], 16) * 0.1;
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
            }
            return JudgementTestItem(ti);
        }


        public bool SWVersionPN2(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = ascii2;
            return JudgementTestItem(ti);
        }


        public bool SWVersionPN3(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = ascii3;
            return JudgementTestItem(ti);
        }


        public bool SWVersionPN4(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = ascii4;
            return JudgementTestItem(ti);
        }


        public bool SWVersionDBPN(TestItem ti)
        {
            isProcessingUI(ti);
            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xF1, 0x20, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xF1, 0x20, 0x00, 0x00, 0x00, 0x00}");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 0A"))
            {                 
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                }
                Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 735 { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}");

                Thread.Sleep(100);
                var rList = new List<string>();
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                    var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < arr.Length; i++)
                    {
                        rList.Add(arr[i]);
                    }
                }
                
                ti.Value_ = rList[4] + rList[5] + rList[6] + rList[7] + GetHexToStr(rList[8] + rList[9] + rList[10]);
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
            }
            return JudgementTestItem(ti);
        }


        public bool SWVersionPBLPN(TestItem ti)
        {
            isProcessingUI(ti);
            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xF1, 0x25, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xF1, 0x25, 0x00, 0x00, 0x00, 0x00}");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 0A"))
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                }
                Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 735 { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}");

                Thread.Sleep(100);
                var rList = new List<string>();
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                    var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < arr.Length; i++)
                    {
                        rList.Add(arr[i]);
                    }
                }

                ti.Value_ = rList[4] + rList[5] + rList[6] + rList[7] + GetHexToStr(rList[8] + rList[9] + rList[10]);
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
            }
            return JudgementTestItem(ti);
        }

    }
}