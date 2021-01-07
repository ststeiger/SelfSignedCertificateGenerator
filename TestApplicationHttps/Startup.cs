
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace TestApplicationHttps
{


    public class Startup
    {

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }




        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<KestrelServerOptions>, KestrelOptionsSetup>(); // Why transient ? 


            //Microsoft.AspNetCore.Hosting.Server.IServer server = services.BuildServiceProvider()
            //    .GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>();

            //Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature serverFeatures = 
            //    server.Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();
            //if (serverFeatures.Addresses.Count == 0)
            //{
            //    // ListenOn(DefaultAddress); // Start the server on the default address
            //    // serverFeatures.Addresses.Add(DefaultAddress) // Add the default address to the IServerAddressesFeature
            // }
            

            services.AddHttpsRedirection(options =>
            {
                bool useKestrel = true;

                // https://stackoverflow.com/questions/42272021/check-if-asp-netcore-application-is-hosted-in-iis
                // https://docs.microsoft.com/en-us/previous-versions/iis/6.0-sdk/ms524602%28v%3Dvs.90%29
                if (System.Environment.GetEnvironmentVariable("APP_POOL_ID") is string)
                    useKestrel = false;

                options.RedirectStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status308PermanentRedirect;

                if (useKestrel || !System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    PseudoUrl url = this.Configuration.GetValue<string>("Kestrel:EndPoints:Https:Url", null);
                    if (url != null)
                        options.HttpsPort = url.Port;
                    else
                        options.HttpsPort = 5005;
                }
                else 
                {
                    // PseudoUrl url = this.Configuration.GetValue<string>("iisSettings:iisExpress:applicationUrl", null);
                    options.HttpsPort = this.Configuration.GetValue<int>("iisSettings:iisExpress:sslPort", 443);
                } 
            });
            

            services.AddRazorPages();
        } // End Sub ConfigureServices 


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Microsoft.AspNetCore.Http.Features.FeatureCollection features = app.Properties["server.Features"] as Microsoft.AspNetCore.Http.Features.FeatureCollection;
            // Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature addresses = features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();
            // string address = System.Linq.Enumerable.First(addresses.Addresses);
            // System.Uri uri = new System.Uri(address);


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
                    | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });


            app.UseMiddleware<LetsEncryptChallengeApprovalMiddleware>();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            // https://stackoverflow.com/questions/52347936/exclude-route-from-middleware-net-core
            //app.MapWhen(
            app.UseWhen(
                delegate (Microsoft.AspNetCore.Http.HttpContext httpContext)
                {
                    // http://localhost:51851/.well-known/acme-challenge/token.txt
                    // http://localhost:51851/Privacy
                    // System.Console.WriteLine(httpContext.Connection.LocalPort);

                    return !httpContext.Request.Path.StartsWithSegments("/.well-known/acme-challenge");
                }
                ,
                delegate (IApplicationBuilder appBuilder)
                {
                    appBuilder.UseHttpsRedirection();
                }
            );
            
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        } // End Sub Configure 


    } // End Class Startup 


} // End Namespace TestApplicationHttps 
