namespace SistemaFacturacion.Application.DTOs
{
    public class FacturaResponseDto
    {
        public int NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<DetalleFacturaResponseDto> Detalles { get; set; } = new();
    }
}