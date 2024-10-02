import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  username: string | undefined | null;
  isAuthenticated :boolean = false;

  constructor(private authService: AuthService,private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.authService.getUsername().subscribe((username: string | null | undefined) => {
      this.username = username;
    });

    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges(); // Manually trigger change detection.
    });
  }
}
