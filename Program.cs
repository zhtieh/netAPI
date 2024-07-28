using Microsoft.EntityFrameworkCore;
using netAPI.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8,0,25))));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    // options.AddPolicy("AllowLocalhost",
    //     builder => builder.WithOrigins("http://127.0.0.1:8080",
    //                                     "http://localhost:5173",
    //                                     "http://127.0.0.1:5173") // Adjust the port as necessary
    //                         .AllowAnyMethod()
    //                         .AllowAnyHeader()
    //                         .AllowCredentials());

    options.AddPolicy("AllowLocalhost",
                      policy  =>
                      {
                          policy.WithOrigins("http://127.0.0.1:8080",
                                        "http://localhost:5173",
                                        "http://127.0.0.1:5173")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
