using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SISDOMI.Helpers;
using SISDOMI.Services;
using Microsoft.OpenApi.Models;
using SISDOMI.Hubs;

namespace SISDOMI
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        readonly string PolizaCORSSISCAR = "_polizaCORSSISCAR";
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy(name: PolizaCORSSISCAR,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:8080")
                                                .AllowAnyMethod()
                                                .AllowAnyHeader();
                                  });
            });
            services.AddControllers();
            services.Configure<SysdomiDatabaseSettings>(
                Configuration.GetSection(nameof(SysdomiDatabaseSettings)));
            services.AddCors();
            services.AddSingleton<ISysdomiDatabaseSettings>(sp =>
               sp.GetRequiredService<IOptions<SysdomiDatabaseSettings>>().Value);
            services.AddScoped<UsuarioService>();
            services.AddScoped<ResidenteService>();
            services.AddScoped<PlanIntervencionIndividualService>();
            services.AddScoped<FichaIngresoEducativoService>();
            services.AddScoped<SeguimientoEducativoService>();
            services.AddScoped<ExpedienteService>();
            services.AddScoped<InformeService>();
            services.AddScoped<FichaIngresoSocialService>();
            services.AddScoped<SesionesEducativasService>();
            services.AddScoped<FichaIngresoPsicologicaService>();
            services.AddScoped<TallerService>();
            services.AddScoped<IncidenciaService>();
            services.AddScoped<MediaService>();
            services.AddScoped<RolService>();
            services.AddScoped<FaseService>();
            services.AddScoped<NotificacionService>();
            services.AddScoped<AnexoService>();
            services.AddScoped<ActaDeExternamientoService>();
            services.AddScoped<DocumentoService>();
            services.AddScoped<FichaEvaluacionDiagnosticoEducativoService>();
            services.AddScoped<EstadisticasService>();
            services.AddScoped<PerfilService>();
            services.AddScoped<UbigeoService>();
            services.AddScoped<HojaProductividadService>();
            services.AddScoped<ActividadService>();
            services.AddScoped<EntrevistaFamiliarService>();

            services.AddScoped<IFileStorage, AzureFileStorage>();
            services.AddScoped<IDocument, Document>();

            //need default token provider
            services.AddAuthentication(JwtBearerDefaults
             .AuthenticationScheme)
                 .AddJwtBearer(options =>
              options.TokenValidationParameters =
              new TokenValidationParameters
              {
                  ValidateIssuer = false,
                  ValidateAudience = false,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(
                 //llave secreta que dice si el token es valido
                 Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                  ClockSkew = TimeSpan.Zero
              });

            // Se encarga de registrar el generador del swagger
            services.AddSwaggerGen(g =>
            {
                g.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "versión 1.0",
                    Title = "SISCAR API",
                    Description = "Aplicación que contiene la descripción y uso de las APIS del SISCAR"
                });

                g.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autorización para la entradas a las apis que generan la cabecera",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                g.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {  new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                   }
                });

            });
            //Notificaciones:
            //services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(PolizaCORSSISCAR);

            //Habilita el uso del swagger
            app.UseSwagger();

            //Habilita el uso de la interfaz del swagger
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "SISCAR API V1.0");
            });


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //Notificaciones:
                //endpoints.MapHub<NotificationHub>("/NotificationHub");
            });
        }
    }
}
