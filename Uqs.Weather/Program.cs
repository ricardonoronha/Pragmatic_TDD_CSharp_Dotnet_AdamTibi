
using Uqs.Weather.Providers;
using Uqs.Weather.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IRandomProvider>(_ => new RandomProvider());
builder.Services.AddSingleton<IDateTimeProvider>(_ => new DateTimeProvider());
builder.Services.AddScoped<IClient>(_ =>
{
    string apiKey = builder.Configuration["OpenWeather:Key"];
    var httpClient = new HttpClient();
    var client = new Client(apiKey, httpClient);
    return client;
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
