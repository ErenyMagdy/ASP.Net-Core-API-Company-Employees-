using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges) =>
           await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();
        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            try
            {
                return await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync();
           
            }

            catch (Exception ex)
            {
                return null;
            }
        }
        public void CreateCompany(Company company) =>
            Create(company);
        public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
            await FindByCondition(c => ids.Contains(c.Id), trackChanges).ToListAsync();
        public void DeleteCompany(Company company) => Delete(company);
    }
}
