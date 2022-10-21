using saladtechnologies.RequestObjects;
namespace saladtechnologies.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private IRawgRepository _rawgRepository;
    private List<User> Users;
    private readonly List<string> AllowedComparisons = new() { "union", "intersection", "difference" };
    public UserController(ILogger<UserController> logger, IMemoryCache cache, IRawgRepository rawgRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rawgRepository = rawgRepository ?? throw new ArgumentNullException(nameof(rawgRepository));
        cache.TryGetValue("Users", out Users);
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{userId}")]
    public IActionResult Get([FromRoute]int userId)
    {
        var user = Users.FirstOrDefault(x => x.Id == userId);
        _logger.Log(LogLevel.Information, $"user {userId} exists? {user != null}");
        return user != null
            ? Ok(user)
            : BadRequest(new { message = "No user found with that userId" });
    }

    [HttpPost]
    public IActionResult Create()
    {
        var newUser = new User(Users.Count() + 1);
        Users.Add(newUser);
        return Created($"/User/{newUser.Id}", newUser);
    }

    [HttpPost("/users/{userId}/games")]
    public async Task<IActionResult> AddGame([FromRoute] int userId, [FromBody] GameRequestObject gameRequestObject)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            _logger.Log(LogLevel.Warning, $"No userId exists with {userId}");
            return BadRequest($"No userId exists with {userId}");
        }

        _logger.Log(LogLevel.Information, $"User found, with userId {user.Id}");

        var game = await _rawgRepository.GetGameById(gameRequestObject.GameId);
        if (game == null)
        {
            _logger.Log(LogLevel.Warning, $"No game exists with {gameRequestObject.GameId}");
            return BadRequest($"No gameId exists with {gameRequestObject.GameId}");
        }

        _logger.Log(LogLevel.Information, $"Game found, with gameId {game.Id}");

        if (user.Games.Select(g => g.Id).Contains(game.Id))
        {
            _logger.Log(LogLevel.Warning, $"{game.Id} already exists for user {userId}");
            return Conflict($"{game.Name} has already been added to user {userId}");
        }
        
        user.Games.Add(game);
        return NoContent();
    }

    [HttpDelete("/users/{userId}/games/{gameId}")]
    public async Task<IActionResult> DeleteGameFromUser([FromRoute]int userId, [FromRoute]int gameId)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            _logger.Log(LogLevel.Warning, $"No user found with id {userId}");
            return NotFound();
        }

        var game = user?.Games.FirstOrDefault(g => g.Id == gameId);
        if (game == null)
        {
            _logger.Log(LogLevel.Warning, $"No game found with id {gameId}");
            return NotFound();
        }
        user.Games.Remove(game);
        return NoContent();
    }

    [HttpPost("/users/{userId:int}/comparison")]
    public IActionResult CompareGames([FromRoute]int userId, [FromBody]GameComparisonRequestObject comparisonRequestObject)
    {
        if (!AllowedComparisons.Contains(comparisonRequestObject.Comparison))
        {
            _logger.Log(LogLevel.Warning, $"Incorrect usage of comparison, used {comparisonRequestObject.Comparison}");
            return BadRequest("Incorrect usage of comparison, please use one of the following available comparisons: " + 
                string.Join(", ", AllowedComparisons.Select(s => $"'{s}'")));
        }
        
        var firstUser = Users.FirstOrDefault(u => u.Id == userId);
        if (firstUser == null)
        {
            return NotFound($"User {userId} does not exist");
        }
        
        var secondUser = Users.FirstOrDefault(u => u.Id == comparisonRequestObject.OtherUserId);
        if (secondUser == null)
        {
            return NotFound($"'otherUser' {comparisonRequestObject.OtherUserId} does not exist");
        }

        var games = CompareGames(firstUser, secondUser, comparisonRequestObject.Comparison);
        return Ok(new { 
            userId = firstUser.Id,
            otherUserId = secondUser.Id,
            comparison = comparisonRequestObject.Comparison,
            games
        });
    }

    private IEnumerable<Game> CompareGames(User firstUser, User secondUser, string comparison)
    {
        _logger.Log(LogLevel.Trace, "First user's (" + firstUser.Id + ") games: " + string.Join(", ", firstUser.Games.Select(g => g.Id)));
        _logger.Log(LogLevel.Trace, "Second user's (" + secondUser.Id + ") games: " + string.Join(", ", secondUser.Games.Select(g => g.Id)));
        switch (comparison)
        {
            case "union": return firstUser.Games.Union<Game>(secondUser.Games).Distinct();
            case "intersection": return firstUser.Games.Intersect<Game>(secondUser.Games);
            case "difference": return firstUser.Games.Except<Game>(secondUser.Games);
        }

        return new List<Game>();
    }
}