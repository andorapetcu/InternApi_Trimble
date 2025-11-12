using InternApi.Services;
using Moq;
using InternApi.ModelDTO;
using InternApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using NuGet.Frameworks;

namespace InternTests
{
    public class InternUpdate
    {
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
    }
}
