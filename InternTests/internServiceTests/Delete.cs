using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;
using InternApi.Services;
using MongoDB.Driver;
using Moq;

namespace InternTests.InternServiceTests
{
    public class Delete
    {
        [Fact] // !!!!!!!!!!! FAILS !!!!!!!!!!!!!!!!!!!!!
        public async Task Delete_ReturnsTrue_WhenInternIsDeleted()
        {
            var internId = Guid.NewGuid();
            var internToDelete = new Intern
            {
                Id = internId,
                Name = "",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<Intern>(It.IsAny<InternDTO>()))
                .Returns(internToDelete);

            var mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            mockCursor.SetupGet(_ => _.Current)
                .Returns(new List<Intern> { internToDelete });

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var deleteResult = Mock.Of<DeleteResult>(r => r.IsAcknowledged == true && r.DeletedCount == 1);

            mockCollection.Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<DeleteOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(deleteResult);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Delete(internId);

            Assert.True(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<DeleteOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
