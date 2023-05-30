using PhoenixonLibrary.ETC;
using System;
using System.Collections.Generic;

namespace Renault_BT6
{
    public class RelayControl
    {
        private DASK_Relay _relay { get; set; }

        private Dictionary<string, string> _relayDict = new Dictionary<string, string>();

        public bool Alive { get { return _relay.IsAlive; } }

        public RelayControl()
        {
            try
            {
                _relay = new DASK_Relay();
                
                InitDcit();
            }
            catch
            {

            }
        }


        public void Open()
        {
            try
            {
                _relay.OpenDIO();
            }
            catch
            {

            }
        }

        public void Close()
        {
            try
            {
                _relay.CloseDIO();
            }
            catch
            {

            }
        }

        private void InitDcit()
        {
            try
            {
                _relayDict.Add(RELAY.LIGHT_BLUE, "LIGHT_BLUE");
                _relayDict.Add(RELAY.LIGHT_GREEN, "LIGHT_GREEN");
                _relayDict.Add(RELAY.LIGHT_RED, "LIGHT_RED");
                _relayDict.Add(RELAY.LIGHT_YELLOW, "LIGHT_YELLOW");
            }
            catch
            {

            }
        }


        public void ReadRelayStatus()
        {
            _relay.ReadRelayStatus();
        }
        public bool GetRelayStatus(int index)
        {
            return _relay.GetRelayStatus(index);
        }

        public void RelayOff()
        {
            try
            {
                Off(RELAY.LIGHT_RED);
                Off(RELAY.LIGHT_GREEN);
                Off(RELAY.LIGHT_BLUE);
                Off(RELAY.LIGHT_YELLOW);
            }
            catch
            {

            }
        }

        public void On(string number)
        {
            try
            {
                if (_relay.IsAlive == false)
                    return;

                _relay.On(number);

                Device.Cycler.LogWrite(string.Format("Relay On - {0},{1}", number, _relayDict[number]));
            }
            catch
            {

            }
        }

        public void Off(string number)
        {
            try
            {
                if (_relay.IsAlive == false)
                    return;

                _relay.Off(number);

                /*
                string relay_name = "";

                if (_relayDict.ContainsKey(number))
                {
                    _relayDict.TryGetValue(number, out relay_name);
                }
                */
                Device.Cycler.LogWrite(string.Format("Relay OFF - {0},{1}", number, _relayDict[number]));
            }
            catch
            {

            }
        }


        public bool GetRelayStatus(string number)
        {
            bool ret = false;
            try
            {
                int index = Convert.ToInt16(number);

                ret  = _relay.GetRelayStatus(index);
            }
            catch
            {

            }
            return ret;
        }

        public void AllOff()
        {
            try
            {
                if (_relay.IsAlive == false)
                    return;

                _relay.AllOnOff(false);

                Device.Cycler.LogWrite("Relay All Off");

            }
            catch
            {

            }
        }

    }
}
