using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Kapasitematik_TakimOmru_v3.Startup))]

namespace Kapasitematik_TakimOmru_v3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            hubConfiguration.EnableJavaScriptProxies = true;

            app.MapSignalR("/signalr", hubConfiguration);
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
