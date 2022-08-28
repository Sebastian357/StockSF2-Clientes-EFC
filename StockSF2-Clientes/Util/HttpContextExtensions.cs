using Microsoft.EntityFrameworkCore;

namespace StockSF2_Clientes.Util
{
    public static class HttpContextExtensions 
        
    {
        public async static Task InsertarParametrosPaginacion<T>(
           this HttpContext httpContext, // extend the base class httpcontext for adding http headers at responses
           IQueryable<T> queryable, //me sirve para poder determinar la cant total de reg en la tabla
           int cantidadRegistrosPorPagina)
        {
            double cantidad = await queryable.CountAsync();//cuento los registros
            double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);
            httpContext.Response.Headers.Add("cantidadPaginas", cantidadPaginas.ToString());
        }
    }
}
