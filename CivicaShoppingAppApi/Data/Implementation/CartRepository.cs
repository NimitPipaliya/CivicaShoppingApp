using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaShoppingAppApi.Data.Implementation
{
    public class CartRepository : ICartRepository
    {
        private readonly IAppDbContext _context;

        public CartRepository(IAppDbContext appDbcontext)
        {
            _context = appDbcontext;
        }
        public IEnumerable<Cart> GetCartItemsByUserId(int userId)
        {
            var cartItems = _context.Carts.Include(c => c.Product).Where(c => c.UserId == userId);

            return cartItems.ToList();
        }

        public Cart GetCartItemByUserIdAndProductId(int userId, int productId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            return cartItem;
        }

        public bool AddToCart(Cart cart)
        {
            var result = false;
            if (cart != null)
            {
                var rowCount = _context.Carts.Where(c => c.UserId == cart.UserId).Count();
                if(rowCount >= 10)
                {
                    return false;
                }
                
                _context.Carts.Add(cart);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool UpdateCart(Cart cart) 
        {
            var result = false;
            if (cart != null)
            {
                _context.Carts.Update(cart);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool RemoveParticularItem(int userId, int productId)
        {
            var result = false;
            if (userId > 0 && productId > 0)
            {

                var item = _context.Carts.FirstOrDefault(e=> e.UserId == userId && e.ProductId == productId);
                if (item != null)
                {
                    _context.Carts.Remove(item);
                    _context.SaveChanges();
                    result = true;

                }
            }
            return result;

        }

        public bool RemoveAllItemsForUser(int userId)
        {
            var result = false;
            if (userId > 0)
            {
                var entriesToDelete = _context.Carts.Where(e => e.UserId == userId);

                if (entriesToDelete != null && entriesToDelete.Any())
                {
                    _context.Carts.RemoveRange(entriesToDelete);
                    _context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
    }
}
