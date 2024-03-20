using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using COMP1640WebAPI.DataAccess.Data;
using System.Reflection;
using COMP1640WebAPI.BusinesLogic.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
//var key = Encoding.ASCII.GetBytes("API-key");
//var jwtKey = Configuration["Authentication:JwtKey"];
builder.Services.AddDbContext<COMP1640WebAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("COMP1640WebAPIContext") ?? throw new InvalidOperationException("Connection string 'COMP1640WebAPIContext' not found.")));

builder.Services.AddDbContext<AuthDataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AuthContext")));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<AuthDataContext>();

builder.Services.AddCors(options => { options.AddPolicy("AllowSpecificOrigin",
    build =>
    {
        build.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

//builder.Services.AddAuthentication().AddJwtBearer();

// Add services to the container.
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
var app = builder.Build();
//var httpContextAccessor = application.Services.GetRequiredService<IHttpContextAccessor>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapIdentityApi<IdentityUser>();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

//app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
