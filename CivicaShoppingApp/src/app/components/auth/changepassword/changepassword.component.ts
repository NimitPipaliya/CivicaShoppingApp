import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ChangePasswordModel } from 'src/app/models/changepassword.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-changepassword',
  templateUrl: './changepassword.component.html',
  styleUrls: ['./changepassword.component.css']
})
export class ChangepasswordComponent {
  user: ChangePasswordModel ={
    loginId: '',
    oldPassword: '',
    newPassword: '',
    newConfirmPassword: ''
  }
  loading: boolean = false;

  constructor(private authService: AuthService,
    private router: Router,
  ){}

  ngOnInit(): void {
    this.authService.getUsername().subscribe((username : string|null|undefined)=>{
      this.user.loginId = username;
    });
  }

  checkPasswords(form: NgForm):void {
    const password = form.controls['newPassword'];
    const confirmPassword = form.controls['newConfirmPassword'];
 
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword.setErrors(null);
    }
  }

  onSubmit(changePasswordForm: NgForm){
    if(changePasswordForm.valid){
      this.loading = true;
      let changePassword :ChangePasswordModel = {
        loginId: this.user.loginId,
        oldPassword: changePasswordForm.controls['oldPassword'].value,
        newPassword: changePasswordForm.controls['newPassword'].value,
        newConfirmPassword: changePasswordForm.controls['newConfirmPassword'].value
      };
      this.authService.changePassword(changePassword).subscribe({
        next:(response) => {
          if(response.success){
            alert(response.message)
            this.authService.signOut();
            this.router.navigate(['/signin']);
          }
          else{
            alert(response.message);
          }
          this.loading = false;
        },
        error:(err) => {
          console.log(err.error.message);
          alert(err.error.message);
          this.loading=false;
        },
        complete:() =>{
          console.log("completed");
          this.loading = false;
        }
      });
    }
  }

}
