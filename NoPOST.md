## No POST pattern walk-through

This [sample code](https://github.com/endintiers/SignalR-AspNetCore-ProgressDemo) shows two alternate patterns for server->browser async reporting of long-running task results using SignalR core.

This walk-through shows how the No POST pattern (nopost.cshtml and friends) works in an ASP.NET core2 Razor Page.

There is a separate [walk-through of the Async POST pattern](https://github.com/endintiers/SignalR-AspNetCore-ProgressDemo/blob/master/AsyncPOST.md).      

### The Hub

All code in different files under the SignalR folder in the solution (combined here for clarity). You can make ProgressInfo (the message definition) as complex as you like (as long as it's serializable). The INoPostHub Interface allows for injection later (in Page Model construction).

```c#
public class ProgressInfo
{
    public string message { get; set; }
    public int pct { get; set; }
}

public interface IProgressHub
{
    Task ReportProgress(ProgressInfo info);
}

public interface INoPostHub : IProgressHub
{
	Task StartLongRunningProcessAsync();
}

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

The **connection** variable is global - this reference is used later to retrieve the connection id.      

On document ready (called only once - after the initial GET) a connection to the SignalR hub is created, and a function is defined to handle reportprogress messages. The connection is then started and the page displayed.

```html
(in _Layout.cshtml)
<script src="~/js/signalr-client-1.0.0-alpha2-final.min.js"></script>
```

```javascript
(scripts section of NoPost.cshtml)
        var connection;
        var progressTimeout;

        $('document').ready(function () {
            connection = new signalR.HubConnection('/nopo');
            connection.on('reportprogress', info => {
                console.log(info.message + ' - ' + info.pct + '%');
                reportProgress(info);
            });
            connection.start();
        });
```

When the Progress button is clicked, it calls the startLongRunningProcess function which sets the state of the Progress to loading (so it can't be clicked again) then calls the server's ProgressHelper.SomeLongRunningTask method via the hub. We don't have to retrieve or pass the connection Id because the hub already knows which client is making this call.

The **progressTimeout** is a simplified/arbitrary way of dealing with server-side issues. In a real app we would catch server-side exceptions and report problems back to the client via reportprogress or some other message.

**resetUI** just resets the UI state - after a timeout or when a reset message is received from the server.

```javascript
        function startLongRunningProcess() {
            $('#progressButton').button('loading');
            progressTimeout = setTimeout(resetUI, 30000);
            connection.invoke('startlongrunningprocessasync');
        }

        function resetUI() {
            $('#pbar').css('width', '0%').attr('aria-valuenow', 0).text('');
            $('#progressButton').button('reset');
        }
```

Whenever the long running task on the server wishes to report back to the client, it calls reportprogress on the hub, which sends a progressinfo message to the client which is passed to the reportProgress function.

 If a reset message is passed by the server the UI is reset and the progress timeout is cleared.      

```javascript
        function reportProgress(info) {
            if (info.message.toLowerCase() == 'reset') {
                clearTimeout(progressTimeout);
                resetUI();
            }
            else {
                $('#pbar').css('width', info.pct + '%')
                    .attr('aria-valuenow', info.pct).text(info.message);
            }
        }
```

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

When the hub's StartLongRunningProcessAsync method is called (because of the message from the client) it calls a helper method passing the a hub reference that contains the context of this call.

```c#
(NoPostHub.cs)
	public async Task StartLongRunningProcessAsync()
    {
    	await ProgressHelper.SomeLongRunningTask(this);
    }
```

The actual long running task is executed, reporting back the page periodically. When finished it sends a reset message to the client/page.

```C#
(ProgressHelper.cs)
    public class ProgressHelper
    {
        public static async Task SomeLongRunningTask(NoPostHub noPostHub)
        {
            ReportAndSleep(noPostHub, "Starting Out", 10, 1000);
            ...

            // Tell the js code to reset
            ReportAndSleep(noPostHub, "Reset", 0, 0);
        }

        private static void ReportAndSleep(NoPostHub noPostHub, string message, int pct, int sleepFor)
        {
            var info = new ProgressInfo() { message = message, pct = pct };
            noPostHub.Clients.Client(noPostHub.Context.ConnectionId).ReportProgress(info);
            Thread.Sleep(sleepFor);
        }
    }
```

In the real world we would define extra message types. The reset should not really be a special case of ReportProgress - it should be a special 