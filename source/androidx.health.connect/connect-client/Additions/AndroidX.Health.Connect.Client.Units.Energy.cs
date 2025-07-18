using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Energy
    {
        public int CompareTo(object? obj)
        {
            if (obj is Energy other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Energy");
        }
    }
}