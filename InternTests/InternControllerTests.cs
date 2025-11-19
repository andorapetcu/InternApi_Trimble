using InternApi.Controllers;
using InternApi.ModelDTO;
using InternApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternTests
{
    public class InternControllerTests
    {
        #region GetAll() Method Tests

        [Fact]
        public async Task GetAllInterns_ReturnsOk_WithListOfInterns()
        {
            var mockService = new Mock<IInternService>();
            var expectedInterns = new List<InternDTO>
            {
                new InternDTO { Id = Guid.NewGuid(), Name = "Intern1", Age = 23, DateOfBirth = DateTime.Parse("2001-01-01T00:00:00Z")},
                new InternDTO { Id = Guid.NewGuid(), Name = "Intern2", Age = 22, DateOfBirth = DateTime.Parse("2002-02-02T00:00:00Z")}
            };

            mockService.Setup(s => s.GetAll())
                .ReturnsAsync(expectedInterns);

            var controller = new InternController(mockService.Object);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<List<InternDTO>>(okResult.Value);
            Assert.Equal("Intern1", model[0].Name);
            Assert.Equal(23, model[0].Age);
            Assert.Equal(DateTime.Parse("2001-01-01T00:00:00Z"), model[0].DateOfBirth);
            Assert.Equal("Intern2", model[1].Name);
            Assert.Equal(22, model[1].Age);
            Assert.Equal(DateTime.Parse("2002-02-02T00:00:00Z"), model[1].DateOfBirth);
            Assert.Equal(2, model.Count);
        }

        #endregion

        #region GetById() Method Tests

        [Fact]
        public async Task GetIntern_ReturnsOk_WhenInternExists()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            var expectedIntern = new InternDTO
            {
                Id = internId,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.Parse("2004-03-03T00:00:00Z")
            };

            mockService.Setup(s => s.GetById(internId))
                       .ReturnsAsync(expectedIntern);

            var controller = new InternController(mockService.Object);

            var result = await controller.GetIntern(internId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<InternDTO>(okResult.Value);
            Assert.Equal(internId, model.Id);
            Assert.Equal("Andora", model.Name);
            Assert.Equal(21, model.Age);
            Assert.Equal(DateTime.Parse("2004-03-03T00:00:00Z"), model.DateOfBirth);
        }

        [Fact]
        public async Task GetIntern_ReturnsBadRequest_WhenIdIsMissing()
        {
            var mockService = new Mock<IInternService>();

            var controller = new InternController(mockService.Object);

            var result = await controller.GetIntern(Guid.Empty);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Invalid intern ID", badRequest.Value);
        }

        [Fact]
        public async Task GetIntern_ReturnsNotFound_WhenInternDoesNotExist()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            mockService.Setup(s => s.GetById(internId))
                       .ReturnsAsync((InternDTO)null);

            var controller = new InternController(mockService.Object);

            var result = await controller.GetIntern(internId);

            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region SortAscByName() Method Tests

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

        #endregion

        #region SortDescByName() Method Tests

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

        #endregion

        #region Create() Method Tests

        [Fact]
        public async Task Create_ReturnsOk_WhenInternIsCreated()
        {
            var mockService = new Mock<IInternService>();
            var newIntern = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };
            mockService.Setup(s => s.Create(newIntern))
                .ReturnsAsync(true);

            var controller = new InternController(mockService.Object);

            var result = await controller.Create(newIntern);

            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsType<InternDTO>(okResult.Value);

            Assert.Equal(newIntern.Id, model.Id);
            Assert.Equal(newIntern.Name, model.Name);
            Assert.Equal(newIntern.Age, model.Age);
            Assert.Equal(newIntern.DateOfBirth, model.DateOfBirth);
        }


        [Fact]
        public async Task Create_ReturnsBadRequest_WhenInternIsNull()
        {
            var mockService = new Mock<IInternService>();
            var controller = new InternController(mockService.Object);

            var result = await controller.Create(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Intern data is missing", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenNameIsEmpty()
        {
            var mockService = new Mock<IInternService>();
            var controller = new InternController(mockService.Object);
            var intern = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };

            var result = await controller.Create(intern);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Intern name cannot be empty", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenUnderEighteen()
        {
            var mockService = new Mock<IInternService>();
            var controller = new InternController(mockService.Object);
            var intern = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "Maria",
                Age = 17,
                DateOfBirth = DateTime.UtcNow.AddYears(-17)
            };

            var result = await controller.Create(intern);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Intern age must be at least 18", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDateOfBirthIsInFuture()
        {
            var mockService = new Mock<IInternService>();
            var controller = new InternController(mockService.Object);
            var intern = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "Maria",
                Age = 22,
                DateOfBirth = DateTime.UtcNow.AddDays(10)
            };

            var result = await controller.Create(intern);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Date of birth cannot be in the future", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenAgeDoesNotMatchDateOfBirth()
        {
            var mockService = new Mock<IInternService>();
            var controller = new InternController(mockService.Object);
            var intern = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "Mismatch",
                Age = 30,
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };

            var result = await controller.Create(intern);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Age does not match date of birth", badRequest.Value);
        }

        #endregion

        #region Update() Method Tests

        [Fact]
        public async Task Update_ReturnsOk_WhenInternIsUpdated()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();
            var internToUpdate = new InternDTO
            {
                Id = internId,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };

            mockService.Setup(s => s.GetById(internId))
                    .ReturnsAsync(internToUpdate);

            mockService.Setup(s => s.Update(internId, internToUpdate))
                .ReturnsAsync(true);

            var controller = new InternController(mockService.Object);

            var result = await controller.Update(internToUpdate, internId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<InternDTO>(okResult.Value);
            Assert.Equal(internToUpdate.Id, model.Id);
            Assert.Equal(internToUpdate.Name, model.Name);
            Assert.Equal(internToUpdate.Age, model.Age);
            Assert.Equal(internToUpdate.DateOfBirth, model.DateOfBirth);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenInternIsNotInList()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();
            var internToUpdate = new InternDTO
            {
                Id = internId,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };

            mockService.Setup(s => s.GetById(internId))
                .ReturnsAsync((InternDTO?)null);

            var controller = new InternController(mockService.Object);

            var result = await controller.Update(internToUpdate, internId);

            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdIsMissing()
        {
            var mockService = new Mock<IInternService>();
            var internToUpdate = new InternDTO
            {
                Id = Guid.NewGuid(),
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };

            var controller = new InternController(mockService.Object);

            var result = await controller.Update(internToUpdate, Guid.Empty);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid intern ID", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInternDataIsNull()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();


            var controller = new InternController(mockService.Object);

            var result = await controller.Update(null, internId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Intern new data is missing", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenNameIsEmpty()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            var intern = new InternDTO
            {
                Id = internId,
                Name = "",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };

            var controller = new InternController(mockService.Object);

            var result = await controller.Update(intern, internId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Intern name cannot be empty", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenUnderEighteen()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            var intern = new InternDTO
            {
                Id = internId,
                Name = "Maria",
                Age = 17,
                DateOfBirth = DateTime.UtcNow.AddYears(-17)
            };

            var controller = new InternController(mockService.Object);

            var result = await controller.Update(intern, internId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Intern age must be at least 18", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenDateOfBirthIsInFuture()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            var intern = new InternDTO
            {
                Id = internId,
                Name = "Maria",
                Age = 22,
                DateOfBirth = DateTime.UtcNow.AddDays(10)
            };

            var controller = new InternController(mockService.Object);

            var result = await controller.Update(intern, internId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Date of birth cannot be in the future", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenAgeDoesNotMatchDateOfBirth()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            var intern = new InternDTO
            {
                Id = internId,
                Name = "Mismatch",
                Age = 30,
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };


            var controller = new InternController(mockService.Object);

            var result = await controller.Update(intern, internId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Age does not match date of birth", badRequest.Value);
        }

        #endregion

        #region Delete() Method Tests

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenInternIsDeleted()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();
            var internToDelete = new InternDTO
            {
                Id = internId,
                Name = "Andora",
                Age = 21,
                DateOfBirth = DateTime.UtcNow.AddYears(-21)
            };

            mockService.Setup(s => s.GetById(internId))
                .ReturnsAsync(internToDelete);

            mockService.Setup(s => s.Delete(internId))
                .ReturnsAsync(true);

            var controller = new InternController(mockService.Object);

            var result = await controller.DeleteIntern(internId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenIdIsMissing()
        {
            var mockService = new Mock<IInternService>();

            var controller = new InternController(mockService.Object);

            var result = await controller.DeleteIntern(Guid.Empty);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid intern ID", badRequest.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInternIsNotInList()
        {
            var mockService = new Mock<IInternService>();
            var internId = Guid.NewGuid();

            mockService.Setup(s => s.GetById(internId))
                .ReturnsAsync((InternDTO?)null);

            var controller = new InternController(mockService.Object);

            var result = await controller.DeleteIntern(internId);

            Assert.IsType<NotFoundResult>(result);
        }

        #endregion
    }
}
