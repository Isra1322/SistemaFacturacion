using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int idCliente);
        Task<IEnumerable<Cliente>> BuscarAsync(string textoBusqueda, int pagina, int tamanioPagina);
        Task<int> ContarBusquedaAsync(string textoBusqueda);
        Task AgregarAsync(Cliente cliente);
        Task ActualizarAsync(Cliente cliente);
        Task EliminarAsync(int idCliente);
    }
}