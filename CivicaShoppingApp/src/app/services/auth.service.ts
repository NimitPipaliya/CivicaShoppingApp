import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '../models/ApiResponse{T}.model';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { User } from '../models/user.model';
import { RegisterUser } from '../models/register-user.model';
import { SecurityQuestion } from '../models/security-question.model';

import { LocalstorageService } from './helpers/localstorage.service';
import { LocalStorageKeys } from './helpers/localstoragekeys';
import { ChangePasswordModel } from '../models/changepassword.model';
import { ForgetPasswordModel } from '../models/forgetpassword.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = "http://localhost:5007/api/Auth/"
  private authState = new BehaviorSubject<boolean>(this.localStorageHelper.hasItem(LocalStorageKeys.TokenName));
  private usernameSubject = new BehaviorSubject<string | null | undefined>(this.localStorageHelper.getItem(LocalStorageKeys.LoginId));
  private userIdSubject = new BehaviorSubject<string | null | undefined>(this.localStorageHelper.getItem(LocalStorageKeys.UserId));

  constructor(private http: HttpClient, private localStorageHelper: LocalstorageService) { }

  signIn(username: string, password: string): Observable<ApiResponse<string>> {
    const body = { username, password };
    return this.http.post<ApiResponse<string>>(this.apiUrl + 'Login', body).pipe(
      tap(response => {
        if (response.success) {
          const token = response.data;
          const payload = token.split('.')[1];
          const decodedPayload = JSON.parse(atob(payload));
          const userId = decodedPayload.UserId;
          this.localStorageHelper.setItem(LocalStorageKeys.TokenName, token);
          this.localStorageHelper.setItem(LocalStorageKeys.LoginId, username);
          this.localStorageHelper.setItem(LocalStorageKeys.UserId, userId);
          this.authState.next(this.localStorageHelper.hasItem(LocalStorageKeys.TokenName));
          this.usernameSubject.next(username);
          this.userIdSubject.next(userId);
        }
      })
    );
  }

  signUp(user: RegisterUser): Observable<ApiResponse<string>> {
    const body = user;
    return this.http.post<any>(this.apiUrl + "Register", body);
  }

  signOut() {
    this.localStorageHelper.removeItem(LocalStorageKeys.TokenName);
    this.localStorageHelper.removeItem(LocalStorageKeys.LoginId);
    this.localStorageHelper.removeItem(LocalStorageKeys.UserId);
    this.authState.next(false);
    this.usernameSubject.next(null);
    this.userIdSubject.next(null);
  }

  isAuthenticated() {
    return this.authState.asObservable();
  }

  getAllUsers(page: number, pageSize: number, sortOrder: string, search?: string): Observable<ApiResponse<User[]>> {
    if (search == null) {
      return this.http.get<ApiResponse<User[]>>(this.apiUrl + "GetAllUsers?page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder);
    }
    else {
      return this.http.get<ApiResponse<User[]>>(this.apiUrl + "GetAllUsers?search=" + search + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder);

    }
  }

  getUserById(userId: number): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(this.apiUrl + 'GetUserById/' + userId);
  }

  fetchUserCount(search?: string): Observable<ApiResponse<number>> {
    if (search == null) {
      return this.http.get<ApiResponse<number>>(this.apiUrl + "GetUsersCount");
    }
    else {
      return this.http.get<ApiResponse<number>>(this.apiUrl + "GetUsersCount?search=" + search);

    }
  }


  deleteUser(userId: number): Observable<ApiResponse<User>> {
    return this.http.delete<ApiResponse<User>>(`${this.apiUrl}Remove/${userId}`)

  }

  getAllQuestions(): Observable<ApiResponse<SecurityQuestion[]>> {
    return this.http.get<ApiResponse<SecurityQuestion[]>>(this.apiUrl + 'GetAllSecurityQuestions')
  }
  getUsername(): Observable<string | null | undefined> {
    return this.usernameSubject.asObservable();
  }
  changePassword(changePassword: ChangePasswordModel): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(this.apiUrl + 'ChangePassword', changePassword)
  }

  forgetPassword(forgetPassword: ForgetPasswordModel): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(this.apiUrl + 'ForgetPassword', forgetPassword)
  }


  getUserId(): Observable<string | null | undefined> {
    return this.userIdSubject.asObservable();
  }


}
