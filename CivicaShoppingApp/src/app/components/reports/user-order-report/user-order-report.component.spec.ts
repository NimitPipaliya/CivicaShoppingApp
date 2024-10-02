import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserOrderReportComponent } from './user-order-report.component';
import { OrderService } from 'src/app/services/order.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UserOrderReport } from 'src/app/models/UserOrderReport.model';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { User } from 'src/app/models/user.model';
import { FormsModule } from '@angular/forms';

describe('UserOrderReportComponent', () => {
  let component: UserOrderReportComponent;
  let fixture: ComponentFixture<UserOrderReportComponent>;
  let orderService : jasmine.SpyObj<OrderService>;
  let authService : jasmine.SpyObj<AuthService>;
  let router : Router;
  const userOrders : UserOrderReport[] =[
    {
      orderDate : new Date(),
      orderNumber : 1
    },
  {
    orderDate : new Date(),
      orderNumber : 2
  }]
  
  beforeEach(() => {
    const orderSpyObj = jasmine.createSpyObj("OrderService",["totalOrderByUser","getUserOrderReport"]);
    const authSpyObj = jasmine.createSpyObj("AuthService",["getUserById"]);
    TestBed.configureTestingModule({
      declarations: [UserOrderReportComponent],
      imports : [HttpClientTestingModule,RouterTestingModule,FormsModule],
      providers : [
        {
          provide : OrderService, useValue : orderSpyObj
        },
        {
          provide : AuthService, useValue : authSpyObj
        },
        {
          provide : ActivatedRoute, useValue : {params : of({id : 1})}
        }
      ]
    });
    fixture = TestBed.createComponent(UserOrderReportComponent);
    component = fixture.componentInstance;
    orderService = TestBed.inject(OrderService) as jasmine.SpyObj<OrderService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load order report count and user details on init', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
    const mockUsers : User ={
      name: 'abc',
    loginId: 'string',
    phone: '12122223344',
    email: 'string@gmail.com',
    userId: 0,
    salutation: "string",
    birthDate: new Date(),
    age: 18,
    gender: "M",

    }
    const mockUser : ApiResponse<User> = {success : true, data : mockUsers, message : ''};
    orderService.totalOrderByUser.and.returnValue(of(mockResponse))
    authService.getUserById.and.returnValue(of(mockUser));

    // Act
    fixture.detectChanges(); // ngOnInit is called here
    

    // Assert
    expect(component.totalItems).toEqual(2);
    expect(component.username).toEqual('string');
    expect(orderService.totalOrderByUser).toHaveBeenCalledWith(1);
    expect(authService.getUserById).toHaveBeenCalledWith(1);
  });

  it('should returns error when order count not loaded', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: false, data: 0, message: '' };
    const mockUsers : User ={
      name: 'abc',
    loginId: 'string',
    phone: '12122223344',
    email: 'string@gmail.com',
    userId: 0,
    salutation: "string",
    birthDate: new Date(),
    age: 18,
    gender: "M",

    }
    const mockUser : ApiResponse<User> = {success : true, data : mockUsers, message : ''};
    orderService.totalOrderByUser.and.returnValue(of(mockResponse))

    // Act
    fixture.detectChanges(); // ngOnInit is called here
    

    // Assert
    expect(component.totalItems).toEqual(0);
    expect(orderService.totalOrderByUser).toHaveBeenCalledWith(1);
  });

  it('should return Http error product quantity count ',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching data : "}};
    orderService.totalOrderByUser.and.returnValue(throwError(mockError));
    authService.getUserById.and.returnValue(throwError(mockError))
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Error fetching data : ',mockError);
  });

  it('should load order report count and user details on init', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
    const mockUsers : User ={
      name: 'abc',
    loginId: 'string',
    phone: '12122223344',
    email: 'string@gmail.com',
    userId: 0,
    salutation: "string",
    birthDate: new Date(),
    age: 18,
    gender: "M",

    }
    const mockUser : ApiResponse<User> = {success : false, data : mockUsers, message : ''};
    orderService.totalOrderByUser.and.returnValue(of(mockResponse))
    authService.getUserById.and.returnValue(of(mockUser));

    // Act
    fixture.detectChanges(); // ngOnInit is called here
    

    // Assert
    expect(component.totalItems).toEqual(2);
    expect(orderService.totalOrderByUser).toHaveBeenCalledWith(1);
    expect(authService.getUserById).toHaveBeenCalledWith(1);
  });


  it('should load order report on sort click', () => {
    // Arrange
     component.userId = 1;
    const mockResponse: ApiResponse<UserOrderReport[]> = { success: true, data: userOrders, message: '' };
    orderService.getUserOrderReport.and.returnValue(of(mockResponse)); // Mock successful response

    component.flag='asc';
    // Act
    component.onClickSortWithDate();
    fixture.detectChanges();

    // Assert
    expect(orderService.getUserOrderReport).toHaveBeenCalledWith(1, component.pageNumber, component.pageSize, component.flag);
    expect(component.allOrders).toEqual(userOrders);
  });

  it('should not load order report on sort click and give error', () => {
    // Arrange
     component.userId = 1;
    const mockResponse: ApiResponse<UserOrderReport[]> = { success: false, data: userOrders, message: '' };
    orderService.getUserOrderReport.and.returnValue(of(mockResponse)); // Mock successful response

    // Act
    component.onClickSortWithDate();
    fixture.detectChanges();

    // Assert
    expect(orderService.getUserOrderReport).toHaveBeenCalledWith(1, component.pageNumber, component.pageSize, component.flag);
    
  });

  it('should return http error when failed to get user order report ',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching data : "}};
    orderService.getUserOrderReport.and.returnValue(throwError(mockError));
    
    spyOn(console,"error");

    //Act
    component.onClickSortWithDate();
    fixture.detectChanges();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Error fetching data : ',mockError);
    
  });

  it('should handle pagination changes', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
    const mockUsers : User ={
      name: 'abc',
    loginId: 'string',
    phone: '12122223344',
    email: 'string@gmail.com',
    userId: 0,
    salutation: "string",
    birthDate: new Date(),
    age: 18,
    gender: "M",

    }
    const mockUser : ApiResponse<User> = {success : true, data : mockUsers, message : ''};
    orderService.totalOrderByUser.and.returnValue(of(mockResponse))
    authService.getUserById.and.returnValue(of(mockUser));

    // Act
    fixture.detectChanges(); // ngOnInit is called here
    

    // Assert
    expect(component.pageSize).toEqual(6);
    expect(component.pageNumber).toEqual(1); 
    expect(component.totalPages).toEqual(1); 
    expect(orderService.totalOrderByUser).toHaveBeenCalledWith(1);
    expect(orderService.getUserOrderReport).toHaveBeenCalledWith(1, 1, 6, 'desc');
  });

  it('should go to first page on click',()=>{
    //Arrange
    component.pageNumber = 2;
    const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
    const mockUsers : User ={
      name: 'abc',
    loginId: 'string',
    phone: '12122223344',
    email: 'string@gmail.com',
    userId: 0,
    salutation: "string",
    birthDate: new Date(),
    age: 18,
    gender: "M",

    }
    const mockUser : ApiResponse<User> = {success : true, data : mockUsers, message : ''};
   
    orderService.totalOrderByUser.and.returnValue(of(mockResponse))
    authService.getUserById.and.returnValue(of(mockUser));
    spyOn(component,"changePage")
   
    //Act
    component.goToFirstPage();
   
  //Assert
  expect(component.changePage).toHaveBeenCalledWith(1);
 })
 //---------------------------------goToLastPage--------------------------------
 it('should go to last page on click',()=>{
   //Arrange
   component.pageNumber = 2;
   component.totalPages = 3;
   const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
   const mockUsers : User ={
     name: 'abc',
   loginId: 'string',
   phone: '12122223344',
   email: 'string@gmail.com',
   userId: 0,
   salutation: "string",
   birthDate: new Date(),
   age: 18,
   gender: "M",

   }
   const mockUser : ApiResponse<User> = {success : true, data : mockUsers, message : ''};
  
   orderService.totalOrderByUser.and.returnValue(of(mockResponse))
   authService.getUserById.and.returnValue(of(mockUser));
   spyOn(component,"changePage")
   
   //Act
   component.goToLastPage();
   
   //Assert
   expect(component.changePage).toHaveBeenCalledWith(component.totalPages);
  })  
 //---------------------------------cchnagePage----------------------------
  it('should load orders on chnagePage with empty searcched',()=>{
    //Arrange    

    spyOn(component,"loadOrderReportCount")
    //Act
    component.changePage(1);
 
    //Assert
    expect(component.loadOrderReportCount).toHaveBeenCalled();
   
   })
 
   
  //-----------------------------changePageSize-------------------------
  it('should load  on chnagePage size with empty searcched',()=>{
    //Arrange    
   
   
    const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
    component.userId = 1;
   
    orderService.totalOrderByUser.and.returnValue(of(mockResponse))
    spyOn(component,"loadOrderReportCount")
    //Act
    component.changePageSize(component.userId);
 
    //Assert
    expect(component.loadOrderReportCount).toHaveBeenCalled();
   
   })
 
  

});
