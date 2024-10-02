import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyProductsComponent } from './buy-products.component';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { Product } from 'src/app/models/Product.model';
import { CartService } from 'src/app/services/cart.service';
import { AuthService } from 'src/app/services/auth.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ProductService } from 'src/app/services/product.service';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { Cart } from 'src/app/models/cart.model';
import { FormsModule } from '@angular/forms';

describe('BuyProductsComponent', () => {
  let component: BuyProductsComponent;
  let fixture: ComponentFixture<BuyProductsComponent>;
  let produtService : jasmine.SpyObj<ProductService>;  
  let cartService : jasmine.SpyObj<CartService>;
  let authService : jasmine.SpyObj<AuthService>;
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
  const mockCartList : Cart[] = [
    {
      cartId: 1,
      userId: 1,
      productId: 1,
      productQuantity: 1,
      product: mockProdutList[0],
      isAddedToCart: true,

    },
    {
      cartId: 2,
      userId: 2,
      productId: 2,
      productQuantity: 1,
      product: mockProdutList[1],
      isAddedToCart: true,
    },
    
  ]

  beforeEach(() => {
    const productSpyObj = jasmine.createSpyObj("ProductService",["getTotalProducts","getAllProductsWithPagination"])
    const cartSpyObj = jasmine.createSpyObj("CartService",["addToCart","getCartItemsByUserId"])
    const authSpyObj = jasmine.createSpyObj("AuthService",["getUserId","isAuthenticated"]);
    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    TestBed.configureTestingModule({
      declarations: [BuyProductsComponent],
      imports:[HttpClientTestingModule,FormsModule],
      providers:[
        {provide : ProductService, useValue : productSpyObj},
        {provide : CartService, useValue : cartSpyObj},
        {provide : AuthService, useValue : authSpyObj}
      ]
    });
    fixture = TestBed.createComponent(BuyProductsComponent);
    component = fixture.componentInstance;
    produtService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    cartService = TestBed.inject(CartService) as jasmine.SpyObj<CartService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

   
   //------------------------onInit------------------------
   it("load properly oninit", ()=>{
    //Arrange
    const mockAuthResponse : string | null | undefined = "2";
    authService.getUserId.and.returnValue(of(mockAuthResponse));
    authService.isAuthenticated.and.returnValue(of(true));

    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockProduct : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
produtService.getAllProductsWithPagination.and.returnValue(of(mockProduct));
cartService.getCartItemsByUserId.and.returnValue(of(mockCart));

    
    //Act
    component.ngOnInit();

    //Assert

    expect(component.userId).toEqual(2);
   })
   //------------------------loadProductsCount------------------------

   it('should return all product count',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: true, data: 2, message: '' };
    var mockProduct : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
produtService.getAllProductsWithPagination.and.returnValue(of(mockProduct));
cartService.getCartItemsByUserId.and.returnValue(of(mockCart));

    //Act
    component.loadProductsCount();
    //Assert

    expect(component.totalItems).toBe(2);
    expect(component.totalPages).toBe(Math.ceil(mockProdutList.length / component.pageSize));
  })

  it('should return error product count',()=>{
    //Arrange
    var mockResponse : ApiResponse<number> =  { success: false, data: 0, message: 'Failed to fetch products count' };
    produtService.getTotalProducts.and.returnValue(of(mockResponse));
    spyOn(console,"error");

    //Act
    component.loadProductsCount();
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
    component.loadProductsCount();
    //Assert

    expect(component.totalItems).toBe(0);
    expect(console.error).toHaveBeenCalledWith('Error fetching products count.',mockError);
  })

  //--------------------------loadAllProductswithPagination---------------------
  it('should return all product with pagination',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    cartService.getCartItemsByUserId.and.returnValue(of(mockCart));

    //Act
    component.loadAllProductswithPagination();
    //Assert

    expect(component.products).toBe(mockProdutList);
    expect(produtService.getAllProductsWithPagination).toHaveBeenCalled();
  })

  it('should return error product with pagination',()=>{
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
  
  //--------------------changePage---------------------------------
  it('should return all product with pagination on pageChange',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
    
    //Act
    component.changePage(1);
    //Assert
    
    expect(component.products).toBe(mockProdutList);
    expect(produtService.getAllProductsWithPagination).toHaveBeenCalled();
  })
  
  it('should return error product with pagination on pageChange',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: false, data: [], message: 'Failed to fetch products' };
    
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    spyOn(console,"error");
    
    //Act
    component.changePage(1);
    //Assert

    expect(component.totalItems).toBe(0);
    expect(component.products).toEqual(mockResponse.data);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch products',mockResponse.message);
    
  })
  it('should return Http error product with pagination on pageChange',()=>{
    //Arrange
 const mockError = {error :{message : "Error fetching products"}};
    produtService.getAllProductsWithPagination.and.returnValue(throwError(mockError));
    spyOn(console,"error");
    
    //Act
    component.changePage(1);
    //Assert
    expect(console.error).toHaveBeenCalledWith('Error fetching products',mockError);
  })
  
  //--------------------changePageSize---------------------------------
  it('should return all product with pagination on pageChange Size',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
    var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
  
    //Act
    component.changePageSize(1);
    //Assert
  
    expect(component.products).toBe(mockProdutList);
    expect(produtService.getAllProductsWithPagination).toHaveBeenCalled();
  })
  
  it('should return error product with pagination on pageChange Size',()=>{
    //Arrange
    var mockResponse : ApiResponse<Product[]> =  { success: false, data: [], message: 'Failed to fetch products' };
  
    produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
    spyOn(console,"error");
  
    //Act
    component.changePageSize(1);
    //Assert
  
    expect(component.totalItems).toBe(0);
    expect(component.products).toEqual(mockResponse.data);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch products',mockResponse.message);
    
  })
  it('should return Http error product with pagination on pageChange Size',()=>{
    //Arrange
  const mockError = {error :{message : "Error fetching products"}};
    produtService.getAllProductsWithPagination.and.returnValue(throwError(mockError));
    spyOn(console,"error");
  
    //Act
    component.changePageSize(1);
    //Assert
    expect(console.error).toHaveBeenCalledWith('Error fetching products',mockError);
  })

  //-------------------------------addToCart----------------------------

 it('should add product to cart suessfully',()=>{
    //Arrange
    const mockProduct = {
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100,
      productId : 1,
      gstPercentage : 1,
      finalPrice : 1
    };
    
    component.userId =1;
    const addToCart = {
      userId : 1,
      productId :1,
      productQuantity : 1
    }

    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Product added successfully' };
    const mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    cartService.addToCart.and.returnValue(of(mockResponse));
    cartService.getCartItemsByUserId.and.returnValue(of(mockCart));

    //Act
    
    component.addToCart(mockProduct);

    //Assert
    expect(cartService.addToCart).toHaveBeenCalledWith(addToCart);

   // expect(router.navigate).toHaveBeenCalledWith(['/products']);
  })

 it('should handle error when product not added to cart',()=>{
    //Arrange
    const mockProduct = {
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100,
      productId : 1,
      gstPercentage : 1,
      finalPrice : 1
    };
    
    component.userId =1;
    const addToCart = {
      userId : 1,
      productId :1,
      productQuantity : 1
    }

    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Product added successfully' };
    const mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
    const mockError = {error :{message : "Error fetching products"}};
    cartService.addToCart.and.returnValue(throwError(mockError));
    cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
    spyOn(console,"log");

    //Act
    
    component.addToCart(mockProduct);

    //Assert
    expect(cartService.addToCart).toHaveBeenCalledWith(addToCart);
expect(console.log).toHaveBeenCalledWith(mockError.error);
   // expect(router.navigate).toHaveBeenCalledWith(['/products']);
  })

  //---------------------------------getCartItemsByUserId---------------------------------
  
  it('should handle error when getCartItem by userId not fetched',()=>{
    //Arrange
    component.userId =1;
    const addToCart = {
      userId : 1,
      productId :1,
      productQuantity : 1
    }

    const mockError = {error :{message : "Failed to fetch users"}};
    cartService.getCartItemsByUserId.and.returnValue(throwError(mockError));
    spyOn(console,"error");

    //Act    
    component.getCartItemsByUserId();

    //Assert
    expect(cartService.getCartItemsByUserId).toHaveBeenCalledWith(1);
expect(console.error).toHaveBeenCalledWith("Failed to fetch users",mockError);
   // expect(router.navigate).toHaveBeenCalledWith(['/products']);
  }) 
 //---------------------------------goToFirstPage--------------------------------
 it('should go to first page on click',()=>{
  //Arrange
  component.pageNumber = 2;
  var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
  var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
  
  produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
  cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
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
  var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
  
  produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
  cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
  spyOn(component,"changePage")

  //Act
  component.goToLastPage();

  //Assert
  expect(component.changePage).toHaveBeenCalledWith(component.totalPages);
 })  
 //------------------onClickSort--------------------------
 it('should sort produts when ascending',()=>{
  //Arrange
  component.flag = "asc";
  
  var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
  var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
  var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
  
  produtService.getTotalProducts.and.returnValue(of(mockProduct))
  produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
  cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
  spyOn(component,"loadProductsCount")

  //Act
  component.onClickSort();

  //Assert
  expect(component.loadProductsCount).toHaveBeenCalled();

 }) 
 it('should sort produts when descending',()=>{
  //Arrange
  component.flag = "desc";
  
  var mockResponse : ApiResponse<Product[]> =  { success: true, data: mockProdutList, message: '' };
  var mockProduct : ApiResponse<number> =  { success: true, data: 2, message: '' };
  var mockCart : ApiResponse<Cart[]> =  { success: true, data: mockCartList, message: '' };
  
  produtService.getTotalProducts.and.returnValue(of(mockProduct))
  produtService.getAllProductsWithPagination.and.returnValue(of(mockResponse));
  cartService.getCartItemsByUserId.and.returnValue(of(mockCart));
  spyOn(component,"loadProductsCount")

  //Act
  component.onClickSort();

  //Assert
  expect(component.loadProductsCount).toHaveBeenCalled();

 }) 
   
});
