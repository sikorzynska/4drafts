using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(_4drafts.Areas.Identity.IdentityHostingStartup))]
namespace _4drafts.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}