using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Mass
    {
        public int CompareTo(object? obj)
        {
            if (obj is Mass other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Mass");
        }
    }
}