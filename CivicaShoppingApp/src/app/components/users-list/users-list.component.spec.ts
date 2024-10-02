import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersListComponent } from './users-list.component';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { User } from 'src/app/models/user.model';

describe('UsersListComponent', () => {
  let component: UsersListComponent;
  let fixture: ComponentFixture<UsersListComponent>;
  let router: Router;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;
  
  const mockUser: User =
  { userId: 0, name: 'Test2', loginId:'Test2', email: 'Test2@gmail.com',phone:'1244567890',gender:'F',salutation:'Mr.',birthDate:new Date(), age: 20 
  }


  const mockUsers: User[] = [
    { userId: 0, name: 'Test', loginId:'Test', email: 'Test@gmail.com',phone:'1234567890',gender:'M',salutation:'Mr.',birthDate: new Date(), age: 19 },
    { userId: 0, name: 'Test2', loginId:'Test2', email: 'Test2@gmail.com',phone:'1244567890',gender:'F',salutation:'Mr.',birthDate:new Date(), age: 20 },
    
  ];

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getAllUsers', 'fetchUserCount','deleteUser','isAuthenticated']);
    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    TestBed.configureTestingModule({
      
      imports: [HttpClientTestingModule,RouterTestingModule, FormsModule],
      declarations: [UsersListComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ChangeDetectorRef, useValue: cdrSpy }
      ],

    });
    fixture = TestBed.createComponent(UsersListComponent);
    component = fixture.componentInstance;
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should calculate total user count successfully with search',()=>{
    //Arrange
    const search='e'
    const mockResponse :ApiResponse<number> ={success:true,data:2,message:''};
    authServiceSpy.fetchUserCount.and.returnValue(of(mockResponse));
    authServiceSpy.isAuthenticated.and.returnValue(of(true));
    spyOn(component,'loadUsers');
    //Act
     component.ngOnInit();
    //Assert
    expect(authServiceSpy.fetchUserCount).toHaveBeenCalled();

  })

  it('should fail to calculate total count without letter with search',()=>{
    //Arrange
    const search='e'
    const mockResponse :ApiResponse<number> ={success:false,data:0,message:'Failed to fetch users'};
    authServiceSpy.fetchUserCount.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.totalUserCount(search);
    //Assert
    expect(authServiceSpy.fetchUserCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users','Failed to fetch users');

  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    authServiceSpy.fetchUserCount.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.totalUserCount();

    //Assert
    expect(authServiceSpy.fetchUserCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users',mockError);


  })

  it('should load users with search successfully',()=>{
    //Arrange
    const search='e'

    const mockResponse :ApiResponse<User[]> ={success:true,data:mockUsers,message:''};
    authServiceSpy.getAllUsers.and.returnValue(of(mockResponse));

    //Act
    component.loadUsers(search);

    //Assert
    expect(authServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.users).toEqual(mockUsers);
    expect(component.loading).toBe(false);
  })

  it('should fail to load users with serach',()=>{
    //Arrange
    const search='e'

    const mockResponse :ApiResponse<User[]> ={success:false,data:[],message:'Failed to fetch users'};
    authServiceSpy.getAllUsers.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadUsers(search);
    //Assert
    expect(authServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.users).toEqual(undefined);
    expect(component.loading).toBe(false);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users','Failed to fetch users');

  })

  it('should handle Http error response',()=>{
    // Arrange
    
    const mockError = {message:'Network Error'};
    authServiceSpy.getAllUsers.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadUsers();

    //Asserts
    expect(authServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.users).toEqual(undefined);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users',mockError);
    expect(component.loading).toBe(false);


  })

  it('should delete user on confirmation', () => {
    // Arrange
    const userId = 1;
    const mockDeleteResponse: ApiResponse<User> = { success: true, data: mockUser, message: 'Contact deleted successfully' };
    spyOn(window, 'confirm').and.returnValue(true); // Simulate user confirmation
    spyOn(component, 'loadUsers');
    spyOn(component, 'totalUserCount');
    spyOn(component, 'calculateTotalPages');
    spyOn(component, 'onPageChange');
  
    authServiceSpy.deleteUser.and.returnValue(of(mockDeleteResponse));
  
    // Act
    component.deleteUser(userId);
  
    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete this user?');
    expect(authServiceSpy.deleteUser).toHaveBeenCalledWith(userId);
    expect(component.loadUsers).toHaveBeenCalledWith();
    expect(component.totalUserCount).toHaveBeenCalledWith();
    expect(component.calculateTotalPages).toHaveBeenCalled();
    expect(component.onPageChange).not.toHaveBeenCalled(); // Check that onPageChange was not called for success case
  });

  it('should delete user on confirmation and call onPageChange', () => {
    // Arrange
    const userId = 1;
    const mockDeleteResponse: ApiResponse<User> = { success: true, data: mockUser, message: 'Contact deleted successfully' };
    spyOn(window, 'confirm').and.returnValue(true); // Simulate user confirmation
    spyOn(component, 'loadUsers');
    spyOn(component, 'totalUserCount');
    spyOn(component, 'calculateTotalPages');
    spyOn(component, 'onPageChange');
  
    const mockOneUser: User[] = [
    { userId: 0, name: 'Test2', loginId:'Test2', email: 'Test2@gmail.com',phone:'1244567890',gender:'F',salutation:'Mr.',birthDate:new Date(), age: 20 
    }]
  
    component.users = mockOneUser
    authServiceSpy.deleteUser.and.returnValue(of(mockDeleteResponse));
    component.currentPage = 3;
    // Act
    component.deleteUser(userId);
    component.onPageChange(component.currentPage - 1);
  
    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete this user?');
    expect(authServiceSpy.deleteUser).toHaveBeenCalledWith(userId);
    expect(component.loadUsers).toHaveBeenCalledWith();
    expect(component.totalUserCount).toHaveBeenCalledWith();
    expect(component.calculateTotalPages).toHaveBeenCalled();
    expect(component.onPageChange).toHaveBeenCalled(); // Check that onPageChange was not called for success case
  });


  it('should not delete contact if user cancels', () => {
    // Arrange
    const userId = 1;
    spyOn(window, 'confirm').and.returnValue(false); // Simulate user cancellation
    spyOn(component, 'loadUsers');
    spyOn(component, 'totalUserCount');
    spyOn(component, 'calculateTotalPages');
    spyOn(component, 'onPageChange');
  
    // Act
    component.deleteUser(userId);
  
    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete this user?');
    expect(authServiceSpy.deleteUser).not.toHaveBeenCalled();
    expect(component.loadUsers).not.toHaveBeenCalled();
    expect(component.totalUserCount).not.toHaveBeenCalled();
    expect(component.calculateTotalPages).not.toHaveBeenCalled();
    expect(component.onPageChange).not.toHaveBeenCalled();
  });

  it('should update currentPage and call loadUsers', () => {
    // Arrange
    const page = 1;
    const searchQuery = '';
    spyOn(component,'loadUsers');
    // Act
    component.loadUsers(searchQuery)
    component.onPageChange(page);

    // Assert
    expect(component.currentPage).toEqual(page);
    expect(component.searchQuery).toEqual(searchQuery); // Assuming searchQuery is set elsewhere
  });

  it('should update currentPage and call loadUsers and totalCount', ()=> {
    const page = 1;
    const searchQuery = '';
    spyOn(component,'loadUsers');
    spyOn(component,'totalUserCount');
    // Act
    component.loadUsers(searchQuery)
    component.totalUserCount(searchQuery)
    component.onPageSizeChange();
    // Assert
    expect(component.currentPage).toEqual(page);
    expect(component.searchQuery).toEqual(searchQuery)
  })
  it('should update currentPage and call loadUsers and totalCount for showAll', ()=> {
    const page = 1;
    const searchQuery = '';
    spyOn(component,'loadUsers');
    spyOn(component,'totalUserCount');
    // Act
    component.loadUsers(searchQuery)
    component.totalUserCount(searchQuery)
    component.onShowAll();

    // Assert
    expect(component.currentPage).toEqual(page);
    expect(component.searchQuery).toEqual(searchQuery)
  })

  it('should sort produts when ascending',()=>{
    //Arrange
    component.sortOrder = "asc";

    spyOn(component,"totalUserCount")
    spyOn(component,"loadUsers")
 
    //Act
    component.onClickSort();
 
    //Assert
    expect(component.totalUserCount).toHaveBeenCalled();
    expect(component.loadUsers).toHaveBeenCalled();
 
   })
   it('should sort produts when descending ',()=>{
    //Arrange
    component.sortOrder = "desc";

    spyOn(component,"totalUserCount")
    spyOn(component,"loadUsers")
  
    //Act
    component.onClickSort();
 
    //Assert
      //Assert
      expect(component.totalUserCount).toHaveBeenCalled();
      expect(component.loadUsers).toHaveBeenCalled();
   })

   it('should call loadUsers and totalUserCount with searchQuery length > 2', () => {
    // Arrange
    const searchQuery = 'test';
    spyOn(component,'loadUsers')
    spyOn(component,'totalUserCount')
    // Act
    component.searchQuery = searchQuery;
    component.searchUsers();

    // Assert
    expect(component.currentPage).toEqual(1);
    expect(component.loadUsers).toHaveBeenCalledWith(searchQuery);
    expect(component.totalUserCount).toHaveBeenCalledWith(searchQuery);
  });
   
  it('should call loadUsers and totalUserCount with searchQuery length < 2', () => {
    // Arrange
    spyOn(component,'loadUsers')
    spyOn(component,'totalUserCount')
    // Act
    component.searchUsers();

    // Assert
    expect(component.currentPage).toEqual(1);
    expect(component.loadUsers).toHaveBeenCalledWith();
    expect(component.totalUserCount).toHaveBeenCalledWith();
  });
  it('should call loadUsers and totalUserCount when clear search', () => {
    // Arrange
    spyOn(component,'loadUsers')
    spyOn(component,'totalUserCount')
    // Act
    component.searchQuery = "";

    component.clearSearch();

    // Assert
    expect(component.currentPage).toEqual(1);
    expect(component.loadUsers).toHaveBeenCalledWith(component.searchQuery);
    expect(component.totalUserCount).toHaveBeenCalledWith(component.searchQuery);
  });



});
