import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Cart } from 'src/app/models/cart.model';
import { UpdateCart } from 'src/app/models/update-cart.model';
import { AuthService } from 'src/app/services/auth.service';
import { CartService } from 'src/app/services/cart.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-shopping-cart',
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.css']
})
export class ShoppingCartComponent {
  cartItems: Cart[] | undefined;
  userId: number | undefined;
  totalCheckoutAmount: number = 0;
  loading: boolean = false;

  constructor(private authService: AuthService, private cartService: CartService ,private orderService: OrderService,private cdr: ChangeDetectorRef,private route : Router) { }

  ngOnInit() {
    this.authService.getUserId().subscribe((userId: string | null | undefined) => {
      this.userId = Number(userId);
    });
    this.getCartItemsByUserId();

  }

  getCartItemsByUserId() {
    this.loading = true;
    this.cartService.getCartItemsByUserId(this.userId)
    .subscribe({
      next:(response: ApiResponse<Cart[]>) => {
        if(response.success)
          {
              this.cartItems = response.data;
              console.log(response.data);
              this.calculateTotalCheckoutAmount();
          }
          this.loading= false;
        
      },
      error:(error => {
        console.error('Failed to fetch users', error);
        this.loading= false;

      })
    })
  }

  incrementQuantity(item: Cart): void {
    const updateCart: UpdateCart = {
      cartId: item.cartId,
      userId: this.userId!,
      productId: item.product.productId,
      productQuantity: item.productQuantity + 1
    };

    this.updateCart(updateCart);

  }

  decrementQuantity(item: Cart): void {
    if (item.productQuantity >= 1) {
      const updateCart: UpdateCart = {
        cartId: item.cartId,
        userId: this.userId!,
        productId: item.product.productId,
        productQuantity: item.productQuantity - 1
      };

      if (updateCart.productQuantity === 0) {
        this.deleteCartItem(item.product.productId);
        // this.updateCart(updateCart);
      } else {
        this.updateCart(updateCart);
      }
  
    }
  }
  updateCart(updateCart: UpdateCart): void {
    this.loading = true;
  updateCart.productQuantity 
    this.cartService.modifyCart(updateCart).subscribe({
        next: (response) => { 
          if(response.success){
            console.log(response.message)
            
            this.getCartItemsByUserId();

          }
          else{
            alert(response.message)
          }
          this.loading = false;
        },
        error:(error) => {
            alert(error.error.message);
            console.error(error.error.message);
            this.loading = false;
        }
  });
}

 placeOrder(){
   this.loading = true;
    this.orderService.placeOrder(this.userId).subscribe({
      next: (response) => { 
        if(response.success){
          console.log(response.message)
          let orderNumber = Number(response.data);
          this.route.navigate(['/order-summary/' + orderNumber])
        }
        else{
          alert(response.message)
        }
        this.loading = false;
      },
      error:(error) => {
          alert(error.error.message);
          console.error(error.error.message);
          this.loading = false;
      }
    })
 }

 calculateTotalCheckoutAmount(): void {
  this.totalCheckoutAmount = this.cartItems!.reduce((total, item) => {
    return total + (item.product.finalPrice * item.productQuantity);
  }, 0);
}

deleteCartItem(productId: number): void {
  if(confirm("Are you sure you want to delete the product from cart???"))
    {
  this.cartService.removeParticularProductFromCart(this.userId, productId).subscribe({
    next: (response) => {
      if (response.success) {
        console.log(response.message);
        // Refresh cart items after deletion
        this.getCartItemsByUserId();

        if (this.cartItems && this.cartItems.length === 1) {
          this.route.navigate(['/buyProducts']); // Navigate to buyProducts page
        }
      }
    },
    error: (error) => {
      alert(error.error.message);
      console.error(error.error.message);
    }
  
  });
}
}

}
