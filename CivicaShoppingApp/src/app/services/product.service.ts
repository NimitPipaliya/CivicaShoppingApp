import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Product } from '../models/Product.model';
import { AddProduct } from '../models/AddProduct.model';
import { UpdateProduct } from '../models/UpdateProduct.model';
import { ProductSales } from '../models/product-sales.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private apiUrl = "http://localhost:5007/api/Product/"
  constructor(private http : HttpClient) { }

  getAllProductsWithPagination(page : number,pageSize : number,sort : string = "asc") : Observable<ApiResponse<Product[]>>{

    return this.http.get<ApiResponse<Product[]>>(this.apiUrl + "GetAllProducts?" + "page=" +page + "&pageSize=" +pageSize +"&sort_dir=" + sort);
   }
   getTotalProducts() : Observable<ApiResponse<number>>{

    return this.http.get<ApiResponse<number>>(this.apiUrl + "TotalProducts");
   }

   
   getProductById(id : number) : Observable<ApiResponse<Product>>{

    return this.http.get<ApiResponse<Product>>(this.apiUrl + "GetProductById/" + id);
   }

   getSearchedProduct(searchChar : string,pageSize : number,pagenumber : number,sort:string = 'asc') : Observable<ApiResponse<Product[]>>{
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}GetAllSearchedProducts?searchString=${searchChar}&page=${pagenumber}&pageSize=${pageSize}&sort_dir=${sort}`)
    
    
   }
   getSearchedProductCount(searchChar : string) : Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(this.apiUrl+'TotalSearchedProducts?search='+searchChar);
  }

  addProduct(addProduct : AddProduct) : Observable<ApiResponse<string>>{
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}Create`,addProduct);
   }

   ModifyProduct(updateProduct : UpdateProduct) : Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}ModifyProduct`,updateProduct);
   }
   DeleteProduct(id : number) : Observable<ApiResponse<string>>{
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}Remove/${id}`);
   }
   
   getQuantityOfProducts(page : number,pageSize : number,sortOrder : string = "asc") : Observable<ApiResponse<Product[]>>{
    return this.http.get<ApiResponse<Product[]>>(this.apiUrl + "GetQuantityOfProducts?" + "page=" +page + "&pageSize=" +pageSize +"&sortOrder=" + sortOrder);
   }

   getProductSalesReport(page : number,pageSize : number,sortOrder: string = "asc") : Observable<ApiResponse<ProductSales[]>>{
    return this.http.get<ApiResponse<ProductSales[]>>(this.apiUrl + "ProductSalesReport?page=" +page + "&pageSize=" +pageSize +"&sortOrder=" + sortOrder);
   }

   getProductsSoldCount() : Observable<ApiResponse<number>>{

    return this.http.get<ApiResponse<number>>(this.apiUrl + "GetProductsSoldCount");
   }

}
