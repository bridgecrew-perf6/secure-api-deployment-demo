using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Companies.Contracts;
using Demo.Core.Companies.DomainModel;
using Demo.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demo.Data.Repositories
{
    public sealed class CompaniesRepository:ICompaniesRepository
    {
        private readonly DbContextOptions _dbContextOptions;

        public CompaniesRepository(DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }



        public async Task<List<Company>> List()
        {
            await using var context = new DemoDataContext(_dbContextOptions);

            
            var companies = context.Companies.Select(e=> new Company(
                e.Id,
                e.Name,
                e.Address,
                e.City,
                e.Zip,
                e.Country,
                e.IsEnabled,
                e.ReceivesNewsletter,
                e.IsDeleted,
                e.CreatedOn,
                e.ModifiedOn));
            return await companies.ToListAsync();
        }

        public async Task<bool> Update(Company company)
        {
            await using var context = new DemoDataContext(_dbContextOptions);

            var companyEntity = await context.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
            company.ToEntity(companyEntity);
            return await context.SaveChangesAsync() > 0;
        }

        public Task<Company> Get(Guid dtoId)
        {
            throw new NotImplementedException();
        }
    }
}
