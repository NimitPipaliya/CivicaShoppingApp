import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductListComponent } from './product-list.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from 'src/app/services/product.service';
import { Product } from 'src/app/models/Product.model';
import { ChangeDetectorRef } from '@angular/core';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let produtService : jasmine.SpyObj<ProductService>;
  let router : Router;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;

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
    const productSpyObj = jasmine.createSpyObj("ProductService",["getTotalProducts","getAllProductsWithPagination","getSearchedProductCount","getSearchedProduct"])
    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    TestBed.configureTestingModule({
      declarations: [ProductListComponent],
      imports : [HttpClientTestingModule,RouterTestingModule,FormsModule],
      providers : [
        {
          provide : ProductService, useValue : productSpyObj
        },
        { provide: ChangeDetectorRef, useValue: cdrSpy }
      ]
    });
    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    produtService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    router = TestBed.inject(Router);
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  //------------------------loadProductsCount------------------------

  it('should return all product count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockProduct : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getAllProductsWithPagination.and.returnValue(of(mockProduct));

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(2);
    expect(component.totalPages).toBe(Math.ceil(mockProdutList.length / component.pageSize));
  })

  it('should return error product count in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: false, data: 0, message: 'Failed to fetch products count' };
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch products count',mockResponse.message);
  })
  it('should return Http error product count ',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching products count."}};
    produtService.getTotalProducts.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.ngOnInit();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Error fetching products count.',mockError);
  })

  //--------------------------loadAllProductswithPagination---------------------
  it('should return all product with pagination in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));

    //Act
    component.loadAllProductswithPagination();
    //Assert

    expect(component.products).toBe(mockProdutList);
    expect(produtService.getAllProductsWithPagination).toHaveBeenCalled();
  })

  it('should return error product with pagination in init',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: false, data: [], message: 'Failed to fetch products' };

    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.loadAllProductswithPagination();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(component.products).toEqual(mockResponse.data);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch products',mockResponse.message);
    
  })
  it('should return Http error product with pagination',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching products"}};
    produtService.getAllProductsWithPagination.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.loadAllProductswithPagination();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Error fetching products',mockError);
  })
  //--------------------------loadSearchedProductsCount---------------------
  
  it('should return Searched product count',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockProduct : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    produtService.getSearchedProductCount.and.returnValue(of(mockResponse));
    produtService.getSearchedProduct.and.returnValue(of(mockProduct));

    //Act
    component.loadSearchedProductsCount();
    //Assert

    expect(component.totalItems).toBe(2);
    expect(component.totalPages).toBe(Math.ceil(mockProdutList.length / component.pageSize));
  })

  it('should return error Searched product count ',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: false, data: 0, message: 'Failed to fetch product count' };
    produtService.getSearchedProductCount.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.loadSearchedProductsCount();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch product count',mockResponse.message);
  })
  it('should return Http error Searched product count ',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching searched products count."}};
    produtService.getSearchedProductCount.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.loadSearchedProductsCount();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Error fetching searched products count.',mockError);
  })
 
  //--------------------------loadSearchedProductswithPagination---------------------
  it('should return searched product with pagination',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    
    produtService.getSearchedProduct.and.returnValue(of(mockResponse));

    //Act
    component.loadSearchedProductswithPagination();
    //Assert

    expect(component.products).toBe(mockProdutList);
    expect(produtService.getSearchedProduct).toHaveBeenCalled();
  })

  it('should return error searched product with pagination',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: false, data: [], message: 'Failed to fetch searched product' };

    produtService.getSearchedProduct.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.loadSearchedProductswithPagination();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(component.products).toEqual(mockResponse.data);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch searched product',mockResponse.message);
    
  })
  it('should return Http error searched product with pagination',()=>{
    //Arrange
 const mockError = {error :{message : "Failed to fetch searched product"}};
    produtService.getSearchedProduct.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act
    component.loadSearchedProductswithPagination();
    //Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch searched product',mockError);
  })

  //-----------------------------showAscSearch-------------------------
  it('should load produts when searcch is empty',()=>{
    //Arrange
    component.searchCharachter = "";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
  
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
  
  
    //Act
    component.showAscSearch("",1);
  
    //Assert
    expect(produtService.getTotalProducts).toHaveBeenCalled();
  
   }) 
   it('should handle produts searcched',()=>{
    //Arrange
    component.searchCharachter = "a";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getSearchedProductCount.and.returnValue(of(mockProduct))
    produtService.getSearchedProduct.and.returnValue(of(mockResponse));

    //Act
    component.showAscSearch("a",1);
  
    //Assert
    expect(produtService.getSearchedProduct).toHaveBeenCalled();
   }) 
  
  //-----------------------------onCClickSort-------------------------
  it('should sort produts when ascending and searcch is empty',()=>{
    //Arrange
    component.flag = "asc";
    component.searchCharachter = "";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
  
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
   
    spyOn(component,"loadProductsCount")
  
    //Act
    component.onClickSort();
  
    //Assert
    expect(component.loadProductsCount).toHaveBeenCalled();
  
   }) 
   it('should sort produts when descending and searche is empty',()=>{
    //Arrange
    component.flag = "desc";
    component.searchCharachter = "";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    spyOn(component,"loadProductsCount")
  
    //Act
    component.onClickSort();
  
    //Assert
    expect(component.loadProductsCount).toHaveBeenCalled();
  
   }) 
  it('should sort produts when ascending and searcched',()=>{
    //Arrange
    component.flag = "asc";
    component.searchCharachter = "a";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
  
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getSearchedProduct.and.returnValue(of(mockResponse));
   
    spyOn(component,"loadProductsCount")
  
    //Act
    component.onClickSort();
  
    //Assert
    expect(produtService.getSearchedProduct).toHaveBeenCalled();
  
   }) 
   it('should sort produts when descending and searcheed',()=>{
    //Arrange
    component.flag = "desc";
    component.searchCharachter = "a";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getSearchedProduct.and.returnValue(of(mockResponse));
    spyOn(component,"loadProductsCount")
  
    //Act
    component.onClickSort();
  
    //Assert
    expect(produtService.getSearchedProduct).toHaveBeenCalled();
  
   }) 
     
  //-----------------------------changePage-------------------------
  it('should load product on chnagePage with empty searcched',()=>{
    //Arrange    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));

    //Act
    component.changePage(1,"");
  
    //Assert
    expect(produtService.getTotalProducts).not.toHaveBeenCalled();
    expect(produtService.getAllProductsWithPagination).toHaveBeenCalled();
   }) 

   it('should load product on chnagePage with searcched',()=>{
    //Arrange
    component.searchCharachter = "a";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getSearchedProductCount.and.returnValue(of(mockProduct))
    produtService.getSearchedProduct.and.returnValue(of(mockResponse));

    //Act
    component.changePage(1,"a");
  
    //Assert
    expect(produtService.getSearchedProduct).toHaveBeenCalled();
   }) 
  //-----------------------------changePageSize-------------------------
  it('should load product on chnagePage size with empty searcched',()=>{
    //Arrange    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockProduct))
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));

    //Act
    component.changePageSize(1,"");
  
    //Assert
    expect(produtService.getTotalProducts).not.toHaveBeenCalled();
    expect(produtService.getAllProductsWithPagination).toHaveBeenCalled();
   }) 

   it('should load product on chnagePage size with searcched',()=>{
    //Arrange
    component.searchCharachter = "a";
    
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
    
    produtService.getSearchedProductCount.and.returnValue(of(mockProduct))
    produtService.getSearchedProduct.and.returnValue(of(mockResponse));

    //Act
    component.changePageSize(1,"a");
  
    //Assert
    expect(produtService.getSearchedProduct).toHaveBeenCalled();
   }) 
  //-----------------------------clearSearch-------------------------
  it('should clear search annd laodall products',()=>{
    //Arrange
    
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockProduct : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    produtService.getAllProductsWithPagination.and.returnValue(of(mockProduct));
   
  
    //Act
    component.clearSearch();
  
    //Assert
    expect(component.totalItems).toBe(2);
    expect(component.totalPages).toBe(Math.ceil(mockProdutList.length / component.pageSize));
  })
  //---------------------------------goToFirstPage--------------------------------
  it('should go to first page on click',()=>{
    //Arrange
    component.pageNumber = 2;
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
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
   var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
   
   produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
   spyOn(component,"changePage")
   
   //Act
   component.goToLastPage();
   
   //Assert
   expect(component.changePage).toHaveBeenCalledWith(component.totalPages);
  })  
  
})