using Microsoft.EntityFrameworkCore;

namespace QualyTeam
{
    public class Contexto : DbContext
    {
        public Contexto (DbContextOptions<Contexto> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Documento> Documento { get; set; }
        public DbSet<Processos> Processo { get; set; }
    }
}