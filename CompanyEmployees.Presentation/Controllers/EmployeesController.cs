using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.RequestFeatures;
using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _services;
        public EmployeesController(IServiceManager services) =>
            _services = services;
        [HttpGet]
        [HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
            [FromQuery] EmployeeParameters employeeParameters)
        {
            //var Pagedemployees = await _services.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, false);
            var linkParams = new LinkParameters(employeeParameters, HttpContext);
            var result = await _services.EmployeeService.GetEmployeesAsync(companyId,
            linkParams, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) :
                Ok(result.linkResponse.ShapedEntities);
        }
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employees = await _services.EmployeeService.GetEmployeeAsync(companyId, id, false);
            return Ok(employees);
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreationDto)
        {
            var employee = await _services.EmployeeService.CreateEmployeeForCompany(companyId, employeeForCreationDto, true);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employee.Id }, employee);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmplyeeForCompany(Guid companyId, Guid id)
        {
            await _services.EmployeeService.DeleteEmployeeForCompany(companyId, id, false);
            return NoContent();
        }
        ////https://localhost:5001/api/companies/3d490a70-94ce-4d15-9494-5248280c2ce3/employees/021ca3c1-0deb-4afd-ae94-2159a8479811

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task <IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id,
            EmployeeForUpdateDto employeeForUpdateDto)
        {
            await _services.EmployeeService.UpdateEmployeeForCompany(companyId, id, employeeForUpdateDto,
                companyTrackChanges: false, employeeTrackChanges: true);
            return NoContent();
        }
        //https://localhost:5001/api/companies/de652e63-4594-4440-adc2-08dd50d1c0a5/employees/81554fbc-657b-47b0-028b-08dd50d1c0c2
        //must put the content type as application/json-patch+json in postman for test
        /* request body example
         [
            {
                "op":"replace",
                "path":"/age",
                "value":"44"
            }
        ]

         */
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDocument)
        {
            if (patchDocument is null)
                return BadRequest("patchDoc object sent from client is null.");
            var result = await _services.EmployeeService.GetEmployeeForPatch(companyId, id,
                companyTrackChanges: false, employeeTrackChanges: true);
            patchDocument.ApplyTo(result.employeeToPatch, ModelState);
            TryValidateModel(result.employeeToPatch);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _services.EmployeeService.SaveChangesForPatch(result.employeeToPatch, result.employee);
            return NoContent();
        }

    }
}
