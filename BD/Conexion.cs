using Microsoft.EntityFrameworkCore;

namespace RRHH.BD
{
    public class Conexion :DbContext
    {
        public Conexion(DbContextOptions<Conexion> con) : base(con)
        {
        
        }
    }
}
