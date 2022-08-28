using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockSF2_Clientes.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockSF2_Clientes.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController:ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IMapper mapper;

        public UsuariosController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager,
            IMapper mapper) 
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        public UserManager<IdentityUser> UserManager { get; }
        public SignInManager<IdentityUser> SignInManager { get; }

        [HttpPost("registrar")]
        [AllowAnonymous] //do not requires auth
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialUsuario credencialUsuario)
        {
            var usuario = new IdentityUser
            {
                Email = credencialUsuario.Email,
                UserName=credencialUsuario.Email
            };
            var resultado= await userManager.CreateAsync(usuario, credencialUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
                      
        }

        [HttpPost("login")] 
        [AllowAnonymous] //do not requires auth
        public async Task <ActionResult<RespuestaAutenticacion>> Login (CredencialUsuario credencialUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialUsuario.Email, credencialUsuario.Password, isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpPost("renovarToken")]
        //solo los autorizados van a poder renovar
        //esto exige que llegue un token valido
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
        public async Task <ActionResult<RespuestaAutenticacion>> RenovarToken ()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            //[0] = {http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress: cura@email.com}
            //esto es un mapeo automatico que hace y para sacarlo en el startup hay que poner
            // en la partye del iconfiguration JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            var email = emailClaim.Value;

            var credencialesUsuario = new CredencialUsuario()
            {
                Email = email,
            };
            return await ConstruirToken(credencialesUsuario);
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialUsuario credencialUsuario)
        {
            // un claim es información del usuario acerca de la cual podemos confiar
            // es info emitida por una fuente en la cual nosotros confiamos
            // es un par de llave y valor
            // estos claim se los voy a añadir al token, de manera tal que cuando el usuario me mande
            // un token, voya poder leer en este caso el email de usuario
            // los claim no son secretos, el cluente de la aplicacion tmb los va a poder leer
            // por lo q  no hay q poner información sensitiva

            var claims = new List<Claim>()
                {
                    new Claim("email",credencialUsuario.Email),
                    new Claim("lo que yo quiera", "cualquier otro valor")
                };
            var usuario = await userManager.FindByEmailAsync(credencialUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario); //tarigo todos los claims para el usuario del email

            claims.AddRange(claimsDB); //aqui junto los claims de var claims + los de claimsdb

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            //clave secreta, como estamos en desarrollo la dejo en appsettingsdevelopment.json
            // tiene q ser lo suficientemente larga xq si no da error en tiempo de ejecución
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);//puedon poner minutos, segs, etc

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);
            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion // a pesar de q va dentro del token a estufa le gusta pasarla de forma explícita
            };
        }

        //hacer abm de claims

        [HttpGet("claims/{usuario}")]
        public async Task <ActionResult<List<ClaimDTO>>> Get (string usuario)
        {
            var resultado = await userManager.FindByEmailAsync (usuario); //en este caso el emai es igual al usuario
            if (resultado == null)
            {
                return NotFound();
            }
            var listaclaims=await userManager.GetClaimsAsync(resultado);
            List<ClaimDTO> claimsxMostrar = new List<ClaimDTO>();
            //claimsxMostrar=  mapper.Map<ListaClaimDTO>(listaclaims);
            
            foreach(Claim x in listaclaims)
            {
                ClaimDTO xclaimDTO = new ClaimDTO();
                xclaimDTO.nombreClaim=x.Type.ToString();xclaimDTO.valorClaim=x.Value;
                claimsxMostrar.Add(xclaimDTO);
            }

            return claimsxMostrar;
        }

        [HttpPost("claims/{usuario}")]
        public async Task <ActionResult> Post (string usuario, [FromBody] ClaimDTO claimDTO)
        {
            var resultado= await userManager.FindByEmailAsync (usuario);
            if (resultado==null)
            {
                return NotFound();
            }
            if (claimDTO==null)
            {
                return BadRequest();
            }
            
            await userManager.AddClaimAsync(resultado, new Claim (claimDTO.nombreClaim, claimDTO.valorClaim));
            return NoContent();
        }

        [HttpDelete("claims/{usuario}/{nombreClaim}")]
        public async Task <ActionResult> Delete (String usuario, String nombreClaim)
        {
            var resultadoUsuario=await userManager.FindByEmailAsync (usuario);
            if (resultadoUsuario==null)
            { 
                return BadRequest("No se ingresó el usuartio");
            }
            if (nombreClaim==null)
            {
                return BadRequest("No se ingresó el nombre del claim");
            }
            var resultadoClaim = await userManager.GetClaimsAsync(resultadoUsuario);
            var existeClaim=resultadoClaim.Where(x=>x.Type==nombreClaim).FirstOrDefault();
            if (existeClaim==null)
            {
                return BadRequest("El claim ingresado no pertenece al usuario");
            }
            await userManager.RemoveClaimAsync(resultadoUsuario, new Claim(nombreClaim, existeClaim.Value));
            return NoContent();
        }
    }
}
