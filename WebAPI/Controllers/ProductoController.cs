using Microsoft.AspNetCore.Mvc;
using SistemaFacturacion.Application.Interfaces;
using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var productos = await _productoRepository.ObtenerTodosAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            return Ok(producto);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(
            [FromQuery] string texto = "",
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanioPagina = 5)
        {
            var productos = await _productoRepository.BuscarAsync(texto, pagina, tamanioPagina);
            var totalRegistros = await _productoRepository.ContarBusquedaAsync(texto);

            return Ok(new
            {
                pagina,
                tamanioPagina,
                totalRegistros,
                totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanioPagina),
                datos = productos
            });
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] Producto producto)
        {
            await _productoRepository.AgregarAsync(producto);
            return Ok(new { mensaje = "Producto agregado correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Producto producto)
        {
            var productoExistente = await _productoRepository.ObtenerPorIdAsync(id);

            if (productoExistente == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            producto.IdProducto = id;
            await _productoRepository.ActualizarAsync(producto);

            return Ok(new { mensaje = "Producto actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var productoExistente = await _productoRepository.ObtenerPorIdAsync(id);

            if (productoExistente == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            await _productoRepository.EliminarAsync(id);
            return Ok(new { mensaje = "Producto eliminado correctamente" });
        }
    }
}