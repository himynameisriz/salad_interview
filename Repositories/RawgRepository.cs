class RawgRepository : IRawgRepository
{
    private string _apiKey;
    private HttpClient _rawgClient;
    private ILogger<RawgRepository> _logger;
    private readonly IServiceProvider _serviceProvider;
    public RawgRepository(IServiceProvider serviceProvider, RawgConfiguration rawgConfiguration, ILogger<RawgRepository> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (rawgConfiguration == null ) 
        {
            throw new ArgumentNullException(nameof(rawgConfiguration));
        }
        _apiKey = rawgConfiguration.ApiKey;
        _rawgClient = new HttpClient() { BaseAddress = new Uri(rawgConfiguration.Endpoint) };
    }
    public async Task<Game> GetGameById(int id)
    {
        var paramString = $"/{id}?key={_apiKey}";
        _logger.Log(LogLevel.Information, $"paramString = '{paramString}'");        
        return await _rawgClient.GetFromJsonAsync<Game>(paramString);
    }

    public async Task<IEnumerable<Game>> GetGames(string query, string sort)
    {
        var paramString = $"?key={_apiKey}&search={query}";
        if (!string.IsNullOrWhiteSpace(sort))
        {
            paramString += $"&ordering={sort}";
        }
        _logger.Log(LogLevel.Information, $"paramString = '{paramString}'");        
        var rawgResponse = await _rawgClient.GetFromJsonAsync<RawgResponse>(paramString);
        return rawgResponse.Results;
    }
}