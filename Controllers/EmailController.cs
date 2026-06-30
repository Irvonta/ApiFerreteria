using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Text;

namespace ApiFerreteria.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        [HttpPost("enviar-pdf")]
        public IActionResult EnviarPdf([FromBody] EmailRequest request)
        {
            try
            {
                // Convertir el PDF base64 a bytes
                byte[] pdfBytes = Convert.FromBase64String(request.PdfBase64);

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("irvingdanielvalenzuelacardenas@gmail.com", "sacgnsrmchjmqmjm");
                    smtp.EnableSsl = true;

                    var mail = new MailMessage("irvingdanielvalenzuelacardenas@gmail.com", request.Destinatario)
                    {
                        Subject = "Pedido generado",
                        Body = "Adjunto el PDF con tu pedido."
                    };

                    mail.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "pedido.pdf"));

                    smtp.Send(mail);
                }

                return Ok("Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al enviar correo: {ex.Message}");
            }
        }
    }

    public class EmailRequest
    {
        public string Destinatario { get; set; }
        public string PdfBase64 { get; set; }
    }
}
