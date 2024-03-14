using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POS.Model
{
    public class DatabaseContext : DbContext
    {
        public string DbPath { get; }

        public DatabaseContext()
        {
            var path = Path.GetDirectoryName(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.FullName);
            //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DbPath = Path.Join(path, "POS.db");
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
        public DbSet<User> Persons { get; set; }
        public virtual DbSet<Product> Products_t { get; set; }
        public virtual DbSet<Order> Orders_t { get; set; }
    }
}
