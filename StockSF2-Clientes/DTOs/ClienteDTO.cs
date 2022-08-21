namespace StockSF2_Clientes.DTOs
{
    public class ClienteDTO
    {
        
        public string CUIT { get; set; }
        public string Nombre { get; set; }
        public string CondIva { get; set; }
        
        public string DomicilioComercial { get; set; }

        public string Telefono { get; set; }
        public string CondicionDeVenta { get; set; }
        
        public decimal IbAlicuota { get; set; }
        //para los decimales hay que declarar la presicion en el applicationdbcontext
        //haspresicion (total de digitos (entero mas decimlaes),cant de decimales)
        //por ej  haspresicion(6,4) significa que 2 enteros y 4 decimales
        public string Email { get; set; }
        
        public string TipoDoc { get; set; }
        public string TipoFact { get; set; }
    }
}
