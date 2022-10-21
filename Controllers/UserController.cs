namespace saladtechnologies.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMemoryCache _cache;
    private IRawgRepository _rawgRepository;

    public UserController(ILogger<UserController> logger, IMemoryCache cache, IRawgRepository rawgRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _rawgRepository = rawgRepository ?? throw new ArgumentNullException(nameof(rawgRepository));
    }

    [HttpGet]
    public IActionResult Get(int userId)
    {
        // return $"Hello World {q} {sort}";
        _cache.TryGetValue("Users", out List<User> Users);
        var user = Users.FirstOrDefault(x => x.Id == userId);
        return user != null 
            ? Ok(user)
            : BadRequest(new { message = "No user found with that userId" });
    }

    [HttpPost]
    public IActionResult Create() 
    {
        _cache.TryGetValue("Users", out List<User> Users);
        var newUser = new User(Users.Count() + 1);
        Users.Add(newUser);
        return Created($"/User/{newUser.Id}", newUser);
    }

    [HttpPost("/users/{userId}/games")]
    public IActionResult AddGame([FromRoute]int userId, [FromBody]Game game)
    {
        return NotFound("Not yet implemented");
    }
}
