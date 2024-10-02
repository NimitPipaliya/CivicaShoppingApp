import { Product } from "./Product.model";

export interface OrderItem {
    orderId: number;
    orderNumber: number;
    userId: number;
    productId: number;
    orderDate: Date;
    orderQuantity: number;
    orderAmount: number;
    product: Product;
}