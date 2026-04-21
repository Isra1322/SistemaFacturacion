using SistemaFacturacion.Application.DTOs;
using SistemaFacturacion.Application.Interfaces;
using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.Application.Services
{
    public class FacturaService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IFacturaRepository _facturaRepository;

        public FacturaService(
            IClienteRepository clienteRepository,
            IProductoRepository productoRepository,
            IFacturaRepository facturaRepository)
        {
            _clienteRepository = clienteRepository;
            _productoRepository = productoRepository;
            _facturaRepository = facturaRepository;
        }

        public async Task<int> CrearFacturaAsync(CrearFacturaDto dto)
        {
            // 🔹 1. Validar cliente
            var cliente = await _clienteRepository.ObtenerPorIdAsync(dto.IdCliente);

            if (cliente == null)
                throw new Exception("Cliente no existe");

            // 🔹 2. Crear factura
            var factura = new Factura
            {
                IdCliente = dto.IdCliente,
                Fecha = DateTime.Now,
                NumeroFactura = new Random().Next(1000, 9999),
                DetallesFactura = new List<DetalleFactura>()
            };

            decimal totalFactura = 0;

            // 🔹 3. Recorrer detalles
            foreach (var item in dto.Detalles)
            {
                var producto = await _productoRepository.ObtenerPorIdAsync(item.IdProducto);

                if (producto == null)
                    throw new Exception($"Producto {item.IdProducto} no existe");

                if (item.Cantidad <= 0)
                    throw new Exception("Cantidad inválida");

                if (producto.Stock < item.Cantidad)
                    throw new Exception($"Stock insuficiente para {producto.Nombre}");

                // 🔹 4. Calcular
                decimal totalLinea = producto.Precio * item.Cantidad;

                // 🔹 5. Crear detalle
                var detalle = new DetalleFactura
                {
                    IdProducto = producto.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio,
                    TotalLinea = totalLinea
                };

                factura.DetallesFactura.Add(detalle);

                // 🔹 6. Descontar stock
                producto.Stock -= item.Cantidad;
                await _productoRepository.ActualizarAsync(producto);

                totalFactura += totalLinea;
            }

            // 🔹 7. Total
            factura.Total = totalFactura;

            // 🔹 8. Guardar factura
            await _facturaRepository.AgregarAsync(factura);

            return factura.NumeroFactura;
        }
    }
}