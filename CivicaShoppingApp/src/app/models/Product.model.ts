export interface Product{
    productId : number,
    productName : string,
    productDescription : string,
    quantity : number,
    productPrice : number,
    gstPercentage : number,
    finalPrice : number, 
    isAddedToCart?: boolean,
}