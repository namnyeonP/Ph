using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Renault_BT6.모듈
{
    public class CDAQ
    {
        MainWindow mw;
        Thread daqTh;
        public bool isAlive = false;
        public SerialPort sp;
        public Dictionary<int, double> DAQList = new Dictionary<int, double>();
        Stopwatch stop = new Stopwatch();

        public DateTime localDt = new DateTime();

        /// <summary>
        ///32. 충방전 시 데이터 밀림현상으로 인해 모듈 #7 DAQ BaudRate 변경(19200 -> 115200)
        ///-> DAQ 채널이 8채널 이상인 경우 BaudRate 관련 전력전자팀과 충분한 테스트 및 검수 후
        ///채널별 BaudRate Define 필요하다고 생각됨
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="daq_PortName">COM port 설정</param>
        public CDAQ(MainWindow mw, string daq_PortName)
        {
            this.mw = mw;
            sp = new System.IO.Ports.SerialPort();
            sp.DataBits = 8;
            sp.BaudRate = 115200;
            sp.PortName = daq_PortName;
            sp.Parity = System.IO.Ports.Parity.None;
            sp.StopBits = System.IO.Ports.StopBits.One;

            for (int index = 0; index < 10; index++)
            {
                DAQList.Add(index, 0);
            }
            try
            {
                sp.Open();
                if (sp.IsOpen)
                {
                    isAlive = true;
                    sp.DiscardInBuffer();
                    sp.DataReceived += sp_DataReceived;
                    //stop.Start();
                    //StartReceive();

                    //mw.LogState(LogType.Success, sp.PortName + " - DAQ Connected");
                    //daqTh = new Thread(() =>
                    //    {
                    //        while (true)
                    //        {
                    //            sp.Write("$VOLT?");
                    //            Thread.Sleep(10);
                    //        }
                    //    });
                    //daqTh.Start();

                    //33. DAQ 연결 실패 시 예외처리 구문 추가
                    sp.WriteLine("$VOLT?");

                    mw.LogState(LogType.Info, "DAQ Connected");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
                else
                {
                    mw.LogState(LogType.Fail, "DAQ" + sp.PortName + " Open");
                    //190106 by grchoi
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "DAQ", ec);
            }
        }

        bool isReceivedFlag = false;

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Console.WriteLine("Start:"+ stw.ElapsedMilliseconds);            
            isReceivedFlag = true;
            try
            {
                int actualLength = sp.BytesToRead;

                while (actualLength < 194)//98)
                {
                    actualLength = sp.BytesToRead;
                }
                //받은 후에 데이터 파싱
                byte[] received = new byte[actualLength];
                sp.Read(received, 0, actualLength);

                //int actualLength = sp.BaseStream.EndRead(ar);
                //byte[] received = new byte[actualLength];
                //Buffer.BlockCopy(buffer, 0, received, 0, actualLength);

                if (received[0] == 0x24)
                {
                    lock (received)
                    {
                        int index = 0;
                        for (int i = 1; i < actualLength - 1; i = i + 4)
                        {
                            var data = received[i + 1] + ((((received[i + 2] << 8) & 0x000000000000ff00) + (received[i + 3])) * 0.0001);
                            if (received[i] != 0x00)
                            {
                                data = data * -1;
                            }
                            if (data > 4)
                            {
                            }

                            if (!DAQList.ContainsKey(index))
                            {
                                DAQList.Add(index, data);
                            }
                            else
                            {
                                DAQList[index] = data;
                            }
                            index++;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Fail, "DAQ - " + ex);
            }
            finally
            {
                isReceivedFlag = false;

                //Console.WriteLine(
                //    DAQList[12].ToString() + "/" +
                //    DAQList[13].ToString() + "/" +
                //    DAQList[14].ToString() + "/" +
                //    DAQList[15].ToString() + "/" +
                //    DAQList[16].ToString() + "/" +
                //    DAQList[17].ToString() + "/" +
                //    DAQList[18].ToString() + "/" +
                //    DAQList[19].ToString() + "-" + stw.ElapsedMilliseconds);

                //sp.DiscardInBuffer();
                //sp.DiscardOutBuffer();

                //34. DAQ 데이터 수신함수 내 연결상태 체크 UI 변경 구문 추가
                //    -> 박진호 선임 요청 사항으로 차후 관련 항목 모두 수시로 체크 및 UI 변경되도록
                //    본사에 요청하겠다 함
                //190104 by grchoi
                Thread.Sleep(40);
                localDt = DateTime.Now;
                //190108 by grchoi
                try
                {
                    sp.Write("$VOLT?");
                    //Thread.Sleep(10);
                    //DAQList.Clear();
                }
                catch
                {
                    mw.LogState(LogType.Fail, "DAQ" + sp.PortName + " Open");
                    //190106 by grchoi
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
            }
        }

        /// <summary>
        /// start receive
        /// 재귀형태로 반복됨
        /// </summary>
        private void StartReceive()
        {
            byte[] buffer = new byte[4096];
            Action kfR = null;

            kfR = delegate
            {
                sp.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate(IAsyncResult ar)
                {
                    try
                    {
                        int actualLength = sp.BaseStream.EndRead(ar);
                        byte[] received = new byte[actualLength];
                        Buffer.BlockCopy(buffer, 0, received, 0, actualLength);

                        if (received[0] == 0x24)
                        {
                            lock (received)
                            {
                                int index = 0;
                                for (int i = 1; i < actualLength - 1; i = i + 4)
                                {
                                    var data = received[i + 1] + ((((received[i + 2] << 8) & 0x000000000000ff00) + (received[i + 3])) * 0.0001);
                                    if (received[i] != 0x00)
                                    {
                                        data = data * -1;
                                    }
                                    if (data > 4)
                                    {
                                    }

                                    if (!DAQList.ContainsKey(index))
                                    {
                                        DAQList.Add(index, data);
                                    }
                                    else
                                    {
                                        DAQList[index] = data;
                                    }
                                    index++;

                                }
                            }
                            localDt = DateTime.Now;
                        }

                    }
                    catch (Exception)
                    {
                    }

                    if (sp.IsOpen)
                    {
                        Thread.Sleep(10);
                        Console.WriteLine(DAQList[0] + "/" + DAQList[1] + "/" + DAQList[2] + stop.Elapsed);

                        if (!isAlive)
                            return;

                        kfR();
                    }
                }, null);
            };
            kfR();

        }

        void Dispose()
        {
            isAlive = false;
            sp.Dispose();
        }
    }
}
