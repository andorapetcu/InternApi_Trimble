using InternApi.Controllers;
using InternApi.ModelDTO;
using InternApi.Services;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace InternTests
{
    public class InternGetByIdTest
    {
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
    }
}