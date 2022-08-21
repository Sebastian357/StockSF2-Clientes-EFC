using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockSF2_Clientes.Modelos;

namespace StockSF2_Clientes
{
    //public class ApplicationDbContext:DbContext sin identity efc
    public class ApplicationDbContext : IdentityDbContext   //con identity efc
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        //DbContext sirve para trabajar sobre la base de datos
        // desde la conexion hasta los crud

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder); tiene q estar esto para identity
            //sin esto no va a funcionar, aqui se sobreescrivio (override) el onmodelcreating

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cliente>().Property(x => x.IbAlicuota).HasPrecision(6,4);
        }
        public DbSet<Cliente> Clientes { get; set; }
    }
}
