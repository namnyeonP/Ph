using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        private int mTotalRunCountDCIR = 0; //1base
        private int mLastStepRunCountDCIR = 0; //1bae

        double tempdcircell1 = 0.0;
        double tempdcircell2 = 0.0;
        double tempdcircell3 = 0.0;
        double tempdcircell4 = 0.0;
        double tempdcircell5 = 0.0;
        double tempdcircell6 = 0.0;
        double tempdcircell7 = 0.0;
        double tempdcircell8 = 0.0;
        double tempdcircell9 = 0.0;
        double tempdcircell10 = 0.0;

        bool isCyclerUse = false;

        // 충방전 내내 500ms마다 계측한 온도값을 저장하는 List
        List<string> tempListInDCIR = new List<string>();
        bool isTempCheck = false;

        #region DCIR 관련 메서드


        Thread Protection_ID1 = null;
        Thread Protection_CHARGE_ID1 = null;
        Thread Protection_DISCHARGE_ID1 = null;

        //221005 저항 측정용 nnkim
        Thread Protection_McResistanceStart = null;
        Thread dp_McShortCheck = null;
        //221017 소모품 카운트용
        Thread Protection_PartsCountStart = null;
        public List<string> rtMcList = new List<string>();

        /// <summary>
        /// 분기 구분
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        private bool ModeSet(TestItem ti)
        {
            var str = "";
            switch (localTypes)
            {
                //0.4P8S +, -
                //1.4P8S(Reverse) -, +
                //2.4P7S 아래가 +, 위가 -
                //3.3P8S +, -
                //4.3P10S +, -
                //5.3P10S(Reverse) -, +
                //AA : 
                //BB : 
                //CC : 아래 +, 위 - 
                case 0:
                case 3:
                case 4:
                    str = "0AA"; //4P8S, 3P10S, 3P8S
                    break;
                case 1:
                case 5:
                    str = "0BB"; //4P8S REV, 3P10S REV
                    break;
                case 2:
                    str = "0CC"; //4P7S
                    break;
            }
            //str = "0AA";

            for (int i = 0; i < 3; i++)
            {
			    cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            	LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            	Thread.Sleep(500);
			
                if (cycler.cycler1voltage < 10)
                {
                    cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                    // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가                  
                }
                else
                {
                    break;
                }
            }

            if (cycler.cycler1voltage < 10)
            {
                cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                relays.RelayOff("IDO_4");
                ti.Value_ = _VOLTAGE_PIN_SENSING_FAIL;
                return JudgementTestItem(ti);
            }

            return true;
        }

        private bool ModeSet_Release(TestItem ti, bool bResToCheck = false)
        {
            // 2019.03.20 jeonhj's comment
            // 박진호 선임 요청사항
            // FF를 먼저 날리고 저항을 계측하도록
            // 어차피 ModeSet 전이기 때문에 순서는 바뀌어도 상관없을 것 같음.
            var str = "0FF";

            cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            Thread.Sleep(500);
            if (cycler.cycler1voltage > 10)
            {
                ti.Value_ = _SENSING_BOARD_CHECK;
                // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
                relays.RelayOff("IDO_4");
                cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                return JudgementTestItem(ti);
            }

            if (bResToCheck)
            {
                if (SensingResToMux())
                {
                    ti.Value_ = _CONT_CLOSE_STUCK;
                    // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
                    relays.RelayOff("IDO_4");
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("Please Check Contactor status", _CONT_CLOSE_STUCK, MessageBoxButton.OK, MessageBoxImage.Information);

                    }));
                    return JudgementTestItem(ti);
                }
            }
            
            return true;
        }
        CMD localitem_ID1;
        
        DetailItems 상세저항1;// = new DetailItems();
        DetailItems 상세저항2;// = new DetailItems();

        /// <summary>
        /// schedule core
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool Do_Rest_Charge(TestItem ti)
        {
            sendList.Clear();

            CCycler.AddToSendListBytes(sendList, MakeSendCMD_INPUT_MC_ON());
            CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_ON());             

            foreach (var step in this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList)
            {
                CCycler.AddToSendListBytes(sendList, MakeOneStepToSendCMD(step));
            } 

            CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_OFF());

            int localpos = 0;
            bool changeOutToRest = true;

            localitem_ID1 = CMD.NULL;
            Protection_ID1 = null;
            Protection_CHARGE_ID1 = null;
            Protection_DISCHARGE_ID1 = null;
            //221006 저항 측정 한번만 할 수 있게 변수 추가 nnkim
            int nMcStartCheck = 0;
            isTempCheck = false;

            double catchLastPoint = mTotalRunCountDCIR - mLastStepRunCountDCIR;

            int totalMilisecond = 0;

            #region 상세저항 타이머 시작부
            상세저항1 = new DetailItems();
            상세저항2 = new DetailItems();

            //timeBeginPeriod(1);
            //상시저항가져오는타이머_핸들러 = new 공용_이벤트핸들러(상시저항가져오는타이머_Tick);
            //상시저항가져오는타이머_아이디 = timeSetEvent(1000, 0, 상시저항가져오는타이머_핸들러, IntPtr.Zero, 1);
            #endregion

            lock (send)
            {
                #region 3차 프로텍션 - 모든과정 +20초까지만 대기, 이후 팝업, STOP

                Protection_ID1 = new Thread(() =>
                {
                    var totalTime = 0.0;
                    foreach (var sitem in sendList)
                    {
                        totalTime += sitem.Duration;
                    }

                    double countLimit = (totalTime * 1000) + 20000; // (??sec * 1000) + 10000 == @ + 20000msec
                    double count = 0;

                    while (!cycler.is84On)//충방전이 진행중에만 루프
                    {
                        count += 1000;
                        Thread.Sleep(1000);
                        if (count > countLimit)
                        {
                            isStop = true;
                            var msgstr = "3rd Protection occured (total time + 20sec)";
                            LogState(LogType.Info, msgstr);
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show("SAFETY_STOPPED", msgstr, MessageBoxButton.OK, MessageBoxImage.Information);
                            }));
                            break;
                        }
                    }
                    //Protection_ID1 = null;
                });
                Protection_ID1.Start();

                #endregion

                foreach (var sitem in sendList)
                {
                    if (isStop || ispause)
                    {
                        DCIR_Stopped();

                        ti.Value_ = _CHECK_TERMINAL;

                        return JudgementTestItem(ti);
                    }

                    int milisecond = 0;

                    #region 2차 프로텍션 - 충/방전일 때 실제 동작시간 +5초까지만 대기, 이후 팝업, STOP

                    localitem_ID1 = sitem.Cmd;

                    if (sitem.Cmd == CMD.CHARGE && Protection_CHARGE_ID1 == null)
                    {
                        Protection_CHARGE_ID1 = new Thread(() =>
                        {
                            double countLimit = (sitem.Duration * 1000) + 5000; // (10sec * 1000) + 5000 == 15000msec
                            double count = 0;


                            while (localitem_ID1 == CMD.CHARGE)
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    var msgstr = "2nd Protection occured (charge time + 5sec)";
                                    LogState(LogType.Info, msgstr);
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        MessageBox.Show("SAFETY_STOPPED", msgstr, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }));
                                    break;
                                }
                            }
                            //Protection_CHARGE_ID1 = null;
                        }
                        );
                        Protection_CHARGE_ID1.Start();
                    }

                    if (sitem.Cmd == CMD.DISCHARGE && Protection_DISCHARGE_ID1 == null)
                    {
                        Protection_DISCHARGE_ID1 = new Thread(() =>
                        {
                            double countLimit = (sitem.Duration * 1000) + 5000; // (10sec * 1000) + 5000 == 15000msec
                            double count = 0;


                            while (localitem_ID1 == CMD.DISCHARGE)
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    var msgstr = "2nd Protection occured (discharge time + 5sec)";
                                    LogState(LogType.Info, msgstr);
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        MessageBox.Show("SAFETY_STOPPED", msgstr, MessageBoxButton.OK, MessageBoxImage.Information);
                                    }));
                                    break;
                                }
                                // 방전 시 전류가 셋팅 된 전류보다 낮게 흐르면 전촉 문제로 판단해서 알람 발생. 
                                // 접촉이 잘 않된 상태에서 충전으로 넘어갔을 경우 번트 발생 방지 인터락.
                                if (count == 9000)
                                {
                                    if (Math.Abs(cycler.cycler1current) < Math.Abs((sitem.Current - (sitem.Current * 0.01))))
                                    {
                                        isStop = true;
                                        var msgstr = "Check the terminal and currnet probe surface";
                                        LogState(LogType.Info, msgstr);
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            MessageBox.Show("CHECK_TERMINAL", msgstr, MessageBoxButton.OK, MessageBoxImage.Information);
                                        }));
                                        break;
                                    }
                                }
                            }
                            //Protection_DISCHARGE_ID1 = null;
                        }
                        );
                        Protection_DISCHARGE_ID1.Start();
                    }

                    #endregion
                    for (int i = 0; i < sitem.Duration * 10; i++)   // *10 이유는 정수로 만들고 (최소 단위 0.1) 100ms를 1로 카운팅 하는걸로 추측
                    {
                        #region Sending Sequence (190312)
                        if (isStop || ispause)
                        {
                            DCIR_Stopped();

                            ti.Value_ = _CHECK_TERMINAL;

                            return JudgementTestItem(ti);
                        }
                        
                        #region 기본 프로텍션 - 전압상하한
                        if (!CheckSafety(0, cycler.cycler1OP, sitem))
                        {
                            DCIR_Stopped();
                            ti.Value_ = _CYCLER_SAFETY;
                            var msgstr = "Default Protection occured (safety voltage/current)";
                            LogState(LogType.Info, msgstr);
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show("SAFETY_STOPPED", msgstr, MessageBoxButton.OK, MessageBoxImage.Information);
                            })); 
                            return JudgementTestItem(ti);
                        }
                        #endregion
                        
                        cycler.cycler1Header = CMD.NULL;
                        cycler.cycler1OP = RECV_OPMode.NULL;
                        
                        if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                        {
                            LogState(LogType.Fail, "cycler #1_Send fail");

                            DCIR_Stopped();
                            ti.Value_ = _COMM_FAIL;
                            return JudgementTestItem(ti);
                        }

                        #region 1차 프로텍션 - 타임아웃 - 설정

                        // 타임아웃을 위한 설정부분
                        var oldTime = DateTime.Now;
                        var ltime = oldTime;
                        DateTime common_Timeout = oldTime.AddSeconds(5);

                        if ((sitem.Cmd == CMD.OUTPUT_MC_ON) || (sitem.Cmd == CMD.INPUT_MC_ON) || (sitem.Cmd == CMD.OUTPUT_MC_OFF))
                        {   // MC 동작은 최대10초까지 걸리니, 해당 동작은 15초로 한다.
                            common_Timeout = oldTime.AddSeconds(15);
                        }

                        #endregion

                        // 해당 슬립이 전체 로직을 100ms로 동작하도록 유지한다.
                        Thread.Sleep(100);
                        
                        while (true)
                        {
                            if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
                            {
                                var sb = new StringBuilder();
                                sb.Append("Cycler(Normal),");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());
                                sb.Append(",");

                                // cell voltage measure point 1,2,3,4,6,7,8,9
                                if (localTypes == 0)
                                {
                                    //4P8S
                                    sb.Append(daq.DAQList[10] + ",");
                                    sb.Append(daq.DAQList[11] + ",");
                                    sb.Append(daq.DAQList[12] + ",");
                                    sb.Append(daq.DAQList[13] + ",");
                                    sb.Append(daq.DAQList[14] + ",");
                                    sb.Append(daq.DAQList[15] + ",");
                                    sb.Append(daq.DAQList[16] + ",");
                                    sb.Append(daq.DAQList[17]);
                                }
                                else if (localTypes == 1)
                                {
                                    //4P8S Rev
                                    sb.Append(daq.DAQList[28] + ",");
                                    sb.Append(daq.DAQList[29] + ",");
                                    sb.Append(daq.DAQList[30] + ",");
                                    sb.Append(daq.DAQList[31] + ",");
                                    sb.Append(daq.DAQList[32] + ",");
                                    sb.Append(daq.DAQList[33] + ",");
                                    sb.Append(daq.DAQList[34] + ",");
                                    sb.Append(daq.DAQList[35]);
                                }
                                else if (localTypes == 2)
                                {
                                    //4P7S
                                    sb.Append(daq.DAQList[36] + ",");
                                    sb.Append(daq.DAQList[37] + ",");
                                    sb.Append(daq.DAQList[38] + ",");
                                    sb.Append(daq.DAQList[39] + ",");
                                    sb.Append(daq.DAQList[40] + ",");
                                    sb.Append(daq.DAQList[41] + ",");
                                    sb.Append(daq.DAQList[42]);
                                }
                                else if (localTypes == 3)
                                {
                                    //3P8S
                                    sb.Append(daq.DAQList[10] + ",");
                                    sb.Append(daq.DAQList[11] + ",");
                                    sb.Append(daq.DAQList[12] + ",");
                                    sb.Append(daq.DAQList[13] + ",");
                                    sb.Append(daq.DAQList[14] + ",");
                                    sb.Append(daq.DAQList[15] + ",");
                                    sb.Append(daq.DAQList[16] + ",");
                                    sb.Append(daq.DAQList[17]);
                                }
                                else if (localTypes == 4)
                                {
                                    //3P10S
                                    sb.Append(daq.DAQList[0] + ",");
                                    sb.Append(daq.DAQList[1] + ",");
                                    sb.Append(daq.DAQList[2] + ",");
                                    sb.Append(daq.DAQList[3] + ",");
                                    sb.Append(daq.DAQList[4] + ",");
                                    sb.Append(daq.DAQList[5] + ",");
                                    sb.Append(daq.DAQList[6] + ",");
                                    sb.Append(daq.DAQList[7] + ",");
                                    sb.Append(daq.DAQList[8] + ",");
                                    sb.Append(daq.DAQList[9]);
                                }
                                else if (localTypes == 5)
                                {
                                    //3P10S Rev
                                    sb.Append(daq.DAQList[18] + ",");
                                    sb.Append(daq.DAQList[19] + ",");
                                    sb.Append(daq.DAQList[20] + ",");
                                    sb.Append(daq.DAQList[21] + ",");
                                    sb.Append(daq.DAQList[22] + ",");
                                    sb.Append(daq.DAQList[23] + ",");
                                    sb.Append(daq.DAQList[24] + ",");
                                    sb.Append(daq.DAQList[25] + ",");
                                    sb.Append(daq.DAQList[26] + ",");
                                    sb.Append(daq.DAQList[27]);
                                }
                                LogState(LogType.Info, sb.ToString(), null, false);

                                //221006 MC 저항 측정하는 부분 
                                if (nMcStartCheck == 0 && cycler.cycler1Header.ToString() == "REST")
                                {
                                    Protection_McResistanceStart = new Thread(() =>
                                    {
                                        McResistanceMeasure(McResistanceposition, out rtMcList);
                                    }
                                    );
                                    Protection_McResistanceStart.Start();
                                    nMcStartCheck = 1;
                                }

                                break;
                            }
                            else
                            {
                                var sb = new StringBuilder();
                                sb.Append("Cycler(Loop),");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());
                                sb.Append(",");

                                // cell voltage measure point 1,2,3,4,6,7,8,9
                                if (localTypes == 0)
                                {
                                    //4P8S
                                    sb.Append(daq.DAQList[10] + ",");
                                    sb.Append(daq.DAQList[11] + ",");
                                    sb.Append(daq.DAQList[12] + ",");
                                    sb.Append(daq.DAQList[13] + ",");
                                    sb.Append(daq.DAQList[14] + ",");
                                    sb.Append(daq.DAQList[15] + ",");
                                    sb.Append(daq.DAQList[16] + ",");
                                    sb.Append(daq.DAQList[17]);
                                }
                                else if (localTypes == 1)
                                {
                                    //4P8S Rev
                                    sb.Append(daq.DAQList[28] + ",");
                                    sb.Append(daq.DAQList[29] + ",");
                                    sb.Append(daq.DAQList[30] + ",");
                                    sb.Append(daq.DAQList[31] + ",");
                                    sb.Append(daq.DAQList[32] + ",");
                                    sb.Append(daq.DAQList[33] + ",");
                                    sb.Append(daq.DAQList[34] + ",");
                                    sb.Append(daq.DAQList[35]);
                                }
                                else if (localTypes == 2)
                                {
                                    //4P7S
                                    sb.Append(daq.DAQList[36] + ",");
                                    sb.Append(daq.DAQList[37] + ",");
                                    sb.Append(daq.DAQList[38] + ",");
                                    sb.Append(daq.DAQList[39] + ",");
                                    sb.Append(daq.DAQList[40] + ",");
                                    sb.Append(daq.DAQList[41] + ",");
                                    sb.Append(daq.DAQList[42]);
                                }
                                else if (localTypes == 3)
                                {
                                    //3P8S
                                    sb.Append(daq.DAQList[10] + ",");
                                    sb.Append(daq.DAQList[11] + ",");
                                    sb.Append(daq.DAQList[12] + ",");
                                    sb.Append(daq.DAQList[13] + ",");
                                    sb.Append(daq.DAQList[14] + ",");
                                    sb.Append(daq.DAQList[15] + ",");
                                    sb.Append(daq.DAQList[16] + ",");
                                    sb.Append(daq.DAQList[17]);
                                }
                                else if (localTypes == 4)
                                {
                                    //3P10S
                                    sb.Append(daq.DAQList[0] + ",");
                                    sb.Append(daq.DAQList[1] + ",");
                                    sb.Append(daq.DAQList[2] + ",");
                                    sb.Append(daq.DAQList[3] + ",");
                                    sb.Append(daq.DAQList[4] + ",");
                                    sb.Append(daq.DAQList[5] + ",");
                                    sb.Append(daq.DAQList[6] + ",");
                                    sb.Append(daq.DAQList[7] + ",");
                                    sb.Append(daq.DAQList[8] + ",");
                                    sb.Append(daq.DAQList[9]);
                                }
                                else if (localTypes == 5)
                                {
                                    //3P10S Rev
                                    sb.Append(daq.DAQList[18] + ",");
                                    sb.Append(daq.DAQList[19] + ",");
                                    sb.Append(daq.DAQList[20] + ",");
                                    sb.Append(daq.DAQList[21] + ",");
                                    sb.Append(daq.DAQList[22] + ",");
                                    sb.Append(daq.DAQList[23] + ",");
                                    sb.Append(daq.DAQList[24] + ",");
                                    sb.Append(daq.DAQList[25] + ",");
                                    sb.Append(daq.DAQList[26] + ",");
                                    sb.Append(daq.DAQList[27]);
                                }
                                LogState(LogType.Info, sb.ToString(), null, false);

                            }

                            if (isStop || ispause)
                            {
                                DCIR_Stopped();

                                ti.Value_ = _CHECK_TERMINAL;

                                return JudgementTestItem(ti);
                            }

                            ltime = DateTime.Now;

                            #region send fail
                            if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                            {
                                LogState(LogType.Fail, "cycler #1_Send fail");

                                DCIR_Stopped();
                                ti.Value_ = "DSP1_SEND_FAIL";
                                return JudgementTestItem(ti);
                            }

                            #endregion
                            Thread.Sleep(100);
                            #region 1차 프로텍션 - 타임아웃 - 트리거

                            //통신간 커맨드가 5 / 15초동안 바뀌지 않으면 강제 스탑 및 메시지
                            if (ltime.Ticks >= common_Timeout.Ticks)
                            {
                                var msgstr = "1st Protection occured (command mismatch 5sec/15sec)";
                                LogState(LogType.Info, msgstr);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    MessageBox.Show("SAFETY_STOPPED", msgstr, MessageBoxButton.OK, MessageBoxImage.Information);
                                })); 

                                DCIR_Stopped();
                                ti.Value_ = _COMM_FAIL;
                                return JudgementTestItem(ti);
                            }

                            #endregion 
                        }
                        #endregion
                        milisecond += 100;

                        totalMilisecond += 100;

                        // 2019.05.16 jeonhj
                        // 여기서 시작하면 REST부터 계측 가능할 듯 한데
                        if (sitem.Cmd == CMD.REST && changeOutToRest == true )
                        {
                            // 타이머 시작
                            timeBeginPeriod(1);
                            상시저항가져오는타이머_핸들러 = new 공용_이벤트핸들러(상시저항가져오는타이머_Tick);
                            상시저항가져오는타이머_아이디 = timeSetEvent(1000, 0, 상시저항가져오는타이머_핸들러, IntPtr.Zero, 1);
                            changeOutToRest = false;
                        }

                        if (sitem.Cmd == CMD.OUTPUT_MC_ON)
                        {
                        }

                        //초기 휴지 9.5                                                         //이래 써놓으니 햇갈리는데 걍 1번스탭 레스트의 1.5sec 시점(1500ms)
                        if (sitem.Cmd == CMD.REST
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 0)  //Duration 0.1 : 100ms 으로 계산 되는듯 하다
                        {
                            dp_rest_bef_disc = new Thread(new ParameterizedThreadStart(Point_REST_BEF_DISC));
                            dp_rest_bef_disc.Start(ti);
                            localpos = 1;
                        }

                        //방전 9.5
                        if (sitem.Cmd == CMD.DISCHARGE
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 1)
                        {
                            dp_disc = new Thread(new ParameterizedThreadStart(Point_DISC));
                            dp_disc.Start(ti);
                            localpos = 2;
                        }

                        //중간 휴지 9.5
                        if (sitem.Cmd == CMD.REST
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 2)
                        {
                            dp_rest_aft_disc = new Thread(new ParameterizedThreadStart(Point_REST_AFT_DISC));
                            dp_rest_aft_disc.Start(ti);
                            localpos = 3;
                        }

                        ////충전 9.5
                        if (sitem.Cmd == CMD.CHARGE
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 3)
                        {
                            dp_char = new Thread(new ParameterizedThreadStart(Point_CHAR));
                            dp_char.Start(ti);
                            localpos = 5;
                        }

                        ////마지막 Rest 4.5초
                        //if (sitem.Cmd == CMD.REST
                        //    && ((sitem.Duration - 35.5) * 1000) == milisecond && localpos == 4)
                        //{
                        //    dp_rest_aft_char_5s = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR_5s));
                        //    dp_rest_aft_char_5s.Start(ti);
                        //    localpos = 5;
                        //}

                        //마지막 Rest 9.5초
                        if (sitem.Cmd == CMD.REST
                            //&& ((sitem.Duration - 30.5) * 1000) == milisecond && localpos == 5)
                            && ((catchLastPoint + 9.5) * 1000) == totalMilisecond && localpos == 5)
                        {
                            LogState(LogType.Info, "Catch 5Step(Rest) 9.5s");
                            dp_rest_aft_char_10s = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR_10s));
                            dp_rest_aft_char_10s.Start(ti);
                            localpos = 6;
                        }
                        //마지막 Rest 19.5초
                        if (sitem.Cmd == CMD.REST
                            //&& ((sitem.Duration - 20.5) * 1000) == milisecond && localpos == 6)
                            && ((catchLastPoint + 19.5) * 1000) == totalMilisecond && localpos == 6)
                        {
                            LogState(LogType.Info, "Catch 5Step(Rest) 19.5s");
                            dp_rest_aft_char_20s = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR_20s));
                            dp_rest_aft_char_20s.Start(ti);
                            localpos = 7;
                        }
                        //끝나기전 마지막 Rest 29.5초                        
                        if (sitem.Cmd == CMD.REST
                            //&& ((sitem.Duration - 10.5) * 1000) == milisecond && localpos == 7)
                            && ((catchLastPoint + 29.5) * 1000) == totalMilisecond && localpos == 7)
                        {
                            LogState(LogType.Info, "Catch 5Step(Rest) 29.5s");
                            dp_rest_aft_char_30s = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR_30s));
                            dp_rest_aft_char_30s.Start(ti);
                            localpos = 8;
                        }
                        //끝나기전 마지막 Rest 39.5초                        
                        if (sitem.Cmd == CMD.REST
                            //&& ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 8)
                            && ((catchLastPoint + 39.5) * 1000) == totalMilisecond && localpos == 8)
                        {
                            LogState(LogType.Info, "Catch 5Step(Rest) 39.5s");
                            dp_rest_aft_char_40s = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR_40s));
                            dp_rest_aft_char_40s.Start(ti);
                            localpos = 9;
                        }

                        if (nMcStartCheck == 1)
                        {
                            //rest 221006 저항 측정한거 상세 수집
                            dp_McShortCheck = new Thread(new ParameterizedThreadStart(Point_Mc_Short_check));
                            dp_McShortCheck.Start(ti);
                            nMcStartCheck = 2;
                        }

                    }
                }
            }

            PutEmptyDetailItemForLastRest(ti); //마지막 5Step(Rest) 시간 변경의 따른 기능 추가
            
            DCIR_Stopped();

            return true;
        }

        private void PutEmptyDetailItemForLastRest(TestItem testItem)
        {
            // 마지막 레스트(5Step) 동작 시간이 기존의 고정 사용에서 변경의 유연하게 대처 가능하도록 수정 요청 됨
            // 충방전 핸들링 및 데이터 캡쳐 타이밍이 전부 하드코딩 되어있다 (Do_Rest_Charge 함수 참고)
            // 전부다 뜯어 고치기엔 리스크가 있고 검토 결과 시간이 단축되면 데이터 캡쳐 자체를 못함 (Point_REST_AFT_CHAR_10s~40s 참고)
            // 때문에 관련된 상세ID가 본 함수 호출 시점 까지 없다면 고객 요청의 "N.A. : rest time is modified" 으로 보고하기 위한 기능만 담당함
            // 당장은 이게 최선으로 보임
            // 참고 : 충방전 핸들링은 총 5Step이 존재하는데(Rest, Discharge, Rest, Charge, Rest) 4Step까지는 기존대로 운영함, 변경 없음
            // 주의 : 본 함수는 DCIR이 온전히 끝나는 시점에만 사용 할 것

            try
            {
                LogState(LogType.Info, "Start " + MethodBase.GetCurrentMethod().Name);

                const string reportMessage = "N.A. : rest time is modified";

                List<string> findDetailItemID = new List<string>();

                findDetailItemID.Add("W2MLMTE4007"); //마지막 레스트(5Step) 10sec 포인트 전압 (셀)
                findDetailItemID.Add("W2MLMTE4008"); //마지막 레스트(5Step) 20sec 포인트 전압 (셀)
                findDetailItemID.Add("W2MLMTE4009"); //마지막 레스트(5Step) 30sec 포인트 전압 (셀)
                findDetailItemID.Add("W2MLMTE4010"); //마지막 레스트(5Step) 40sec 포인트 전압 (셀)
                findDetailItemID.Add("W2MLMTE4018"); //마지막 레스트(5Step) 10sec 포인트 전압 (모듈)
                findDetailItemID.Add("W2MLMTE4019"); //마지막 레스트(5Step) 20sec 포인트 전압 (모듈)
                findDetailItemID.Add("W2MLMTE4020"); //마지막 레스트(5Step) 30sec 포인트 전압 (모듈)
                findDetailItemID.Add("W2MLMTE4021"); //마지막 레스트(5Step) 40sec 포인트 전압 (모듈)
                //필요시 추가

                foreach (var id in findDetailItemID)
                {
                    bool bIsFind = false;

                    foreach (var refValue in testItem.refValues_)
                    {
                        DetailItems detailItem = refValue as DetailItems;

                        if(detailItem == null)
                        {
                            continue;
                        }

                        if (detailItem.Key == id)
                        {
                            bIsFind = true;

                            break;
                        }
                    }

                    if (bIsFind == false)
                    {
                        LogState(LogType.Info, "detail ID : " + id +
                                               ", empty data! put report message : " + reportMessage);

                        testItem.refValues_.Add(MakeDetailItem(id, reportMessage));
                    }
                }
            }
            catch(Exception ec)
            {
                LogState(LogType.Fail, MethodBase.GetCurrentMethod().Name + 
                                       ", Exception : " + 
                                       ec.Message);
            }

            LogState(LogType.Info, "End " + MethodBase.GetCurrentMethod().Name);
        }

        //private void 상시저항가져오는타이머_Tick(object obj)
        private void 상시저항가져오는타이머_Tick(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (isTempCheck == false)
            {
                Thread.Sleep(3000);
                isTempCheck = true;
            }

            if (!cycler.is84On)
            {
                double tempp = 0.0;
                double outTemp1 = 0.0;
                double outTemp2 = 0.0;
                var dtiStr = getModuleTemp1N2(out tempp, out outTemp1, out outTemp2);

                // 타이밍 문제로 기대 수집보다 1~2 카운트 더 측정될 수 있다
                // 마지막 레스트 시간 변경으로 인해  < 총 토탈 시간 일때만 수집해야 정확한 수집 갯수를 가질수있음 이래야만 대응이 가능

                if (상세저항1.Reportitems.Count < mTotalRunCountDCIR) 
                {
                    상세저항1.Reportitems.Add(outTemp1.ToString("F3"));
                    상세저항2.Reportitems.Add(outTemp2.ToString("F3"));
                }
            }
        }

        private string TempAcqusitionInDCIR()
        {
            if (isLineFlag) // new Line
            {
                List<double> resList = new List<double>();
                if (daq970_1 == null)
                {
                    LogState(LogType.Info, "Disconnected DMM.");
                    return "";
                }

                if (!daq970_1.MeasRes_Multi(out resList, "318,319", 2))
                {
                    LogState(LogType.Info, "Disconnected DMM.");
                    return "";
                }
                else
                {
                    finalTempList = new List<double>();
                    foreach (var item in resList)
                    {
                        finalTempList.Add(item * 0.001);
                    }
                }
            }
            else
            {
                if (keysight == null)
                {
                    LogState(LogType.Info, "Disconnected DMM.");
                    return "";
                }

                keysight.rtstring = "";
                finalTempCnt = 2;
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
                                LogState(LogType.Info, "Disconnected DMM.");
                                return "";
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

                finalTempList = new List<double>();
                foreach (var item in vArr)
                {
                    double dv = 0;
                    if (double.TryParse(item, out dv))
                    {
                        finalTempList.Add(dv * 0.001);
                    }
                }
            }

            var tempTher1 = 0.0;
            if (finalTempList != null && finalTempList.Count == finalTempCnt)
            {
                double resultTemp1 = 0.0;
                double resultTemp2 = 0.0;
                Calculate(finalTempList[0], out resultTemp1);
                Calculate(finalTempList[1], out resultTemp2);

                tempTher1 = resultTemp1 - resultTemp2;
                LogState(LogType.Info, string.Format("{0} - {1} = {2}", resultTemp1, resultTemp2, tempTher1));
            }

            string ret_value = "";

            return ret_value;
        }

        private SendCMD MakeOneStepToSendCMD(MiniScheduler.Step step)
        {
            string time = "0.000";
            try
            {
                time = (step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Time") as MiniScheduler.ClearCase).Value;
            }
            catch (Exception)
            {
            }

            double total = 0.0;
            if (time == "0.000" || time == "00:00:00:00")
            {
                total = 0.1;
            }
            else
            {
                var tarr = time.Split(':');//시분초
                total = (int.Parse(tarr[0]) * 86400) + (int.Parse(tarr[1]) * 3600) + (int.Parse(tarr[2]) * 60) + (int.Parse(tarr[3]));
            }

            CMD cmd = CMD.NULL;
            OPMode opm = OPMode.READY;
            RECV_OPMode rop = RECV_OPMode.READY;
            double voltHighLimit = 0.0;
            double voltLowLimit = 0.0;

            double currHighLimit = 0.0;

            switch (step.Step_mode)
            {
                case "CC": opm = OPMode.CHARGE_CC; break;
            }

            switch (step.Step_type)
            {
                case "Rest":
                    {
                        cmd = CMD.REST;
                        rop = RECV_OPMode.READY_TO_CHARGE;
                    }
                    break;
                case "Charge":
                    {
                        cmd = CMD.CHARGE;
                        if (step.SafecaseData.VoMax != "0.000" || step.SafecaseData.VoMax != "0")
                        {
                            double.TryParse(step.SafecaseData.VoMax, out voltHighLimit);
                        }

                        double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

                        if (step.Step_mode == "CC")
                        {
                            opm = OPMode.CHARGE_CC;
                            rop = RECV_OPMode.CHARGE_CC;
                        }
                        else
                        {
                            opm = OPMode.CHARGE_CV;
                            rop = RECV_OPMode.CHARGE_CV;
                        }
                    }
                    break;
                case "Discharge":
                    {
                        cmd = CMD.DISCHARGE;
                        if (step.SafecaseData.VoMin != "0.000" || step.SafecaseData.VoMin != "0")
                        {
                            double.TryParse(step.SafecaseData.VoMin, out voltLowLimit);
                        }

                        double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

                        if (step.Step_mode == "CC")
                        {
                            opm = OPMode.DISCHARGE_CC;
                            rop = RECV_OPMode.DISCHARGE_CC;
                        }
                        else
                        {
                            opm = OPMode.DISCHARGE_CV;
                            rop = RECV_OPMode.DISCHARGE_CV;
                        }
                    }
                    break;
            }
            var voltage = 0.0;
            if (cmd == CMD.DISCHARGE)
            {
                double.TryParse(step.Discharge_voltage, out voltage);
            }
            else
            {
                double.TryParse(step.Voltage, out voltage);
            }

            var current = 0.0;
            double.TryParse(step.Current, out current);

            return new SendCMD()
            {
                Duration = total,
                Index = 0,
                Cmd = cmd,
                Pmode = BranchMode.CHANNEL1,
                Opmode = opm,
                Voltage = voltage,
                Current = current,
                NextOPMode = rop,
                VoltLowLimit = voltLowLimit,
                VoltHighLimit = voltHighLimit,
                CurrHighLimit = currHighLimit,
            };

        }

        private SendCMD MakeSendCMD_INPUT_MC_ON()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.INPUT_MC_ON,
                Pmode = BranchMode.CHANNEL1,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            };
        }

        private SendCMD MakeSendCMD_OUTPUT_MC_ON()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_ON,
                Pmode = BranchMode.CHANNEL1,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            };
        }

        private SendCMD MakeSendCMD_REST()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.REST,
                Pmode = BranchMode.CHANNEL1,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            };
        }

        private SendCMD MakeSendCMD_OUTPUT_MC_OFF()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_OFF,
                Pmode = BranchMode.CHANNEL1,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            };
        }
               
        #endregion
        public bool Module_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);

            InitDCIRVariables();

            #region DAQ Check
            deviceStatus = "";
            List<string> ngDeviceList = new List<string>();
            string checkDeviceName;
            int nDaqCnt = 0;
            checkDeviceName = "DAQ";

            if (daq != null && daq.sp.IsOpen)
            {
                var nowDaqDt = daq.localDt;
                //221101 DAQ 10s
                nowDaqDt = nowDaqDt.AddSeconds(10);
                //nowDaqDt = nowDaqDt.AddSeconds(5);

				//220926
                //20180808 wjs add daq retry connection
                if (DateTime.Now > nowDaqDt)
                {
                //연결이 되었는 지 재 확인 220926
                DaqReConnect:
                    //LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    LogState(LogType.TEST, checkDeviceName + " Myunghwan Dispose resources"
                                            + DateTime.Now + "/" + nowDaqDt);

                    daq.Close();

                    Thread.Sleep(100);
                    daq = null;

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                    }));
                    Thread.Sleep(1000);

                    nowDaqDt = daq.localDt;
                    //221101 DAQ 10s
                    nowDaqDt = nowDaqDt.AddSeconds(10);
                    //nowDaqDt = nowDaqDt.AddSeconds(5);
                    
                    //220927
                    //연결이 되었는 지 재 확인 
                    if (daq.isAlive)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                        }));
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                    else
                    {
                        if (nDaqCnt >= 5)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                            }));

                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                        else
                        {
                            nDaqCnt++;
                            goto DaqReConnect;
                        }
                    }
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                    }));

                    LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                }
            }
            else
            {
            //연결이 되었는 지 재 확인 220926
            DaqReConnect:

                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                if (daq != null && daq.sp != null)
                {
                    LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                }));

                Thread.Sleep(1000);

                if (daq != null && daq.sp.IsOpen)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                    }));

                    LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                }
                else
                {
                    if (nDaqCnt >= 5)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                        }));

                        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                        ngDeviceList.Add(checkDeviceName);
                    }
                    else
                    {
                        nDaqCnt++;
                        goto DaqReConnect;
                    }
                }
            }

            if(ngDeviceList.Count > 0)
            {
                ti.Value_ = "DAQ_COMMUNICATION_NG";
                return JudgementTestItem(ti);
            }

            bool bCheckDaqVolage = DaqVoltageCheck_BeforeDcir();

            if (bCheckDaqVolage == false)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DeviceCheckWindow rt = new DeviceCheckWindow(this);
                    rt.Height += 100;
                    rt.Width += 200;
                    rt.maintitle.FontSize += 20;
                    rt.maintitle.Content = "DAQ DEVICE PROBLEM";
                    rt.reason.FontSize += 13;
                    rt.reason.Content = "Please Turn Off/On DAQ!!";
                    rt.okbt.FontSize += 13;
                    rt.Show();
                }));

                ti.Value_ = "DAQ_VOLTAGE_NG";
                return JudgementTestItem(ti);
            }
            #endregion

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
            
            ti.refValues_.Clear();

            // 20191125 Noah Choi DCIR전 모듈온도 측정부분 추가 및 상세수집 항목 추가 요청
            Before_DCIR_Module_Thermistor_1();
            Before_DCIR_Module_Thermistor_2();

            
            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 
            moduleTempAverage = (thermistor1 + thermistor2) / 2;
            LogState(LogType.TEST, string.Format("(moduleThermistor1 :{0} + moduleThermistor2 :{1}) / 2 = moduleTempAverage :{2}", thermistor1, thermistor2, moduleTempAverage));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4058", moduleTempAverage.ToString()));


            dcirTemp = temps.tempStr;
            LogState(LogType.Info, "Room Temp :" + dcirTemp.ToString());
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4002", dcirTemp.ToString("N3")));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4027", befDiscRestTime));      //1step, rest run time
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4028", discCur));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4029", discTime));             //2step, discharge run time
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4030", discCurLimit));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4031", aftDiscRestTime));      //3step, rest run time
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4032", charCur));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4033", charTime));             //4step, charge run time
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4034", aftCharRestTime));      //5step, rest run time
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4035", charCurLimit));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4036", safeVoltHighLimit));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4037", safeVoltLowLimit));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4040", cellFomula1.ToString()));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4041", cellFomula2.ToString()));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4042", moduleFomula1.ToString()));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4043", moduleFomula2.ToString()));

            if (!ModeSet_Release(ti,false))
            {
                return false;
            }

            if (!ModeSet(ti))
            { 
                return false;
            }

            cycler.is84On = false;
            counter_Cycler++;

            if (!Do_Rest_Charge(ti))
            { 
                return false;
            }

            // 211216 JKD - DMM34970 -> DMM970A 변경으로 인해 TackTime 지연(4sec ->13sec)됨으로 기존라인에서만 동작
            if (!isLineFlag)
            {
                if (!ModeSet_Release(ti,false))
                {
                    return false;
                }
            }

            //DCIR_Stopped();
            SetMainVoltage("0.00");
            SetMainCurrent("0.00");
            SetMainCState("Ready");
            DCIRThreadAbort();
            
            Thread.Sleep(500);

            #endregion

            SetCounter_Cycler();

            endCurrent_DIS = double.Parse(discCur);

            #region 저항 관련 상세수집 추가 부분

            상세저항1.Key = "W2MLMTE4049";
            상세저항2.Key = "W2MLMTE4050";
            ti.refValues_.Add(상세저항1);
            ti.refValues_.Add(상세저항2);


            //초기 rest 2초 - 2번째
            //방전 10초 - 14번째
            //중기 rest 10초 - 24번째
            //충전 10초 - 34번째
            //후기 rest 40초 - 74번째
            //MC 붙이는 시간이 있으니 2초씩 미룬다
            //그럼 갯수는 무적권 70개 이상일것으로 예상된다.
            //> [FW: Ford C727 module Temp 측정 후 상세수집항목 기록 timing 수정 요청 건] 메일 참조

            /*
            //190612
            if (상세저항1.Reportitems.Count > 70)
            {
                //Module Temp before discharge
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4022", 상세저항1.Reportitems[0].ToString() + "&" + 상세저항2.Reportitems[0].ToString()));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4023", 상세저항1.Reportitems[11].ToString() + "&" + 상세저항2.Reportitems[11].ToString()));
                discharge10sTemp1 = double.Parse(상세저항1.Reportitems[11].ToString());
                discharge10sTemp2 = double.Parse(상세저항2.Reportitems[11].ToString());
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4024", 상세저항1.Reportitems[32].ToString() + "&" + 상세저항2.Reportitems[32].ToString()));


                ti.refValues_.Add(MakeDetailItem("W2MLMTE4047", 상세저항1.Reportitems[11].ToString() + "&" + 상세저항2.Reportitems[11].ToString()));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4048", 상세저항1.Reportitems[21].ToString() + "&" + 상세저항2.Reportitems[21].ToString()));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4051", 상세저항1.Reportitems[31].ToString() + "&" + 상세저항2.Reportitems[31].ToString()));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4052", 상세저항1.Reportitems[71].ToString()));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4053", 상세저항2.Reportitems[71].ToString()));
                rest40stemp1 = double.Parse(상세저항1.Reportitems[71].ToString());
                rest40stemp2 = double.Parse(상세저항2.Reportitems[71].ToString());

                reststemp1 = double.Parse(상세저항1.Reportitems[0].ToString());
                reststemp2 = double.Parse(상세저항2.Reportitems[0].ToString());

            }
            else
            {
                LogState(LogType.Fail, "-----------------------------------------------상세저항1.Reportitems.Count < 70, refValue is NOT Added!!");
            }
            */

            //220223기준 위 주석은 개소리다 초심을 잃지 않고 언제나 돌다리도 개같이 두들겨 보고 건너기 위해 주석은 유지한다

            if ( ( 상세저항1.Reportitems.Count == mTotalRunCountDCIR && 상세저항2.Reportitems.Count == mTotalRunCountDCIR ) &&
                 ( mTotalRunCountDCIR >= 33) ) //수집된 상세 저항은 총 진행 시간과 동일 해야하고, 총 진행 시간은 최소 33초 이상 이어야함
            {
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4022", 상세저항1.Reportitems[0 ].ToString() + "&" + 상세저항2.Reportitems[0 ].ToString()));    //1step(rest) 1초
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4023", 상세저항1.Reportitems[11].ToString() + "&" + 상세저항2.Reportitems[11].ToString()));    //2step(dis)  12초
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4047", 상세저항1.Reportitems[11].ToString() + "&" + 상세저항2.Reportitems[11].ToString()));    //2step(dis)  12초(왜 또 넣지?)
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4048", 상세저항1.Reportitems[21].ToString() + "&" + 상세저항2.Reportitems[21].ToString()));    //3step(rest) 22초
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4051", 상세저항1.Reportitems[31].ToString() + "&" + 상세저항2.Reportitems[31].ToString()));    //4step(cha)  32초
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4024", 상세저항1.Reportitems[32].ToString() + "&" + 상세저항2.Reportitems[32].ToString()));    //5step(rest) 33초

                reststemp1 = double.Parse(상세저항1.Reportitems[0].ToString());                                                                         //다른 시험 항목에서 해당 값 사용
                reststemp2 = double.Parse(상세저항2.Reportitems[0].ToString());                                                                         //다른 시험 항목에서 해당 값 사용

                //-> 해당 포인트까지는 변경 없이 기존 내용 유지

                ti.refValues_.Add(MakeDetailItem("W2MLMTE4052", 상세저항1.Reportitems.Last().ToString()));                                              //5step(rest) 72초 에서 마지막 데이터 
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4053", 상세저항2.Reportitems.Last().ToString()));                                              //5step(rest) 72초 에서 마지막 데이터

                mLastResistanceTemp1 = double.Parse(상세저항1.Reportitems.Last().ToString());                                                           //다른 시험 항목에서 해당 값 사용
                mLastResistanceTemp2 = double.Parse(상세저항2.Reportitems.Last().ToString());                                                           //다른 시험 항목에서 해당 값 사용
            }
            else
            {
                LogState(LogType.Fail, "1.Reportitems.Count : "         + 상세저항1.Reportitems.Count.ToString() +
                                       ", 2.Reportitems.Count : "       + 상세저항2.Reportitems.Count.ToString() +
                                       ", mTotalRunCountDCIR Count : "  + mTotalRunCountDCIR.ToString());
            }

            foreach (var item in 상세저항1.Reportitems)
            {
                LogState(LogType.Info, "[IN DCIR] TEMP1," + item.ToString());
            }
            foreach (var item in 상세저항2.Reportitems)
            {
                LogState(LogType.Info, "[IN DCIR] TEMP2," + item.ToString());
            }

            #endregion

            var point1 = modulePoint_RES_DIS;
            var point2 = modulePoint_DIS;
            // ((REST_DISCHARGE_Voltage - DISCHARGE_Voltage) / (DISCHARGE_Current)) * 1000
            tempdcirmodule_DIS = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴
             
            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcirmodule_DIS));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4055", tempdcirmodule_DIS.ToString()));

            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcirmodule_DIS, moduleFomula1, dcirTemp, moduleFomula2));
            //ti.Value_ = tempdcirmodule_DIS = (tempdcirmodule_DIS + (moduleFomula1 * (dcirTemp - 25))) / moduleFomula2;

            //20201102 BEFORE CHAGE
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcirmodule_DIS, moduleFomula1, moduleTempAverage, moduleFomula2));
            //ti.Value_ = tempdcirmodule_DIS = (tempdcirmodule_DIS + (moduleFomula1 * (moduleTempAverage - 25))) / moduleFomula2;

            //20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcirmodule_DIS, moduleFomula1, moduleTempAverage, moduleFomula2));
            ti.Value_ = tempdcirmodule_DIS = (tempdcirmodule_DIS + (moduleFomula1 * (moduleTempAverage - 25))) + moduleFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        private int GetStepRunCountDCIR(int stepNumber/*1base*/)
        {
            int runTime = 0; //1base

            try
            {
                switch (stepNumber)
                {
                    case 1: runTime = int.Parse(befDiscRestTime);   break;
                    case 2: runTime = int.Parse(discTime);          break;
                    case 3: runTime = int.Parse(aftDiscRestTime);   break;
                    case 4: runTime = int.Parse(charTime);          break;
                    case 5: runTime = int.Parse(aftCharRestTime);   break;
                }
            }
            catch(Exception ec)
            {
                LogState(LogType.Info, MethodBase.GetCurrentMethod().Name +
                                   ", Exception : " + ec.Message);

                runTime = 0;
            }

            return runTime;
        }

        private int GetTotalRunCountDCIR()
        {
            // 1Step ~ 5Step(현재 고정 사용) 의 각각의 런타임 합을 반환 한다

            int totalRunTime = 0; //1base

            try
            {
                totalRunTime += int.Parse(befDiscRestTime);

                totalRunTime += int.Parse(discTime);

                totalRunTime += int.Parse(aftDiscRestTime);

                totalRunTime += int.Parse(charTime);

                totalRunTime += int.Parse(aftCharRestTime);
            }
            catch(Exception ec)
            {
                LogState(LogType.Info, MethodBase.GetCurrentMethod().Name +
                                   ", Exception : " + ec.Message);

                totalRunTime = 0;
            }

            LogState(LogType.Info, MethodBase.GetCurrentMethod().Name + 
                                   ", Total Run Time(sec) :" + 
                                   totalRunTime.ToString());

            return totalRunTime;
        }

        private void DCIRThreadAbort()
        {
            if (Protection_ID1 != null)
            {
                Protection_ID1.Abort();
                Protection_ID1 = null;
            }
            if (Protection_CHARGE_ID1 != null)
            {
                Protection_CHARGE_ID1.Abort();
                Protection_CHARGE_ID1 = null;
            }
            if (Protection_DISCHARGE_ID1 != null)
            {
                Protection_DISCHARGE_ID1.Abort();
                Protection_DISCHARGE_ID1 = null;
            }             
            if (dp_disc != null)
            {
                dp_disc.Abort();
                dp_disc = null;
            }
            if (dp_char != null)
            {
                dp_char.Abort();
                dp_char = null;
            }
            if (dp_rest_bef_disc != null)
            {
                dp_rest_bef_disc.Abort();
                dp_rest_bef_disc = null;
            }
            if (dp_rest_aft_disc != null)
            {
                dp_rest_aft_disc.Abort();
                dp_rest_aft_disc = null;
            }
            if (dp_rest_aft_char != null)
            {
                dp_rest_aft_char.Abort();
                dp_rest_aft_char = null;
            }
            if (dp_rest_aft_char_10s != null)
            {
                dp_rest_aft_char_10s.Abort();
                dp_rest_aft_char_10s = null;
            }
            if (dp_rest_aft_char_20s != null)
            {
                dp_rest_aft_char_20s.Abort();
                dp_rest_aft_char_20s = null;
            }
            if (dp_rest_aft_char_30s != null)
            {
                dp_rest_aft_char_30s.Abort();
                dp_rest_aft_char_30s = null;
            }
            if (dp_rest_aft_char_40s != null)
            {
                dp_rest_aft_char_40s.Abort();
                dp_rest_aft_char_40s = null;
            }

            //221006 MC 저항 측정 및 수집항목 스레드 추가 nnkim
            if (dp_McShortCheck != null)
            {
                dp_McShortCheck.Abort();
                dp_McShortCheck = null;
            }

            if (Protection_McResistanceStart != null)
            {
                Protection_McResistanceStart.Abort();
                Protection_McResistanceStart = null;
            }

            if (Protection_PartsCountStart != null)
            {
                Protection_PartsCountStart.Abort();
                Protection_PartsCountStart = null;
            }
        }


        public bool Cell1_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_1;
            var point2 = cellDetailList[1].CellVolt_1;

            tempdcircell1 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell1));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE 
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell1, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell1 = (tempdcircell1 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE 
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell1, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell1 = (tempdcircell1 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell2_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_2;
            var point2 = cellDetailList[1].CellVolt_2;

            tempdcircell2 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell2));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE            
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell2, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell2 = (tempdcircell2 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell2, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell2 = (tempdcircell2 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());
      
            
            return JudgementTestItem(ti);
        }
        public bool Cell3_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_3;
            var point2 = cellDetailList[1].CellVolt_3;

            tempdcircell3 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell3));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE 
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell3, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell3 = (tempdcircell3 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE 
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell3, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell3 = (tempdcircell3 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell4_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_4;
            var point2 = cellDetailList[1].CellVolt_4;

            tempdcircell4 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell4));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE 
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell4, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell4 = (tempdcircell4 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell4, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell4 = (tempdcircell4 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell5_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_5;
            var point2 = cellDetailList[1].CellVolt_5;

            tempdcircell5 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell5));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE 
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell5, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell5 = (tempdcircell5 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE 
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell5, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell5 = (tempdcircell5 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell6_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_6;
            var point2 = cellDetailList[1].CellVolt_6;

            tempdcircell6 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell6));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell6, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell6 = (tempdcircell6 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell6, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell6 = (tempdcircell6 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell7_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_7;
            var point2 = cellDetailList[1].CellVolt_7;

            tempdcircell7 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell7));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell7, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell7 = (tempdcircell7 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell7, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell7 = (tempdcircell7 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell8_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_8;
            var point2 = cellDetailList[1].CellVolt_8;

            tempdcircell8 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell8));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell8, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell8 = (tempdcircell8 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());
            
            // 20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell8, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell8 = (tempdcircell8 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell9_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_9;
            var point2 = cellDetailList[1].CellVolt_9;

            tempdcircell9 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell9));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell9, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell9 = (tempdcircell9 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 BEFORE CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell9, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell9 = (tempdcircell9 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell10_DCIR_C727(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_10;
            var point2 = cellDetailList[1].CellVolt_10;

            tempdcircell10 = ((point1 - point2) / (endCurrent_DIS)) * 1000;//밀리옴

            LogState(LogType.TEST, "origin DCIR Value:" +
                string.Format("({0} - {1}) / {2}) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell10));

            // 20191124 Noah Choi 온도보정식 계산을 외기온도가 아닌 모듈온도(Thermistor 2개의 평균값)로 계산하도록 수정 요청(유경석 사원님) 

            // 20201102 BEFORE CHAGE
            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) / {3}", tempdcircell10, cellFomula1, moduleTempAverage, cellFomula2));
            //ti.Value_ = tempdcircell10 = (tempdcircell10 + (cellFomula1 * (moduleTempAverage - 25))) / cellFomula2;
            //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            // 20201102 AFTER CHAGE
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 25)) + {3}", tempdcircell10, cellFomula1, moduleTempAverage, cellFomula2));
            ti.Value_ = tempdcircell10 = (tempdcircell10 + (cellFomula1 * (moduleTempAverage - 25))) + cellFomula2;
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell_DCIR_Deviation_C727(TestItem ti)
        {
            isProcessingUI(ti);
            ti.refValues_.Clear();
            var dcircell = new List<double>();
            //localTypes 0 - 4P8S, 1 - 4P8S_Rev, 2 - 4P7S, 3 - 3P8S, 4 - 3P10S, 5 - 3P10S_Rev
            if (localTypes == 0 || localTypes == 1 || localTypes == 3)
            {
                dcircell.Add(tempdcircell1);
                dcircell.Add(tempdcircell2);
                dcircell.Add(tempdcircell3);
                dcircell.Add(tempdcircell4);
                dcircell.Add(tempdcircell5);
                dcircell.Add(tempdcircell6);
                dcircell.Add(tempdcircell7);
                dcircell.Add(tempdcircell8);

                var dti = new DetailItems() { Key = "W2MLMTE4056" };
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_1 - cellDetailList[1].CellVolt_1) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_2 - cellDetailList[1].CellVolt_2) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_3 - cellDetailList[1].CellVolt_3) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_4 - cellDetailList[1].CellVolt_4) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_5 - cellDetailList[1].CellVolt_5) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_6 - cellDetailList[1].CellVolt_6) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_7 - cellDetailList[1].CellVolt_7) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_8 - cellDetailList[1].CellVolt_8) / (endCurrent_DIS)) * 1000); 
                ti.refValues_.Add(dti);
            }
            else if(localTypes == 4 || localTypes == 5)
            {
                dcircell.Add(tempdcircell1);
                dcircell.Add(tempdcircell2);
                dcircell.Add(tempdcircell3);
                dcircell.Add(tempdcircell4);
                dcircell.Add(tempdcircell5);
                dcircell.Add(tempdcircell6);
                dcircell.Add(tempdcircell7);
                dcircell.Add(tempdcircell8);
                dcircell.Add(tempdcircell9);
                dcircell.Add(tempdcircell10);


                var dti = new DetailItems() { Key = "W2MLMTE4056" };
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_1 - cellDetailList[1].CellVolt_1) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_2 - cellDetailList[1].CellVolt_2) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_3 - cellDetailList[1].CellVolt_3) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_4 - cellDetailList[1].CellVolt_4) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_5 - cellDetailList[1].CellVolt_5) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_6 - cellDetailList[1].CellVolt_6) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_7 - cellDetailList[1].CellVolt_7) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_8 - cellDetailList[1].CellVolt_8) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_9 - cellDetailList[1].CellVolt_9) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_10 - cellDetailList[1].CellVolt_10) / (endCurrent_DIS)) * 1000);
                ti.refValues_.Add(dti);
            }
            else if(localTypes == 2) 
            {
                dcircell.Add(tempdcircell1);
                dcircell.Add(tempdcircell2);
                dcircell.Add(tempdcircell3);
                dcircell.Add(tempdcircell4);
                dcircell.Add(tempdcircell5);
                dcircell.Add(tempdcircell6);
                dcircell.Add(tempdcircell7);


                var dti = new DetailItems() { Key = "W2MLMTE4056" };
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_1 - cellDetailList[1].CellVolt_1) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_2 - cellDetailList[1].CellVolt_2) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_3 - cellDetailList[1].CellVolt_3) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_4 - cellDetailList[1].CellVolt_4) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_5 - cellDetailList[1].CellVolt_5) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_6 - cellDetailList[1].CellVolt_6) / (endCurrent_DIS)) * 1000);
                dti.Reportitems.Add(((cellDetailList[0].CellVolt_7 - cellDetailList[1].CellVolt_7) / (endCurrent_DIS)) * 1000); 
                ti.refValues_.Add(dti);
            }
            // 20191126 Noah Choi 반올림 없이로 변경 요청(유경석 사원님)
            var max = double.Parse(dcircell.Max().ToString());//190830 ljs request
            var min = double.Parse(dcircell.Min().ToString());

            ti.Value_ = max - min; 


            LogState(LogType.Info,
                "DCIR Devation - Max Cell DCIR :" + max.ToString() +
                "/Min Cell DCIR :" + min.ToString() +
                "/Result :" + ti.Value_.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell_DCIR_Ratio_C727(TestItem ti)
        {
            isProcessingUI(ti);

            var dcircell = new List<double>();
            //localTypes 0 - 4P8S, 1 - 4P8S_Rev, 2 - 4P7S, 3 - 3P8S, 4 - 3P10S, 5 - 3P10S_Rev
            if (localTypes == 0 || localTypes == 1 || localTypes == 3)
            {
                dcircell.Add(tempdcircell1);
                dcircell.Add(tempdcircell2);
                dcircell.Add(tempdcircell3);
                dcircell.Add(tempdcircell4);
                dcircell.Add(tempdcircell5);
                dcircell.Add(tempdcircell6);
                dcircell.Add(tempdcircell7);
                dcircell.Add(tempdcircell8);
            }
            else if (localTypes == 4 || localTypes == 5)
            {
                dcircell.Add(tempdcircell1);
                dcircell.Add(tempdcircell2);
                dcircell.Add(tempdcircell3);
                dcircell.Add(tempdcircell4);
                dcircell.Add(tempdcircell5);
                dcircell.Add(tempdcircell6);
                dcircell.Add(tempdcircell7);
                dcircell.Add(tempdcircell8);
                dcircell.Add(tempdcircell9);
                dcircell.Add(tempdcircell10);
            }
            else if (localTypes == 2)
            {
                dcircell.Add(tempdcircell1);
                dcircell.Add(tempdcircell2);
                dcircell.Add(tempdcircell3);
                dcircell.Add(tempdcircell4);
                dcircell.Add(tempdcircell5);
                dcircell.Add(tempdcircell6);
                dcircell.Add(tempdcircell7);
            }
            //var max = double.Parse(dcircell.Max().ToString("F2"));//190830 ljs request
            //var min = double.Parse(dcircell.Min().ToString("F2"));

            // 20191126 Noah Choi 반올림 없이로 변경 요청(유경석 사원님)
            var max = double.Parse(dcircell.Max().ToString());
            var min = double.Parse(dcircell.Min().ToString());

            ti.Value_ = (max / min) * 100;

            LogState(LogType.Info,
                "DCIR Ratio - Max Cell DCIR :" + max.ToString() +
                "/Min Cell DCIR :" + min.ToString() +
                "/Result :" + ti.Value_.ToString());

            // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
            relays.RelayOff("IDO_4");

            return JudgementTestItem(ti);
        }


        public void Before_DCIR_Module_Thermistor_1()
        {
            var tempTher1 = 0.0;
            double resultTemp = 0.0;

            if (isLineFlag)
            {
                #region DAQ970A

                List<double> resList = new List<double>();
                tempCnt = 2;

                if (daq970_1 == null)
                {
                    LogState(LogType.Fail, "KeysightDMM Open_Fail");
                    return;
                }
            
                if (!daq970_1.MeasRes_Multi(out resList, "318,319", 2))
                {
                    LogState(LogType.Fail, "KeysightDMM Read IDN Time out");
                    return;
                }
                else
                {
                    tempList = new List<double>();
                    foreach (var item in resList)
                    {
                        tempList.Add(item * 0.001);
                    }
                }
                #endregion
            }
            else
            {
                #region DMM34970

                if (keysight == null)
                {
                    LogState(LogType.Fail, "KeysightDMM Open_Fail");
                    return;
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
                                LogState(LogType.Fail, "KeysightDMM Read IDN Time out");
                                return;
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
                #endregion
            }

            tempTher1 = 0.0;
            if (tempList != null && tempList.Count == tempCnt)
            {
                tempTher1 = tempList[0];
            }

            resultTemp = 0.0;
            //tempTher1 = 38.0606;
            Calculate(tempTher1, out resultTemp);
            thermistor1 = resultTemp;

            //ti.refValues_.Add(MakeDetailItem("W2MLMTE4001", dcirTemp.ToString("N3")));
        }

        public void Before_DCIR_Module_Thermistor_2()
        {
            var tempTher1 = 0.0;
            if (tempList != null && tempList.Count == tempCnt)
            {
                tempTher1 = tempList[1];
            }
            double resultTemp = 0.0;
            Calculate(tempTher1, out resultTemp);
            thermistor2 = resultTemp;

            //ti.refValues_.Add(MakeDetailItem("W2MLMTE4046", thermistor1.ToString() + "&" + thermistor2.ToString()));
        }

        public bool DaqVoltageCheck_BeforeDcir()
        {
            if (localTypes == 0)
            {
                //190107 by grchoi
                //DAQ 포트 이상 및 코드 통일을 위한 소켓 변경
                //4P8S
                if (daq.DAQList[10] < 3 || daq.DAQList[11] < 3 || daq.DAQList[12] < 3 || daq.DAQList[13] < 3
                    || daq.DAQList[14] < 3 || daq.DAQList[15] < 3 || daq.DAQList[16] < 3 || daq.DAQList[17] < 3)
                {
                    return false;
                }                                                                                              
            }
            else if (localTypes == 1)
            {
                //4P8S Reverse
                if (daq.DAQList[28] < 3 || daq.DAQList[29] < 3 || daq.DAQList[30] < 3 || daq.DAQList[31] < 3
                || daq.DAQList[32] < 3 || daq.DAQList[33] < 3 || daq.DAQList[34] < 3 || daq.DAQList[35] < 3)
                {
                    return false;
                }    
            }
            else if (localTypes == 2)
            {
                //4P7S
                if (daq.DAQList[36] < 3 || daq.DAQList[37] < 3 || daq.DAQList[38] < 3 || daq.DAQList[39] < 3
                || daq.DAQList[40] < 3 || daq.DAQList[41] < 3 || daq.DAQList[42] < 3)
                {
                    return false;
                }  
            }
            else if (localTypes == 3)
            {
                //3P8S
                if (daq.DAQList[10] < 3 || daq.DAQList[11] < 3 || daq.DAQList[12] < 3 || daq.DAQList[13] < 3
                    || daq.DAQList[14] < 3 || daq.DAQList[15] < 3 || daq.DAQList[16] < 3 || daq.DAQList[17] < 3)
                {
                    return false;
                }    
            }
            else if (localTypes == 4)
            {
                //3P10S
                if (daq.DAQList[0] < 3 || daq.DAQList[1] < 3 || daq.DAQList[2] < 3 || daq.DAQList[3] < 3
                  || daq.DAQList[4] < 3 || daq.DAQList[5] < 3 || daq.DAQList[6] < 3 || daq.DAQList[7] < 3
                  || daq.DAQList[8] < 3 || daq.DAQList[9] < 3)
                {
                    return false;
                }    
            }
            else if (localTypes == 5)
            {
                //3P10S Rev
                if (daq.DAQList[18] < 3 || daq.DAQList[19] < 3 || daq.DAQList[20] < 3 || daq.DAQList[21] < 3
                    || daq.DAQList[22] < 3 || daq.DAQList[23] < 3 || daq.DAQList[24] < 3 || daq.DAQList[25] < 3
                   || daq.DAQList[26] < 3 || daq.DAQList[27] < 3)
                {
                    return false;
                } 
            }

            //ti.refValues_.Add(MakeDetailItem("W2MLMTE4046", thermistor1.ToString() + "&" + thermistor2.ToString()));

            return true;
        }


        public string mcResistanceposition;

        public string McResistanceposition
        {
            get { return mcResistanceposition; }
            set { mcResistanceposition = value; }
        }

        //221004 저항 측정하는 함수 nnkim
        public void McResistanceMeasure(string strPosition, out List<string> list, bool bDelayResult = false)
        {
            list = new List<string>();
            var rtList = new List<double>();
            var rtListNew = new List<double>();

            var dmmChInfo = new DAQChannelInfo(localTypes);

            double dResultTemp = 0.0;
            var rtListTemp = new List<double>();

            //
            if (lineFlag == "1")
            {
                daq970_1.MeasRes_Multi(out rtListNew, dmmChInfo.ModuleResMCCH, 5);

                switch (dmmChInfo.ModuleResMCCnt)
                {
                    case 0:
                        for (int i = 0; i < 18; i++)
                        {
                            if (i == 0) rtList.Add(rtListNew[0]);
                            else if (i == 1 || i == 2) rtList.Add(rtListNew[1]);
                            else if (i == 3 || i == 4) rtList.Add(rtListNew[2]);
                            else if (i == 5 || i == 6) rtList.Add(rtListNew[3]);
                            else if (i == 15) rtList.Add(rtListNew[4]);
                            else rtList.Add(1.234);

                        }
                        break;
                    case 1:
                        for (int i = 0; i < 18; i++)
                        {
                            if (i == 0) rtList.Add(rtListNew[0]);
                            else if (i == 1 || i == 2) rtList.Add(rtListNew[1]);
                            else if (i == 7 || i == 8) rtList.Add(rtListNew[2]);
                            else if (i == 9 || i == 10) rtList.Add(rtListNew[3]);
                            else if (i == 16) rtList.Add(rtListNew[4]);
                            else rtList.Add(1.234);
                        }
                        break;
                    case 2:
                        for (int i = 0; i < 18; i++)
                        {
                            if (i == 0) rtList.Add(rtListNew[0]);
                            else if (i == 1 || i == 2) rtList.Add(rtListNew[1]);
                            else if (i == 11 || i == 12) rtList.Add(rtListNew[2]);
                            else if (i == 13 || i == 14) rtList.Add(rtListNew[3]);
                            else if (i == 17) rtList.Add(rtListNew[4]);
                            else rtList.Add(1.234);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //////////////////////
                if (keysight != null)
                {
                    //isConnFail = false;
                    keysight.MeasTemp(dmmChInfo.ModuleResMCCH);

                    int rec = keysight.sp.BytesToRead;

                    int cnt = 0;
                    //채널한개값 16 + etx
                    while (rec < 81)
                    {
                        Thread.Sleep(100);
                        rec = keysight.sp.BytesToRead;
                        cnt += 100;
                        Thread.Sleep(100);
                        //not received data
                        if (cnt == 5000)
                        {
                            keysight.MeasTemp(dmmChInfo.ModuleResMCCH);

                            rec = keysight.sp.BytesToRead;

                            cnt = 0;
                            while (rec < 17)
                            {
                                Thread.Sleep(100);
                                rec = keysight.sp.BytesToRead;
                                cnt += 100;
                                if (cnt == 5000)
                                {
                                    LogState(LogType.Info, "Cooling pin connection check limit - Connection Check FAIL ");
                                }
                            }
                            break;
                        }
                    }

                    if (cnt != 5000)
                    {

                        //받은 후에 데이터 파싱
                        byte[] bt = new byte[rec];
                        keysight.sp.Read(bt, 0, rec);

                        keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

                        var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        switch (dmmChInfo.ModuleResMCCnt)
                        {
                            case 0:
                                for (int i = 0; i < 18; i++)
                                {
                                    if (i == 0) rtList.Add(double.Parse(vArr[0]));
                                    else if (i == 1) rtList.Add(double.Parse(vArr[1]));
                                    else if (i == 2) rtList.Add(double.Parse(vArr[2]));
                                    else if (i == 3) rtList.Add(double.Parse(vArr[3]));
                                    else if (i == 8) rtList.Add(double.Parse(vArr[4]));
                                    else rtList.Add(1.234);
                                }
                                break;
                            case 1:
                                for (int i = 0; i < 18; i++)
                                {
                                    if (i == 0) rtList.Add(double.Parse(vArr[0]));
                                    else if (i == 1) rtList.Add(double.Parse(vArr[1]));
                                    else if (i == 4) rtList.Add(double.Parse(vArr[2]));
                                    else if (i == 5) rtList.Add(double.Parse(vArr[3]));
                                    else if (i == 9) rtList.Add(double.Parse(vArr[4]));
                                    else rtList.Add(1.234);
                                }
                                break;
                            case 2:
                                for (int i = 0; i < 18; i++)
                                {
                                    if (i == 0) rtList.Add(double.Parse(vArr[0]));
                                    else if (i == 1) rtList.Add(double.Parse(vArr[1]));
                                    else if (i == 6) rtList.Add(double.Parse(vArr[2]));
                                    else if (i == 7) rtList.Add(double.Parse(vArr[3]));
                                    else if (i == 10) rtList.Add(double.Parse(vArr[4]));
                                    else rtList.Add(1.234);
                                }
                                break;
                            default:
                                break;
                        }                       
                    }
                }
            }
            
            ////////////////////////////////////////
            try
            {
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

                 var regStrOffset1ch = rk.GetValue("MC3_OFFSET") as string;
                 var regStrOffset2ch = rk.GetValue("MC4_OFFSET") as string;
                 var regStrOffset3ch = rk.GetValue("MC5_OFFSET") as string;
                 var regStrOffset4ch = rk.GetValue("MC6_OFFSET") as string;
                 var regStrOffset5ch = rk.GetValue("MC7_OFFSET") as string;
                 var regStrOffset6ch = rk.GetValue("MC8_OFFSET") as string;
                 var regStrOffset7ch = rk.GetValue("MC9_OFFSET") as string;
                 var regStrOffset8ch = rk.GetValue("MC10_OFFSET") as string;
                 var regStrOffset9ch = rk.GetValue("MC11_OFFSET") as string;
                 var regStrOffset10ch = rk.GetValue("MC12_OFFSET") as string;
                 var regStrOffset11ch = rk.GetValue("MC13_OFFSET") as string;
                 var regStrOffset12ch = rk.GetValue("MC14_OFFSET") as string;
                 var regStrOffset13ch = rk.GetValue("MC15_OFFSET") as string;
                 var regStrOffset14ch = rk.GetValue("MC16_OFFSET") as string;
                 var regStrOffset15ch = rk.GetValue("MC17_OFFSET") as string;
                 var regStrOffset16ch = rk.GetValue("MC18_OFFSET") as string;
                 var regStrOffset17ch = rk.GetValue("MC19_OFFSET") as string;
                 var regStrOffset18ch = rk.GetValue("MC20_OFFSET") as string;
                 
                 //Data 정리
                 for (int i = 0; i < 18; i++)
                 {
                     rtListTemp.Add(rtList[i]);
                 }
                 
                 //MC3
                 if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                 else { dResultTemp = double.Parse(regStrOffset1ch); rtListTemp[0] = rtListTemp[0] - dResultTemp; }
                 
                 //MC4
                 if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset2ch); rtListTemp[1] = rtListTemp[1] - dResultTemp; }
                 
                 //MC5
                 if (regStrOffset3ch == null) { rk.SetValue("MC5_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset3ch); rtListTemp[2] = rtListTemp[2] - dResultTemp; }
                 
                 if (regStrOffset4ch == null) { rk.SetValue("MC6_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset4ch); rtListTemp[3] = rtListTemp[3] - dResultTemp; }
                 
                 if (regStrOffset5ch == null) { rk.SetValue("MC7_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset5ch); rtListTemp[4] = rtListTemp[4] - dResultTemp; }
                 
                 if (regStrOffset6ch == null) { rk.SetValue("MC8_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset6ch); rtListTemp[5] = rtListTemp[5] - dResultTemp; }
                 
                 if (regStrOffset7ch == null) { rk.SetValue("MC9_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset7ch); rtListTemp[6] = rtListTemp[6] - dResultTemp; }
                 
                 if (regStrOffset8ch == null) { rk.SetValue("MC10_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset8ch); rtListTemp[7] = rtListTemp[7] - dResultTemp; }
                 
                 if (regStrOffset9ch == null) { rk.SetValue("MC11_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset9ch); rtListTemp[8] = rtListTemp[8] - dResultTemp; }
                 
                 if (regStrOffset10ch == null) { rk.SetValue("MC12_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset10ch); rtListTemp[9] = rtListTemp[9] - dResultTemp; }
                 
                 if (regStrOffset11ch == null) { rk.SetValue("MC13_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset11ch); rtListTemp[10] = rtListTemp[10] - dResultTemp; }
                 
                 if (regStrOffset12ch == null) { rk.SetValue("MC14_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset12ch); rtListTemp[11] = rtListTemp[11] - dResultTemp; }
                 
                 if (regStrOffset13ch == null) { rk.SetValue("MC15_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset13ch); rtListTemp[12] = rtListTemp[12] - dResultTemp; }
                 
                 if (regStrOffset14ch == null) { rk.SetValue("MC16_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset14ch); rtListTemp[13] = rtListTemp[13] - dResultTemp; }
                 
                 if (regStrOffset15ch == null) { rk.SetValue("MC17_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset15ch); rtListTemp[14] = rtListTemp[14] - dResultTemp; }
                 
                 if (regStrOffset16ch == null) { rk.SetValue("MC18_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset16ch); rtListTemp[15] = rtListTemp[15] - dResultTemp; }
                 
                 if (regStrOffset17ch == null) { rk.SetValue("MC19_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset17ch); rtListTemp[16] = rtListTemp[16] - dResultTemp; }
                 
                 if (regStrOffset18ch == null) { rk.SetValue("MC20_OFFSET", "0.0"); dResultTemp = 0.0; }
                 else { dResultTemp = double.Parse(regStrOffset18ch); rtListTemp[17] = rtListTemp[17] - dResultTemp; }

                 for (int i = 0; i < 18; i++)
                 {
                    if(rtListTemp[i] == 1.234) { list.Add("not used");               }
                    else                       { list.Add(rtListTemp[i].ToString()); }                     
                 }

                 string strTemp = list[0].ToString() + "," + list[1].ToString()
                  + "," + list[2].ToString() + "," + list[3].ToString()
                   + "," + list[4].ToString() + "," + list[5].ToString()
                   + "," + list[6].ToString() + "," + list[7].ToString()
                   + "," + list[8].ToString() + "," + list[9].ToString()
                   + "," + list[10].ToString() + "," + list[11].ToString()
                   + "," + list[12].ToString() + "," + list[13].ToString()
                   + "," + list[14].ToString() + "," + list[15].ToString()
                   + "," + list[16].ToString() + "," + list[17].ToString();


                 LogState(LogType.Info, "Result : ," + strTemp + ", (McResistanceMeasure())");
            }
            catch (Exception ec)
            {
                this.LogState(LogType.Fail, "MC McResistanceMeasure NG", ec);
            }

        }

        //221006 저항 측정 상세수집
        void Point_Mc_Short_check(object aa)
        {
            Thread.Sleep(8000);

            try
            {
                var ti = aa as TestItem;
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_3_SHORT_CHECK_NEW, rtMcList[0]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_4_SHORT_CHECK_NEW, rtMcList[1]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_5_SHORT_CHECK_NEW, rtMcList[2]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_6_SHORT_CHECK_NEW, rtMcList[3]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_7_SHORT_CHECK_NEW, rtMcList[4]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_8_SHORT_CHECK_NEW, rtMcList[5]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_9_SHORT_CHECK_NEW, rtMcList[6]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_10_SHORT_CHECK_NEW, rtMcList[7]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_11_SHORT_CHECK_NEW, rtMcList[8]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_12_SHORT_CHECK_NEW, rtMcList[9]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_13_SHORT_CHECK_NEW, rtMcList[10]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_14_SHORT_CHECK_NEW, rtMcList[11]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_15_SHORT_CHECK_NEW, rtMcList[12]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_16_SHORT_CHECK_NEW, rtMcList[13]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_17_SHORT_CHECK_NEW, rtMcList[14]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_18_SHORT_CHECK_NEW, rtMcList[15]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_19_SHORT_CHECK_NEW, rtMcList[16]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_20_SHORT_CHECK_NEW, rtMcList[17]));
                
                string strTemp = rtMcList[0].ToString() + "," + rtMcList[1].ToString()
                     + "," + rtMcList[2].ToString() + "," + rtMcList[3].ToString()
                      + "," + rtMcList[4].ToString() + "," + rtMcList[5].ToString()
                      + "," + rtMcList[6].ToString() + "," + rtMcList[7].ToString()
                      + "," + rtMcList[8].ToString() + "," + rtMcList[9].ToString()
                      + "," + rtMcList[10].ToString() + "," + rtMcList[11].ToString()
                      + "," + rtMcList[12].ToString() + "," + rtMcList[13].ToString()
                      + "," + rtMcList[14].ToString() + "," + rtMcList[15].ToString()
                      + "," + rtMcList[16].ToString() + "," + rtMcList[17].ToString();                      


                LogState(LogType.Info, "Result : ," + strTemp + ", (MC Short Check)");

                rtMcList.Clear();
            }
            catch (Exception e)
            {
                LogState(LogType.Info, "MC Short NG : ," + e);
            }
            
            
        }
    }
}

