namespace TritechPruebaLeo.Models
{
    public class movimientoDetalle
    {
        public int? id { get; set; }
        public int? id_movimiento { get; set; }
        public string? noParte_producto { get; set; }
        public int? id_contenedor { get; set; }
        public int? piezasXContenedor { get; set; }
        public int? contenedorXPallet { get; set; }
        public int? TotalPiezas { get; set; }

        public contenedor? contenedor { get; set; }

    }
}
