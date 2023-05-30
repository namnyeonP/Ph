using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Tower Lamp 클래스
 * 경광등 제어를 위한 클래스이며
 * 릴레이 제어와는 다르게 별도 이름 설정 없이 Relay 번호로 사용한다.
 * 경광등 제어 시나리오에 따른 메소드를 작성한다.
 * 메소드 안에 내용은 중복될 수 있다.
 * 실제 동작 코드는 중복되지만 혼동을 막기 위해 별도로 메소드를 만든다.
 */
namespace EOL_BASE.클래스
{
    public class TowerLamp
    {
        MainWindow mw;

        // Tower Lamp Relay 번호에 맞춰 주석을 달아놓는다.
        // 스테이션 갯수에 따라 아래 부분은 달라질 수 있다.
        // 현재까지 나온 프로젝트 중 최대 스테이션 구분은 4개 이므로 position #4까지 만들었다.
        // 실제로 position이 더 적은 경우는 해당 릴레이 포트를 다른곳에서 사용할 수 있다.
        #region TowerLamp Relay No

        // position #1
        // Red      = IDO_0
        // Orange   = IDO_1
        // Green    = IDO_2
        // Blue     = IDO_3

        // position #2
        // Red      = IDO_4
        // Orange   = IDO_5
        // Green    = IDO_6
        // Blue     = IDO_7

        // position #3
        // Red      = IDO_8
        // Orange   = IDO_9
        // Green    = IDO_10
        // Blue     = IDO_11

        // position #4
        // Red      = IDO_12
        // Orange   = IDO_13
        // Green    = IDO_14
        // Blue     = IDO_15

        #endregion

        public TowerLamp(MainWindow mw)
        {
            this.mw = mw;

            SetTLampRed(false);
            SetTLampOrange(false);
            SetTLampGreen(false);
            SetTLampBlue(false);
        }

        private bool GetTLampRed()
        {
            if (mw.position == "#1")
            {
                return mw.relays.RelayStatus("IDO_0");
            }
            else if (mw.position == "#2")
            {
                return mw.relays.RelayStatus("IDO_4");
            }
            else if (mw.position == "#3")
            {
                return mw.relays.RelayStatus("IDO_8");
            }
            else if (mw.position == "#4")
            {
                return mw.relays.RelayStatus("IDO_12");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 경광등 Red On/Off
        /// </summary>
        /// <param name="isOn"></param>
        private void SetTLampRed(bool isOn)
        {
            if (isOn)
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOn("IDO_0");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOn("IDO_4");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOn("IDO_8");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOn("IDO_12");
                }
            }
            else
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOff("IDO_0");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOff("IDO_4");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOff("IDO_8");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOff("IDO_12");
                }
            }
        }

        /// <summary>
        /// 경광등 Orange On/Off
        /// </summary>
        /// <param name="isOn"></param>
        private void SetTLampOrange(bool isOn)
        {
            if (isOn)
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOn("IDO_1");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOn("IDO_5");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOn("IDO_9");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOn("IDO_13");
                }
            }
            else
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOff("IDO_1");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOff("IDO_5");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOff("IDO_9");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOff("IDO_13");
                }
            }
        }

        /// <summary>
        /// 경광등 Green On/Off
        /// </summary>
        /// <param name="isOn"></param>
        private void SetTLampGreen(bool isOn)
        {
            if (isOn)
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOn("IDO_2");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOn("IDO_6");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOn("IDO_10");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOn("IDO_14");
                }
            }
            else
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOff("IDO_2");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOff("IDO_6");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOff("IDO_10");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOff("IDO_14");
                }
            }
        }

        /// <summary>
        /// 경광등 Blue On/Off
        /// </summary>
        /// <param name="isOn"></param>
        private void SetTLampBlue(bool isOn)
        {
            if (isOn)
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOn("IDO_3");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOn("IDO_7");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOn("IDO_11");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOn("IDO_15");
                }
            }
            else
            {
                if (mw.position == "#1")
                {
                    mw.relays.RelayOff("IDO_3");
                }
                if (mw.position == "#2")
                {
                    mw.relays.RelayOff("IDO_7");
                }
                if (mw.position == "#3")
                {
                    mw.relays.RelayOff("IDO_11");
                }
                if (mw.position == "#4")
                {
                    mw.relays.RelayOff("IDO_15");
                }
            }
        }

        /// <summary>
        /// NG 시 경광등 처리
        /// Red Lamp 3초간 켠 후 Off(다시 Off 하는 부분은 NG처리하는 부분에서 처리)
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampNG(bool isOn)
        {
            SetTLampRed(isOn);
        }

        public bool GetTLampNG()
        {
            return GetTLampRed();
        }

        /// <summary>
        /// 계측기 Off 시 Red 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampInstrumentOff(bool isOn)
        {
            SetTLampRed(isOn);
        }

        /// <summary>
        /// Manual 검사 전환 시 Orange 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampManualTest(bool isOn)
        {
            SetTLampOrange(isOn);
        }

        /// <summary>
        /// 대기 상태에서 Orange 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampStandBy(bool isOn)
        {
            SetTLampOrange(isOn);
        }

        /// <summary>
        /// 검사 완료 시 Orange 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampTestFinished(bool isOn)
        {
            SetTLampOrange(isOn);
        }

        /// <summary>
        /// 검사 중 상태는 Green 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampTesting(bool isOn)
        {
            SetTLampGreen(isOn);
        }

        /// <summary>
        /// MES 연동 시 Blue 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampMESStatus(bool isOn)
        {
            SetTLampBlue(isOn);
        }

        /// <summary>
        /// 프로그램 종료 시 경광등 릴레이 전체를 Off 시키기 위함
        /// </summary>
        public void OffTLamp()
        {
            if (mw.position == "#1")
            {
                mw.relays.RelayOff("IDO_0");
                mw.relays.RelayOff("IDO_1");
                mw.relays.RelayOff("IDO_2");
                mw.relays.RelayOff("IDO_3");
            }

            if (mw.position == "#2")
            {
                mw.relays.RelayOff("IDO_4");
                mw.relays.RelayOff("IDO_5");
                mw.relays.RelayOff("IDO_6");
                mw.relays.RelayOff("IDO_7");
            }

            if (mw.position == "#3")
            {
                mw.relays.RelayOff("IDO_8");
                mw.relays.RelayOff("IDO_9");
                mw.relays.RelayOff("IDO_10");
                mw.relays.RelayOff("IDO_11");
            }

            if (mw.position == "#4")
            {
                mw.relays.RelayOff("IDO_12");
                mw.relays.RelayOff("IDO_13");
                mw.relays.RelayOff("IDO_14");
                mw.relays.RelayOff("IDO_15");
            }
        }
    }
}
