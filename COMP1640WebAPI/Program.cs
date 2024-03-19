using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using COMP1640WebAPI.DataAccess.Data;
using System.Reflection;
using COMP1640WebAPI.BusinesLogic.Repositories;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<COMP1640WebAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("COMP1640WebAPIContext") ?? throw new InvalidOperationException("Connection string 'COMP1640WebAPIContext' not found.")));
builder.Services.AddCors(options => { options.AddPolicy("AllowSpecificOrigin",
    build =>
    {
        build.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
builder.Services.AddScoped<UsersRepository>();
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
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
