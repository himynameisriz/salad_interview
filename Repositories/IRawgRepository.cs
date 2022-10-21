public interface IRawgRepository
{
    Task<IEnumerable<Game>> GetGames(string query, string sort);
    Task<Game> GetGameById(int id);
}