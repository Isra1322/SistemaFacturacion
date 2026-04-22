using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Application.Interfaces;
using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var clientes = await _clienteRepository.ObtenerTodosAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            return Ok(cliente);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(
            [FromQuery] string texto = "",
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanioPagina = 5)
        {
            var clientes = await _clienteRepository.BuscarAsync(texto, pagina, tamanioPagina);
            var totalRegistros = await _clienteRepository.ContarBusquedaAsync(texto);

            return Ok(new
            {
                pagina,
                tamanioPagina,
                totalRegistros,
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanioPagina),
                datos = clientes
            });
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] Cliente cliente)
        {
            try
            {
                await _clienteRepository.AgregarAsync(cliente);
                return Ok(new { mensaje = "Cliente agregado correctamente" });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UQ_Clientes_Correo"))
                {
                    return BadRequest(new { mensaje = "El correo ya está registrado" });
                }

                return BadRequest(new { mensaje = "No se pudo guardar el cliente" });
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "Ocurrió un error inesperado al guardar el cliente" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Cliente cliente)
        {
            var clienteExistente = await _clienteRepository.ObtenerPorIdAsync(id);

            if (clienteExistente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            cliente.IdCliente = id;
            await _clienteRepository.ActualizarAsync(cliente);

            return Ok(new { mensaje = "Cliente actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var clienteExistente = await _clienteRepository.ObtenerPorIdAsync(id);

            if (clienteExistente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            await _clienteRepository.EliminarAsync(id);
            return Ok(new { mensaje = "Cliente eliminado correctamente" });
        }
    }
}