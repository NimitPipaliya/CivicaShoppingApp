import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LocalstorageService } from '../services/helpers/localstorage.service';
import { LocalStorageKeys } from '../services/helpers/localstoragekeys';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService, private router: Router, private localStorageHelper: LocalstorageService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.localStorageHelper.getItem(LocalStorageKeys.TokenName);
    if(token){
      const cloneReq = request.clone({
        headers: request.headers.set('Authorization',`Bearer ${token}`)
      });
      // return next.handle(cloneRequest);
      return next.handle(cloneReq).pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            this.authService.signOut();
            this.router.navigate(['/signin']);
          }
          return throwError(error);
        })
      );
    }
    else{
      return next.handle(request);
    }
  }
}
