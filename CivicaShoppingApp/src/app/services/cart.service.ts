import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Observable } from 'rxjs';
import { AddToCart } from '../models/add-to-cart.model';
import { UpdateCart } from '../models/update-cart.model';
import { Cart } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = "http://localhost:5007/api/Cart/"
  constructor(private http: HttpClient) 
  { 
    
  }

  addToCart(addToCart: AddToCart): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}AddToCart`, addToCart);
  }

  modifyCart(updateCart: UpdateCart): Observable<ApiResponse<UpdateCart>> {
    return this.http.put<ApiResponse<UpdateCart>>(`${this.apiUrl}UpdateCart`, updateCart);
  }

      
  getCartItemsByUserId(userId: number | undefined) : Observable<ApiResponse<Cart[]>>{
    return this.http.get<ApiResponse<Cart[]>>(this.apiUrl + "GetCartItemsByUserId?userId=" + userId);
   }

   removeParticularProductFromCart(userId: number | undefined, productId: number) : Observable<ApiResponse<string>>{
    return this.http.delete<ApiResponse<string>>(this.apiUrl +"RemoveParticularProductFromCart?userId=" +userId+"&productId=" +productId);
   }
}
