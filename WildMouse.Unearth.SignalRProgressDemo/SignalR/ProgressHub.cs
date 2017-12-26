using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildMouse.Unearth.SignalRProgressDemo
{
    public class ProgressHub : Hub<IProgressHub>
    {
        public Task ReportProgress(ProgressInfo info)
        {
            return Clients.All.ReportProgress(info);
        }
    }
}
