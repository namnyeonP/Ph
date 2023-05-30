using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Renault_BT6
{

    public partial class MainWindow
    {
        public bool ECU_PartNo(TestItem ti)
        {
            isProcessingUI(ti);
            var rList = new List<string>();
            GetToDID_singleData(0xF1, 0x2A, "10 ", true, out rList);
            ti.Value_ = rList[4] + rList[5] + rList[6] + rList[7] + GetHexToStr(rList[8] + rList[9] + rList[10]);
            return JudgementTestItem(ti);
        }


        public bool Pack_PartNo(TestItem ti)
        {
            isProcessingUI(ti);
            var rList = new List<string>();
            GetToDID_singleData(0xF1, 0x2B, "10 ", true, out rList);
            ti.Value_ = rList[4] + rList[5] + rList[6] + rList[7] + GetHexToStr(rList[8] + rList[9] + rList[10]);
            return JudgementTestItem(ti);
        }

        public bool IsolationStatus(TestItem ti)
        {
            isProcessingUI(ti);

            tempGBTID = tempFlashedPackID = string.Empty;

            #region Pack ID Write

            Thread.Sleep(2000);

            LogState(LogType.Info, "Writing Pack ID - " + strId);
            var originMode1 = Hybrid_Instru_CAN.IsSendMsg;
            var originMode2 = Hybrid_Instru_CAN.ExtendedMsg;

            Thread.Sleep(1000);
            if (Flash(strId))
            {
                LogState(LogType.Success, "Writing Pack ID - " + strId);
            }
            else
            {
                tempFlashedPackID = "NOT WRITTED";
                ti.Value_ = "NOT WRITTED";
                return JudgementTestItem(ti);
            }

            Hybrid_Instru_CAN.IsSendMsg = originMode1;
            Hybrid_Instru_CAN.ExtendedMsg = originMode2;

            #endregion

            #region EF Write

            Thread.Sleep(500);

            if (!SetDefaultMode())
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);
            
            if (!SetExtendedMode())
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);

            var rList = new List<string>();
            if (!GetToDID_singleData(0x48, 0x95, "03 ", false, out rList, 0x04, 0x2E, 0xEF))
            {
                ti.Value_ = _VALUE_NOT_MATCHED;
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);

            if (!SetDefaultMode())
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                return JudgementTestItem(ti);
            }

            #endregion

            #region GBTID Write

            //var gbtid = "LGCPE02POL00058580000001";

            //var gbtBytes = Encoding.ASCII.GetBytes(gbtid);
            //rList = new List<string>();
            //if (GetToDID_singleData(0x2E, 0xDB, "30 ", true, out rList, 0x10, 0x1B, 0x92, gbtBytes[0], gbtBytes[1], gbtBytes[2]))
            //{
            //    Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x21, gbtBytes[3], gbtBytes[4], gbtBytes[5], gbtBytes[6], gbtBytes[7], gbtBytes[8], gbtBytes[9] });
            //    LogState(LogType.Info, "Send 735 { 0x21" +
            //    " " + gbtBytes[3].ToString("X2") +
            //    " " + gbtBytes[4].ToString("X2") +
            //    " " + gbtBytes[5].ToString("X2") +
            //    " " + gbtBytes[6].ToString("X2") +
            //    " " + gbtBytes[7].ToString("X2") +
            //    " " + gbtBytes[8].ToString("X2") +
            //    " " + gbtBytes[9].ToString("X2"));
            //    Thread.Sleep(10); 
                
            //    Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x22, gbtBytes[10], gbtBytes[11], gbtBytes[12], gbtBytes[13], gbtBytes[14], gbtBytes[15], gbtBytes[16] });
            //    LogState(LogType.Info, "Send 735 { 0x22" +
            //    " " + gbtBytes[10].ToString("X2") +
            //    " " + gbtBytes[11].ToString("X2") +
            //    " " + gbtBytes[12].ToString("X2") +
            //    " " + gbtBytes[13].ToString("X2") +
            //    " " + gbtBytes[14].ToString("X2") +
            //    " " + gbtBytes[15].ToString("X2") +
            //    " " + gbtBytes[16].ToString("X2"));
            //    Thread.Sleep(10);

            //    rList = new List<string>();
            //    if (!GetToDID_singleData(gbtBytes[18], gbtBytes[19], "03 6E ", false, out rList, 0x23, gbtBytes[17], gbtBytes[20], gbtBytes[21], gbtBytes[22], gbtBytes[23]))
            //    {
            //        tempGBTID = "NOT WRITTED";
            //    }
            //    else
            //    {
            //        LogState(LogType.Success, "Writing GBT ID - " + gbtid);
            //    }
            //}
            //else
            //{
            //    tempGBTID = "NOT WRITTED";
            //}

            #endregion

            //if success, reboot bms, read data
            //becm_powerOff(40000);
            //becm_powerOn();
            Thread.Sleep(1000);

            #region Check EF Msg
            rList = new List<string>();
            if (GetToDID_singleData(0x48, 0x95, "04 ", false, out rList))
            {
                ti.Value_ = rList[3].ToString();
            }
            else
            {
                ti.Value_ = _VALUE_NOT_MATCHED;
            }
            #endregion
            
            return JudgementTestItem(ti);
        }

        string strId = "0000T034343434";//todo
        string parsed = "4444";//todo
        string tempFlashedPackID = "NOT WRITTED";
        string tempGBTID = "NOT WRITTED";

        public bool Flash_Pack_ID(TestItem ti)
        {
            isProcessingUI(ti);

            string readedID = "";
            ti.Value_ = tempFlashedPackID;

            if (tempFlashedPackID != "NOT WRITTED")
            {
                if (Check_FlaSh_ID(out readedID) == 0)
                {
                    ti.Value_ = readedID;
                    
                    #region simple Judgement

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (IsManual)
                            blinder.Visibility = System.Windows.Visibility.Hidden;
                    }));

                    if (parsed == ti.Value_.ToString())
                    {
                        ti.Result = "PASS";
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Test :");
                        sb.Append(ti.Name);
                        sb.Append(" End - Pass [Min:");
                        sb.Append(ti.Min.ToString());
                        sb.Append("][Value:");
                        sb.Append(ti.Value_);
                        sb.Append("][Max:");
                        sb.Append(ti.Max.ToString());
                        sb.Append("]");
                        this.LogState(LogType.Pass, sb.ToString());
                        return true;
                    }
                    else
                    {
                        ti.Result = "NG";
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Test :");
                        sb.Append(ti.Name);
                        sb.Append(" End - NG [Min:");
                        sb.Append(ti.Min.ToString());
                        sb.Append("][Value:");
                        sb.Append(ti.Value_);
                        sb.Append("][Max:");
                        sb.Append(ti.Max.ToString());
                        sb.Append("]");
                        this.LogState(LogType.NG, sb.ToString());
                    
                        if (isSkipNG_)
                        {
                            return true;
                        }
                    
                        return false;
                    }
                    #endregion
                }
                else
                {
                    tempFlashedPackID = "READ FAIL";
                    ti.Value_ = tempFlashedPackID;
                    return JudgementTestItem(ti);
                }
            }
            else
            {
                return JudgementTestItem(ti);
            }
        }

        public bool Flash_GBT_ID(TestItem ti)
        {
            isProcessingUI(ti);
            var gbtid = "LGCPE02POL00058580000001";
            var rList = new List<string>();
            if (GetToDID_singleData(0xDB, 0x92, "10 ", true, out rList))
            {
                var str = "";
                foreach (var item in rList)
                {
                    str += item;
                }

                ti.Value_ = GetHexToStr(str);

                #region simple Judgement

                 this.Dispatcher.BeginInvoke(new Action(() =>
                 {
                     if (IsManual)
                         blinder.Visibility = System.Windows.Visibility.Hidden;
                 }));

                 if (gbtid == ti.Value_.ToString())
                 {
                     ti.Result = "PASS";
                     StringBuilder sb = new StringBuilder();
                     sb.Append("Test :");
                     sb.Append(ti.Name);
                     sb.Append(" End - Pass [Min:");
                     sb.Append(ti.Min.ToString());
                     sb.Append("][Value:");
                     sb.Append(ti.Value_);
                     sb.Append("][Max:");
                     sb.Append(ti.Max.ToString());
                     sb.Append("]");
                     this.LogState(LogType.Pass, sb.ToString());
                     return true;
                 }
                 else
                 {
                     ti.Result = "NG";
                     StringBuilder sb = new StringBuilder();
                     sb.Append("Test :");
                     sb.Append(ti.Name);
                     sb.Append(" End - NG [Min:");
                     sb.Append(ti.Min.ToString());
                     sb.Append("][Value:");
                     sb.Append(ti.Value_);
                     sb.Append("][Max:");
                     sb.Append(ti.Max.ToString());
                     sb.Append("]");
                     this.LogState(LogType.NG, sb.ToString());

                     if (isSkipNG_)
                     {
                         return true;
                     }

                     return false;
                 }
                 #endregion
            }
            else
            {
                ti.Value_ = "READ FAIL";
            }

            return JudgementTestItem(ti);
        }

        #region Flash PACK ID

        public bool Flash(string ProductId)
        {
            string[] parser = ProductId.Split('T');
            string MarkingID = string.Empty;
            if (parser.Length == 2)
            {
                //MarkingID = string.Format("{0}{1}", "000", parser[1]);
                MarkingID = parser[1].Remove(0, 1);
            }

            /* Flash Pack ID //스펙임시조치 (Pack ID 어떻게 쓸건지 정해서 MarkingID에 8자리 STRING값 집어 넣어서 
             * Send_Flash_ID 호출하면 써짐 */
            if (MarkingID != null)
            {
                //if ( Send_FlaSh_ID(MarkingID, out strOutID) == 0)
                if (Send_FlaSh_ID(MarkingID) == 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        

        bool BMSUseComplete = false;
        public int Check_FlaSh_ID(out string strReturnPackID)
        {
            byte[] RcvID = new byte[4];
            uint iReceiveID;
            byte[] recv;
            strReturnPackID = "";
            //쓴거 확인

            var rList = new List<string>();
            if (GetToDID_singleData(0xF1, 0x8C, "62 ", false, out rList))
            {
                if (rList[3] == "FF" || rList[4] == "FF" || rList[5] == "FF" || rList[6] == "FF")
                {
                    return -1;
                }
                strReturnPackID = GetHexToStr(rList[3] + rList[4] + rList[5] + rList[6]);
                return 0;
            }
            else
            {
                return -1;
            }
            
        }

        public int Send_FlaSh_ID(string strPackID)
        {   //PackID는 8자리만 쓸수 있음 8자리 이상 들어올시 짤릴 것임
            //SystemLogger.Log(Level.Debug, "Send_FlaSh_ID", Name, Name);
            //써야할 팩 ID
            byte[] WritePackID = new byte[4];

            //strReturnPackID = "";

            try
            {   //들어온 string을 Hex바이트 배열로 변환(packid는 Hex 4byte)                
                WritePackID = Enumerable.Range(0, strPackID.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(strPackID.Substring(x, 2), 16))
                             .ToArray();
            }
            catch (Exception ex)
            {
                //SystemLogger.Log(Level.Debug, ex, Name, Name);
                return -1;
            }


            string filePath = @"D:\SBL_06.01.07.B2.02.CPI.vbf";

            if (!File.Exists(filePath))
            {   //파일 없으면 
                return -1;
            }

            //byte[] bFrame1Add_Length = new byte[8];
            //byte[] bFrame1Data;
            byte[] bFrame2Add_Length = new byte[8];
            byte[] bFrame2Data;
            byte[] bFrame3Add_Length = new byte[8];
            byte[] bFrame3Data;

            uint iReceiveID;

            ushort MaxBlockLength = 0;

            #region 파일 읽기
            try
            {
                using (FileStream VBLReader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    //7d를 찾는다.
                    while (VBLReader.ReadByte() != 0x7d) ;

                    //주소 4byte와 길이4byte
                    //for (int i = 0; i < 8; i++)
                    //    bFrame1Add_Length[i] = (byte)VBLReader.ReadByte();

                    //bFrame1 4~7바이트가 길이 바이트 오더가 역순 최상위 상위 하위 최하위
                    //uint Length = bFrame1Add_Length[4];
                    //Length <<= 8;
                    //Length |= bFrame1Add_Length[5];
                    //Length <<= 8;
                    //Length |= bFrame1Add_Length[6];
                    //Length <<= 8;
                    //Length |= bFrame1Add_Length[7];

                    //bFrame1Data = new byte[Length];

                    //for (int i = 0; i < bFrame1Data.Length; i++)
                    //    bFrame1Data[i] = (byte)VBLReader.ReadByte();

                    //두바이트는 데이터가 아니라 체크섬인데 안보냄
                    //VBLReader.ReadByte();
                    //VBLReader.ReadByte();                    

                    //주소 4byte와 길이4byte
                    for (int i = 0; i < 8; i++)
                        bFrame3Add_Length[i] = (byte)VBLReader.ReadByte();

                    //bFrame3 4~7바이트가 길이 바이트 오더가 역순 최상위 상위 하위 최하위
                    uint Length = bFrame3Add_Length[4];
                    Length <<= 8;
                    Length |= bFrame3Add_Length[5];
                    Length <<= 8;
                    Length |= bFrame3Add_Length[6];
                    Length <<= 8;
                    Length |= bFrame3Add_Length[7];

                    bFrame3Data = new byte[Length];
                    for (int i = 0; i < bFrame3Data.Length; i++)
                        bFrame3Data[i] = (byte)VBLReader.ReadByte();

                    //두바이트는 데이터가 아니라 체크섬인데 안보냄
                    VBLReader.ReadByte();
                    VBLReader.ReadByte();

                    //주소 4byte와 길이4byte
                    for (int i = 0; i < 8; i++)
                        bFrame2Add_Length[i] = (byte)VBLReader.ReadByte();

                    //bFrame2 4~7바이트가 길이 바이트 오더가 역순 최상위 상위 하위 최하위
                    Length = bFrame2Add_Length[4];
                    Length <<= 8;
                    Length |= bFrame2Add_Length[5];
                    Length <<= 8;
                    Length |= bFrame2Add_Length[6];
                    Length <<= 8;
                    Length |= bFrame2Add_Length[7];

                    bFrame2Data = new byte[Length];

                    for (int i = 0; i < bFrame2Data.Length; i++)
                        bFrame2Data[i] = (byte)VBLReader.ReadByte();

                    //다운로드할거 다 읽었음
                }
            }
            catch (Exception ex)
            {   //파일 로드 에러
                return -1;
            }
            #endregion
            //this.Open_Extended_EOL_Mode();
            //앞으로 이동
            //파일 전송전 준비1
            //BECM Diag Session 추가
            //0x735, 8, new byte[] { 0x02, 0x10, 0x02, 0, 0, 0, 0, 0 });

            var rList = new List<string>();
            if (!GetToDID_singleData(0x02, 0x00, "50 ", false, out rList, 0x02, 0x10))
            {
                bool failcheck = true;
                for (int i = 0; i < 9; i++)
                {
                    Thread.Sleep(500);
                    //응답 7EC 06 50 02 00 19 01 F4 00 - in code, 635
                    rList = new List<string>();
                    if (GetToDID_singleData(0x02, 0x00, "50 ", false, out rList, 0x02, 0x10))
                    {
                        failcheck = false;
                        break;
                    }
                }

                if (failcheck)
                    return -1;                                
            }


            //141222 추가한 메시지 팩쓰기할동안은 차단
            Hybrid_Instru_CAN.IsSendMsg = false;
            Hybrid_Instru_CAN.ExtendedMsg = false;
            System.Threading.Thread.Sleep(500);

            //파일 전송전 준비2
            //BECM Security SEED 해독
            //0x735, 8 ,new byte[] { 0x02, 0x27, 0x01, 0, 0, 0, 0, 0});

            rList = new List<string>();
            if (!GetToDID_singleData(0x01, 0x00, "67 ", false, out rList, 0x02, 0x27))
            {
                return -1;
            }

            System.Threading.Thread.Sleep(500);
            
            //시드키 받아서 uint 형태로 저장

            //hex string to hex byte

            var tstr = rList[2] + rList[3] + rList[4];
            var recv = Enumerable.Range(0, tstr.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(tstr.Substring(x, 2), 16))
                             .ToArray();

            uint seedkey = recv[2];
            seedkey <<= 8;
            seedkey |= recv[1];
            seedkey <<= 8;
            seedkey |= recv[0];

            //받은 시드키로 해독키 생성
            uint returnkey = calc_key(seedkey);

            //키 데이터 바이트 배열로 저장
            byte[] keydata = new byte[3];

            keydata[0] = (byte)((returnkey >> 16) & 0xff);
            keydata[1] = (byte)((returnkey >> 8) & 0xff);
            keydata[2] = (byte)(returnkey & 0xff);
            //0x735, 8 ,new byte[] { 0x5, 0x27, 0x2, keydata[0], keydata[1], keydata[2], 0, 0 });

            rList = new List<string>();
            if (!GetToDID_singleData(0x02, keydata[0], "67 ", false, out rList, 0x05, 0x27,keydata[1],keydata[2]))
            {
                return -1;
            }
            
            //bFrame3 전송
            //먼저 주소 길이 전송함
            //헤더 0x1z zz z zz는 전송할 길이
            //3은 명령어 34 00 44 길이
            ushort len = (ushort)(3 + bFrame3Add_Length.Length);
            //헤더 만들기
            len |= 0x1000;

            //7e4 8 len상위 len하위 34 00 44 bFrame3Add_Length[0] bFrame3Add_Length[1] bFrame3Add_Length[2]

            rList = new List<string>();
            if (GetToDID_singleData(0x34, 0x10, "30 ", true, out rList, (byte)(len >> 8), (byte)(len & 0xff), 0x44, bFrame3Add_Length[0], bFrame3Add_Length[1], bFrame3Add_Length[2]))
            {
                byte[] senddata = new byte[8];

                senddata[0] = 0x21;
                for (int i = 3, j = 1; i < bFrame3Add_Length.Length; i++)
                {
                    senddata[j++] = bFrame3Add_Length[i];
                }

                rList = new List<string>();
                if (!GetToDID_singleData(senddata[2],senddata[3], "20 ", false, out rList, senddata[0],senddata[1],senddata[4],senddata[5],senddata[6],senddata[7]))
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
            
            //3601~3606까지 전송해야함
            ////bFrame3Data 전송
            ////먼저 주소 길이 전송함
            ////헤더 0x1z zz z zz는 전송할 길이
            ////2는 명령어 36 01 
            //c62는 위에서 받은 응답으로 안 값임
            //ushort MaxSend = 0xc62 - 2; //-2는 명령어오버헤드 //한번에 보낼수 있는 최대 데이터
            ushort MaxSend = 0x802 - 2;
            ushort SendDataCount = 0;
            //
            ushort Blockindex = 1;

            ushort SendLength = 0;
            ushort PacketNo = 1;

            byte[] sdata;

            int CanMsgLength = 0;

            CanMsg[] tempCanMSG;

            while (bFrame3Data.Length > SendDataCount)
            {
                SendLength = 0;
                PacketNo = 1;
                if (bFrame3Data.Length - SendDataCount >= MaxSend)
                    len = (ushort)(MaxSend + 2);
                else
                    len = (ushort)(bFrame3Data.Length - SendDataCount + 2);
                //len = (ushort)(2 + bFrame3Data.Length);
                //헤더 만들기
                len |= 0x1000;

                sdata = new byte[8];

                sdata[0] = (byte)(len >> 8);
                sdata[1] = (byte)(len & 0xff);
                sdata[2] = 0x36;
                sdata[3] = (byte)Blockindex++;

                sdata[4] = bFrame3Data[SendDataCount + SendLength++];
                sdata[5] = bFrame3Data[SendDataCount + SendLength++];
                sdata[6] = bFrame3Data[SendDataCount + SendLength++];
                sdata[7] = bFrame3Data[SendDataCount + SendLength++];

                CanMsgLength = ((len & 0xfff) - 6) / 7;
                if ((((len & 0xfff) - 6) % 7) > 0)
                {
                    CanMsgLength++;
                }

                //
                uint sendbyte = 2;
                tempCanMSG = new CanMsg[CanMsgLength];
                for (int i = 0; i < CanMsgLength; i++)
                {
                    tempCanMSG[i] = new CanMsg();
                    tempCanMSG[i].id = 0x735;
                    tempCanMSG[i].length = 8;
                    tempCanMSG[i].data[0] = (byte)((PacketNo & 0x0f) | 0x20);
                    PacketNo++;
                    for (int j = 1; j < 8; j++)
                    {
                        if (!(SendLength >= (len & 0xfff) - 2))
                            tempCanMSG[i].data[j] = bFrame3Data[SendDataCount + SendLength++];
                    }
                }

                rList = new List<string>();
                if (!GetToDID_singleData(sdata[2], sdata[3], "3", false, out rList, sdata[0], sdata[1], sdata[4], sdata[5], sdata[6], sdata[7]))
                {
                    return -1;
                }
                
                //벡터사 버그임 256개 이상 메시지는 txque를 아무리 늘려도 제대로 전송못함
                //x놈의 벡터 이거때문에 일주일을 날려 먹음
                //구버젼 CanCaseXL은 잘됨, 신버젼 VN1630은 나눠서 보내야함
                //SendCanData(tempCanMSG);
                //SendDataCount += SendLength;
                //System.Threading.Thread.Sleep(1);




                //if (tempCanMSG.Length > 250)
                //{
                //    CanMsg[] temp1 = new CanMsg[250];
                //    CanMsg[] temp2 = new CanMsg[tempCanMSG.Length - 250];
                //    Array.Copy(tempCanMSG, temp1, 250);
                //    Array.Copy(tempCanMSG, 250, temp2, 0, tempCanMSG.Length - 250);
                //    SendCanData(temp1);
                //    SendCanData(temp2);
                //}
                //else
                //{
                //    SendCanData(tempCanMSG);
                //}
                var t = 3;

                Hybrid_Instru_CAN._635List.Clear();
                foreach(var cm in tempCanMSG)
                {
                    Hybrid_Instru_CAN.SendToCAN(cm);
                    Thread.Sleep(1);
                }
                
                SendDataCount += SendLength;

                Thread.Sleep(1000);
                if (Hybrid_Instru_CAN._635List.Count > 0 && Hybrid_Instru_CAN._635List[0].Contains("76 "))
                {                    
                    lock (_635_obj)
                    {
                        foreach (var fe009c in Hybrid_Instru_CAN._635List)
                        {
                            LogState(LogType.Success, "Recved data - " + fe009c);
                            var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 1; i < arr.Length; i++)
                            {
                                rList.Add(arr[i]);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Fail, "Recved data - " + fe009c);
                    }
                    return -1;
                }

            }
            Thread.Sleep(500);
            rList = new List<string>();
            if (!GetToDID_singleData2(0x0, 0x0, "77 ", false, out rList, 0x01, 0x37))
            {
                if (!GetToDID_singleData2(0x0, 0x0, "77 ", false, out rList, 0x01, 0x37))
                {
                    return -1;
                }                
            }


            //bFrame2 전송
            //먼저 주소 길이 전송함
            //헤더 0x1z zz z zz는 전송할 길이
            //3은 명령어 34 00 44 길이
            len = (ushort)(3 + bFrame2Add_Length.Length);
            //헤더 만들기
            len |= 0x1000;

            //7e4 8 len상위 len하위 34 00 44 bFrame2Add_Length[0] bFrame2Add_Length[1] bFrame2Add_Length[2]
            byte order = 0x21;


            rList = new List<string>();
            if (GetToDID_singleData(0x34, 0x10, "30 ", true, out rList, (byte)(len >> 8), (byte)(len & 0xff), 0x44, bFrame2Add_Length[0], bFrame2Add_Length[1], bFrame2Add_Length[2]))
            {
                byte[] senddata = new byte[8];
                senddata[0] = order++;
                for (int i = 3, j = 1; i < bFrame2Add_Length.Length; i++)
                {
                    senddata[j++] = bFrame2Add_Length[i];
                }

                rList = new List<string>();
                if (!GetToDID_singleData(senddata[2], senddata[3], "20 ", false, out rList, senddata[0], senddata[1], senddata[4], senddata[5], senddata[6], senddata[7]))
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }

            //bFrame2Data 전송
            //먼저 주소 길이 전송함
            //헤더 0x1z zz z zz는 전송할 길이
            //2는 명령어 36 01 
            len = (ushort)(2 + bFrame2Data.Length);
            //헤더 만들기
            len |= 0x1000;

            sdata = new byte[8];

            sdata[0] = (byte)(len >> 8);
            sdata[1] = (byte)(len & 0xff);
            sdata[2] = 0x36;
            sdata[3] = 0x01;

            SendLength = 0;

            sdata[4] = bFrame2Data[SendLength++];
            sdata[5] = bFrame2Data[SendLength++];
            sdata[6] = bFrame2Data[SendLength++];
            sdata[7] = bFrame2Data[SendLength++];

            CanMsgLength = (bFrame2Data.Length - 4) / 7;
            if (((bFrame2Data.Length - 4) % 7) > 0)
            {
                CanMsgLength++;
            }

            PacketNo = 1;
            tempCanMSG = new CanMsg[CanMsgLength];
            for (int i = 0; i < CanMsgLength; i++)
            {
                tempCanMSG[i] = new CanMsg();
                tempCanMSG[i].id = 0x735;
                tempCanMSG[i].length = 8;
                tempCanMSG[i].data[0] = (byte)((PacketNo & 0x0f) | 0x20);
                PacketNo++;
                for (int j = 1; j < 8; j++)
                {
                    if (!(SendLength >= bFrame2Data.Length))
                        tempCanMSG[i].data[j] = bFrame2Data[SendLength++];
                }
            }

            rList = new List<string>();
            if (!GetToDID_singleData(sdata[2], sdata[3], "30 ", true, out rList, sdata[0], sdata[1], sdata[4], sdata[5], sdata[6], sdata[7]))
            {
                return -1;
            }


            Hybrid_Instru_CAN._635List.Clear();
            foreach (var cm in tempCanMSG)
            {
                Hybrid_Instru_CAN.SendToCAN(cm);
                Thread.Sleep(1);
            }

            SendDataCount += SendLength;

            Thread.Sleep(70);
            if (Hybrid_Instru_CAN._635List.Count > 0 && Hybrid_Instru_CAN._635List[0].Contains("76 "))
            {
                lock (_635_obj)
                {
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                        var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 1; i < arr.Length; i++)
                        {
                            rList.Add(arr[i]);
                        }
                    }
                }
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
                return -1;
            }

            Thread.Sleep(300);
            rList = new List<string>();
            if (!GetToDID_singleData(0x0, 0x0, "77 ", false, out rList, 0x01, 0x37))
            {
                if (!GetToDID_singleData(0x0, 0x0, "77 ", false, out rList, 0x01, 0x37))
                {
                    return -1;
                }
            }
            


            rList = new List<string>();
            if (!GetToDID_singleData(0x31, 0x01, "30 ", false, out rList, 0x10, 0x08,0x03,0x01,0x40))
            {
                return -1;
            }

            rList = new List<string>();
            if (!GetToDID_singleData2(0x0, 0x0, "71 ", false, out rList, 0x21, 0x30))
            {
                if (!GetToDID_singleData(0x0, 0x0, "71 ", false, out rList, 0x21, 0x30))
                {
                    return -1;
                }
            }
            
            //기존 아이디 읽기
            byte[] RcvID = new byte[4];

             rList = new List<string>();
             if (GetToDID_singleData(0xF1, 0x8C, "62 ", false, out rList))
             {
                 if (rList[3] == "FF" && rList[4] == "FF" && rList[5] == "FF" && rList[6] == "FF")
                 {
                 }
                 else
                 {
                     return 0;
                 }
             }
            
            Thread.Sleep(1000);

             rList = new List<string>();
             if (!GetToDID_singleData(0xF1, 0x8C, "6E ", false, out rList, 0x07, 0x2e, WritePackID[0], WritePackID[1], WritePackID[2], WritePackID[3]))
             {
                 return -1;
             }
             return 0;
        }


        //Pack 플래시시 시큐러티 시드 해독 키 발생함수
        private uint calc_key(uint seed)
        {
            uint chhb, chlb, A_reg, B_reg;
            uint R_bytes;
            UInt16 i;

            chhb = 0xffffffff;
            chlb = 0xff000000 | seed;

            A_reg = 0x00c541a9;

            for (i = 0; i < 32; i++)
            {
                B_reg = (chlb % 2) ^ (A_reg % 2);
                chlb >>= 1;
                A_reg >>= 1;
                if (B_reg != 0)
                {
                    B_reg <<= 23;
                    B_reg |= A_reg;
                    B_reg ^= 0x109028;
                    A_reg = B_reg;
                }
            }

            for (i = 0; i < 32; i++)
            {
                B_reg = (chhb % 2) ^ (A_reg % 2);
                chhb >>= 1;
                A_reg >>= 1;
                if (B_reg != 0)
                {
                    B_reg <<= 23;
                    B_reg |= A_reg;
                    B_reg ^= 0x109028;
                    A_reg = B_reg;
                }
            }

            R_bytes = (A_reg << 12) & 0xff0000;
            R_bytes |= (A_reg & 0xf000) | ((A_reg >> 12) & 0xf00);
            R_bytes |= ((A_reg << 4) & 0xf0) | ((A_reg >> 16) & 0x0f);
            R_bytes &= 0x00FFFFFF;

            return R_bytes;
        }

        private void StopKeepEOLMode()
        {
            throw new NotImplementedException();
        }

        private void StopkeepBMSOnMessage()
        {
            throw new NotImplementedException();
        }

        private void SendCanData(uint _SEND_ID, int p1, byte[] p2)
        {
            throw new NotImplementedException();
        }

        private int ReceiveData(out uint iReceiveID, out byte[] recv, int p, object _RECEIVE_ID)
        {
            throw new NotImplementedException();
        }

        private void SendCanData(int id, int length, byte[] byteArr)
        {
            Hybrid_Instru_CAN.SendToCAN(id.ToString(), byteArr, length);
        }

        #endregion Flash PACK ID
    }
}