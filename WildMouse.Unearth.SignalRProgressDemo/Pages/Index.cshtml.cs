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

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Click the Progress button to see some async progress reporting";
        }

        public void OnPost()
        {
            ReportAndSleep("Starting Out", 10);
            ReportAndSleep("Getting Along", 20);
            ReportAndSleep("Getting Along Nicely", 30);
            ReportAndSleep("Just Cruising", 40);
            ReportAndSleep("Half Way!", 50);
            ReportAndSleep("The Hump", 60);
            ReportAndSleep("Working Away", 70);
            ReportAndSleep("Entering The Home Stretch", 80);
            ReportAndSleep("Almost Done", 90);
            ReportAndSleep("All Done Now", 100);

            Message = "Click the About menu item for an explanation and source code link";
        }

        private void ReportAndSleep(string message, int pct)
        {
            var info = new ProgressInfo() { message = message, pct = pct };
            _progressHubContext.Clients.All.ReportProgress(info);
            Thread.Sleep(1000);
        }
    }
}
