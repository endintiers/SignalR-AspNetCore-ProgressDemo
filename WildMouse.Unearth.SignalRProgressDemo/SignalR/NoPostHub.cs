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
        public async Task StartLongRunningProcessAsync(LongRunningTaskParameters parms)
        {
            await ProgressHelper.SomeLongRunningTask(this, parms);
        }

        //public Task SomethingWentWrong(string message)
        //{
        //    return Clients.Client(this.Context.ConnectionId).SomethingWentWrong(message);
        //}

        //// This is only called if a CLIENT invokes reportprogress
        //// (so never in the NoPost.cshtml example)
        //public Task ReportProgress(ProgressInfo info)
        //{
        //    return Clients.Client(this.Context.ConnectionId).ReportProgress(info);
        //}
    }
}
