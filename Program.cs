using iikoTask.Contracts;
using iikoTask.Data;
using iikoTask.Interface;
using iikoTask.Interfaces;
using iikoTask.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<ApplicationDbContext>(
//    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("TestDb"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
    build => build.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
});


var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    dbContext.Database.Migrate();
//}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.Run();
