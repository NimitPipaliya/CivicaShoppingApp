import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductDetailsComponent } from './product-details.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ProductService } from 'src/app/services/product.service';
import { Product } from 'src/app/models/Product.model';
import { useAnimation } from '@angular/animations';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';

describe('ProductDetailsComponent', () => {
  let component: ProductDetailsComponent;
  let fixture: ComponentFixture<ProductDetailsComponent>;
  let productService : jasmine.SpyObj<ProductService>;
  let router: Router;

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
    const productServicespy = jasmine.createSpyObj('ProductService',['getProductById','DeleteProduct']);
    TestBed.configureTestingModule({
      declarations: [ProductDetailsComponent],
      imports : [HttpClientTestingModule,RouterTestingModule],
      providers : [
        {
          provide : ProductService, useValue : productServicespy
        },
        {
          provide : ActivatedRoute, useValue : {params : of({id : 1})}
        }
      ]
    });
    fixture = TestBed.createComponent(ProductDetailsComponent);
    component = fixture.componentInstance;
    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    router = TestBed.inject(Router);
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize productId from route params and load product details', () => {
    // Arrange
    const mockResponse: ApiResponse<Product> = { success: true, data: mockProduct, message: '' };
    productService.getProductById.and.returnValue(of(mockResponse));

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(component.product.productId).toBe(1);
    expect(productService.getProductById).toHaveBeenCalledWith(1);
    expect(component.product).toEqual(mockProduct);
  });

  it('should fail to load product details', () => {
    // Arrange
    const mockResponse: ApiResponse<Product> = { success: false, data: mockProduct, message: 'Failed to fetch product' };
    productService.getProductById.and.returnValue(of(mockResponse));
    spyOn(console, 'error')
    
    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch product',mockResponse.message)
    expect(productService.getProductById).toHaveBeenCalledWith(1);
  });

  it('should handle http error', () => {
    // Arrange
    const mockError = { message: 'Error fetching product' };
    productService.getProductById.and.returnValue(throwError(() => mockError));
    spyOn(console, 'error')

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(console.error).toHaveBeenCalledWith("Error fetching product",mockError)
    expect(productService.getProductById).toHaveBeenCalledWith(1);
  });

  it('should delete product and navigate to produccts list', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: true, data: '', message: 'Product deleted successfully' };
    const productId = 1;
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(router, 'navigate').and.stub();
    productService.DeleteProduct.and.returnValue(of(mockDeleteResponse));

    // Act
    component.confirmDelete(productId);

    // Assert
    //expect(window.confirm).toHaveBeenCalledWith("Are you sure?")
    expect(productService.DeleteProduct).toHaveBeenCalledWith(productId);
    expect(router.navigate).toHaveBeenCalledWith(['/products']);
  });


  it('should not delete product if user cancels', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: false, data: '', message: 'Failed to delete product' };
    const productId = 1;
    spyOn(window, 'confirm').and.returnValue(false);
    spyOn(router, 'navigate').and.stub();
    productService.DeleteProduct.and.returnValue(of(mockDeleteResponse));

    // Act
    component.confirmDelete(productId);

    // Assert
    expect(window.confirm).toHaveBeenCalledWith("Are you sure?")
    expect(productService.DeleteProduct).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });
  //--------------------delete----------------
  it('should not delete product if error', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: false, data: '', message: 'Failed to delete product' };
    const productId = 1;
    spyOn(window, 'confirm').and.returnValue(false);
    spyOn(router, 'navigate').and.stub();
    spyOn(window,"alert");
    
    productService.DeleteProduct.and.returnValue(of(mockDeleteResponse));

    // Act
    component.deleteProduct();

    // Assert
   expect(window.alert).toHaveBeenCalledWith(mockDeleteResponse.message);
    expect(router.navigate).not.toHaveBeenCalled();
  });
  it('should not delete product if Http error', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: false, data: '', message: 'Failed to delete product' };
    const mockError = {error : {message : "Failed to delete product"}};
    
  
    spyOn(router, 'navigate').and.stub();
    spyOn(window,"alert");
    
    productService.DeleteProduct.and.returnValue(throwError(mockError));

    // Act
    component.deleteProduct();

    // Assert
   expect(window.alert).toHaveBeenCalledWith(mockDeleteResponse.message);
    expect(router.navigate).not.toHaveBeenCalled();
  });


});
