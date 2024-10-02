import { ComponentFixture, TestBed, tick } from '@angular/core/testing';

import { AddProductComponent } from './add-product.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ProductService } from 'src/app/services/product.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { ProductListComponent } from '../product-list/product-list.component';

describe('AddProductComponent', () => {
  let component: AddProductComponent;
  let fixture: ComponentFixture<AddProductComponent>;
  let productSpy : jasmine.SpyObj<ProductService>;
  let router : Router;

  beforeEach(() => {
    const productServieSpy = jasmine.createSpyObj('ProductService',["addProduct"]);
    
    TestBed.configureTestingModule({
      declarations: [AddProductComponent],
      imports : [HttpClientTestingModule,FormsModule,ReactiveFormsModule,RouterTestingModule.withRoutes([{path:'products',component : ProductListComponent}])],
      providers : [
        {
          provide : ProductService,useValue : productServieSpy
        },
        // {
        //    provide: Router, useClass: class { navigate = jasmine.createSpy('navigate'); } 
        // }
      ]
    });
    fixture = TestBed.createComponent(AddProductComponent);
    component = fixture.componentInstance;
    productSpy = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    router = TestBed.inject(Router);
 
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize productForm with default values', () => {
    expect(component.productForm).toBeDefined();
    expect(component.productForm.get('productName')).toBeTruthy();
    expect(component.productForm.get('productDescription')).toBeTruthy();
    expect(component.productForm.get('quantity')).toBeTruthy();
    expect(component.productForm.get('productPrice')).toBeTruthy();
  });

  it('should add produt suessfully and nevigate to products list',()=>{
    //Arrange
    const mockProduct = {
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100
    };

    component.productForm.setValue(mockProduct)
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Product added successfully' };

    productSpy.addProduct.and.returnValue(of(mockResponse));

    //Act
    
    component.onSubmit();

    //Assert
    expect(productSpy.addProduct).toHaveBeenCalledWith(mockProduct);
   // expect(router.navigate).toHaveBeenCalledWith(['/products']);


  })

  it('should handle error when add product fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockProduct = {
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100
    };

    component.productForm.setValue(mockProduct)
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    
    productSpy.addProduct.and.returnValue(of(mockResponse));

    // Act
    component.onSubmit();

    // Assert
   expect(productSpy.addProduct).toHaveBeenCalledWith(mockProduct);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message);
  })


  it('should handle error when http errpr', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockProduct = {
      productName: 'Test Prodcut',
      productDescription: 'Test Description',
      quantity: 10,
      productPrice: 100
    };

    component.productForm.setValue(mockProduct)
    //const mockError = { error: { message: 'HTTP error' } };
    const mockError = { error: { message: 'Failed to add product' } };
    productSpy.addProduct.and.returnValue(throwError(mockError));

    // Act
    component.onSubmit();

    // Assert
   expect(productSpy.addProduct).toHaveBeenCalledWith(mockProduct);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  })
});
