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
        public static async Task SomeLongRunningTask(NoPostHub noPostHub)
        {
            var connectionId = noPostHub.Context.ConnectionId;

            ReportAndSleep(noPostHub, "Starting Out", 10, 1000);
            ReportAndSleep(noPostHub, "Reticulating Splines", 20, 1000);
            ReportAndSleep(noPostHub, "Graphing Whale Migration", 30, 1000);
            ReportAndSleep(noPostHub, "Obfuscating Quigley Matrix", 40, 1000);
            ReportAndSleep(noPostHub, "Realigning Alternate Time Frames", 50, 1000);
            ReportAndSleep(noPostHub, "Calculating Inverse Probability Matrices", 60, 1000);
            ReportAndSleep(noPostHub, "Routing Neural Network Infrastructure", 70, 1000);
            ReportAndSleep(noPostHub, "Calculating Llama Expectoration Trajectory", 80, 1000);
            ReportAndSleep(noPostHub, "Resolving GUID Conflict", 90, 1000);
            ReportAndSleep(noPostHub, "One Hundred Percent Finished", 100, 1000);

            // Tell the js code to reset
            ReportAndSleep(noPostHub, "Reset", 0, 0);
        }

        private static void ReportAndSleep(NoPostHub noPostHub, string message, int pct, int sleepFor)
        {
            var info = new ProgressInfo() { message = message, pct = pct };
            noPostHub.Clients.Client(noPostHub.Context.ConnectionId).ReportProgress(info);
            Thread.Sleep(sleepFor);
        }
    }
}
