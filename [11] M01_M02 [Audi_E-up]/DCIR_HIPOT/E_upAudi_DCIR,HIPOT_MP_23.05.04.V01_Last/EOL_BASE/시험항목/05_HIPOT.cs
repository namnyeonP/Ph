﻿using EOL_BASE.클래스;
using EOL_BASE.모듈;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EOL_BASE.Forms;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        int connectionCheckRetryCount1 = 0;
        int connectionCheckRetryCount2 = 0;

        /// <summary>
        /// 접촉성검사 1
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck1(TestItem ti)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            isProcessingUI(ti);

            connectionCheckRetryCount1 = -1;
            
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            int Channel_HI = 0;
            int Channel_LO = 0;

            if (!SetChannels(out Channel_HI, out Channel_LO))
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    double cap = -1;
                    connectionCheckRetryCount1++;
                    chroma.GetCapacitance(Channel_HI, Channel_LO, out cap);

                    if (chroma.judgementStr == this._CHAN_HI_FAIL ||
                        chroma.judgementStr == this._CHAN_LO_FAIL)
                    {
                        ti.Value_ = chroma.judgementStr;
                        break;
                    }

                    if (cap == -1)
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                    }
                    else
                    {
                        ti.Value_ = cap * 1000000000;// 0.000001;
                    }

                    if (ispause)
                    {
                        this.LogState(LogType.Info, "Pause Stopped");
                        ti.Value_ = _EMG_STOPPED;
                        break;
                    }

                    //200616 _HIPOT_EMO 추가
                    if (isStop)
                    {
                        this.LogState(LogType.Info, "EMO Stopped");
                        ti.Value_ = _HIPOT_EMO;
                        break;
                	}
                
                    double rst = 0.0;
                    if (double.TryParse(ti.Value_.ToString(), out rst))
                    {
                        double maxT = double.Parse(ti.Max.ToString());
                        double minT = double.Parse(ti.Min.ToString());
                        LogState(LogType.Info, "value : " + rst.ToString());
                        if (rst > maxT || rst < minT)
                        {
                            LogState(LogType.Fail, "Retry Contact check CNT :" + (i + 1).ToString());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                    }
                }


            }
            catch (Exception ec)
            {
                ti.Value_ = (double)-1;
                return JudgementTestItem(ti);
            }
            this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
            sp.Stop();
            sp = null;

            if (JudgementTestItem(ti) == false)
            {
                isMESskip = true;  // ks yoo 0802
            }


            return JudgementTestItem(ti);
        }

        /// <summary>
        /// 접촉성검사 2
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck2(TestItem ti)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            int Channel_HI = 0;
            int Channel_LO = 0;

            if (!SetChannels(out Channel_HI, out Channel_LO))
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }

            connectionCheckRetryCount2 = -1;

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    double cap = -1;
                    connectionCheckRetryCount2++;
                    chroma.GetCapacitance(Channel_HI, Channel_LO, out cap);

                    if (chroma.judgementStr == this._CHAN_HI_FAIL ||
                        chroma.judgementStr == this._CHAN_LO_FAIL)
                    {
                        ti.Value_ = chroma.judgementStr;
                        break;
                    }

                    if (cap == -1)
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                    }
                    else
                    {
                        ti.Value_ = cap * 1000000000;// 0.000001;
                    }

                    if (ispause)
                    {
                        this.LogState(LogType.Info, "Pause Stopped");
                        ti.Value_ = _EMG_STOPPED;
                        break;
                    }

                    //200616 _HIPOT_EMO 추가
                    if (isStop)
                    {
                        this.LogState(LogType.Info, "EMO Stopped");
                        ti.Value_ = _HIPOT_EMO;
                        break;
                    }

                    double rst = 0.0;
                    if (double.TryParse(ti.Value_.ToString(), out rst))
                    {
                        double maxT = double.Parse(ti.Max.ToString());
                        double minT = double.Parse(ti.Min.ToString());
                        LogState(LogType.Info, "value : " + rst.ToString());
                        if (rst > maxT || rst < minT)
                        {
                            LogState(LogType.Fail, "Retry Contact check CNT :" + (i + 1).ToString());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                    }

                }


            }
            catch (Exception ec)
            {
                ti.Value_ = (double)-1;
                return JudgementTestItem(ti);
            }
            this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
            sp.Stop();
            sp = null;

            if (JudgementTestItem(ti) == false)
            {
                isMESskip = true;  // ks yoo 0804
            }


            return JudgementTestItem(ti);
        }

        /// <summary>
        /// 절연저항검사
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool InsulationResistance1(TestItem ti)
        {

            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            // 2203222 Retry
            for (int x = 0; x < 1; x++)
            {
                Stopwatch sp = new Stopwatch();
                sp.Start();

                int Channel_HI = 0;
                int Channel_LO = 0;

                if (!SetChannels(out Channel_HI, out Channel_LO) || chroma.isAlive == false)
                {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
                }

                try
                {
                    double current = -1;
                    var rtv = chroma.GetResistance(Channel_HI, Channel_LO, out current);
                    SetValueByReturnValue_Insulation(rtv, current, ti);
                }
                catch (Exception ec)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }
                this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
                sp.Stop();
                sp = null;

                if (ispause)
                {
                    this.LogState(LogType.Info, "Pause Stopped");
                    ti.Value_ = _EMG_STOPPED;
                }

                //200616 _HIPOT_EMO 추가
                if (isStop)
                {
                    this.LogState(LogType.Info, "EMO Stopped");
                    ti.Value_ = _HIPOT_EMO;
                }

                // 2203222 Retry
                if (JudgementTestItem(ti))
                {
                    //PASS
                    break;
                }
                else
                {
                    //NG
                    if (isSkipNG_)
                    {
                        break;
                    }
                    LogState(LogType.Fail, "IR data error");
                }
            }
            return JudgementTestItem(ti);
        }


        /// <summary>
        /// 절연저항검사
        /// fetch 값 사용
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool InsulationResistance_fetch(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            //221106 소모품 카운트
            FormPartsCountHipotSetting PCHS = new FormPartsCountHipotSetting(this, "START");
            PCHS.CountRealData();
            this.SetPartsCountData(ti, "StartRun");

            // 220208 Retry
            for (int x = 0; x < 1; x++)
            {
                Stopwatch sp = new Stopwatch();
                sp.Start();

                int Channel_HI = 0;
                int Channel_LO = 0;

                if (!SetChannels(out Channel_HI, out Channel_LO) || chroma.isAlive == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                try
                {
                    double current = -1;
                    chroma.isUseFetch = true;
                    //fetch_list.Clear();

                    var rtv = chroma.GetResistance(Channel_HI, Channel_LO, out current);
                    SetValueByReturnValue_Insulation(rtv, current, ti);
                    chroma.LastFetchDataProc(rtv, ti.Value_); // SetValueByReturnValue_Insulation 에서 단위 환산이 이뤄지기 때문에 적용 시점은 이곳이 맞다

                    chroma.isUseFetch = false;
                }
                catch (Exception ec)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    chroma.isUseFetch = false;
                    return JudgementTestItem(ti);
                }
                this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
                sp.Stop();
                sp = null;

                if (ispause)
                {
                    this.LogState(LogType.Info, "Pause Stopped");
                    ti.Value_ = _EMG_STOPPED;
                    break;
                }

                //200616 _HIPOT_EMO 추가
                if (isStop)
                {
                    this.LogState(LogType.Info, "EMO Stopped");
                    ti.Value_ = _HIPOT_EMO;
                    break;
                }

                // 220208 Retry
                if (JudgementTestItem(ti))
                {
                    //PASS
                    break;
                }
                else
                {
                    //NG
                    if(isSkipNG_)
                    {
                        break;
                    }
                    LogState(LogType.Fail, "IR NG Retry");
                }
            }

            #region 패치 데이터 로깅 참고

            /* 주의 : LogState가 Queue 구조가 아님으로 많은 라인 Write 처리시 씹힐수 있다
             * 딜레이 적용으로 해소 가능하나 택이 늘어나고
             * LogState 내부 \n 삭제 코드를 구분 하여 문자열을 한번에 모아서 1번만 Write 처리를 하면 되지만
             * 가능한 디버깅 용도가 아닌 이상, 아래의 코드 적용 하지 말 것
            */

            //데이터 전부 로깅
            /*
            foreach (var list in chroma.m_mapFetchData)
            {
                foreach(var data in list.Value)
                {
                    string strDebug = string.Empty;
                    strDebug += ("key :"        + list.Key.ToString()           + ", ");
                    strDebug += ("collect :"    + list.Value.Count.ToString()   + ", ");
                    strDebug += ("realtime :"   + data.m_strProcTime            + ", ");
                    strDebug += ("measure :"    + data.m_strMeasure             + ", ");
                    strDebug += ("volt :"       + data.m_strOutVolt);
                    LogState(LogType.Info, strDebug);
                }
            }
            */

            //단위(10초 간격)의 마지막 데이터만 로깅
            /*
            foreach (var key in Enum.GetValues(typeof(EN_FETCH_KEY)))
            {
                CFecthData list = chroma.GetLastFecthData((EN_FETCH_KEY)key);

                if (list != null)
                {
                    string strDebug = string.Empty;
                    strDebug += ("key :"        + key.ToString()                + ", ");
                    strDebug += ("realtime:"    + list.m_strProcTime            + ", ");
                    strDebug += ("measure :"    + list.m_strMeasure             + ", ");
                    strDebug += ("volt :"       + list.m_strOutVolt);
                    LogState(LogType.Info, strDebug);
                }
            }
            */

            #endregion

            #region 패치 디테일 아이템 가공

            string keyVolt = string.Empty;
            string keyTime = string.Empty;
            string keyMeasure = string.Empty;
            
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                keyVolt = KEY_HIP_POR_NOR_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_POR_NOR_FETCH_IR_TIME;
                keyMeasure = KEY_HIP_POR_NOR_FETCH_IR_MEASURE;
            }

            if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                keyVolt = KEY_HIP_POR_F_L_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_POR_F_L_FETCH_IR_TIME;
                keyMeasure = KEY_HIP_POR_F_L_FETCH_IR_MEASURE;
            }
            //221101 wjs add mase m183
            if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                keyVolt = KEY_HIP_MAS_M183_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_MAS_M183_FETCH_IR____TIME;
                keyMeasure = KEY_HIP_MAS_M183_FETCH_IR_MEASURE;
            }

            if (localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.E_UP)
            {
                keyVolt = KEY_HIP_MAS_NOR_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_MAS_NOR_FETCH_IR_TIME;
                keyMeasure = KEY_HIP_MAS_NOR_FETCH_IR_MEASURE;
            }

            if(keyVolt != null && keyTime != null && keyMeasure != null)
            {
                DetailItems dtVolt = new DetailItems() { Key = keyVolt };
                DetailItems dtTime = new DetailItems() { Key = keyTime };
                DetailItems dtMeasure = new DetailItems() { Key = keyMeasure };

                foreach (var list in chroma.m_mapFetchData)
                {
                    foreach (var data in list.Value)
                    {
                        dtVolt.Reportitems.Add(data.m_strOutVolt);
                        dtTime.Reportitems.Add(data.m_strProcTime);
                        dtMeasure.Reportitems.Add(data.m_strMeasure);
                    }
                }

                ti.refValues_.Add(dtVolt);
                ti.refValues_.Add(dtTime);
                ti.refValues_.Add(dtMeasure);
            }

            #endregion

            return JudgementTestItem(ti);
        }

        public bool InsulationResistance_fetch_J1PA(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            // 220208 Retry
            for (int x = 0; x < 1; x++)
            {
                Stopwatch sp = new Stopwatch();
                sp.Start();

                int Channel_HI = 0;
                int Channel_LO = 0;

                if (!SetChannels(out Channel_HI, out Channel_LO) || chroma.isAlive == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                try
                {
                    double current = -1;
                    chroma.isUseFetch = true;
                    //fetch_list.Clear();

                    var rtv = chroma.GetResistance(Channel_HI, Channel_LO, out current);
                    SetValueByReturnValue_Insulation(rtv, current, ti);
                    chroma.LastFetchDataProc(rtv, ti.Value_); // SetValueByReturnValue_Insulation 에서 단위 환산이 이뤄지기 때문에 적용 시점은 이곳이 맞다

                    chroma.isUseFetch = false;
                }
                catch (Exception ec)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    chroma.isUseFetch = false;
                    return JudgementTestItem(ti);
                }
                this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
                sp.Stop();
                sp = null;

                if (ispause)
                {
                    this.LogState(LogType.Info, "Pause Stopped");
                    ti.Value_ = _EMG_STOPPED;
                    break;
                }

                //200616 _HIPOT_EMO 추가
                if (isStop)
                {
                    this.LogState(LogType.Info, "EMO Stopped");
                    ti.Value_ = _HIPOT_EMO;
                    break;
                }

                // 220208 Retry
                if (JudgementTestItem(ti))
                {
                    //PASS
                    break;
                }
                else
                {
                    //NG
                    if (isSkipNG_)
                    {
                        break;
                    }
                    LogState(LogType.Fail, "IR NG Retry");
                }
            }

            #region 패치 데이터 로깅 참고

            /* 주의 : LogState가 Queue 구조가 아님으로 많은 라인 Write 처리시 씹힐수 있다
             * 딜레이 적용으로 해소 가능하나 택이 늘어나고
             * LogState 내부 \n 삭제 코드를 구분 하여 문자열을 한번에 모아서 1번만 Write 처리를 하면 되지만
             * 가능한 디버깅 용도가 아닌 이상, 아래의 코드 적용 하지 말 것
            */

            //데이터 전부 로깅
            /*
            foreach (var list in chroma.m_mapFetchData)
            {
                foreach(var data in list.Value)
                {
                    string strDebug = string.Empty;
                    strDebug += ("key :"        + list.Key.ToString()           + ", ");
                    strDebug += ("collect :"    + list.Value.Count.ToString()   + ", ");
                    strDebug += ("realtime :"   + data.m_strProcTime            + ", ");
                    strDebug += ("measure :"    + data.m_strMeasure             + ", ");
                    strDebug += ("volt :"       + data.m_strOutVolt);
                    LogState(LogType.Info, strDebug);
                }
            }
            */

            //단위(10초 간격)의 마지막 데이터만 로깅
            /*
            foreach (var key in Enum.GetValues(typeof(EN_FETCH_KEY)))
            {
                CFecthData list = chroma.GetLastFecthData((EN_FETCH_KEY)key);

                if (list != null)
                {
                    string strDebug = string.Empty;
                    strDebug += ("key :"        + key.ToString()                + ", ");
                    strDebug += ("realtime:"    + list.m_strProcTime            + ", ");
                    strDebug += ("measure :"    + list.m_strMeasure             + ", ");
                    strDebug += ("volt :"       + list.m_strOutVolt);
                    LogState(LogType.Info, strDebug);
                }
            }
            */

            #endregion

            #region 패치 디테일 아이템 가공

            string keyVolt = string.Empty;
            string keyTime = string.Empty;
            string keyMeasure = string.Empty;

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                keyVolt = KEY_HIP_POR_NOR_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_POR_NOR_FETCH_IR_TIME;
                keyMeasure = KEY_HIP_POR_NOR_FETCH_IR_MEASURE;
            }

            if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                keyVolt = KEY_HIP_POR_F_L_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_POR_F_L_FETCH_IR_TIME;
                keyMeasure = KEY_HIP_POR_F_L_FETCH_IR_MEASURE;
            }
            //221101 wjs add mase m183
            if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                keyVolt = KEY_HIP_MAS_M183_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_MAS_M183_FETCH_IR____TIME;
                keyMeasure = KEY_HIP_MAS_M183_FETCH_IR_MEASURE;
            }

            if (localTypes == ModelType.MASERATI_NORMAL)
            {
                keyVolt = KEY_HIP_MAS_NOR_FETCH_IR_OUTVOLT;
                keyTime = KEY_HIP_MAS_NOR_FETCH_IR_TIME;
                keyMeasure = KEY_HIP_MAS_NOR_FETCH_IR_MEASURE;
            }

            if (keyVolt != null && keyTime != null && keyMeasure != null)
            {
                DetailItems dtVolt = new DetailItems() { Key = keyVolt };
                DetailItems dtTime = new DetailItems() { Key = keyTime };
                DetailItems dtMeasure = new DetailItems() { Key = keyMeasure };

                foreach (var list in chroma.m_mapFetchData)
                {
                    foreach (var data in list.Value)
                    {
                        dtVolt.Reportitems.Add(data.m_strOutVolt);
                        dtTime.Reportitems.Add(data.m_strProcTime);
                        dtMeasure.Reportitems.Add(data.m_strMeasure);
                    }
                }

                ti.refValues_.Add(dtVolt);
                ti.refValues_.Add(dtTime);
                ti.refValues_.Add(dtMeasure);
            }

            #endregion

            return JudgementTestItem(ti);
        }



        /// <summary>
        /// 내전압검사
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool WithstandVoltage1(TestItem ti)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            isProcessingUI(ti);

            //200616 내전압 리트라이 여부 false
            //200622 내전압 리트라이를 최대 2회로 변경하며 int type으로 변경
            withstandRetry = 0;

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            int Channel_HI = 0;
            int Channel_LO = 0;

            if (!SetChannels(out Channel_HI, out Channel_LO) || chroma.isAlive == false)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
             

            try
            {
                double current = -1;
                var rtv = chroma.GetVoltage(Channel_HI, Channel_LO, out current);
                SetValueByReturnValue_Withstand(rtv, current, ti, Channel_HI, Channel_LO);
            }
            catch (Exception ec)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
            sp.Stop();
            sp = null;

            if (ispause)
            {
                this.LogState(LogType.Info, "Pause Stopped");
                ti.Value_ = _EMG_STOPPED;
            }

            //200616 _HIPOT_EMO 추가
            if (isStop)
            {
                this.LogState(LogType.Info, "EMO Stopped");
                ti.Value_ = _HIPOT_EMO;
            }

            return JudgementTestItem(ti);
        }

        /// <summary> 
        /// 1:audi normal plus
        /// 2:audi mirror plus
        /// 3:audi normal minus(maybe)
        /// 4:audi mirror minus(maybe)
        /// 5:audi chassis
        /// 6:pors normal plus (mirror minus) / E_UP plus
        /// 7:pors normal minus (mirror plus) / E_UP minus
        /// 8:pors chassis                    / E_UP chassis
        /// </summary>
        /// <param name="Channel_HI"></param>
        /// <param name="Channel_LO"></param>
        /// <returns></returns>
        bool SetChannels(out int Channel_HI, out int Channel_LO)
        {
            int channelHIgh = -1;

            int channelLow = -1;
             
            try
            {
                string regData = string.Empty;

                string regTitle = string.Empty;

                switch (localTypes)
                {
                    case ModelType.AUDI_NORMAL:             {regTitle = "HIPOT_PORT_AUDI_NORMAL";}break;
                    case ModelType.AUDI_MIRROR:             {regTitle = "HIPOT_PORT_AUDI_MIRROR";}break;
                    case ModelType.PORSCHE_NORMAL:          {regTitle = "HIPOT_PORT_PORSCHE_NORMAL";}break;
                    case ModelType.PORSCHE_MIRROR:          {regTitle = "HIPOT_PORT_PORSCHE_MIRROR";}break;
                    case ModelType.MASERATI_NORMAL:         {regTitle = "HIPOT_PORT_MASERATI_NORMAL";}break;
                    case ModelType.PORSCHE_FACELIFT_NORMAL: {regTitle = "HIPOT_PORT_PORSCHE_FACELIFT_NORMAL";}break;
                    case ModelType.PORSCHE_FACELIFT_MIRROR: {regTitle = "HIPOT_PORT_PORSCHE_FACELIFT_MIRROR";}break;
                    case ModelType.MASERATI_M183_NORMAL:    {regTitle = "HIPOT_PORT_MASERATI_M183_NORMAL"; } break; //221101 wjs add mase m183
                    case ModelType.E_UP:                    {regTitle = "HIPOT_PORT_E_UP";}break;
                }

                regData = (Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger") //221101 wjs 체크필요
                                      .GetValue(regTitle) as string).ToString();

                if(regData != string.Empty)
                {
                    string[] arrChannel = regData.Split(',');

                    Int32.TryParse(arrChannel[0], out channelHIgh);

                    Int32.TryParse(arrChannel[1], out channelLow);

                    if(channelHIgh == 0 || channelLow == 0)
                    {
                        channelHIgh = -1;

                        channelLow = -1;

                        LogState(LogType.Fail, "Check Registry Data:" + regData.ToString());
                    }
                    else
                    {
                        LogState(LogType.Info, "ModelType:"      + localTypes.ToString() +
                                               " ,Channel High:" + channelHIgh.ToString() +
                                               " ,Channel Low:"  + channelLow.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                //reg 등록이 없어도 기존 사용의 영향 없게 예외 적용? 아님 -1 고정? 일단 기본값 적용

                switch (localTypes)
                {
                    case ModelType.AUDI_NORMAL:             { channelHIgh = 1; channelLow = 5;} break;
                    case ModelType.AUDI_MIRROR:             { channelHIgh = 2; channelLow = 5;} break;
                    case ModelType.PORSCHE_NORMAL:          { channelHIgh = 6; channelLow = 8;} break;
                    case ModelType.PORSCHE_MIRROR:          { channelHIgh = 7; channelLow = 8;} break;
                    case ModelType.MASERATI_NORMAL:         { channelHIgh = 6; channelLow = 8;} break;
                    case ModelType.PORSCHE_FACELIFT_NORMAL: { channelHIgh = 3; channelLow = 5;} break;
                    case ModelType.PORSCHE_FACELIFT_MIRROR: { channelHIgh = 4; channelLow = 5;} break;
                    case ModelType.MASERATI_M183_NORMAL:    { channelHIgh = 3; channelLow = 5;} break;  //221101 wjs add mase m183
                    case ModelType.E_UP:                    { channelHIgh = 6; channelLow = 8;} break;
                    default:                                { channelHIgh =-1; channelLow =-1;} break;
                }

                LogState(LogType.Fail, ex.Message);

                LogState(LogType.Fail, "Use Defualt"      +
                                       " ,Channel High:"  + channelHIgh.ToString() +
                                       " ,Channel Low:"   + channelLow.ToString());
            }

            Channel_HI = channelHIgh;

            Channel_LO = channelLow;

            if(Channel_HI == -1 || Channel_LO == -1)
            {
                LogState(LogType.Fail, "Fail, Setting Hipot Channel!");

                return false;
            }
            
            return true;

            /*
            switch (localTypes)
            {
                case ModelType.AUDI_NORMAL: { Channel_HI = 1; Channel_LO = 5; return true; };
                case ModelType.AUDI_MIRROR: { Channel_HI = 2; Channel_LO = 5; return true; };
                case ModelType.PORSCHE_NORMAL: { Channel_HI = 6; Channel_LO = 8; return true; };
                case ModelType.PORSCHE_MIRROR: { Channel_HI = 7; Channel_LO = 8; return true; };
                case ModelType.MASERATI_NORMAL: { Channel_HI = 6; Channel_LO = 8; return true; };
                //case ModelType.MASERATI_MIRROR: { Channel_HI = 7; Channel_LO = 8; return true; };
                //210312 wjs add pors fl
                case ModelType.PORSCHE_FACELIFT_NORMAL: { Channel_HI = 3; Channel_LO = 5; return true; };
                case ModelType.PORSCHE_FACELIFT_MIRROR: { Channel_HI = 4; Channel_LO = 5; return true; };
                case ModelType.E_UP: { Channel_HI = 6; Channel_LO = 8; return true; };
                default: { Channel_HI = 0; Channel_LO = 0; return false; };
            }
            */
        }

        // 1 Normal Plus, Mir Min
        // 2 Nor Min, Mir plus
        // 3 Chassis

        /// <summary>
        /// 리턴값을 기반으로 값을 넣는부분
        /// </summary>
        /// <param name="rtv"></param>
        /// <param name="current"></param>
        /// <param name="ti"></param>
        void SetValueByReturnValue_Insulation(int rtv, double res, TestItem ti)
        {
            if (rtv == 0)
            {
                //정상
                res = res * 0.000001;
                if (res > 999999)
                {
                    //200612 999999 > 60000 pjh/ht
                    //ti.Value_ = res = 60000;

                    //200616 overflow 적용
                    //ti.Value_ = _HIPOT_OVERFLOW;

                    //200622 pjh 기존대로 6만, 품질하고 이야기된게 아니라 일단 이 프로젝트들만 한정
                    ti.Value_ = res = 60000;
                }
                else
                {
                    ti.Value_ = res;
                }
            }
            else
            {
                //판정이상하면 결과로
                ti.Value_ = chroma.judgementStr;
            }
        }

        /// <summary>
        /// 리턴값을 기반으로 값을 넣는부분
        /// 리트라이도 함
        /// </summary>
        /// <param name="rtv"></param>
        /// <param name="current"></param>
        /// <param name="ti"></param>
        void SetValueByReturnValue_Withstand(int rtv, double current, TestItem ti, int ch_hi, int ch_lo)
        {
            if (rtv == -9 && chroma.judgementStr == _CASE_35)
            {
                //200622 audi porsche는 별도 분기로 두개 모델만 아크 리트라이
                //200622 내전압 Arc Retry 가 현장에서 6/10에 1->2회로 변경된 부분
                LogState(LogType.Info, "judgement : " + chroma.judgementStr.ToString());

                ti.Value_ = chroma.judgementStr;

                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR ||
                    localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                    || localTypes == ModelType.E_UP)
                {
                    withstandRetry++;
                    LogState(LogType.Info, string.Format("Retry Withstand - {0} / Count :{1}", _CASE_35, withstandRetry.ToString()));

                    rtv = chroma.GetVoltage(ch_hi, ch_lo, out current);

                    if (rtv == 0)
                    {
                        //정상
                        current = current * 1000;
                        ti.Value_ = current;
                    }
                    else if (rtv == -9 && chroma.judgementStr == _CASE_35)
                    {
                        LogState(LogType.Info, "judgement : " + chroma.judgementStr.ToString());

                        withstandRetry++;
                        LogState(LogType.Info, string.Format("Retry Withstand - {0} / Count :{1}", _CASE_35, withstandRetry.ToString()));

                        rtv = chroma.GetVoltage(ch_hi, ch_lo, out current);
                        if (rtv == 0)
                        {
                            //정상
                            current = current * 1000;
                            ti.Value_ = current;
                        }
                        else
                        {
                            //뭔지모르면 기존대로 -1
                            ti.Value_ = chroma.judgementStr;
                        }
                    }
                    else
                    {
                        //뭔지모르면 기존대로 -1
                        ti.Value_ = chroma.judgementStr;
                    }
                }

            }
            else if (rtv == 0)
            {
                //정상
                current = current * 1000;
                ti.Value_ = current;
            }
            else
            {
                //뭔지모르면 기존대로 -1
                ti.Value_ = chroma.judgementStr;
            }
        }
    }
}
