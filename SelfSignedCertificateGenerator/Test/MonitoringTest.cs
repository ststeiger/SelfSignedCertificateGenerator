
namespace SelfSignedCertificateGenerator.Test
{


    public class MonitoringTest
    {


        public static void TestMonitorChanges()
        {
            using (RegistryUtils.RegistryMonitor mon = MonitorChanges())
            {
                System.Console.WriteLine(" --- Press any key to stop monitoring --- ");
                System.Console.ReadKey();

                mon.Stop();
            }
        }


        // https://www.dreamincode.net/forums/topic/245219-monitor-registry-changes/
        // https://stackoverflow.com/questions/34199566/is-there-a-way-to-monitor-registry-changes
        public static RegistryUtils.RegistryMonitor MonitorChanges()
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return null;

#if true // MONITOR_REGISTRY

            RegistryUtils.RegistryMonitor mon = new RegistryUtils.RegistryMonitor(
                  Microsoft.Win32.RegistryHive.CurrentUser
                , @"Software\COR\All\ssl_cert"
            );

            mon.RegChangeNotifyFilter = RegistryUtils.RegChangeNotifyFilter.Value;

            mon.RegChanged += delegate (object sender, System.EventArgs e)
            {
                System.Console.WriteLine("registry key changed...");
                System.Console.WriteLine(e.GetType().FullName);
            };

            mon.Error += delegate (object sender, System.IO.ErrorEventArgs e)
            {
                System.Console.WriteLine(e.GetType().FullName);
            };

            mon.Start();
            // mon.Stop();

            return mon;
#else
            return null;
#endif
        }


    }
}
