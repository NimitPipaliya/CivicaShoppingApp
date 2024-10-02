import { TestBed } from '@angular/core/testing';

import { OrderService } from './order.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserOrderReport } from '../models/UserOrderReport.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { OrderItem } from '../models/order-item.model';

describe('OrderService', () => {
  let service: OrderService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers : [OrderService]

    });
    service = TestBed.inject(OrderService);
    httpMock = TestBed.inject(HttpTestingController);

  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch user order report successfully', () => {
    // Arrange
    const userId = 1;
    const page = 1;
    const pageSize = 10;
    const sortDirection = 'asc';

    const mockSuccessResponse: ApiResponse<UserOrderReport[]> = {
      success: true,
      data: [
        {
          orderNumber: 1,
          orderDate: new Date()
        }
      ],
      message: 'Orders fetched successfully'
    };

    // Act
    service.getUserOrderReport(userId, page, pageSize, sortDirection).subscribe(response => {
      // Assert
      expect(response).toEqual(mockSuccessResponse);
      expect(response.data.length).toBe(1); // Assuming one item is returned
      expect(response.data[0].orderNumber).toEqual(1);
      expect(response.message).toBe('Orders fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/GetAllOrdersByUserId?userId=${userId}&page=${page}&pageSize=${pageSize}&sort_direction=${sortDirection}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  });
  it('should not fetch user order report successfully', () => {
    // Arrange
    const userId = 1;
    const page = 1;
    const pageSize = 10;
    const sortDirection = 'asc';

    const mockErrorResponse: ApiResponse<UserOrderReport[]> = {
      success: false,
      data: [
      ],
      message: 'Orders failed fetched successfully'
    };

    // Act
    service.getUserOrderReport(userId, page, pageSize, sortDirection).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.data.length).toBe(0); // Assuming one item is returned
      expect(response.message).toBe('Orders failed fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/GetAllOrdersByUserId?userId=${userId}&page=${page}&pageSize=${pageSize}&sort_direction=${sortDirection}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while fetching user order report', () => {
    // Arrange
    const userId = 1;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.getUserOrderReport(userId, 1, 10, 'asc').subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/GetAllOrdersByUserId?userId=${userId}&page=1&pageSize=10&sort_direction=asc`);
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should fetch order summary successfully', () => {
    // Arrange
    const orderNumber = 1;

    const mockSuccessResponse: ApiResponse<OrderItem[]> = {
      success: true,
      data: [
        {
          orderId: 1,
          orderNumber: 1,
          userId: 1,
          productId: 1,
          orderDate: new Date(),
          orderQuantity: 1,
          orderAmount: 50,
          product: {
            productId: 1,
            productName: 'Sample Product',
            productDescription: 'Sample Description',
            quantity: 10,
            productPrice: 50,
            gstPercentage: 18,
            finalPrice: 59
          }
        }
      ],
      message: 'Order details fetched successfully'
    };

    // Act
    service.getOrderSummary(orderNumber).subscribe(response => {
      // Assert
      expect(response).toEqual(mockSuccessResponse);
      expect(response.data.length).toBe(1); // Assuming one item is returned
      expect(response.data[0].orderNumber).toEqual(orderNumber);
      expect(response.message).toBe('Order details fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/GetOrderByOrderNumber/${orderNumber}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  });

  it('should not fetch order summary successfully', () => {
    // Arrange
    const orderNumber = 1;

    const mockErrorResponse: ApiResponse<OrderItem[]> = {
      success: false,
      data: [
      ],
      message: 'Order details failed fetched successfully'
    };

    // Act
    service.getOrderSummary(orderNumber).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.data.length).toBe(0); // Assuming one item is returned
      expect(response.message).toBe('Order details failed fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/GetOrderByOrderNumber/${orderNumber}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while fetching order summary', () => {
    // Arrange
    const orderNumber = 1;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.getOrderSummary(orderNumber).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/GetOrderByOrderNumber/${orderNumber}`);
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });


  it('should not fetch total orders by user successfully', () => {
    // Arrange
    const userId = 1;

    const mockErrorResponse: ApiResponse<number> = {
      success: false,
      data: 0,
      message: 'Total orders not fetched successfully'
    };

    // Act
    service.totalOrderByUser(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.data).toEqual(0); // Assuming 5 orders for the user
      expect(response.message).toBe('Total orders not fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/TotalOrderByUserId?userId=${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);
  });

  it('should fetch total orders by user successfully', () => {
    // Arrange
    const userId = 1;

    const mockSuccessResponse: ApiResponse<number> = {
      success: true,
      data: 5,
      message: 'Total orders fetched successfully'
    };

    // Act
    service.totalOrderByUser(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockSuccessResponse);
      expect(response.data).toEqual(5); // Assuming 5 orders for the user
      expect(response.message).toBe('Total orders fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/TotalOrderByUserId?userId=${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  });

  it('should handle HTTP error while fetching total orders by user', () => {
    // Arrange
    const userId = 1;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.totalOrderByUser(userId).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/TotalOrderByUserId?userId=${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should place an order successfully', () => {
    // Arrange
    const userId = 1;

    const mockSuccessResponse: ApiResponse<string> = {
      success: true,
      data: '',
      message: 'Order placed successfully'
    };

    // Act
    service.placeOrder(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockSuccessResponse);
      expect(response.message).toBe('Order placed successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/PlaceOrder/${userId}`);
    expect(req.request.method).toBe('POST');
    req.flush(mockSuccessResponse);
  });

  it('should not place an order successfully', () => {
    // Arrange
    const userId = 1;

    const mockErrorResponse: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Order not placed successfully'
    };

    // Act
    service.placeOrder(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.message).toBe('Order not placed successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/PlaceOrder/${userId}`);
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while placing an order', () => {
    // Arrange
    const userId = 1;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.placeOrder(userId).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/Api/Order/PlaceOrder/${userId}`);
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });


});
