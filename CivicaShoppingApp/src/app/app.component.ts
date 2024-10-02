import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { ActivatedRoute, Route, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'CivicaShoppingApp';
  isAuthenticated :boolean = false;
  username: string | null | undefined;
  userId : number | undefined;

  constructor(
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges(); // Manually trigger change detection.
    });
    this.authService.getUsername().subscribe((username: string | null | undefined) => {
      this.username = username;
      this.cdr.detectChanges(); // Manually trigger change detection.
    });
    this.authService.getUserId().subscribe((userId: string | null | undefined) => {
      this.userId = Number(userId);
    });
  }
  signOut() {
    this.authService.signOut();
    this.router.navigate(['/home']);
  }
}
