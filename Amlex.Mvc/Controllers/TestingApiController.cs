using Amlex.Interfaces.Services;
using Amlex.Models.Models.SparqlModels;
using Amlex.Services.Sparql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Amlex.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingApiController(SparqlApiService sparqlApiService, ILegalActService legalActService) : ControllerBase
    {
        //[HttpGet]
        //public async Task<ActionResult<SparqlResponse?>> Get()
        //{
        //    return await sparqlApiService.GetSparqlDataAsync();
        //}

        [HttpGet]
        public async Task<ActionResult<List<SparqlBinding>?>> BackFill()
        {
            var result = await sparqlApiService.RunBackFillSparqlDataAsync(maxPages: 999);

            if (result is null)
            {
                return NoContent();
            }

            await legalActService.CreateLegalActsAsync(result);

            return result;
        }
    }
}
