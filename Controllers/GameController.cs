using System.Net.Http.Json;

namespace saladtechnologies.Controllers;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private IConfiguration _config;
    private readonly ILogger<GameController> _logger;
    private readonly List<string> AvalabileOrdering = new List<string>()
    {
        "name", "released", "added", "created", "updated", "rating", "metacritic",
        "-name", "-released", "-added", "-created", "-updated", "-rating", "-metacritic",
    };

    public GameController(IConfiguration config, ILogger<GameController> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Get(string q, string? sort)
    {
        if (string.IsNullOrWhiteSpace(q)) 
        {
            _logger.Log(LogLevel.Warning, "No query was supplied");
            return BadRequest("Please provide a query, such as '?q=undertale' in the query");
        }
        if (!string.IsNullOrWhiteSpace(sort) && !AvalabileOrdering.Contains(sort)) 
        {
            _logger.Log(LogLevel.Warning, $"Improper use of sort, with value '{sort}'");
            return BadRequest("Improper use of sort, please use the following valid values: " + 
                string.Join(",", AvalabileOrdering.Select(s => $"'{s}'")));
        }
        var games = await GetGames(q, sort);
        return games == null ? BadRequest("No games found") : Ok(games);
    }

    private async Task<IEnumerable<Game>> GetGames(string query, string sort)
    {

        var (endpoint, key) = GetRawgApi();
        var paramString = $"?key={key}&search={query}";
        if (!string.IsNullOrWhiteSpace(sort))
        {
            paramString += $"&ordering={sort}";
        }

        _logger.Log(LogLevel.Debug, $"endpoint: {endpoint}, key: {key}, paramString: {paramString}");

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(endpoint);
        var rawgResponse = await httpClient.GetFromJsonAsync<RawgResponse>(paramString);
        return rawgResponse.Results;
    }

    private (string endpoint, string key) GetRawgApi() 
    {
        var endpoint = _config.GetValue<string>("RawgApiEndpoint");
        var key = _config.GetValue<string>("RawgApiKey");
        return (endpoint, key);
    }
}
