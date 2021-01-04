namespace RingSharp
{
    using RingSharp.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRingClient
    {
        Task<string> AuthenticateAsync(string username, string password, string twoFactorCode = null);

        Task<IEnumerable<Device>> GetAllDevicesAsync();

        Task UpdateSnapshotAsync(params int[] deviceIds);

        Task<Snapshot> GetSnapshotAsync(int deviceId);
    }
}