﻿using StockSF2_Clientes.DTOs;

namespace StockSF2_Clientes.Util
{
    
        public static class QueryableExtensions
        {
            public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
            {
               int i=0;
                return queryable
                    .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.CantidadRegistrosPorPagina)
                    .Take(paginacionDTO.CantidadRegistrosPorPagina);
            }
        }
    
}
