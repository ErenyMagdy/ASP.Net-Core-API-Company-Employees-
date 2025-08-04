using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId, LinkParameters linkParameters, bool trackChanges);
         Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
         Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employee, bool trackChanges);
         Task DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges);
         Task UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool companyTrackChanges, bool employeeTrackChanges);
         Task<(EmployeeForUpdateDto employeeToPatch, Employee employee)> GetEmployeeForPatch(
            Guid companyId, Guid id, bool companyTrackChanges, bool employeeTrackChanges);
        Task SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employee);
    }
}
