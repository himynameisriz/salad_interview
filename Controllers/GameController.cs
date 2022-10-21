using System.Net.Http.Json;

namespace saladtechnologies.Controllers;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private IConfiguration _config;
    private readonly ILogger<GameController> _logger;

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
            return BadRequest();
        }

        var game = await GetGames(q, sort);
        return game == null ? BadRequest("No games found") : Ok(game);
    }

    private async Task<Game> GetGames(string query, string sort)
    {

        var (endpoint, key) = GetRawgApi();
        var paramString = $"?key={key}&search={query}";

        _logger.Log(LogLevel.Debug, $"endpoint: {endpoint}, key: {key}, paramString: {paramString}");

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(endpoint);
        var rawgResponse = await httpClient.GetFromJsonAsync<RawgResponse>(paramString);
        return rawgResponse.Results.FirstOrDefault();
    }

    private (string endpoint, string key) GetRawgApi() 
    {
        var endpoint = _config.GetValue<string>("RawgApiEndpoint");
        var key = _config.GetValue<string>("RawgApiKey");
        return (endpoint, key);
    }
}
