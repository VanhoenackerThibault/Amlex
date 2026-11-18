using Amlex.Models.Models.SparqlModels;
using Amlex.Services.Sparql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Amlex.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingApiController(SparqlApiService sparqlApiService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<SparqlResponse?>> Get()
        {
            return await sparqlApiService.GetSparqlData();
        }
    }
}
