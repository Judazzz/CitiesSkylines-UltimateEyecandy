using System;

namespace UltimateEyecandy
{
    [Serializable]
    public struct Fraction
    {
        public uint num;
        public uint den;


        public override string ToString()
        {
            if (num == 0)
                return "paused";

            if (num == 1 && den == 1)
                return "normal";

            if (num > den)
                return num + "x";

            return num + "/" + den;
        }
    }
}
