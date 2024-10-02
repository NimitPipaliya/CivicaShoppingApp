import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShoppingCartComponent } from './shopping-cart.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Cart } from 'src/app/models/cart.model';
import { CartService } from 'src/app/services/cart.service';
import { AuthService } from 'src/app/services/auth.service';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { User } from 'src/app/models/user.model';
import { UpdateCart } from 'src/app/models/update-cart.model';
import { OrderService } from 'src/app/services/order.service';

describe('ShoppingCartComponent', () => {
  let component: ShoppingCartComponent;
  let fixture: ComponentFixture<ShoppingCartComponent>;
  let cartServiceSpy: jasmine.SpyObj<CartService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let orderServiceSpy: jasmine.SpyObj<OrderService>;

  const mockCartItems: Cart[] = [
    {
      cartId: 1,
      userId: 1,
      productId: 1,
      productQuantity: 10,
      product: {
        productId: 1,
        productName: '',
        productDescription: '',
        quantity: 100,
        productPrice: 100,
        gstPercentage: 10,
        finalPrice: 110,
      },
      isAddedToCart: false,
    },
  ];

  const mockCart: Cart = {
    cartId: 1,
    userId: 1,
    productId: 1,
    productQuantity: 5,
    product: {
      productId: 1,
      productName: 'Test Product',
      productDescription: 'Test Description',
      quantity: 100,
      productPrice: 100,
      gstPercentage: 10,
      finalPrice: 110,
    },
    isAddedToCart: false,
  };
  beforeEach(() => {
    cartServiceSpy = jasmine.createSpyObj('CartService', [
      'getCartItemsByUserId', 'modifyCart', 'removeParticularProductFromCart'
    ]);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getUserId']);
    orderServiceSpy = jasmine.createSpyObj('OrderService',['placeOrder'])
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [ShoppingCartComponent], 
      providers: [
        { provide: CartService, useValue: cartServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: OrderService, useValue: orderServiceSpy },

      ],
    });
    fixture = TestBed.createComponent(ShoppingCartComponent);
    component = fixture.componentInstance;
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load cart items successfully', () => {
    //Arrange
    const mockResponse: ApiResponse<Cart[]> = {
      success: true,
      data: mockCartItems,
      message: '',
    };
    authServiceSpy.getUserId.and.returnValue(of('1'));
    component.userId = 1;
    cartServiceSpy.getCartItemsByUserId.and.returnValue(of(mockResponse));
    //Act
    component.ngOnInit();

    //Assert
    expect(authServiceSpy.getUserId).toHaveBeenCalled();
    expect(cartServiceSpy.getCartItemsByUserId).toHaveBeenCalledWith(
      component.userId
    );
    expect(component.cartItems).toEqual(mockCartItems);
  });

  it('should handle Http error response', () => {
    // Arrange

    const mockError = { message: 'Network Error' };
    cartServiceSpy.getCartItemsByUserId.and.returnValue(throwError(mockError));
    authServiceSpy.getUserId.and.returnValue(of(undefined));
    spyOn(console, 'error');
    //Act
    component.ngOnInit();
    //Asserts
    expect(authServiceSpy.getUserId).toHaveBeenCalled();
    expect(cartServiceSpy.getCartItemsByUserId).toHaveBeenCalled();
    expect(component.cartItems).toEqual(undefined);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users', mockError  );
  });

  it('should update cart handle success response', () => {
    // Arrange
    const mockCart: UpdateCart =  {
      cartId: 1,
      userId: 1,
      productId: 1,
      productQuantity: 10,
    };
  
    // Mock editContact to return success
    const successResponse = { success: true, data:mockCart,message: 'Contact updated successfully' };
    cartServiceSpy.modifyCart.and.returnValue(of(successResponse));
    spyOn(console,'log')
    spyOn(component,'getCartItemsByUserId')
    
    // Act
    component.updateCart(mockCart);
  
    // Assert
    expect(cartServiceSpy.modifyCart).toHaveBeenCalledWith(mockCart);
    expect(console.log).toHaveBeenCalledWith(successResponse.message)
  });

  it('should update cart and handle false response', () => {
    // Arrange
    const mockCart: UpdateCart =  {
      cartId: 1,
      userId: 1,
      productId: 1,
      productQuantity: 10,
    };
  
    // Mock editContact to return success
    const successResponse = { success: false, data:mockCart,message: 'Contact updated successfully' };
    cartServiceSpy.modifyCart.and.returnValue(of(successResponse));
    spyOn(window,'alert')
    spyOn(component,'getCartItemsByUserId')

        // Act
    component.updateCart(mockCart);
    // Assert
    expect(cartServiceSpy.modifyCart).toHaveBeenCalledWith(mockCart);
    expect(window.alert).toHaveBeenCalledWith(successResponse.message)
  });

  it('should handle error response for update', () => {

    const mockCart: UpdateCart =  {
      cartId: 1,
      userId: 1,
      productId: 1,
      productQuantity: 10,
    };
  
    // Mock editContact to return error
    const errorResponse = {error: { message: 'Failed to update cart' }};
    cartServiceSpy.modifyCart.and.returnValue(throwError(errorResponse));
    spyOn(console,'error');
    spyOn(window,'alert')
  
    // Act
    component.updateCart(mockCart);
  
    // Assert
    expect(cartServiceSpy.modifyCart).toHaveBeenCalledWith(mockCart);
    expect(window.alert).toHaveBeenCalledWith(errorResponse.error.message)
    expect(console.error).toHaveBeenCalledWith(errorResponse.error.message);
  });

  it('should place order successfully', () => {
    // Arrange
    const orderId = '123'; // Assuming this is the order ID returned upon placing an order
    const mockResponse: ApiResponse<string> = {
      success: true,
      data: orderId,
      message: 'Order placed successfully',
    };
    spyOn(console, 'log');
    spyOn(component['route'], 'navigate'); // Mocking the route navigation method
    
    orderServiceSpy.placeOrder.and.returnValue(of(mockResponse));
    
    // Act
    component.placeOrder();
    
    // Assert
    expect(orderServiceSpy.placeOrder).toHaveBeenCalledWith(component.userId);
    expect(console.log).toHaveBeenCalledWith(mockResponse.message);
    expect(component['route'].navigate).toHaveBeenCalledWith(['/order-summary/' + orderId]);
  });

  it('should handle false response when placing order', () => {
    // Arrange
    const orderId = "123"
    const mockErrorResponse: ApiResponse<string> = {
      success: false,
      data: orderId,
      message: 'Order failed to place',
    };
    
    spyOn(window, 'alert');
    
    orderServiceSpy.placeOrder.and.returnValue(of(mockErrorResponse));
    
    // Act
    component.placeOrder();
    
    // Assert
    expect(orderServiceSpy.placeOrder).toHaveBeenCalledWith(component.userId);
    expect(window.alert).toHaveBeenCalledWith(mockErrorResponse.message);
  });

  it('should handle error response when placing order', () => {
    // Arrange
    const errorResponse = { error: { message: 'Internal Server Error' } };
    spyOn(window, 'alert');
    spyOn(console, 'error');
    
    orderServiceSpy.placeOrder.and.returnValue(throwError(errorResponse));
    
    // Act
    component.placeOrder();
    
    // Assert
    expect(orderServiceSpy.placeOrder).toHaveBeenCalledWith(component.userId);
    expect(window.alert).toHaveBeenCalledWith(errorResponse.error.message);
    expect(console.error).toHaveBeenCalledWith(errorResponse.error.message);
  });

  it('should delete cart item when confirmed', () => {
    // Arrange
    const productId = 1;
    spyOn(window, 'confirm').and.returnValue(true); // Mocking confirmation dialog
    const successResponse = { success: true, data:"",message: 'Contact updated successfully' };
    spyOn(console, 'log');
    spyOn(component, 'getCartItemsByUserId');
    spyOn(component['route'], 'navigate'); // Mocking the route navigation method
  
    cartServiceSpy.removeParticularProductFromCart.and.returnValue(of(successResponse));
  
    // Act
    component.deleteCartItem(productId);
  
    // Assert
    expect(window.confirm).toHaveBeenCalledWith("Are you sure you want to delete the product from cart???"); // Ensure confirm dialog was triggered
    expect(cartServiceSpy.removeParticularProductFromCart).toHaveBeenCalledWith(component.userId, productId);
    expect(console.log).toHaveBeenCalledWith(successResponse.message);
    expect(component.getCartItemsByUserId).toHaveBeenCalled(); // Ensure getCartItemsByUserId was called
    expect(component['route'].navigate).not.toHaveBeenCalled(); // Since cartItems.length > 1, route.navigate should not be called
  });

  it('should delete cart item when confirmed and redirect to buyProducts', () => {
    // Arrange
    const productId = 1;
    spyOn(window, 'confirm').and.returnValue(true); // Mocking confirmation dialog
    const successResponse = { success: true, data:"",message: 'Contact updated successfully' };
    spyOn(console, 'log');
    spyOn(component, 'getCartItemsByUserId');
    spyOn(component['route'], 'navigate'); // Mocking the route navigation method
  
    cartServiceSpy.removeParticularProductFromCart.and.returnValue(of(successResponse));
    component.cartItems = mockCartItems
    // Act
    component.deleteCartItem(productId);
  
    // Assert
    expect(window.confirm).toHaveBeenCalledWith("Are you sure you want to delete the product from cart???"); // Ensure confirm dialog was triggered
    expect(cartServiceSpy.removeParticularProductFromCart).toHaveBeenCalledWith(component.userId, productId);
    expect(console.log).toHaveBeenCalledWith(successResponse.message);
    expect(component.getCartItemsByUserId).toHaveBeenCalled(); // Ensure getCartItemsByUserId was called
    expect(component['route'].navigate).toHaveBeenCalledWith(['/buyProducts']); // Since cartItems.length > 1, route.navigate should not be called
  });

  it('should not delete cart item when canceled', () => {
    // Arrange
    const productId = 1;
    spyOn(window, 'confirm').and.returnValue(false); // Mocking confirmation dialog  
  
    // Act
    component.deleteCartItem(productId);
  
    // Assert
    expect(window.confirm).toHaveBeenCalled(); // Ensure confirm dialog was triggered
    expect(cartServiceSpy.removeParticularProductFromCart).not.toHaveBeenCalledWith(component.userId, productId);
  });

  it('should handle failed response when deleting cart Item', () => {
    // Arrange
    const productId = 1;
    component.userId = 1;
   
    
    spyOn(window, 'alert');
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(console,'error');
    
   
    const errorResponse = {error: { message: 'Failed to update cart' }};
    cartServiceSpy.removeParticularProductFromCart.and.returnValue(throwError(errorResponse));
    
    // Act
   component.deleteCartItem(productId);
    
    // Assert
   expect(cartServiceSpy.removeParticularProductFromCart).toHaveBeenCalledWith(component.userId,productId);
    expect(window.alert).toHaveBeenCalledWith(errorResponse.error.message);
    expect(console.error).toHaveBeenCalledOnceWith(errorResponse.error.message);
  });

  it('should increment quantity of item in cart', () => {
    // Arrange
    const updateCart: UpdateCart = {
      cartId: mockCart.cartId,
      userId: undefined,
      productId: mockCart.product.productId,
      productQuantity: mockCart.productQuantity + 1,
    };
    spyOn(component, 'updateCart');

    // Act
    component.incrementQuantity(mockCart);

    // Assert
    expect(component.updateCart).toHaveBeenCalled();
  });

  it('should decrement quantity of item in cart and call updateCart', () => {
    // Arrange
    const updateCart: UpdateCart = {
      cartId: mockCart.cartId,
      userId: undefined,
      productId: mockCart.product.productId,
      productQuantity: mockCart.productQuantity - 1,
    };
    spyOn(component, 'updateCart');

    // Act
    component.decrementQuantity(mockCart);

    // Assert
    expect(component.updateCart).toHaveBeenCalled();
    expect(cartServiceSpy.removeParticularProductFromCart).not.toHaveBeenCalled();
  });

  it('should decrement quantity to zero and call deleteCartItem', () => {
    // Arrange
    const itemWithZeroQuantity: Cart = {
      ...mockCart,
      productQuantity: 1, // Simulate decrement to zero
    };
    spyOn(component, 'deleteCartItem');

    // Act
    component.decrementQuantity(itemWithZeroQuantity);

    // Assert
    expect(component.deleteCartItem).toHaveBeenCalledWith(itemWithZeroQuantity.product.productId);
    expect(cartServiceSpy.modifyCart).not.toHaveBeenCalled();
  });

  it('should not decrement quantity if item quantity is zero', () => {
    // Arrange
    const itemWithZeroQuantity: Cart = {
      ...mockCart,
      productQuantity: 0,
    };
    spyOn(component, 'deleteCartItem');
    spyOn(component, 'updateCart');

    // Act
    component.decrementQuantity(itemWithZeroQuantity);

    // Assert
    expect(component.deleteCartItem).not.toHaveBeenCalled();
    expect(component.updateCart).not.toHaveBeenCalled();
    expect(cartServiceSpy.removeParticularProductFromCart).not.toHaveBeenCalled();
  });


});


