using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationalController : Controller
    {
        private readonly IConfiguration configuration;

        public OperationalController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpOptions("reloadconfig")]
        public IActionResult ReloadConfig()
        {
            try
            {
                var root = configuration as IConfigurationRoot;
                root.Reload();
                return Ok(root.Providers);

            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
