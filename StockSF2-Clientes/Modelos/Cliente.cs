using System.ComponentModel.DataAnnotations;

namespace StockSF2_Clientes.Modelos
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "El {0} Debe ser de {1} números"),MinLength(8)]
        public string CUIT { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El {0} es obligatorio")]
        public string CondIva {get; set; }
        [Required (ErrorMessage="El {0} es obligatorio")]
        public string DomicilioComercial {get; set; }
        [StringLength(15, ErrorMessage = "El {0} debe ser de hasta {1} numeros")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El {0} es obligatorio")]
        public string CondicionDeVenta { get; set; }
        [Required(ErrorMessage = "El {0} es obligatorio")]
        public decimal IbAlicuota { get; set; }
        //para los decimales hay que declarar la presicion en el applicationdbcontext
        //haspresicion (total de digitos (entero mas decimlaes),cant de decimales)
        //por ej  haspresicion(6,4) significa que 2 enteros y 4 decimales
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "El {0} es obligatorio") ]
        public string TipoDoc { get; set; }
        public string TipoFact { get; set; }

    }
}
