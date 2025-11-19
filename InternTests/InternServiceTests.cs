using AutoMapper;
using InternApi.ModelDTO;
using InternApi.ModelEntity;
using InternApi.Services;
using MongoDB.Driver;
using Moq;

namespace InternTests
{
    public class InternServiceTests
    {
        #region GetAll() Method Tests

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

        #endregion

        #region GetById() Method Tests

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

        #endregion

        #region SortAscByName() Method Tests

        [Fact]
        public async Task SortAscByName_ReturnsSortedInterns()
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

            var result = await service.SortAscByName();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Andora", result[0].Name);
            Assert.Equal("Bogdan", result[1].Name);
            Assert.Equal("Teo", result[2].Name);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region SortDescByName() Method Tests

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

        #endregion

        #region Create() Method Tests

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

        #endregion

        #region Update() Method Tests

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

            var internDTOToReturn = new InternDTO { Id = internId, Name = internEntity.Name, Age = internEntity.Age, DateOfBirth = internEntity.DateOfBirth };
            mockMapper.Setup(m => m.Map<InternDTO>(It.IsAny<Intern>()))
                .Returns(internDTOToReturn);

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

        [Fact]
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

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Update(internId, internDTO);

            Assert.False(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<Intern>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);

            mockMapper.Verify(
               m => m.Map<Intern>(internDTO),
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

        #endregion

        #region Delete() Method Tests

        [Fact]
        public async Task Delete_ReturnsTrue_WhenInternIsDeleted()
        {
            var internId = Guid.NewGuid();
            var internToDelete = new Intern
            {
                Id = internId,
                Name = "Delete",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var internToDeleteDTO = new InternDTO
            {
                Id = internId,
                Name = "Delete",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-24T00:00:00Z")
            };

            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();

            var internDTOToReturn = new InternDTO
            {
                Id = internId,
                Name = internToDelete.Name,
                Age = internToDelete.Age,
                DateOfBirth = internToDelete.DateOfBirth
            };

            mockMapper.Setup(m => m.Map<InternDTO>(It.IsAny<Intern>()))
                .Returns(internDTOToReturn);

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
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_WhenInternIdIsEmpty()
        {
            var mockCollection = new Mock<IMongoCollection<Intern>>();
            var mockMapper = new Mock<IMapper>();
            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Delete(Guid.Empty);

            Assert.False(result);

            mockCollection.Verify(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<DeleteOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_WhenInternIsNotFound()
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

            mockCursor = new Mock<IAsyncCursor<Intern>>();

            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var service = new InternService(mockCollection.Object, mockMapper.Object);

            var result = await service.Delete(internId);

            Assert.False(result);

            mockCollection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<FindOptions<Intern, Intern>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockCollection.Verify(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Intern>>(),
                It.IsAny<DeleteOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion
    }
}
