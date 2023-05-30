using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.클래스
{
    public class CellDetail : INotifyPropertyChanged
    {
        public CellDetail()
        {
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string testName;

        public string TestName
        {
            get { return testName; }
            set { testName = value; OnPropertyChange("TestName"); }
        }

        double moduleVolt;

        public double ModuleVolt
        {
            get { return moduleVolt; }
            set { moduleVolt = value; OnPropertyChange("ModuleVolt"); }
        }

        double cellVolt_1;

        public double CellVolt_1
        {
            get { return cellVolt_1; }
            set { cellVolt_1 = value; OnPropertyChange("CellVolt_1"); }
        }
        double cellVolt_2;

        public double CellVolt_2
        {
            get { return cellVolt_2; }
            set { cellVolt_2 = value; OnPropertyChange("CellVolt_2"); }
        }
        double cellVolt_3;

        public double CellVolt_3
        {
            get { return cellVolt_3; }
            set { cellVolt_3 = value; OnPropertyChange("CellVolt_3"); }
        }
        double cellVolt_4;

        public double CellVolt_4
        {
            get { return cellVolt_4; }
            set { cellVolt_4 = value; OnPropertyChange("CellVolt_4"); }
        }
        double cellVolt_5;

        public double CellVolt_5
        {
            get { return cellVolt_5; }
            set { cellVolt_5 = value; OnPropertyChange("CellVolt_5"); }
        }
        double cellVolt_6;

        public double CellVolt_6
        {
            get { return cellVolt_6; }
            set { cellVolt_6 = value; OnPropertyChange("CellVolt_6"); }
        }
        double cellVolt_7;

        public double CellVolt_7
        {
            get { return cellVolt_7; }
            set { cellVolt_7 = value; OnPropertyChange("CellVolt_7"); }
        }
        double cellVolt_8;

        public double CellVolt_8
        {
            get { return cellVolt_8; }
            set { cellVolt_8 = value; OnPropertyChange("CellVolt_8"); }
        }
        double cellVolt_9;

        public double CellVolt_9
        {
            get { return cellVolt_9; }
            set { cellVolt_9 = value; OnPropertyChange("CellVolt_9"); }
        }
        double cellVolt_10;

        public double CellVolt_10
        {
            get { return cellVolt_10; }
            set { cellVolt_10 = value; OnPropertyChange("CellVolt_10"); }
        }
        double cellVolt_11;

        public double CellVolt_11
        {
            get { return cellVolt_11; }
            set { cellVolt_11 = value; OnPropertyChange("CellVolt_11"); }
        }
        double cellVolt_12;

        public double CellVolt_12
        {
            get { return cellVolt_12; }
            set { cellVolt_12 = value; OnPropertyChange("CellVolt_12"); }
        }
        double cellVolt_13;

        public double CellVolt_13
        {
            get { return cellVolt_13; }
            set { cellVolt_13 = value; OnPropertyChange("CellVolt_13"); }
        }
        double cellVolt_14;

        public double CellVolt_14
        {
            get { return cellVolt_14; }
            set { cellVolt_14 = value; OnPropertyChange("CellVolt_14"); }
        }
        double cellVolt_15;

        public double CellVolt_15
        {
            get { return cellVolt_15; }
            set { cellVolt_15 = value; OnPropertyChange("CellVolt_15"); }
        }
        double cellVolt_16;

        public double CellVolt_16
        {
            get { return cellVolt_16; }
            set { cellVolt_16 = value; OnPropertyChange("CellVolt_16"); }
        }
        double cellVolt_17;

        public double CellVolt_17
        {
            get { return cellVolt_17; }
            set { cellVolt_17 = value; OnPropertyChange("CellVolt_17"); }
        }
        double cellVolt_18;

        public double CellVolt_18
        {
            get { return cellVolt_18; }
            set { cellVolt_18 = value; OnPropertyChange("CellVolt_18"); }
        }
        double cellVolt_19;

        public double CellVolt_19
        {
            get { return cellVolt_19; }
            set { cellVolt_19 = value; OnPropertyChange("CellVolt_19"); }
        }
        double cellVolt_20; public double CellVolt_20 { get { return cellVolt_20; } set { cellVolt_20 = value; OnPropertyChange("CellVolt_20"); } }
        double cellVolt_21; public double CellVolt_21 { get { return cellVolt_21; } set { cellVolt_21 = value; OnPropertyChange("CellVolt_21"); } }
        double cellVolt_22; public double CellVolt_22 { get { return cellVolt_22; } set { cellVolt_22 = value; OnPropertyChange("CellVolt_22"); } }
        double cellVolt_23; public double CellVolt_23 { get { return cellVolt_23; } set { cellVolt_23 = value; OnPropertyChange("CellVolt_23"); } }
        double cellVolt_24; public double CellVolt_24 { get { return cellVolt_24; } set { cellVolt_24 = value; OnPropertyChange("CellVolt_24"); } }
        double cellVolt_25; public double CellVolt_25 { get { return cellVolt_25; } set { cellVolt_25 = value; OnPropertyChange("CellVolt_25"); } }
        double cellVolt_26; public double CellVolt_26 { get { return cellVolt_26; } set { cellVolt_26 = value; OnPropertyChange("CellVolt_26"); } }
        double cellVolt_27; public double CellVolt_27 { get { return cellVolt_27; } set { cellVolt_27 = value; OnPropertyChange("CellVolt_27"); } }
        double cellVolt_28; public double CellVolt_28 { get { return cellVolt_28; } set { cellVolt_28 = value; OnPropertyChange("CellVolt_28"); } }
        double cellVolt_29; public double CellVolt_29 { get { return cellVolt_29; } set { cellVolt_29 = value; OnPropertyChange("CellVolt_29"); } }
        double cellVolt_30; public double CellVolt_30 { get { return cellVolt_30; } set { cellVolt_30 = value; OnPropertyChange("CellVolt_30"); } }
        double cellVolt_31; public double CellVolt_31 { get { return cellVolt_31; } set { cellVolt_31 = value; OnPropertyChange("CellVolt_31"); } }
        double cellVolt_32; public double CellVolt_32 { get { return cellVolt_32; } set { cellVolt_32 = value; OnPropertyChange("CellVolt_32"); } }
        double cellVolt_33; public double CellVolt_33 { get { return cellVolt_33; } set { cellVolt_33 = value; OnPropertyChange("CellVolt_33"); } }
        double cellVolt_34; public double CellVolt_34 { get { return cellVolt_34; } set { cellVolt_34 = value; OnPropertyChange("CellVolt_34"); } }
        double cellVolt_35; public double CellVolt_35 { get { return cellVolt_35; } set { cellVolt_35 = value; OnPropertyChange("CellVolt_35"); } }
        double cellVolt_36; public double CellVolt_36 { get { return cellVolt_36; } set { cellVolt_36 = value; OnPropertyChange("CellVolt_36"); } }
        double cellVolt_37; public double CellVolt_37 { get { return cellVolt_37; } set { cellVolt_37 = value; OnPropertyChange("CellVolt_37"); } }
        double cellVolt_38; public double CellVolt_38 { get { return cellVolt_38; } set { cellVolt_38 = value; OnPropertyChange("CellVolt_38"); } }
        double cellVolt_39; public double CellVolt_39 { get { return cellVolt_39; } set { cellVolt_39 = value; OnPropertyChange("CellVolt_39"); } }
        double cellVolt_40; public double CellVolt_40 { get { return cellVolt_40; } set { cellVolt_40 = value; OnPropertyChange("CellVolt_40"); } }
        double cellVolt_41; public double CellVolt_41 { get { return cellVolt_41; } set { cellVolt_41 = value; OnPropertyChange("CellVolt_41"); } }
        double cellVolt_42; public double CellVolt_42 { get { return cellVolt_42; } set { cellVolt_42 = value; OnPropertyChange("CellVolt_42"); } }
        double cellVolt_43; public double CellVolt_43 { get { return cellVolt_43; } set { cellVolt_43 = value; OnPropertyChange("CellVolt_43"); } }
        double cellVolt_44; public double CellVolt_44 { get { return cellVolt_44; } set { cellVolt_44 = value; OnPropertyChange("CellVolt_44"); } }
        double cellVolt_45; public double CellVolt_45 { get { return cellVolt_45; } set { cellVolt_45 = value; OnPropertyChange("CellVolt_45"); } }
        double cellVolt_46; public double CellVolt_46 { get { return cellVolt_46; } set { cellVolt_46 = value; OnPropertyChange("CellVolt_46"); } }
        double cellVolt_47; public double CellVolt_47 { get { return cellVolt_47; } set { cellVolt_47 = value; OnPropertyChange("CellVolt_47"); } }
        double cellVolt_48; public double CellVolt_48 { get { return cellVolt_48; } set { cellVolt_48 = value; OnPropertyChange("CellVolt_48"); } }
        double cellVolt_49; public double CellVolt_49 { get { return cellVolt_49; } set { cellVolt_49 = value; OnPropertyChange("CellVolt_49"); } }
        double cellVolt_50; public double CellVolt_50 { get { return cellVolt_50; } set { cellVolt_50 = value; OnPropertyChange("CellVolt_50"); } }
        double cellVolt_51; public double CellVolt_51 { get { return cellVolt_51; } set { cellVolt_51 = value; OnPropertyChange("CellVolt_51"); } }
        double cellVolt_52; public double CellVolt_52 { get { return cellVolt_52; } set { cellVolt_52 = value; OnPropertyChange("CellVolt_52"); } }
        double cellVolt_53; public double CellVolt_53 { get { return cellVolt_53; } set { cellVolt_53 = value; OnPropertyChange("CellVolt_53"); } }
        double cellVolt_54; public double CellVolt_54 { get { return cellVolt_54; } set { cellVolt_54 = value; OnPropertyChange("CellVolt_54"); } }
        double cellVolt_55; public double CellVolt_55 { get { return cellVolt_55; } set { cellVolt_55 = value; OnPropertyChange("CellVolt_55"); } }
        double cellVolt_56; public double CellVolt_56 { get { return cellVolt_56; } set { cellVolt_56 = value; OnPropertyChange("CellVolt_56"); } }
        double cellVolt_57; public double CellVolt_57 { get { return cellVolt_57; } set { cellVolt_57 = value; OnPropertyChange("CellVolt_57"); } }
        double cellVolt_58; public double CellVolt_58 { get { return cellVolt_58; } set { cellVolt_58 = value; OnPropertyChange("CellVolt_58"); } }
        double cellVolt_59; public double CellVolt_59 { get { return cellVolt_59; } set { cellVolt_59 = value; OnPropertyChange("CellVolt_59"); } }
        double cellVolt_60; public double CellVolt_60 { get { return cellVolt_60; } set { cellVolt_60 = value; OnPropertyChange("CellVolt_60"); } }
        double cellVolt_61; public double CellVolt_61 { get { return cellVolt_61; } set { cellVolt_61 = value; OnPropertyChange("CellVolt_61"); } }
        double cellVolt_62; public double CellVolt_62 { get { return cellVolt_62; } set { cellVolt_62 = value; OnPropertyChange("CellVolt_62"); } }
        double cellVolt_63; public double CellVolt_63 { get { return cellVolt_63; } set { cellVolt_63 = value; OnPropertyChange("CellVolt_63"); } }
        double cellVolt_64; public double CellVolt_64 { get { return cellVolt_64; } set { cellVolt_64 = value; OnPropertyChange("CellVolt_64"); } }
        double cellVolt_65; public double CellVolt_65 { get { return cellVolt_65; } set { cellVolt_65 = value; OnPropertyChange("CellVolt_65"); } }
        double cellVolt_66; public double CellVolt_66 { get { return cellVolt_66; } set { cellVolt_66 = value; OnPropertyChange("CellVolt_66"); } }
        double cellVolt_67; public double CellVolt_67 { get { return cellVolt_67; } set { cellVolt_67 = value; OnPropertyChange("CellVolt_67"); } }
        double cellVolt_68; public double CellVolt_68 { get { return cellVolt_68; } set { cellVolt_68 = value; OnPropertyChange("CellVolt_68"); } }
        double cellVolt_69; public double CellVolt_69 { get { return cellVolt_69; } set { cellVolt_69 = value; OnPropertyChange("CellVolt_69"); } }
        double cellVolt_70; public double CellVolt_70 { get { return cellVolt_70; } set { cellVolt_70 = value; OnPropertyChange("CellVolt_70"); } }
        double cellVolt_71; public double CellVolt_71 { get { return cellVolt_71; } set { cellVolt_71 = value; OnPropertyChange("CellVolt_71"); } }
        double cellVolt_72; public double CellVolt_72 { get { return cellVolt_72; } set { cellVolt_72 = value; OnPropertyChange("CellVolt_72"); } }
        double cellVolt_73; public double CellVolt_73 { get { return cellVolt_73; } set { cellVolt_73 = value; OnPropertyChange("CellVolt_73"); } }
        double cellVolt_74; public double CellVolt_74 { get { return cellVolt_74; } set { cellVolt_74 = value; OnPropertyChange("CellVolt_74"); } }
        double cellVolt_75; public double CellVolt_75 { get { return cellVolt_75; } set { cellVolt_75 = value; OnPropertyChange("CellVolt_75"); } }
        double cellVolt_76; public double CellVolt_76 { get { return cellVolt_76; } set { cellVolt_76 = value; OnPropertyChange("CellVolt_76"); } }
        double cellVolt_77; public double CellVolt_77 { get { return cellVolt_77; } set { cellVolt_77 = value; OnPropertyChange("CellVolt_77"); } }
        double cellVolt_78; public double CellVolt_78 { get { return cellVolt_78; } set { cellVolt_78 = value; OnPropertyChange("CellVolt_78"); } }
        double cellVolt_79; public double CellVolt_79 { get { return cellVolt_79; } set { cellVolt_79 = value; OnPropertyChange("CellVolt_79"); } }
        double cellVolt_80; public double CellVolt_80 { get { return cellVolt_80; } set { cellVolt_80 = value; OnPropertyChange("CellVolt_80"); } }
        double cellVolt_81; public double CellVolt_81 { get { return cellVolt_81; } set { cellVolt_81 = value; OnPropertyChange("CellVolt_81"); } }
        double cellVolt_82; public double CellVolt_82 { get { return cellVolt_82; } set { cellVolt_82 = value; OnPropertyChange("CellVolt_82"); } }
        double cellVolt_83; public double CellVolt_83 { get { return cellVolt_83; } set { cellVolt_83 = value; OnPropertyChange("CellVolt_83"); } }
        double cellVolt_84; public double CellVolt_84 { get { return cellVolt_84; } set { cellVolt_84 = value; OnPropertyChange("CellVolt_84"); } }
        double cellVolt_85; public double CellVolt_85 { get { return cellVolt_85; } set { cellVolt_85 = value; OnPropertyChange("CellVolt_85"); } }
        double cellVolt_86; public double CellVolt_86 { get { return cellVolt_86; } set { cellVolt_86 = value; OnPropertyChange("CellVolt_86"); } }
        double cellVolt_87; public double CellVolt_87 { get { return cellVolt_87; } set { cellVolt_87 = value; OnPropertyChange("CellVolt_87"); } }
        double cellVolt_88; public double CellVolt_88 { get { return cellVolt_88; } set { cellVolt_88 = value; OnPropertyChange("CellVolt_88"); } }
        double cellVolt_89; public double CellVolt_89 { get { return cellVolt_89; } set { cellVolt_89 = value; OnPropertyChange("CellVolt_89"); } }
        double cellVolt_90; public double CellVolt_90 { get { return cellVolt_90; } set { cellVolt_90 = value; OnPropertyChange("CellVolt_90"); } }
        double cellVolt_91; public double CellVolt_91 { get { return cellVolt_91; } set { cellVolt_91 = value; OnPropertyChange("CellVolt_91"); } }
        double cellVolt_92; public double CellVolt_92 { get { return cellVolt_92; } set { cellVolt_92 = value; OnPropertyChange("CellVolt_92"); } }
        double cellVolt_93; public double CellVolt_93 { get { return cellVolt_93; } set { cellVolt_93 = value; OnPropertyChange("CellVolt_93"); } }
        double cellVolt_94; public double CellVolt_94 { get { return cellVolt_94; } set { cellVolt_94 = value; OnPropertyChange("CellVolt_94"); } }
        double cellVolt_95; public double CellVolt_95 { get { return cellVolt_95; } set { cellVolt_95 = value; OnPropertyChange("CellVolt_95"); } }
        double cellVolt_96; public double CellVolt_96 { get { return cellVolt_96; } set { cellVolt_96 = value; OnPropertyChange("CellVolt_96"); } }
                
        double temp1 ; public double Temp1 { get { return temp1; } set { temp1 = value; OnPropertyChange("Temp1"); } }
        double temp2 ; public double Temp2 { get { return temp2; } set { temp2 = value; OnPropertyChange("Temp2"); } }
        double temp3 ; public double Temp3 { get { return temp3; } set { temp3 = value; OnPropertyChange("Temp3"); } }
        double temp4 ; public double Temp4 { get { return temp4; } set { temp4 = value; OnPropertyChange("Temp4"); } }
        double temp5 ; public double Temp5 { get { return temp5; } set { temp5 = value; OnPropertyChange("Temp5"); } }
        double temp6 ; public double Temp6 { get { return temp6; } set { temp6 = value; OnPropertyChange("Temp6"); } }
        double temp7 ; public double Temp7 { get { return temp7; } set { temp7 = value; OnPropertyChange("Temp7"); } }
        double temp8 ; public double Temp8 { get { return temp8; } set { temp8 = value; OnPropertyChange("Temp8"); } }
        double temp9 ; public double Temp9 { get { return temp9; } set { temp9 = value; OnPropertyChange("Temp9"); } }
        double temp10; public double Temp10 { get { return temp10; } set { temp10 = value; OnPropertyChange("Temp10"); } }
        double temp11; public double Temp11 { get { return temp11; } set { temp11 = value; OnPropertyChange("Temp11"); } }
        double temp12; public double Temp12 { get { return temp12; } set { temp12 = value; OnPropertyChange("Temp12"); } }


    }
}
