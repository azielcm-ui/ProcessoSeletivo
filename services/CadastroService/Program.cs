using Cadastro.Domain;
using Cadastro.Infrastructure;
using CadastroService.Domain;
using CadastroService.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
//using Cadastro.Application;
using RabbitMQ.Client;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetValue<string>("ConnectionStrings:Default") 
           ?? "Host=postgres;Database=cadastro_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<CadastroDbContext>(opts =>
    opts.UseNpgsql(conn));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<CadastroDbContext>()
    .AddDefaultTokenProviders();

// Token service
builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = builder.Configuration.GetValue<string>("RABBITMQ__HOST") ?? "rabbitmq" };
    return new RabbitMqPersistentConnection(factory);
});

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddHangfire(hf => hf.UsePostgreSqlStorage(conn));
builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CadastroDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();
