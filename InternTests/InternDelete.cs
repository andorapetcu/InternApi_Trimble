using InternApi.Controllers;
using InternApi.Services;
using InternApi.ModelDTO;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace InternTests
{
    public class InternDelete
    {
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
    }
}
