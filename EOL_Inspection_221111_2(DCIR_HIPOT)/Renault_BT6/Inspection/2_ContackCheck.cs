using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixonLibrary.Device;

namespace Renault_BT6
{
    public partial class MainWindow
    {

        
        public bool ContactCheck_Bushing(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                double cap = 0.0;

                var judge = Device.Chroma.GetCapacitance(EOL.IR_Recipe.BushingUpperCH,
                                                                         EOL.IR_Recipe.BushingLowCH,
                                                                         out cap);

                if( judge == Judge.EMERGENCY)
                {
                    ti.Value_ = _EMG_STOPPED;
                }
                else if(judge == Judge.DISCONNECT)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                }
                else
                {
                    ti.Value_ = cap;
                }

                if (_clResultData[0] == "")
                {
                    SetContorlLimitData(0, ti.Value_.ToString());
                }
                else
                {
                    SetContorlLimitData(6, ti.Value_.ToString());
                }
            
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }


        public bool ContactCheck_CoolingPlate(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                double cap = 0.0;

                var judge = Device.Chroma.GetCapacitance(EOL.IR_Recipe.PlusCH,
                                                                        EOL.IR_Recipe.CoolingPlateLeftCH,
                                                                        out cap);

                if (judge == Judge.EMERGENCY)
                {
                    ti.Value_ = _EMG_STOPPED;
                }
                else if (judge == Judge.DISCONNECT)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                }
                else
                {
                    ti.Value_ = cap;
                }

                if (_clResultData[1] == "")
                {
                    SetContorlLimitData(1, ti.Value_.ToString());
                }
                else
                {
                    SetContorlLimitData(7, ti.Value_.ToString());
                }
                
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }
    }
}
