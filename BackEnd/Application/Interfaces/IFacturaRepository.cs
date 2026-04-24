using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.Application.Interfaces
{
    public interface IFacturaRepository
    {
        Task<Factura?> ObtenerPorIdAsync(int idFactura);
        Task<Factura?> ObtenerPorNumeroAsync(int numeroFactura);
        Task<IEnumerable<Factura>> ObtenerPorFechaAsync(DateTime fecha, int pagina, int tamanioPagina);
        Task<int> ContarPorFechaAsync(DateTime fecha);
        Task AgregarAsync(Factura factura);
    }
}