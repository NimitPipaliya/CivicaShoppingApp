import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateProductComponent } from './update-product.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from 'src/app/services/product.service';
import { ProductListComponent } from '../product-list/product-list.component';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/Product.model';

describe('UpdateProductComponent', () => {
  let component: UpdateProductComponent;
  let fixture: ComponentFixture<UpdateProductComponent>;
  let productSpy : jasmine.SpyObj<ProductService>;
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
    let productSpyObj = jasmine.createSpyObj('ProductService',['getProductById','ModifyProduct']);
    TestBed.configureTestingModule({
      declarations: [UpdateProductComponent],
      imports : [HttpClientTestingModule,RouterTestingModule.withRoutes([{path : 'products',component:ProductListComponent}]),FormsModule,ReactiveFormsModule],
      providers : [
        {provide : ProductService,useValue : productSpyObj},
        {
          provide : ActivatedRoute, useValue : {params : of({id : 1})}
        }
      ]
    });
    fixture = TestBed.createComponent(UpdateProductComponent);
    component = fixture.componentInstance;
    productSpy = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;;
    router = TestBed.inject(Router) as any;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize productId from route params and load product details', () => {
    // Arrange
    const mockResponse: ApiResponse<Product> = { success: true, data: mockProduct, message: '' };
    productSpy.getProductById.and.returnValue(of(mockResponse));

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.productId).toBe(1);
    expect(productSpy.getProductById).toHaveBeenCalledWith(1);
    expect(component.productForm).toBeDefined();
    expect(component.productForm.get('productId')).toBeTruthy();
    expect(component.productForm.get('productName')).toBeTruthy();
    expect(component.productForm.get('productDescription')).toBeTruthy();
    expect(component.productForm.get('quantity')).toBeTruthy();
    expect(component.productForm.get('productPrice')).toBeTruthy();
  });

  it('should handle error on fething data', () => {
    // Arrange
    var mockData !: Product ;
    const mockResponse: ApiResponse<Product> = { success: false, data: mockData, message: 'Failed to fetch contact' };
    productSpy.getProductById.and.returnValue(of(mockResponse));
    spyOn(console,"error")

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.productId).toBe(1);
    expect(productSpy.getProductById).toHaveBeenCalledWith(1);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch product',mockResponse.message);
  });
  it('should handle Http error for fetching data', () => {
    // Arrange
    var mockData !: Product ;
    const mockError = { error: { message: 'Failed to add product' } };
    productSpy.getProductById.and.returnValue(throwError(mockError));
    spyOn(console,"error");
    spyOn(window,"alert");

    // Act
    //fixture.detectChanges(); // ngOnInit is called here
    component.ngOnInit();

    // Assert
    expect(component.productId).toBe(1);
    expect(productSpy.getProductById).toHaveBeenCalledWith(1);
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  });

  //-------------------on submit-----------
  it('should update product suessccfully and nevigate to products list',()=>{
    //Arrange
    spyOn(router,"navigate")
    const mockProduct = {
      productId : 1,
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100,
    };

    component.productForm.setValue(mockProduct)
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Product added successfully' };

    productSpy.ModifyProduct.and.returnValue(of(mockResponse));

    //Act
    
    component.onSubmit();

    //Assert
    expect(productSpy.ModifyProduct).toHaveBeenCalledWith(mockProduct);
    expect(router.navigate).toHaveBeenCalledWith(['/products']);


  })

  it('should handle error when add product fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockProduct = {
      productId : 1,
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100
    };

    component.productForm.setValue(mockProduct)
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    
    productSpy.ModifyProduct.and.returnValue(of(mockResponse));

    // Act
    component.onSubmit();

    // Assert
   expect(productSpy.ModifyProduct).toHaveBeenCalledWith(mockProduct);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message);
  })


  it('should handle error when http error on update product', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockProduct = {
      productId: 1,
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100
    };

    component.productForm.setValue(mockProduct)
    //const mockError = { error: { message: 'HTTP error' } };
    const mockError = { error: { message: 'Failed to add product' } };
    productSpy.ModifyProduct.and.returnValue(throwError(mockError));

    // Act
    component.onSubmit();

    // Assert
   expect(productSpy.ModifyProduct).toHaveBeenCalledWith(mockProduct);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  })

  it("should handle formcontrol method",()=>{
    component.formControls;
  })

});
