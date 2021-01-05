using Microsoft.Extensions.Configuration;


namespace TestApplicationHttps.Trash
{


    public class TrashHostBuilder
    {

        public static Microsoft.Extensions.Hosting.IHostBuilder CreateHostBuilder()
        {
            // Microsoft.AspNetCore.Server.IIS
            string dir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);

            IConfigurationRoot hostConfig = new ConfigurationBuilder()
                // .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .SetBasePath(dir)
                .AddJsonFile("hosting.json", optional: false, reloadOnChange: true)
                .Build();

            // https://stackoverflow.com/questions/54461422/iconfiguration-getsection-as-properties-returns-null
            IConfigurationSection sect = hostConfig.GetSection("Logging");
            System.Console.WriteLine(hostConfig.GetSection("Kestrel:EndPoints:Http:Url").Value);


            return null;
        }

    }
}
