using System;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using Demo.Data;
using Demo.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;

namespace Demo.DataMigration
{
    class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "SQL Server Data Migrator",
                Description = "Entity Framework Migrations Runner"
            };

            app.HelpOption("-?|-h|--help");

            app.Command("migrate", command =>
            {
                command.HelpOption("-?|-h|--help");

                var databaseNameOption = command.Option("-n | --databaseName", "The name of the sql database.",
                    CommandOptionType.SingleValue);
                var serverNameOption = command.Option("-s | --databaseServerName", "The name of the sql server.",
                    CommandOptionType.SingleValue);
                var userNameOption = command.Option("-u | --user", "The name of database user.",
                    CommandOptionType.SingleValue);
                var passwordOption = command.Option("-p | --password", "The password of the database user.",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    try
                    {
                        if (!databaseNameOption.HasValue() || !serverNameOption.HasValue() ||
                            !userNameOption.HasValue() || !passwordOption.HasValue())
                        {
                            app.ShowHint();
                            return 1;
                        }

                        Console.WriteLine(
                            $"Updating database {databaseNameOption.Value()} on server {serverNameOption.Value()} ...");

                        var builder = new SqlConnectionStringBuilder
                        {
                            ["Data Source"] = $"tcp:{serverNameOption.Value()}.database.windows.net,1433",
                            ["Initial Catalog"] = databaseNameOption.Value(),
                            ["User Id"] = $"{userNameOption.Value()}@{serverNameOption.Value()}",
                            ["Password"] = passwordOption.Value()
                        };
                        var optionsBuilder = new DbContextOptionsBuilder<DemoDataContext>();
                        optionsBuilder.UseSqlServer(builder.ConnectionString,
                            x => x.MigrationsAssembly(typeof(DemoDataContext).Assembly.GetName().Name));
                        optionsBuilder.EnableDetailedErrors();

                        var dataContextInstance = new DemoDataContext(optionsBuilder.Options);
                        var pendingMigrations = dataContextInstance.Database
                            .GetPendingMigrations()
                            .ToArray();

                        if (pendingMigrations.Any())
                        {
                            Console.WriteLine("Migration to be applied: {0}", string.Join(",", pendingMigrations));
                            dataContextInstance.Database.Migrate();
                            Console.WriteLine("Done. Database updated successfully");
                        }

                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Update database failed! {0}", ex);
                        return -1;
                    }
                });
            });
            app.Command("seed", command =>
            {
                command.HelpOption("-?|-h|--help");

                var databaseNameOption = command.Option("-n | --databaseName", "The name of the sql database.",
                    CommandOptionType.SingleValue);
                var serverNameOption = command.Option("-s | --databaseServerName", "The name of the sql server.",
                    CommandOptionType.SingleValue);
                var userNameOption = command.Option("-u | --user", "The name of database user.",
                    CommandOptionType.SingleValue);
                var passwordOption = command.Option("-p | --password", "The password of the database user.",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    try
                    {
                        if (!databaseNameOption.HasValue() || !serverNameOption.HasValue() ||
                            !userNameOption.HasValue() || !passwordOption.HasValue())
                        {
                            app.ShowHint();
                            return 1;
                        }

                        Console.WriteLine(
                            $"Seeding database {databaseNameOption.Value()} on server {serverNameOption.Value()} ...");

                        var builder = new SqlConnectionStringBuilder
                        {
                            ["Data Source"] = $"tcp:{serverNameOption.Value()}.database.windows.net,1433",
                            ["Initial Catalog"] = databaseNameOption.Value(),
                            ["User Id"] = $"{userNameOption.Value()}@{serverNameOption.Value()}",
                            ["Password"] = passwordOption.Value()
                        };

                        var optionsBuilder = new DbContextOptionsBuilder<DemoDataContext>();
                        optionsBuilder.UseSqlServer(builder.ConnectionString,
                            x => x.MigrationsAssembly(typeof(DemoDataContext).Assembly.GetName().Name));
                        optionsBuilder.EnableDetailedErrors();

                        var svcApiDbContext = new DemoDataContext(optionsBuilder.Options);
                        SeedDatabase(svcApiDbContext);


                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Seeding database failed! {0}", ex);
                        return -1;
                    }
                });
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }

        private static void SeedDatabase(DemoDataContext ddc)
        {
            SeedCompanies(ddc);
        }

        private static void SeedCompanies(DemoDataContext ddc)
        {
            var companiesCount = ddc.Companies.Count();
            if (companiesCount == 0)
            {
                var companyGenerator = GetCompanyGenerator();
                var companies = companyGenerator.Generate(500);
                ddc.AddRange(companies);
                ddc.SaveChanges();
            }
        }

        private static Faker<CompanyEntity> GetCompanyGenerator()
        {
            var companyGenerator = new Faker<CompanyEntity>()
                .CustomInstantiator(f => new CompanyEntity())
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Name, (f, u) => f.Company.CompanyName())
                .RuleFor(u => u.Address, (f, u) => $"{f.Address.BuildingNumber()}, {f.Address.StreetName()}")
                .RuleFor(u => u.City, f => f.Address.City())
                .RuleFor(u => u.Zip, (f, u) => f.Address.ZipCode())
                .RuleFor(u => u.Country, (f, u) => f.Address.Country())
                .RuleFor(u => u.IsEnabled, f => f.Random.Bool())
                .RuleFor(u => u.IsDeleted, f => f.Random.Bool())
                .RuleFor(u => u.CreatedOn,
                    f => f.Date.BetweenOffset(DateTimeOffset.UtcNow.AddTicks(-3), DateTimeOffset.UtcNow.AddYears(-2)))
                .RuleFor(u => u.ModifiedOn,
                    f => f.Date.BetweenOffset(DateTimeOffset.UtcNow.AddTicks(-1), DateTimeOffset.UtcNow));
            return companyGenerator;
        }

    }
}