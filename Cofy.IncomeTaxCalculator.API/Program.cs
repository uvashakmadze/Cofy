using Cofy.IncomeTaxCalculator.API.Extensions;
using Cofy.IncomeTaxCalculator.API.MiddleWares;
using Cofy.IncomeTaxCalculator.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBasicAuthentication();
builder.Services.AddSwaggerOpenApi(builder.Configuration);

builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<SwaggerBasicAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
