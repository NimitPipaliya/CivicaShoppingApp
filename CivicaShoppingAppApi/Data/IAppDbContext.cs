using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaShoppingAppApi.Data
{
    public interface IAppDbContext : IDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        DbSet<Product> Products { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

    }
}
