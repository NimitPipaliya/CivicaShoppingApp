using CivicaShoppingAppApi.Models;

namespace CivicaShoppingAppApi.Data.Contract
{
    public interface ICartRepository
    {
        IEnumerable<Cart> GetCartItemsByUserId(int userId);
        Cart GetCartItemByUserIdAndProductId(int userId, int productId);
        bool AddToCart(Cart cart);
        bool UpdateCart(Cart cart);

        bool RemoveParticularItem(int userId, int productId);
        bool RemoveAllItemsForUser(int userId);
    }
}
