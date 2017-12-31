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
    }
}
