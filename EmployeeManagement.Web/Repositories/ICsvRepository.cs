namespace EmployeeManagement.Web.Repositories
{
    public interface ICsvRepository
    {
        Task<int> LoadFromCSVAsync(StreamReader stream);
    }
}
