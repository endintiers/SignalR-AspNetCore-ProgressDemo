using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildMouse.Unearth.SignalRProgressDemo.SignalR
{
    public interface INoPostHub : IProgressHub
    {
        Task StartLongRunningProcessAsync(bool throwEx);
        Task ProcessCompleted(ProgressInfo info);
        Task SomethingWentWrong(string message);
    }
}
