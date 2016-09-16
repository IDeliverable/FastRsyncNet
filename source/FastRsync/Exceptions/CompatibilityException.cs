using System;

namespace FastRsync.Exceptions
{
    public class CompatibilityException : Exception
    {
        public CompatibilityException(string message) : base(message)
        {
            
        }
    }
}