using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

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
            Message = "Click the Progress button to see some async progress reporting";
        }

        public void OnPost()
        {
            ReportAndSleep("Starting Out", 10, ConnectionId);
            ReportAndSleep("Getting Along", 20, ConnectionId);
            ReportAndSleep("Getting Along Nicely", 30, ConnectionId);
            ReportAndSleep("Just Cruising", 40, ConnectionId);
            ReportAndSleep("Half Way!", 50, ConnectionId);
            ReportAndSleep("The Hump", 60, ConnectionId);
            ReportAndSleep("Working Away", 70, ConnectionId);
            ReportAndSleep("Entering The Home Stretch", 80, ConnectionId);
            ReportAndSleep("Almost Done", 90, ConnectionId);
            ReportAndSleep("All Done Now", 100, ConnectionId);

            Message = "Click the About menu item for an explanation and source code link";
        }

        private void ReportAndSleep(string message, int pct, string connectionId)
        {
            var info = new ProgressInfo() { message = message, pct = pct };
            _progressHubContext.Clients.Client(connectionId).ReportProgress(info);
            Thread.Sleep(1000);
        }
    }
}
