
namespace SelfSignedCertificateGenerator.Test
{
    

    /// <summary>
    /// </summary>
    public class WmiChangeEventTester
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiChangeEventTester"/> class.
        /// </summary>
        public WmiChangeEventTester()
        {
            try
            {
                // Your query goes below; "KeyPath" is the key in the registry that you
                // want to monitor for changes. Make sure you escape the \ character.
                System.Management.WqlEventQuery query = new System.Management.WqlEventQuery(
@"SELECT * FROM RegistryValueChangeEvent 
WHERE Hive = 'HKEY_LOCAL_MACHINE' 
AND KeyPath = 'SOFTWARE\\Microsoft\\.NETFramework' 
AND ValueName = 'InstallRoot' 
");

                System.Management.ManagementEventWatcher watcher = new System.Management.ManagementEventWatcher(query);
                System.Console.WriteLine("Waiting for an event...");

                // Set up the delegate that will handle the change event.
                watcher.EventArrived += new System.Management.EventArrivedEventHandler(HandleEvent);

                // Start listening for events.
                watcher.Start();

                // Do something while waiting for events. In your application,
                // this would just be continuing business as usual.
                System.Threading.Thread.Sleep(100000000);

                // Stop listening for events.
                watcher.Stop();
            }
            catch (System.Management.ManagementException managementException)
            {
                System.Console.WriteLine("An error occurred: " + managementException.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleEvent(object sender, System.Management.EventArrivedEventArgs e)
        {
            System.Console.WriteLine("Received an event.");
            // RegistryKeyChangeEvent occurs here; do something.
        }

        /// <summary>
        /// </summary>
        public static void Main()
        {
            // Just calls the class above to check for events...
            WmiChangeEventTester receiveEvent = new WmiChangeEventTester();
        }
    }
}
