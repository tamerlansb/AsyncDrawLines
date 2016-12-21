using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AsyncDrawLines
{
    public class DataContext:DbContext
    {
        public DataContext(string connectionString = "DBConnection") : base(connectionString)
        { }
        public DbSet<Data> DataS { get; set; }
    }
}
