using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {

        public bool BalanceSwitchShort(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            cmc.b_isRecieveOk = true;
            cmc.CMC_Short();
            Thread.Sleep(2000);

            ti.refValues_.Clear();
            int ocnt = 0;
            string ostr = "";

            if (cmc.b_isRecieveOk == true)
            {
                if (cmc.CB1_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB1_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                if (cmc.CB2_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB2_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                if (cmc.CB3_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB3_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                if (cmc.CB4_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB4_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                if (cmc.CB5_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB5_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                if (cmc.CB6_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB6_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };

                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_BAL__ST_SWITCH, ostr.ToString()));
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_BAL__ST_SWITCH, ostr.ToString()));
                }
                //210312 WJS ADD PORS FL
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_BAL__ST_SWITCH, ostr.ToString()));
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_BAL__ST_SWITCH, ostr.ToString()));
                }
                LogState(LogType.Info, "balanceSwitch_short :" + ostr.ToString());// + "balanceSwitch_short :" + scnt.ToString());

                ti.Value_ = ocnt;// +scnt;   
            }
            else
            {
                int nNGRetryCount = 3;

                for (int i = 0; i < nNGRetryCount; ++i)
                {
                    LogState(LogType.Info, "BalanceSwitchShort : FAIL! CMC RECV.. RETRY");

                    Reboot_CMC();
                    //Thread.Sleep(500);
                    
                    //cmc.CMC_WakeUp();
                    //Thread.Sleep(CMC_WakeUp_after_sleep);

                    cmc.b_isRecieveOk = true;

                    cmc.CMC_SEND_CMD(CMC_COMMAND.SM_RESET);
                    Thread.Sleep(3000);

                    cmc.CMC_Short();
                    Thread.Sleep(2000);

                    ti.Value_ = null;
                    ti.refValues_.Clear();

                    ocnt = 0;
                    ostr = "";

                    if (cmc.b_isRecieveOk == true)
                    {
                        if (cmc.CB1_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB1_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                        if (cmc.CB2_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB2_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                        if (cmc.CB3_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB3_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                        if (cmc.CB4_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB4_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                        if (cmc.CB5_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB5_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };
                        if (cmc.CB6_SHORT_FLT) { ocnt += 1; LogState(LogType.Info, "CB6_SHORT_FLT:1"); ostr += "1"; } else { ostr += "0"; };

                        //201217 wjs add maserati
                        if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_BAL__ST_SWITCH, ostr.ToString()));
                        }
                        else if (localTypes == ModelType.MASERATI_NORMAL)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_BAL__ST_SWITCH, ostr.ToString()));
                        }
                        //210312 WJS ADD PORS FL
                        else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                        {
                            //210611 확인필요!!
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_BAL__ST_SWITCH, ostr.ToString()));
                        }
                        //221101 wjs add mase m183
                        else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_BAL__ST_SWITCH, ostr.ToString()));
                        }
                        LogState(LogType.Info, "balanceSwitch_short :" + ostr.ToString());// + "balanceSwitch_short :" + scnt.ToString());

                        ti.Value_ = ocnt;// +scnt;

                        break;
                    }

                    Thread.Sleep(1);
                }
            }

            return JudgementTestItem(ti);
        }

        public bool Plausibility_Check(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var pcb_Temp = 0.0;
            cmc.CalculateToMC33772PCB(CMClist[14], out pcb_Temp);

            var ict = cmc.IC_TEMP;
            var ic_Temp = (ict * 0.032) - 273.15;

            LogState(LogType.Info, string.Format("( {0} * 0.032 ) - 273.15 = {1}", ict, ic_Temp));


            ti.Value_ = Math.Abs(pcb_Temp - ic_Temp);
            LogState(LogType.Info, string.Format("Math.Abs( {0} - {1} ) = {2}", pcb_Temp, ic_Temp, ti.Value_));

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_IC_TEMP_TSHORT, ic_Temp.ToString() + "&&" + pcb_Temp.ToString()));                
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_IC_TEMP_TSHORT, ic_Temp.ToString() + "&&" + pcb_Temp.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_IC_TEMP_TSHORT, ic_Temp.ToString() + "&&" + pcb_Temp.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_IC_TEMP_TSHORT, ic_Temp.ToString() + "&&" + pcb_Temp.ToString()));
            }
            return JudgementTestItem(ti);
        }

        #region SM01

        public bool Cell_Voltage_OV_UV_Cell_1_3_5_UV_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            

            //cmc.CheckSM();

            //ti.Value_ = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM01_a_O_UV) ? 1 : 0;

            var list = cmc.i0076_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[0]) { ocnt += 1; LogState(LogType.Info, "ODD_UV_1:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_UV_1:0"); ostr += "0"; };
                if (list[1]) { ocnt += 1; LogState(LogType.Info, "ODD_UV_2:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_UV_2:0"); ostr += "0"; };
                if (list[2]) { ocnt += 1; LogState(LogType.Info, "ODD_UV_3:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_UV_3:0"); ostr += "0"; };
                if (list[3]) { ocnt += 1; LogState(LogType.Info, "ODD_UV_4:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_UV_4:0"); ostr += "0"; };
                if (list[4]) { ocnt += 1; LogState(LogType.Info, "ODD_UV_5:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_UV_5:0"); ostr += "0"; };
                if (list[5]) { ocnt += 1; LogState(LogType.Info, "ODD_UV_6:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_UV_6:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                ti.Value_ = null;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_OV_UV_Cell_2_4_6_OV_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var list = cmc.i0075_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[0]) { ocnt += 1; LogState(LogType.Info, "ODD_OV_1:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_OV_1:0"); ostr += "0"; };
                if (list[1]) { ocnt += 1; LogState(LogType.Info, "ODD_OV_2:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_OV_2:0"); ostr += "0"; };
                if (list[2]) { ocnt += 1; LogState(LogType.Info, "ODD_OV_3:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_OV_3:0"); ostr += "0"; };
                if (list[3]) { ocnt += 1; LogState(LogType.Info, "ODD_OV_4:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_OV_4:0"); ostr += "0"; };
                if (list[4]) { ocnt += 1; LogState(LogType.Info, "ODD_OV_5:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_OV_5:0"); ostr += "0"; };
                if (list[5]) { ocnt += 1; LogState(LogType.Info, "ODD_OV_6:1"); ostr += "1"; } else { LogState(LogType.Info, "ODD_OV_6:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                ti.Value_ = null;
            }


            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_OV_UV_Cell_2_4_6_UV_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var list = cmc.i0074_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[0]) { ocnt += 1; LogState(LogType.Info, "EVEN_UV_1:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_UV_1:0"); ostr += "0"; };
                if (list[1]) { ocnt += 1; LogState(LogType.Info, "EVEN_UV_2:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_UV_2:0"); ostr += "0"; };
                if (list[2]) { ocnt += 1; LogState(LogType.Info, "EVEN_UV_3:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_UV_3:0"); ostr += "0"; };
                if (list[3]) { ocnt += 1; LogState(LogType.Info, "EVEN_UV_4:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_UV_4:0"); ostr += "0"; };
                if (list[4]) { ocnt += 1; LogState(LogType.Info, "EVEN_UV_5:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_UV_5:0"); ostr += "0"; };
                if (list[5]) { ocnt += 1; LogState(LogType.Info, "EVEN_UV_6:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_UV_6:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                ti.Value_ = null;
            }


            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_OV_UV_Cell_1_3_5_OV_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            cmc.i0073_data.Clear();
            cmc.i0074_data.Clear();
            cmc.i0075_data.Clear();
            cmc.i0076_data.Clear();

            cmc.CMC_SEND_CMD(CMC_COMMAND.SM01);
            Thread.Sleep(1000);

            var list = cmc.i0073_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[0]) { ocnt += 1; LogState(LogType.Info, "EVEN_OV_1:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_OV_1:0"); ostr += "0"; };
                if (list[1]) { ocnt += 1; LogState(LogType.Info, "EVEN_OV_2:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_OV_2:0"); ostr += "0"; };
                if (list[2]) { ocnt += 1; LogState(LogType.Info, "EVEN_OV_3:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_OV_3:0"); ostr += "0"; };
                if (list[3]) { ocnt += 1; LogState(LogType.Info, "EVEN_OV_4:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_OV_4:0"); ostr += "0"; };
                if (list[4]) { ocnt += 1; LogState(LogType.Info, "EVEN_OV_5:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_OV_5:0"); ostr += "0"; };
                if (list[5]) { ocnt += 1; LogState(LogType.Info, "EVEN_OV_6:1"); ostr += "1"; } else { LogState(LogType.Info, "EVEN_OV_6:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                ti.Value_ = null;
            }


            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_OV_UV_All_Off_UV_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            var evenUV = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM01_a_E_UV);
            var oddUV = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM01_a_O_UV);

            if (evenUV == 0 && oddUV == 0)
                ti.Value_ = 0;
            else
                ti.Value_ = 1;

            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_OV_UV_All_Off_OV_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            var evenOV = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM01_a_E_OV);
            var oddOV = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM01_a_O_OV);

            if (evenOV == 0 && oddOV == 0)
                ti.Value_ = 0;
            else
                ti.Value_ = 1;

            return JudgementTestItem(ti);
        }

        #endregion

        #region SM03

         double MEAS_CELL1 = 0.0;
         double MEAS_CELL2=0.0;
         double MEAS_CELL3=0.0;
         double MEAS_CELL4=0.0;
         double MEAS_CELL5=0.0;
         double MEAS_CELL6=0.0;




        public bool Cell_Voltage_Verify_Cell_3_4_Check(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region before
            /*
            //cmc.CMC_SEND_CMD(CMC_COMMAND.SM03);

            //Thread.Sleep(50);

            //ti.Value_ = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM03);
            MEAS_CELL1 = 0.0;
            MEAS_CELL2 = 0.0;
            MEAS_CELL3 = 0.0;
            MEAS_CELL4 = 0.0;
            MEAS_CELL5 = 0.0;
            MEAS_CELL6 = 0.0;

            MEAS_CELL1 = CMClist[1] * 0.0001525925;
            MEAS_CELL2 = CMClist[2] * 0.0001525925;
            MEAS_CELL3 = CMClist[3] * 0.0001525925;
            MEAS_CELL4 = CMClist[4] * 0.0001525925;
            MEAS_CELL5 = CMClist[5] * 0.0001525925;
            MEAS_CELL6 = CMClist[6] * 0.0001525925;

            //V_err_x = (MEAS_CELL3 + MEAS_CELL4) ÷ 2 - (MEAS_CELL1 + MEAS_CELL2) ÷ 2
            ti.Value_ = ((MEAS_CELL3 + MEAS_CELL4) / 2 - (MEAS_CELL1 + MEAS_CELL2) / 2) * 1000;

            LogState(LogType.Info, string.Format("(({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000 = {4}",
                MEAS_CELL3, MEAS_CELL4, MEAS_CELL1, MEAS_CELL2, ti.Value_));

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_VOL_VERFY, string.Format("{0}&{1}&{2}&{3}&{4}&{5}", MEAS_CELL1, MEAS_CELL2, MEAS_CELL3, MEAS_CELL4, MEAS_CELL5, MEAS_CELL6)));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CELL_VOL_VERFY, string.Format("{0}&{1}&{2}&{3}&{4}&{5}", MEAS_CELL1, MEAS_CELL2, MEAS_CELL3, MEAS_CELL4, MEAS_CELL5, MEAS_CELL6)));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                //210611 확인필요!!

                //if (pro_Type == ProgramType.EOLInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_BAL__OP_SWITCH, ostr.ToString()));
                //else if (pro_Type == ProgramType.VoltageInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_BAL__OP_SWITCH, ostr.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210611 확인필요!!

                //if (pro_Type == ProgramType.EOLInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_BAL__OP_SWITCH, ostr.ToString()));
                //else if (pro_Type == ProgramType.VoltageInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_BAL__OP_SWITCH, ostr.ToString()));
            }
            */
            #endregion

            #region After 210807 Try회수 추가, NG시 CMC Reboot추가, 계산식 수정

            //Pack1동은 셀전압 계산할때 이미 0.0001525925 값을 사용하고 있었음
            //어쨌거나 저쨌거나 Pack1동 m4,6,8 내용 동일하게 적용(포르쉐 노멀,미러만 해당)

            ti.Value_ = null;

            ti.refValues_.Clear();

            //if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)  //20210808_ljs
            //{
                const int nTryCount = 2;

                for (int i = 0; i < nTryCount; ++i)
                {
                    LogState(LogType.Info, "Try Voltage_Verify..");

                    ti.Value_ = null;
                    ti.refValues_.Clear();

                    cmc.raw_cmcList.Remove("0087");
                    cmc.raw_cmcList.Remove("0088");
                    cmc.raw_cmcList.Remove("0089");
                    cmc.raw_cmcList.Remove("008A");
                    cmc.raw_cmcList.Remove("008B");
                    cmc.raw_cmcList.Remove("008C");

                    cmc.CV1_VRFY = MEAS_CELL1 = 0.0;
                    cmc.CV2_VRFY = MEAS_CELL2 = 0.0;
                    cmc.CV3_VRFY = MEAS_CELL3 = 0.0;
                    cmc.CV4_VRFY = MEAS_CELL4 = 0.0;
                    cmc.CV5_VRFY = MEAS_CELL5 = 0.0;
                    cmc.CV6_VRFY = MEAS_CELL6 = 0.0;

                    var targetCMD = CMC_COMMAND.SM03_a;
                    var addDelay = 0;

                    switch (targetCMD)
                    {
                        case CMC_COMMAND.SM03_a: addDelay = 5; break;
                        case CMC_COMMAND.SM03_b: addDelay = 30; break;
                        case CMC_COMMAND.SM03_c: addDelay = 70; break;
                        case CMC_COMMAND.SM03_d: addDelay = 100; break;
                    }

                    cmc.CMC_SEND_CMD(targetCMD);

                    Thread.Sleep(20 + addDelay);

                    int nTimeCount = 0;

                    //6개 다 들어오면 탈출, 5초안에 안들어오면 리턴
                    while (cmc.CV1_VRFY == 0.0 || cmc.CV2_VRFY == 0.0 ||
                           cmc.CV3_VRFY == 0.0 || cmc.CV4_VRFY == 0.0 ||
                           cmc.CV5_VRFY == 0.0 || cmc.CV6_VRFY == 0.0)
                    {

                        Thread.Sleep(10);

                        nTimeCount += 10;

                        if (nTimeCount > 5000)
                        {
                            LogState(LogType.Fail, ti.Name + " - Timeout(5sec)");

                            ti.Value_ = null;

                            return JudgementTestItem(ti);
                        }
                    }

                    MEAS_CELL1 = cmc.CV1_VRFY;  // ADC
                    MEAS_CELL2 = cmc.CV2_VRFY;  // ADC
                    MEAS_CELL3 = cmc.CV3_VRFY;  // ADC
                    MEAS_CELL4 = cmc.CV4_VRFY;  // ADC
                    MEAS_CELL5 = cmc.CV5_VRFY;  // ADC
                    MEAS_CELL6 = cmc.CV6_VRFY;  // ADC

                    string strDebug;
                    strDebug = string.Empty;
                    strDebug = string.Format("MEAS_CELL1:{0} MEAS_CELL2:{1} MEAS_CELL3:{2} MEAS_CELL4:{3} MEAS_CELL5:{4} MEAS_CELL6:{5}"
                        , MEAS_CELL1
                        , MEAS_CELL2
                        , MEAS_CELL3
                        , MEAS_CELL4
                        , MEAS_CELL5
                        , MEAS_CELL6);
                    LogState(LogType.Info, strDebug.ToString());

                    const double dCalcValue = 0.0001525925;
                    MEAS_CELL1 *= dCalcValue;  // Cell Voltage
                    MEAS_CELL2 *= dCalcValue;  // Cell Voltage
                    MEAS_CELL3 *= dCalcValue;  // Cell Voltage
                    MEAS_CELL4 *= dCalcValue;  // Cell Voltage
                    MEAS_CELL5 *= dCalcValue;  // Cell Voltage
                    MEAS_CELL6 *= dCalcValue;  // Cell Voltage

                    strDebug = string.Empty;
                    strDebug = string.Format("CalcVal{0} / MEAS_CELL1:{1} MEAS_CELL2:{2} MEAS_CELL3:{3} MEAS_CELL4:{4} MEAS_CELL5:{5} MEAS_CELL6:{6}"
                        , dCalcValue
                        , MEAS_CELL1
                        , MEAS_CELL2
                        , MEAS_CELL3
                        , MEAS_CELL4
                        , MEAS_CELL5
                        , MEAS_CELL6);
                    LogState(LogType.Info, strDebug.ToString());

                    cmc.raw_cmcList.Remove("0087");
                    cmc.raw_cmcList.Remove("0088");
                    cmc.raw_cmcList.Remove("0089");
                    cmc.raw_cmcList.Remove("008A");
                    cmc.raw_cmcList.Remove("008B");
                    cmc.raw_cmcList.Remove("008C");

                    ti.Value_ = (((MEAS_CELL3 + MEAS_CELL4) / 2 - (MEAS_CELL1 + MEAS_CELL2) / 2) * 1000)/*.ToString("F1")*/; //2공장은 판정시 소수점자름(eollist)

                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_VOL_VERFY, string.Format("{0}&{1}&{2}&{3}&{4}&{5}"
                            , MEAS_CELL1, MEAS_CELL2, MEAS_CELL3, MEAS_CELL4, MEAS_CELL5, MEAS_CELL6)));

                        LogState(LogType.Info, string.Format("3~4 : (({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000 = {4}"
                            , MEAS_CELL3
                            , MEAS_CELL4
                            , MEAS_CELL1
                            , MEAS_CELL2
                            , ti.Value_));
                    }
                }
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CELL_VOL_VERFY, string.Format("{0}&{1}&{2}&{3}&{4}&{5}"
                            , MEAS_CELL1, MEAS_CELL2, MEAS_CELL3, MEAS_CELL4, MEAS_CELL5, MEAS_CELL6)));

                        LogState(LogType.Info, string.Format("3~4 : (({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000 = {4}"
                            , MEAS_CELL3
                            , MEAS_CELL4
                            , MEAS_CELL1
                            , MEAS_CELL2
                            , ti.Value_));
                    }
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CELL_VOL_VERFY, string.Format("{0}&{1}&{2}&{3}&{4}&{5}"
                            , MEAS_CELL1, MEAS_CELL2, MEAS_CELL3, MEAS_CELL4, MEAS_CELL5, MEAS_CELL6)));

                        LogState(LogType.Info, string.Format("3~4 : (({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000 = {4}"
                            , MEAS_CELL3
                            , MEAS_CELL4
                            , MEAS_CELL1
                            , MEAS_CELL2
                            , ti.Value_));
                    }
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CELL_VOL_VERFY, string.Format("{0}&{1}&{2}&{3}&{4}&{5}"
                            , MEAS_CELL1, MEAS_CELL2, MEAS_CELL3, MEAS_CELL4, MEAS_CELL5, MEAS_CELL6)));

                        LogState(LogType.Info, string.Format("3~4 : (({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000 = {4}"
                            , MEAS_CELL3
                            , MEAS_CELL4
                            , MEAS_CELL1
                            , MEAS_CELL2
                            , ti.Value_));
                    }
                }

                // NG를 대비한 재시도를 위해 5~6 시험항목 판정을 여기서 체크만 한다

                TestItem tItem5_6 = null;

                    foreach (var item in viewModel.TestItemList)
                    {
                        if (item.Key == "Cell Voltage Verify_Cell #5/6 Check")
                        {
                            tItem5_6 = item.Value;

                            break;
                        }
                    }

                    if(tItem5_6 == null)
                    {
                        LogState(LogType.Fail, "Test Item(Cell Voltage Verify_Cell #5/6 Check) is Null");

                        return false;
                    }

                    tItem5_6.Value_ = (((MEAS_CELL5 + MEAS_CELL6) / 2 - (MEAS_CELL1 + MEAS_CELL2) / 2) * 1000).ToString("F1");

                    LogState(LogType.Info, string.Format("5~6 : (({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000  = {4}"
                        , MEAS_CELL5
                        , MEAS_CELL6
                        , MEAS_CELL1
                        , MEAS_CELL2
                        , tItem5_6.Value_));

                    bool bJudgeCheck3_4 = false;
                    bool bJudgeCheck5_6 = false;

                    bJudgeCheck3_4 = JudgementTestItem(ti);
                    bJudgeCheck5_6 = JudgementTestItem(tItem5_6);

                    LogState(LogType.Info, string.Format("bJudgeCheck3_4:{0} bJudgeCheck5_6:{1})"
                        , bJudgeCheck3_4
                        , bJudgeCheck5_6));

                    if (bJudgeCheck3_4 == false || bJudgeCheck5_6 == false)
                    {
                        //1개라도 NG시 CMC Reboot, Wake Up 이후 재시도

                        if (i == nTryCount - 1)
                        {
                            break;
                        }

                        if (Reboot_CMC() == true)
                        {
                            //cmc.CMC_WakeUp();
                            //Thread.Sleep(CMC_WakeUp_after_sleep);
                            //cmc.CMC_V4328();
                            //Thread.Sleep(CMC_V4328_after_sleep);
                        }
                        else
                        {
                            ti.Value_ = "CMC_OPEN_FAIL";
                            ti.Result = "NG";

                            LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                            if (isSkipNG_)
                            {
                                return true;
                            }

                            return false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            //}
            //else { } // 20210808_ljs 필요시 추가

            #endregion

            return JudgementTestItem(ti);
        }

        public bool Cell_Voltage_Verify_Cell_5_6_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            //cmc.CMC_SEND_CMD(CMC_COMMAND.SM04_2);

            //Thread.Sleep(50);

            //ti.Value_ = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM04_b);

            //V_err_x = (MEAS_CELL5 + MEAS_CELL6) ÷ 2 - (MEAS_CELL1 + MEAS_CELL2) ÷ 2
            ti.Value_ = ((MEAS_CELL5 + MEAS_CELL6) / 2 - (MEAS_CELL1 + MEAS_CELL2) / 2) * 1000;

            LogState(LogType.Info, string.Format("(({0} + {1}) / 2 - ({2} + {3}) / 2) * 1000  = {4}",
                MEAS_CELL5, MEAS_CELL6, MEAS_CELL1, MEAS_CELL2, ti.Value_));

            return JudgementTestItem(ti);
        }

        #endregion

        #region SM04

        double CV1_LEAK_POL_0=0.0;
        double CV2_LEAK_POL_0=0.0;
        double CV3_LEAK_POL_0=0.0;
        double CV4_LEAK_POL_0=0.0;
        double CV5_LEAK_POL_0=0.0;
        double CV6_LEAK_POL_0=0.0;
        double STK_LEAK_POL_0=0.0; 
        double CV1_LEAK_POL_1=0.0;
        double CV2_LEAK_POL_1=0.0;
        double CV3_LEAK_POL_1=0.0;
        double CV4_LEAK_POL_1=0.0;
        double CV5_LEAK_POL_1=0.0;
        double CV6_LEAK_POL_1=0.0;
        double STK_LEAK_POL_1 = 0.0;

        public bool Cell_Terminal_Leakage_Dectection_1(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region logs
            cmc.raw_cmcList.Remove("0079");
            cmc.raw_cmcList.Remove("007A");
            cmc.raw_cmcList.Remove("007B");
            cmc.raw_cmcList.Remove("007C");
            cmc.raw_cmcList.Remove("007D");
            cmc.raw_cmcList.Remove("007E");
            cmc.raw_cmcList.Remove("007F");
            cmc.raw_cmcList.Remove("0080");
            cmc.raw_cmcList.Remove("0081");
            cmc.raw_cmcList.Remove("0082");
            cmc.raw_cmcList.Remove("0083");
            cmc.raw_cmcList.Remove("0084");
            cmc.raw_cmcList.Remove("0085");
            cmc.raw_cmcList.Remove("0086");

            LogState(LogType.Info, string.Format("BEF DATA CV1~STK_LEAK_POL0:{0}/{1}/{2}/{3}/{4}/{5}/{6}",
                cmc.CV1_LEAK_POL_0.ToString(),
                cmc.CV2_LEAK_POL_0.ToString(),
                cmc.CV3_LEAK_POL_0.ToString(),
                cmc.CV4_LEAK_POL_0.ToString(),
                cmc.CV5_LEAK_POL_0.ToString(),
                cmc.CV6_LEAK_POL_0.ToString(),
                cmc.STK_LEAK_POL_0.ToString()));
            LogState(LogType.Info, string.Format("BEF DATA CV1~STK_LEAK_POL1:{0}/{1}/{2}/{3}/{4}/{5}/{6}",
               cmc.CV1_LEAK_POL_1.ToString(),
               cmc.CV2_LEAK_POL_1.ToString(),
               cmc.CV3_LEAK_POL_1.ToString(),
               cmc.CV4_LEAK_POL_1.ToString(),
               cmc.CV5_LEAK_POL_1.ToString(),
               cmc.CV6_LEAK_POL_1.ToString(),
               cmc.STK_LEAK_POL_1.ToString()));
            #endregion

            CV1_LEAK_POL_0 = 0.0;
            CV2_LEAK_POL_0 = 0.0;
            CV3_LEAK_POL_0 = 0.0;
            CV4_LEAK_POL_0 = 0.0;
            CV5_LEAK_POL_0 = 0.0;
            CV6_LEAK_POL_0 = 0.0;
            STK_LEAK_POL_0 = 0.0;
            CV1_LEAK_POL_1 = 0.0;
            CV2_LEAK_POL_1 = 0.0;
            CV3_LEAK_POL_1 = 0.0;
            CV4_LEAK_POL_1 = 0.0;
            CV5_LEAK_POL_1 = 0.0;
            CV6_LEAK_POL_1 = 0.0;
            STK_LEAK_POL_1 = 0.0;

            cmc.CMC_SEND_CMD(CMC_COMMAND.SM04_1);

            int i = 0;
            while (i != 1000)
            { 
                LogState(LogType.Info, i.ToString() + string.Format(" DATA CV1~STK_LEAK_POL0:{0}/{1}/{2}/{3}/{4}/{5}/{6}",
                    cmc.CV1_LEAK_POL_0.ToString(),
                    cmc.CV2_LEAK_POL_0.ToString(),
                    cmc.CV3_LEAK_POL_0.ToString(),
                    cmc.CV4_LEAK_POL_0.ToString(),
                    cmc.CV5_LEAK_POL_0.ToString(),
                    cmc.CV6_LEAK_POL_0.ToString(),
                    cmc.STK_LEAK_POL_0.ToString()));

                LogState(LogType.Info, i.ToString() + string.Format(" DATA CV1~STK_LEAK_POL1:{0}/{1}/{2}/{3}/{4}/{5}/{6}",
                   cmc.CV1_LEAK_POL_1.ToString(),
                   cmc.CV2_LEAK_POL_1.ToString(),
                   cmc.CV3_LEAK_POL_1.ToString(),
                   cmc.CV4_LEAK_POL_1.ToString(),
                   cmc.CV5_LEAK_POL_1.ToString(),
                   cmc.CV6_LEAK_POL_1.ToString(),
                   cmc.STK_LEAK_POL_1.ToString()));

                CV1_LEAK_POL_0 = cmc.CV1_LEAK_POL_0 ;
                CV2_LEAK_POL_0 = cmc.CV2_LEAK_POL_0 ;
                CV3_LEAK_POL_0 = cmc.CV3_LEAK_POL_0 ;
                CV4_LEAK_POL_0 = cmc.CV4_LEAK_POL_0 ;
                CV5_LEAK_POL_0 = cmc.CV5_LEAK_POL_0 ;
                CV6_LEAK_POL_0 = cmc.CV6_LEAK_POL_0 ;
                STK_LEAK_POL_0 = cmc.STK_LEAK_POL_0 ;
                CV1_LEAK_POL_1 = cmc.CV1_LEAK_POL_1 ;
                CV2_LEAK_POL_1 = cmc.CV2_LEAK_POL_1 ;
                CV3_LEAK_POL_1 = cmc.CV3_LEAK_POL_1 ;
                CV4_LEAK_POL_1 = cmc.CV4_LEAK_POL_1 ;
                CV5_LEAK_POL_1 = cmc.CV5_LEAK_POL_1 ;
                CV6_LEAK_POL_1 = cmc.CV6_LEAK_POL_1 ;
                STK_LEAK_POL_1 = cmc.STK_LEAK_POL_1;

                Thread.Sleep(100);
                i += 100;
            }

            ti.Value_ = CV1_LEAK_POL_0 > CV1_LEAK_POL_1 ? CV1_LEAK_POL_0 : CV1_LEAK_POL_1;

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_TEMN_LK_1, 
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_0,
                        CV2_LEAK_POL_0,
                        CV3_LEAK_POL_0,
                        CV4_LEAK_POL_0,
                        CV5_LEAK_POL_0,
                        CV6_LEAK_POL_0,
                        STK_LEAK_POL_0
                        )));

                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_TEMN_LK_2,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_1,
                        CV2_LEAK_POL_1,
                        CV3_LEAK_POL_1,
                        CV4_LEAK_POL_1,
                        CV5_LEAK_POL_1,
                        CV6_LEAK_POL_1,
                        STK_LEAK_POL_1
                        )));
                }
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CELL_TEMN_LK_1,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_0,
                        CV2_LEAK_POL_0,
                        CV3_LEAK_POL_0,
                        CV4_LEAK_POL_0,
                        CV5_LEAK_POL_0,
                        CV6_LEAK_POL_0,
                        STK_LEAK_POL_0
                        )));

                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CELL_TEMN_LK_2,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_1,
                        CV2_LEAK_POL_1,
                        CV3_LEAK_POL_1,
                        CV4_LEAK_POL_1,
                        CV5_LEAK_POL_1,
                        CV6_LEAK_POL_1,
                        STK_LEAK_POL_1
                        )));
                }
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CELL_TEMN_LK_1,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_0,
                        CV2_LEAK_POL_0,
                        CV3_LEAK_POL_0,
                        CV4_LEAK_POL_0,
                        CV5_LEAK_POL_0,
                        CV6_LEAK_POL_0,
                        STK_LEAK_POL_0
                        )));

                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CELL_TEMN_LK_2,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_1,
                        CV2_LEAK_POL_1,
                        CV3_LEAK_POL_1,
                        CV4_LEAK_POL_1,
                        CV5_LEAK_POL_1,
                        CV6_LEAK_POL_1,
                        STK_LEAK_POL_1
                        )));
                }
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CELL_TEMN_LK_1,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_0,
                        CV2_LEAK_POL_0,
                        CV3_LEAK_POL_0,
                        CV4_LEAK_POL_0,
                        CV5_LEAK_POL_0,
                        CV6_LEAK_POL_0,
                        STK_LEAK_POL_0
                        )));

                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CELL_TEMN_LK_2,
                        string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                        CV1_LEAK_POL_1,
                        CV2_LEAK_POL_1,
                        CV3_LEAK_POL_1,
                        CV4_LEAK_POL_1,
                        CV5_LEAK_POL_1,
                        CV6_LEAK_POL_1,
                        STK_LEAK_POL_1
                        )));
                }
            }


            return JudgementTestItem(ti);
        }
        public bool Cell_Terminal_Leakage_Dectection_2(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = CV2_LEAK_POL_0 > CV2_LEAK_POL_1 ? CV2_LEAK_POL_0 : CV2_LEAK_POL_1;
            return JudgementTestItem(ti);
        }
        public bool Cell_Terminal_Leakage_Dectection_3(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = CV3_LEAK_POL_0 > CV3_LEAK_POL_1 ? CV3_LEAK_POL_0 : CV3_LEAK_POL_1;
            return JudgementTestItem(ti);
        }
        public bool Cell_Terminal_Leakage_Dectection_4(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = CV4_LEAK_POL_0 > CV4_LEAK_POL_1 ? CV4_LEAK_POL_0 : CV4_LEAK_POL_1;
            return JudgementTestItem(ti);
        }
        public bool Cell_Terminal_Leakage_Dectection_5(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = CV5_LEAK_POL_0 > CV5_LEAK_POL_1 ? CV5_LEAK_POL_0 : CV5_LEAK_POL_1;
            return JudgementTestItem(ti);
        }
        public bool Cell_Terminal_Leakage_Dectection_6(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = CV6_LEAK_POL_0 > CV6_LEAK_POL_1 ? CV6_LEAK_POL_0 : CV6_LEAK_POL_1;
            return JudgementTestItem(ti);
        }
        public bool Cell_Terminal_Leakage_Dectection_7(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = STK_LEAK_POL_0 > STK_LEAK_POL_1 ? STK_LEAK_POL_0 : STK_LEAK_POL_1;
            return JudgementTestItem(ti);
        }

        #endregion

        #region SM05~46

        public bool GPIO_Verify_UT_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region 211020 Make spec

            List<bool> GPIO_Verify_UT_SPEC = new List<bool>();
            var val = viewModel.TestItemList["GPIO Verify_UT Fault Check"].Min.ToString();

            while(val.Length<7)
            {
                val = "0" + val;
            }

            LogState(LogType.Info, "GPIO Verify_UT Fault Check : " + val);
            foreach (var cha in val.ToArray())
            {
                if (cha == '0')
                    GPIO_Verify_UT_SPEC.Add(false);
                else if (cha == '1')
                    GPIO_Verify_UT_SPEC.Add(true);
            }

            List<bool> GPIO_Verify_OT_SPEC = new List<bool>();
            val = viewModel.TestItemList["GPIO Verify_OT Fault Check"].Min.ToString();

            while (val.Length < 7)
            {
                val = "0" + val;
            }
            LogState(LogType.Info, "GPIO Verify_OT Fault Check : " + val);
            foreach (var cha in val.ToArray())
            {
                if (cha == '0')
                    GPIO_Verify_OT_SPEC.Add(false);
                else if (cha == '1')
                    GPIO_Verify_OT_SPEC.Add(true);
            }

            if(GPIO_Verify_UT_SPEC.Count != 7 ||
                GPIO_Verify_OT_SPEC.Count != 7)
            {
                LogState(LogType.Info, "SPEC ERROR!!");
                ti.Value_ = null;
                return JudgementTestItem(ti);
            }

            



            #endregion

            for (int i = 0; i < 10; i++)
            {
                cmc.i0077_data.Clear();

                cmc.CMC_SEND_CMD(CMC_COMMAND.SM05);
                Thread.Sleep(1000);

                if (cmc.i0077_data.Count >= 16)
                {
                    //211020 spec check
                    if (cmc.i0077_data[0] != GPIO_Verify_UT_SPEC[0] || //UT
                        cmc.i0077_data[1] != GPIO_Verify_UT_SPEC[1] ||
                        cmc.i0077_data[2] != GPIO_Verify_UT_SPEC[2] ||
                        cmc.i0077_data[3] != GPIO_Verify_UT_SPEC[3] ||
                        cmc.i0077_data[4] != GPIO_Verify_UT_SPEC[4] ||
                        cmc.i0077_data[5] != GPIO_Verify_UT_SPEC[5] ||
                        cmc.i0077_data[6] != GPIO_Verify_UT_SPEC[6] ||
                        cmc.i0077_data[8] != GPIO_Verify_OT_SPEC[0] || //OT
                        cmc.i0077_data[9] != GPIO_Verify_OT_SPEC[1] ||
                        cmc.i0077_data[10] != GPIO_Verify_OT_SPEC[2] ||
                        cmc.i0077_data[11] != GPIO_Verify_OT_SPEC[3] ||
                        cmc.i0077_data[12] != GPIO_Verify_OT_SPEC[4] ||
                        cmc.i0077_data[13] != GPIO_Verify_OT_SPEC[5] ||
                        cmc.i0077_data[14] != GPIO_Verify_OT_SPEC[6])
                    {
                        // 210629 Reboot Retry Add
                        this.LogState(LogType.Info, "UT_OT_DATA_CHECK_NG - Reboot CMC");
                        Reboot_CMC();
                        //Thread.Sleep(500);
                        //cmc.CMC_WakeUp();
                        //Thread.Sleep(CMC_WakeUp_after_sleep);

                        //this.LogState(LogType.Info, "UT_OT_DATA_CHECK_NG");
                        //cmc.CMC_SEND_CMD(CMC_COMMAND.SM_RESET);
                        //Thread.Sleep(3000);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // 210629 Reboot Retry Add
                    Reboot_CMC();
                    //this.LogState(LogType.NG, "NOT_RECIEVE_DATA - Reboot CMC");
                    //Thread.Sleep(500);
                    //cmc.CMC_WakeUp();
                    //Thread.Sleep(CMC_WakeUp_after_sleep);

                    //cmc.CMC_SEND_CMD(CMC_COMMAND.SM_RESET);
                    //Thread.Sleep(3000);
                    //this.LogState(LogType.Info, "NOT_RECIVED_DATA");
                }
            }

            var list = cmc.i0077_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[0]) { ocnt += 1; LogState(LogType.Info, "AN0_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN0_UT:0"); ostr += "0"; };
                if (list[1]) { ocnt += 1; LogState(LogType.Info, "AN1_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN1_UT:0"); ostr += "0"; };
                if (list[2]) { ocnt += 1; LogState(LogType.Info, "AN2_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN2_UT:0"); ostr += "0"; };
                if (list[3]) { ocnt += 1; LogState(LogType.Info, "AN3_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN3_UT:0"); ostr += "0"; };
                if (list[4]) { ocnt += 1; LogState(LogType.Info, "AN4_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN4_UT:0"); ostr += "0"; };
                if (list[5]) { ocnt += 1; LogState(LogType.Info, "AN5_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN5_UT:0"); ostr += "0"; };
                if (list[6]) { ocnt += 1; LogState(LogType.Info, "AN6_UT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN6_UT:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                ti.Value_ = null;
            }

            return JudgementTestItem(ti);
        }
        public bool GPIO_Verify_OT_Fault_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var list = cmc.i0077_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[8]) { ocnt += 1; LogState(LogType.Info,  "AN0_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN0_OT:0"); ostr += "0"; };
                if (list[9]) { ocnt += 1; LogState(LogType.Info,  "AN1_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN1_OT:0"); ostr += "0"; };
                if (list[10]) { ocnt += 1; LogState(LogType.Info, "AN2_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN2_OT:0"); ostr += "0"; };
                if (list[11]) { ocnt += 1; LogState(LogType.Info, "AN3_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN3_OT:0"); ostr += "0"; };
                if (list[12]) { ocnt += 1; LogState(LogType.Info, "AN4_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN4_OT:0"); ostr += "0"; };
                if (list[13]) { ocnt += 1; LogState(LogType.Info, "AN5_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN5_OT:0"); ostr += "0"; };
                if (list[14]) { ocnt += 1; LogState(LogType.Info, "AN6_OT:1"); ostr += "1"; } else { LogState(LogType.Info, "AN6_OT:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                ti.Value_ = null;
            }

            return JudgementTestItem(ti);
        }
        public bool GPIO_Open_Wire_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region 211014 old logics
            //ti.Value_ = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM06_a);

            //var list = cmc.i0078_data;

            //if (list.Count >= 16)
            //{
            //    int ocnt = 0;
            //    string ostr = "";
            //    if (list[0]) { ocnt += 1; LogState(LogType.Info, "OW0:1"); ostr += "1"; } else { LogState(LogType.Info, "OW0:0"); ostr += "0"; };
            //    if (list[1]) { ocnt += 1; LogState(LogType.Info, "OW1:1"); ostr += "1"; } else { LogState(LogType.Info, "OW1:0"); ostr += "0"; };
            //    if (list[2]) { ocnt += 1; LogState(LogType.Info, "OW2:1"); ostr += "1"; } else { LogState(LogType.Info, "OW2:0"); ostr += "0"; };
            //    if (list[3]) { ocnt += 1; LogState(LogType.Info, "OW3:1"); ostr += "1"; } else { LogState(LogType.Info, "OW3:0"); ostr += "0"; };
            //    if (list[4]) { ocnt += 1; LogState(LogType.Info, "OW4:1"); ostr += "1"; } else { LogState(LogType.Info, "OW4:0"); ostr += "0"; };
            //    if (list[5]) { ocnt += 1; LogState(LogType.Info, "OW5:1"); ostr += "1"; } else { LogState(LogType.Info, "OW5:0"); ostr += "0"; };
            //    if (list[6]) { ocnt += 1; LogState(LogType.Info, "OW6:1"); ostr += "1"; } else { LogState(LogType.Info, "OW6:0"); ostr += "0"; };

            //    ti.Value_ = ostr;
            //}

            #endregion

            #region 211015 new logics

            if (cmc.raw_cmcList.ContainsKey("0078"))
                cmc.raw_cmcList.Remove("0078");
            
            cmc.i0078_data.Clear();

            Stopwatch st = new Stopwatch();
            st.Start();

            //220314 Porsche FL 사용안함
            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            //{
            //    cmc.CMC_SEND_CMD(CMC_COMMAND.SM06_1);
            //}
            //else
            {
                cmc.CMC_SEND_CMD(CMC_COMMAND.SM06);
            }

            //211015 pjh confirm
            int cnt = 1000;

            while (!cmc.raw_cmcList.ContainsKey("0078"))
            {
                Thread.Sleep(1);
                cnt--;
                if (cnt < 0)
                {
                    LogState(LogType.Info, "NOT RECEIVED 0078");
                    ti.Value_ = null;
                    return JudgementTestItem(ti);
                }
            }

            st.Stop();

            LogState(LogType.Info, "RECEIVED i0078 in " + st.ElapsedMilliseconds.ToString() + "ms");

            LogState(LogType.Info, "Wait 20ms (in progress)");

            Thread.Sleep(20);


            var list = cmc.i0078_data;

            if (list.Count >= 16)
            {
                int ocnt = 0;
                string ostr = "";
                if (list[0]) { ocnt += 1; LogState(LogType.Info, "OW0:1"); ostr += "1"; } else { LogState(LogType.Info, "OW0:0"); ostr += "0"; };
                if (list[1]) { ocnt += 1; LogState(LogType.Info, "OW1:1"); ostr += "1"; } else { LogState(LogType.Info, "OW1:0"); ostr += "0"; };
                if (list[2]) { ocnt += 1; LogState(LogType.Info, "OW2:1"); ostr += "1"; } else { LogState(LogType.Info, "OW2:0"); ostr += "0"; };
                if (list[3]) { ocnt += 1; LogState(LogType.Info, "OW3:1"); ostr += "1"; } else { LogState(LogType.Info, "OW3:0"); ostr += "0"; };
                if (list[4]) { ocnt += 1; LogState(LogType.Info, "OW4:1"); ostr += "1"; } else { LogState(LogType.Info, "OW4:0"); ostr += "0"; };
                if (list[5]) { ocnt += 1; LogState(LogType.Info, "OW5:1"); ostr += "1"; } else { LogState(LogType.Info, "OW5:0"); ostr += "0"; };
                if (list[6]) { ocnt += 1; LogState(LogType.Info, "OW6:1"); ostr += "1"; } else { LogState(LogType.Info, "OW6:0"); ostr += "0"; };

                ti.Value_ = ostr;
            }
            else
            {
                //0000이 들어왔을 경우임
                int ocnt = 0;
                string ostr = "";
                ocnt += 1; LogState(LogType.Info, "OW0:0"); ostr += "0";
                ocnt += 1; LogState(LogType.Info, "OW1:0"); ostr += "0";
                ocnt += 1; LogState(LogType.Info, "OW2:0"); ostr += "0";
                ocnt += 1; LogState(LogType.Info, "OW3:0"); ostr += "0";
                ocnt += 1; LogState(LogType.Info, "OW4:0"); ostr += "0";
                ocnt += 1; LogState(LogType.Info, "OW5:0"); ostr += "0";
                ocnt += 1; LogState(LogType.Info, "OW6:0"); ostr += "0";

                ti.Value_ = ostr;
            }

            #endregion

            return JudgementTestItem(ti);
        }
        public bool ADC_Verify_Check_ADC1_A_Fault(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM07_ADC1_A_FLT);
            return JudgementTestItem(ti);
        }
        public bool ADC_Verify_Check_ADC1_B_Fault(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM07_ADC1_B_FLT);
            return JudgementTestItem(ti);
        }
        public bool Oscillator_Monitor_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.OSC_ERR_FLT);
            return JudgementTestItem(ti);
        }
        public bool VCOM_UV_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM09_VCOM_UV_FLT);
            return JudgementTestItem(ti);
        }
        public bool VANA_UV_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM10_VANA_UV_FLT);
            return JudgementTestItem(ti);
        }
        public bool Otmp_Protection_Mode_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM11_IC_TSD_FLT);
            return JudgementTestItem(ti);
        }
        public bool GND_Loss_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM12_GND_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Fuses_ECC_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM13_DED_ERR_FLT);
            return JudgementTestItem(ti);
        }
        public bool VANA_OV_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM15_VANA_OV_FLT);
            return JudgementTestItem(ti);
        }
        public bool Frame_Register_Address_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Cyclic_Config_Check_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Frame_CRC_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Start_Stop_Bit_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Loss_of_Comunication_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Frame_Tag_ID_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool VCOM_OV_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM23_VCOM_OV_FLT);
            return JudgementTestItem(ti);
        }
        public bool VPWR_OV_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM24_VPWR_OV_FLT);
            return JudgementTestItem(ti);
        }
        public bool VPWR_UV_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM25_VPWR_LV_FLT);
            return JudgementTestItem(ti);
        }
        public bool Frame_Response_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Data_Ready_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Fuse_Bit_Error_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT2(CMC_FAULT2_STATUS.SM31_FUSE_ERR_FLT);
            return JudgementTestItem(ti);
        }
        public bool Frame_Rolling_Counter_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT1(CMC_FAULT1_STATUS.SM21_COM_LOSS_FLT);
            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_Stack_Compare_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            //cmc.CMC_SEND_CMD(CMC_COMMAND.SM45);
            //Thread.Sleep(5);

            //ti.Value_ = cmc.CMC_RECV_i0072(CMC_i0072.CMC_SM45_a);

            var mv = CMClist[0] * 0.0024415;
            var cv1 = CMClist[1] * 0.0001525925;
            var cv2 = CMClist[2] * 0.0001525925;
            var cv3 = CMClist[3] * 0.0001525925;
            var cv4 = CMClist[4] * 0.0001525925;
            var cv5 = CMClist[5] * 0.0001525925;
            var cv6 = CMClist[6] * 0.0001525925;

            ti.Value_ = Math.Abs((cv1 + cv2 + cv3 + cv4 + cv5 + cv6) - mv) * 1000;//mv


            LogState(LogType.Info, string.Format("{0} : ({1} + {2} + {3} + {4} + {5} + {6}) - {7} = Math.abs({8})",
                ti.Name, cv1, cv2, cv3, cv4, cv5, cv6, mv, ti.Value_));

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_STAK_COPR, string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", cv1, cv2, cv3, cv4, cv5, cv6, mv)));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CELL_STAK_COPR, string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", cv1, cv2, cv3, cv4, cv5, cv6, mv)));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CELL_STAK_COPR, string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", cv1, cv2, cv3, cv4, cv5, cv6, mv)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CELL_STAK_COPR, string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", cv1, cv2, cv3, cv4, cv5, cv6, mv)));
            }
            return JudgementTestItem(ti);
        }
        public bool VCP_UV_Detection_Check(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cmc.CMC_RECV_FAULT3(CMC_FAULT3_STATUS.SM46_VCP_UV);
            return JudgementTestItem(ti);
        }

        #endregion
    }
}
