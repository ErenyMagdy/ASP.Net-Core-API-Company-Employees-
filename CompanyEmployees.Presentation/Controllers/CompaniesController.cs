
using CompanyEmployees.Presentation.Extenstions;
using CompanyEmployees.Presentation.ModelBinders;
using Entities.Responses;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    // [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public class CompaniesController : ApiControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager service) =>
            _service = service;
        /// <summary>
        /// GETS THE LIST OF ALL Companies 
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCompanies")]
        [Authorize]
        public async Task<IActionResult> GetCompanies()
        {
            //var baseResult = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            //var companies = baseResult.GetResult<IEnumerable<CompanyDto>>();
            var companies = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
        [HttpGet("{id:guid}", Name ="CompanybyId")]
        // [ResponseCache(Duration = 60)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge =60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var baseResult = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            if (!baseResult.Success)
                return ProcessError(baseResult);
            var company = baseResult.GetResult<CompanyDto>();
            // var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }
        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name = "CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            //if (company is null)
            //    return BadRequest("CompanyForCreationDto object is null");
            var createdCompany = await _service.CompanyService.CreateCompany(company);
            return CreatedAtRoute("CompanyById", new {id=createdCompany.Id},createdCompany);
        }
        [HttpGet("collection/({ids})", Name ="CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder (BinderType =
            typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIds(ids,trackChanges:false);
            return Ok(companies);
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanycollection(companyCollection);
            return CreatedAtRoute("CompanyCollection", new {result.ids},result.companies);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompany(id, trackChanges: false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody]CompanyForUpdateDto companyForUpdateDto) {
            //if (companyForUpdateDto is null)
             //   return BadRequest("CompanyForUpdateDto object is null");
            await _service.CompanyService.UpdateCompany(id, companyForUpdateDto, trackChanges: true);
            return NoContent();
        }
        //[HttpGet("collection2/({ids}", Name = "TestEndpoint")]
        //public IActionResult TestEndpoint(string ids)
        //{
        //    return Ok("Test endpoint reached.");
        //}
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

    }
}
