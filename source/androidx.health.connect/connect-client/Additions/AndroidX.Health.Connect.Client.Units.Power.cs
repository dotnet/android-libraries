using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Power
    {
        public int CompareTo(object? obj)
        {
            if (obj is Power other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Power");
        }
    }
}