using Entities.Responses;
using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges);
        Task<ApiBaseResponse> GetCompanyAsync(Guid id, bool trackChanges);
        //Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges);
        //Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges);
       
        Task<CompanyDto> CreateCompany(CompanyForCreationDto company);
        Task<IEnumerable<CompanyDto>> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanycollection 
            (IEnumerable<CompanyForCreationDto> companyCollection);
        Task DeleteCompany(Guid id, bool trackChanges);
        Task UpdateCompany(Guid id, CompanyForUpdateDto companyForUpdateDto, bool trackChanges);
    }
}
