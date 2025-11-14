using InternApi.Services;
using Moq;
using InternApi.ModelDTO;
using MongoDB.Driver;
using AutoMapper;
using InternApi.ModelEntity;

namespace InternTests.InternServiceTests
{
    public class SortDescByName
    {
        [Fact]
        public async Task SortDescByName_ReturnsSortedInterns()
        {
            var unsortedInternDTOs = new List<InternDTO>
            {
                new InternDTO { Id = Guid.NewGuid(), Name = "Teo", Age = 22, DateOfBirth = DateTime.Parse("2002-05-15T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Andora", Age = 21, DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Bogdan", Age = 23, DateOfBirth = DateTime.Parse("2001-11-30T00:00:00Z") }
            };

            var unsortedInternEntities = new List<Intern>
            {
                new Intern { Id = Guid.NewGuid(), Name = "Teo", Age = 22, DateOfBirth = DateTime.Parse("2002-05-15T00:00:00Z") },
                new Intern { Id = Guid.NewGuid(), Name = "Andora", Age = 21, DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z") },
                new Intern { Id = Guid.NewGuid(), Name = "Bogdan", Age = 23, DateOfBirth = DateTime.Parse("2001-11-30T00:00:00Z") }
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();

            var mockMapper = new Mock<IMapper>();

            var mockCursor = new Mock<IAsyncCursor<Intern>>();
            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.SetupGet(_ => _.Current).Returns(unsortedInternEntities);

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            mockMapper.Setup(m => m.Map<List<InternDTO>>(unsortedInternEntities))
                .Returns(unsortedInternDTOs);

            mockMapper.Setup(m => m.Map<List<InternDTO>>(It.IsAny<List<InternDTO>>()))
                .Returns((List<InternDTO> sortedDTOs) => sortedDTOs);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.SortDescByName();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Teo", result[0].Name);
            Assert.Equal("Bogdan", result[1].Name); 
            Assert.Equal("Andora", result[2].Name);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
