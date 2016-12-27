using System;

namespace FastRsync.Diagnostics
{
    public class NullProgressReporter : IProgress<ProgressReport>
    {
        public void Report(ProgressReport value)
        {
        }
    }
}