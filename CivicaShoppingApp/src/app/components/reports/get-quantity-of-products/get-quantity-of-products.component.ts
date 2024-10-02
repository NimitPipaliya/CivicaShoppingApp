import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuantityOfProduct } from 'src/app/models/quantity-of-products.model';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-get-quantity-of-products',
  templateUrl: './get-quantity-of-products.component.html',
  styleUrls: ['./get-quantity-of-products.component.css']
})
export class GetQuantityOfProductsComponent implements OnInit {
  loading : boolean = false; 
  quantityOfProducts : QuantityOfProduct[] = [];
  productId!: number;
  isExist : boolean = true;  
  //pagination
  pageNumber: number = 1;
  pageSize: number = 2;
  totalItems: number = 0;
  totalPages: number = 0;
  flag: string = 'asc';

  constructor(private productService: ProductService, private router: Router) {}
  ngOnInit(): void {
    this.loadProductsCount();
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
              console.error('Failed to fetch contacts count', response.message);
            }
            this.loading = false;
          },
          error: (error) => {
            console.error('Error fetching contacts count.', error);
            this.loading = false;
          }
        });
      }

      loadAllProductswithPagination(){
        this.loading = true;
        this.isExist = true;
        this.productService.getQuantityOfProducts(this.pageNumber,this.pageSize,this.flag).subscribe({
          next : (response) =>{
            if (response.success) {
              console.log(response.data);
              this.quantityOfProducts = response.data;
            } else {
              console.error('Failed to fetch contacts count', response.message);
            }
            this.loading = false;
          },
          error: (error) => {
            console.error('Error fetching contacts count.', error);
            this.quantityOfProducts = [];
            this.isExist = false;
            this.loading = false;
          }
        })
      }

      onClickSort(): void{
        this.loading = true;
        //this.flag = 'contactId'; 
        if(this.flag == 'asc'){
          this.flag = 'desc';
        }
        else if(this.flag == 'desc'){
          this.flag = 'asc';
        }
          this.loadProductsCount();
       
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

}
