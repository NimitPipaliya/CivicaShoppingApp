import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderHistoryComponent } from './order-history.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { OrderService } from 'src/app/services/order.service';
import { of, throwError } from 'rxjs';
import { UserOrderReport } from 'src/app/models/UserOrderReport.model';

describe('OrderHistoryComponent', () => {
  let component: OrderHistoryComponent;
  let fixture: ComponentFixture<OrderHistoryComponent>;
  let orderServiceSpy: jasmine.SpyObj<OrderService>;

  beforeEach(() => {
    let orderServiceSpyObj = jasmine.createSpyObj('OrderService', ['totalOrderByUser', 'getUserOrderReport'])
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([]), FormsModule],
      declarations: [OrderHistoryComponent],
      providers:[
        {
          provide : OrderService,useValue:orderServiceSpyObj
        }
      ]
    });
    fixture = TestBed.createComponent(OrderHistoryComponent);
    orderServiceSpy = TestBed.inject(OrderService) as jasmine.SpyObj<OrderService>;
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load order report count', () => {
    let userId = 1;
    let mockCountResponse: ApiResponse<number> = {
      data: 10,
      success: true,
      message: ''
    }

    let mockResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    }

    orderServiceSpy.totalOrderByUser.and.returnValue(of(mockCountResponse));
    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockResponse));

    component.loadOrderReportCount(userId);

    expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled()
    expect(component.totalItems).toEqual(mockCountResponse.data);
  });

  it('should set console error when response is false', () => {
    let userId = 1;
    spyOn(console, 'error');
    let mockCountResponse: ApiResponse<number> = {
      data: 10,
      success: false,
      message: 'Error fetching data'
    }
    orderServiceSpy.totalOrderByUser.and.returnValue(of(mockCountResponse));

    component.loadOrderReportCount(userId);

    expect(console.error).toHaveBeenCalledWith('Failed to fetch data ', mockCountResponse.message)
    expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled()
    expect(component.loading).toBeFalse();
  });

  it('should set console error when api returns error', () => {
    let userId = 1;
    spyOn(console, 'error');
    const mockError = { error: { message: 'HTTP error' } };
    
    orderServiceSpy.totalOrderByUser.and.returnValue(throwError(mockError));

    component.loadOrderReportCount(userId);

    expect(console.error).toHaveBeenCalledWith('Error fetching data : ', mockError)
    expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled()
    expect(component.loading).toBeFalse();
  });

  it('should load order report successfully', () => {
    let userId = 0;
    let pageNumber = 1;
    let pageSize = 6;
    let flag = 'desc';

    const mockResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    };

    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockResponse));

    component.loadOrderReport();

    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalledWith(userId, pageNumber, pageSize, flag);
    expect(component.allOrders).toEqual(mockResponse.data);
    expect(component.loading).toBeFalse();
  });

  it('should set console error when response is false', () => {
    let userId = 0;
    let pageNumber = 1;
    let pageSize = 6;
    let flag = 'desc';
    spyOn(console, 'error');

    const mockResponse: ApiResponse<UserOrderReport[]> = {
      data: [],
      success: false,
      message: 'Error fetching data'
    };

    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockResponse));

    component.loadOrderReport();

    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalledWith(userId, pageNumber, pageSize, flag);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch data ', mockResponse.message)
    expect(component.loading).toBeFalse();
  });

  it('should set console error api returns error', () => {
    let userId = 0;
    let pageNumber = 1;
    let pageSize = 6;
    let flag = 'desc';
    spyOn(console, 'error');

    const mockError = { error: { message: 'HTTP error' } };

    orderServiceSpy.getUserOrderReport.and.returnValue(throwError(mockError));

    component.loadOrderReport();

    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalledWith(userId, pageNumber, pageSize, flag);
    expect(console.error).toHaveBeenCalledWith('Error fetching data : ', mockError)
    expect(component.loading).toBeFalse();
  });

  //-----------------------------onCClickSort-------------------------
  it('should sort produts when sord order ascending',()=>{
    //Arrange
    component.flag = "asc";
   
    const mockResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    };

    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockResponse));
   
    //Act
    component.onClickSortWithDate();
 
    //Assert
    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalled();
    expect(component.flag).toEqual('desc');
   })

   it('should sort produts when sort order is descending',()=>{
    //Arrange
    component.flag = "desc";
   
    const mockResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    };

    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockResponse));
   
    //Act
    component.onClickSortWithDate();
 
    //Assert
    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalled();
    expect(component.flag).toEqual('asc');
   })

//   //-----------------------------changePage-------------------------
  it('should load product on chnagePage with empty searcched',()=>{
    //Arrange    
    let pageNumber = 1;
    let mockCountResponse: ApiResponse<number> = {
      data: 10,
      success: true,
      message: ''
    }

    let mockApiResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    }

    orderServiceSpy.totalOrderByUser.and.returnValue(of(mockCountResponse));
    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockApiResponse));
 
    //Act
    component.changePage(pageNumber);
 
    //Assert
    expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled();
    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalled();
   })
 

//   //-----------------------------changePageSize-------------------------
  it('should load product on chnagePage size with empty searcched',()=>{
    //Arrange    
    let pageSize = 6;
    let mockCountResponse: ApiResponse<number> = {
      data: 10,
      success: true,
      message: ''
    }

    let mockApiResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    }

    orderServiceSpy.totalOrderByUser.and.returnValue(of(mockCountResponse));
    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockApiResponse));
 
    //Act
    component.changePageSize(pageSize);
 
    //Assert
    expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled();
    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalled();
    expect(component.pageSize).toEqual(pageSize)
    expect(component.pageNumber).toEqual(1)
   })
 

//   //---------------------------------goToFirstPage--------------------------------
  it('should go to first page on click',()=>{
    //Arrange
    component.pageNumber = 2;
    let pageSize = 6;
    let mockCountResponse: ApiResponse<number> = {
      data: 10,
      success: true,
      message: ''
    }

    let mockApiResponse: ApiResponse<UserOrderReport[]> = {
      data: [{
        orderNumber: 1,
        orderDate: new Date,
      }],
      success: true,
      message: ''
    }

    orderServiceSpy.totalOrderByUser.and.returnValue(of(mockCountResponse));
    orderServiceSpy.getUserOrderReport.and.returnValue(of(mockApiResponse));
   
    //Act
    component.goToFirstPage();
   
    //Assert
    expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled();
    expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalled();
 })
//  //---------------------------------goToLastPage--------------------------------
it('should go to last page on click',()=>{
  //Arrange
  component.pageNumber = 1;
  component.totalPages = 4;
  let mockCountResponse: ApiResponse<number> = {
    data: 10,
    success: true,
    message: ''
  }

  let mockApiResponse: ApiResponse<UserOrderReport[]> = {
    data: [{
      orderNumber: 1,
      orderDate: new Date,
    }],
    success: true,
    message: ''
  }

  orderServiceSpy.totalOrderByUser.and.returnValue(of(mockCountResponse));
  orderServiceSpy.getUserOrderReport.and.returnValue(of(mockApiResponse));
 
  //Act
  component.goToLastPage();
 
  //Assert
  expect(orderServiceSpy.totalOrderByUser).toHaveBeenCalled();
  expect(orderServiceSpy.getUserOrderReport).toHaveBeenCalled();
})
});
