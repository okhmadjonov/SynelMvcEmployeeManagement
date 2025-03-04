using EmployeeManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Domain.Entity;
using EmployeeManagement.Context.DataContext;


namespace EmployeeManagement.Web.Services
{
    public class EmployeeService : IEmployeeRepository
    {
        private readonly AppDataContext _context;


        public EmployeeService(AppDataContext appDataContext)
        {
            _context = appDataContext;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees;
        }

        public Employee GetById(int id)
        {
            return _context.Employees.Find(id);
        }


        public async Task<Employee> GetByIdAsync(int id)
        {

            return await _context.Employees.FindAsync(id);

        }

        public IEnumerable<Employee> Find(Func<Employee, bool> predicate)
        {
            return _context.Employees.Where(predicate).ToList();
        }

        public async Task CreateRangeAsync(IEnumerable<Employee> employees)
        {
            await _context.Employees.AddRangeAsync(employees);
        }

        public void Update(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
