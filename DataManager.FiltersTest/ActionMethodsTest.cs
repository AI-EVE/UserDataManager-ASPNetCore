using AutoFixture;
using CRUDExample.Controllers;
using CRUDProject.Filters.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.PersonsServiceContracts;
using FluentAssertions;


namespace CRUDTests
{
    public class ActionMethodsTest
    {
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;


        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        private readonly PersonsController _personsController;



        private readonly Fixture _fixture;


        public ActionMethodsTest() {

            _fixture = new Fixture();

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _countriesServiceMock = new Mock<ICountriesService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();

            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _logger = _loggerMock.Object;


            _countriesService = _countriesServiceMock.Object;

            _personsController = new PersonsController(_personsGetterService, _personsDeleterService, _personsUpdaterService, _personsAdderService, _personsSorterService, _countriesService, _logger);

        }

        [Fact]
        public async Task OnActionExecutionAsync_Should_Add_Countries_And_Errors_To_ViewBag_When_ModelState_Is_Invalid()
        {

            // Arrange

            var actionContext = new ActionContext() {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            var actionArguments = new Dictionary<string, object>() { { "personRequest", new object() } };
            var filters = new List<IFilterMetadata>();
            var actionExecutingContext = new ActionExecutingContext(actionContext, filters, actionArguments, _personsController);

            _personsController.ModelState.AddModelError("personName", "Person name is required");

            var countries = _fixture.CreateMany<CountryResponse>(5).ToList();
            var expectedListItems = countries.Select(temp =>
                    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            var actionFilter = new PersonCreateEditActionFilter(_countriesService);
            
            var actionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult<ActionExecutedContext>(null));


            // Act
            await actionFilter.OnActionExecutionAsync(actionExecutingContext, actionExecutionDelegate);

            // Assert
            Assert.NotNull(_personsController.ViewBag.Countries);
            expectedListItems.Should().BeEquivalentTo(_personsController.ViewBag.Countries);
            Assert.NotNull(_personsController.ViewBag.Errors);
            Assert.Equal("Person name is required", _personsController.ViewBag.Errors[0]);
        }
    }
}
