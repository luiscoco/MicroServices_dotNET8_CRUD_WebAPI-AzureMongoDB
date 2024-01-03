using BookStoreApi.Models;
using BookStoreApi.Services;
using MongoDB.Driver;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// MongoDB configuration
string connectionString =
    @"mongodb://mymongodbinazure:4rriLsdDhrtgcjtb2N6LON1sVoMNyujurKiWzhKXoS5cNdNloIZ8pPclPqNVsoilPK4QlnuQjtIfACDb6AY2mA==@mymongodbinazure.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@mymongodbinazure@";
MongoClientSettings settings = MongoClientSettings.FromUrl(
    new MongoUrl(connectionString)
);
settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

var mongoClient = new MongoClient(settings);

// Register MongoClient with DI container
builder.Services.AddSingleton<IMongoClient>(mongoClient);


ConfigurationManager Configuration = builder.Configuration;

// Add services to the container.
builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));

builder.Services.AddSingleton<BooksService>();

//builder.Services.AddHttpContextAccessor();

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
