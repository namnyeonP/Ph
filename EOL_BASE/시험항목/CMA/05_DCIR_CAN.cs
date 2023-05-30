using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        string max_cell_volt_before_dcir;
        string max_cell_volt_after_dcir;

        // DCIR 후 셀 전압이 최대인 셀의 델타값을 구한다.
        public bool DeltaMaxCellVoltageAfterDCIR(TestItem ti)
        {
            isProcessingUI(ti);

            InitDCIRVariables();

            // 검사 루틴
            // 1. 최대 셀 전압값을 먼저 계측
            // 2. DCIR
            // 3. DCIR 이후 최대 셀 전압값을 계측
            // 4. Delta 값 계산

            max_cell_volt_before_dcir = Hybrid_Instru_CAN.GetBMSData(210, 13, 13);
            double point1 = double.Parse(max_cell_volt_before_dcir) * 0.001;

            #region DCIR

            if (!cycler.isAlive1)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }

            if (counter_Cycler_limit < counter_Cycler)
            {
                ti.Value_ = "LIMIT OF CYCLER COUNT";
                return JudgementTestItem(ti);
            }

            calAvg = temps.tempStr;
            LogState(LogType.Info, "Room Temp :" + calAvg.ToString());

            cycler.is84On = false;

            ti.refValues_.Clear();

            while (GetCyclerFlag())
            {
                LogState(LogType.Info, "Waiting to Use Cycler", null, true, false);
                Thread.Sleep(100);
                if (isStop || ispause)
                {
                    DCIR_Stopped();
                    ti.Value_ = "EMERGENCY_STOPPED";
                    return JudgementTestItem(ti);
                }
            }

            SetEnableCyclerFlag();

            if (!ModeSet(ti) || !Do_Rest_Charge(ti) || !ModeSet_Release(ti))
            {
                SetDisableCyclerFlag();
                return false;
            }

            SetDisableCyclerFlag();

            Thread.Sleep(1000);

            SetMainVoltage("0.00");
            SetMainCurrent("0.00");
            SetMainCState("Ready");
            Thread.Sleep(500);

            #endregion

            max_cell_volt_after_dcir = Hybrid_Instru_CAN.GetBMSData(210, 13, 13);
            double point2 = double.Parse(max_cell_volt_after_dcir) * 0.001;

            SetCounter_Cycler();

            double delta_max_cell_volt = Math.Abs(point2 - point1);
            LogState(LogType.TEST, "Delta Max Cell Voltage after DCIR: " + string.Format("{0} - {1} = {3}", point2, point1, Math.Abs(point2 - point1)));

            ti.Value_ = delta_max_cell_volt;

            return JudgementTestItem(ti);
        }

        // DCIR 후 셀 전압이 최대인 셀의 DCIR을 구한다.
        public bool DCIRMaxCellVoltage(TestItem ti)
        {
            isProcessingUI(ti);

            // 충방전기가 연결되지 않았을 때 값 처리가 되지 않아 오류가 나는것을 방지하기 위함
            if (max_cell_volt_before_dcir == null)
            {
                max_cell_volt_before_dcir = "0.0";
            }

            if (max_cell_volt_after_dcir == null)
            {
                max_cell_volt_after_dcir = "0.0";
            }

            double point1 = double.Parse(max_cell_volt_before_dcir) * 0.001;
            double point2 = double.Parse(max_cell_volt_after_dcir) * 0.001;

            // 여기는 일단 계산식이 없어서 이전에 사용하던 것 그대로 사용
            double dcir_max_cell_volt = Math.Abs(((point2 - point1) / endCurrent_DIS) * 1000); // 밀리옴
            LogState(LogType.TEST, "DCIR Value: " + string.Format("((({0} - {1}) / {2}) * 1000 = {3}))", point1, point2, endCurrent_DIS, (((point1 - point2) / endCurrent_DIS) * 1000)));

            ti.Value_ = dcir_max_cell_volt;

            return JudgementTestItem(ti);
        }
    }
}
