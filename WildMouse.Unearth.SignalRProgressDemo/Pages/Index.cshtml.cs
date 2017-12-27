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
            ReportAndSleep("Getting Along", 20, ConnectionId, 1000);
            ReportAndSleep("Getting Along Nicely", 30, ConnectionId, 1000);
            ReportAndSleep("Just Cruising", 40, ConnectionId, 1000);
            ReportAndSleep("Half Way!", 50, ConnectionId, 1000);
            ReportAndSleep("The Hump", 60, ConnectionId, 1000);
            ReportAndSleep("Working Away", 70, ConnectionId, 1000);
            ReportAndSleep("Entering The Home Stretch", 80, ConnectionId, 1000);
            ReportAndSleep("Almost Done", 90, ConnectionId, 1000);
            ReportAndSleep("All Done Now", 100, ConnectionId, 1000);

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
