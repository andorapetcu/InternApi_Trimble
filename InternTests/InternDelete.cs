using InternApi.Controllers;
using InternApi.Services;
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

            mockService.Setup(s => s.Delete(internId))
                .ReturnsAsync(true);

            var controller = new InternController(mockService.Object);

            var result = await controller.DeleteIntern(internId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInternDoesNotExist()
        {
            var mockService = new Mock<IInternService>();

            var internId = Guid.NewGuid();

            mockService.Setup(s => s.Delete(internId))
                .ReturnsAsync(false);

            var controller = new InternController(mockService.Object);

            var result = await controller.DeleteIntern(internId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
