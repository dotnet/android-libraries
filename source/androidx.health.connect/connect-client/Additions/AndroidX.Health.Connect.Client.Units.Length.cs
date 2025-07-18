using System;

namespace AndroidX.Health.Connect.Client.Units
{
    public partial class Length
    {
        public int CompareTo(object? obj)
        {
            if (obj is Length other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type Length");
        }
    }
}