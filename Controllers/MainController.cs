using ExecWindowsService.Singletons;
using Microsoft.AspNetCore.Mvc;

namespace ExecWindowsService.Controllers
{
    public class ExecBody
    {
        public string accessToken { get; set; }
        public string path { get; set; }
    }

    [Route("/")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IProcessManager m_processManager;
        private readonly IConfiguration m_configuration;

        public MainController(IProcessManager processManager, IConfiguration _configuration)
        {
            m_processManager = processManager;
            m_configuration = _configuration;
        }

        [HttpPost("exec")]
        public IActionResult exec([FromBody] ExecBody execBody)
        {
            try
            {
                if (m_configuration["accessToken"] != execBody.accessToken)
                    throw new Exception($"Invalid token.");
                var pid = m_processManager.StartProcess(execBody.path);
                return Ok();
            } 
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
