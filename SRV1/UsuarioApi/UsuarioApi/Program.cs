using Microsoft.EntityFrameworkCore;
using UsuarioApi.Business;
using UsuarioApi.Configuration;
using UsuarioApi.DataAccess;
using UsuarioApi.Filter;


var builder = WebApplication.CreateBuilder(args);

//Config de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Confi del token
builder.Services.Configure<TokenValidationOptions>(
    builder.Configuration.GetSection("TokenValidation"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<Validate>();

//Config DbContext
builder.Services.AddDbContext<CarnetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Inyeccion de Business
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();

// Config del middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
