using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace NetCore11c
{
    class Program
    {
        static void Main(string[] args)
        {
            var path=Path.Combine(Directory.GetCurrentDirectory(),"www");
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseWebRoot(path)
                .Build();
            host.Run();
        }
    }
}
