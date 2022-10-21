using saladtechnologies.RequestObjects;
namespace saladtechnologies.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private IRawgRepository _rawgRepository;
    private List<User> Users;
    public UserController(ILogger<UserController> logger, IMemoryCache cache, IRawgRepository rawgRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rawgRepository = rawgRepository ?? throw new ArgumentNullException(nameof(rawgRepository));
        cache.TryGetValue("Users", out Users);
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
}