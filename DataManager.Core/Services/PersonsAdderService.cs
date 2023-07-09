using Entities;
using ServiceContracts.DTO;
using services.Helpers;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using ServiceContracts.PersonsServiceContracts;

namespace Services.PersonsServices
{
    public class PersonsAdderService : IPersonsAdderService
    {
        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsAdderService> _logger;

        //constructor
        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsAdderService> logger)
        {
            _personsRepository = personsRepository;
            _logger = logger;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            _logger.LogInformation("AddPerson method called from Persons services");

            //check if PersonAddRequest is not null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            //add person object to persons list
            Person addedPerson = await _personsRepository.AddPerson(person);
            //_db.sp_InsertPerson(person);

            //convert the Person object into PersonResponse type
            return person.ToPersonResponse();
        }
    }
}