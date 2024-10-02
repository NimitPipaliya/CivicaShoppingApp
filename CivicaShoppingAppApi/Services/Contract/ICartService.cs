using CivicaShoppingAppApi.Dtos;

namespace CivicaShoppingAppApi.Services.Contract
{
    public interface ICartService
    {
        ServiceResponse<IEnumerable<UserCartDto>> GetCartItemsByUserId(int userId);
        ServiceResponse<string> AddToCart(AddToCartDto addToCartDto);
        ServiceResponse<string> UpdateCart(UpdateCartDto updateCartDto);
        ServiceResponse<string> RemoveAllItemsForUser(int userId);
        ServiceResponse<string> RemoveParticularItemFromCart(int userId, int productId);
    }
}
