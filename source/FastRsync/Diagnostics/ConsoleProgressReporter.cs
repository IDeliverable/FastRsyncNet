using System;

namespace FastRsync.Diagnostics
{
    public class ConsoleProgressReporter : IProgress<ProgressReport>
    {
        private ProgressOperationType currentOperation;
        private int progressPercentage;

        public void Report(ProgressReport progress)
        {
            var percent = (int)((double)progress.CurrentPosition / progress.Total * 100d + 0.5);
            if (currentOperation != progress.Operation)
            {
                progressPercentage = -1;
                currentOperation = progress.Operation;
            }

            if (progressPercentage != percent && percent % 10 == 0)
            {
                progressPercentage = percent;
                Console.WriteLine("{0}: {1}%", currentOperation, percent);
            }
        }
    }
}