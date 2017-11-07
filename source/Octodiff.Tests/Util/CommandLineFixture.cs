using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using OctodiffAsync;
using Octopus.Platform.Util;

namespace Octodiff.Tests.Util
{
    public enum OctodiffAppVariant
    {
        Sync,
        Async
    }

    public abstract class CommandLineFixture
    {
        protected string StdErr { get; private set; }
        protected string StdOut { get; private set; }
        protected string Output { get; private set; }
        protected int ExitCode { get; set; }

        public void Run(string args, OctodiffAppVariant octodiff)
        {
            var stdErrBuilder = new StringBuilder();
            var stdOutBuilder = new StringBuilder();
            var outputBuilder = new StringBuilder();
            var path = octodiff == OctodiffAppVariant.Sync ? 
                new Uri(typeof (OctodiffProgram).Assembly.CodeBase).LocalPath
                : new Uri(typeof(OctodiffAsyncProgram).Assembly.CodeBase).LocalPath;

            var exit = SilentProcessRunner.ExecuteCommand(path,
                args,
                TestContext.CurrentContext.TestDirectory,
                output =>
                {
                    stdOutBuilder.AppendLine(output);
                    outputBuilder.AppendLine(output);
                    Trace.WriteLine(output);
                },
                output =>
                {
                    stdErrBuilder.AppendLine(output);
                    outputBuilder.AppendLine(output);
                    Trace.WriteLine(output);
                });

            StdErr = stdErrBuilder.ToString();
            StdOut = stdOutBuilder.ToString();
            Output = outputBuilder.ToString();
            ExitCode = exit;
        }
    }
}
