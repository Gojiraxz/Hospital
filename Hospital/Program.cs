using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Hospital.Data;
using Hospital.Extensions;
using Hospital.Mapping;
using Hospital.Middleware;
using Hospital.Models.settings;
using Hospital.Repository;
using Hospital.Service;
using static Hospital.Authorization.JwtPermissionPolice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<jwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<smtpSettings>(builder.Configuration.GetSection("Smtp"));

var jwtConfig =
    builder.Configuration
    .GetSection("Jwt")
    .Get<jwtSettings>()
    ?? throw new InvalidOperationException("No esta presente la configuracion del JWT");

if (string.IsNullOrWhiteSpace(jwtConfig.key))
    throw new InvalidOperationException("Falta Jwt:key. Configurela con Secret Manager: dotnet user-secrets set \"Jwt:key\" \"<clave>\"");
if (Encoding.UTF8.GetBytes(jwtConfig.key).Length < 32)
    throw new InvalidOperationException("Jwt:key es demasiado corta: debe tener al menos 32 caracteres (si es mas corta el login falla con error 500).");

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
// Servicios del hospital (logica de negocio)
builder.Services.AddScoped<IEspecialidadService, EspecialidadService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IEnfermeroService, EnfermeroService>();
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
builder.Services.AddScoped<ILaboratorioService, LaboratorioService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IConsultaMedicaService, ConsultaMedicaService>();
builder.Services.AddScoped<IExamenMedicoService, ExamenMedicoService>();
builder.Services.AddScoped<ICirugiaService, CirugiaService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddScoped<IDetalleFacturaService, DetalleFacturaService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRoleRepository, RolRepository>();
builder.Services.AddScoped<IpermissionRepository, PermissionRepository>();
// Repositorios del hospital (acceso a datos)
builder.Services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IEnfermeroRepository, EnfermeroRepository>();
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();
builder.Services.AddScoped<IHabitacionRepository, HabitacionRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
builder.Services.AddScoped<ILaboratorioRepository, LaboratorioRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<IConsultaMedicaRepository, ConsultaMedicaRepository>();
builder.Services.AddScoped<IExamenMedicoRepository, ExamenMedicoRepository>();
builder.Services.AddScoped<ICirugiaRepository, CirugiaRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IDetalleFacturaRepository, DetalleFacturaRepository>();
builder.Services.AddScoped<IRecetaRepository, RecetaRepository>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<IEmailService, EmailService>();

//=======================DBContext=========================
builder.Services.AddDbContext<AppDBcontext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalDigital")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token =
                        context.Request.Cookies["access_token"];
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/Login");
                }

                return Task.CompletedTask;
            },

            OnForbidden = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/AccessDenied");
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.issuer,

                ValidateAudience = true,
                ValidAudience = jwtConfig.audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig.key)),

                ClockSkew = TimeSpan.Zero
            };
    });
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin.panel", policy =>
        policy.RequireAssertion(context =>
            context.User.HasPermission("rol.ver") ||
            context.User.HasPermission("permiso.ver") ||
            context.User.HasPermission("usuario.ver")));

    options.AddPolicy("permiso.actualizar", policy =>
        policy.RequireAssertion(context =>
            context.User.HasAnyPermission("permiso.actualizar", "permiso.crear")));

    options.AddPolicy("permiso.borrar", policy =>
        policy.RequireAssertion(context =>
            context.User.HasAnyPermission("permiso.borrar", "permiso.crear")));
});

var app = builder.Build();
// Crea la base de datos, las tablas, roles y permisos si todavia no existen
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDBcontext>();
        await DbSeeder.seedAsync(db);
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex, "No se pudo crear/conectar la base de datos. Revisa la cadena de conexion HospitalDB y que SQL Server LocalDB este instalado.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404 &&
          !response.HasStarted &&
         !context.HttpContext.Request.Path.StartsWithSegments("/Home/NotFoundPage"))
         {
        response.Redirect("/Home/NotFoundPage");
           }

    await Task.CompletedTask;
});
app.UseRouting();
app.UseSession();
app.UseMiddleware<JwtRefreshMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
