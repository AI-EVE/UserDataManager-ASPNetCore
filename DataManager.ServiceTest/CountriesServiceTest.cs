using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using services;
using Moq;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly ILogger<CountriesService> _logger;

        private readonly Mock<ILogger<CountriesService>> _loggerMock;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;


        //constructor
        public CountriesServiceTest()
        {
            //List<Country> countries = new List<Country>();
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(x => x.Countries, countries);
            //_countriesService = new CountriesService(dbContext);

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _loggerMock = new Mock<ILogger<CountriesService>>();

            _logger = _loggerMock.Object;
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository, _logger);

        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            
            _countriesRepositoryMock.Setup(x => x.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(request1.ToCountry);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request1);
            });

        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };
            Country country = request.ToCountry();

            //Mocking the AddCountry method
            _countriesRepositoryMock.Setup(x => x.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
        }

        #endregion


        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange
            _countriesRepositoryMock.Setup(x => x.GetAllCountries()).ReturnsAsync(new List<Country>());

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>() {
                                                           new CountryAddRequest() { CountryName = "USA" },
                                                           new CountryAddRequest() { CountryName = "UK" }
            };

            var countries = country_request_list.Select(x => x.ToCountry()).ToList();

            _countriesRepositoryMock.Setup(x => x.GetAllCountries()).ReturnsAsync(countries);

            var expected_country_response_list = countries.Select(x => x.ToCountryResponse()).ToList();

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            actual_country_response_list.Should().BeEquivalentTo(expected_country_response_list);
        }
        #endregion


        #region GetCountryByCountryID

        [Fact]
        //If we supply null as CountryID, it should return null as CountryResponse
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countrID = null;

            //Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countrID);


            //Assert
            Assert.Null(country_response_from_get_method);
        }


        [Fact]
        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "China" };
            Country country = country_add_request.ToCountry();
            country.CountryID = Guid.NewGuid();

            _countriesRepositoryMock.Setup(x => x.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);

            CountryResponse expectedResponse = country.ToCountryResponse();

            //Act
            CountryResponse? actualCountryResponse = await _countriesService.GetCountryByCountryID(country.CountryID);

            //Assert
            actualCountryResponse.Should().BeEquivalentTo(expectedResponse);
        }
        #endregion
    }
}
