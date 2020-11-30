using Microsoft.AspNetCore.Mvc;
using RingSharp;
using RingSharp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficialUnofficialRing.Controllers
{
    [ApiController]
    [Route("device")]
    public sealed class DeviceController : ControllerBase
    {
        private readonly IRingClient _ringClient;

        public DeviceController(IRingClient ringClient)
            => this._ringClient = ringClient;

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesAsync()
        {
            var response = await this._ringClient.GetAllDevicesAsync().ConfigureAwait(true);
            return this.Ok(response);
        }
    }
}