using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
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

        int ByteLength = 98;
        int Baudrate = 115200;

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
            if (daq_PortName == "")
                return;

            sp = new System.IO.Ports.SerialPort();
            sp.DataBits = 8;
            sp.BaudRate = Baudrate;
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
                    var nt = new Thread(()=>
                    {
                        Thread.Sleep(2000);
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            for (int i = 0; i < DAQList.Count; i++)
                            {
                                if (DAQList[i] != 0)
                                {
                                    mw.LogState(LogType.DEVICE_CHECK, "DAQ - " + daq_PortName + " Open Success");
                                    mw.contBt_daq.Background = System.Windows.Media.Brushes.Green;
                                    break;
                                }
                                else
                                {
                                    mw.LogState(LogType.DEVICE_CHECK, "DAQ - " + daq_PortName + " Open Fail");
                                    mw.contBt_daq.Background = System.Windows.Media.Brushes.Red;
                                    break;
                                }
                            }
                        }));
                    });
                    nt.Start();

                }
                else
                {
                    mw.LogState(LogType.DEVICE_CHECK, "DAQ - " + daq_PortName + " Open Fail");
                    //190106 by grchoi
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_daq.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.DEVICE_CHECK, "DAQ Exception", ec);
            }
        }

        public void _Dispose()
        {
            if(sp != null)
            {
                if(sp.IsOpen)
                {
                    sp.Close();
                }
                sp.Dispose();
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

                while (actualLength < ByteLength)
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
