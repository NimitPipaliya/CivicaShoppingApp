import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { Product } from 'src/app/models/Product.model';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit{

  loading : boolean = false;
  product : Product = {
    productId : 0,
    productName : '',
    productDescription : '',
    quantity : 0,
    gstPercentage : 0,
    finalPrice : 0,
    productPrice : 0
  };
  productId !: number;

  constructor(
    private productService : ProductService,
    private router : Router,
    private route : ActivatedRoute
  ){}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.productId = params['id'];
    });
    this.loadProduct();
  }

  loadProduct() : void{
this.loading = true;
    this.productService.getProductById(this.productId).subscribe({
      next : (response) =>{
        if(response.success){
          this.product = response.data;
        }
        else{
          console.error('Failed to fetch product', response.message);
        }
        this.loading = false;
      },
      error : (error) =>{
        console.error('Error fetching product', error);
        this.loading = false;
      }
    })
  }

   //delete the contact
   confirmDelete(id: number): void {
    if (confirm('Are you sure?')) {
      this.productId = id;
      this.deleteProduct();
    }
  }

  deleteProduct(): void {
    this.productService.DeleteProduct(this.productId).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigate(['/products'])
        } else {
          alert(response.message);
        }
      },
      error: (err) => {
        alert(err.error.message);
      },
      complete: () => {
        console.log('Completed');
      },
    });
  }


}
