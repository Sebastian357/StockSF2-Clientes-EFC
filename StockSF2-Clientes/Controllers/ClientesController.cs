using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSF2_Clientes.DTOs;
using StockSF2_Clientes.Modelos;

namespace StockSF2_Clientes.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ClientesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "obtenerClientes")]
        public async Task<ActionResult<List<ClienteDTO>>> GetTodos()
        {
            var resultado = await context.Clientes.ToListAsync();
            return mapper.Map<List<ClienteDTO>>(resultado);
        }

        [HttpGet("cuit/{CUIT}", Name = "obtenerClientexCuit")]
        public async Task<ActionResult<ClienteDTO>> GetXCuit([FromRoute] string CUIT)
        {
            var resultado = await context.Clientes.FirstOrDefaultAsync(x => x.CUIT == CUIT);
            if (resultado == null)
            {
                return NotFound($"No existe un cliente con el CUIT {CUIT}");
            }
            return mapper.Map<ClienteDTO>(resultado);
        }

        [HttpGet("nombre/{nombre}", Name = "obtenerXNombre")]
        public async Task<ActionResult<List<ClienteDTO>>> GetXNombre(string nombre)
        {
            var resultado = await context.Clientes.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            if (resultado == null)
            {
                return NotFound($"No existe un cliente con el nombre {nombre}");
            }
            return mapper.Map<List<ClienteDTO>>(resultado);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ClienteDTO clienteDTO)
        {
            var existeCliente = await context.Clientes.AnyAsync(x => x.CUIT == clienteDTO.CUIT);//el await responde bool

            if (existeCliente)
            {
                return BadRequest($"Existe un cliente regsitrado con el CUIT {clienteDTO.CUIT} ");
            }
            var cliente = mapper.Map<Cliente>(clienteDTO);
            context.Add(cliente);
            await context.SaveChangesAsync();
            //var clientefinal = mapper.Map<ClienteDTO>(cliente);
            return CreatedAtRoute("obtenerClientexCuit", new { CUIT = cliente.CUIT }, clienteDTO); //va al header

        }

        [HttpPatch("{CUIT}")]
        //hay q agregar
        //microsoft.aspnetcore.mvc.newtonsoftjson
        //y despues configuarar en servicio en startp
        public async Task<ActionResult> Patch(string CUIT, [FromBody] JsonPatchDocument<ClienteDTO> patchDocument)
        //https://benfoster.io/blog/aspnet-core-json-patch-partial-api-updates
        /*
        {
            "op": "replace",
            "path": "/firstname",  aqui va el dato que quiero cambiar
            "value": "Benjamin"
	     },*/
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var cliente = await context.Clientes.FirstOrDefaultAsync(x => x.CUIT == CUIT);

            if (cliente == null)
            {
                return NotFound();
            }
            var clienteDTO = mapper.Map<ClienteDTO>(cliente);

            patchDocument.ApplyTo(clienteDTO, ModelState);//si hay error va a parar a ModelState
            //se aplican los cambios que llegan en patchDocument

            var esValido = TryValidateModel(clienteDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(clienteDTO, cliente);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{CUIT}")]
        public async Task <ActionResult> Delete (string CUIT)
        {
            //esto devuelve un tipo Cliente            
            //await anyasync devuevle bool
            var resultado = await context.Clientes.FirstOrDefaultAsync(x => x.CUIT == CUIT);
            
            
            if (resultado==null)
            {
                return NotFound();
            }

            context.Remove(resultado);
            await context.SaveChangesAsync();
            return NoContent();

            // funciona pero el context es sin await y devuelve un field
            // el await devuelve boolean
            /*
            var resultado = context.Clientes.Where(x => x.CUIT == CUIT).First();
            if (resultado == null)
            {
                return NotFound();
            }
            context.Clientes.Remove(resultado); //borro x clave primaria
            await context.SaveChangesAsync();
            return NoContent();
            */
        }
    }
}
