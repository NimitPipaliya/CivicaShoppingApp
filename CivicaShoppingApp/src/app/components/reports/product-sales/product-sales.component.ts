import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/Product.model';
import { ProductSales } from 'src/app/models/product-sales.model';
import { AuthService } from 'src/app/services/auth.service';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-product-sales',
  templateUrl: './product-sales.component.html',
  styleUrls: ['./product-sales.component.css']
})
export class ProductSalesComponent implements OnInit{
  productSales: ProductSales[] | undefined;
  username:string |null|undefined;
  totalProductsSold!: number;
  pageSize = 2;
  currentPage = 1;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  totalPages: number[] = [];
  sortOrder: string = 'asc';

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef,private route : Router, private productService: ProductService) { }


  ngOnInit(){
    this.loadSoldProducts();
    this.totalProductsSoldCount();
    this.authService.isAuthenticated().subscribe((authState:boolean)=>{
      this.isAuthenticated=authState;
      this.cdr.detectChanges();
     });

  }

  totalProductsSoldCount() {
    this.productService.getProductsSoldCount()
    .subscribe({
      next: (response: ApiResponse<number>) => {
        if(response.success)
          {
            this.totalProductsSold = response.data;
            console.log(this.totalProductsSold);
            this.calculateTotalPages();

          }
          else{
            console.error('Failed to fetch contacts', response.message);
          }
      },
      error:(error => {
        console.error('Failed to fetch contacts', error);
        this.loading = false;
      })
    });
  }

  loadSoldProducts() {
    this.loading = true;
    this.productService.getProductSalesReport(this.currentPage, this.pageSize, this.sortOrder)
      .subscribe({
        next:(response: ApiResponse<ProductSales[]>) => {
          if(response.success){
            this.productSales = response.data;
            console.log(response.data);
          }
          else{
            console.error('Failed to fetch data', response.message);
          }
          this.loading = false;

        },
        error:(error => {
          console.error('Failed to fetch data', error);
          this.loading = false;
        })
      });
  }


  calculateTotalPages() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalProductsSold / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.loadSoldProducts();
  }

  onPageSizeChange() {
    this.currentPage = 1; // Reset to first page when page size changes
    this.loadSoldProducts();
    this.totalProductsSoldCount();
  }


  onClickSort(): void{
    this.loading = true;
    //this.flag = 'contactId'; 
    if(this.sortOrder == 'asc'){
      this.sortOrder = 'desc';
    }
    else if(this.sortOrder == 'desc'){
      this.sortOrder = 'asc';
    }
    this.currentPage = 1;
    this.totalProductsSoldCount();
    this.loadSoldProducts();
  }

  




}
