namespace saladtechnologies.Controllers;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private IRawgRepository _rawgRepository;
    private readonly ILogger<GameController> _logger;
    private readonly List<string> AvalabileOrdering = new()
    {
        "name", "released", "added", "created", "updated", "rating", "metacritic",
        "-name", "-released", "-added", "-created", "-updated", "-rating", "-metacritic",
    };

    public GameController(IRawgRepository rawgRepository, ILogger<GameController> logger)
    {
        _rawgRepository = rawgRepository ?? throw new ArgumentNullException(nameof(rawgRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Get(string q, string? sort = "")
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
        var games = await _rawgRepository.GetGames(q, sort);
        return games == null ? BadRequest("No games found") : Ok(games);
    }
}