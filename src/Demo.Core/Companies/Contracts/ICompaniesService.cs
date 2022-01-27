using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Core.Companies.DataTransferObjects;

namespace Demo.Core.Companies.Contracts
{
    public interface ICompaniesService
    {
        Task<List<CompanyListItemDto>> List();
        Task<bool> Update(CompanyDetailsDto dto);
    }
}