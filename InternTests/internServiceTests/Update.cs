using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;
using InternApi.Services;
using MongoDB.Driver;
using Moq;

namespace InternTests.InternServiceTests
{
    public class Update
    {
        [Fact]
        public async Task Update_ReturnsTrue_WhenInternIsUpdated()
        {
            var internId = Guid.NewGuid();
            var internEntity = new Intern
            {
                Id = internId,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var internDTO = new InternDTO
            {
                Id = internId,
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
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            mockCursor.SetupGet(_ => _.Current)
                .Returns(new List<Intern> { internEntity });

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var replaceResult = Mock.Of<ReplaceOneResult>(r => r.IsAcknowledged == true && r.ModifiedCount == 1);

            mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Intern>>(), 
                It.IsAny<Intern>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(replaceResult);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Update(internId, internDTO);

            Assert.True(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.Is<Intern>(i => i.Id == internEntity.Id),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                m => m.Map<Intern>(internDTO),
                Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsFalse_WhenInternIsNull()
        {
            var internId = Guid.NewGuid();
            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();
            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Update(internId, null);

            Assert.False(result);

            mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<Intern>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Update_ReturnsFalse_WhenInternIdIsEmpty()
        {
            var internDTO = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();
            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Update(Guid.Empty, internDTO);

            Assert.False(result);

            mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<Intern>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]  // !!!!!!!!!!! FAILS !!!!!!!!!!!!!!!!!!!!!
        public async Task Update_ReturnsFalse_WhenInternIsNotFound()
        {
            var internId = Guid.NewGuid();
            var internEntity = new Intern
            {
                Id = internId,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var internDTO = new InternDTO
            {
                Id = internId,
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

            //var replaceResult = Mock.Of<ReplaceOneResult>(r => r.IsAcknowledged == false && r.ModifiedCount == 0);

            //mockCollection.Setup(c => c.ReplaceOneAsync(
            //    It.IsAny<FilterDefinition<Intern>>(),
            //    It.IsAny<Intern>(),
            //    It.IsAny<ReplaceOptions>(),
            //    It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(replaceResult);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Update(internId, internDTO);

            Assert.False(result);

            mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<Intern>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Update_ReturnsFalse_WhenNameIsNullOrWhiteSpace()
        {
            var internId = Guid.NewGuid();
            var internEntity = new Intern
            {
                Id = internId,
                Name = "",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };
            var internDTO = new InternDTO
            {
                Id = internId,
                Name = "",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };
            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();

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

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Update(internId, internDTO);

            Assert.False(result);

            mockCollection.Verify(c => c.ReplaceOneAsync(
               It.IsAny<FilterDefinition<Intern>>(),
               It.IsAny<Intern>(),
               It.IsAny<ReplaceOptions>(),
               It.IsAny<CancellationToken>()),
               Times.Never);
        }
    }
}
