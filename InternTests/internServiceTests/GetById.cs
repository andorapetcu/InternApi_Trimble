using InternApi.ModelEntity;
using InternApi.ModelDTO;
using InternApi.Services;
using Moq;
using MongoDB.Driver;
using AutoMapper;

namespace InternTests.InternServiceTests
{
    public class GetById
    {
        [Fact]
        public async Task GetById_ReturnsIntern_WhenInternExists()
        {
            var internId = Guid.NewGuid();
            var internEntity = new Intern
            {
                Id = internId,
                Name = "Alice",
                Age = 22,
                DateOfBirth = DateTime.Parse("2002-03-15T00:00:00Z")
            };

            var internDTO = new InternDTO
            {
                Id = internId,
                Name = "Alice",
                Age = 22,
                DateOfBirth = DateTime.Parse("2002-03-15T00:00:00Z")
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();

            var mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            mockCursor.SetupGet(_ => _.Current)
                .Returns(new List<Intern> { internEntity });

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<InternDTO>(internEntity))
                .Returns(internDTO);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.GetById(internId);

            Assert.NotNull(result);
            Assert.Equal(internDTO.Name, result.Name);

            mockCollection.Verify(
                c => c.FindAsync(
                    It.IsAny<FilterDefinition<Intern>>(),
                    It.IsAny<FindOptions<Intern, Intern>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                m => m.Map<InternDTO>(internEntity),
                Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenIdIsEmpty()
        {
            var mockCollection = new Mock<IMongoCollection<Intern>>();

            var mockMapper = new Mock<IMapper>();

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.GetById(Guid.Empty);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenInternIsNotInList()
        {
            var internId = Guid.NewGuid();

            var mockCollection = new Mock<IMongoCollection<Intern>>();

            var mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var mockMapper = new Mock<IMapper>();

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.GetById(internId);

            Assert.Null(result);

            mockCollection.Verify(
                c => c.FindAsync(
                    It.IsAny<FilterDefinition<Intern>>(),
                    It.IsAny<FindOptions<Intern, Intern>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
