using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Temperature
    {
        public int CompareTo(object? obj)
        {
            if (obj is Temperature other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Temperature");
        }
    }
}