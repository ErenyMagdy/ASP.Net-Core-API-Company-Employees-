using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Entities.Responses;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class CompanyService :ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges){
                var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
                var companiesDto =_mapper.Map<IEnumerable<CompanyDto>>(companies);
                return new ApiOkResponse<IEnumerable<CompanyDto>>(companiesDto);
        }
        public async Task<ApiBaseResponse> GetCompanyAsync(Guid id, bool trackChanges){

            //var company = await GetCompanyAndCheckIfItExists(id, trackChanges);
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
            if (company is null)
                return new CompanyNotFoundResponse(id);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return  new ApiOkResponse<CompanyDto>(companyDto);
        }
        public async Task<CompanyDto> CreateCompany(CompanyForCreationDto companyForCreationDto)
        {
            var company = _mapper.Map<Company>(companyForCreationDto);
            _repository.Company.CreateCompany(company);
           await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(company);
            return companyToReturn;
        }
        public async Task<IEnumerable<CompanyDto>> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            if(ids is null)
                throw new IdParametersBadRequestException();
            var companies = await _repository.Company.GetByIds(ids, trackChanges);
            if (ids.Count() != companies.Count())
                throw new CollectionByIdsBadRequestException();
            var companiesDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return companiesDtos;
        }
        public async Task< (IEnumerable<CompanyDto> companies, string ids)> CreateCompanycollection
          (IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection is null)
                throw new CompanyCollectionBadRequest();
            var companies = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach(var company in companies)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();
            var companiesDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            var ids = string.Join(",",companiesDtos.Select(company => company.Id));
            return (companiesDtos, ids);
        }
        public async Task DeleteCompany(Guid id, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(id, trackChanges);
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }
        public async Task UpdateCompany(Guid id, CompanyForUpdateDto companyForUpdateDto, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExists(id, trackChanges);
            _mapper.Map(companyForUpdateDto, company);
            await _repository.SaveAsync();
        }

        private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(id);
            return company;
        }
    }
}
