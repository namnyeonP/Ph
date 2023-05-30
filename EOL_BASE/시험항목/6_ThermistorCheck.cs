using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        double thermistor1 = 0.0;
        double thermistor2 = 0.0;
        
        public bool Module_Thermistor_1(TestItem ti)
        {
            double tempTher1 = 0.0;

            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (isLineFlag)
            {
                #region DAQ970
                if (daq970_1 == null)
                {
                    ti.Value_ = "NotConnected";
                    return JudgementTestItem(ti);
                }
          
                if (daq970_1.MeasRes_Multi(out tempList, "318,319", 2))
                {
                    tempList[0] = tempList[0] * 0.001;
                    tempList[1] = tempList[1] * 0.001;

                    tempTher1 = tempList[0];
                    LogState(LogType.RESPONSE, "KeysightDAQ970:" + tempList[0].ToString() + tempList[1].ToString());
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }
                #endregion
            }
            else
            {
                #region DMM34970
                if (keysight == null)
                {
                    ti.Value_ = "NotConnected";
                    return JudgementTestItem(ti);
                }

                keysight.rtstring = "";
                tempCnt = 2;
                keysight.MeasTemp("318,319");

                int rec = keysight.sp.BytesToRead;

                int cnt = 0;
                while (rec < 33)//33
                {
                    Thread.Sleep(100);
                    rec = keysight.sp.BytesToRead;
                    cnt += 100;
                    if (cnt == 2000)
                    {
                        keysight.MeasTemp("318,319");

                        rec = keysight.sp.BytesToRead;

                        cnt = 0;
                        while (rec < 33)//33
                        {
                            Thread.Sleep(100);
                            rec = keysight.sp.BytesToRead;
                            cnt += 100;
                            if (cnt == 2000)
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
                LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                tempList = new List<double>();
                foreach (var item in vArr)
                {
                    double dv = 0;
                    if (double.TryParse(item, out dv))
                    {
                        tempList.Add(dv * 0.001);
                    }
                }

                if (tempList != null && tempList.Count == tempCnt)
                {
                    tempTher1 = tempList[0];
                }
                #endregion
            }


            double resultTemp = 0.0;
            //tempTher1 = 38.0606;
            Calculate(tempTher1, out resultTemp);
            thermistor1 = resultTemp;
            ti.Value_ = resultTemp;

            dcirTemp = temps.tempStr;
            LogState(LogType.Info, "Room Temp :" + dcirTemp.ToString());
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4001", dcirTemp.ToString("N3")));


            return JudgementTestItem(ti);
        }
        public bool Module_Thermistor_2(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            var tempTher1 = 0.0;
            if (tempList != null && tempList.Count == tempCnt)
            {
                tempTher1 = tempList[1];
            }
            double resultTemp = 0.0;
            Calculate(tempTher1, out resultTemp);
            thermistor2 = resultTemp;
            ti.Value_ = resultTemp;

            
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4046", thermistor1.ToString() + "&" + thermistor2.ToString()));

            return JudgementTestItem(ti);
        }
        public bool Thermistor_Difference(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            var tempTher1 = 0.0;
            if (tempList != null && tempList.Count == tempCnt)
            {
                tempTher1 = thermistor1 - thermistor2;
                LogState(LogType.Info, string.Format("{0} - {1} = {2}", thermistor1, thermistor2, tempTher1));
            }
            ti.Value_ = tempTher1;
            return JudgementTestItem(ti);
        }
        /// <summary>
        /// 상세수집을 위한 Module 온도 측정
        /// </summary>
        /// <returns></returns>
        private string getModuleTemp1()
        {
            string temp = "";

            keysight.MeasTemp("318");

            int rec = keysight.sp.BytesToRead;

            int cnt = 0;
            while (rec < 17)//33
            {

            }

            byte[] bt = new byte[rec];
            keysight.sp.Read(bt, 0, rec);

            string str = Encoding.Default.GetString(bt, 0, rec);
            var vArr = str.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<double> templist = new List<double>();
            foreach (var item in vArr)
            {
                double dv = 0;
                if (double.TryParse(item, out dv))
                {
                    templist.Add(dv * 0.001);
                }
            }

            var tempTher1 = 0.0;
            if (templist != null && templist.Count == 1)
            {
                tempTher1 = templist[0];
            }
            temp = tempTher1.ToString();
            return temp;
        }
        /// <summary>
        /// 상세수집을 위한 Module 온도 측정
        /// </summary>
        /// <returns></returns>
        private string getModuleTemp2()
        {
            string temp = "";

            keysight.MeasTemp("319");

            int rec = keysight.sp.BytesToRead;

            int cnt = 0;
            while (rec < 17)//33
            {

            }

            byte[] bt = new byte[rec];
            keysight.sp.Read(bt, 0, rec);

            string str = Encoding.Default.GetString(bt, 0, rec);
            var vArr = str.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<double> templist = new List<double>();
            foreach (var item in vArr)
            {
                double dv = 0;
                if (double.TryParse(item, out dv))
                {
                    templist.Add(dv * 0.001);
                }
            }

            var tempTher1 = 0.0;
            if (templist != null && templist.Count == 1)
            {
                tempTher1 = templist[0];
            }
            temp = tempTher1.ToString();
            return temp;
        }

        // 2019.05.08 jeonhj's comment
        // 측정 구간마다 저장되는 값이 달라야하므로 파라미터로 temp1, 2를 받는다.
        //private string getModuleTemp1N2(out double avg)
        private string getModuleTemp1N2(out double avg, out double temp1, out double temp2)
        { 
            temp1 = 0.0;
            temp2 = 0.0;

            if (isLineFlag)
            {
                #region DMM970A
                if (daq970_1.MeasRes_Multi(out tempList, "318,319", 2))
                {
                    tempList[0] = tempList[0] * 0.001;
                    tempList[1] = tempList[1] * 0.001;
                }
                #endregion
            }
            else
            {
                #region DMM34970
                keysight.rtstring = "";
                tempCnt = 2;
                keysight.MeasTemp("318,319");

                int rec = keysight.sp.BytesToRead;

                int cnt = 0;
                while (rec < 33)//33
                {
                    Thread.Sleep(100);
                    rec = keysight.sp.BytesToRead;
                    cnt += 100;
                    if (cnt == 2000)
                    {
                        keysight.MeasTemp("318,319");

                        rec = keysight.sp.BytesToRead;

                        cnt = 0;
                        while (rec < 33)//33
                        {
                            Thread.Sleep(100);
                            rec = keysight.sp.BytesToRead;
                            cnt += 100;
                            avg = 0;
                            return "0&0";
                        }
                        break;
                    }
                }
                //받은 후에 데이터 파싱
                byte[] bt = new byte[rec];
                keysight.sp.Read(bt, 0, rec);

                keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);
                LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                tempList = new List<double>();
                foreach (var item in vArr)
                {
                    double dv = 0;
                    if (double.TryParse(item, out dv))
                    {
                        tempList.Add(dv * 0.001);
                    }
                }
                #endregion

            }

            double resultTemp1 = 0.0;
            //tempTher1 = 38.0606;
            Calculate(tempList[0], out resultTemp1);
            //discharge10sTemp1 = resultTemp1;
            temp1 = resultTemp1;

            double resultTemp2 = 0.0;
            //tempTher1 = 38.0606;
            Calculate(tempList[1], out resultTemp2);
            //discharge10sTemp2 = resultTemp2;
            temp2 = resultTemp2;

            avg = (resultTemp1 + resultTemp2) / 2;
            return resultTemp1.ToString() + "&" + resultTemp2.ToString();
        }




    }
}