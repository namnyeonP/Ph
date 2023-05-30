using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class DMMChannelInfo
    {
        public string CellCH { get; set; }
        public string ModuleCH { get; set; }
        public string ModuleRes1 { get; set; }
        public string ModuleRes2 { get; set; }
        public string ModuleRes { get; set; }
        public string ModuleRes_LV { get; set; }

        public int ChannelCount { get; set; }
        //200710 변수 추가
        public int CellCount { get; set; }

        //221117 MC 저항 nnkim
        public string ModuleResMCCH { get; set; }


        //200804 구조 일부 변경
        public DMMChannelInfo(ModelType type)
        {
            switch (type)
            {
                case ModelType.AUDI_NORMAL:
                    {
                        ChannelCount = 3;
                        CellCH = "101,102,103";
                        ModuleCH = "104";
                        ModuleRes1 = "105";
                        ModuleRes2 = "106";
                        ModuleRes = "105,106";
                        CellCount = 3;
                        ModuleResMCCH = "";
                    }; break;
                case ModelType.AUDI_MIRROR:
                    {
                        ChannelCount = 3;
                        CellCH = "111,112,113";
                        ModuleCH = "114";
                        ModuleRes1 = "115";
                        ModuleRes2 = "116";
                        ModuleRes = "115,116";
                        CellCount = 3;
                        ModuleResMCCH = "";
                    }; break;
                case ModelType.PORSCHE_NORMAL:
                    {
                        ChannelCount = 6;
                        CellCH = "";
                        ModuleCH = "109";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 6;
                        ModuleResMCCH = "111,112,113,114";
                    }; break;
                case ModelType.PORSCHE_MIRROR:
                    {
                        ChannelCount = 6;
                        CellCH = "";
                        ModuleCH = "110";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 6;
                        ModuleResMCCH = "111,112,115,116";
                    }; break;
                    //210111 wjs modify maserati
                case ModelType.MASERATI_NORMAL:
                    {
                        ChannelCount = 6;
                        CellCH = "";
                        ModuleCH = "109";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 6;
                        ModuleResMCCH = "111,112,113,114";
                    }; break;

                    //210312 wjs add pors fl
                case ModelType.PORSCHE_FACELIFT_NORMAL:
                    {
                        ChannelCount = 6;
                        CellCH = "";
                        ModuleCH = "107";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 6;
                        ModuleResMCCH = "111,112,117,118";
                    }; break;
                case ModelType.PORSCHE_FACELIFT_MIRROR:
                    {
                        ChannelCount = 6;
                        CellCH = "";
                        ModuleCH = "108";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 6;
                        ModuleResMCCH = "111,112,119,120";
                    }; break;
                //221101 wjs add mase m183
                case ModelType.MASERATI_M183_NORMAL:
                    {
                        ChannelCount = 6;
                        CellCH = "";
                        ModuleCH = "107";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 6;
                        ModuleResMCCH = "111,112,117,118";
                    }; break;
                case ModelType.E_UP:
                    {
                        ChannelCount = 6;
                        CellCH = "201,202,203,204,205,206";
                        ModuleCH = "207";
                        ModuleRes1 = "208";
                        ModuleRes2 = "209";
                        ModuleRes = "208,209,210";
                        ModuleRes_LV = "208,209";
                        CellCount = 6;
                        ModuleResMCCH = "301,302,303,304";
                    }; break;
                default:
                    {
                        ChannelCount = 0;
                        CellCH = "";
                        ModuleCH = "";
                        ModuleRes1 = "";
                        ModuleRes2 = "";
                        ModuleRes = "";
                        CellCount = 0;
                        ModuleResMCCH = "";
                    }; break;
            }
        }
    }
}
