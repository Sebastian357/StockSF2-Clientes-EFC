using AutoMapper;
using StockSF2_Clientes.DTOs;
using StockSF2_Clientes.Modelos;
using System.Collections.Generic;
using System.Security.Claims;

namespace StockSF2_Clientes.Util
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cliente, ClienteDTO>().ReverseMap();
            
        }
    }
}
