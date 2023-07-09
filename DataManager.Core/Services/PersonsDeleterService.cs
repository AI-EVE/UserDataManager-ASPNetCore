using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Exceptions;
using ServiceContracts.PersonsServiceContracts;

namespace Services.PersonsServices
{
    public class PersonsDeleterService : IPersonsDeleterService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsDeleterService> _logger;

        //constructor
        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsDeleterService> logger)
        {
            _personsRepository = personsRepository;
            _logger = logger;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            _logger.LogInformation("DeletePerson() method called from PersonServices Class");

            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            if (await _personsRepository.GetPersonByPersonID(personID) == null)
            {
                throw new InvalidIDException(nameof(personID));
            }

            return await _personsRepository.DeletePerson(personID);
        }
    }
}