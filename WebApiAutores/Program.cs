using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores;
using WebApiAutores.Filtros;
using WebApiAutores.Middleware;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

[assembly:ApiConventionType(typeof(DefaultApiConventions))]
var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
//Agregar intterfaces
builder.Services.AddAutoMapper(typeof(Program));
// Add services to the container.

builder.Services.AddControllers(options=> 
        { 
            options.Filters.Add(typeof(FiltrodeExcepcion));
            options.Conventions.Add(new SwaggerAgrupaPorVersion());
        }).AddJsonOptions(x=>x.JsonSerializerOptions.ReferenceHandler= ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title="WebApiAutores",Version="v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAutores", Version = "v2" });

    c.OperationFilter<AgregarParametroHATEOAS>();
    c.OperationFilter<AgregarParametroXVersion>();

    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name }.xml";
    var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
    c.IncludeXmlComments(rutaXML);
});


builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
});

builder.Services.AddDataProtection();
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders(new string[] { "cantidadTotalRegistro"});
    });
});

builder.Services.AddTransient<GeneradorEnlaces>();
builder.Services.AddTransient<HATEOASAutorFilterAttribute>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddApplicationInsightsTelemetry();
//builder.Services.AddResponseCaching();
//Configura el uso de la cadena de conexión entre la app y la bd


builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer( opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
        ClockSkew = TimeSpan.Zero
    });

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();
var config = app.Configuration;

// Configure the HTTP request pipeline.

//app.Map("/ruta1", app =>
//{
//    app.Run(async contexto =>
//    {
//        await contexto.Response.WriteAsync("Estoy interceptando a tuberia");
//    });
//});
//app.UseMiddleware<LoggearRespuestaHTTPMiddleware>();

app.UseLoggearRespuestaHTTP();
//app.Use(async (contexto, siguiente) =>
//{
//    using (var ms = new MemoryStream()) 
//    {
//        var cuerpoOriginalRespuesta = contexto.Response.Body;
//        contexto.Response.Body = ms;

//        await siguiente.Invoke();

//        ms.Seek(0, SeekOrigin.Begin);
//        string respuesta = new StreamReader(ms).ReadToEnd();
//        ms.Seek(0, SeekOrigin.Begin);

//        await ms.CopyToAsync(cuerpoOriginalRespuesta);
//        contexto.Response.Body = cuerpoOriginalRespuesta;
//        app.Logger.LogInformation(respuesta);
//    }
//});
if (app.Environment.IsDevelopment())
{
    
}
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("v1/swagger.json", "WebApiAutores v1");
    c.SwaggerEndpoint("v2/swagger.json", "WebApiAutores v2");
});
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();
//app.UseResponseCaching();

app.UseAuthorization();

app.UseEndpoints(enpoints => {
    enpoints.MapControllers();
});

app.Run();
