namespace SistemaFacturacion.Application.DTOs
{
    public class DetalleFacturaResponseDto
    {
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalLinea { get; set; }
    }
}