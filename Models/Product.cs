namespace ApiFerreteria.Models
{
    public class Producto
    {
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = "";
        public string Imagen { get; set; } = "";
    }
}
