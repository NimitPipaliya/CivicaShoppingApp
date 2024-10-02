import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LocalstorageService } from './helpers/localstorage.service';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Observable } from 'rxjs';
import { UserOrderReport } from '../models/UserOrderReport.model';
import { OrderItem } from '../models/order-item.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = "http://localhost:5007/Api/Order/";

  constructor(private http: HttpClient) { }
  getUserOrderReport(id : number,page : number,pageSize : number,sort_dir : string) : Observable<ApiResponse<UserOrderReport[]>>{
    return this.http.get<ApiResponse<UserOrderReport[]>>(this.apiUrl + "GetAllOrdersByUserId?userId=" + id + "&page=" + page + "&pageSize=" + pageSize + "&sort_direction=" + sort_dir );
  }

    getOrderSummary(orderNumber: number): Observable<ApiResponse<OrderItem[]>> {
        return this.http.get<ApiResponse<OrderItem[]>>(this.apiUrl + 'GetOrderByOrderNumber/' + orderNumber);
    }
  totalOrderByUser(id : number) : Observable<ApiResponse<number>>{
    return this.http.get<ApiResponse<number>>(this.apiUrl + "TotalOrderByUserId?userId=" + id  );
    }

    placeOrder(userId: number | undefined): Observable<ApiResponse<string>> {
        return this.http.post<ApiResponse<string>>(this.apiUrl + "PlaceOrder/" + userId, { userId });
    }
}
