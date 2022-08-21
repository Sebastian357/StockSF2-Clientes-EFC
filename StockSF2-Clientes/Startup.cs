using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

namespace StockSF2_Clientes
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //Adds services for controllers to the specified IServiceCollection. This method will not register services used for views or pages.
            // AddControllers() registra los servicios absolutamente necesarios para que funcionen los controladores, y nada relativo a vistas o páginas Razor.
            // Esto incluye los servicios de autorización, soporte para formateadores, CORS, anotaciones de datos, el application model,
            // mecanismos de selección de acciones, servicios de binding, etc.
            // Este el método de registro ideal para backends que sólo actúan como API.
            services.AddControllers().
                AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles) //evita el ciclo de que un autor tiene muchos libros 
                    .AddNewtonsoftJson(); //este es para el jsonpatch


            //Only use AddEndpointsApiExplorer if you use v6's "minimal APIs", which look like this:
            // app.MapGet("/", () => "Hello World!");
            services.AddEndpointsApiExplorer();
            
            services.AddDbContext<ApplicationDbContext>(
                options =>
                
                    options.UseSqlServer(Configuration.GetConnectionString("testing"))
                );

            services.AddAutoMapper(typeof(Startup));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            //aqui agregamos el jwt para poder validar el token
            // lo q se agrega es lo q va dentro de options
            opciones => opciones.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,  //xq quiero validar la duración del token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                ClockSkew = TimeSpan.Zero //para no tener problemas de tiempo
            });

            services.AddIdentity<IdentityUser, IdentityRole>()  //uso user y role de identity
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            //middlewares
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //The major purpose of  UseDeveloperExceptionPage() is to help the user to inspect
                //exception details during the development phase.
               
            }

            app.UseHttpsRedirection();  //Adds middleware for redirecting HTTP Requests to HTTPS.
            //app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseRouting(); //Routing is responsible for matching incoming HTTP requests and dispatching
                              //those requests to the app's executable endpoints. Endpoints are the app's
                              //units of executable request-handling code.
            //app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


    //UseRouting adds route matching to the middleware pipeline. This middleware looks at the set of endpoints defined in the app, and selects the best match based on the request.
    //UseEndpoints adds endpoint execution to the middleware pipeline. It runs the delegate associated with the selected endpoint.

        }
    }
}
