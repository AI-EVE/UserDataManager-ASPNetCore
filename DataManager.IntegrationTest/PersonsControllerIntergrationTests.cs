using FluentAssertions;

namespace CRUDTests
{
    public class PersonsControllerIntergrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        public PersonsControllerIntergrationTests(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task IndexTest()
        { 

            // Arrange

            // Act
            HttpResponseMessage response = await _httpClient.GetAsync("/Persons/Index");

            // Assert
            response.Should().BeSuccessful();
        }
    }
}
