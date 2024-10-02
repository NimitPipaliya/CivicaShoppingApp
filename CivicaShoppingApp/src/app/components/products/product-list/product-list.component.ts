import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/Product.model';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit{
  loading : boolean = false; 
  products : Product[] = [];
  productId!: number;
  searchCharachter : string = "";
  isExist : boolean = true;  
flag: string = 'asc';

  //pagination
  pageNumber: number = 1;
  pageSize: number = 2;
totalItems: number = 0;
totalPages: number = 0;

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

      loadSearchedProductsCount() : void{
        this.loading = true;
            this.productService.getSearchedProductCount(this.searchCharachter).subscribe({
              next:(response) =>{
                if (response.success) {
                  console.log(response.data);
                  this.totalItems = response.data;
                  this.totalPages = Math.ceil(this.totalItems / this.pageSize);
                  this.loadSearchedProductswithPagination();
                } else {
                  console.error('Failed to fetch product count', response.message);
                }
                this.loading = false;
              },
              error: (error) => {
                console.error('Error fetching searched products count.', error);
                this.loading = false;
              }
            });
          }
          
          loadSearchedProductswithPagination(){
            this.loading = true;
            this.isExist = true;
            console.log(this.searchCharachter)
            this.productService.getSearchedProduct(this.searchCharachter,this.pageSize,this.pageNumber,this.flag).subscribe({
              next : (response) =>{
                if (response.success) {
                  console.log(response.data);
                  this.products = response.data;
                } else {
                  console.error('Failed to fetch searched product', response.message);
                }
                this.loading = false;
              },
              error: (error) => {
                console.error('Failed to fetch searched product', error);
                this.products = [];
                this.isExist = false;
                this.loading = false;
              }
            })
          }

      showAscSearch(search: string,pageNumber:number): void {
        this.searchCharachter = search;
        console.log(this.searchCharachter);
        this.pageNumber = pageNumber;
        if(search == '#' || search==''){
          this.loadProductsCount();
        }
        else{
          this.loading = true;
          this.loadSearchedProductsCount();
        }
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

        if(this.searchCharachter == ""){
          this.pageNumber = 1;
          this.loadProductsCount();
        }
        else{
          this.pageNumber = 1;
          this.loadSearchedProductswithPagination();  
        }
      }

      changePage(pageNumber: number,search = this.searchCharachter): void {
        this.pageNumber = pageNumber;
         if(search == undefined || search == ''){
           this.loadAllProductswithPagination();
         }
         else{
        this.loadSearchedProductsCount();
        }
      }
    
      changePageSize(pageSize: number,search = this.searchCharachter): void { 
        this.pageSize = pageSize;
        this.pageNumber = 1; // Reset to first page
        this.totalPages = Math.ceil(this.totalItems / this.pageSize); // Recalculate total pages
        if(search == undefined || search == ''){
          this.loadAllProductswithPagination();
        }
        else{
       this.loadSearchedProductsCount();
        }
       
    
      }

      clearSearch(){
this.searchCharachter=''
        this.ngOnInit();
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
