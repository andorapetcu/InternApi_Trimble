using InternApi.ModelEntity;
using InternApi.ModelDTO;
using Moq;
using MongoDB.Driver;
using AutoMapper;
using InternApi.Services;
using Xunit.Abstractions;

namespace InternTests.InternServiceTests
{
    public class Create
    {

        private readonly ITestOutputHelper _output;

        public Create(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Create_ReturnsTrue_WhenInternIsCreated()
        {
            var internEntity = new Intern
            {
                Id = Guid.NewGuid(),
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var internDTO = new InternDTO
            {
                Id = internEntity.Id,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<Intern>(It.IsAny<InternDTO>()))
                  .Returns(internEntity);

            var mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Create(internDTO);

            Assert.True(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.InsertOneAsync(
              It.IsAny<Intern>(),
              It.IsAny<InsertOneOptions>(),
              It.IsAny<CancellationToken>()),
              Times.Once);

            mockMapper.Verify(
                m => m.Map<Intern>(internDTO),
                Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsFalse_WhenInternIsNull()
        {
            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();
            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Create(null);

            Assert.False(result);

            mockCollection.Verify(c => c.InsertOneAsync(
                It.IsAny<Intern>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ReturnsNull_WhenInternIdAlreadyExistsInList()
        {
            var internDTO = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "New Intern",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var existingIntern = new Intern
            {
                Id = internDTO.Id,
                Name = "existing Intern",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var existingInternDTO = new InternDTO
            {
                Id = existingIntern.Id,
                Name = "existing Intern",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };


            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<InternDTO>(It.IsAny<Intern>()))
                  .Returns(existingInternDTO);

            var mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            mockCursor.SetupGet(_ => _.Current)
               .Returns(new List<Intern> { existingIntern });

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Create(internDTO);

            Assert.False(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.InsertOneAsync(
              It.IsAny<Intern>(),
              It.IsAny<InsertOneOptions>(),
              It.IsAny<CancellationToken>()),
              Times.Never);
        }

        [Fact]
        public async Task Create_ReturnsFalse_WhenNameIsNullOrWhiteSpace()
        {
            var internDTO = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };
            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();

            var mockCursor = new Mock<IAsyncCursor<Intern>>();
            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mockCollection.Setup(c => c.FindAsync(
               It.IsAny<FilterDefinition<Intern>>(),
               It.IsAny<FindOptions<Intern, Intern>>(),
               It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockCursor.Object);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Create(internDTO);

            Assert.False(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.InsertOneAsync(
              It.IsAny<Intern>(),
              It.IsAny<InsertOneOptions>(),
              It.IsAny<CancellationToken>()),
              Times.Never);
        }
    }
}
