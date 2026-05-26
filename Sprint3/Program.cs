using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Sprint3.Data;
using Sprint3.Repositories;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services;
using Sprint3.Services.Interfaces;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var appPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(appPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{appPort}");
}

LoadDotEnv(builder.Configuration, builder.Environment.ContentRootPath);

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TASKPI API",
        Version = "v1",
        Description = "API para autenticação, projetos, atividades, tarefas, convites e perfil de usuários."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu_token}"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddScoped<ITarefaService, TarefaService>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<IProjetoAcessoRepository, ProjetoAcessoRepository>();
builder.Services.AddScoped<IProjetoConviteRepository, ProjetoConviteRepository>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("Jwt__SecretKey was not found. Configure it in Sprint3/.env or in the environment variables.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "authToken";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.Path = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.LoginPath = "/index.html";
        options.AccessDeniedPath = "/index.html";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TASKPI API v1");
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

static void LoadDotEnv(ConfigurationManager configuration, string contentRootPath)
{
    var candidates = new[]
    {
        Path.Combine(contentRootPath, ".env"),
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(Directory.GetCurrentDirectory(), "Sprint3", ".env")
    };

    var envPath = candidates
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .FirstOrDefault(File.Exists);

    if (envPath is null)
    {
        return;
    }

    var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    foreach (var rawLine in File.ReadAllLines(envPath))
    {
        var line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith("#"))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim().Replace("__", ":");
        var value = line[(separatorIndex + 1)..].Trim();
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            value = value[1..^1];
        }

        values[key] = value;
    }

    configuration.AddInMemoryCollection(values);
    configuration.AddEnvironmentVariables();
}
