using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.Application.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int idProducto);
        Task<IEnumerable<Producto>> BuscarAsync(string textoBusqueda, int pagina, int tamanioPagina);
        Task<int> ContarBusquedaAsync(string textoBusqueda);
        Task AgregarAsync(Producto producto);
        Task ActualizarAsync(Producto producto);
        Task EliminarAsync(int idProducto);
    }
}