
namespace funny.Logic
{
    public interface IOldSexLogic
    {
        Task<IEnumerable<string>> GetJokes(bool?[] sex, int?[] old);
    }
}
