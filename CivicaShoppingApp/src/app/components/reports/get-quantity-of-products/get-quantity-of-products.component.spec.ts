import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetQuantityOfProductsComponent } from './get-quantity-of-products.component';
import { ProductService } from 'src/app/services/product.service';
import { Router } from '@angular/router';
import { QuantityOfProduct } from 'src/app/models/quantity-of-products.model';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/Product.model';

describe('GetQuantityOfProductsComponent', () => {
  let component: GetQuantityOfProductsComponent;
  let fixture: ComponentFixture<GetQuantityOfProductsComponent>;
  let produtService : jasmine.SpyObj<ProductService>;
  let router : Router;
  const mockProdutQuantityList : QuantityOfProduct[] = [
    {
      productId : 1,
      productName : "test", 
      quantity : 1,
     
    },
    {
      productId : 2,
      productName : "test 1",
      quantity : 1,
    }
  ];
  const mockProdutList : Product[] = [
    {
      productId : 1,
      productName : "test",
      productDescription : "test",
      productPrice : 1,
      quantity : 1,
      gstPercentage : 1,
      finalPrice : 1
    },
    {
      productId : 2,
      productName : "test 1",
      productDescription : "test 1",
      productPrice : 1,
      quantity : 1,
      gstPercentage : 1,
      finalPrice : 1
    }
  ]

  beforeEach(() => {
    const productSpyObj = jasmine.createSpyObj("ProductService",["getTotalProducts","getQuantityOfProducts"])
    TestBed.configureTestingModule({
      declarations: [GetQuantityOfProductsComponent],
      imports : [HttpClientTestingModule,RouterTestingModule],
      providers : [
        {
          provide : ProductService, useValue : productSpyObj
        }
      ]
    });
    fixture = TestBed.createComponent(GetQuantityOfProductsComponent);
    component = fixture.componentInstance;
    produtService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    router = TestBed.inject(Router);
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return all product quantity count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };

    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(2);
    expect(component.totalPages).toBe(Math.ceil(mockProdutQuantityList.length / component.pageSize));
  })

  it('should return error product quantity count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: false, data: 0, message: 'Failed to fetch contacts count' };
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch contacts count',mockResponse.message);
  })
  it('should return Http error product quantity count ',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching contacts count."}};
    produtService.getTotalProducts.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Error fetching contacts count.',mockError);
  })

  it('should return all product quantity with pagination in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse));

    //Act
    component.loadAllProductswithPagination();
    //Assert

    expect(component.quantityOfProducts).toBe(mockProdutList);
    expect(produtService.getQuantityOfProducts).toHaveBeenCalled();
  })

  it('should return error product quantity with pagination in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: false, data: [], message: 'Failed to fetch products' };

    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.loadAllProductswithPagination();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(component.quantityOfProducts).toEqual(mockResponse.data);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch contacts count',mockResponse.message);
    
  })
  it('should return Http error product quantity with pagination',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching contacts count."}};
    produtService.getQuantityOfProducts.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.loadAllProductswithPagination();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Error fetching contacts count.',mockError);
  })

  it('should go to first page on click',()=>{
    //Arrange
    component.pageNumber = 2;
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };

    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));
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
   var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };

    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));
   spyOn(component,"changePage")
   
   //Act
   component.goToLastPage();
   
   //Assert
   expect(component.changePage).toHaveBeenCalledWith(component.totalPages);
  });
  
  it('should sort produts when ascending and searcch is empty',()=>{
    //Arrange
    component.flag = "asc";
   
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };

    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));
   
   
 
    //Act
    component.onClickSort();
 
    //Assert
    expect(produtService.getTotalProducts).toHaveBeenCalled();
    expect(produtService.getQuantityOfProducts).toHaveBeenCalled();
 
   })
   it('should sort produts when descending and searche is empty',()=>{
    //Arrange
    component.flag = "desc";
   
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };

    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));
    //Act
    component.onClickSort();
 
    //Assert
    expect(produtService.getTotalProducts).toHaveBeenCalled();
    expect(produtService.getQuantityOfProducts).toHaveBeenCalled();
   })

   it('should load product on chnagePage with empty searcched',()=>{
    //Arrange    
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));
    
    //Act
    component.changePage(1);
 
    //Assert
    expect(produtService.getQuantityOfProducts).toHaveBeenCalled();
   })

   it('should load product on chnagePage size with empty searcched',()=>{
    //Arrange    
    var mockResponse1 : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    produtService.getQuantityOfProducts.and.returnValue(of(mockResponse1));
 
    //Act
    component.changePageSize(1);
 
    //Assert
    expect(produtService.getQuantityOfProducts).toHaveBeenCalled();
   })
});
