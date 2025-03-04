using EmployeeManagement.Domain.Entity;

namespace EmployeeManagement.Web.Repositories
{
    public interface IEmployeeRepository
    {

        IEnumerable<Employee> GetAll();
        Employee GetById(int id);
        Task<Employee> GetByIdAsync(int id);
        IEnumerable<Employee> Find(Func<Employee, Boolean> predicate);
        public Task CreateRangeAsync(IEnumerable<Employee> employees);
        void Update(Employee employee);
        public void Save();
        public Task SaveAsync();

    }
}
