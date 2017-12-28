using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace WildMouse.Unearth.SignalRProgressDemo.Pages
{
    public class IndexModel : PageModel
    {
        IHubContext<ProgressHub, IProgressHub> _progressHubContext;
        public IndexModel(IHubContext<ProgressHub, IProgressHub> progressHubContext)
        {
            _progressHubContext = progressHubContext;
        }

        [BindProperty]
        public string ConnectionId { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Click the Progress button to see some async progress reporting, click the About menu item for an explanation and source code link";
        }

        public void OnPost()
        {
            // 10% report is done in js code...
            Thread.Sleep(1000);
            ReportAndSleep("Reticulating Splines", 20, ConnectionId, 1000);
            ReportAndSleep("Graphing Whale Migration", 30, ConnectionId, 1000);
            ReportAndSleep("Obfuscating Quigley Matrix", 40, ConnectionId, 1000);
            ReportAndSleep("Realigning Alternate Time Frames", 50, ConnectionId, 1000);
            ReportAndSleep("Calculating Inverse Probability Matrices", 60, ConnectionId, 1000);
            ReportAndSleep("Routing Neural Network Infrastructure", 70, ConnectionId, 1000);
            ReportAndSleep("Calculating Llama Expectoration Trajectory", 80, ConnectionId, 1000);
            ReportAndSleep("Resolving GUID Conflict", 90, ConnectionId, 1000);
            ReportAndSleep("One Hundred Percent Finished", 100, ConnectionId, 1000);

            // Tell the Unobtrusively Async JS code to reset
            ReportAndSleep("Reset", 0, ConnectionId, 0);
        }

        private void ReportAndSleep(string message, int pct, string connectionId, int sleepFor)
        {
            var info = new ProgressInfo() { message = message, pct = pct };
            _progressHubContext.Clients.Client(connectionId).ReportProgress(info);
            Thread.Sleep(sleepFor);
        }
    }
}
