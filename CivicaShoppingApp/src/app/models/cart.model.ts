import { Product } from "./Product.model";

export interface Cart{
    cartId: number,
    userId: number,
    productId: number,
    productQuantity: number,
    product: Product,
    isAddedToCart: boolean,

}