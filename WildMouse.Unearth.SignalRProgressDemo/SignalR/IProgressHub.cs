using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WildMouse.Unearth.SignalRProgressDemo.SignalR
{
    public interface IProgressHub
    {
        Task ReportProgress(ProgressInfo info);
    }
}
