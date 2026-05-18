using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sprint3.Data;
using Sprint3.Repositories;
using Sprint3.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITarefaService, TarefaService>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrWhiteSpace(context.Token) &&
                    context.Request.Cookies.TryGetValue("authToken", out var cookieToken))
                {
                    context.Token = cookieToken;
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
}
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tarefas v1");
       
    });
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals("/MainScreenComponent/MainScreen.html", StringComparison.OrdinalIgnoreCase))
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await next();
            return;
        }
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync("Not Found");
        return;
    }
    await next();
});

app.UseStaticFiles();

app.MapControllers();

app.Run();
