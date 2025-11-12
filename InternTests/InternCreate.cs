using InternApi.Controllers;
using InternApi.ModelDTO;
using InternApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InternTests
{
    public class InternCreate
    {
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
    }
}
