using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Extensions;
namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        ////could be like this for data of milions records to be better 
        //public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
        //    EmployeeParameters employeeParameters, bool trackChanges)
        //{
        //    var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
        //     .OrderBy(e => e.Name)
        //     .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
        //     .Take(employeeParameters.PageSize)
        //     .ToListAsync();
        //    var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
        //    .CountAsync();
        //    return new PagedList<Employee>(employees,count, employeeParameters.PageNumber, employeeParameters.PageSize);
        //}

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
            EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId) , trackChanges)
            //&&(e.Age>= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm)
            .Sort(employeeParameters.OrderBy)
            .ToListAsync();
            return PagedList<Employee>
                .ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
        }
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee) {
            employee.CompanyId = companyId;
            Create(employee);
        }
        public void DeleteEmployee(Employee employee) => Delete(employee);


    }
}
