using CsvHelper.Configuration;
using CsvHelper;
using EmployeeManagement.Domain.Entity;
using EmployeeManagement.Web.Repositories;
using OfficeOpenXml;
using System.Globalization;

namespace EmployeeManagement.Web.Services
{
    public class CsvService : ICsvRepository
    {

        private readonly IEmployeeRepository _employeeRepository;

        public CsvService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }



        // ********************  LOAD FROM CSV    *****************************

        public async Task<int> LoadFromCSVAsync(StreamReader stream)
        {
            var employees = GetEmployeesCSV(stream);

            // add data and save to db
            await _employeeRepository.CreateRangeAsync(employees);
            await _employeeRepository.SaveAsync();

            return employees.Count();
        }



        // ********************  GET EMPLOYEES CSV   *****************************
        private List<Employee> GetEmployeesCSV(StreamReader stream)
        {

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };

            using var csv = new CsvReader(stream, config);

            csv.Context.RegisterClassMap<EmployeeClassMap>();

            var employees = csv.GetRecords<Employee>().ToList();

            return employees;

        }


        // ******************** RETURN EMPLOYEES AS EXCEL   *****************************


        private async Task<List<Employee>> GetEmployeesExcel(Stream stream)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            await package.LoadAsync(stream);
            var ws = package.Workbook.Worksheets[0];

            // check if the worksheet is completely empty
            if (ws.Dimension == null)
            {
                // return an empty list
                return new List<Employee>();
            }

            List<Employee> employees = new List<Employee>();

            // loop and get contents
            for (int i = 2; i <= ws.Dimension.End.Row; i++)
            {
                var row = ws.Cells[i, 1, i, ws.Dimension.End.Column];

                var col = 1;
                Employee employee = new Employee();

                // loop all cells in the row
                employee.PayrollNumber = row[i, col].Value.ToString();
                employee.ForeNames = row[i, col + 1].Value.ToString();
                employee.Surname = row[i, col + 2].Value.ToString();
                employee.DateOfBirth = DateTime.Parse(row[i, col + 3].Value.ToString());
                employee.Telephone = row[i, col + 4].Value.ToString();
                employee.Mobile = row[i, col + 5].Value.ToString();
                employee.Address = row[i, col + 6].Value.ToString();
                employee.Address2 = row[i, col + 7].Value.ToString();
                employee.Postcode = row[i, col + 8].Value.ToString();
                employee.EmailHome = row[i, col + 9].Value.ToString();
                employee.StartDate = DateTime.Parse(row[i, col + 10].Value.ToString());

                employees.Add(employee);
            }

            return employees;

        }


    }

}
