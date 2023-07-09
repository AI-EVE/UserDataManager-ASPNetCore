using Entities;
using ServiceContracts.DTO;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Exceptions;
using ServiceContracts.PersonsServiceContracts;

namespace Services.PersonsServices
{
    public class PersonsGetterService : IPersonsGetterService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;

        //constructor
        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger)
        {
            _personsRepository = personsRepository;
            _logger = logger;
        }


        public async Task<List<PersonResponse>> GetAllPersons()
        {
            //SELECT * from Persons
            var persons = await _personsRepository.GetAllPersons();

            return persons
              .Select(temp => temp.ToPersonResponse()).ToList();

            //return _db.sp_GetAllPersons()
            //  .Select(temp => temp.ToPersonResponse()).ToList();
        }


        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = await _personsRepository.GetPersonByPersonID(personID);


            if (person == null)
                throw new InvalidIDException("Given Person ID Doesn't Exist");


            return person.ToPersonResponse();
        }


        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons() method called from PersonServices Class");

            if (string.IsNullOrEmpty(searchString))
            {
                return await GetAllPersons();
            }

            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.DateOfBirth.Value.ToString("dd MM yyyy").Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryID) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                await _personsRepository.GetFilteredPersons(temp =>
                temp.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };


            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }
    }
}