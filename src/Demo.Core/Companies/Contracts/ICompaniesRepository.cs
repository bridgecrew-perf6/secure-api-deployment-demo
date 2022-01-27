using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Core.Companies.DomainModel;

namespace Demo.Core.Companies.Contracts
{
    public interface ICompaniesRepository
    {
        Task<List<Company>> List();
        Task<bool> Update(Company company);
        Task<Company> Get(Guid dtoId);
    }
}