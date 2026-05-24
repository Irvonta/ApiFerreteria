using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Globalization; // 👈 necesario para CultureInfo
using ApiFerreteria.Models;

namespace ApiFerreteria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductosController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // URL publicada de tu Google Sheets en formato CSV
            var url = _config["GoogleSheets:CatalogUrl"];

            using var client = new HttpClient();
            var csv = await client.GetStringAsync(url);

            var productos = new List<Producto>();
            var lineas = csv.Split('\n');

            foreach (var linea in lineas.Skip(1)) // saltar encabezados
            {
                var columnas = linea.Split(',');
                if (columnas.Length >= 7)
                {
                    // Evitar filas vacías o sin código/descripcion
                    if (string.IsNullOrWhiteSpace(columnas[0]) || string.IsNullOrWhiteSpace(columnas[1]))
                    {
                        continue; // saltar esta fila
                    }

                    // 🔧 Normalizar precio: quitar símbolos, espacios y convertir a decimal
                    var precioTexto = columnas[2]
                        .Replace("$", "")
                        .Replace("MXN", "")
                        .Replace(",", ".")
                        .Trim();

                    decimal precio = 0;
                    decimal.TryParse(
                        precioTexto,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture,
                        out precio
                    );

                    // Normalizar categoría y construir ruta de imagen
                    var categoria = columnas[4].ToLower().Trim();
                    var codigo = columnas[0].Trim();
                    var imagen = $"/img/{categoria}/{codigo}.png";

                    productos.Add(new Producto {
                        Codigo = codigo,
                        Descripcion = columnas[1].Trim(),
                        Precio = precio,
                        Categoria = categoria,
                        Imagen = imagen
                    });
                }
            }

            return Ok(productos);
        }
    }
}
