using AutoFixture;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using ServiceContracts.PersonsServiceContracts;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly Fixture? _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _countriesServiceMock = new Mock<ICountriesService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();

            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
            _logger = _loggerMock.Object;
        }

        [Fact]
        public async void IndexTest()
        {
            // Arrange
            List<PersonResponse> personResponses = new List<PersonResponse>() {
                _fixture.Build<PersonResponse>().With(x => x.Country, null as string).Create(),
                _fixture.Build<PersonResponse>().With(x => x.Country, null as string).Create(),
                _fixture.Build<PersonResponse>().With(x => x.Country, null as string).Create(),
                _fixture.Build<PersonResponse>().With(x => x.Country, null as string).Create(),
                _fixture.Build<PersonResponse>().With(x => x.Country, null as string).Create(),
                _fixture.Build<PersonResponse>().With(x => x.Country, null as string).Create(),
            };

            List<PersonResponse> personResponsesSorted = personResponses.OrderBy(x => x.PersonName, StringComparer.OrdinalIgnoreCase).ToList();

            PersonsController personsController = new PersonsController(_personsGetterService, _personsDeleterService, _personsUpdaterService, _personsAdderService, _personsSorterService, _countriesService, _logger);

            // Moq
            _personsGetterServiceMock.Setup(x => x.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponses);
            
            _personsSorterServiceMock.Setup(x => x.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponsesSorted);

            _countriesServiceMock.Setup(x => x.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(null as CountryResponse);

            // Act

            IActionResult result = await personsController.Index("", "", nameof(PersonResponse.PersonName), SortOrderOptions.ASC);

            ViewResult view = Assert.IsType<ViewResult>(result);
            Assert.Equal(personResponsesSorted, view.Model);

        }
    }
}
