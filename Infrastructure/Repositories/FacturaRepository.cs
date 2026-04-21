using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Application.Interfaces;
using SistemaFacturacion.Domain.Entities;
using SistemaFacturacion.Infrastructure.Persistence;

namespace SistemaFacturacion.Infrastructure.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly ApplicationDbContext _context;

        public FacturaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Factura?> ObtenerPorIdAsync(int idFactura)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.IdFactura == idFactura);
        }

        public async Task<Factura?> ObtenerPorNumeroAsync(int numeroFactura)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.NumeroFactura == numeroFactura);
        }

        public async Task<IEnumerable<Factura>> ObtenerPorFechaAsync(DateTime fecha, int pagina, int tamanioPagina)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Where(f => f.Fecha.Date == fecha.Date)
                .OrderByDescending(f => f.Fecha)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();
        }

        public async Task<int> ContarPorFechaAsync(DateTime fecha)
        {
            return await _context.Facturas
                .CountAsync(f => f.Fecha.Date == fecha.Date);
        }

        public async Task AgregarAsync(Factura factura)
        {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
        }
    }
}