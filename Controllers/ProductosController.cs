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

            // 👇 dividir correctamente por saltos de línea (\r\n o \n)
            var lineas = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // 👇 saltar las tres primeras filas (nota + encabezados + fila fantasma)
            foreach (var linea in lineas.Skip(3))
            {
                var columnas = linea.Split(',');

                // Evitar filas vacías o encabezados
                if (columnas.Length < 7 ||
                    string.IsNullOrWhiteSpace(columnas[0]) ||
                    string.IsNullOrWhiteSpace(columnas[1]) ||
                    columnas[0].Trim().ToLower() == "clave" ||
                    columnas[1].Trim().ToLower() == "descripcion")
                {
                    continue;
                }

                // Validar que el código sea numérico
                if (!int.TryParse(columnas[0].Trim(), out _))
                {
                    continue;
                }

                // Normalizar precio
                var precioTexto = columnas[2]
                    .Replace("$", "")
                    .Replace("MXN", "")
                    .Replace(",", ".")
                    .Trim();

                if (!decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out var precio))
                {
                    continue; // 👈 si no hay precio válido, saltar
                }

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

            return Ok(productos);
        }
    }
}
