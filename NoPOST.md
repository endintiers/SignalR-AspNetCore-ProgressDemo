## No POST pattern walk-through

This [sample code](https://github.com/endintiers/SignalR-AspNetCore-ProgressDemo) shows two alternate patterns for server->browser async reporting of long-running task results using SignalR core.

This walk-through shows how the No POST pattern (nopost.cshtml and friends) works in an ASP.NET core2 Razor Page. This sample is worked-up somewhat - a little more complex and 'real' with multiple message types.

There is a separate [walk-through of the Async POST pattern](https://github.com/endintiers/SignalR-AspNetCore-ProgressDemo/blob/master/AsyncPOST.md) (simplified: a better place to start).    

### The Hub

All code in different files under the SignalR folder in the solution (combined here for clarity).  The Interface(s) define what messages can be sent, only messages *sent by the client*s need to be implemented in the hub. As you can see the only implementation here is of StartLongRunningProcessAsync.

```c#
    public class ProgressInfo
    {
        public string message { get; set; }
        public int pct { get; set; }
    }

    public class LongRunningTaskParameters
    {
        public bool ThrowEx { get; set; }
        public string SomeOtherStuff { get; set; }
    }

    public interface IProgressHub
    {
        Task ReportProgress(ProgressInfo info);
    }

    public interface INoPostHub : IProgressHub
    {
        Task StartLongRunningProcessAsync(bool throwEx);
        Task ProcessCompleted(ProgressInfo info);
        Task SomethingWentWrong(string message);
    }

    public class NoPostHub : Hub<INoPostHub>
    {
        public async Task StartLongRunningProcessAsync(LongRunningTaskParameters parms)
        {
            await ProgressHelper.SomeLongRunningTask(this, parms);
        }
    }
```

### At Startup

In Startup.cs. Wire up and start the NoPostHub.             

```c#
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddSignalR();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.UseSignalR(routes =>
    {
      routes.MapHub<NoPostHub>("nopo");
      ...
    });
}
```

### In the page

The **connection** variable is global - this reference is used later to start the long running process on the server.      

On document ready (called only once - after the initial GET) a connection to the SignalR hub is created, and a functions are defined to handle reportprogress, processcompleted and somethingwentwrong messages. The connection is then started and the page displayed.

```html
(in _Layout.cshtml)
<script src="~/js/signalr-client-1.0.0-alpha2-final.min.js"></script>
```

```javascript
(scripts section of NoPost.cshtml)
        var connection;

        $('document').ready(function () {
            connection = new signalR.HubConnection('/nopo');
            connection.on('reportprogress', info => {
                console.log(info.message + ' - ' + info.pct + '%');
                $('#pbar').css('width', info.pct + '%')
                    .attr('aria-valuenow', info.pct).text(info.message);
            });
            connection.on('processcompleted', info => {
                console.log('Process Completed - ' + info.message);
                $('#msg').html('Process Completed - ' + info.message);
                resetUI();
            });
            connection.on('somethingwentwrong', message => {
                console.log(message);
                $('#msg').html('<span class="text-danger">' + message + '</span>');
                resetUI();
            });
            connection.start();
        });
```

When the Progress button is clicked, it calls the **startLongRunningProcess** function which sets the state of the Progress to loading (so it can't be clicked again) then calls the server's ProgressHelper.SomeLongRunningTask method via the hub passing in any parameters we need. We don't have to retrieve or pass the connection Id because the hub already knows which client is making this call. 

**resetUI** just resets the UI state - called after a processcompleted or somethingwentwrong message is received from the server.

```javascript
        function startLongRunningProcess() {
            $('#progressButton').button('loading');
            $('#msg').html('');
            var throwEx = $('#throwExCheckBox').prop('checked');
            connection.invoke('startlongrunningprocessasync', { ThrowEx: throwEx, SomeOtherStuff: "blah"} );
        }

        function resetUI() {
            $('#pbar').css('width', '0%').attr('aria-valuenow', 0).text('');
            $('#progressButton').button('reset');
        }
```

Whenever the long running task on the server wishes to report back to the client, it calls reportprogress on the hub, which sends a progressinfo message to the client which is displayed in the progress bar.

### In the Page code-behind

Nothing to see here. No binding. No POST. Only a GET.

```c#
(NoPost.cshtml)
	public class NoPostModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Click the Progress button to see some async progress reporting - No Form, No POST, just SignalR";
        }
    }
```

### In the Hub and Helper

When the hub's **StartLongRunningProcessAsync** method is called (because of the message from the client) it calls a helper method passing the a hub reference that contains the context of this call and whatever parameters are defined.

```c#
(NoPostHub.cs)
    public async Task StartLongRunningProcessAsync(LongRunningTaskParameters parms)
    {
      await ProgressHelper.SomeLongRunningTask(this, parms);
    }
```

The actual long running task is executed, reporting back the page periodically using ReportProgress messages. When finished it sends a ProcessCompleted message to the client/page. If an exception is thrown the message is sent back to the page in a SomethingWentWrong message. Simples :-).

```C#
(ProgressHelper.cs)
  public class ProgressHelper
  {
    public static async Task SomeLongRunningTask(NoPostHub noPostHub,
                                                 LongRunningTaskParameters parms)
    {
      try
      {
        ReportAndSleep(noPostHub, "Starting Out", 10, 1000);
        ...
        if (parms.ThrowEx) { throw new ApplicationException("Something bad happened"); }
        ...
        ReportAndSleep(noPostHub, "One Hundred Percent Finished", 100, 1000);

        // Tell the js code we finished
        var info = new ProgressInfo() { message = "All messages sent OK", pct = 0 };
        await noPostHub.Clients.Client(noPostHub.Context.ConnectionId).ProcessCompleted(info);
      }
      catch (Exception ex)
      {
        await noPostHub.Clients.Client(noPostHub.Context.ConnectionId)
          .SomethingWentWrong(ex.Message);
      }
    }

    private static void ReportAndSleep(NoPostHub noPostHub, string message, int pct, int sleepFor)
    {
      var info = new ProgressInfo() { message = message, pct = pct };
      noPostHub.Clients.Client(noPostHub.Context.ConnectionId).ReportProgress(info);
      Thread.Sleep(sleepFor);
    }
  }

```

