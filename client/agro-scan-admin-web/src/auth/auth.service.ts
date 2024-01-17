import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  authState: any = {
    email: null,
    accessToken: null,
    refreshToken: null,
    authenticated: null
  };

  constructor() { }
}
