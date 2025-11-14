using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;
using InternApi.Services;
using MongoDB.Driver;
using Moq;
namespace InternTests.InternServiceTests
{
    public class GetAllTests
    {
        [Fact]
        public async Task GetAll_ReturnsListOfInterns()
        {
            var internEntities = new List<Intern>
            {
                new Intern { Id = Guid.NewGuid(), Name = "Alice", Age = 22, DateOfBirth = DateTime.Parse("2002-03-15T00:00:00Z") },
                new Intern { Id = Guid.NewGuid(), Name = "Bob", Age = 24, DateOfBirth = DateTime.Parse("2000-07-20T00:00:00Z") }
            };

            var internDTOs = new List<InternDTO>
            {
                new InternDTO { Id = Guid.NewGuid(), Name = "Alice", Age = 22, DateOfBirth = DateTime.Parse("2002-03-15T00:00:00Z") },
                new InternDTO { Id = Guid.NewGuid(), Name = "Bob", Age = 24, DateOfBirth = DateTime.Parse("2000-07-20T00:00:00Z") }
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();

            var mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true) 
                      .ReturnsAsync(false); 
            mockCursor.SetupGet(_ => _.Current).Returns(internEntities);

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<List<InternDTO>>(internEntities))
                .Returns(internDTOs);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(internDTOs.Count, result.Count);
            Assert.Equal(internDTOs[0].Name, result[0].Name);
            Assert.Equal(internDTOs[1].Name, result[1].Name);

            mockCollection.Verify(
                c => c.FindAsync(
                    It.IsAny<FilterDefinition<Intern>>(),
                    It.IsAny<FindOptions<Intern, Intern>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(m => m.Map<List<InternDTO>>(internEntities), Times.Once);
        }
    }
}
