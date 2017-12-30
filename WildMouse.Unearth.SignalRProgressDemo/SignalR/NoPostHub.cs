using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WildMouse.Unearth.SignalRProgressDemo.Helpers;

namespace WildMouse.Unearth.SignalRProgressDemo.SignalR
{
    public class NoPostHub : Hub<INoPostHub>
    {
        public async Task StartLongRunningProcessAsync()
        {
            await ProgressHelper.SomeLongRunningTask(this);
        }

        // This is only called if a CLIENT invokes reportprogress
        // (so never in the NoPost.cshtml example)
        public Task ReportProgress(ProgressInfo info)
        {
            return Clients.Client(this.Context.ConnectionId).ReportProgress(info);
        }
    }
}
