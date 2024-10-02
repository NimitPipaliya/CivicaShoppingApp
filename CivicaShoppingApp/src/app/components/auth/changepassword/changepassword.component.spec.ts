import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ChangepasswordComponent } from './changepassword.component';
import { AbstractControl, FormControl, FormsModule, NgForm } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { ChangePasswordModel } from 'src/app/models/changepassword.model';

describe('ChangepasswordComponent', () => {
  let component: ChangepasswordComponent;
  let fixture: ComponentFixture<ChangepasswordComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: Router;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getUsername', 'changePassword', 'signOut']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule, RouterTestingModule, FormsModule],
      declarations: [ChangepasswordComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
      ]
    });
    fixture = TestBed.createComponent(ChangepasswordComponent);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router);
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

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
    form.controls['newConfirmPassword'] = new FormControl('password2');

    component.checkPasswords(form);

    expect(form.controls['newConfirmPassword'].hasError('passwordMismatch')).toBeTrue();
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
    form.controls['newConfirmPassword'] = new FormControl('password1');

    component.checkPasswords(form);

    expect(form.controls['newConfirmPassword'].errors).toBeNull();
  });

  it('should get username on init', () => {
    authServiceSpy.getUsername.and.returnValue(of('testLoginId'));

    fixture.detectChanges();
    expect(authServiceSpy.getUsername());
  })

  it('should navigate to /signin on successful password change', () => {
    spyOn(routerSpy, 'navigate')
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };
    authServiceSpy.changePassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        oldPassword: 'TestPassword@123',
        newPassword: 'TestPassword@1234',
        newConfirmPassword: 'TestPassword@1234',
      },
      controls: {
        loginId: { value: 'TestId' },
        oldPassword: { value: 'TestPassword@123' },
        newPassword: { value: 'TestPassword@1234' },
        newConfirmPassword: { value: 'TestPassword@1234' },
      }
    };
 
    component.onSubmit(form);
    expect(authServiceSpy.signOut());
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/signin']);
    expect(component.loading).toBe(false);
  });
 
  it('should alert error message on unsuccessful password change', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error changing password' };
    authServiceSpy.changePassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        oldPassword: 'TestPassword@123',
        newPassword: 'TestPassword@1234',
        newConfirmPassword: 'TestPassword@1234',
      },
      controls: {
        loginId: { value: 'TestId' },
        oldPassword: { value: 'TestPassword@123' },
        newPassword: { value: 'TestPassword@1234' },
        newConfirmPassword: { value: 'TestPassword@1234' },
      }
    };
 
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Error changing password');
    expect(component.loading).toBe(false);
  });
 
  it('should alert error message on HTTP error', () => {
    spyOn(window, 'alert');
    const mockError = { error: { message: 'HTTP error' } };
    authServiceSpy.changePassword.and.returnValue(throwError(mockError));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'TestId',
        oldPassword: 'TestPassword@123',
        newPassword: 'TestPassword@1234',
        newConfirmPassword: 'TestPassword@1234',
      },
      controls: {
        loginId: { value: 'TestId' },
        oldPassword: { value: 'TestPassword@123' },
        newPassword: { value: 'TestPassword@1234' },
        newConfirmPassword: { value: 'TestPassword@1234' },
      }
    };
 
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('HTTP error');
    expect(component.loading).toBe(false);
  });
 
  it('should not call authservice.changePassword on invalid form submission', () => {
    const form = <NgForm>{ valid: false };
 
    component.onSubmit(form);
 
    expect(authServiceSpy.changePassword).not.toHaveBeenCalled();
    expect(component.loading).toBe(false);
  });
});
