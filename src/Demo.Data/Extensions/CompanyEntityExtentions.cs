using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Core.Companies.DomainModel;
using Demo.Data.Entities;

namespace Demo.Data.Extensions
{
    public static class CompanyEntityExtentions
    {

        public static CompanyEntity ToEntity(this Company input, CompanyEntity existing)
        {
            if (input == null)
            {
                return null;

            }
            var entity = existing ?? new CompanyEntity
            {
                Id = input.Id,
                CreatedOn = input.CreatedOn
            };
            entity.Name = input.Name;
            entity.Address = input.Address;
            entity.Zip = input.Zip;
            entity.City = input.City;
            entity.Country = input.Country;
            entity.IsEnabled = input.IsEnabled;
            entity.IsDeleted = input.IsDeleted;
            entity.ReceivesNewsletter = input.ReceivesNewsletter;
            entity.ModifiedOn = input.ModifiedOn;
            return entity;
        }

    }
}
