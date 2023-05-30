using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace EOL_BASE.클래스
{

    //아직 미사용, 구조만 생성
    public class DAQChannelInfo
    {
        public int StartCellIndex { get; set; }
        public string ContactCH { get; set; }
        public string Plus_IVCH { get; set; }
        public string Minus_IVCH { get; set; }
        public string ModuleCH { get; set; }
        public string ModuleRes { get; set; }
        public string String_IVCH { get; set; }
        public string String_ContactCH { get; set; }
        public int ContactCount { get; set; }
        //221117 MC 저항 nnkim
        public string ModuleResMCCH { get; set; }
        public int ModuleResMCCnt { get; set; }

        public string strMachineModelType = "";
        public int CellCount { get; set; }

        private void LoadMachineModelType()
        {
            //크로마 모델 타입 
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var regMCResistanceDelay = rk.GetValue("LINE_FLAG(Old:0, New:1)") as string;

            if (regMCResistanceDelay == null) { rk.SetValue("LINE_FLAG(Old:0, New:1)", "0"); strMachineModelType = "0"; }
            else { strMachineModelType = regMCResistanceDelay; }
        }

        /// <summary>
        /// / 727 NEW 라인 NNKIM DAQ 채널 220926
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="type"></param>
        #region     // 727 NEW 라인 NNKIM DAQ 채널 220926
        public DAQChannelInfo(int type)
        {
            LoadMachineModelType();

            if (strMachineModelType == "1")
            {
                switch (type)
                {
                    case 0: //ModelType.C727_4P8S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "219";
                            Plus_IVCH = "104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,Cooling pin16";
                            String_ContactCH = "line to line";
                            ContactCount = 1;
                            CellCount = 8;
                            ModuleResMCCH = "119,308,312,313,309";
                            ModuleResMCCnt = 0;
                        }; break;
                    case 1: //ModelType.C727_4P8S_REVERSE:
                        {
                            StartCellIndex = 0;
                            ContactCH = "219";
                            Plus_IVCH = "204,220,219,218,217,216,215,214,213,212,211,210,209,208,207,206,205";
                            Minus_IVCH = "104,120,119,118,117,116,115,114,113,112,111,110,109,108,107,106,105";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,Cooling pin16";
                            String_ContactCH = "line to line";
                            ContactCount = 1;
                            CellCount = 8;
                            ModuleResMCCH = "119,308,314,315,310";
                            ModuleResMCCnt = 1;
                        }; break;
                    case 2: //ModelType.C727_4P7S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "219";
                            Plus_IVCH = "304,305,306,307,308,309,310,311,312,313,314,315,316,317,318";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216,217,218";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14";
                            String_ContactCH = "line to line";
                            ContactCount = 1;
                            CellCount = 7;
                            ModuleResMCCH = "119,308,316,317,311";
                            ModuleResMCCnt = 2;
                        }; break;
                    case 3: //ModelType.C727_3P8S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "219";
                            Plus_IVCH = "104,105,106,107,108,109,110,111,112,113,114,115,116";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12";
                            String_ContactCH = "line to line";
                            ContactCount = 1;
                            CellCount = 8;
                            ModuleResMCCH = "119,308,312,313,309";
                            ModuleResMCCnt = 0;
                        }; break;
                    case 4: //ModelType.C727_3P10S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "219";
                            Plus_IVCH = "104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15";
                            String_ContactCH = "line to line";
                            ContactCount = 1;
                            CellCount = 10;
                            ModuleResMCCH = "119,308,312,313,309";
                            ModuleResMCCnt = 0;
                        }; break;
                    case 5: //ModelType.C727_3P10S_REVERSE:
                        {
                            StartCellIndex = 0;
                            ContactCH = "219";
                            Plus_IVCH = "204,219,218,217,216,215,214,213,212,211,210,209,208,207,206,205";
                            Minus_IVCH = "104,119,118,117,116,115,114,113,112,111,110,109,108,107,106,105";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15";
                            String_ContactCH = "line to line";
                            ContactCount = 1;
                            CellCount = 10;
                            ModuleResMCCH = "119,308,314,315,310";
                            ModuleResMCCnt = 1;
                        }; break;
                    default:
                        {
                            StartCellIndex = 0;
                            ContactCH = "";
                            Plus_IVCH = "";
                        }; break;
                }                
            }
            else
            {
                switch (type)
                {
                    case 0: //ModelType.C727_4P8S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "101,102,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,201";
                            Plus_IVCH = "104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,Cooling pin16";
                            String_ContactCH = "AA,BB,End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,Cooling pin16,line to line";
                            ContactCount = 20;
                            CellCount = 8;
                            ModuleResMCCH = "119,308,312,313,309";
                            ModuleResMCCnt = 0;
                        }; break;
                    case 1: //ModelType.C727_4P8S_REVERSE:
                        {
                            StartCellIndex = 0;
                            ContactCH = "101,102,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,201";
                            Plus_IVCH = "204,220,219,218,217,216,215,214,213,212,211,210,209,208,207,206,205";
                            Minus_IVCH = "104,120,119,118,117,116,115,114,113,112,111,110,109,108,107,106,105";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,Cooling pin16";
                            String_ContactCH = "AA,BB,End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,Cooling pin16,line to line";
                            ContactCount = 20;
                            CellCount = 8;
                            ModuleResMCCH = "111,112,314,315,310";
                            ModuleResMCCnt = 1;
                        }; break;
                    case 2: //ModelType.C727_4P7S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "103,102,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,201";
                            Plus_IVCH = "304,305,306,307,308,309,310,311,312,313,314,315,316,317,318";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216,217,218";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14";
                            String_ContactCH = "CC,BB,End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,line to line";
                            ContactCount = 18;
                            CellCount = 7;
                            ModuleResMCCH = "111,112,316,317,311";
                            ModuleResMCCnt = 2;
                        }; break;
                    case 3: //ModelType.C727_3P8S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "101,102,104,105,106,107,108,109,110,111,112,113,114,115,116,201";
                            Plus_IVCH = "104,105,106,107,108,109,110,111,112,113,114,115,116";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12";
                            String_ContactCH = "AA,BB,End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,line to line";
                            ContactCount = 16;
                            CellCount = 8;
                            ModuleResMCCH = "119,308,312,313,309";
                            ModuleResMCCnt = 0;
                        }; break;
                    case 4: //ModelType.C727_3P10S:
                        {
                            StartCellIndex = 0;
                            ContactCH = "101,102,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,201";
                            Plus_IVCH = "104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119";
                            Minus_IVCH = "204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15";
                            String_ContactCH = "AA,BB,End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,line to line";
                            ContactCount = 19;
                            CellCount = 10;
                            ModuleResMCCH = "119,308,312,313,309";
                            ModuleResMCCnt = 0;
                        }; break;
                    case 5: //ModelType.C727_3P10S_REVERSE:
                        {
                            StartCellIndex = 0;
                            ContactCH = "101,102,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,201";
                            Plus_IVCH = "204,219,218,217,216,215,214,213,212,211,210,209,208,207,206,205";
                            Minus_IVCH = "104,119,118,117,116,115,114,113,112,111,110,109,108,107,106,105";
                            String_IVCH = "End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15";
                            String_ContactCH = "AA,BB,End Plate,Cooling pin1,Cooling pin2,Cooling pin3,Cooling pin4,Cooling pin5,Cooling pin6,Cooling pin7,Cooling pin8,Cooling pin9,Cooling pin10,Cooling pin11,Cooling pin12,Cooling pin13,Cooling pin14,Cooling pin15,line to line";
                            ContactCount = 19;
                            CellCount = 10;
                            ModuleResMCCH = "111,112,314,315,310";
                            ModuleResMCCnt = 1;
                        }; break;
                    default:
                        {
                            StartCellIndex = 0;
                            ContactCH = "";
                            Plus_IVCH = "";
                        }; break;
                }
            }



        }
        #endregion
    }
}
