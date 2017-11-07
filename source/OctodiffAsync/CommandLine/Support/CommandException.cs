using System;

namespace OctodiffAsync.CommandLine.Support
{
    class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}