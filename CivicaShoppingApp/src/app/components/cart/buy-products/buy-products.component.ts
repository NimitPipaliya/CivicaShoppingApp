import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/Product.model';
import { AddToCart } from 'src/app/models/add-to-cart.model';
import { Cart } from 'src/app/models/cart.model';
import { UpdateCart } from 'src/app/models/update-cart.model';
import { AuthService } from 'src/app/services/auth.service';
import { CartService } from 'src/app/services/cart.service';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-buy-products',
  templateUrl: './buy-products.component.html',
  styleUrls: ['./buy-products.component.css']
})
export class BuyProductsComponent {
  loading : boolean = false; 
  products : Product[] = [];
  cartItems: Cart[] | undefined;
  productId!: number;
  isExist : boolean = true; 
  pageNumber: number = 1;
  pageSize: number = 6;
  totalItems: number = 0;
  totalPages: number = 0;
  flag: string = 'asc';
  userId: number | undefined;
  isAuthenticated: boolean = false;
  productCartCount: boolean = false;


  constructor(private productService: ProductService, private cartService: CartService, private authService: AuthService,private router: Router,private cdr: ChangeDetectorRef) {}


  ngOnInit(): void{
    this.authService.getUserId().subscribe((userId: string | null | undefined) => {
      this.userId = Number(userId);
    });
    this.authService.isAuthenticated().subscribe((authState:boolean)=>{
      this.isAuthenticated=authState;
      this.cdr.detectChanges();
     });
    this.loadProductsCount();
    this.cdr.detectChanges();
  }

  
  loadProductsCount() : void{
    this.loading = true;
        this.productService.getTotalProducts().subscribe({
          next:(response) =>{
            if (response.success) {
              console.log(response.data);
              this.totalItems = response.data;
              this.totalPages = Math.ceil(this.totalItems / this.pageSize);
              this.loadAllProductswithPagination();
            } else {
              console.error('Failed to fetch products count', response.message);
            }
            this.loading = false;
          },
          error: (error) => {
            console.error('Error fetching products count.', error);
            this.loading = false;
          }
        });
      }

      loadAllProductswithPagination(){
        this.loading = true;
        this.isExist = true;
        this.productService.getAllProductsWithPagination(this.pageNumber,this.pageSize,this.flag).subscribe({
          next : (response) =>{
            if (response.success) {
              console.log(response.data);
              this.products = response.data;
              this.getCartItemsByUserId();
            } else {
              console.error('Failed to fetch products', response.message);
            }
            this.loading = false;
          },
          error: (error) => {
            console.error('Error fetching products', error);
            this.products = [];
            this.isExist = false;
            this.loading = false;
          }
        })
      }

      changePage(pageNumber: number): void {
        this.pageNumber = pageNumber;
           this.loadAllProductswithPagination();
         
      }
    
      changePageSize(pageSize: number): void { 
        this.pageSize = pageSize;
        this.pageNumber = 1; // Reset to first page
        this.totalPages = Math.ceil(this.totalItems / this.pageSize); // Recalculate total pages
          this.loadAllProductswithPagination();
    
      }

      addToCart(item: Product) 
      {
        this.loading = true;
        const addToCartModel: AddToCart = {
          userId: this.userId!, 
          productId: item.productId,
          productQuantity: 1 
        };
        this.getCartItemsByUserId();

        this.cartService.addToCart(addToCartModel).subscribe({
          next:(response) => {
            console.log(response)
            this.getCartItemsByUserId();
            this.loading = false;
          },
          error: (err) =>{
            alert(err.error.message)
            console.log(err.error)
            this.loading = false;
          }
        
        })
      }

      getCartItemsByUserId() {
        this.cartService.getCartItemsByUserId(this.userId)
        .subscribe({
          next:(response: ApiResponse<Cart[]>) => {
            if(response.success)
              {
                  this.cartItems = response.data;
                  console.log(response.data);
                  this.cartItems.forEach(cartItem => {
                    this.products.forEach(product => {
                      if(cartItem.productId == product.productId) {
                        product.isAddedToCart = true;
                      }
                    })  
                  });   
              }            
          },
          error:(error => {
            console.error('Failed to fetch users', error);
    
          })
        })
      }

      goToFirstPage(): void {
        if (this.pageNumber > 1) {
          this.changePage(1);
        }
      }
    
      goToLastPage(): void {
        if (this.pageNumber < this.totalPages) {
          this.changePage(this.totalPages);
        }
    }


    onClickSort(): void {
        this.loading = true;
        //this.flag = 'contactId'; 
        if (this.flag == 'asc') {
            this.flag = 'desc';
        }
        else if (this.flag == 'desc') {
            this.flag = 'asc';
        }
        this.pageNumber =1;
        this.loadProductsCount();

    }

}
