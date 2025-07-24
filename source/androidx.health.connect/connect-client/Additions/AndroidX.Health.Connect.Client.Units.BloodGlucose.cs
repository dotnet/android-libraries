using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class BloodGlucose
    {
        public int CompareTo(object? obj)
        {
            if (obj is BloodGlucose other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type BloodGlucose");
        }
    }
}