using InternApi.Services;
using Moq;
using InternApi.ModelDTO;
using InternApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace InternTests
{
    public class InternGetAllTest
    {
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
    }
}
