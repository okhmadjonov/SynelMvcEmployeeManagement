using EmployeeManagement.Domain.Entity;
using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using Xunit;

namespace EmployeeManagement.UnitTest.Controller
{
    public class HomeControllerTest
    {
        private readonly HomeController _sut;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        private readonly Mock<IEmployeeRepository> _repositoryMock = new Mock<IEmployeeRepository>();
        private readonly Mock<ILogger<HomeController>> _loggerMock = new Mock<ILogger<HomeController>>();
        private readonly Mock<ICsvRepository> _loadCSVServiceMock = new Mock<ICsvRepository>();

        public HomeControllerTest()
        {
            _sut = new HomeController(_loggerMock.Object, _repositoryMock.Object, _loadCSVServiceMock.Object);
        }

        [Fact]
        public void HomeController_Index_ShouldReturnAListOfEmployees()
        {
            // Arrange
            _repositoryMock.Setup(p => p.GetAll())
                .Returns(GetTestEmployees());

            // Act
            var result = _sut.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.Model);
            Assert.Equal(GetTestEmployees().Count, model.Count());
        }

        [Fact]
        public async Task HomeController_Edit_ShouldReturnEmployee()
        {
            // Arrange
            var employeeId = 1;
            var forenames = "John";
            var surname = "William";
            var employee = new Employee
            {
                Id = employeeId,
                ForeNames = forenames,
                Surname = surname
            };
            _repositoryMock.Setup(p => p.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            var result = await _sut.Edit(employeeId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Employee>(viewResult.Model);
            Assert.Equal(employeeId, model.Id);
            Assert.Equal(forenames, model.ForeNames);
            Assert.Equal(surname, model.Surname);
        }

        [Fact]
        public async Task HomeController_Edit_ShouldReturnBadRequestResultWhenIdIsNull()
        {
            // Arrange
            int? employeeId = null;

            // Act
            var result = await _sut.Edit(employeeId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task HomeController_Edit_ShouldReturnNotFoundResultWhenEmployeeNotFound()
        {
            // Arrange
            int? employeeId = 1;
            _repositoryMock.Setup(p => p.GetByIdAsync(employeeId.Value))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.Edit(employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        /*
        [Fact]
        public async Task HomeController_LoadCSV_ShouldReturnAddedRowsJsonResult()
        {
            Arrange
           var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net7.0", string.Empty));
            var testFilePath = Path.Combine(dirName, @"Assets\dataset.csv");
            using var streamReader = new StreamReader(testFilePath);
            using var stream = File.OpenRead(testFilePath);
            _loadCSVServiceMock.Setup(p => p.LoadFromCSVAsync(streamReader))
                .ReturnsAsync(2);

            Act
           var result = await _sut.LoadCSV(new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(testFilePath)));

            Assert
            Assert.IsType<JsonResult>(result);
        }
        */
        private List<Employee> GetTestEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    ForeNames = "John",
                    Surname = "William",
                },
                new Employee
                {
                    Id = 1,
                    ForeNames = "Jerry",
                    Surname = "Jackson",
                }
            };

            return employees;
        }
    }
}