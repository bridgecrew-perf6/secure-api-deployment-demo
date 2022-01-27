using System.Threading.Tasks;
using Demo.Core.Companies.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompaniesService _service;

        public CompaniesController(ICompaniesService service)
        {
            _service = service;
        }


        public async Task<IActionResult> Get()
        {
            var models = await _service.List();
            return Ok(models);
        }

    }
}
