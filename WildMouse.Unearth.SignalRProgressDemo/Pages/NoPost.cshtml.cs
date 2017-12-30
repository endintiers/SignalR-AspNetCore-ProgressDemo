using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace WildMouse.Unearth.SignalRProgressDemo.Pages
{
    public class NoPostModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Click the Progress button to see some async progress reporting - No Form, No POST, just SignalR";
        }
    }
}
