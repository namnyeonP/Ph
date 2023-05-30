using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.클래스
{
    public class SafetyCyclerSetting
    {
        PGLB global = new PGLB();

        double safetyVoltageLevel;
        double safetyCurrentLevel;

        // 일단은 내부에서 사용하도록 만들었고
        // 외부에서 사용하도록 변경해야 할 수도 있을 것 같아서 프로퍼티 만들어두었음.
        public double SafetyVoltageLevel
        {
            set { safetyVoltageLevel = value; }
            get { return safetyVoltageLevel; }
        }

        public double SafetyCurrentLevel
        {
            set { safetyCurrentLevel = value; }
            get { return safetyCurrentLevel; }
        }

        public SafetyCyclerSetting()
        {
        }

        public string CheckCyclerSpec(string volt, string curr)
        {
            string str = "";

            double voltage = double.Parse(volt);
            double current = double.Parse(curr);

            if (voltage > safetyVoltageLevel)
            {
                str += string.Format("Cycler Voltage over spec. Safety Spec: {0}, MES Spec: {1}", safetyVoltageLevel, voltage);
            }

            if (current > safetyCurrentLevel)
            {
                if (str != "")
                {
                    str += "\n";
                }
                str += string.Format("Cycler Currnet over spec. Safety Spec: {0}, MES Spec: {1}", safetyCurrentLevel, current);
            }

            return str;
        }

        public void LoadSafetyCyclerSpec()
        {
            StringBuilder sb = new StringBuilder();

            global.GetConfigData("CyclerSafetyCondition", "Volt", "", sb, 15);
            if (sb.ToString() != "") { safetyVoltageLevel = double.Parse(sb.ToString()); };

            global.GetConfigData("CyclerSafetyCondition", "Curr", "", sb, 15);
            if (sb.ToString() != "") { safetyCurrentLevel = double.Parse(sb.ToString()); };
        }
    }
}
