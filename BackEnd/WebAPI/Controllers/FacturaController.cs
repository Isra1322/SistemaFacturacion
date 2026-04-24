using Microsoft.AspNetCore.Mvc;
using SistemaFacturacion.Application.DTOs;
using SistemaFacturacion.Application.Services;
using SistemaFacturacion.Application.Interfaces;

namespace SistemaFacturacion.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaService _facturaService;
        private readonly IFacturaRepository _facturaRepository;
        private readonly PdfFacturaService _pdfFacturaService;

        public FacturaController(
            FacturaService facturaService,
            IFacturaRepository facturaRepository,
            PdfFacturaService pdfFacturaService)
        {
            _facturaService = facturaService;
            _facturaRepository = facturaRepository;
            _pdfFacturaService = pdfFacturaService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearFactura([FromBody] CrearFacturaDto dto)
        {
            try
            {
                var numeroFactura = await _facturaService.CrearFacturaAsync(dto);

                return Ok(new
                {
                    mensaje = "Factura creada correctamente",
                    numeroFactura = numeroFactura
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{numeroFactura}")]
        public async Task<IActionResult> ObtenerPorNumero(int numeroFactura)
        {
            var factura = await _facturaRepository.ObtenerPorNumeroAsync(numeroFactura);

            if (factura == null)
                return NotFound(new { mensaje = "Factura no encontrada" });

            var respuesta = new FacturaResponseDto
            {
                NumeroFactura = factura.NumeroFactura,
                Fecha = factura.Fecha,
                Cliente = factura.Cliente != null
                    ? $"{factura.Cliente.Nombre} {factura.Cliente.Apellido}"
                    : string.Empty,
                Correo = factura.Cliente != null ? factura.Cliente.Correo : string.Empty,
                Telefono = factura.Cliente != null ? factura.Cliente.Telefono : string.Empty,
                Direccion = factura.Cliente != null ? factura.Cliente.Direccion : string.Empty,
                Subtotal = factura.Subtotal,
                Iva = factura.Iva,
                Total = factura.Total,
                Detalles = factura.DetallesFactura.Select(d => new DetalleFacturaResponseDto
                {
                    Producto = d.Producto != null ? d.Producto.Nombre : string.Empty,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    TotalLinea = d.TotalLinea
                }).ToList()
            };

            return Ok(respuesta);
        }

        [HttpGet("fecha")]
        public async Task<IActionResult> ObtenerPorFecha(
            [FromQuery] DateTime fecha,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanioPagina = 5)
        {
            var facturas = await _facturaRepository.ObtenerPorFechaAsync(fecha, pagina, tamanioPagina);
            var total = await _facturaRepository.ContarPorFechaAsync(fecha);

            var datos = facturas.Select(f => new FacturaListaDto
            {
                NumeroFactura = f.NumeroFactura,
                Fecha = f.Fecha,
                Cliente = f.Cliente != null
                    ? $"{f.Cliente.Nombre} {f.Cliente.Apellido}"
                    : string.Empty,
                Total = f.Total
            }).ToList();

            return Ok(new
            {
                pagina,
                tamanioPagina,
                totalRegistros = total,
                totalPaginas = (int)Math.Ceiling((double)total / tamanioPagina),
                datos
            });
        }

        [HttpGet("pdf/{numeroFactura}")]
        public async Task<IActionResult> GenerarPdf(int numeroFactura)
        {
            var factura = await _facturaRepository.ObtenerPorNumeroAsync(numeroFactura);

            if (factura == null)
            return NotFound(new { mensaje = "Factura no encontrada" });

            var facturaDto = new FacturaResponseDto
            {
                NumeroFactura = factura.NumeroFactura,
                Fecha = factura.Fecha,
                Cliente = factura.Cliente != null
                ? $"{factura.Cliente.Nombre} {factura.Cliente.Apellido}"
                : string.Empty,
                Correo = factura.Cliente?.Correo ?? "",
                Telefono = factura.Cliente?.Telefono ?? "",
                Direccion = factura.Cliente?.Direccion ?? "",
                Subtotal = factura.Subtotal,
                Iva = factura.Iva,
                Total = factura.Total,
                Detalles = factura.DetallesFactura.Select(d => new DetalleFacturaResponseDto
            {
                Producto = d.Producto != null ? d.Producto.Nombre : string.Empty,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                TotalLinea = d.TotalLinea
            }).ToList()
        };

        var pdfBytes = _pdfFacturaService.GenerarPdf(facturaDto);

    // 🔹 Ruta fija donde se guardará el PDF
        var carpeta = @"C:\Users\stali\Desktop\Facturas";

        if (!Directory.Exists(carpeta))
        {
            Directory.CreateDirectory(carpeta);
        }

            var rutaArchivo = Path.Combine(carpeta, $"Factura_{factura.NumeroFactura}.pdf");

        // Guardar archivo en disco
            await System.IO.File.WriteAllBytesAsync(rutaArchivo, pdfBytes);

            return Ok(new
        {
            mensaje = "Factura guardada correctamente",
            ruta = rutaArchivo
        }); 
        }
    }
}