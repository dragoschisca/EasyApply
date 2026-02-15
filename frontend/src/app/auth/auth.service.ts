import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

export interface User {
  email: string;
  password: string;
  userType: 'candidate' | 'company';
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly mockUser: User = {
    email: 'admin',
    password: 'password',
    userType: 'candidate'
  };

  constructor(private router: Router) {}

  login(email: string, password: string): boolean {
    if (email === this.mockUser.email && password === this.mockUser.password) {
      localStorage.setItem('user', JSON.stringify(this.mockUser));
      return true;
    }
    return false;
  }

  logout() {
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('user');
  }

  getUser(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
}
