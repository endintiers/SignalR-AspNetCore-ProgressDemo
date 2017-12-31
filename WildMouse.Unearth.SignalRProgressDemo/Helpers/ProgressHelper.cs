using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WildMouse.Unearth.SignalRProgressDemo.SignalR;

namespace WildMouse.Unearth.SignalRProgressDemo.Helpers
{
    public class ProgressHelper
    {
        public static async Task SomeLongRunningTask(NoPostHub noPostHub, LongRunningTaskParameters parms)
        {
            try
            {
                ReportAndSleep(noPostHub, "Starting Out", 10, 1000);
                ReportAndSleep(noPostHub, "Stocking Ponds", 20, 1000);
                ReportAndSleep(noPostHub, "Individualizing Snowflakes", 30, 1000);
                ReportAndSleep(noPostHub, "Hydrating Harvestables", 40, 1000);
                ReportAndSleep(noPostHub, "Unexpectedly Reticulating Splines", 50, 1000);
                if (parms.ThrowEx) { throw new ApplicationException("Something bad happened"); }
                ReportAndSleep(noPostHub, "Calculating Snowball Trajectories", 60, 1000);
                ReportAndSleep(noPostHub, "Assessing Loam Particle Sizes", 70, 1000);
                ReportAndSleep(noPostHub, "Timing Temperature Transference", 80, 1000);
                ReportAndSleep(noPostHub, "Preparing Bacon for Homeward Transportation", 90, 1000);
                ReportAndSleep(noPostHub, "One Hundred Percent Finished", 100, 1000);

                // Tell the js code we finished
                var info = new ProgressInfo() { message = "All messages sent OK", pct = 0 };
                await noPostHub.Clients.Client(noPostHub.Context.ConnectionId).ProcessCompleted(info);
            }
            catch (Exception ex)
            {
                await noPostHub.Clients.Client(noPostHub.Context.ConnectionId).SomethingWentWrong(ex.Message);
            }
        }

        private static void ReportAndSleep(NoPostHub noPostHub, string message, int pct, int sleepFor)
        {
            var info = new ProgressInfo() { message = message, pct = pct };
            noPostHub.Clients.Client(noPostHub.Context.ConnectionId).ReportProgress(info);
            Thread.Sleep(sleepFor);
        }
    }
}
