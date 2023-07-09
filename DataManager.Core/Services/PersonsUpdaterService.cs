using Entities;
using ServiceContracts.DTO;
using services.Helpers;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using ServiceContracts.PersonsServiceContracts;

namespace Services.PersonsServices
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;

        //constructor
        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger)
        {
            _personsRepository = personsRepository;
            _logger = logger;
        }


        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            _logger.LogInformation("UpdatePerson() method called from PersonServices Class");

            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            Person matchingPerson = await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());

            return matchingPerson.ToPersonResponse();
        }
    }
}