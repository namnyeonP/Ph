using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EOL_BASE.Forms;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        private int DAQErrcnt = 0;

        bool Reboot_CMC()
        {
            const int nTryCount = 3;

            for(int i=0; i<nTryCount; ++i)
            {
                this.LogState(LogType.DEVICE_CHECK, "[REBOOT CMC] Try Count" + (i+1).ToString());

                if (cmc == null)
                {
                    this.LogState(LogType.DEVICE_CHECK, "[REBOOT CMC] CMC obj is null, return false");

                    return false/*true*/;
                }

                cmc._Dispose();

                cmc = null;

                Thread.Sleep(100);

                cmc = new CCMC(this);

                Thread.Sleep(5000);

                if(cmc.isAlive == true)
                {
                    Thread.Sleep(500);

                    cmc.CMC_WakeUp();

                    Thread.Sleep(CMC_WakeUp_after_sleep);

                    cmc.CMC_V4328();

                    Thread.Sleep(CMC_V4328_after_sleep);

                    this.LogState(LogType.DEVICE_CHECK, "[REBOOT CMC] Success");

                    return true;
                }
            }

            this.LogState(LogType.DEVICE_CHECK, "[REBOOT CMC] Fail");

            return false;
        }

        //210315 wjs add for porsche cmc box <---> porsche fl cmc box change
        bool Change_CMC()
        {
            bool rst = false;

            //모델체인지에서만 사용해야하고 만약 모델체인지 할때 현재 켜져있는 CMC 가 대상 모델과 컴포트 및 릴레이 번호가 다르다면 해당 모델 포트와 릴레이로 설정하고
            //CMC 클래스를 재 생성한다. 사용하지 않는 박스의 전원라인은 off
            if (pro_Type == ProgramType.EOLInspector || pro_Type == ProgramType.VoltageInspector)
            {
                if ((localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                && (cmc_PortName != cmc_PortName_FL || CMC_Relay != CMC_Relay_FL))
                {
                    this.LogState(LogType.DEVICE_CHECK, "[CHANGE CMC] START");
                    //포르쉐 cmc box 전원을 끊는다.

                    //221102 wjs add 아래 off 해야하는 릴레이가 FACELIFT or PORS 의 두 경우에 하나가 더 추가 되었으므로(M183) Off Relay를 현재의CMC_Relay 로 수정
                    //       현장에서 테스트 요망
                    //relays.RelayOff(CMC_Relay_PORS);
                    relays.RelayOff(CMC_Relay);
                    //210323 wjs add port & relay num info log
                    LogState(LogType.Info, "CMC Port : " + cmc_PortName + " CMC Relay Number : " + CMC_Relay);
                    cmc_PortName = cmc_PortName_FL;
                    CMC_Relay = CMC_Relay_FL;

                    cmc._Dispose();
                    LogState(LogType.DEVICE_CHECK, "CMC Dispose");
                    cmc = null;
                    Thread.Sleep(100);
                    cmc = new CCMC(this);

                    Sleep_Event(5000);

                    if (cmc.isAlive)
                        rst = cmc.isAlive;
                    this.LogState(LogType.DEVICE_CHECK, "[CHANGE CMC] END");
                }
                //221101 wjs add mase m183
                else if ((localTypes == ModelType.MASERATI_M183_NORMAL)
                && (cmc_PortName != cmc_PortName_Mas_M183 || CMC_Relay != CMC_Relay_Mas_M183)) //221101 wjs 체크필요
                {
                    this.LogState(LogType.DEVICE_CHECK, "[CHANGE CMC] START");
                    //포르쉐 cmc box 전원을 끊는다.

                    //221102 wjs add 아래 off 해야하는 릴레이가 FACELIFT or PORS 의 두 경우에 하나가 더 추가 되었으므로(M183) Off Relay를 현재의CMC_Relay 로 수정
                    //       현장에서 테스트 요망
                    //relays.RelayOff(CMC_Relay_PORS);
                    relays.RelayOff(CMC_Relay);
                    //210323 wjs add port & relay num info log
                    LogState(LogType.Info, "CMC Port : " + cmc_PortName + " CMC Relay Number : " + CMC_Relay);
                    cmc_PortName = cmc_PortName_Mas_M183;
                    CMC_Relay = CMC_Relay_Mas_M183;

                    cmc._Dispose();
                    LogState(LogType.DEVICE_CHECK, "CMC Dispose");
                    cmc = null;
                    Thread.Sleep(100);
                    cmc = new CCMC(this);

                    Sleep_Event(5000);

                    if (cmc.isAlive)
                        rst = cmc.isAlive;
                    this.LogState(LogType.DEVICE_CHECK, "[CHANGE CMC] END");
                }
                else if ((localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR || localTypes == ModelType.MASERATI_NORMAL)
                    && (cmc_PortName != cmc_PortName_PORS || CMC_Relay != CMC_Relay_PORS))
                {
                    this.LogState(LogType.DEVICE_CHECK, "[CHANGE CMC] START");
                    //포르쉐 페이스리프트 cmc box 전원을 끊는다.

                    //221102 wjs add 아래 off 해야하는 릴레이가 FACELIFT or PORS 의 두 경우에 하나가 더 추가 되었으므로(M183) Off Relay를 현재의CMC_Relay 로 수정
                    //       현장에서 테스트 요망
                    //relays.RelayOff(CMC_Relay_FL);
                    relays.RelayOff(CMC_Relay);
                    //210323 wjs add port & relay num info log
                    LogState(LogType.Info, "CMC Port : " + cmc_PortName + " CMC Relay Number : " + CMC_Relay);
                    cmc_PortName = cmc_PortName_PORS;
                    CMC_Relay = CMC_Relay_PORS;

                    cmc._Dispose();
                    LogState(LogType.DEVICE_CHECK, "CMC Dispose");
                    cmc = null;
                    Thread.Sleep(100);
                    cmc = new CCMC(this);

                    Sleep_Event(5000);

                    if (cmc.isAlive)
                        rst = cmc.isAlive;
                    this.LogState(LogType.DEVICE_CHECK, "[CHANGE CMC] END");
                }
                else
                {
                    this.LogState(LogType.Info, "[CHANGE CMC] END");
                }
            }
            return rst;
        }
        public bool BarcodeReading(TestItem ti)
        {
            isProcessingUI(ti);

            this.Dispatcher.Invoke(new Action(() =>
            {
                ti.Value_ = this.viewModel.LotId = lotTb.Text;
                //49. 바코드 리딩 값 적용 구문 및 로그 추가
                LogState(LogType.Info, "Value : " + ti.Value_);
            }));

            // 상세수집항목 저장
            SaveDetailItem(ti);

            deviceStatus = "";
            deviceRetryCount = ""; // 2020.01.22 KSM : 결과 파일에 기재할 문자열 변수 추가

            // Device 상태 확인 메서드
            CheckDeviceConnectStatus(ti);

            //2022.10.17소모품 카운트 하는 부분
            Protection_PartsCountStart = new Thread(() =>
            {
                if ((int)this.pro_Type == (int)ProgramType.EOLInspector)
                {
                    FormPartsCountSetting PCS = new FormPartsCountSetting(this, "START");
                    PCS.CountRealData();
                    this.SetPartsCountData(ti, "StartRun");
                }
                else if ((int)this.pro_Type == (int)ProgramType.HipotInspector)
                {
                }
                else
                {

                }


            });
            Protection_PartsCountStart.Start();

            return JudgementTestItem(ti);
        }

        private void SaveDetailItem(TestItem ti)
        { 
            ti.refValues_.Clear();

            #region Report Control spec to Detail items
            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_BEF_DIS_REST_T, this.viewModel.ControlItemList[AUDI_EOL_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_DISCHARGE_CURR, this.viewModel.ControlItemList[AUDI_EOL_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_DISCHARGE_TIME, this.viewModel.ControlItemList[AUDI_EOL_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_DIS_CURR_LIMIT, this.viewModel.ControlItemList[AUDI_EOL_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_AFT_DIS_REST_T, this.viewModel.ControlItemList[AUDI_EOL_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_CHARGE____CURR, this.viewModel.ControlItemList[AUDI_EOL_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_CHARGE____TIME, this.viewModel.ControlItemList[AUDI_EOL_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_CHA_CURR_LIMIT, this.viewModel.ControlItemList[AUDI_EOL_ControlKey08]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_AFT_CHA_RES__T, this.viewModel.ControlItemList[AUDI_EOL_ControlKey09]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_SAFE_VOL_H_LIM, this.viewModel.ControlItemList[AUDI_EOL_ControlKey10]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_SAFE_VOL_L_LIM, this.viewModel.ControlItemList[AUDI_EOL_ControlKey11]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_CELL_FOMULA__1, this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_CELL_FOMULA__2, this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_MODULE_FOMULA1, this.viewModel.ControlItemList[AUDI_EOL_ControlKey14]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_MODULE_FOMULA2, this.viewModel.ControlItemList[AUDI_EOL_ControlKey15]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_MODULE_FOMULA3, this.viewModel.ControlItemList[AUDI_EOL_ControlKey16]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_SPEC_CELL_FOMULA__3, this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]));

                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS__IR_LEVEL, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS___IR_TIME, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI__LEVEL, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI___TIME, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI_RAMP_T, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI_ARC_ON, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI_ARC_LM, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey07]));

                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_IR__RANGE, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey08]));
                    //ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI__RANGE, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey09]));
                }
                else if (pro_Type == ProgramType.VoltageInspector)
                {

                }
            }
            else if (localTypes == ModelType.E_UP)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_BEF_DIS_REST_T, this.viewModel.ControlItemList[E_UP_EOL_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_DISCHARGE_CURR, this.viewModel.ControlItemList[E_UP_EOL_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_DISCHARGE_TIME, this.viewModel.ControlItemList[E_UP_EOL_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_DIS_CURR_LIMIT, this.viewModel.ControlItemList[E_UP_EOL_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_AFT_DIS_REST_T, this.viewModel.ControlItemList[E_UP_EOL_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_CHARGE____CURR, this.viewModel.ControlItemList[E_UP_EOL_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_CHARGE____TIME, this.viewModel.ControlItemList[E_UP_EOL_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_CHA_CURR_LIMIT, this.viewModel.ControlItemList[E_UP_EOL_ControlKey08]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_AFT_CHA_RES__T, this.viewModel.ControlItemList[E_UP_EOL_ControlKey09]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_SAFE_VOL_H_LIM, this.viewModel.ControlItemList[E_UP_EOL_ControlKey10]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_SAFE_VOL_L_LIM, this.viewModel.ControlItemList[E_UP_EOL_ControlKey11]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_CELL_FOMULA__1, this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_CELL_FOMULA__2, this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_MODULE_FOMULA1, this.viewModel.ControlItemList[E_UP_EOL_ControlKey14]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_MODULE_FOMULA2, this.viewModel.ControlItemList[E_UP_EOL_ControlKey15]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_MODULE_FOMULA3, this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_SPEC_CELL_FOMULA__3, this.viewModel.ControlItemList[E_UP_EOL_ControlKey16]));

                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS__IR_LEVEL, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS___IR_TIME, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS_WI__LEVEL, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS_WI___TIME, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS_WI_RAMP_T, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS_WI_ARC_ON, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS_WI_ARC_LM, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey07]));

                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_E_UP_PLUS_IR__RANGE, this.viewModel.ControlItemList[E_UP_HIPOT_ControlKey08]));
                    //ti.refValues_.Add(MakeDetailItem(KEY_HIP_AUD_NOR_PLUS_WI__RANGE, this.viewModel.ControlItemList[AUDI_HIPOT_ControlKey09]));
                }
                else if (pro_Type == ProgramType.VoltageInspector)
                {

                }
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)  //201217 wjs maserati model delete
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_BEF_DIS_REST_T, this.viewModel.ControlItemList[PORS_EOL_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_DISCHARGE_CURR, this.viewModel.ControlItemList[PORS_EOL_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_DISCHARGE_TIME, this.viewModel.ControlItemList[PORS_EOL_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_DIS_CURR_LIMIT, this.viewModel.ControlItemList[PORS_EOL_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_AFT_DIS_REST_T, this.viewModel.ControlItemList[PORS_EOL_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_CHARGE____CURR, this.viewModel.ControlItemList[PORS_EOL_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_CHARGE____TIME, this.viewModel.ControlItemList[PORS_EOL_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_CHA_CURR_LIMIT, this.viewModel.ControlItemList[PORS_EOL_ControlKey08]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_AFT_CHA_RES__T, this.viewModel.ControlItemList[PORS_EOL_ControlKey09]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_SAFE_VOL_H_LIM, this.viewModel.ControlItemList[PORS_EOL_ControlKey10]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_SAFE_VOL_L_LIM, this.viewModel.ControlItemList[PORS_EOL_ControlKey11]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_CELL_FOMULA__1, this.viewModel.ControlItemList[PORS_EOL_ControlKey12]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_CELL_FOMULA__2, this.viewModel.ControlItemList[PORS_EOL_ControlKey13]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_MODULE_FOMULA1, this.viewModel.ControlItemList[PORS_EOL_ControlKey14]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_SPEC_MODULE_FOMULA2, this.viewModel.ControlItemList[PORS_EOL_ControlKey15]));
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS__IR_LEVEL, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS___IR_TIME, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI__LEVEL, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI___TIME, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI_RAMP_T, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI_ARC_ON, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI_ARC_LM, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey07]));

                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_IR__RANGE, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey08]));
                    //ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI__RANGE, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey09]));
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_POR_NOR_PLUS__IR_LEVEL, this.viewModel.ControlItemList[PORS_HIPNR_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_POR_NOR_PLUS___IR_TIME, this.viewModel.ControlItemList[PORS_HIPNR_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_POR_NOR_PLUS_IR__RANGE, this.viewModel.ControlItemList[PORS_HIPNR_ControlKey03]));
                }
                else if (pro_Type == ProgramType.VoltageInspector)
                {

                }
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)    //201217 wjs maserati model add
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_BEF_DIS_REST_T, this.viewModel.ControlItemList[MAS_EOL_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_DISCHARGE_CURR, this.viewModel.ControlItemList[MAS_EOL_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_DISCHARGE_TIME, this.viewModel.ControlItemList[MAS_EOL_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_DIS_CURR_LIMIT, this.viewModel.ControlItemList[MAS_EOL_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_AFT_DIS_REST_T, this.viewModel.ControlItemList[MAS_EOL_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_CHARGE____CURR, this.viewModel.ControlItemList[MAS_EOL_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_CHARGE____TIME, this.viewModel.ControlItemList[MAS_EOL_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_CHA_CURR_LIMIT, this.viewModel.ControlItemList[MAS_EOL_ControlKey08]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_AFT_CHA_RES__T, this.viewModel.ControlItemList[MAS_EOL_ControlKey09]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_SAFE_VOL_H_LIM, this.viewModel.ControlItemList[MAS_EOL_ControlKey10]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_SAFE_VOL_L_LIM, this.viewModel.ControlItemList[MAS_EOL_ControlKey11]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_CELL_FOMULA__1, this.viewModel.ControlItemList[MAS_EOL_ControlKey12]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_CELL_FOMULA__2, this.viewModel.ControlItemList[MAS_EOL_ControlKey13]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_MODULE_FOMULA1, this.viewModel.ControlItemList[MAS_EOL_ControlKey14]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_SPEC_MODULE_FOMULA2, this.viewModel.ControlItemList[MAS_EOL_ControlKey15]));
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS__IR_LEVEL, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS___IR_TIME, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS_WI__LEVEL, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS_WI___TIME, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS_WI_RAMP_T, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS_WI_ARC_ON, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS_WI_ARC_LM, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey07]));

                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_NOR_PLUS_IR__RANGE, this.viewModel.ControlItemList[MAS_HIPOT_ControlKey08]));
                    //ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI__RANGE, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey09]));
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_MAS_NOR_PLUS__IR_LEVEL, this.viewModel.ControlItemList[MAS_HIPNR_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_MAS_NOR_PLUS___IR_TIME, this.viewModel.ControlItemList[MAS_HIPNR_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_MAS_NOR_PLUS_IR__RANGE, this.viewModel.ControlItemList[MAS_HIPNR_ControlKey03]));
                }
                else if (pro_Type == ProgramType.VoltageInspector)
                {

                }
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)  //201217 wjs maserati model delete
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_BEF_DIS_REST_T, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_DISCHARGE_CURR, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_DISCHARGE_TIME, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_DIS_CURR_LIMIT, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_AFT_DIS_REST_T, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_CHARGE____CURR, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_CHARGE____TIME, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_CHA_CURR_LIMIT, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey08]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_AFT_CHA_RES__T, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey09]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_SAFE_VOL_H_LIM, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey10]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_SAFE_VOL_L_LIM, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey11]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_CELL_FOMULA__1, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_CELL_FOMULA__2, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_MODULE_FOMULA1, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey14]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_SPEC_MODULE_FOMULA2, this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey15]));
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS__IR_LEVEL, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS___IR_TIME, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS_IR__RANGE, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS_WI__LEVEL, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS_WI___TIME, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS_WI_RAMP_T, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS_WI_ARC_ON, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_F_L_PLUS_WI_ARC_LM, this.viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey08]));
                    //ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI__RANGE, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey09]));
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_POR_F_L_PLUS__IR_LEVEL, this.viewModel.ControlItemList[PORS_FL_HIPNR_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_POR_F_L_PLUS___IR_TIME, this.viewModel.ControlItemList[PORS_FL_HIPNR_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_POR_F_L_PLUS_IR__RANGE, this.viewModel.ControlItemList[PORS_FL_HIPNR_ControlKey03]));
                }
                else if (pro_Type == ProgramType.VoltageInspector)
                {

                }
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_BEF_DIS_REST_T, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_DISCHARGE_CURR, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_DISCHARGE_TIME, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_DIS_CURR_LIMIT, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_AFT_DIS_REST_T, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_CHARGE____CURR, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_CHARGE____TIME, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_CHA_CURR_LIMIT, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey08]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_AFT_CHA_RES__T, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey09]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_SAFE_VOL_H_LIM, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey10]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_SAFE_VOL_L_LIM, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey11]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_CELL_FOMULA__1, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_CELL_FOMULA__2, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_MODULE_FOMULA1, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey14]));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_SPEC_MODULE_FOMULA2, this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey15]));
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS__IR_LEVEL, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS___IR_TIME, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS_IR__RANGE, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey03]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS_WI__LEVEL, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey04]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS_WI___TIME, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey05]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS_WI_RAMP_T, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey06]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS_WI_ARC_ON, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey07]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HIP_MAS_M183_PLUS_WI_ARC_LM, this.viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey08]));
                    //ti.refValues_.Add(MakeDetailItem(KEY_HIP_POR_NOR_PLUS_WI__RANGE, this.viewModel.ControlItemList[PORS_HIPOT_ControlKey09]));
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_MAS_M183_PLUS____IR_LEVEL, this.viewModel.ControlItemList[MAS_M183_HIPNR_ControlKey01]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_MAS_M183_PLUS_____IR_TIME, this.viewModel.ControlItemList[MAS_M183_HIPNR_ControlKey02]));
                    ti.refValues_.Add(MakeDetailItem(KEY_HNR_MAS_M183_PLUS_IR____RANGE, this.viewModel.ControlItemList[MAS_M183_HIPNR_ControlKey03]));
                }
                else if (pro_Type == ProgramType.VoltageInspector)
                {

                }
            }
            #endregion

            if ((int)this.pro_Type == (int)ProgramType.EOLInspector)
            {
                //221017 바코드 리딩할때 저항 한번 찍음
                var rtList = new List<string>();
                McResistanceMeasure(mcResistanceposition, out rtList);

                try
                {
                    //221004 저항 측정 값 추가 Open 되었을 때
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_3_OPEN_CHECK, rtList[0].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_4_OPEN_CHECK, rtList[1].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_5_OPEN_CHECK, rtList[2].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_6_OPEN_CHECK, rtList[3].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_7_OPEN_CHECK, rtList[4].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_8_OPEN_CHECK, rtList[5].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_9_OPEN_CHECK, rtList[6].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_10_OPEN_CHECK, rtList[7].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_11_OPEN_CHECK, rtList[8].ToString()));
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_12_OPEN_CHECK, rtList[9].ToString()));

                    rtList.Clear();
                }
                catch
                {
                    rtList.Clear();
                }
            }
        }

        // 2020.01.21 KSM
        private void CheckDeviceConnectStatus(TestItem ti)
        {
            List<string> ngDeviceList = new List<string>();
            List<string> retryCountList = new List<string>(); // 2020.01.22 KSM : 재시도 횟수를 기재할 리스트 변수 추가

            var checkDeviceName = "";
            int retryCount; // 2020.01.22 KSM : 재시도 횟수 변수 추가

            isDoSleep = false;
            
            // 2020.01.22 KSM : TEMP, DMM, CYCLER, DAQ, HIPOT 재시도 3회 로직으로 수정
            if (pro_Type == ProgramType.EOLInspector)
            {
                #region TEMP CHECK
                bool result = ConnectionCheck_TEMP(out retryCount);

                if (!result)
                    ngDeviceList.Add("[TEMP]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[TEMP:{0}]", retryCount));
                #endregion

                #region DMM Check
                result = ConnectionCheck_DMM(out retryCount);

                if (!result)
                    ngDeviceList.Add("[DMM]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[DMM:{0}]", retryCount));
                #endregion

                #region Cycler Check
                result = ConnectionCheck_CYCLER(out retryCount);

                if (!result)
                    ngDeviceList.Add("[CYCLER]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[CYCLER:{0}]", retryCount));
                #endregion

                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.cmcStatus.Visibility = System.Windows.Visibility.Collapsed;
                        this.daqStatus.Visibility = System.Windows.Visibility.Collapsed;
                    }));

                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                    || localTypes == ModelType.E_UP)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    { 
                        this.daqStatus.Visibility = System.Windows.Visibility.Visible;
                    }));

                    //2. 검사기 공통) 계측기 연결성 점검 전체 정리
                    //-> DAQ) 연결성 점검 Retry 주석 처리됨.
                    #region DAQ Check
                    result = ConnectionCheck_DAQ(out retryCount);

                    if (!result)
                        ngDeviceList.Add("[DAQ]");

                    if (retryCount != 0)
                        retryCountList.Add(string.Format("[DAQ:{0}]", retryCount));
                    #endregion

                }
                else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                    || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    )
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.cmcStatus.Visibility = System.Windows.Visibility.Visible; 
                    }));

                    #region CMC Check
                    result = ConnectionCheck_CMC(out retryCount);

                    if (!result)
                        ngDeviceList.Add("[CMC]");

                    if (retryCount != 0)
                        retryCountList.Add(string.Format("[CMC:{0}]", retryCount));
                    #endregion
                }
            }
            else if (pro_Type == ProgramType.HipotInspector || pro_Type == ProgramType.Hipot_no_resin_Inspector)
            {
                #region HIPOT Check
                bool result = ConnectionCheck_HIPOT(out retryCount);

                if (!result)
                    ngDeviceList.Add("[HIPOT]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[HIPOT:{0}]", retryCount));
                #endregion
            }
            else if (pro_Type == ProgramType.VoltageInspector)
            {
                #region TEMP Check
                bool result = ConnectionCheck_TEMP(out retryCount);

                if (!result)
                    ngDeviceList.Add("[TEMP]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[TEMP:{0}]", retryCount));
                #endregion

                #region DMM Check
                result = ConnectionCheck_DMM(out retryCount);

                if (!result)
                    ngDeviceList.Add("[DMM]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[DMM:{0}]", retryCount));
                #endregion

                #region Not Use
                //if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                //{
                //    #region DAQ Check

                //    checkDeviceName = "[DAQ]";
                //    if (daq != null && daq.sp.IsOpen)
                //    {
                //        var nowDaqDt = daq.localDt;
                //        nowDaqDt = nowDaqDt.AddSeconds(5);

                //        //20180808 wjs add daq retry connection
                //        if (DateTime.Now > nowDaqDt)
                //        {
                //            LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                //            daq.sp.Close();
                //            daq.sp.Dispose();
                //            daq = null;
                //            Thread.Sleep(100);

                //            this.Dispatcher.Invoke(new Action(() =>
                //            {
                //                daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                //            }));

                //            nowDaqDt = daq.localDt;
                //            nowDaqDt = nowDaqDt.AddSeconds(5);

                //            if (DateTime.Now > nowDaqDt)
                //            {
                //                Dispatcher.BeginInvoke(new Action(() =>
                //                {
                //                    contBt_daq.Background = System.Windows.Media.Brushes.Red;
                //                }));

                //                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                //                ngDeviceList.Add(checkDeviceName);
                //            }
                //            else
                //            {
                //                Dispatcher.BeginInvoke(new Action(() =>
                //                {
                //                    contBt_daq.Background = System.Windows.Media.Brushes.Green;
                //                }));
                //                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                //            }
                //        }
                //        else
                //        {
                //            Dispatcher.BeginInvoke(new Action(() =>
                //            {
                //                contBt_daq.Background = System.Windows.Media.Brushes.Green;
                //            }));

                //            LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                //        }
                //    }
                //    else
                //    {
                //        LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                //        if (daq != null && daq.sp != null)
                //        {
                //            LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                //            daq.sp.Close();
                //            daq.sp.Dispose();
                //            daq = null;
                //        }

                //        this.Dispatcher.Invoke(new Action(() =>
                //        {
                //            daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                //        }));

                //        if (daq != null && daq.sp.IsOpen)
                //        {
                //            Dispatcher.BeginInvoke(new Action(() =>
                //            {
                //                contBt_daq.Background = System.Windows.Media.Brushes.Green;
                //            }));

                //            LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                //        }
                //        else
                //        {
                //            Dispatcher.BeginInvoke(new Action(() =>
                //            {
                //                contBt_daq.Background = System.Windows.Media.Brushes.Red;
                //            }));

                //            LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                //            ngDeviceList.Add(checkDeviceName);
                //        }
                //    }

                //    #endregion

                //}
                //else
                #endregion


                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.cmcStatus.Visibility = System.Windows.Visibility.Collapsed; 
                }));

                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                    || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    )
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.cmcStatus.Visibility = System.Windows.Visibility.Visible;
                    }));

                    #region CMC

                    checkDeviceName = "[CMC BOX]";
                    try
                    {
                        if (cmc != null && cmc.sp.IsOpen)
                        {
                            var nowCMCDt = cmc.localDt;
                            nowCMCDt = nowCMCDt.AddSeconds(1);

                            if (DateTime.Now > nowCMCDt)
                            {
                                if (!Reboot_CMC())
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                                    }));
                                    LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                                    ngDeviceList.Add(checkDeviceName);
                                }
                                else
                                {
                                    nowCMCDt = cmc.localDt;
                                    nowCMCDt = nowCMCDt.AddSeconds(1);

                                    if (DateTime.Now > nowCMCDt)
                                    {
                                        Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                                        }));
                                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                                        ngDeviceList.Add(checkDeviceName);
                                    }
                                    else
                                    {
                                        Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                                        }));
                                        LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                                        LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                                }));
                                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                                LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                            }
                        }
                        else
                        {
                            LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                            if (!Reboot_CMC())
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                                }));
                                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                                ngDeviceList.Add(checkDeviceName);
                            }
                            else
                            {
                                if (cmc != null && cmc.sp.IsOpen)
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                                    }));

                                    LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                                    LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                                }
                                else
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                                    }));
                                    LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                                    ngDeviceList.Add(checkDeviceName);
                                }
                            }
                        }
                    }
                    catch (Exception ec)
                    {
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName,ec);

                        try
                        {
                            if (!Reboot_CMC())
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                                }));
                                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                                ngDeviceList.Add(checkDeviceName);
                            }
                            else
                            {
                                if (cmc != null && cmc.sp.IsOpen)
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                                    }));

                                    LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                                    LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                                }
                                else
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                                    }));
                                    LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                                    ngDeviceList.Add(checkDeviceName);
                                }
                            }
                        }
                        catch (Exception evv)
                        {
                            LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName, evv);

                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                            }));
                            LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);

                        }
                    }

                    #endregion
                }
            }

            #region PLC Check

            checkDeviceName = "[PLC]";
            if (plc != null && !plc.isAlive)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_plc.Background = System.Windows.Media.Brushes.Red;
                }));
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                ngDeviceList.Add(checkDeviceName);
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_plc.Background = System.Windows.Media.Brushes.Green;
                }));

                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
            }
            #endregion

            #region MES Check
            if (!isMESskip)
            {
                bool result = ConnectionCheck_MES(out retryCount);

                if (!result)
                    ngDeviceList.Add("[MES]");

                if (retryCount != 0)
                    retryCountList.Add(string.Format("[MES:{0}]", retryCount));
            }
            #endregion

            // 2020.01.22 KSM : 재시도 리스트 변수의 값을 결과파일에 사용할 문자열 변수로 파싱 추가
            if (retryCountList.Count > 0)
            {
                foreach (var device in retryCountList)
                {
                    deviceRetryCount += device + "/";
                }
                deviceRetryCount = deviceRetryCount.Remove(deviceRetryCount.Length - 1, 1);
                LogState(LogType.DEVICE_CHECK, "[DEVICE_RETRY_COUNT] " + deviceRetryCount);
            }

            if (ngDeviceList.Count > 0)
            { 
                foreach (var device in ngDeviceList)
                {
                    deviceStatus += device + "/";
                }
                deviceStatus = deviceStatus.Remove(deviceStatus.Length - 1, 1);
                LogState(LogType.Fail, "[DEVICE_CHECK_RESULT] " + deviceStatus);

                tlamp.SetTLampInstrumentOff(true);
                ViewRetryWindow(deviceStatus, ti);
            }
            else
            { 
                tlamp.SetTLampInstrumentOff(false);
            }
        }

        #region ConnectionCheck_TEMP
        private bool ConnectionCheck_TEMP(out int retryCount)
        {
            string checkDeviceName = "[TEMP]";
            retryCount = 0;

            string regSubkey = "Software\\EOL_Trigger\\Relays";
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            string tempWd_Flag = rk.GetValue("NHT_RS232_WD_FLAG").ToString();

            if (tempWd_Flag == "0")
            {
                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                return true;
            }
            else
            {
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                for (int i = 0; i < 3; i++)
                {
                    if (isStop || ispause)
                    {
                        return false;
                    }
                    retryCount++;
                    Thread.Sleep(2000);

                    tempWd_Flag = rk.GetValue("NHT_RS232_WD_FLAG").ToString();

                    if (tempWd_Flag == "0")
                    {
                        LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                        return true;
                    }
                    else
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                }

                return false;
            }
        }
        #endregion

        #region ConnectionCheck_DMM
        private bool ConnectionCheck_DMM(out int retryCount)
        {
            string checkDeviceName = "[DMM]";
            retryCount = 0;

            string ret = keysight.IDN();

            if (ret.Length > 10)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                }));

                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                return true;
            }
            else
            {
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                for (int i = 0; i < 3; i++)
                {
                    if (isStop || ispause)
                    {
                        return false;
                    }
                    retryCount++;

                    keysight.Dispose();
                    keysight = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        keysight = new CKeysightDMM(this, true, keysight_PortName);
                    }));

                    Thread.Sleep(300);

                    ret = keysight.IDN();

                    if (ret.Length > 10)
                    {
                        LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                        }));

                        return true;
                    }
                    else
                    {
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                    }
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                }));

                return false;
            }
        }
        #endregion

        #region ConnectionCheck_CYCLER
        private bool ConnectionCheck_CYCLER(out int retryCount)
        {
            string checkDeviceName = "[CYCLER]";
            retryCount = 0;

            bool result = CyclerStatusCheck();

            if (result)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_cycler.Background = System.Windows.Media.Brushes.Green;
                }));
                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                return true;
            }
            else
            {
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                for (int i = 0; i < 3; i++)
                {
                    if (isStop || ispause)
                    {
                        return false;
                    }

                    retryCount++;

                    cycler._Dispose();
                    cycler = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        cycler = new CCycler(this, this.can_cycler1);
                    }));

                    result = CyclerStatusCheck();

                    if (result)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_cycler.Background = System.Windows.Media.Brushes.Green;
                        }));
                        LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                        return true;
                    }
                    else
                    {
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                    }
                }

                SetMainCState("NOT READY");

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_cycler.Background = System.Windows.Media.Brushes.Red;
                }));

                return false;
            }
        }
        #endregion

        private void Wait_INPUT_MC(int sec)
        {
            LogState(LogType.Info, "Wait to INPUT MC ON ( Max " + sec.ToString() + "sec )");
            int cnt = 0;
            while (cnt < sec * 1000)
            {
                if (isStop || ispause)
                {
                    return;
                }
                cnt += 100;
                Thread.Sleep(100);

                if (cycler.bmsList["110h"].Data.Contains("90 10 00 00 00 00 00 00"))        //OPCODE가 직렬일때
                {
                    LogState(LogType.Info, "INPUT MC ON in " + cnt.ToString() + "msec");
                    break;
                }
                else if (cycler.bmsList["110h"].Data.Contains("90 11 00 00 00 00 00 00"))   //OPCODE가 병렬일때
                {
                    LogState(LogType.Info, "INPUT MC ON in " + cnt.ToString() + "msec");
                    break;
                }
            }
        }

        #region CyclerStatusCheck()
        private bool CyclerStatusCheck()
        {
            if (cycler != null && cycler.isAlive1)
            {
                if (cycler.bmsList.Count > 0)
                {
                    #region 변경 구조
                    if (cycler.bmsList.ContainsKey("110h"))
                    {
                        if (cycler.bmsList["110h"].Data.Contains("00 00 00 00 00 00"))
                        {
                            //충전이나 방전일때는 어차피 여기를 탈수 없다!(전압/전류가 00이기 때문)
                            
                            if (cycler.bmsList["110h"].Data.Contains("90 10")) //입력MC ON
                            {
                                LogState(LogType.Info, "Cycler Status :[INPUT_MC_ON]");
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("91 20"))
                            {
                                LogState(LogType.Info, "Cycler Voltage sensing fail :[OUTPUT_MC_ON]");
                                return false;
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("92 "))
                            {
                                LogState(LogType.Info, "Cycler Voltage sensing fail :[CHARGE]");
                                return false;
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("93 "))
                            {
                                LogState(LogType.Info, "Cycler Voltage sensing fail :[DISCHARGE]");
                                return false;
                            } 
                            else if (cycler.bmsList["110h"].Data.Contains("96 "))
                            {
                                LogState(LogType.Info, "Cycler Voltage sensing fail :[REST]");
                                return false;
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("94 10"))//출력MC OFF(입력MC ON상태)
                            {
                                LogState(LogType.Info, "Cycler Status :[OUTPUT_MC_OFF]");
                            }
                            else                                                        //모두 아니라면(ex> 89 (에러클리어, 입력MC OFF상태))
                            {                                                           //입력MC만 붙이도록 시도
                                LogState(LogType.Info, "Try to Cycler [INPUT_MC_ON]");
                                cycler.SendToDSP1("100", new byte[] { 0x80, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });

                                Wait_INPUT_MC(30);

                                //위에서 붙이는 시도를 했는데, 여전히 OPCODE가 안바뀌었다면 장비 정지상태
                                if (cycler.bmsList["110h"].Data.Contains("90 10"))
                                {
                                    LogState(LogType.Info, "Cycler Status :[INPUT_MC_ON]");
                                } 
                                else
                                {
                                    LogState(LogType.Info, "Cycler is suspended(0x110)!");
                                    LogState(LogType.Info, "Receive Data: " + cycler.bmsList["110h"].Data.ToString());
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            //전압/전류가 0이 아니라면, 충전(92) 또는 방전(93), 휴지(96)가 진행중이라고 볼수있다.
                            //크게 출력MC OFF(입력MC ON상태)도 정상이라 볼수있겠다.
                            if (cycler.bmsList["110h"].Data.Contains("91 20"))
                            {
                                LogState(LogType.Info, "Cycler Status :[OUTPUT_MC_ON]");
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("90 10"))
                            {
                                LogState(LogType.Info, "Cycler Status :[INPUT_MC_ON]");
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("92 "))
                            {
                                LogState(LogType.Info, "Cycler Status :[CHARGE]");
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("93 "))
                            {
                                LogState(LogType.Info, "Cycler Status :[DISCHARGE]");
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("94 10"))
                            {
                                LogState(LogType.Info, "Cycler Status :[OUTPUT_MC_OFF]");
                            }
                            else if (cycler.bmsList["110h"].Data.Contains("96 "))
                            {
                                LogState(LogType.Info, "Cycler Status :[REST]");
                            }
                            else
                            {
                                LogState(LogType.Info, "Try to Cycler [INPUT_MC_ON]");
                                cycler.SendToDSP1("100", new byte[] { 0x80, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });

                                Wait_INPUT_MC(30);

                                //위에서 붙이는 시도를 했는데, 여전히 OPCODE가 안바뀌었다면 장비 정지상태
                                if (cycler.bmsList["110h"].Data.Contains("90 10"))
                                {
                                    LogState(LogType.Info, "Cycler Status :[INPUT_MC_ON]");
                                }
                                else
                                {
                                    LogState(LogType.Info, "Cycler is suspended(0x110)!");
                                    LogState(LogType.Info, "Receive Data: " + cycler.bmsList["110h"].Data.ToString());
                                    return false;
                                }
                            }
                        }
                    }
                    if (cycler.bmsList.ContainsKey("120h"))
                    {
                        if (!(cycler.bmsList["120h"].Data.Remove(0, 6).Contains("00 00 00 00 00 00")))
                        {
                            LogState(LogType.Info, "Cycler is suspended(0x120)!");
                            LogState(LogType.Info, "Receive Data: " + cycler.bmsList["120h"].Data.ToString());

                            return false;
                        }
                    }
                    #endregion
                }
                else
                {
                    return false;
                }

                var nowCycler = cycler.localDt;
                nowCycler = nowCycler.AddSeconds(5);

                if (DateTime.Now > nowCycler)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ConnectionCheck_CMC
        private bool ConnectionCheck_CMC(out int retryCount)
        {
            string checkDeviceName = "[CMC]";
            retryCount = 0;

             
            var nowCMCDt = cmc.localDt.AddSeconds(1);

            try
            {
                if (DateTime.Now <= nowCMCDt)
                {
                    if (isStop || ispause)
                    {
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                        return false;
                    }
                    LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                    }));

                    //200212 ht
                    LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                    return true;
                }
                else
                {
                    LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                    for (int i = 0; i < 3; i++)
                    {
                    	if (isStop || ispause)
                    {
                    	    return false;
                    	}

                        retryCount++;

                        Reboot_CMC();

                        nowCMCDt = cmc.localDt.AddSeconds(1);

                        if (DateTime.Now <= nowCMCDt)
                        {
                            LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);

                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                            }));

                            //200212 ht
                            LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                            return true;
                        }
                        else
                        {
                            LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                        }
                    }

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                    }));

                    return false;
                }
            }
            catch (Exception ec)
            {
                try
                {
                    LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName + " Exception : " + ec.ToString());

                    for (int i = 0; i < 3; i++)
                    {
                    	if (isStop || ispause)
                    	{
                        	return false;
                    	}

                        retryCount++;

                        Reboot_CMC();

                        nowCMCDt = cmc.localDt.AddSeconds(1);

                        if (DateTime.Now <= nowCMCDt)
                        {
                            LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);

                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                            }));

                            //200212 ht
                            LogState(LogType.TEST, checkDeviceName + " TEST :" + cmc.raw_cmcList["000A"].ToString());
                            return true;
                        }
                        else
                        {
                            LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                        }
                    }

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                    }));

                    return false;
                }
                catch (Exception ecs)
                {
                    LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName + " Exception : " + ecs.ToString());

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                    }));

                    return false;
                }
            }
        }
        #endregion

        #region ConnectionCheck_DAQ
        private bool ConnectionCheck_DAQ(out int retryCount)
        {
            string checkDeviceName = "[DAQ]";
            retryCount = 0;

            if (daq.sp == null)
            {
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName+" Initialize fail");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_daq.Background = System.Windows.Media.Brushes.Red;
                }));

                return false;
            }


            var nowDaqDt = daq.localDt.AddSeconds(5);

            if (DateTime.Now <= nowDaqDt)
            {
                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_daq.Background = System.Windows.Media.Brushes.Green;
                }));

                return true;
            }
            else
            {
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                for (int i = 0; i < 3; i++)
                {
                    if (isStop || ispause)
                    {
                        return false;
                    }
                    retryCount++;

                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                    }));

                    Thread.Sleep(1000);

                    nowDaqDt = daq.localDt.AddSeconds(5);

                    if (DateTime.Now <= nowDaqDt)
                    {
                        LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq.Background = System.Windows.Media.Brushes.Green;
                        }));

                        return true;
                    }
                    else
                    {
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                    }
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_daq.Background = System.Windows.Media.Brushes.Red;
                }));

                return false;
            }
        }
        #endregion

        #region ConnectionCheck_HIPOT
        private bool ConnectionCheck_HIPOT(out int retryCount)
        {
            string checkDeviceName = "[HIPOT]";
            retryCount = 0;

            if (chroma.IDN() != "")
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_hipot.Background = System.Windows.Media.Brushes.Green;
                }));

                LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);
                chroma.isAlive = true;
                return true;
            }
            else
            {
                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                for (int i = 0; i < 3; i++)
                {
                    if (isStop || ispause)
                    {
                        return false;
                    }
                    retryCount++;

                    chroma._Dispose();
                    chroma = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        chroma = new CChroma(this);
                    }));

                    Thread.Sleep(300);

                    if (chroma.IDN() != "")
                    {
                        LogState(LogType.DEVICE_CHECK, "[Success] - " + checkDeviceName);

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_hipot.Background = System.Windows.Media.Brushes.Green;
                        }));

                        return true;
                    }
                    else
                    {
                        LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);
                    }
                }

                LogState(LogType.DEVICE_CHECK, "[Fail] - " + checkDeviceName);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_hipot.Background = System.Windows.Media.Brushes.Red;
                }));

                return false;
            }
        }
        #endregion

        #region Not Use
        private void TempCheck(TestItem ti)
        {
            string checkDeviceName = "[TEMP]";
            var now_plus_10s_dt = temps.localDt1;
            now_plus_10s_dt = now_plus_10s_dt.AddSeconds(10);

            if (DateTime.Now > now_plus_10s_dt)
            {
                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                temps = null;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    temps = new CNhtRS232_Receiver(this);
                }));

                Thread.Sleep(100);

                now_plus_10s_dt = temps.localDt1;
                now_plus_10s_dt = now_plus_10s_dt.AddSeconds(10);
                if (DateTime.Now > now_plus_10s_dt)
                {
                    LogState(LogType.TEST, checkDeviceName + " ReConnection FAIL !! Restart EOL Program");
                    ViewRetryWindow(checkDeviceName, ti);
                }
                else
                    LogState(LogType.TEST, checkDeviceName + " TEST :" + temps.tempStr.ToString());
            }
            else
            {
                LogState(LogType.TEST, checkDeviceName + " TEST :" + temps.tempStr.ToString());
            }
        }

        private void DMMCheck(TestItem ti, int type)
        {
            string checkDeviceName = "[DMM]";
            //0이면 TCP, 1이면 Serial
            if (type == 0)
            {
                DMMTCPCheck(ti, checkDeviceName);
            }
            else if(type == 1)
            {
                DMMSerialCheck(ti, checkDeviceName);
            }
            
        }
        /// <summary>
        /// DMM TCP Type Connect Check
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="deviceName"></param>
        private void DMMTCPCheck(TestItem ti, string deviceName)
        {
            if (keysight != null && keysight.isAlive)
            {
                float res = keysight.TrySend("MEAS:VOLT:DC?\n");
                if (res != 0)
                    LogState(LogType.TEST, deviceName + " TEST :" + res);
                else
                {
                    LogState(LogType.TEST, deviceName + " Communication FAIL !! retry connection");                    
                    keysight = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        keysight = new CKeysightDMM(this, dmmIP, dmmPort);
                    }));
                    Thread.Sleep(300);

                    res = keysight.TrySend("MEAS:VOLT:DC?\n");
                    if (res != 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
            }
            else
            {
                LogState(LogType.TEST, deviceName + " Connection FAIL !! retry connection");

                if (keysight != null && keysight.isAlive != true)
                {
                    LogState(LogType.TEST, deviceName + " Dispose resources");

                    keysight.Dispose();
                    keysight = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    keysight = new CKeysightDMM(this, dmmIP, dmmPort);
                }));
                Thread.Sleep(300);

                if (keysight != null && keysight.isAlive)
                {
                    float res = keysight.TrySend("MEAS:VOLT:DC?\n");
                    if (res != 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
                else
                {
                    ViewRetryWindow(deviceName, ti);
                }
            }
        }
        /// <summary>
        /// DMM Serial Connect Check
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="deviceName"></param>
        private void DMMSerialCheck(TestItem ti, string deviceName)
        {
            if (keysight != null && keysight.isAlive)
            {
                string res = keysight.Read_Version();
                if (res.Length > 0)
                    LogState(LogType.TEST, deviceName + " TEST :" + res);
                else
                {
                    LogState(LogType.TEST, deviceName + " Communication FAIL !! retry connection");
                    keysight.Dispose();
                    keysight = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        keysight = new CKeysightDMM(this, true, this.keysight_PortName);
                    }));
                    Thread.Sleep(300);

                    res = keysight.Read_Version();
                    if (res.Length > 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
            }
            else
            {
                LogState(LogType.TEST, deviceName + " Connection FAIL !! retry connection");

                if (keysight != null && keysight.isAlive != true)
                {
                    LogState(LogType.TEST, deviceName + " Dispose resources");

                    keysight.Dispose();
                    keysight = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    keysight = new CKeysightDMM(this, true, this.keysight_PortName);
                }));
                Thread.Sleep(300);

                if (keysight != null && keysight.isAlive)
                {
                    string res = keysight.Read_Version();
                    if (res.Length > 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
                else
                {
                    ViewRetryWindow(deviceName, ti);
                }
            }
        }

        /// <summary>
        /// DAQ Connect Check
        /// </summary>
        /// <param name="ti">Testitem</param>
        private void DAQCheck(TestItem ti)
        {
            string checkDeviceName = "[DAQ]";
            if (daq != null && daq.sp.IsOpen)
            {
                var nowDaqDt = daq.localDt;
                nowDaqDt = nowDaqDt.AddSeconds(2);

                if (DateTime.Now > nowDaqDt)
                {
                    LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;
                    Thread.Sleep(100);
                    DAQErrcnt++;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        daq = new CDAQ(this, daq_PortName);
                    }));

                    Thread.Sleep(200);

                    if (daq != null && daq.sp.IsOpen)
                    {
                        nowDaqDt = daq.localDt;
                        nowDaqDt = nowDaqDt.AddSeconds(2);

                        if (DateTime.Now > nowDaqDt)
                        {
                            DAQErrcnt++;
                            ViewRetryWindow(checkDeviceName, ti);
                        }
                        else
                            LogState(LogType.TEST, checkDeviceName + " TEST :" + daq.sp.ToString());

                    }
                }

                LogState(LogType.TEST, checkDeviceName + " TEST :" + daq.sp.ToString());
            }
            else
            {
                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                if (daq != null && daq.sp.IsOpen)
                {
                    LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    daq = new CDAQ(this, daq_PortName);
                }));

                if (daq != null && daq.sp.IsOpen)
                {
                    LogState(LogType.TEST, checkDeviceName + " TEST :" + daq.sp.ToString());
                }
                else
                {
                    ViewRetryWindow(checkDeviceName, ti);
                }
            }
            LogState(LogType.Info, "DAQ Error Count : " + DAQErrcnt);
        }

        #region ConnectionCheck_MES
        private bool ConnectionCheck_MES(out int retryCount)
        {
            string checkDeviceName = "[MES]";
            retryCount = 0;

            for (int i = 0; i < 3; i++)
            {
                if (isStop || ispause)
                {
                    return false;
                }

                if (!MES.heartbeating())
                {
                    LogState(LogType.Fail, string.Format("Not connected MES. {0}", (i + 1).ToString()));
                    MES.StartConnect();
                    Thread.Sleep(1000);
                }
                else
                {
                    LogState(LogType.Success, "Connected MES.");
                    tlamp.SetTLampMESStatus(true);
                    return true;
                }
            }
            tlamp.SetTLampMESStatus(false);

            return false;
        }
        #endregion

        /// <summary>
        /// Cycler Connect Check
        /// </summary>
        /// <param name="ti">Testitem</param>
        /// not used
        private void CyclerCheck(TestItem ti)
        {
            string checkDeviceName = "[CYCLER]";

            if (cycler != null && cycler.isAlive1)
            {
                cycler.m_LastMsgsList1.Clear();
                cycler.bmsList.Clear();
                Thread.Sleep(500);

                var nowCycler = cycler.localDt;
                nowCycler = nowCycler.AddSeconds(5);

                if (DateTime.Now > nowCycler)
                {
                    SetMainCState("NOT READY");
                    ViewRetryWindow(checkDeviceName, ti);
                }
                LogState(LogType.TEST, checkDeviceName + " TEST :" + cycler.cycler1OP.ToString());
            }
            else
            {
                SetMainCState("NOT READY");
                ViewRetryWindow(checkDeviceName, ti);
            }
        }
        /// <summary>
        /// Chroma Connect Check
        /// </summary>
        /// <param name="ti">Testitem</param>
        private void ChromaCheck(TestItem ti)
        {
            string checkDeviceName = "[Hipot]";
            if (chroma.IDN() != "")
            {
                LogState(LogType.TEST, checkDeviceName + " Hipot Conneted.");
            }
            else
            {
                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! Restart EOL Program");
                ViewRetryWindow(checkDeviceName, ti);
            }
        }

        public bool isCyclerFail = false;
        public bool isDeviceFail = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDeviceName">Device Name</param>
        /// <param name="ti">Testitem</param>
        private void ViewRetryWindow(string checkDeviceName, TestItem ti)
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

            //ng스킵일때 계측기에서 멈춰도 계속 진행함
            if (isSkipNG_)
            {
                return;
            }
             
            this.Dispatcher.Invoke(new Action(() =>
            {
                DeviceCheckWindow rt = new DeviceCheckWindow(this);
                rt.Height += 100;
                rt.Width += 200;
                rt.maintitle.FontSize += 20;
                rt.maintitle.Content = "DEVICE NOT READY";
                rt.reason.FontSize += 13;
                rt.reason.Content = checkDeviceName + " is NOT ready to TEST !!\nPlease restart EOL Program";
                rt.okbt.FontSize += 13;
                rt.Show();
            }));

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isManual)
                    blinder.Visibility = System.Windows.Visibility.Hidden;
            }));
        }
        #endregion
    }
}