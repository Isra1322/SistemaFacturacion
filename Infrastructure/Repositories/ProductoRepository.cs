using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Application.Interfaces;
using SistemaFacturacion.Domain.Entities;
using SistemaFacturacion.Infrastructure.Persistence;

namespace SistemaFacturacion.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int idProducto)
        {
            return await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);
        }

        public async Task<IEnumerable<Producto>> BuscarAsync(string textoBusqueda, int pagina, int tamanioPagina)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(textoBusqueda));
            }

            return await query
                .OrderBy(p => p.IdProducto)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();
        }

        public async Task<int> ContarBusquedaAsync(string textoBusqueda)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(textoBusqueda));
            }

            return await query.CountAsync();
        }

        public async Task AgregarAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int idProducto)
        {
            var producto = await _context.Productos.FindAsync(idProducto);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }
    }
}