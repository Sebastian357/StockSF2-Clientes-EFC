using System.ComponentModel.DataAnnotations;

namespace StockSF2_Clientes.DTOs
{
    public class CredencialUsuario
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
