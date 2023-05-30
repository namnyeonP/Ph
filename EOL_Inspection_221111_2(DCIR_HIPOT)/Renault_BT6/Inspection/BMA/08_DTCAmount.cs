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
        public bool DTCCheck(TestItem ti)
        {
            isProcessingUI(ti);
            if (SetDefaultMode())
            {

                var rList = new List<string>();
                GetToDID_singleData(0x02, 0x8F, " ", true, out rList, 0x03, 0x19);

                if (rList != null)
                {
                    List<string> dtcs = new List<string>();

                    for (int i = 0; i < rList.Count; i = i + 4)
                    {
                        if ((i + 3) > rList.Count-1)
                            break;

                        if (rList[i + 3] == "FF")
                            continue;

                        dtcs.Add(rList[i] + " " + rList[i + 1] + " " + rList[i + 2] + " " + rList[i + 3]);
                    }

                    int cnt = 0;
                    foreach (var dtc in dtcs)
                    {
                        if (dtc == "00 00 00 00")
                            continue;

                        LogState(LogType.Info, "[DTC] " + dtc);
                        if (dtc.Contains("06 00 00") ||
                            dtc.Contains("25 59 00") ||
                            dtc.Contains("90 87 87") ||
                            dtc.Contains("C0 64 87") ||
                            dtc.Contains("C1 00 00") ||
                            dtc.Contains("C1 00 87") ||
                            dtc.Contains("59 02 FF") )
                        {
                            LogState(LogType.Info, "[DTC] Ignore DTC:" + dtc);
                        }
                        else
                        {
                            LogState(LogType.Info, "[DTC] DTC:" + dtc);
                            cnt++;
                        }
                    }

                    ti.Value_ = cnt;
                }
                else
                {
                    ti.Value_ = _NOT_POS_MSG;
                }
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            }
            return JudgementTestItem(ti);
        }

    }
}