import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgetpasswordComponent } from './forgetpassword.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormControl, FormsModule, NgForm } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { SecurityQuestion } from 'src/app/models/security-question.model';

describe('ForgetpasswordComponent', () => {
  let component: ForgetpasswordComponent;
  let fixture: ComponentFixture<ForgetpasswordComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: Router;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getAllQuestions', 'forgetPassword', 'signOut']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule, RouterTestingModule, FormsModule],
      declarations: [ForgetpasswordComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        // { provide: Router, useValue: routerSpy }
      ]
    });
    fixture = TestBed.createComponent(ForgetpasswordComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router);
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load questions on init', () => {
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        question: 'Question'
      }
    ];

    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    fixture.detectChanges();

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalled();
    expect(component.questions).toEqual(mockQuestions);
  })

  it('should set questions when questions are loaded successfully', () => {
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        question: 'Question'
      }
    ];

    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalled();
    expect(component.questions).toEqual(mockQuestions);
  });

  it('should set console error when loading questions fails', () => {
    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: [],
      success: false,
      message: 'Error fetching questions'
    };
    spyOn(console, 'error');

    authServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch security questions ', mockApiResponse.message)
  })

  it('should set console error when load questions returns error', () => {
    const mockError = { error: { message: 'HTTP error' } };
    authServiceSpy.forgetPassword.and.returnValue(throwError(mockError));
    spyOn(console, 'error');

    authServiceSpy.getAllQuestions.and.returnValue(throwError(mockError))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Error fetching security questions : ', mockError)
  })

  it('should invalidate password mismatch', () => {
    const form = <NgForm><unknown>{
      valid: true,
      value: {
          loginId: 'TestId',
          oldPassword: 'TestPassword@123',
          newPassword: 'TestPassword@1234',
          newConfirmPassword: 'TestPassword@12345',
      },
        controls: {
          loginId: { value: 'TestId' },
          oldPassword: { value: 'TestPassword@123' },
          newPassword: { value: 'TestPassword@1234' },
          newConfirmPassword: { value: 'TestPassword@12345' },
      },
    };
    form.controls['newPassword'] = new FormControl('password1');
    form.controls['confirmNewPassword'] = new FormControl('password2');

    component.checkPasswords(form);

    expect(form.controls['confirmNewPassword'].hasError('passwordMismatch')).toBeTrue();
  });

  it('should validate password', () => {
    const form = <NgForm><unknown>{
      valid: true,
      value: {
          loginId: 'TestId',
          oldPassword: 'TestPassword@123',
          newPassword: 'TestPassword@1234',
          newConfirmPassword: 'TestPassword@12345',
      },
        controls: {
          loginId: { value: 'TestId' },
          oldPassword: { value: 'TestPassword@123' },
          newPassword: { value: 'TestPassword@1234' },
          newConfirmPassword: { value: 'TestPassword@12345' },
      },
    };
    form.controls['newPassword'] = new FormControl('password1');
    form.controls['confirmNewPassword'] = new FormControl('password1');

    component.checkPasswords(form);

    expect(form.controls['confirmNewPassword'].errors).toBeNull();
  });
  
  it('should navigate to /signin on successful changing password', () => {
    spyOn(routerSpy, 'navigate');
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };
    authServiceSpy.forgetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        securityQuestionId: 1,
        answer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        securityQuestionId: { value: 1 },
        answer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
 
    component.onSubmit(form);
 
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/signin']);
  });

  it('should alert error message on unsuccessful password change', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error changing password.' };
    authServiceSpy.forgetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        securityQuestionId: 1,
        answer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        securityQuestionId: { value: 1 },
        answer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Error changing password.');
  });

  it('should alert error message on verification fails', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Verification failed!' };
    authServiceSpy.forgetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        securityQuestionId: 1,
        answer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        securityQuestionId: { value: 1 },
        answer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Verification failed!');
  });

  it('should alert error message on invaild username', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Invaild loginId.' };
    authServiceSpy.forgetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        securityQuestionId: 1,
        answer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        securityQuestionId: { value: 1 },
        answer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Invaild loginId.');
  });

  it('should alert error message on HTTP error', () => {
    spyOn(window, 'alert');
    const mockError = { error: { message: 'HTTP error' } };
    authServiceSpy.forgetPassword.and.returnValue(throwError(mockError));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        securityQuestionId: 1,
        answer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        securityQuestionId: { value: 1 },
        answer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
 
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('HTTP error');
  });
});
