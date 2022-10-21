var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<RawgConfiguration>(new RawgConfiguration() {
    Endpoint = builder.Configuration.GetValue<string>("rawg:endpoint"),
    ApiKey = builder.Configuration.GetValue<string>("rawg:apiKey"),
});
builder.Services.AddScoped<IRawgRepository, RawgRepository>();

var app = builder.Build();

var cache = app.Services.GetService<IMemoryCache>();
cache.Set<List<User>>("Users", new List<User>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
