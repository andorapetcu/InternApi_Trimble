using InternApi.Services;
using InternApi.ModelDTO;
using Moq;

namespace InternTests
{
    public class InternSortAscByName
    {
        [Fact]
        public async Task SortAscByName_ReturnsSortedInterns()
        {
            var mockService = new Mock<IInternService>();
            var unsortedInterns = new List<InternDTO>
            {
                new InternDTO { Id = Guid.NewGuid(), Name = "Teo", Age = 22, DateOfBirth = DateTime.Parse("2002-05-15T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Andora", Age = 21, DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Bogdan", Age = 23, DateOfBirth = DateTime.Parse("2001-11-30T00:00:00Z") }
            };

            var expectedSortedInterns = unsortedInterns.OrderBy(i => i.Name).ToList();

            mockService.Setup(s => s.SortAscByName())
                       .ReturnsAsync(expectedSortedInterns);

            var service = mockService.Object;

            var result = await service.SortAscByName();

            Assert.Equal(3, result.Count);
            Assert.Equal("Andora", result[0].Name);
            Assert.Equal("Bogdan", result[1].Name);
            Assert.Equal("Teo", result[2].Name);
        }
    }
}
