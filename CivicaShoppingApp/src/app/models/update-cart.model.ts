export interface UpdateCart{
    cartId: number,
    userId: number | undefined,
    productId: number,
    productQuantity: number
}