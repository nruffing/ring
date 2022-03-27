using RingSharp;
using Serilog;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Validation;

namespace RingService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.json")
                .CreateLogger();

            var refreshToken = ConfigurationManager.AppSettings?["RingRefreshToken"];
            Requires.NotNullOrWhiteSpace(refreshToken, nameof(refreshToken));

            var client = new RingClient(refreshToken);

            var devices = await client.GetAllDevicesAsync().ConfigureAwait(false);
            var deviceIds = devices.Select(d => d.Id).ToArray();

            while (true)
            {
                try
                {
                    await client.UpdateSnapshotAsync(deviceIds).ConfigureAwait(false);
                    await Task.Delay(20000).ConfigureAwait(false);
                    Console.WriteLine($"HEARTBEAT: {DateTime.Now}");
                }
                catch (Exception e)
                {
                    log.Error(e, "There was an error updating snapshots.");
                }
            }
        }
    }
}