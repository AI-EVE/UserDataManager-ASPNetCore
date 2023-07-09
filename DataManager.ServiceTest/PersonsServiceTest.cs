using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using AutoFixture;
using Moq;
using RepositoryContracts;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using ServiceContracts.PersonsServiceContracts;
using Services.PersonsServices;
using Exceptions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IFixture _fixture;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            //List<Person> persons = new List<Person>();
            //List<Country> countries = new List<Country>();

            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            //ApplicationDbContext dbContext = dbContextMock.Object;

            //dbContextMock.CreateDbSetMock(x => x.Persons, persons);
            //dbContextMock.CreateDbSetMock(x => x.Countries, countries);

            //_countriesService = new CountriesService(dbContext);
            //_personService = new PersonsService(dbContext, _countriesService);

            var PersonsGetterLogger = new Mock<ILogger<PersonsGetterService>>();
            var PersonsAdderLogger = new Mock<ILogger<PersonsAdderService>>();
            var PersonsDeleterLogger = new Mock<ILogger<PersonsDeleterService>>();
            var PersonsUpdaterLogger = new Mock<ILogger<PersonsUpdaterService>>();
            var PersonsSorterLogger = new Mock<ILogger<PersonsSorterService>>();
            _personsRepositoryMock = new Mock<IPersonsRepository>();

            _personsRepository = _personsRepositoryMock.Object;

            _personsGetterService = new PersonsGetterService(_personsRepository, PersonsGetterLogger.Object);
            _personsAdderService = new PersonsAdderService(_personsRepository, PersonsAdderLogger.Object);
            _personsDeleterService = new PersonsDeleterService(_personsRepository, PersonsDeleterLogger.Object);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, PersonsUpdaterLogger.Object);
            _personsSorterService = new PersonsSorterService(_personsRepository, PersonsSorterLogger.Object);
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            });
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(x => x.PersonName, null as string).Create();
            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(x => x.Email, "person@example.com").Create();

            Person person = personAddRequest.ToPerson();
            _personsRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personsAdderService.AddPerson(personAddRequest);

            //Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
        }

        #endregion


        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(person_response_from_get);
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            // Arrange
            Person person = _fixture.Create<Person>();

            _personsRepositoryMock.Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act 
            PersonResponse person_response_from_get = await _personsGetterService.GetPersonByPersonID(person.PersonID);
            // Assert
            Assert.Equal(person_response_from_get, person.ToPersonResponse());
        }

        #endregion


        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            _personsRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(new List<Person>());

            //Act
            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange

            List<Person> persons = _fixture.CreateMany<Person>(5).ToList();

            _personsRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> persons_from_add = new List<PersonResponse>();
            foreach (Person person in persons)
            { 
                persons_from_add.Add(person.ToPersonResponse());
            }

            //Act

            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();


            // Assert
            Assert.Equal(persons_from_add, persons_from_get);
        }
        #endregion


        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            List<Person> persons = _fixture.CreateMany<Person>(5).ToList();
            List<PersonResponse> persons_expected = persons.Select(x => x.ToPersonResponse()).ToList();

            // Mock
            _personsRepositoryMock.Setup(x => x.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            _personsRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(persons);

            // Act
            List<PersonResponse> persons_filtered = await _personsGetterService.GetFilteredPersons(nameof(Person.Email), "");

            // Assert
            Assert.Equal(persons_expected, persons_filtered);
        }


        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            List<Person> persons = _fixture.CreateMany<Person>(5).ToList();
            List<PersonResponse> persons_expected = persons.Select(x => x.ToPersonResponse()).ToList();

            // Mock
            _personsRepositoryMock.Setup(x => x.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            // Act
            List<PersonResponse> persons_filtered = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            // Assert
            Assert.Equal(persons_expected, persons_filtered);
        }

        #endregion


        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            List<PersonResponse> personResponses = _fixture.CreateMany<PersonResponse>(5).ToList();

            List<PersonResponse> personsResponse_sorted_expected = personResponses.OrderBy(x => x.PersonName, StringComparer.OrdinalIgnoreCase).ToList();

            // Act
            List<PersonResponse> personsResponse_sorted = await _personsSorterService.GetSortedPersons(personResponses, nameof(PersonResponse.PersonName), SortOrderOptions.ASC);

            // Assert
            Assert.Equal(personsResponse_sorted, personsResponse_sorted_expected);
        }
        #endregion


        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => {
                //Act
                await _personsUpdaterService.UpdatePerson(person_update_request);
            });
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest updateRequest = _fixture.Build<PersonUpdateRequest>().With(x => x.Email, "kiajsasi@kijai.com").Create();
            Person person = updateRequest.ToPerson();
            PersonResponse personResponse = person.ToPersonResponse();
            _personsRepositoryMock.Setup(x => x.UpdatePerson(It.IsAny<Person>()))!.ReturnsAsync(person);

            // Act
            PersonResponse personResponse2 = await _personsUpdaterService.UpdatePerson(updateRequest);

            // Assert
            Assert.Equal(personResponse, personResponse2);
        }


        //When PersonName is null, it should throw ArgumentException
        [Fact]
        public async void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            PersonUpdateRequest updateRequest = _fixture.Build<PersonUpdateRequest>().With(x => x.PersonName, null as string).With(x => x.Email, "kiajsasi@kijai.com").Create();


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => {
                //Act
                await _personsUpdaterService.UpdatePerson(updateRequest);
            });

        }


        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            // Arrange
            PersonUpdateRequest updateRequest = _fixture.Build<PersonUpdateRequest>().With(x => x.Email, "kiajsasi@kijai.com").Create();
            Person person = updateRequest.ToPerson();
            PersonResponse personResponse = person.ToPersonResponse();
            _personsRepositoryMock.Setup(x => x.UpdatePerson(It.IsAny<Person>()))!.ReturnsAsync(person);

            // Act
            PersonResponse personResponse2 = await _personsUpdaterService.UpdatePerson(updateRequest);

            // Assert
            Assert.Equal(personResponse, personResponse2);

        }

        #endregion


        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            // Arrange
            Person person = _fixture.Build<Person>().With(x => x.Email, "asasas@asasa.com").Create();
            _personsRepositoryMock.Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(x => x.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);

            // Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonID);

            // Assert
            Assert.True(isDeleted);

        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            // Arrange
            Person person = _fixture.Build<Person>().With(x => x.Email, "asasas@asasa.com").Create();
            _personsRepositoryMock.Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(null as Person);
            _personsRepositoryMock.Setup(x => x.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(false);

            // Assert
            await Assert.ThrowsAsync<InvalidIDException>(async () =>
            {
                bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonID);
            });
        }

        #endregion
    }
}