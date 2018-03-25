using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace shoppingCart.Models.Data
{
    public class DB : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<SidebarDTO> Sidebar { get; set; }
        public DbSet<CategoriesDTO> Categories { get; set; }
        public DbSet<ProductDTO> Products { get; set; }
    }
}