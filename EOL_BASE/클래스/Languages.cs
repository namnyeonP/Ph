using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class Languages
    {
        public bool isEn = true;
        public Languages()
        {
        }

        public string NoData { get { if (isEn) return "No Data"; else return "데이터가 없습니다"; } }
        public string lSettingPort { get { if (isEn) return "Setting Port"; else return "포트 설정"; } }
        public string lModbusSettings { get { if (isEn) return "Modbus Settings"; else return "Modbus 설정"; } }
        public string lPCANSettings { get { if (isEn) return "PCANSettings"; else return "PCAN 설정"; } }
        public string lModbusConnect { get { if (isEn) return "Modbus Connect"; else return "Modbus 연결"; } }
        public string lCANConnect { get { if (isEn) return "CAN Connect"; else return "CAN 연결"; } }

        public string lSystemMessage_DefaultStatus { get { if (isEn) return "Please Set Port."; else return "포트를 설정해주세요."; } }
        public string lSystemMessage_NotSetPort { get { if (isEn) return "Port is Not Set."; else return "포트가 설정되지 않았습니다."; } }
        public string lSystemMessage_SettingConfirmed { get { if (isEn) return "Setting Confirmed."; else return "설정이 확인되었습니다."; } }
        public string lSystemMessage_Canceled { get { if (isEn) return "Canceled."; else return "취소되었습니다."; } }
        public string lSystemMessage_ConnectMonitoring { get { if (isEn) return "Connected Device :"; else return "장비에 연결합니다. :"; } }
        public string lSystemMessage_DisconnectMonitoring { get { if (isEn) return "Disconnected Device :"; else return "장비에 연결을 끊습니다 :"; } }
        public string lSystemMessage_ReceiveData { get { if (isEn) return "Data Received."; else return "데이터를 받았습니다."; } }

        public string lMonitoring = "Monitoring";
        public string lDataLog = "Data Log";
        public string lCAN = "CAN";
        public string lEXIT = "EXIT";
        public string lSystem = "System";
        public string lBMSTotalStatus = "BMS Total Status";
        public string lBMSModule1 = "BMS Module 1";
        public string lBMSModule2 = "BMS Module 2";
        public string lSystem_IDNUM = "ID/NUM";
        public string lSystem_Command = "Command";

        public string lSystem_OutputVol { get { if (isEn) return "AC Output Voltage"; else return "AC 전압"; } }
        public string lSystem_OutputCur { get { if (isEn) return "AC Output Current"; else return "AC 전류"; } }
        public string lSystem_OutputPow { get { if (isEn) return "AC Output Power"; else return "소비전력"; } }
        public string lSystem_BatVol { get { if (isEn) return "Battery Voltage"; else return "Battery 전압"; } }
        public string lSystem_BatCur { get { if (isEn) return "Battery Current"; else return "Battery 전력"; } }
        public string lSystem_CharPow { get { if (isEn) return "Charge Power"; else return "충전전력"; } }

        public string lSystem_DayCharge { get { if (!isEn) return "MPPT 충전 전력량 (DAY) [DSP]"; else return "MPPT Charging Power(DAY) [DSP]"; } }
        public string lSystem_WeekCharge { get { if (!isEn) return "MPPT 충전 전력량 (WEEK) [DSP]"; else return "MPPT Charging Power(WEEK) [DSP]"; } }
        public string lSystem_MonthCharge { get { if (!isEn) return "MPPT 충전 전력량 (MONTH) [DSP]"; else return "MPPT Charging Power(MONTH) [DSP]"; } }
        public string lSystem_DayDischarge { get { if (!isEn) return "AC 방전 전력량 (DAY) [DSP]"; else return "AC Discharging Power(DAY) [DSP]"; } }
        public string lSystem_WeekDischarge { get { if (!isEn) return "AC 방전 전력량 (WEEK) [DSP]"; else return "AC Discharging Power(WEEK) [DSP]"; } }
        public string lSystem_MonthDischarge { get { if (!isEn) return "AC 방전 전력량 (MONTH) [DSP]"; else return "AC Discharging Power(MONTH) [DSP]"; } }

        public string lSystem_ChSeleMode = "Ch Sele/Mode";
        public string lSystem_ParallelMode = "Parallel Mode";
        public string lSystem_WarningAlarm = "Warning Alarm";
        public string lSystem_GridAlarm = "Grid Alarm";
        public string lSystem_DCAlarm = "DC Alarm";
        public string lSystem_FaultAlarm = "Fault Alarm";
        public string lSystem_CheckSum = "CheckSum";

        public string lBMSTotalStatus_40000 { get { if (isEn) return "Alarm Flags "; else return "Alarm Flags "; } }
        public string lBMSTotalStatus_40001 { get { if (isEn) return "Warning Flags "; else return "Warning Flags "; } }
        public string lBMSTotalStatus_40002 { get { if (isEn) return "Fault Flags "; else return "Fault Flags "; } }

        public string lBMSTotalStatus_40003 { get { if (isEn) return "Alarm Module Position "; else return "Alarm Module Position "; } }
        public string lBMSTotalStatus_40004 { get { if (isEn) return "Warning Module Position "; else return "Warning Module Position "; } }
        public string lBMSTotalStatus_40005 { get { if (isEn) return "Fault Module Position "; else return "Fault Module Position "; } }
        public string lBMSTotalStatus_40006 { get { if (isEn) return "Battery Rack Voltage "; else return "Battery Rack Voltage "; } }
        public string lBMSTotalStatus_40007 { get { if (isEn) return "Battery Rack Current "; else return "Battery Rack Current "; } }
        public string lBMSTotalStatus_40008 { get { if (isEn) return "Battery Rack SOC "; else return "Battery Rack SOC "; } }
        public string lBMSTotalStatus_40009 { get { if (isEn) return "Battery Rack SOH "; else return "Battery Rack SOH "; } }
        public string lBMSTotalStatus_40010 { get { if (isEn) return "Battery Rack Min. Cell Voltage "; else return "Battery Rack Min. Cell Voltage "; } }
        public string lBMSTotalStatus_40011 { get { if (isEn) return "Module ID of Battery Rack Min. Cell Voltage"; else return "Module ID of Battery Rack Min. Cell Voltage"; } }
        public string lBMSTotalStatus_40012 { get { if (isEn) return "Battery Rack Max. Cell Voltage"; else return "Battery Rack Max. Cell Voltage"; } }
        public string lBMSTotalStatus_40013 { get { if (isEn) return "Module ID of Battery Rack Max. Cell Voltage"; else return "Module ID of Battery Rack Max. Cell Voltage"; } }
        public string lBMSTotalStatus_40014 { get { if (isEn) return "Battery Rack Min. Temperature "; else return "Battery Rack Min. Temperature "; } }
        public string lBMSTotalStatus_40015 { get { if (isEn) return "Module ID of Battery Rack Min. Temperature "; else return "Module ID of Battery Rack Min. Temperature "; } }
        public string lBMSTotalStatus_40016 { get { if (isEn) return "Battery Rack Max. Temperature "; else return "Battery Rack Max. Temperature "; } }
        public string lBMSTotalStatus_40017 { get { if (isEn) return "Module ID of Battery Rack Max. Temperature "; else return "Module ID of Battery Rack Max. Temperature "; } }
        public string lBMSTotalStatus_40018 { get { if (isEn) return "Battery Rack DC Charge Current Limit "; else return "Battery Rack DC Charge Current Limit "; } }
        public string lBMSTotalStatus_40019 { get { if (isEn) return "Battery Rack DC Discharge Current Limit "; else return "Battery Rack DC Discharge Current Limit "; } }
        public string lBMSTotalStatus_40020 { get { if (isEn) return "Max. DC Charge Current Limit Per Module "; else return "Max. DC Charge Current Limit Per Module "; } }
        public string lBMSTotalStatus_40021 { get { if (isEn) return "Max. DC Discharge Current Limit Per Module "; else return "Max. DC Discharge Current Limit Per Module "; } }
        public string lBMSTotalStatus_40022 { get { if (isEn) return "Number of Battery Modules "; else return "Number of Battery Modules "; } }
        public string lBMSTotalStatus_40023 { get { if (isEn) return "Number of Cells in Series per "; else return "Number of Cells in Series per "; } }

        public string lBMSModule1_40045 { get { if (isEn) return "BMS Hardware Version "; else return "BMS의 하드웨어 버전"; } }
        public string lBMSModule1_40046 { get { if (isEn) return "BMS Software Version "; else return "BMS의 소프트웨어 버전"; } }
        public string lBMSModule1_40047 { get { if (isEn) return "Battery Module Status "; else return "배터리 모듈 상태"; } }
        public string lBMSModule1_40048 { get { if (isEn) return "Battert Module Alarm Flags "; else return "알람 flags"; } }
        public string lBMSModule1_40049 { get { if (isEn) return "Battery Rack Module Flags "; else return "경고 flags"; } }
        public string lBMSModule1_40050 { get { if (isEn) return "Battery Module Fault Flags "; else return "실패 flags"; } }
        public string lBMSModule1_40051 { get { if (isEn) return "-"; else return "-"; } }
        public string lBMSModule1_40052 { get { if (isEn) return "SOC "; else return "배터리 모듈의 SOC"; } }
        public string lBMSModule1_40053 { get { if (isEn) return "SOH "; else return "배터리 모듈의 SOH"; } }
        public string lBMSModule1_40054 { get { if (isEn) return "Module Voltage "; else return "배터리 모듈의 전압"; } }
        public string lBMSModule1_40055 { get { if (isEn) return "Module Current "; else return "배터리 모듈의 전류"; } }
        public string lBMSModule1_40056 { get { if (isEn) return "Min. Cell Voltage"; else return "셀의 최저 전압"; } }
        public string lBMSModule1_40057 { get { if (isEn) return "Max. Cell Voltage"; else return "셀의 최고 전압"; } }
        public string lBMSModule1_40058 { get { if (isEn) return "Avg. Cell Voltage"; else return "평균 셀 전압"; } }
        public string lBMSModule1_40059 { get { if (isEn) return "Charge Current Limit"; else return "한계 충전 전류"; } }
        public string lBMSModule1_40060 { get { if (isEn) return "Discharge Current Limit"; else return "한계 방전 전류"; } }
        public string lBMSModule1_40061 { get { if (isEn) return "Cell 1 Voltage"; else return "1번 셀의 전압"; } }
        public string lBMSModule1_40062 { get { if (isEn) return "Cell 2 Voltage"; else return "2번 셀의 전압"; } }
        public string lBMSModule1_40063 { get { if (isEn) return "Cell 3 Voltage"; else return "3번 셀의 전압"; } }
        public string lBMSModule1_40064 { get { if (isEn) return "Cell 4 Voltage"; else return "4번 셀의 전압"; } }
        public string lBMSModule1_40065 { get { if (isEn) return "Cell 5 Voltage"; else return "5번 셀의 전압"; } }
        public string lBMSModule1_40066 { get { if (isEn) return "Cell 6 Voltage"; else return "6번 셀의 전압"; } }
        public string lBMSModule1_40067 { get { if (isEn) return "Cell 7 Voltage"; else return "7번 셀의 전압"; } }
        public string lBMSModule1_40068 { get { if (isEn) return "Cell 8 Voltage"; else return "8번 셀의 전압"; } }
        public string lBMSModule1_40069 { get { if (isEn) return "Cell 9 Voltage"; else return "9번 셀의 전압"; } }
        public string lBMSModule1_40070 { get { if (isEn) return "Cell 10 Voltage"; else return "10번 셀의 전압"; } }
        public string lBMSModule1_40071 { get { if (isEn) return "Cell 11 Voltage"; else return "11번 셀의 전압"; } }
        public string lBMSModule1_40072 { get { if (isEn) return "Cell 12 Voltage"; else return "12번 셀의 전압"; } }
        public string lBMSModule1_40073 { get { if (isEn) return "Cell 13 Voltage"; else return "13번 셀의 전압"; } }
        public string lBMSModule1_40074 { get { if (isEn) return "Cell 14 Voltage"; else return "14번 셀의 전압"; } }
        public string lBMSModule1_40075 { get { if (isEn) return "Cell 15 Voltage"; else return "15번 셀의 전압"; } }
        public string lBMSModule1_40076 { get { if (isEn) return "Cell 16 Voltage"; else return "16번 셀의 전압"; } }
        public string lBMSModule1_40077 { get { if (isEn) return "Module Temperature 1"; else return "모듈 내부 온도1"; } }
        public string lBMSModule1_40078 { get { if (isEn) return "Module Temperature 2"; else return "모듈 내부 온도2"; } }
        public string lBMSModule1_40079 { get { if (isEn) return "Avg. Temperature"; else return "모듈 내부 평균 온도"; } }
        public string lBMSModule1_40080 { get { if (isEn) return "Balancing Target"; else return "Balancing Target"; } }
        public string lBMSModule1_40081 { get { if (isEn) return "No. of cycles"; else return "재충전 사이클 횟수"; } }
        public string lBMSModule1_40082 { get { if (isEn) return "Design C.o.M"; else return "설계 용량"; } }
        public string lBMSModule1_40083 { get { if (isEn) return "Usable C.o.M"; else return "사용가능 용량"; } }
        public string lBMSModule1_40084 { get { if (isEn) return "Remaining C.o.M"; else return "남은 용량"; } }

        public string lSystem120CMD91 { get { if (isEn) return "Grid-connected and test preparation request"; else return "Grid-connected and test preparation request"; } }
        public string lSystem120CMD92 { get { if (isEn) return "Charge"; else return "Charge"; } }
        public string lSystem120CMD93 { get { if (isEn) return "Discharge"; else return "Discharge"; } }
        public string lSystem120CMD95 { get { if (isEn) return "Grid-connected release request"; else return "Grid-connected release request"; } }

        public string lSystem110CMD90 { get { if (isEn) return "Charge/discharge input MC ON"; else return "Charge/discharge input MC ON"; } }
        public string lSystem110CMD91 { get { if (isEn) return "Charge/discharge output MC ON"; else return "Charge/discharge output MC ON"; } }
        public string lSystem110CMD92 { get { if (isEn) return "Charge Start"; else return "Charge Start"; } }
        public string lSystem110CMD93 { get { if (isEn) return "Discharge Start"; else return "Discharge Start"; } }
        public string lSystem110CMD94 { get { if (isEn) return "Charge/discharge output MC OFF"; else return "Charge/discharge output MC OFF"; } }
        public string lSystem110CMD95 { get { if (isEn) return "Charge/discharge input/output MC OFF"; else return "Charge/discharge input/output MC OFF"; } }
        public string lSystem110CMD96 { get { if (isEn) return "Charge/discharge Stop"; else return "Charge/discharge Stop"; } }
        public string lSystem110CMD97 { get { if (isEn) return "Collect external pattern data "; else return "Collect external pattern data "; } }
        public string lSystem110CMD98 { get { if (isEn) return "Operation external pattern"; else return "Operation external pattern"; } }
        public string lSystem110CMD99 { get { if (isEn) return "Error Code Clear"; else return "Error Code Clear"; } }

        public string lSystemOPMode0 { get { if (isEn) return "Stand by"; else return "Stand by"; } }
        public string lSystemOPMode1 { get { if (isEn) return "Ready for input"; else return "Ready for input"; } }
        public string lSystemOPMode2 { get { if (isEn) return "Charge/discharge test Ready"; else return "Charge/discharge test Ready"; } }
        public string lSystemOPMode3 { get { if (isEn) return "Charge CC in progress"; else return "Charge CC in progress"; } }
        public string lSystemOPMode4 { get { if (isEn) return "Charge CV in progress"; else return "Charge CV in progress"; } }
        public string lSystemOPMode5 { get { if (isEn) return "Discharge CC in progress"; else return "Discharge CC in progress"; } }
        public string lSystemOPMode6 { get { if (isEn) return "Discharge CV in progress"; else return "Discharge CV in progress"; } }
        public string lSystemOPMode7 { get { if (isEn) return "Charge CP in progress"; else return "Charge CP in progress"; } }
        public string lSystemOPMode8 { get { if (isEn) return "Discharge CP in progress"; else return "Discharge CP in progress"; } }
        public string lSystemOPMode9 { get { if (isEn) return "Complete test"; else return "Complete test"; } }

        public string lSystemParMode0 { get { if (isEn) return "Individual operating mode complete"; else return "Individual operating mode complete"; } }
        public string lSystemParMode1 { get { if (isEn) return "Preparing for parallel operating mode"; else return "Preparing for parallel operating mode"; } }
        public string lSystemParMode2 { get { if (isEn) return "Parallel operating mode complete"; else return "Parallel operating mode complete"; } }

        public string lSystem120Warning001 { get { if (isEn) return "Battery Connection"; else return "Battery Connection"; } }
        public string lSystem120Warning002 { get { if (isEn) return "Channel1 test Current Over"; else return "Channel1 test Current Over"; } }
        public string lSystem120Warning004 { get { if (isEn) return "Channel1 test Voltage Over"; else return "Channel1 test Voltage Over"; } }
        public string lSystem120Warning080 { get { if (isEn) return "Battery#1 sample is Empty"; else return "Battery#1 sample is Empty"; } }
        public string lSystem120Warning100 { get { if (isEn) return "Inspector opened the front door"; else return "Inspector opened the front door"; } }

        public string lSystem120Fault001 { get { if (isEn) return "High temperature generate heat sink"; else return "High temperature generate heat sink"; } }
        public string lSystem120Fault002 { get { if (isEn) return "Input fuse Damaged"; else return "Input fuse Damaged"; } }
        public string lSystem120Fault004 { get { if (isEn) return "Output fuse Damaged"; else return "Output fuse Damaged"; } }
        public string lSystem120Fault008 { get { if (isEn) return "Heat fan Defects"; else return "Heat fan Defects"; } }
        public string lSystem120Fault010 { get { if (isEn) return "Emergency switch action"; else return "Emergency switch action"; } }
        public string lSystem120Fault020 { get { if (isEn) return "Input R IGBT issues"; else return "Input R IGBT issues"; } }
        public string lSystem120Fault040 { get { if (isEn) return "Input S IGBT issues"; else return "Input S IGBT issues"; } }
        public string lSystem120Fault080 { get { if (isEn) return "Input T IGBT issues"; else return "Input T IGBT issues"; } }
        public string lSystem120Fault100 { get { if (isEn) return "Channel1 IGBT issues"; else return "Channel1 IGBT issues"; } }
        public string lSystem120Fault800 { get { if (isEn) return "PC-DSP Communication issues"; else return "PC-DSP Communication issues"; } }

        public string lSystem120DC001 { get { if (isEn) return "Current Stage overcharge occurs"; else return "Current Stage overcharge occurs"; } }
        public string lSystem120DC002 { get { if (isEn) return "Current Stage voltage unbalanced"; else return "Current Stage voltage unbalanced"; } }
        public string lSystem120DC004 { get { if (isEn) return "Capacitor#1 overcharge occurs"; else return "Capacitor#1 overcharge occurs"; } }
        public string lSystem120DC008 { get { if (isEn) return "Capacitor#1 voltage unbalanced"; else return "Capacitor#1 voltage unbalanced"; } }
        public string lSystem120DC010 { get { if (isEn) return "Battery#1 overcharge occurs"; else return "Battery#1 overcharge occurs"; } }
        public string lSystem120DC020 { get { if (isEn) return "Battery#1 overcurrent occurs"; else return "Battery#1 overcurrent occurs"; } }

        public string lSystem120Grid001 { get { if (isEn) return "Input cable early defects"; else return "Input cable early defects"; } }
        public string lSystem120Grid002 { get { if (isEn) return "Input R overcurrent issues"; else return "Input R overcurrent issues"; } }
        public string lSystem120Grid004 { get { if (isEn) return "Input S overcurrent issues"; else return "Input S overcurrent issues"; } }
        public string lSystem120Grid010 { get { if (isEn) return "Input T overcurrent issues"; else return "Input T overcurrent issues"; } }
        public string lSystem120Grid020 { get { if (isEn) return "Input overvoltage issues"; else return "Input overvoltage issues"; } }
        public string lSystem120Grid040 { get { if (isEn) return "Input overvoltage issues"; else return "Input overvoltage issues"; } }
        public string lSystem120Grid100 { get { if (isEn) return "Initial charge fail"; else return "Initial charge fail"; } }
        public string lSystem120Grid200 { get { if (isEn) return "Input voltage unbalanced"; else return "Input voltage unbalanced"; } }





        public string lSystem120_28335_W001 { get { return "Battery connection is reversed / 배터리 역결선 / Warning 001"; } }
        public string lSystem120_28335_W002 { get { return "Battery test current exceed / 배터리 시험 전류 초과 / Warning 002"; } }
        public string lSystem120_28335_W004 { get { return "Battery test voltage exceed / 배터리 시험 전압 초과 / Warning 004"; } }
        public string lSystem120_28335_W080 { get { return "Battery sample is Empty / 배터리 시료 없음 / Warning 080"; } }
        
        public string lSystem120_28335_G001 { get { return "Input cable 3-Phase order failure / 입력 케이블 상순 불량 / Grid 001"; } }
        public string lSystem120_28335_G002 { get { return "Input R over current occurs / 입력R상 과전류 발생 / Grid 002"; } }
        public string lSystem120_28335_G004 { get { return "Input S over current occurs / 입력S상 과전류 발생 / Grid 004"; } }
        public string lSystem120_28335_G010 { get { return "Input T over current occurs / 입력T상 과전류 발생 / Grid 010"; } }
        public string lSystem120_28335_G020 { get { return "Input over Voltage occurs / 입력 과전압 발생 / Grid 020"; } }
        public string lSystem120_28335_G040 { get { return "Input under Voltage occurs / 입력 저전압 발생 / Grid 040"; } }
        public string lSystem120_28335_G100 { get { return "Initial charge fail / 초기 충전 실패 / Grid 100"; } }
        public string lSystem120_28335_G200 { get { return "Input voltage unbalanced / 입력 전압 불평형 / Grid 200"; } }

        public string lSystem120_28335_D001 { get { return "Current Stage over charge occurs / 정류단 과충전 발생 / DC 001"; } }
        public string lSystem120_28335_D002 { get { return "Current Stage under Voltage occurs / 정류단 저전압 발생 / DC 002"; } }
        public string lSystem120_28335_D004 { get { return "Output Line failure occurs / 출력 라인 불량 발생 / DC 004"; } }
        public string lSystem120_28335_D010 { get { return "Battery over charge occurs / 배터리 과충전 발생 / DC 010"; } }
        public string lSystem120_28335_D020 { get { return "Battery over current occurs / 배터리 과전류 발생 / DC 020"; } }
        
        public string lSystem120_28335_F001 { get { return "AC/DC Stage heat sink high temperature occurs / AC/DC부 방열판 과온도 발생 / Fault 001"; } }
        public string lSystem120_28335_F002 { get { return "Input fuse Damaged / 입력 퓨즈 소손 / Fault 002"; } }
        public string lSystem120_28335_F004 { get { return "Output fuse Damaged / 출력 퓨즈 소손 / Fault 004"; } }
        public string lSystem120_28335_F008 { get { return "DC/DC Stage heat sink high temperature occurs / DC/DC부 방열판 과온도 발생 / Fault 008"; } }
        public string lSystem120_28335_F010 { get { return "Emergency switch action / 비상스위치 동작 / Fault 010"; } }
        public string lSystem120_28335_F020 { get { return "Input R IGBT issues occurs/ 입력 R상 IGBT 문제 발생 / Fault 020"; } }
        public string lSystem120_28335_F040 { get { return "Input S IGBT issues occurs/ 입력 S상 IGBT 문제 발생 / Fault 040"; } }
        public string lSystem120_28335_F080 { get { return "Input T IGBT issues occurs/ 입력 T상 IGBT 문제 발생 / Fault 080"; } }
        public string lSystem120_28335_F100 { get { return "DCDC#01 IGBT issues occurs/ DCDC#01 IGBT 문제 발생 / Fault 100"; } }
        public string lSystem120_28335_F200 { get { return "DCDC#02 IGBT issues occurs/ DCDC#02 IGBT 문제 발생 / Fault 200"; } }
        public string lSystem120_28335_F400 { get { return "DCDC#03 IGBT issues occurs/ DCDC#03 IGBT 문제 발생 / Fault 400"; } }
        public string lSystem120_28335_F800 { get { return "PC-DSP Communication issues occurs/ PC-DSP 통신 문제 발생 / Fault 800"; } }








        public string lSystem120_28377_W001 { get { return "Battery connection is reversed / 배터리 역결선 / Warning 001"; } }
        public string lSystem120_28377_W002 { get { return "Battery sample is Empty (Current line check) / 배터리 시료 없음 (전류 라인 체크) / Warning 002"; } }
        public string lSystem120_28377_W004 { get { return "Battery test current exceed / 배터리 시험 전류 초과 / Warning 004"; } }
        public string lSystem120_28377_W008 { get { return "Battery test voltage exceed / 배터리 시험 전압 초과 / Warning 008"; } }

        public string lSystem120_28377_W010 { get { return "Inverter stage IGBT high temperature occurs / 인버터부 IGBT 과온도 / Warning 010"; } }
        public string lSystem120_28377_W020 { get { return "DC/DC stage IGBT high temperature occurs / DC/DC부 IGBT 과온도 / Warning 020"; } }
        public string lSystem120_28377_W040 { get { return "Heat sink colling fan issues occurs / 방열판 Cooling FAN 동작 이상 / Warning 040"; } }
        public string lSystem120_28377_W080 { get { return "AC fan issuses occurs / AC FAN 동작 이상 / Warning 080"; } }     

        public string lSystem120_28377_G001 { get { return "Input cable 3-Phase order failure / 입력 케이블 상순 불량 / Grid 001"; } }
        public string lSystem120_28377_G002 { get { return "Initial charge fail / 초기 충전 실패 / Grid 002"; } }
        public string lSystem120_28377_G004 { get { return "Input R over current occurs / 입력R상 과전류 발생 / Grid 004"; } }
        public string lSystem120_28377_G008 { get { return "Input S over current occurs / 입력S상 과전류 발생 / Grid 008"; } }
        public string lSystem120_28377_G010 { get { return "Input T over current occurs / 입력T상 과전류 발생 / Grid 010"; } }
        public string lSystem120_28377_G020 { get { return "Input over Voltage occurs / 입력 과전압 발생 / Grid 020"; } }
        public string lSystem120_28377_G040 { get { return "Input under Voltage occurs / 입력 저전압 발생 / Grid 040"; } }
        public string lSystem120_28377_G080 { get { return "Inverter stage heat sink high temperature occurs / 인버터부 방열판 과온도 발생/ Grid 080"; } }
        public string lSystem120_28377_G100 { get { return "Input fuse Damaged / 입력 퓨즈 소손 / Grid 100"; } }
        public string lSystem120_28377_G200 { get { return "Input voltage unbalanced / 입력 전압 불평형 / Grid 200"; } }
        public string lSystem120_28377_G400 { get { return "Input voltage unbalanced / 입력 전압 불평형 / Grid 400"; } }

        public string lSystem120_28377_D001 { get { return "Current Stage over charge occurs / 정류단 과충전 발생 / DC 001"; } }
        public string lSystem120_28377_D002 { get { return "Output Line failure occurs / 출력 라인 불량 발생 / DC 002"; } }
        public string lSystem120_28377_D004 { get { return "Battery over voltage occurs / 배터리 과충전 발생 / DC 004"; } }
        public string lSystem120_28377_D008 { get { return "Battery over current occurs / 배터리 과전류 발생 / DC 008"; } }        
        public string lSystem120_28377_D010 { get { return "DC/DC Stage heat sink high temperature occurs / DC/DC부 방열판 과온도 발생 / DC 010"; } }
        public string lSystem120_28377_D020 { get { return "Output fuse Damaged / 출력 퓨즈 소손 / DC 020"; } }
        public string lSystem120_28377_D040 { get { return "Current Stage under Voltage occurs / 정류단 저전압 발생 / DC 040"; } }
        public string lSystem120_28377_D080 { get { return "Safety Stopped!!! (over Charge/discharge 13sec) / 안전 조건 (13초 초과) / DC 080"; } }

        public string lSystem120_28377_F001 { get { return "Emergency switch action / 비상스위치 동작 / Fault 001"; } }
        public string lSystem120_28377_F002 { get { return "Input R IGBT issues occurs / 입력 R상 IGBT 문제 발생 / Fault 002"; } }
        public string lSystem120_28377_F004 { get { return "Input S IGBT issues occurs / 입력 S상 IGBT 문제 발생 / Fault 004"; } }
        public string lSystem120_28377_F008 { get { return "Input T IGBT issues occurs / 입력 T상 IGBT 문제 발생 / Fault 008"; } }
        public string lSystem120_28377_F010 { get { return "DCDC IGBT01 issues occurs / DCDC IGBT01 문제 발생 / Fault 010"; } }
        public string lSystem120_28377_F020 { get { return "DCDC IGBT02 issues occurs / DCDC IGBT02 문제 발생 / Fault 020"; } }
        public string lSystem120_28377_F040 { get { return "DCDC IGBT03 issues occurs / DCDC IGBT03 문제 발생 / Fault 040"; } }
        public string lSystem120_28377_F080 { get { return "DCDC IGBT04 issues occurs / DCDC IGBT04 문제 발생 / Fault 080"; } }
        public string lSystem120_28377_F100 { get { return "DCDC IGBT05 issues occurs / DCDC IGBT05 문제 발생 / Fault 100"; } }
        public string lSystem120_28377_F200 { get { return "DCDC IGBT06 issues occurs / DCDC IGBT06 문제 발생 / Fault 200"; } }
        public string lSystem120_28377_F400 { get { return "PC-DSP Communication issues occurs/ PC-DSP 통신 문제 발생 / Fault 400"; } }

    }
}
