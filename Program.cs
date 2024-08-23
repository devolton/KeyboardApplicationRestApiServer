using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.Entity;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.MaxValidationDepth = 64;
})
   
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("TypingTutorDb");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'TypingTutorDb' not found.");
}

// Add DbContext with connection string
builder.Services.AddScoped<TypingTutorDbContext>(provider => new TypingTutorDbContext(connectionString));
builder.Services.AddScoped<UserModel>();
builder.Services.AddScoped<TypingTestResultModel>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Database.SetInitializer(new TypingTutorDbInitializer());
        var context = services.GetRequiredService<TypingTutorDbContext>();
        context.Database.Initialize(true);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();
