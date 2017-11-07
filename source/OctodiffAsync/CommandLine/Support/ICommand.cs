using System.IO;

namespace OctodiffAsync.CommandLine.Support
{
    interface ICommand
    {
        void GetHelp(TextWriter writer);
        int Execute(string[] commandLineArguments);
    }
}
