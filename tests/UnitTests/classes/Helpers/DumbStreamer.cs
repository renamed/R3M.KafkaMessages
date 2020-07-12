using Microsoft.Extensions.Logging;
using WritRead.Streamers;

namespace UnitTests.classes.Helpers
{
    public class DumbStreamer : Streamer
    {
        public int CleanUpCalls;
        public int RunCalls;
        public DumbStreamer(string microserviceName, string topicName, ILoggerFactory logger) : base(microserviceName, topicName, logger)
        {
        }

        protected override void CleanUp()
        {
            CleanUpCalls++;
        }

        protected override void Run()
        {
            RunCalls++;
        }
    }
}