using System;

namespace AndroidX.Health.Connect.Client.Records
{
    public partial class FhirVersion
    {
        public int CompareTo(object? obj)
        {
            if (obj is FhirVersion other)
                return CompareTo(other);
            throw new ArgumentException("Object must be of type FhirVersion");
        }
    }
}