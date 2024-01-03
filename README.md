# How to create a .NET8 WebAPI CRUD MongoDB Microservice

The code for this example is available in this github repo: https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB

This application is very similar to the one we developed to connect to a MongDB in a local Docker container, but in this case we are going to connect our WebAPI to Azure MongoDB account.

The main changes we introduce are in **Program.cs** and **appsettings.json** files:

In the **appsettings.json** file we include the Azure MongoDB connection string, and in the **Program.cs** file we include these changes:

```csharp
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
```

## 0. Prerequisites

- Install Docker Desktop

- Install Visual Studio 2022 Community Edition version 17.8

- Install Studio 3T Free for MongoDB

## 1. Create Azure CosmosDB

We navigate to **Azure CosmosDB** service and we click in this option to create a new account:

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/448847ba-4e3c-4048-a477-980bada707dd)

We click on **Azure CosmosDB account** button

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/d9aa0fce-e126-476a-b97a-1db075097e7a)

Now we select the option Azure CosmosDB for MongoDB, and we press the **create** button

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/7b3640b0-1dee-49c5-aa9b-bce605b271e6)

In order to optimize the service cost we choose the option **Request unit (RU) database account** 

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/916abf0b-6c7c-4723-a50e-3edb13b84f3f)

In following screen we input the required data for creating the service

We create a new ResourceGroup: myRG

We set the service name: myazuremongodbaccount

We choose the service location/region: West Europe

Capacity mode: **serverless**

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/ce10ab29-a6c4-4719-a3b3-8950ed4e106a)

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/3807c092-9ee5-4042-8d43-4d78f14ba697)

By default the first screen we are going to see after the service creation in the **QuickStart** page

Pay attention and copy the ConnectionString values and configuration to set up your **program.cs** and **appsettings.json** files in your .NET WebAPI 

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/06c6ce1f-5ee5-4fce-b5f2-3bfd3c277a37)

We navigate to the **Data Explorer** page in order to create a new database and collection, and populate it with two documents

We press the **Open Mongo Shell** button

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/9d31294c-491b-49b4-9842-8b4aa9118d1f)

Run these commands in you terminal window for creating the new database, collection and documents

To list the databases run the command: 

```
show databases 
```

To switch or create the database “BookStore” run the command: 

```
use BookStore 
```

To create a new collection “Books” run the command 

```
db.createCollection('Books') 
```

You can also show the collections inside the database running the command: 

```
show collections
```

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/5bc6d63f-e5b2-46cb-b54c-2814d32879ba)

To insert may items/documents inside the “Books” collections then type the command: 

```
db.Books.insertMany([{ "Name": "Design Patterns", "Price": 54.93, "Category": "Computers", 
"Author": "Ralph Johnson" }, { "Name": "Clean Code", "Price": 43.15, "Category": 
"Computers","Author": "Robert C. Martin" }])
```

Finally, if you would like to show the items/documents inside the “Books” collection then type 
the command: 

```
db.Books.find().pretty() 
```

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-AzureMongoDB/assets/32194879/07e481be-7a3c-4dca-aa04-4d34a9cf663a)

## 2. Create .NET8 WebAPI in Visual Studio 2022 Community Edition

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-MongoDB/assets/32194879/a8457769-9718-4954-9d14-6a3950cf8b53)

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-MongoDB/assets/32194879/dec4d20a-1283-4093-98b4-b42e07732742)

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-MongoDB/assets/32194879/34e18f05-54ae-42c3-bf08-e95cf2796537)

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-MongoDB/assets/32194879/719673cc-23c0-4f7a-a037-689b04eb5685)

## 3. Add the MongoDB.Driver dependency

Select the menu option Tools->Nuget Package Manager->Manage Nuget Packages for Solution...

Then browse Mongo.DB.Driver and install it in your solution

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-MongoDB/assets/32194879/c793e87e-eb6a-4464-b9d5-d50a2d5b71b3)

## 4. Add the Models

Create the Models folder and inside include the following two files:

**Book.cs**

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BookStoreApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        [JsonPropertyName("Name")]
        public string BookName { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        public string Author { get; set; } = null!;
    }
}
```

**BookStoreDatabaseSettings.cs**

```csharp
namespace BookStoreApi.Models
{
    public class BookStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string BooksCollectionName { get; set; } = null!;
    }
}
```

## 5. Add the Service

Create a Services folder with the following file:

**BookService.cs**

```csharp
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services
{
    public class BooksService
    {
        private readonly IMongoCollection<Book> _booksCollection;

        public BooksService(
            IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _booksCollection = mongoDatabase.GetCollection<Book>(
                bookStoreDatabaseSettings.Value.BooksCollectionName);
        }

        public async Task<List<Book>> GetAsync() =>
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book?> GetAsync(string id) =>
            await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Book newBook) =>
            await _booksCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Book updatedBook) =>
            await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _booksCollection.DeleteOneAsync(x => x.Id == id);
    }
}
```

## 6. Add the Controller

In the Controllers folder include the following file:

**BooksController.cs**

```csharp
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;

    public BooksController(BooksService booksService) =>
        _booksService = booksService;

    [HttpGet]
    public async Task<List<Book>> Get() =>
        await _booksService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Book newBook)
    {
        await _booksService.CreateAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Book updatedBook)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedBook.Id = book.Id;

        await _booksService.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _booksService.RemoveAsync(id);

        return NoContent();
    }
}
```

## 7. Modify Program.cs file

In the **Program.cs** file include the following code:

```csharp
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
```

## 8. Modify appsettings.json file

In the **appsettings.json** file include the following code:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "BookStoreDatabase": {
    "ConnectionString": "mongodb://mymongodbinazure:4rriLsdDhrtgcjtb2N6LON1sVoMNyujurKiWzhKXoS5cNdNloIZ8pPclPqNVsoilPK4QlnuQjtIfACDb6AY2mA==@mymongodbinazure.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@mymongodbinazure@",
    "DatabaseName": "test",
    "BooksCollectionName": "Books"
  },
  "AllowedHosts": "*"
}
```

**Logging Configuration**:

"**Logging**": This section configures the logging behavior of the application.

"**LogLevel**": This subsection specifies the minimum level of events to log.

"**Default**": "Information": By default, the application logs events that are at the "Information" level or higher. "Information" level typically includes general application flow events and operational information.

"**Microsoft.AspNetCore**": "**Warning**": For components from the "Microsoft.AspNetCore" namespace, only "Warning" level events or higher are logged. "Warning" level logs are used for potentially harmful situations or cautionary messages.

**Database Configuration for a Book Store**:

"**BookStoreDatabase**": This section contains settings specific to a database used by a Book Store application.

"**ConnectionString**": "mongodb://localhost:27017": Defines the connection string for the database. This particular string indicates that the application connects to a MongoDB instance running on localhost (the same machine where the application is running) and listening on port 27017.

"**DatabaseName**": "**BookStore**": Specifies the name of the database within MongoDB to be used, which is "BookStore" in this case.

"**BooksCollectionName**": "**Books**": Indicates the name of the collection within the "BookStore" database that will store the book data. In MongoDB, a collection is analogous to a table in a relational database.

**Allowed Hosts Configuration**:

"**AllowedHosts**": "*": This setting configures which hosts are allowed to send requests to the application. The asterisk (*) is a wildcard that means any host can send requests. This is an important setting for web applications, especially concerning CORS (Cross-Origin Resource Sharing) policies.

Overall, this JSON file is used to configure logging, database connections, and security policies for a web application, likely built with technologies like ASP.NET Core (indicated by the Microsoft.AspNetCore logging configuration).

## 9. How to run the application

### 9.1. First, we need to pull and run the MondoDB database Docker container image

For pulling the MondoDB docker image from Docker hub we run these commands

```
docker login
```

```
docker pull mongo
```

For running the Mongodb docker image we type the command:

```
docker run -d -p 27017:27017 --name mongodb-container mongo
```

To enter in the Mongodb database we type this command:

```
docker exec -it mongodb-container mongosh
```

And also we create a new database with a collections and two documents inside, see this picture:



### 9.2. Second, we verify the data with 3T Studio Free for MongoDb

We connect to the MongoDB running container from 3T Studio setting the connection string: **mongodb://localhost:27017**



We set the connection name



And we connect to the database



See the data inside the new database and collection


### 9.3. Third, we build and run the WebAPI application with HTTP protocol




