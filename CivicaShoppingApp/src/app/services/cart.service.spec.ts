import { TestBed } from '@angular/core/testing';
import { UpdateCart } from '../models/update-cart.model';
import { CartService } from './cart.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AddToCart } from '../models/add-to-cart.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Cart } from '../models/cart.model';

describe('CartService', () => {
  let service: CartService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers : [CartService]

    });
    service = TestBed.inject(CartService);
    httpMock = TestBed.inject(HttpTestingController);

  }); 

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('shuould add to cart',()=>{
    //arrange
   const addToCart : AddToCart ={
     userId: 1,
     productId: 1,
     productQuantity: 2
   }

   //Act
   const mockSuccessResponse :ApiResponse<string> ={
     success : true,
     message : "Add to cart successfully",
     data : ''
   }
    //act
    service.addToCart(addToCart).subscribe(response=>{
     expect(response).toBe(mockSuccessResponse);
   });
   const req = httpMock.expectOne('http://localhost:5007/api/Cart/AddToCart');
   expect(req.request.method).toBe('POST');
   req.flush(mockSuccessResponse);
 });

 it('should handle an error while addition of Cart',()=>{
   //Arrange
   const addToCart : AddToCart ={
    userId: 1,
    productId: 1,
    productQuantity: 2
  }

   const mockErrorResponse : ApiResponse<string> ={
     success :false,
    message : 'Something went wrong',
     data : " "
   };

   //Act
   service.addToCart(addToCart).subscribe(response=>{
    expect(response).toBe(mockErrorResponse);
  });
  const req = httpMock.expectOne('http://localhost:5007/api/Cart/AddToCart');
  expect(req.request.method).toBe('POST');
   req.flush(mockErrorResponse);

 })

 it('should handle an http error for addition',()=>{
   //Arrange
   const addToCart : AddToCart ={
    userId: 1,
    productId: 1,
    productQuantity: 2
  }

   const mockHttpError ={
     statusText: "Internal Server Error",
     status: 500
     };
   //Act
    //Act
    service.addToCart(addToCart).subscribe({
     next:()=> fail('should have failed with the 500 error'),
     error: (error=> {
      expect(error.status).toEqual(500);
      expect(error.statusText).toEqual("Internal Server Error");
     })
  });
  const req = httpMock.expectOne('http://localhost:5007/api/Cart/AddToCart');
  expect(req.request.method).toBe('POST');
   req.flush({},mockHttpError);
   

 });

 it('should update a Cart successfully', ()=>{
  const UpdateCart : UpdateCart={
    userId: 1,
    productId: 1,
    productQuantity: 2,
    cartId: 1
  }
 
 
  const mockSuccessResponse : ApiResponse<UpdateCart> ={
    success: true,
    message: "Cart updated successfully.",
    data: {} as UpdateCart
        }
 
  //Act
  service.modifyCart(UpdateCart).subscribe(response => {
    expect(response).toEqual(mockSuccessResponse);
  });
 
  const req = httpMock.expectOne( 'http://localhost:5007/api/Cart/UpdateCart');
  expect(req.request.method).toBe('PUT');
  req.flush(mockSuccessResponse);
 
  });
 
 
  it('should handle failed update of Cart', ()=>{
    const UpdateCart : UpdateCart={
      userId: 1,
      productId: 1,
      productQuantity: 2,
      cartId: 1
    } 
    const mockErrorResponse : ApiResponse<UpdateCart> ={
      success: false,
      message: "Cart already exists.",
      data: {} as UpdateCart
          }
   
    //Act
    service.modifyCart(UpdateCart).subscribe(response => {
      expect(response).toEqual(mockErrorResponse);
    });
   
    const req = httpMock.expectOne( 'http://localhost:5007/api/Cart/UpdateCart');
    expect(req.request.method).toBe('PUT');
    req.flush(mockErrorResponse);
   
    });
 
    it('should handle http error for update', ()=>{
      const UpdateCart : UpdateCart={
        userId: 1,
        productId: 1,
        productQuantity: 2,
        cartId: 1
      }
      const mockHttpError ={
        statusText: "Internal Server Error",
        status: 500
        };
     
      //Act
      service.modifyCart(UpdateCart).subscribe({
       next:()=> fail('should have failed with the 500 error'),
       error: (error=> {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual("Internal Server Error");
       })
    });
     
    const req = httpMock.expectOne( 'http://localhost:5007/api/Cart/UpdateCart');
    expect(req.request.method).toBe('PUT');
      req.flush({},mockHttpError);
     
      });

      it('should fetch cart by id',()=>{
        const userId = 1;
        const mockSuccessResponse :ApiResponse<Cart[]>={
          success :true,
          data: [
            {
              cartId: 1,
              userId: 1,
              productId: 1,
              productQuantity: 10,
              product: {
                productId: 1,
                productName: 'Sample Product',
                productDescription: 'Sample Description',
                quantity: 100,
                productPrice: 50,
                gstPercentage: 18,
                finalPrice: 59
              },
              isAddedToCart: false
            },
            {
              cartId: 2,
              userId: 1,
              productId: 2,
              productQuantity: 5,
              product: {
                productId: 2,
                productName: 'Another Product',
                productDescription: 'Another Description',
                quantity: 50,
                productPrice: 100,
                gstPercentage: 18,
                finalPrice: 118
              },
              isAddedToCart: true
            }
          ],
          message:''
        };
        //act
        service.getCartItemsByUserId(userId).subscribe(response =>{
          //assert
          expect(response).toBe(mockSuccessResponse);
          expect(response.data.length).toBe(2); // Assuming two items are returned
          expect(response.data[0].userId).toEqual(userId);
     
        });
        const req =httpMock.expectOne('http://localhost:5007/api/Cart/GetCartItemsByUserId?userId=' +userId);
        expect(req.request.method).toBe('GET');
        req.flush(mockSuccessResponse);
      });
     
      it('should handle failed cart retrival',()=>{
        //arrange
        const userId =1;
        const mockErrorResponse : ApiResponse<Cart[]>={
          success : false,
          data: {} as Cart[],
          message : "No record found"
        };
        //act
        service.getCartItemsByUserId(userId).subscribe(response => {
          //assert
          expect(response).toEqual(mockErrorResponse);
          expect(response.message).toEqual("No record found");
          expect(response.success).toBeFalse();
     
        });
        const req =httpMock.expectOne('http://localhost:5007/api/Cart/GetCartItemsByUserId?userId=' +userId);
        expect(req.request.method).toBe('GET');
        req.flush(mockErrorResponse);
     
     });
     
     it('should handle http error for get by cart by id',()=>{
      const userId =1;
      const mockHttpError ={
        status: 500,
        statusText :'Internal server error'
      };
      //act
      service.getCartItemsByUserId(userId).subscribe({
        next : ()=> fail('should have faild with 500 error'),
        error:(error)=>{
          //assert
          expect(error.status).toBe(500);
          expect(error.statusText).toBe('Internal server error');
        }
      });
      const req =httpMock.expectOne('http://localhost:5007/api/Cart/GetCartItemsByUserId?userId=' +userId);
      expect(req.request.method).toBe('GET');
      req.flush({},mockHttpError);
     
     
     });
     

     it('should delete successfully',()=>{
      //arrange
       const userId =1;
       const productId =1;
        const mockSuccessResponse :ApiResponse<string>={
          success : false,
          data: "",
          message : "deleted successfully"
     
        };
        service.removeParticularProductFromCart(userId,productId).subscribe(response=>{
          //Assert
          expect(response).toEqual(mockSuccessResponse);
          expect(response.message).toBe("deleted successfully");
          expect(response.data).toEqual;(mockSuccessResponse.data);
     
        });
        const req = httpMock.expectOne("http://localhost:5007/api/Cart/RemoveParticularProductFromCart?userId="+ userId + "&productId=" + productId);
        expect(req.request.method).toBe('DELETE');
        req.flush(mockSuccessResponse);
     })
     
     it('should not delete the contact by id successfully',()=>{
      //arrange
      const userId =1;
      const productId =1;
      
      const mockErrorResponse :ApiResponse<string>={
          success : false,
          data: "",
          message : " not deleted successfully"
     
        };
        service.removeParticularProductFromCart(userId,productId).subscribe(response=>{
          //Assert
          expect(response).toEqual(mockErrorResponse);
          expect(response.message).toBe(" not deleted successfully");
          expect(response.data).toEqual;(mockErrorResponse.data);
     
        });
        const req = httpMock.expectOne("http://localhost:5007/api/Cart/RemoveParticularProductFromCart?userId="+ userId + "&productId=" + productId);
        expect(req.request.method).toBe('DELETE');
        req.flush(mockErrorResponse);
     });
     
     
     it('should handle http error while deleting',()=>{
      const userId =1;
      const productId =1;
            const mockHttpError ={
        status: 500,
        statusText :'Internal server error'
      };
      //act
      service.removeParticularProductFromCart(userId,productId).subscribe({
        next : ()=> fail('should have faild with 500 error'),
        error:(error)=>{
          //assert
          expect(error.status).toBe(500);
          expect(error.statusText).toBe('Internal server error');
        }
      });
      const req = httpMock.expectOne("http://localhost:5007/api/Cart/RemoveParticularProductFromCart?userId="+ userId + "&productId=" + productId);
      expect(req.request.method).toBe('DELETE');
        req.flush({},mockHttpError);
     
     
     });
     
 
 
});
