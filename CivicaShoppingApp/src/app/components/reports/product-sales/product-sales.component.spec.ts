import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductSalesComponent } from './product-sales.component';
import { ProductService } from 'src/app/services/product.service';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { ProductSales } from 'src/app/models/product-sales.model';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('ProductSalesComponent', () => {
  let component: ProductSalesComponent;
  let fixture: ComponentFixture<ProductSalesComponent>;
  let produtService : jasmine.SpyObj<ProductService>;
  let authService : jasmine.SpyObj<AuthService>;
  let router : Router;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;
  const mockProduct : ProductSales[] = [
    {
      productId : 1,
      orderDate : new Date(12/12/2000),
      totalQuantitySold : 11,
      productName : 1
  },
  {
    productId : 2,
    orderDate : new Date(12/12/2000),
    totalQuantitySold : 1,
    productName : 2
}
    
  ]

  beforeEach(() => {
    const productSpyObj = jasmine.createSpyObj("ProductService",["getProductsSoldCount","getProductSalesReport"]);
    const authSpyObj = jasmine.createSpyObj("AuthService",["isAuthenticated"]);
    cdrSpy = jasmine.createSpyObj("ChangeDetectorRef",["detectChanges"]);
    TestBed.configureTestingModule({
      declarations: [ProductSalesComponent],
      imports : [HttpClientTestingModule,RouterTestingModule,FormsModule],
      providers : [
        {
          provide : ProductService, useValue : productSpyObj
        },
        {
          provide : AuthService, useValue : authSpyObj
        },
        
      ]
    });
    fixture = TestBed.createComponent(ProductSalesComponent);
    component = fixture.componentInstance;
    produtService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return all product sold quantity count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };

    
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
    produtService.getProductsSoldCount.and.returnValue(of(mockResponse));
    authService.isAuthenticated.and.returnValue(of(true));

    //Act
    component.ngOnInit();
    //Assert

    expect(mockResponse.data).toBe(2);
   
  })

  it('should return error product sold quantity count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: false, data: 0, message: 'Failed to fetch contacts count' };
    var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
    produtService.getProductsSoldCount.and.returnValue(of(mockResponse));
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
    authService.isAuthenticated.and.returnValue(of(true));
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(mockResponse.data).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch contacts',mockResponse.message);
  })
  it('should return Http error product quantity count ',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching contacts count."}};
 var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
    produtService.getProductsSoldCount.and.returnValue(throwError(mockError));
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
    authService.isAuthenticated.and.returnValue(of(true));
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(console.error).toHaveBeenCalledWith('Failed to fetch contacts',mockError);
  })

  it('should return all product quantity with pagination in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
    
    
    produtService.getProductSalesReport.and.returnValue(of(mockResponse));

    //Act
    component.loadSoldProducts();
    //Assert

    expect(component.productSales).toBe(mockProduct);
    expect(produtService.getProductSalesReport).toHaveBeenCalled();
  })

  it('should return error product quantity with pagination in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<ProductSales[]> =  { success: false, data: [], message: 'Failed to fetch products' };

    produtService.getProductSalesReport.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.loadSoldProducts();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch data',mockResponse.message);
    
  })
  it('should return Http error product quantity with pagination',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching contacts count."}};
    produtService.getProductSalesReport.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.loadSoldProducts();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch data',mockError);
  })

  it('should return error product sold quantity count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: false, data: 0, message: 'Failed to fetch contacts' };
    produtService.getProductsSoldCount.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.totalProductsSoldCount();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch contacts',mockResponse.message);
  })
  it('should return Http error product quantity count ',()=>{
    //Arrange
 const mockError = {error :{message : "Failed to fetch contacts"}};
    produtService.getProductsSoldCount.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.totalProductsSoldCount();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch contacts',mockError);
  })

  it('should sort produts when ascending and searcch is empty',()=>{
    //Arrange
    component.sortOrder = "asc";
   
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
   
    produtService.getProductsSoldCount.and.returnValue(of(mockResponse))
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
   
    spyOn(component,"totalProductsSoldCount")
 
    //Act
    component.onClickSort();
 
    //Assert
    expect(component.totalProductsSoldCount).toHaveBeenCalled();
 
   })
   it('should sort produts when descending and searche is empty',()=>{
    //Arrange
    component.sortOrder = "desc";
   
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
   
    produtService.getProductsSoldCount.and.returnValue(of(mockResponse))
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
   
    spyOn(component,"totalProductsSoldCount")
 
    //Act
    component.onClickSort();
 
    //Assert
    expect(component.totalProductsSoldCount).toHaveBeenCalled();
 
   })

   it('should load product on chnagePage with empty searcched',()=>{
    //Arrange    
   
    var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
   
    
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
 
    //Act
    component.onPageChange(1);
 
    //Assert
    expect(produtService.getProductSalesReport).toHaveBeenCalled();
    
   })

   it('should load product on chnagePage size with empty searcched',()=>{
    //Arrange    
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<ProductSales[]> =  { success: true, data: mockProduct, message: '' };
   
    produtService.getProductsSoldCount.and.returnValue(of(mockResponse))
    produtService.getProductSalesReport.and.returnValue(of(mockResponse1));
   
 
    //Act
    component.onPageSizeChange();
 
    //Assert
    expect(produtService.getProductsSoldCount).toHaveBeenCalled();
    expect(produtService.getProductSalesReport).toHaveBeenCalled();
   })

});
