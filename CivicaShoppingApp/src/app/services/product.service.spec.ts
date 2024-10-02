import { TestBed } from '@angular/core/testing';

import { ProductService } from './product.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Product } from '../models/Product.model';
import { AddProduct } from '../models/AddProduct.model';
import { ProductListComponent } from '../components/products/product-list/product-list.component';
import { ProductSales } from '../models/product-sales.model';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock :HttpTestingController;
  const mockApiResponse: ApiResponse<Product[]> = {
    success: true,
    message: '',
    data: [
      {
        productId: 1,
        productName: 'product 1',
        productDescription: 'description 1',
        quantity :1,
        productPrice : 1,
        finalPrice : 1,
        gstPercentage : 1
      },
      {
        productId: 2,
        productName: 'product 1',
        productDescription: 'description 1',
        quantity :1,
        productPrice : 1,
        finalPrice : 1,
        gstPercentage : 1
      },
    ],
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers : [ProductService]
    });
    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  afterEach(() => {
    httpMock.verify();
  });
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  //---------------------------------getAllProductsWithPagination-------------------
  it('should fetch all Products successfully', () => {
    //Arrange
    const apiUrl = 'http://localhost:5007/api/Product/GetAllProducts?page=1&pageSize=2&sort_dir=asc';

    //Act
    service.getAllProductsWithPagination(1,2,"asc").subscribe((response) => {
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });
  it('should handle an empty product list', () => {
    //Arrange
    const apiUrl = 'http://localhost:5007/api/Product/GetAllProducts?page=1&pageSize=2&sort_dir=asc';
    const emptyResponse: ApiResponse<Product[]> = {
      success: true,
      message: '',
      data: [],
    };
    //Act
    service.getAllProductsWithPagination(1,2,"asc").subscribe((response) => {
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle Http error gracefully', () => {
    //Arrange
    const apiUrl = 'http://localhost:5007/api/Product/GetAllProducts?page=1&pageSize=2&sort_dir=asc';

    const errorMessage = 'Failed to load products';

    //Act
    service.getAllProductsWithPagination(1,2,"asc").subscribe(
      () => fail('expected an error, not categories'),
      (error) => {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    //Respond With error
    req.flush(errorMessage, {
      status: 500,
      statusText: 'Internal Server Error',
    });
  });

  //---------------------------------getTotalProducts-------------------
  it('should fetch total product count',()=>{
    //Arrange
    const mockSuccessResponse: ApiResponse<number> = {
      success: true,
      data: 2,
      message:""
    };
  
    //Act
    service.getTotalProducts().subscribe(response => {
      //Assert
      expect(response).toBe(mockSuccessResponse);
      expect(response.message).toBe('');
      expect(response.success).toBeTrue();
    });
  
    const req = httpMock.expectOne("http://localhost:5007/api/Product/TotalProducts");
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  
  });
  
  it('should handle a failed product count retival',()=>{
    //Arrange
    const mockErrorResponse: ApiResponse<number> = {
      success: false,
      data: 0,
      message:""
    };
  
    //Act
    service.getTotalProducts().subscribe(response => {
      //Assert
      expect(response).toBe(mockErrorResponse);
      expect(response.success).toBeFalse();
      expect(response.message).toEqual('');
    });
  
    const req = httpMock.expectOne("http://localhost:5007/api/Product/TotalProducts");

    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);
  });
  
  it('should handle Http errors for total',()=>{
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };
    //Act
    service.getTotalProducts().subscribe({
      next:() => fail('should have faild with the 500 error'),
      error: (error) =>{
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });
  
    const req = httpMock.expectOne("http://localhost:5007/api/Product/TotalProducts");

    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  
  });
  
  //---------------------------------getProductById-------------------
  it('should fetch product by id successfully', () => {
    //Arrange
    const productId = 1;

    const mockSuccessResposne: ApiResponse<Product> = {
      success: true,
      data: {
        productId:1,
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
      gstPercentage : 1,
      finalPrice : 1
      },
      message: '',
    };
    //Act
    service.getProductById(productId).subscribe((response) => {
      expect(response).toBe(mockSuccessResposne);
      expect(response.data).toEqual(mockSuccessResposne.data);
      expect(response.data.productId).toEqual(productId);

    });

    const req = httpMock.expectOne("http://localhost:5007/api/Product/GetProductById/" + productId);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResposne);
  });


  it('should handle failed product retrival ', () => {
    //Arrange
    const productId = 1;

    const mockErrorResposne: ApiResponse<Product> = {
      success: false,
      data: {} as Product,
      message: 'No record found!',
    };

    //Act
    service.getProductById(productId).subscribe((response) => {
      expect(response).toBe(mockErrorResposne);
      expect(response.data).toEqual(mockErrorResposne.data);

    });

    const req = httpMock.expectOne("http://localhost:5007/api/Product/GetProductById/" + productId);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResposne);
  });

  it('should handle httpError ', () => {
    //Arrange
    const productId = 1;

    const mockHttpError = {
      status: 500,
      statusText: 'Internal server error.',
    };
    //Act
    service.getProductById(productId).subscribe({
      next: () => fail('should have failed with 500 error'),
      error: (error) => {
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toBe('Internal server error.');
      },
    });

    const req = httpMock.expectOne("http://localhost:5007/api/Product/GetProductById/" + productId);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });


  //---------------------------------getSearchedProduct-------------------
  it('should fetch searched Products successfully', () => {
    //Arrange
    const apiUrl = 'http://localhost:5007/api/Product/GetAllSearchedProducts?searchString=a&page=1&pageSize=2&sort_dir=asc';

    //Act
    service.getSearchedProduct('a',2,1,"asc").subscribe((response) => {
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });
  it('should handle an empty searched product list', () => {
    //Arrange
    const apiUrl = 'http://localhost:5007/api/Product/GetAllSearchedProducts?searchString=a&page=1&pageSize=2&sort_dir=asc';
    const emptyResponse: ApiResponse<Product[]> = {
      success: true,
      message: '',
      data: [],
    };
    //Act
    service.getSearchedProduct("a",2,1,"asc").subscribe((response) => {
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle Http error gracefully for searched product', () => {
    //Arrange
    const apiUrl = 'http://localhost:5007/api/Product/GetAllSearchedProducts?searchString=a&page=1&pageSize=2&sort_dir=asc';

    const errorMessage = 'Failed to load products';

    //Act
    service.getSearchedProduct("a",2,1,"asc").subscribe(
      () => fail('expected an error, not categories'),
      (error) => {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    //Respond With error
    req.flush(errorMessage, {
      status: 500,
      statusText: 'Internal Server Error',
    });
  });
  //---------------------------------getSearchedProductCount-------------------
  it('should fetch total searched products count',()=>{
    //Arrange
    const mockSuccessResponse: ApiResponse<number> = {
      success: true,
      data: 2,
      message:""
    };
    const searchChar = 'a';
  
    //Act
    service.getSearchedProductCount(searchChar).subscribe(response => {
      //Assert
      expect(response).toBe(mockSuccessResponse);
      expect(response.message).toBe('');
      expect(response.success).toBeTrue();
    });
  
    const req = httpMock.expectOne("http://localhost:5007/api/Product/TotalSearchedProducts?search=a");
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  
  });
  
  it('should handle a failed searched product count retival',()=>{
    //Arrange
    const mockErrorResponse: ApiResponse<number> = {
      success: false,
      data: 0,
      message:""
    };
    const searchChar = 'a';
  
    //Act
    service.getSearchedProductCount(searchChar).subscribe(response => {
      //Assert
      expect(response).toBe(mockErrorResponse);
      expect(response.success).toBeFalse();
      expect(response.message).toEqual('');
    });
  
    const req = httpMock.expectOne("http://localhost:5007/api/Product/TotalSearchedProducts?search=a");
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);
  });
  
  it('should handle Http errors for searche product count',()=>{
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };
    const searchChar = 'a';
    //Act
    service.getSearchedProductCount(searchChar).subscribe({
      next:() => fail('should have faild with the 500 error'),
      error: (error) =>{
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });
  
    const req = httpMock.expectOne("http://localhost:5007/api/Product/TotalSearchedProducts?search=a");
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  
  });
  
  //---------------------------------addProduct-------------------
  it('should add a product successfully', () => {
    //Arrange
    const addProduct: AddProduct = {
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
    };

    const mockSuccessResposne: ApiResponse<string> = {
      success: true,
      data: '',
      message: 'Product saved successfully.',
    };

    const apiUrl = 'http://localhost:5007/api/Product/Create';

    //Act
    service.addProduct(addProduct).subscribe((response) => {
      expect(response).toBe(mockSuccessResposne);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');

    req.flush(mockSuccessResposne);
  });

  it('should handle failed product addition', () => {
    //Arrange
    const addProduct: AddProduct = {
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
    };
    const mockErrorResposne: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Product already exists.',
    };

    const apiUrl = 'http://localhost:5007/api/Product/Create';


    //Act
    service.addProduct(addProduct).subscribe((response) => {
      expect(response).toBe(mockErrorResposne);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');

    req.flush(mockErrorResposne);
  });

  it('should handle http Error', () => {
    //Arrange
    const addProduct: AddProduct = {
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
    };
    const mockHttpError = {
      success: false,
      status: 500,
      statusText: 'Internal server error.',
    };

    const apiUrl = 'http://localhost:5007/api/Product/Create';


    //Act
    service.addProduct(addProduct).subscribe({
      next: () => fail('should have fail with the error'),
      error: (error) => {
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toBe('Internal server error.');
      },
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');

    req.flush({},mockHttpError);
  });

  //---------------------------------ModifyProduct-------------------
  it('should update a product successfully', () => {
    //Arrange
    const updateProduct: Product = {
      productId:1,
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
      gstPercentage : 1,
      finalPrice : 1
    };

    const mockSuccessResposne: ApiResponse<string> = {
      success: true,
      data: '',
      message: 'Product updated successfully.',
    };

    const apiUrl = 'http://localhost:5007/api/Product/ModifyProduct';

    //Act
    service.ModifyProduct(updateProduct).subscribe((response) => {
      expect(response).toBe(mockSuccessResposne);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('PUT');

    req.flush(mockSuccessResposne);
  });


  it('should handle failed product updatation', () => {
    //Arrange
    const updateProduct: Product = {
      productId:1,
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
      gstPercentage : 1,
      finalPrice : 1
    };
    const mockErrorResposne: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Something went wrong, please try after sometime.',
    };

    const apiUrl = 'http://localhost:5007/api/Product/ModifyProduct';

    //Act
    service.ModifyProduct(updateProduct).subscribe((response) => {
      expect(response).toBe(mockErrorResposne);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('PUT');

    req.flush(mockErrorResposne);
  });

  it('should handle http Error when updating produccct', () => {
    //Arrange
    const updateProduct: Product = {
      productId:1,
      productDescription : "test",
      productName: "test",
      productPrice : 1,
      quantity : 1,
      gstPercentage : 1,
      finalPrice : 1
    };
    const mockHttpError = {
      success: false,
      status: 500,
      statusText: 'Internal server error.',
    };  

    const apiUrl = 'http://localhost:5007/api/Product/ModifyProduct';

    //Act
    service.ModifyProduct(updateProduct).subscribe({
      next: () => fail('should have fail with the error'),
      error: (error) => {
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toBe('Internal server error.');
      },
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('PUT');

    req.flush({},mockHttpError);
  });

  //---------------------------------DeleteProduct-------------------
  it('should delete product successfully', () => {
    //Arrange
    const productId = 1;

    const mockSuccessResposne: ApiResponse<string> = {
      success: true,
      data: '',
      message: 'Product deleted successfully.',
    };
    //Act
    service.DeleteProduct(productId).subscribe((response) => {
      expect(response).toBe(mockSuccessResposne);
    });

    const req = httpMock.expectOne("http://localhost:5007/api/Product/Remove/" + productId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockSuccessResposne);
  });

  it('should handle error delete producct failed', () => {
    //Arrange
    const productId = 1;

    const mockErrorResposne: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Something went wrong, please try after sometimes.',
    };
    //Act
    service.DeleteProduct(productId).subscribe((response) => {
      expect(response).toBe(mockErrorResposne);
    });

    const req = httpMock.expectOne("http://localhost:5007/api/Product/Remove/" + productId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockErrorResposne);
  });

  it('should handle httpError while deleting producct ', () => {
    //Arrange
    const productId = 1;

    const mockHttpError = {
      status: 500,
      statusText: 'Internal server error.',
    };
    //Act
    service.DeleteProduct(productId).subscribe({
      next: () => fail('should have failed with 500 error'),
      error: (error) => {
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toBe('Internal server error.');
      },
    });

    const req = httpMock.expectOne("http://localhost:5007/api/Product/Remove/" + productId);
    expect(req.request.method).toBe('DELETE');
    req.flush({},mockHttpError);
  });
//---------------------------------getQuantityOfProducts-------------------
it('should fetch all Products successfully', () => {
  //Arrange
  const apiUrl = 'http://localhost:5007/api/Product/GetQuantityOfProducts?page=1&pageSize=2&sortOrder=asc';

  //Act
  service.getQuantityOfProducts(1,2,"asc").subscribe((response) => {
    expect(response.data.length).toBe(2);
    expect(response.data).toEqual(mockApiResponse.data);
  });

  const req = httpMock.expectOne(apiUrl);
  expect(req.request.method).toBe('GET');
  req.flush(mockApiResponse);
});
it('should handle an empty product list', () => {
  //Arrange
  const apiUrl = 'http://localhost:5007/api/Product/GetQuantityOfProducts?page=1&pageSize=2&sortOrder=asc';
  const emptyResponse: ApiResponse<Product[]> = {
    success: true,
    message: '',
    data: [],
  };
  //Act
  service.getQuantityOfProducts(1,2,"asc").subscribe((response) => {
    expect(response.data.length).toBe(0);
    expect(response.data).toEqual([]);
  });

  const req = httpMock.expectOne(apiUrl);
  expect(req.request.method).toBe('GET');
  req.flush(emptyResponse);
});

it('should handle Http error gracefully', () => {
  //Arrange
  const apiUrl = 'http://localhost:5007/api/Product/GetQuantityOfProducts?page=1&pageSize=2&sortOrder=asc';

  const errorMessage = 'Failed to load products';

  //Act
  service.getQuantityOfProducts(1,2,"asc").subscribe(
    () => fail('expected an error, not products'),
    (error) => {
      expect(error.status).toBe(500);
      expect(error.statusText).toBe('Internal Server Error');
    }
  );

  const req = httpMock.expectOne(apiUrl);
  expect(req.request.method).toBe('GET');
  //Respond With error
  req.flush(errorMessage, {
    status: 500,
    statusText: 'Internal Server Error',
  });
});
//---------------------------------getProductSalesReport-------------------
it('should fetch all Products successfully', () => {
  //Arrange
  const apiUrl = 'http://localhost:5007/api/Product/ProductSalesReport?page=1&pageSize=2&sortOrder=asc';

  const salesData : ProductSales[] = [
    {
      productId : 1,
      productName : 2,
      totalQuantitySold : 4,
      orderDate : new Date()
    }
  ];
const mockApiResponse1 : ApiResponse<ProductSales[]> = {
  success : true,
  data : salesData,
  message : ""
}
  //Act
  service.getProductSalesReport(1,2,"asc").subscribe((response) => {
    expect(mockApiResponse1.data.length).toBe(1);
    expect(mockApiResponse1.data).toEqual(mockApiResponse1.data);
  });

  const req = httpMock.expectOne(apiUrl);
  expect(req.request.method).toBe('GET');
  req.flush(mockApiResponse);
});
it('should handle an empty product list', () => {
  //Arrange
  const apiUrl = 'http://localhost:5007/api/Product/ProductSalesReport?page=1&pageSize=2&sortOrder=asc';
  const emptyResponse: ApiResponse<Product[]> = {
    success: true,
    message: '',
    data: [],
  };
  //Act
  service.getProductSalesReport(1,2,"asc").subscribe((response) => {
    expect(response.data.length).toBe(0);
    expect(response.data).toEqual([]);
  });

  const req = httpMock.expectOne(apiUrl);
  expect(req.request.method).toBe('GET');
  req.flush(emptyResponse);
});

it('should handle Http error gracefully', () => {
  //Arrange
  const apiUrl = 'http://localhost:5007/api/Product/ProductSalesReport?page=1&pageSize=2&sortOrder=asc';

  const errorMessage = 'Failed to load products';

  //Act
  service.getProductSalesReport(1,2,"asc").subscribe(
    () => fail('expected an error, not products'),
    (error) => {
      expect(error.status).toBe(500);
      expect(error.statusText).toBe('Internal Server Error');
    }
  );

  const req = httpMock.expectOne(apiUrl);
  expect(req.request.method).toBe('GET');
  //Respond With error
  req.flush(errorMessage, {
    status: 500,
    statusText: 'Internal Server Error',
  });
});
 //---------------------------------getProductsSoldCount-------------------
 it('should fetch total product count',()=>{
  //Arrange
  const mockSuccessResponse: ApiResponse<number> = {
    success: true,
    data: 2,
    message:""
  };

  //Act
  service.getProductsSoldCount().subscribe(response => {
    //Assert
    expect(response).toBe(mockSuccessResponse);
    expect(response.message).toBe('');
    expect(response.success).toBeTrue();
  });

  const req = httpMock.expectOne("http://localhost:5007/api/Product/GetProductsSoldCount");
  expect(req.request.method).toBe('GET');
  req.flush(mockSuccessResponse);

});

it('should handle a failed product count retival',()=>{
  //Arrange
  const mockErrorResponse: ApiResponse<number> = {
    success: false,
    data: 0,
    message:""
  };

  //Act
  service.getProductsSoldCount().subscribe(response => {
    //Assert
    expect(response).toBe(mockErrorResponse);
    expect(response.success).toBeFalse();
    expect(response.message).toEqual('');
  });

  const req = httpMock.expectOne("http://localhost:5007/api/Product/GetProductsSoldCount");

  expect(req.request.method).toBe('GET');
  req.flush(mockErrorResponse);
});

it('should handle Http errors for total',()=>{
  const mockHttpError = {
    status: 500,
    statusText: 'Internal Server Error'
  };
  //Act
  service.getProductsSoldCount().subscribe({
    next:() => fail('should have faild with the 500 error'),
    error: (error) =>{
      // Assert
      expect(error.status).toBe(500);
      expect(error.statusText).toBe('Internal Server Error');
    }
  });

  const req = httpMock.expectOne("http://localhost:5007/api/Product/GetProductsSoldCount");

  expect(req.request.method).toBe('GET');
  req.flush({},mockHttpError);

});


});
