
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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
            services.AddHttpsRedirection(options =>
            {
                bool useKestrel = true;

                if (useKestrel || !System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    options.HttpsPort = 5005;
                else // options.HttpsPort = 443;
                    options.HttpsPort = 44322;    
            });
            

            services.AddRazorPages();
        } // End Sub ConfigureServices 


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
                    | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });

            
            
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
