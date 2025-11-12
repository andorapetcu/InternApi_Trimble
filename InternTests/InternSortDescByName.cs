using Moq;
using InternApi.Services;
using InternApi.ModelDTO;
using InternApi.Controllers;

namespace InternTests
{
    public class InternSortDescByName
    {
        [Fact]
        public async Task SortDescByName_ReturnsSortedInterns()
        {
            var mockService = new Mock<IInternService>();
            var unsortedInterns = new List<InternDTO>
            {
                new InternDTO { Id = Guid.NewGuid(), Name = "Bogdan", Age = 23, DateOfBirth = DateTime.Parse("2001-11-30T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Teo", Age = 22, DateOfBirth = DateTime.Parse("2002-05-15T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Andora", Age = 21, DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z") }
            };

            var expectedSortedInterns = unsortedInterns.OrderByDescending(i => i.Name).ToList();

            mockService.Setup(s => s.SortDescByName())
                .ReturnsAsync(expectedSortedInterns);

            var service = mockService.Object;

            var result = await service.SortDescByName();

            Assert.Equal(3, result.Count);
            Assert.Equal("Teo", result[0].Name);
            Assert.Equal("Bogdan", result[1].Name);
            Assert.Equal("Andora", result[2].Name);
        }
    }
}
