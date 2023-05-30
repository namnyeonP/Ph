using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{
    public class CLambdaPower
    {
        MainWindow mw;
        public bool isAlive = false;
        SerialPort sp;
        string portName = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="portname">COM3</param>
        /// <param name="setVolt">13.9000</param>
        /// <param name="setCurrent">5.0000</param>
        public CLambdaPower(MainWindow mw, string portname,string setVolt,string setCurrent)
        {
            this.mw = mw;
            portName = portname;
            sp = new SerialPort();
            sp.DataBits = 8;
            sp.BaudRate = 19200;
            sp.PortName = portName;
            sp.Parity = System.IO.Ports.Parity.None;
            sp.StopBits = System.IO.Ports.StopBits.One;

            try
            {
                sp.Open();
                if (sp.IsOpen)
                {
                    //crashPower.DiscardInBuffer();
                    mw.LogState(LogType.Success, sp.PortName + ", Power");
                    sp.DataReceived += sp_DataReceived;

                    isAlive = true;

                    ////명령어
                    Set_Initialize(6);
                    //Set_VOLT("13.9000");
                    Set_VOLT(setVolt);
                    //Set_CURR("5.0000");
                    Set_CURR(setCurrent);

                    Set_OUTP_Off();


                }
                else
                    mw.LogState(LogType.Fail, "Power" + sp.PortName + " Open");
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Power", ec);
                
            }       
        }

        public string rtString = "";
        Object becmObj = new Object();
        void sp_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            lock (becmObj)
            {
                SerialPort sp = sender as SerialPort;
                var rcvbytes = new byte[sp.BytesToRead];
                sp.Read(rcvbytes, 0, sp.BytesToRead);
                rtString = Encoding.ASCII.GetString(rcvbytes);
                Thread.Sleep(1);
                if (sp.PortName == mw.power_PortName1)
                {
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_power1.Background = System.Windows.Media.Brushes.Green;
                    }));
                }                
                mw.LogState(LogType.Info, portName + " - Recv : " + rtString.Replace("\r\n", ""));
            }
        }

        public void Dispose()
        {
            sp.Dispose();
        }

        public bool Set_OUTP_On()
        {
            var str = "OUTP:STAT 1";
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            str = "OUTP?";
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            return true;
        }

        public bool Set_OUTP_Off()
        {
            var str = "OUTP:STAT 0";
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            str = "OUTP?";
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            return true;
        }


        /// <summary>
        /// Set voltage 
        /// </summary>
        /// <param name="volt">12.0000</param>
        /// <returns></returns>
        public bool Set_VOLT(string volt)
        {
            var str = ":VOLT "+volt;
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            str = "VOLT?";
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selNo">계측기에서 SEL Number를 본 파라미터와 동일하게 세팅해야 제어됨</param>
        /// <returns></returns>
        public bool Set_Initialize(int selNo)
        {
            var str = "INST:NSEL " + selNo.ToString();
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            return true;
        }

        /// <summary>
        /// Set current 
        /// </summary>
        /// <param name="cur">5.0000</param>
        /// <returns></returns>
        public bool Set_CURR(string cur)
        {
            var str = ":CURR " + cur;
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            str = "CURR?";
            sp.WriteLine(str);
            mw.LogState(LogType.Info, portName + ":" + str);
            Thread.Sleep(300);
            return true;
        }

        public string Read_Version()
        {
            string cmd = "";
            string rtstring = "";
            sp.DiscardInBuffer();
            cmd = "*IDN?";
            mw.LogState(LogType.Info, "send data : " + cmd);
            sp.WriteLine(cmd);
            Thread.Sleep(300);
            int comTimeout = 0;
            while (sp.BytesToRead < 33)
            {
                if (comTimeout++ > 10)
                {
                    //throw new Exception("Keysight com check Communication Time out");
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    sp.DiscardOutBuffer();
                    return "";
                }
                Thread.Sleep(100);
            }
            rtstring = sp.ReadLine();
            mw.LogState(LogType.Info, "read data : " + rtstring);
            var p = rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            sp.DiscardOutBuffer();
            return rtstring;
        }


        int tryCnt = 20;
        int WriteNSleepTime = 500;
        /// <summary>
        /// 다음과 같은 명령어를 쓴다.
        /// 자리수는 맞춰야 한다.
        /// SetPowerToStr("VOL12.000");
        /// SetPowerToStr("CUR02.900");
        /// SetPowerToStr("OUT0");
        /// </summary>
        /// <param name="p"></param>
        public void SetPowerToStr(string p)
        {
            try
            {
                if (!sp.IsOpen)
                {
                    sp = null;

                    sp.DataBits = 8;
                    sp.BaudRate = 19200;
                    sp.PortName = portName;
                    sp.Parity = System.IO.Ports.Parity.None;
                    sp.StopBits = System.IO.Ports.StopBits.One;
                    try
                    {
                        sp.Open();
                        if (sp.IsOpen)
                        {
                            mw.LogState(LogType.Info, sp.PortName + " - Power Re opened");
                            sp.DataReceived += sp_DataReceived;

                            isAlive = true;
                        }
                        else
                            mw.LogState(LogType.Fail, sp.PortName + " - Power Re opened");
                    }
                    catch (Exception ec)
                    {
                        mw.LogState(LogType.Fail, "Power", ec);
                        isAlive = false;

                    }   
                }

                if (p == "OUT0")
                {
                    var str = ":ADR01;:" + p + ";";
                    sp.Write(str); Thread.Sleep(300);
                    sp.Write(str); Thread.Sleep(300);
                    sp.Write(str); Thread.Sleep(300);

                    str = ":ADR01;:OUT?;";
                    sp.Write(str);
                    Thread.Sleep(300);
                    mw.LogState(LogType.Info, sp.PortName + " - Power Output OFF");
                    rtString = "";
                }
                else if (p == "OUT1")
                {
                    var str = ":ADR01;:" + p + ";";
                    sp.Write(str);Thread.Sleep(300);
                    sp.Write(str);Thread.Sleep(300);
                    sp.Write(str);Thread.Sleep(300);

                    str = ":ADR01;:OUT?;";
                    sp.Write(str);
                    Thread.Sleep(300);

                    mw.LogState(LogType.Info, sp.PortName + " - Power Output ON");
                    rtString = "";
                }
                else if (p.Contains("VOL"))
                {
                    var originVolt = double.Parse(p.Replace("VOL", ""));

                    if (originVolt < 10)
                    {
                        p = "VOL0" + p.Replace("VOL", "");
                    }
                    if (p.Replace("VOL", "").Length == 5)
                    {
                        p = "VOL" + p.Replace("VOL", "") + "0";
                    }
                    var str = ":ADR01;:" + p + ";";
                    sp.Write(str); Thread.Sleep(300);
                    sp.Write(str); Thread.Sleep(300);
                    sp.Write(str); Thread.Sleep(300);

                    str = ":ADR01;:VOL!;";
                    sp.Write(str);
                    Thread.Sleep(300);
                }
                else if (p.Contains("CUR"))
                {
                    var str = ":ADR01;:" + p + ";";
                    sp.Write(str);
                    Thread.Sleep(300);

                    str = ":ADR01;:CUR!;";
                    sp.Write(str);
                    Thread.Sleep(300);

                    mw.LogState(LogType.Info, sp.PortName + " - Power SET Current : " + rtString.Replace("\r\n", ""));
                }
                else
                {
                    mw.LogState(LogType.Fail, sp.PortName + " - Power NOT COMMANDS!");
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "SET_POWER", ec);

            }

        }
    }
}
