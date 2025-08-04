using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;
using System.Threading;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,
            IEmployeeLinks employeeLinks)
          //  IDataShaper<EmployeeDto> dataShaper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
            // _dataShaper = dataShaper;
        }
        public async Task<(LinkResponse linkResponse, MetaData metaData)>
            GetEmployeesAsync(Guid companyId, 
            LinkParameters linkParameters, bool trackChanges)
          //  EmployeeParameters employeeParameters, bool trackChanges)
        {
            //if (!employeeParameters.ValidAgeRange)
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            var company = await _repository.Company.GetCompanyAsync(companyId,
            trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employeesMetaData = await  _repository.Employee.GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);
            //var employeesMetaData = await  _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesMetaData);
            var links = _employeeLinks.TryGenerateLinks(employeesDto,linkParameters.EmployeeParameters.Fields, companyId, linkParameters.Context);
            //var shapedData = _dataShaper.ShapeData(employeesDto,employeeParameters.Fields);
            return (linkResponse:links, metaData: employeesMetaData.MetaData);
        }
        //public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, 
        //    EmployeeParameters employeeParameters, bool trackChanges)
        //{
        //    if (!employeeParameters.ValidAgeRange)
        //        throw new MaxAgeRangeBadRequestException();

        //    var company = await _repository.Company.GetCompanyAsync(companyId,
        //    trackChanges);
        //    if (company is null)
        //        throw new CompanyNotFoundException(companyId);
        //    var employeesMetaData = await  _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
        //    var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesMetaData);
        //    return (employeesDto,employeesMetaData.MetaData);
        //}
        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
            trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
                            trackChanges);
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return employeeDto;
        }
        public async Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
            trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employee = _mapper.Map<Employee>(employeeForCreationDto);
            _repository.Employee.CreateEmployeeForCompany(companyId, employee);
            await _repository.SaveAsync();
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return employeeDto;
        }
        public async Task DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
            trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            trackChanges);
            _repository.Employee.DeleteEmployee(employee);
            await _repository.SaveAsync();
        }
        public async Task UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool companyTrackChanges, bool employeeTrackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
            employeeTrackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            employeeTrackChanges);
            _mapper.Map(employeeForUpdateDto, employee);
            await _repository.SaveAsync();
        }
        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employee)> GetEmployeeForPatch(
          Guid companyId, Guid id, bool companyTrackChanges, bool employeeTrackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
          employeeTrackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            employeeTrackChanges);
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employee);
            return (employeeToPatch, employee);
        }
        public async Task SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employee)
        {
            _mapper.Map(employeeToPatch, employee);
            await _repository.SaveAsync();
        }
        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists
        (Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id,
            trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);
            return employeeDb;
        }

        //private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        //{

        //    bool acquired = await _semaphore.WaitAsync(TimeSpan.FromSeconds(30));
        //    try
        //    {
        //        if (!acquired)
        //        {
        //            throw new TimeoutException("Timeout waiting for semaphore in CheckIfCompanyExists.");
        //        }
        //        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        //        if (company is null)
        //            throw new CompanyNotFoundException(companyId);
        //    }
        //    finally
        //    {
        //        if (acquired)
        //            _semaphore.Release();
        //    }

        //    //await _semaphore.WaitAsync();
        //    //try
        //    //{
        //    //    var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        //    //    if (company is null)
        //    //        throw new CompanyNotFoundException(companyId);
        //    //}
        //    //finally
        //    //{
        //    //    _semaphore.Release();
        //    //}
        //}
    }
}
