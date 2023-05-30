using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{

    //아직 미사용, 구조만 생성
    public class DAQChannelInfo
    {
        public int StartCellIndex { get; set; }
        public int CellCount { get; set; }

    
        public DAQChannelInfo(ModelType type)
        {
            switch (type)
            {
                case ModelType.AUDI_NORMAL: { StartCellIndex = 0; CellCount = 3; }; break;
                case ModelType.AUDI_MIRROR: { StartCellIndex = 3; CellCount = 3; }; break;
                case ModelType.PORSCHE_NORMAL: { StartCellIndex = 0; CellCount = 0; }; break;
                case ModelType.PORSCHE_MIRROR: { StartCellIndex = 0; CellCount = 0; }; break;
                case ModelType.MASERATI_NORMAL: { StartCellIndex = 0; CellCount = 0; }; break;
                //case ModelType.MASERATI_MIRROR: { StartCellIndex = 0; CellCount = 0; }; break;
                //210312 wjs add pors fl
                case ModelType.PORSCHE_FACELIFT_NORMAL: { StartCellIndex = 0; CellCount = 0; }; break;
                case ModelType.PORSCHE_FACELIFT_MIRROR: { StartCellIndex = 0; CellCount = 0; }; break;
                case ModelType.MASERATI_M183_NORMAL:    { StartCellIndex = 0; CellCount = 0; }; break;  //221101 wjs add mase m183
                case ModelType.E_UP: { StartCellIndex = 8; CellCount = 6; }; break;
                default: { StartCellIndex = 0; CellCount = 0; }; break;
            }
        }
    }
}
