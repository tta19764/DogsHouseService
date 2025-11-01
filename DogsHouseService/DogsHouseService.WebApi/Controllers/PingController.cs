using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogsHouseService.WebApi.Controllers
{

    /// <summary>
    /// Controller for health check and version information.
    /// </summary>
    /// <param name="configuration"></param>
    [Route("[controller]")]
    [ApiController]
    public class PingController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        /// Health check endpoint that returns application name and version.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            var version = configuration["AppSettings:Version"] ?? "1.0.0";
            var appName = configuration["AppSettings:ApplicationName"] ?? "Appservice";
            return Ok($"{appName}.Version{version}");
        }
    }
}
