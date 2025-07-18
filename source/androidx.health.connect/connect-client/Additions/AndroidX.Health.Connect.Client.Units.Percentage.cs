using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Percentage
    {
        public int CompareTo(object? obj)
        {
            if (obj is Percentage other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Percentage");
        }
    }
}