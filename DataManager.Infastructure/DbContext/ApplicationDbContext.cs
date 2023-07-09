using System.Text.Json;
using DataManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed

            List<Country> countries = JsonSerializer.Deserialize<List<Country>>(System.IO.File.ReadAllText("countries.json"))!;
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(System.IO.File.ReadAllText("persons.json"))!;

            foreach (var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            foreach (var person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            // Fluint API
            modelBuilder.Entity<Person>().Property(p => p.TIN).HasColumnType("varchar(10)").HasDefaultValue("ABC12345");

            // modelBuilder.Entity<Person>().HasIndex(p => p.TIN).IsUnique();
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TIN]) = 8");
        }

        public async Task<List<Person>> sp_GetAllPersons()
        { 
            return await Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToListAsync();
        }

        public async Task<int> sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                                        new SqlParameter("@PersonID", person.PersonID),
                                        new SqlParameter("@PersonName", person.PersonName),
                                        new SqlParameter("@Email", person.Email),
                                        new SqlParameter("@DateOfBirth", person.DateOfBirth),
                                        new SqlParameter("@Gender", person.Gender),
                                        new SqlParameter("@CountryID", person.CountryID),
                                        new SqlParameter("@Address", person.Address),
                                        new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };

            return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
        }

        public async Task<int> sp_DeletePerson(Guid? personID)
        { 
            SqlParameter sqlParameter = new SqlParameter("@PersonID", personID);

            return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[DeletePerson] @PersonID", sqlParameter);
        }
    }
}
