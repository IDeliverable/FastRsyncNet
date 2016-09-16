using System;

namespace FastRsync.Exceptions
{
    public class CorruptFileFormatException : Exception
    {
        public CorruptFileFormatException(string message) : base(message)
        {
        }
    }
}