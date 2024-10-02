import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderSummaryReportComponent } from './order-summary-report.component';
import { OrderService } from 'src/app/services/order.service';
import { RoundOffsets } from '@popperjs/core/lib/modifiers/computeStyles';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { OrderItem } from 'src/app/models/order-item.model';
import { Product } from 'src/app/models/Product.model';

describe('OrderSummaryReportComponent', () => {
  let component: OrderSummaryReportComponent;
  let fixture: ComponentFixture<OrderSummaryReportComponent>;
  let orderService : jasmine.SpyObj<OrderService>;
  let router : Router;

  const mockProduct : Product = {
    productId : 1,
    productDescription : "Test",
    productName : "Test",
    quantity : 1,
    productPrice : 1,
    gstPercentage : 1,
    finalPrice : 1
  }

  beforeEach(() => {
   const orderServiceSpyObj = jasmine.createSpyObj("OrderService",["getOrderSummary"]);
    TestBed.configureTestingModule({
      declarations: [OrderSummaryReportComponent],
      imports : [HttpClientTestingModule,RouterTestingModule],
      providers : [
      {
        provide : OrderService,useValue : orderServiceSpyObj 
      },
      {
        provide : ActivatedRoute, useValue : {params : of({id : 1})}
      }
      ]
    });
    fixture = TestBed.createComponent(OrderSummaryReportComponent);
    component = fixture.componentInstance;
    orderService = TestBed.inject(OrderService) as jasmine.SpyObj<OrderService>;
    router = TestBed.inject(Router);
    //fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });

  //--------------------------------------------------
  it("should load details oninit", ()=>{
//Arrange
const mockData : OrderItem[] = [
  {
    orderId: 2,
    orderNumber: 2,
    userId: 2,
    productId: 1,
    orderDate: new Date(),
    orderQuantity: 1,
    orderAmount: 1,
    product: mockProduct
    
  },
  {
  orderId: 1,
  orderNumber: 1,
  userId: 1,
  productId: 1,
  orderDate: new Date(),
  orderQuantity: 1,
  orderAmount: 1,
  product: mockProduct
}
]
const mockResponse : ApiResponse<OrderItem[]> = {success : true,data : mockData,message : ""};
orderService.getOrderSummary.and.returnValue(of(mockResponse));
component.orderNumber = 1;

//Act
component.ngOnInit();

//Assert
expect(orderService.getOrderSummary).toHaveBeenCalled();
  })

  it("should handle error oninit", ()=>{
    //Arrange
    const mockData : OrderItem[] = [
      {
        orderId: 2,
        orderNumber: 2,
        userId: 2,
        productId: 1,
        orderDate: new Date(),
        orderQuantity: 1,
        orderAmount: 1,
        product: mockProduct
        
      }
    ]
    const mockResponse : ApiResponse<OrderItem[]> = {success : false,data : mockData,message : "Failed to fetch order summary"};
    orderService.getOrderSummary.and.returnValue(of(mockResponse));
    component.orderNumber = 1;
    spyOn(console,"error");
    
    //Act
    component.ngOnInit();
    
    //Assert
    expect(orderService.getOrderSummary).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith("Failed to fetch order summary",mockResponse.message)
      })
  it("should handle HTTP error oninit", ()=>{
    //Arrange
    const mockError = {error : {message : 'Error fetching order summary'}};
    
    orderService.getOrderSummary.and.returnValue(throwError(mockError));
    component.orderNumber = 1;
    spyOn(console,"error");
    
    //Act
    component.ngOnInit();
    
    //Assert
    expect(orderService.getOrderSummary).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith("Error fetching order summary",mockError)
      })
    


});
