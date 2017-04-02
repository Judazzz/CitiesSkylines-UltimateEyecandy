using ColossalFramework;
using ColossalFramework.IO;
using System;
using UnityEngine;

namespace UltimateEyecandy
{
    [Serializable]
    public class Settings
    {
        public Fraction speed;
        public uint dayOffsetFrames;
        public float longitude;
        public float lattitude;
        public float sunSize;
        public float sunIntensity;
    }

    public class DayNightCycleManager : MonoBehaviour, ISimulationManager
    {
        public Fraction speed;
        private uint tick;
        SimulationManager sim = Singleton<SimulationManager>.instance;
        private uint dayOffsetFrames;

        public bool DayNightEnabled
        {
            get
            {
                return sim.m_enableDayNight;
            }
        }

        public ThreadProfiler GetSimulationProfiler()
        {
            return null;
        }

        public void Awake()
        {
            dayOffsetFrames = sim.m_dayTimeOffsetFrames;
            SimulationManager.RegisterSimulationManager(this);
            speed = new Fraction() { num = 1, den = 1 };
        }

        //  Called after SimulationManager.SimulationStep() => overwrite what they were doing:
        public void SimulationStep(int subStep)
        {
            if (!DayNightEnabled)
                return;

            if (!sim.SimulationPaused && !sim.ForcedSimulationPaused)
            {
                //  Do every nth frame for fractional speeds:
                if (tick == 0)
                {
                    dayOffsetFrames = (dayOffsetFrames + (uint)speed.num - 1) % SimulationManager.DAYTIME_FRAMES;
                }
                else
                {
                    dayOffsetFrames = (dayOffsetFrames - 1) % SimulationManager.DAYTIME_FRAMES;
                }
                tick = (tick + 1) % speed.den;
            }
            sim.m_dayTimeOffsetFrames = dayOffsetFrames;
        }

        public float TimeOfDay
        {
            set
            {
                if (!DayNightEnabled)
                {
                    return;
                }
                int offset = (int)((value - sim.m_currentDayTimeHour) / SimulationManager.DAYTIME_FRAME_TO_HOUR);
                dayOffsetFrames = (uint)(((long)dayOffsetFrames + offset) % SimulationManager.DAYTIME_FRAMES);
                sim.m_currentDayTimeHour = value;
            }
            get
            {
                return sim.m_currentDayTimeHour;
            }
        }

        public string GetName()
        {
            return "Sun Controller";
        }

        public void EarlyUpdateData()
        {
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {
        }

        public void LateUpdateData(SimulationManager.UpdateMode mode)
        {
        }

        public void GetData(FastList<IDataContainer> data)
        {
        }
    }
}