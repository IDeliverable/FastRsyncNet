using System;

namespace FastRsync.Exceptions
{
    public class UsageException : Exception
    {
        public UsageException(string message) : base(message)
        {
            
        }
    }
}