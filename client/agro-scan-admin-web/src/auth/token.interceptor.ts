import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const accessToken = this.authService.getAccessToken();
    const refreshToken = this.authService.getRefreshToken();

    // Add the access token to the request header if available
    if (accessToken) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && refreshToken) {
          // If the error is due to unauthorized access, try to refresh the token
          return this.authService.refresh().pipe(
            switchMap((newAccessToken: string) => {
              // If the refresh is successful, update the request with the new access token
              req = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newAccessToken}`,
                },
              });
              return next.handle(req);
            }),
            catchError((refreshError) => {
              // If refreshing fails, you can handle it here or rethrow the error
              return throwError(refreshError);
            })
          );
        } else {
          // For other errors or when there's no refresh token, rethrow the error
          return throwError(error);
        }
      })
    );
  }
}
