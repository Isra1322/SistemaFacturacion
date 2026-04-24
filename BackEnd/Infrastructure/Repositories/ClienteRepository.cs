using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Application.Interfaces;
using SistemaFacturacion.Domain.Entities;
using SistemaFacturacion.Infrastructure.Persistence;

namespace SistemaFacturacion.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int idCliente)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == idCliente);
        }

public async Task<IEnumerable<Cliente>> BuscarAsync(string textoBusqueda, int pagina, int tamanioPagina)
{
    var query = _context.Clientes.AsQueryable();

    if (!string.IsNullOrWhiteSpace(textoBusqueda))
    {
        textoBusqueda = textoBusqueda.ToLower();

        query = query.Where(c =>
            c.Nombre.ToLower().Contains(textoBusqueda) ||
            c.Apellido.ToLower().Contains(textoBusqueda) ||
            c.Correo.ToLower().Contains(textoBusqueda));
    }

    return await query
        .OrderBy(c => c.IdCliente)
        .Skip((pagina - 1) * tamanioPagina)
        .Take(tamanioPagina)
        .ToListAsync();
}

public async Task<int> ContarBusquedaAsync(string textoBusqueda)
{
    var query = _context.Clientes.AsQueryable();

    if (!string.IsNullOrWhiteSpace(textoBusqueda))
    {
        textoBusqueda = textoBusqueda.ToLower();

        query = query.Where(c =>
            c.Nombre.ToLower().Contains(textoBusqueda) ||
            c.Apellido.ToLower().Contains(textoBusqueda) ||
            c.Correo.ToLower().Contains(textoBusqueda));
    }

    return await query.CountAsync();
}

        public async Task AgregarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int idCliente)
        {
            var cliente = await _context.Clientes.FindAsync(idCliente);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
        }
    }
}