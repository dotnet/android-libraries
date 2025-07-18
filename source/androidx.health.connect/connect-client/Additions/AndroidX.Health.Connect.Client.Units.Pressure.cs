using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Pressure
    {
        public int CompareTo(object? obj)
        {
            if (obj is Pressure other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Pressure");
        }
    }
}