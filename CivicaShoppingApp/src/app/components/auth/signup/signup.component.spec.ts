import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SignupComponent } from './signup.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { SecurityQuestion } from 'src/app/models/security-question.model';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { HomeComponent } from '../../home/home.component';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: Router;

  beforeEach(() => {
    let authServiceSpyObj = jasmine.createSpyObj('AuthService', ['getAllQuestions', 'forgetPassword', 'signUp']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ReactiveFormsModule,FormsModule, RouterTestingModule.withRoutes([{ path: 'home', component: HomeComponent}])],
      declarations: [SignupComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpyObj },
      ]
    });
    fixture = TestBed.createComponent(SignupComponent);
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
    expect(console.error).toHaveBeenCalledWith('Failed to fetch questions ', mockApiResponse.message)
  });

  it('should set console error when loading questions fails', () => {
    let mockError = { error: { message: 'Failed' }}
    spyOn(console, 'error');

    authServiceSpy.getAllQuestions.and.returnValue(throwError(mockError))
    component.loadQuestions()

    expect(authServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Error fetching questions : ', mockError)
  });

  it('should navigate to home when user signs up successfully', () => {
    spyOn(routerSpy, 'navigate');
    spyOn(window, 'alert');
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        question: 'Question'
      }
    ];
    let mockQuestionsResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };
    const mockResponse: ApiResponse<string> = {
      success: true,
      data: '',
      message: '',
    }
    const mockSignUp = {
      salutation: 'Mr.',
      name: 'Test',
      loginId: 'test',
      phone: '1234567890',
      email: 'test@test.com',
      password: 'Password@123',
      confirmPassword: 'Password@123',
      birthDate: new Date(1/1/2001),
      gender: 'M',
      securityQuestionId: 1,
      answer: 'answer'
    }
    authServiceSpy.getAllQuestions.and.returnValue(of(mockQuestionsResponse))
    authServiceSpy.signUp.and.returnValue(of(mockResponse));
    
    fixture.detectChanges();
    component.signUpForm.setValue(mockSignUp);
    
    component.onSubmit()

    expect(authServiceSpy.signUp).toHaveBeenCalledWith(component.signUpForm.value)
    expect(window.alert).toHaveBeenCalledOnceWith('User registered successfully!!');
    expect(routerSpy.navigate).toHaveBeenCalledOnceWith(['/home'])
  })

  it('should set alert when response is false', () => {
    spyOn(routerSpy, 'navigate');
    spyOn(window, 'alert');
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        question: 'Question'
      }
    ];
    let mockQuestionsResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };
    const mockResponse: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Error while registering',
    }
    const mockSignUp = {
      salutation: 'Mr.',
      name: 'Test',
      loginId: 'test',
      phone: '1234567890',
      email: 'test@test.com',
      password: 'Password@123',
      confirmPassword: 'Password@123',
      birthDate: new Date(1/1/2001),
      gender: 'M',
      securityQuestionId: 1,
      answer: 'answer'
    }
    authServiceSpy.getAllQuestions.and.returnValue(of(mockQuestionsResponse))
    authServiceSpy.signUp.and.returnValue(of(mockResponse));
    
    fixture.detectChanges();
    component.signUpForm.setValue(mockSignUp);
    
    component.onSubmit()

    expect(authServiceSpy.signUp).toHaveBeenCalledWith(component.signUpForm.value)
    expect(window.alert).toHaveBeenCalledOnceWith(mockResponse.message);
  })

  it('should set alert when api returns error', () => {
    spyOn(routerSpy, 'navigate');
    spyOn(window, 'alert');
    let mockQuestions: SecurityQuestion[] = [
      {
        securityQuestionId: 1,
        question: 'Question'
      }
    ];
    let mockQuestionsResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };
    const mockError = { error: {message: 'HTTP Error' } }
    const mockSignUp = {
      salutation: 'Mr.',
      name: 'Test',
      loginId: 'test',
      phone: '1234567890',
      email: 'test@test.com',
      password: 'Password@123',
      confirmPassword: 'Password@123',
      birthDate: new Date(1/1/2001),
      gender: 'M',
      securityQuestionId: 1,
      answer: 'answer'
    }
    authServiceSpy.getAllQuestions.and.returnValue(of(mockQuestionsResponse))
    authServiceSpy.signUp.and.returnValue(throwError(mockError));
    
    fixture.detectChanges();
    component.signUpForm.setValue(mockSignUp);
    
    component.onSubmit()

    expect(authServiceSpy.signUp).toHaveBeenCalledWith(component.signUpForm.value)
    expect(window.alert).toHaveBeenCalledOnceWith(mockError.error.message);
  })

});
