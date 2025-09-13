using UTGenDemo.Service.Services;
using UTGenDemo.Service.Interfaces;
using UTGenDemo.Repository.Interfaces;
using UTGenDemo.Repository.Repositories;
using UTGenDemo.Controller.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();

// Register our services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmailService, MockEmailService>();

var app = builder.Build();

// Add exception middleware (must be first in the pipeline)
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
