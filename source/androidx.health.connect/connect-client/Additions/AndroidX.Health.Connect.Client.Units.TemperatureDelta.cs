using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class TemperatureDelta
    {
        public int CompareTo(object? obj)
        {
            if (obj is TemperatureDelta other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type TemperatureDelta");
        }
    }
}