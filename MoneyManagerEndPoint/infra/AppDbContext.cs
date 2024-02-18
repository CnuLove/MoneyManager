using Microsoft.EntityFrameworkCore;
using MoneyManagerEndPoint.Model;
using System.Data;

namespace MoneyManagerEndPoint.infra
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MoneyGroupMaster> Tblgrouplegermaster { get; set; }

        public DbSet<MoneySubGroupMaster> TblSubgrouplegermaster { get; set; }

        public DbSet<MoneyLegerMaster> Tbllegertypemaster { get;set; }

        public DbSet<MoneyTransaction> Tbltransactions {  get; set; }
    }
}
