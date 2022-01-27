using System;
using Demo.Core.Base;
using Demo.Core.Companies.Exceptions;

namespace Demo.Core.Companies.DomainModel
{
    public sealed class Company : DomainModelBase<Guid>
    {
        public string Name { get; private set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool IsEnabled { get; set; }
        public bool ReceivesNewsletter { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }


        public void SetName(string value)
        {
            SetState(TrackingState.Touched);
            if (string.IsNullOrWhiteSpace(value) || value.Length < Constants.MinimumCompanynameLength)
            {
                throw new InvalidCompanyNameException(value);
            }

            if (!Name.Equals(value))
            {
                Name = value;
                SetState(TrackingState.Modified);
            }
        }


        public Company(Guid id, string name, string address, string city, string zip, string country,
            bool enabled, bool newsletter, bool deleted, DateTimeOffset createdOn, DateTimeOffset modifiedOn)
        : base(id)
        {
            Name = name;
            Address = address;
            City = city;
            Zip = zip;
            Country = country;
            IsEnabled = enabled;
            ReceivesNewsletter = newsletter;
            IsDeleted = deleted;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
        }

    }
}