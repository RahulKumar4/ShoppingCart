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
    }
}