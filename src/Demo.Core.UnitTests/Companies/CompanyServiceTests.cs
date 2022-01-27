using System;
using System.Threading.Tasks;
using Demo.Core.Companies;
using Demo.Core.Companies.Contracts;
using Demo.Core.Companies.DataTransferObjects;
using Demo.Core.Companies.DomainModel;
using Demo.Core.Companies.Exceptions;
using Moq;
using NUnit.Framework;

namespace Demo.Core.UnitTests.Companies
{
    [TestFixture]
    public class CompanyServiceTests
    {

        private Mock<ICompaniesRepository> _companiesRepositoryMock;
            

        [SetUp]
        public void Setup()
        {
            _companiesRepositoryMock = new Mock<ICompaniesRepository>();
        }


        [TestCase("n")]
        [TestCase("")]
        [TestCase(null)]
        public void WhenCompanyIsChangedToInvalidName_ItThrowsInvalidCompanyNameException(string name)
        {
            var company = new Company(
                Guid.NewGuid(), 
                "Name", 
                "Address", 
                "City", 
                "Zip", 
                "Country",
                true, 
                false, 
                false, 
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddSeconds(-30));

            var action = new TestDelegate(() => company.SetName(name));
            Assert.Throws<InvalidCompanyNameException>(action);
        }

        [TestCase("n")]
        [TestCase("")]
        [TestCase(null)]
        public void WhenCompanyNameIsInvalid_ItThrowsInvalidNameException(string name)
        {
            var company = new Company(
                Guid.NewGuid(), 
                "Name", 
                "Address", 
                "City", 
                "Zip", 
                "Country",
                true, 
                false, 
                false, 
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddSeconds(-30));

            _companiesRepositoryMock
                .Setup(e => e.Get(company.Id))
                .ReturnsAsync(company);

            var companiesService = new CompaniesService(_companiesRepositoryMock.Object);

            var dto = new CompanyDetailsDto
            {
                Id = company.Id,
                Name = name
            };

            var act = new AsyncTestDelegate(() => companiesService.Update(dto));
            Assert.ThrowsAsync<InvalidCompanyNameException>(act);
        }
    }
}