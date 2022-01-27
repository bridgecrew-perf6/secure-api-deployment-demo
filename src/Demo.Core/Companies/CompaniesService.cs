using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Companies.Contracts;
using Demo.Core.Companies.DataTransferObjects;

namespace Demo.Core.Companies
{
    public sealed class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _repository;

        public CompaniesService(ICompaniesRepository repository)
        {
            _repository = repository;
        }


        public async Task<List<CompanyListItemDto>> List()
        {
            var companies = await _repository.List();
            return companies.Select(dm => new CompanyListItemDto
            {
                Id = dm.Id,
                Address = dm.Address,
                City = dm.City,
                Country = dm.Country,
                Name = dm.Name,
                Zip = dm.Zip
            }).ToList();
        }

        public async Task<bool> Update(CompanyDetailsDto dto)
        {
            var company = await _repository.Get(dto.Id);
            company.SetName(dto.Name);
            return await _repository.Update(company);
        }
    }
}