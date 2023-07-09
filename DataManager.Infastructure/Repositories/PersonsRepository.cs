using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;
        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db; 
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person? person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            _db.Persons.Remove(_db.Persons.First(person => person.PersonID == personID));
            int rows = await _db.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons called from PersonsRepository Class");
            return await _db.Persons.ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> Predicate)
        {
            _logger.LogInformation("GetFilteredPersons called from PersonsRepository Class");
            return await _db.Persons.Where(Predicate).ToListAsync();
        }

        public Task<Person?> GetPersonByPersonID(Guid? personID)
        {
            return _db.Persons.FirstOrDefaultAsync(person => person.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);
            if (matchingPerson == null)
            {
                return person;
            }

            //update all details
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender.ToString();
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.Address = person.Address;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

            await _db.SaveChangesAsync(); //UPDATE

            return matchingPerson;
        }
    }
}
