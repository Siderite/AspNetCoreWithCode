using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NetCore11c
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            var logger=loggerFactory.CreateLogger("Sample app logger");
            loggerFactory.AddConsole();
            logger.LogInformation("Starting app");
            app.UseStaticFiles();

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages(/*context=>{
                throw new Exception($"Page return status code: {context.HttpContext.Response.StatusCode}");
            }*/);

            var defaultHandler = new RouteHandler(
                c => c.Response.WriteAsync($"Default handler! Route values: {string.Join(", ", c.GetRouteData().Values)}")
            );

            var routeBuilder = new RouteBuilder(app, defaultHandler);

            routeBuilder.Routes.Add(new Route(new HelloRouter(), "hello/{name:alpha?}",
                    app.ApplicationServices.GetService<IInlineConstraintResolver>()));

            routeBuilder.MapRoute("Track Package Route",
                "package/{operation:regex(track|create|detonate)}/{id:int}");

            var router = routeBuilder.Build();
            app.UseRouter(router);

            /*app.Run(context =>
            {
                return context.Response.WriteAsync("Hello from ASP.NET Core!");
            });*/
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

       public class HelloRouter : IRouter
        {
            public VirtualPathData GetVirtualPath(VirtualPathContext context)
            {
                return null;
            }

            public Task RouteAsync(RouteContext context)
            {
                var name = context.RouteData.Values["name"] as string;
                if (String.IsNullOrEmpty(name)) name="World";

                var requestPath = context.HttpContext.Request.Path;
                if (requestPath.StartsWithSegments("/hello", StringComparison.OrdinalIgnoreCase))
                {
                    context.Handler = async c =>
                    {
                        await c.Response.WriteAsync($"Hi, {name}!");
                    };
                }
                return Task.FromResult(0);
            }
        }
    }
}