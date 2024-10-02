import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderSummaryComponent } from './order-summary.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { CurrencyPipe } from '@angular/common';
import { OrderService } from 'src/app/services/order.service';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { OrderItem } from 'src/app/models/order-item.model';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/Product.model';

describe('OrderSummaryComponent', () => {
  let component: OrderSummaryComponent;
  let fixture: ComponentFixture<OrderSummaryComponent>;
  let orderServiceSpy: jasmine.SpyObj<OrderService>;

  const mockOrderItems : OrderItem[] =
  [
    {
      orderId: 1,
      orderNumber: 1,
      userId: 1,
      productId: 1,
      orderDate: new Date(),
      orderQuantity: 10,
      orderAmount: 700,
      product:  {
        productId: 1,
        productName: '',
        productDescription: '',
        quantity: 100,
        productPrice: 10,
        gstPercentage: 10,
        finalPrice: 20
      }
    }

  ]

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
    orderServiceSpy = jasmine.createSpyObj('OrderService',['getOrderSummary'])
    
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([]), CurrencyPipe],
      declarations: [OrderSummaryComponent],
      providers: [
        {provide: OrderService, useValue: orderServiceSpy},
        {
          provide : ActivatedRoute, useValue : {params : of({id : 1})}
        }
      ]
    });
    fixture = TestBed.createComponent(OrderSummaryComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

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
    orderServiceSpy.getOrderSummary.and.returnValue(of(mockResponse));
    component.orderNumber = 1;
     
    //Act
    component.ngOnInit();
     
    //Assert
    expect(orderServiceSpy.getOrderSummary).toHaveBeenCalled();
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
        orderServiceSpy.getOrderSummary.and.returnValue(of(mockResponse));
        component.orderNumber = 1;
        spyOn(console,"error");
       
        //Act
        component.ngOnInit();
       
        //Assert
        expect(orderServiceSpy.getOrderSummary).toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith("Failed to fetch order summary",mockResponse.message)
          })
      it("should handle HTTP error oninit", ()=>{
        //Arrange
        const mockError = {error : {message : 'Error fetching order summary'}};
       
        orderServiceSpy.getOrderSummary.and.returnValue(throwError(mockError));
        component.orderNumber = 1;
        spyOn(console,"error");
       
        //Act
        component.ngOnInit();
       
        //Assert
        expect(orderServiceSpy.getOrderSummary).toHaveBeenCalled();
        expect(console.error).toHaveBeenCalledWith("Error fetching order summary",mockError)
          })


});
