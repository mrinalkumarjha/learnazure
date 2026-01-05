using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using testApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with Entra token
// builder.Services.AddDbContext<MyDbContext>(static async (sp, options) =>
// {
//     var cfg = sp.GetRequiredService<IConfiguration>();
//     var baseConn = cfg.GetConnectionString("Sql");

//     var credential = new DefaultAzureCredential();
//     var token = await credential.GetTokenAsync(
//         new TokenRequestContext(new[] { "https://database.windows.net/.default" }));

//     var sqlConn = new SqlConnection(baseConn) { AccessToken = token.Token };

//     options.UseSqlServer(sqlConn, sql =>
//     {
//         // Recommended for transient network errors (App Service ↔ SQL)
//         sql.EnableRetryOnFailure();
//     });
// });



var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
