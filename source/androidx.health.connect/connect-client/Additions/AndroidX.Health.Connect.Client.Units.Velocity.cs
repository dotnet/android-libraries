using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Velocity
    {
        public int CompareTo(object? obj)
        {
            if (obj is Velocity other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Velocity");
        }
    }
}