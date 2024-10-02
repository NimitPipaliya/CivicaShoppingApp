import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getUserId().pipe(
    map(userId => {
      if (userId == "1") {
        return true;
      } else {
        router.navigate(['/signin']); // Redirect to a different route if not admin
        return false;
      }
    })
  );};
