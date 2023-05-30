using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.클래스
{
    public class CanMsg
    {
        public uint id = 0;
        public ushort length = 0;
        public byte[] data;
        public CanMsg()
        {
            data = new byte[8];
        }
    }
}
