import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  email = '';
  password = '';
  userType: 'candidate' | 'company' = 'candidate';

  constructor(private router: Router) {}

  signup() {
    alert('Signup temporar: doar test, nu se salvează');
    this.router.navigate(['/login']);
  }
}
