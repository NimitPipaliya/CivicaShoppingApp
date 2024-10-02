import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RegisterUser } from '../models/register-user.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { User } from '../models/user.model';
import { SecurityQuestion } from '../models/security-question.model';
import { ChangePasswordModel } from '../models/changepassword.model';
import { ForgetPasswordModel } from '../models/forgetpassword.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });
  
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should sign up a user successfully', () => {
    // Arrange
    const mockUser: RegisterUser = {
      salutation: 'Mr.',
      name: 'John Doe',
      birthDate: '1990-01-01',
      loginId: 'johndoe',
      gender: 'Male',
      email: 'johndoe@example.com',
      phoneNumber: '1234567890',
      password: 'password123',
      confirmPassword: 'password123',
      securityQuestionId: 1,
      answer: 'answer123'
    };

    const mockResponse: ApiResponse<string> = {
      success: true,
      message: 'User registered successfully',
      data: ''
    };

    // Act
    service.signUp(mockUser).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User registered successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/Register');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });
  it('should not sign up a user', () => {
    // Arrange
    const mockUser: RegisterUser = {
      salutation: 'Mr.',
      name: 'John Doe',
      birthDate: '1990-01-01',
      loginId: 'johndoe',
      gender: 'Male',
      email: 'johndoe@example.com',
      phoneNumber: '1234567890',
      password: 'password123',
      confirmPassword: 'pas',
      securityQuestionId: 1,
      answer: 'answer123'
    };

    const mockResponse: ApiResponse<string> = {
      success: false,
      message: 'User failed to register',
      data: ''
    };

    // Act
    service.signUp(mockUser).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User failed to register');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/Register');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while signing up a user', () => {
    // Arrange
    const mockUser: RegisterUser = {
      salutation: 'Mr.',
      name: 'John Doe',
      birthDate: '1990-01-01',
      loginId: 'johndoe',
      gender: 'Male',
      email: 'johndoe@example.com',
      phoneNumber: '1234567890',
      password: 'password123',
      confirmPassword: 'password123',
      securityQuestionId: 1,
      answer: 'answer123'
    };

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.signUp(mockUser).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/Register');
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });

  it('should fetch all users successfully', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';

    const mockResponse: ApiResponse<User[]> = {
      success: true,
      message: 'Users fetched successfully',
      data: [
        {
          salutation: 'Mr.',
          name: 'John Doe',
          loginId: 'johndoe',
          phone: '1234567890',
          email: 'johndoe@example.com',
          userId: 0,
          birthDate: new Date('1990-01-01'),
          age: 34,
          gender: 'Male'
        },
        {
          salutation: 'Ms.',
          name: 'Jane Smith',
          loginId: 'janesmith',
          phone: '9876543210',
          email: 'janesmith@example.com',
          userId: 0,
          birthDate: new Date('1985-05-15'),
          age: 39,
          gender: 'Female'
        }
      ]
    };

    // Act
    service.getAllUsers(page, pageSize, sortOrder).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Users fetched successfully');
      expect(response.data.length).toBe(2); // Assuming two users are returned
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetAllUsers?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should fetch all users successfully with search query', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const search = "a";

    const mockResponse: ApiResponse<User[]> = {
      success: true,
      message: 'Users fetched successfully',
      data: [
        {
          salutation: 'Mr.',
          name: 'John Doe',
          loginId: 'johndoe',
          phone: '1234567890',
          email: 'johndoe@example.com',
          userId: 0,
          birthDate: new Date('1990-01-01'),
          age: 34,
          gender: 'Male'
        },
        {
          salutation: 'Ms.',
          name: 'Jane Smith',
          loginId: 'janesmith',
          phone: '9876543210',
          email: 'janesmith@example.com',
          userId: 0,
          birthDate: new Date('1985-05-15'),
          age: 39,
          gender: 'Female'
        }
      ]
    };

    // Act
    service.getAllUsers(page, pageSize, sortOrder, search).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Users fetched successfully');
      expect(response.data.length).toBe(2); // Assuming two users are returned
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetAllUsers?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should not fetch all users successfully', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';

    const mockResponse: ApiResponse<User[]> = {
      success: false,
      message: 'User failed to fetch',
      data: [
     
      ]
    };

    // Act
    service.getAllUsers(page, pageSize, sortOrder).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User failed to fetch');
      expect(response.data.length).toBe(0); // Assuming two users are returned
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetAllUsers?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while fetching all users', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.getAllUsers(page, pageSize, sortOrder).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetAllUsers?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should get user by ID successfully', () => {
    // Arrange
    const userId = 0;

    const mockResponse: ApiResponse<User> = {
      success: true,
      message: 'User fetched successfully',
      data: {
        salutation: 'Mr.',
        name: 'John Doe',
        loginId: 'johndoe',
        phone: '1234567890',
        email: 'johndoe@example.com',
        userId: 0,
        birthDate: new Date('1990-01-01'),
        age: 34,
        gender: 'Male'
      }
    };

    // Act
    service.getUserById(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User fetched successfully');
      expect(response.data.userId).toBe(userId);
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetUserById/${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should not get user by ID successfully', () => {
    // Arrange
    const userId = 0;

    const mockResponse: ApiResponse<User> = {
      success: false,
      message: 'User not fetched successfully',
      data: {
       
      } as User
    };

    // Act
    service.getUserById(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User not fetched successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetUserById/${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while getting user by ID', () => {
    // Arrange
    const userId = 1;

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.getUserById(userId).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/GetUserById/${userId}`);
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should fetch user count successfully', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: true,
      message: 'User count fetched successfully',
      data: 100 // Assuming there are 100 users
    };

    // Act
    service.fetchUserCount().subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User count fetched successfully');
      expect(response.data).toBe(100);
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetUsersCount');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
  it('should fetch user count successfully with search query', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: true,
      message: 'User count fetched successfully',
      data: 100 // Assuming there are 100 users
    };

    // Act
    service.fetchUserCount("a").subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User count fetched successfully');
      expect(response.data).toBe(100);
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetUsersCount?search=a');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should not fetch user count successfully', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: false,
      message: 'User count not fetched successfully',
      data: 0 // Assuming there are 100 users
    };

    // Act
    service.fetchUserCount().subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User count not fetched successfully');
      expect(response.data).toBe(0);
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetUsersCount');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while fetching user count', () => {
    // Arrange
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.fetchUserCount().subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetUsersCount');
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should delete a user successfully', () => {
    // Arrange
    const userId = 0;

    const mockResponse: ApiResponse<User> = {
      success: true,
      message: 'User deleted successfully',
      data: {
        salutation: 'Mr.',
        name: 'John Doe',
        loginId: 'johndoe',
        phone: '1234567890',
        email: 'johndoe@example.com',
        userId: 0,
        birthDate: new Date('1990-01-01'),
        age: 34,
        gender: 'Male'
      }
    };

    // Act
    service.deleteUser(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User deleted successfully');
      expect(response.data.userId).toBe(userId);
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/Remove/${userId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });
 
  it('should NOT delete a user successfully', () => {
    // Arrange
    const userId = 0;

    const mockResponse: ApiResponse<User> = {
      success: false,
      message: 'User not deleted successfully',
      data: {
      
      } as User
    };

    // Act
    service.deleteUser(userId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('User not deleted successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/Remove/${userId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while deleting a user', () => {
    // Arrange
    const userId = 1;

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.deleteUser(userId).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne(`http://localhost:5007/api/Auth/Remove/${userId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush({}, mockHttpError);
  });

  it('should fetch all security questions successfully', () => {
    // Arrange
    const mockResponse: ApiResponse<SecurityQuestion[]> = {
      success: true,
      message: 'Security questions fetched successfully',
      data: [
        { securityQuestionId: 1, question: 'What is your favorite color?' },
        { securityQuestionId: 2, question: 'What is your pet\'s name?' }
      ]
    };

    // Act
    service.getAllQuestions().subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Security questions fetched successfully');
      expect(response.data.length).toBe(2); // Assuming two security questions are returned
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetAllSecurityQuestions');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
 
  it('should not fetch all security questions successfully', () => {
    // Arrange
    const mockResponse: ApiResponse<SecurityQuestion[]> = {
      success: false,
      message: 'Security questions not fetched successfully',
      data: [
       
      ]
    };

    // Act
    service.getAllQuestions().subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Security questions not fetched successfully');
      expect(response.data.length).toBe(0); // Assuming two security questions are returned
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetAllSecurityQuestions');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while fetching all security questions', () => {
    // Arrange
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.getAllQuestions().subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/GetAllSecurityQuestions');
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should change password successfully', () => {
    // Arrange
    const mockChangePassword: ChangePasswordModel = {
      loginId: 'johndoe',
      oldPassword: 'oldPassword123',
      newPassword: 'newPassword123',
      newConfirmPassword: 'newPassword123'
    };

    const mockResponse: ApiResponse<string> = {
      success: true,
      message: 'Password changed successfully',
      data: ''
    };

    // Act
    service.changePassword(mockChangePassword).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Password changed successfully');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('should not change password successfully', () => {
    // Arrange
    const mockChangePassword: ChangePasswordModel = {
      loginId: 'johndoe',
      oldPassword: 'oldPassword123',
      newPassword: 'newPassword123',
      newConfirmPassword: 'neassword123'
    };

    const mockResponse: ApiResponse<string> = {
      success: false,
      message: 'Password not changed',
      data: ''
    };

    // Act
    service.changePassword(mockChangePassword).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Password not changed');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while changing password', () => {
    // Arrange
    const mockChangePassword: ChangePasswordModel = {
      loginId: 'johndoe',
      oldPassword: 'oldPassword123',
      newPassword: 'newPassword123',
      newConfirmPassword: 'newPassword123'
    };

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.changePassword(mockChangePassword).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });

  it('should set forget password successfully', () => {
    // Arrange
    const mockForgetPassword: ForgetPasswordModel = {
      loginId: 'johndoe',
      securityQuestionId: 1,
      answer: 'answer123',
      newPassword: 'newPassword123',
      confirmNewPassword: 'newPassword123'
    };

    const mockResponse: ApiResponse<string> = {
      success: true,
      message: 'Password reset successful',
      data: ''
    };

    // Act
    service.forgetPassword(mockForgetPassword).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Password reset successful');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/ForgetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('should not set forget password', () => {
    // Arrange
    const mockForgetPassword: ForgetPasswordModel = {
      loginId: 'johndoe',
      securityQuestionId: 1,
      answer: 'answer123',
      newPassword: 'newPassword123',
      confirmNewPassword: 'neassword123'
    };

    const mockResponse: ApiResponse<string> = {
      success: false,
      message: 'Password reset failed',
      data: ''
    };

    // Act
    service.forgetPassword(mockForgetPassword).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
      expect(response.message).toBe('Password reset failed');
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/ForgetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('should handle HTTP error while forgetting password', () => {
    // Arrange
    const mockForgetPassword: ForgetPasswordModel = {
      loginId: 'johndoe',
      securityQuestionId: 1,
      answer: 'answer123',
      newPassword: 'newPassword123',
      confirmNewPassword: 'newPassword123'
    };

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act & Assert
    service.forgetPassword(mockForgetPassword).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error => {
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      })
    });

    // Mock HTTP request
    const req = httpMock.expectOne('http://localhost:5007/api/Auth/ForgetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });


});
