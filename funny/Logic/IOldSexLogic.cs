
namespace funny.Logic
{
    public interface IOldSexLogic
    {
        Task<string> GetJoke(bool sex, int old);
        Task<IEnumerable<string>> GetJokes(bool?[] sex, int?[] old);
        Task<IEnumerable<string>> GetAllJokes();
    }
}
