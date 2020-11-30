using Microsoft.AspNetCore.Mvc;
using OfficialUnofficialRing.Contracts;
using RingSharp;
using RingSharp.Models;
using System.Threading.Tasks;

namespace OfficialUnofficialRing.Controllers
{
    [ApiController]
    [Route("snapshot")]
    public sealed class SnapshotController : ControllerBase
    {
        private readonly IRingClient _ringClient;

        public SnapshotController(IRingClient ringClient)
            => this._ringClient = ringClient;

        [HttpGet]
        [Route("device/{deviceId:int}")]
        public async Task<ActionResult<Snapshot>> GetSnapshotAsync(int deviceId)
        {
            var response = await this._ringClient.GetSnapshotAsync(deviceId).ConfigureAwait(true);
            return this.Ok(response);
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UpdateSnapshotsAsync(UpdateSnapshotsRequest request)
        {
            await this._ringClient.UpdateSnapshotAsync(request.DeviceIds).ConfigureAwait(true);
            return this.Ok();
        }
    }
}