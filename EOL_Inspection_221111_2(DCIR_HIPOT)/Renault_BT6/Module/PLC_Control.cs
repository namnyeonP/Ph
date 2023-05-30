using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PhoenixonLibrary.ETC;

namespace Renault_BT6.Module
{
    public enum ReadModes { Read_Bit, Read_LBit, Read_Word, Read_String, Read_Dec, Read_WDec };
    public enum WriteModes { Write_Bit, Write_LBit, Write_Word, Write_DWord, Write_String };


    #region PLC Write Command
    public static class PLC_WriteCMD
    {
        // Bt6 Hipot #1
        public static ushort AutoMode;
        public static ushort Testing;
        public static ushort TestOK;
        public static ushort TestNG;
        public static ushort TestStop;
        public static ushort TestReady;

        public static ushort LifeSignal;

        public static void Initialize(int channel)
        {
            ushort offset = (ushort)((channel - 1) * 0x20);

            AutoMode = (ushort)(0x0804A + offset);
            Testing = (ushort)(0x08050 + offset);

            TestOK = (ushort)(0x08052 + offset);
            TestNG = (ushort)(0x08053 + offset);

            TestStop = (ushort)(0x08054 + offset);
            TestReady = (ushort)(0x0805E + offset);

            LifeSignal = (ushort)(0x0805F + offset);
        }
    }
    #endregion

    #region PLC Read Command
    public static class PLC_ReadCMD
    {
        // Bt6 Hipot #1
        public static ushort TestStart;
        public static ushort ResultRecvOk;
        public static ushort Pause;
        public static ushort PauseContinue;
        public static ushort TestStop;
        public static ushort LifeSignal;

        public static ushort Barcode;

        public static void Initialize(int channel)
        {
            ushort offset = (ushort)((channel - 1) * 0x20);

            TestStart = (ushort)(0x08040 + offset);
            ResultRecvOk = (ushort)(0x08042 + offset);

            Pause = (ushort)(0x08044 + offset);
            PauseContinue = (ushort)(0x08045 + offset);
            TestStop = (ushort)(0x08046 + offset);

            LifeSignal = (ushort)(0x0805F + offset);

            Barcode = (ushort)(0x08040 + offset);
        }
    }
    #endregion

    public class PLC_Control 
    {
        private Melsec_PLC _PLC { get; set; }
        private Thread _MonitorThread { get; set; }
        private bool _isMonitor { get; set; }

        private bool _isLifeSignal { get; set; }

        public bool Connected
        {
            get
            {
                if (_PLC == null)
                    return false;

                return _PLC.IsAlive;
            }
        }

        private static object monitorLock = new object();
        private List<PLCMonitor> _Monitorlist = new List<PLCMonitor>();
        public List<PLCMonitor> Monitorlist
        {
            get { lock (monitorLock) return _Monitorlist; }
            set { lock (monitorLock) _Monitorlist = value; }
        }

        /// <summary>
        /// 값 변경 이벤트
        /// </summary>
        public event EventHandler PLC_MonitorEvent;

        #region PLC 상태 변수
        public PLCMonitor AutoMode { get { return Monitorlist[0]; } }
        #endregion

        private PLCMonitor _OldData { get; set; }

        public PLC_Control(int ch = 0)
        {
            _isLifeSignal = false;

            int channel = ch;

            PLC_ReadCMD.Initialize(channel);
            PLC_WriteCMD.Initialize(channel);

            MonitorSetting();
        }

        public int InitialSetting(string sLogicalStationNumber)
        {
            int ret = -1;
            try
            {
                _PLC = new Melsec_PLC(sLogicalStationNumber);
                ret = _PLC.IsAlive == true ? 0 : -1;

                if(_PLC.IsAlive)
                {
                    MonitorRead();

                    MonitorThStart();
                }

                return ret;
            }
            catch (Exception ex)
            {
                ret = -1;
            }

            return ret;
        }

        public void Close()
        {
            try
            {
                _PLC.Close();
            }
            catch
            {

            }
        }

        /// <summary>+
        /// 
        /// 주기적으로 감시
        /// </summary>
        public void MonitorThStart()
        {
            try
            {
                _isMonitor = true;
                _MonitorThread = new Thread(new ThreadStart(MonitorTh));
                _MonitorThread.Name = "PLC Monitor";
                _MonitorThread.IsBackground = true;
                _MonitorThread.Start();
            }
            catch
            {

            }
        }

        private void MonitorRead()
        {
            try
            {
                if (_PLC.IsAlive == false)
                    return;

                int hexSize = 16;
                int labelSize = 4;
                int startAddress = Convert.ToInt32(Monitorlist[0].address, 16);
                int[] lplData = new int[4];

                // 0 아니면 연결 상태 중지.
                if (_PLC.ReadDeviceBlock("B", startAddress, labelSize, out lplData) != 0)
                {
                    _PLC.IsAlive = false;
                    return;
                }

                _PLC.IsAlive = true;

                for (int i = 0; i < lplData.Length; i++)
                {
                    var plcBitData = new BitArray(BitConverter.GetBytes(lplData[i]));

                    for (int j = 0; j < hexSize; j++)
                    {
                        _OldData = Monitorlist[((j + 1) + (i * 16)) - 1];

                        // 기존 plc bit랑 다를 경우 Event 발생
                        bool newBit = plcBitData[j];
                        if (_OldData.value != newBit)
                        {
                            _OldData.value = newBit;

                            if (PLC_MonitorEvent != null)
                                PLC_MonitorEvent(_OldData, new EventArgs());
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private void MonitorTh()
        {
            try
            {
                while (_isMonitor)
                {

                    if (_PLC.IsAlive == false)
                    {
                        //FrmMain.Alarm.AlarmSet("PLC ALARM", "1001", true, true);

                        //PLCDisconnected();
                        _isMonitor = false;
                        break;
                    }

                    MonitorRead();

                    //CPAlarmRead();
                    //AlarmRead();
                    LifeSignal();

                    Thread.Sleep(100);
                }


                if (_MonitorThread != null)
                    _MonitorThread = null;
            }
            catch
            {

            }
        }
        private void MonitorSetting()
        {
            try
            {
                // Read Final address
                //ushort PLCaddresMax = 0x3F; 
                ushort addresMaxSize = 0x20;

                // PLC Read start address
                int laddress = PLC_ReadCMD.TestStart;  

                for (int i = 0; i <= addresMaxSize; i++)
                {
                    PLCMonitor mo = new PLCMonitor(PLCMonitor.PLCType.Bit, laddress)
                    {
                        ReadLength = 1
                    };

                    Monitorlist.Add(mo);
                    laddress++;
                }
            }
            catch
            {

            }
        }

        private byte[] Getbytes(string binary)
        {
            var list = new List<byte>();

            try
            {
                for (int i = 0; i < binary.Length; i = i + 8)
                {
                    string t = binary.Substring(i, 8);
                    list.Add(Convert.ToByte(t, 2));
                }
            }
            catch
            {

            }

            return new byte[] { list[1], list[0] };
        }


        public string BarcodeRequest()
        {
            string barcode = "";
            int addr = 0;
            int[] lplData = new int[4];

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    addr = PLC_ReadCMD.Barcode + j + (i * 16);
                    _PLC.ReadDeviceBlock("W", addr, 1, out lplData);

                    var plcRecv = Convert.ToString(lplData[0], 2);
                    int size = plcRecv.Length;

                    while (size < 16)
                    {
                        plcRecv = "0" + plcRecv;
                        size++;
                    }

                    var data = this.Getbytes(plcRecv);
                    barcode += Encoding.ASCII.GetString(data);
                }
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                barcode = barcode.Replace("\0", string.Empty);
                barcode = barcode.Replace("\r", string.Empty);
            }

            return barcode;
        }
        public int Read(ReadModes readMode, int address, int readlength, out string reply)
        {
            reply = "0";

            try
            {

            }
            catch
            {

            }
            return 0;
        }


        private int PLC_Write(WriteModes Command, ushort scmd, bool isOnOff)
        {
            int ret = 0;

            try
            {
                string szType = string.Empty;
                int writedata = isOnOff ? 1 : 0;

                if (Command == WriteModes.Write_Bit)
                {
                    szType = "B";
                }

                if (_PLC.IsAlive == false)
                    return -1;

                ret = _PLC.WriteDeviceBlock(szType, scmd, writedata);
            }
            catch
            {
                ret = -1;
            }

            return ret;
        }



        private void LifeSignal()
        {
            try
            {
                _isLifeSignal = !_isLifeSignal;
                PLC_Write(WriteModes.Write_Bit, PLC_WriteCMD.AutoMode, _isLifeSignal);
            }
            catch
            {

            }
        }


        public int SetAutoStart(bool isOn)
        {
            int ret = 0;

            // 매뉴얼 모드를 끔
            ret = PLC_Write(WriteModes.Write_Bit, PLC_WriteCMD.TestReady, isOn);


            return ret;
        }

    

        public int SetAutoStop(bool isOn)
        {
            int ret = 0;
            string szType = "B";
            int writedata = isOn ? 1 : 0;
            int off = 0;

            if (isOn)
            {
                // Auto Start를 끔
               // ret = _PLC.WriteDeviceBlock(szType, CWriteCmd.AutoStart, 1, ref off);
                //ret = _PLC.WriteDeviceBlock(szType, CReadCmd.StartStop, 1, ref off);
            }

            //ret = _PLC.WriteDeviceBlock(szType, CWriteCmd.AutoStop, 1, ref writedata);

            return ret;
        }
      
    }// end class

    #region PLCMonitor
    public class PLCMonitor
    {
        public enum PLCType { Bit, Word, Stringm, LBit }
        public string type { get; set; }
        public string address { get; set; }
        public bool value { get; set; }

        public int ReadLength { get; set; }

        public PLCMonitor(PLCType type, string address)
        {
            this.type = type.ToString();
            this.address = address;
            this.value = false;
        }

        public PLCMonitor(PLCType type, int address)
        {
            this.type = type.ToString();
            this.address = address.ToString("X6");
            this.value = false;
        }

        public PLCMonitor(PLCType type, int address, string format)
        {
            this.type = type.ToString();
            this.address = address.ToString(format);
            this.value = false;
        }
    }
    #endregion
}
