using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Volume
    {
        public int CompareTo(object? obj)
        {
            if (obj is Volume other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Volume");
        }
    }
}