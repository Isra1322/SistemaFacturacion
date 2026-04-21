using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Domain.Entities;

namespace SistemaFacturacion.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFactura { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(e => e.IdCliente);
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.IdProducto);
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("Facturas");
                entity.HasKey(e => e.IdFactura);

                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Facturas)
                    .HasForeignKey(e => e.IdCliente);
            });

            modelBuilder.Entity<DetalleFactura>(entity =>
            {
                entity.ToTable("DetalleFactura");
                entity.HasKey(e => e.IdDetalleFactura);

                entity.HasOne(e => e.Factura)
                    .WithMany(f => f.DetallesFactura)
                    .HasForeignKey(e => e.IdFactura);

                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.DetallesFactura)
                    .HasForeignKey(e => e.IdProducto);
            });
        }
    }
}