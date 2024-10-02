import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { ForgetPasswordModel } from 'src/app/models/forgetpassword.model';
import { SecurityQuestion } from 'src/app/models/security-question.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-forgetpassword',
  templateUrl: './forgetpassword.component.html',
  styleUrls: ['./forgetpassword.component.css']
})
export class ForgetpasswordComponent implements OnInit{
  forgetPassword : ForgetPasswordModel = {
    loginId: '',
    securityQuestionId: 0,
    answer: '',
    newPassword: '',
    confirmNewPassword: ''
  }
  username: string | null | undefined;
  questions: SecurityQuestion[] | undefined;
  loading : boolean = false;
  constructor(
    private authService: AuthService,
    private router : Router,
  ){}
  ngOnInit(): void {
    this.loadQuestions();
  }
  loadQuestions(): void{
    this.loading = true;
    this.authService.getAllQuestions().subscribe({
      next:(response: ApiResponse<SecurityQuestion[]>) =>{
        if(response.success){
          this.questions = response.data;
        }
        else{
          console.error('Failed to fetch security questions ', response.message);
        }
        this.loading = false;
      },error:(error)=>{
        console.error('Error fetching security questions : ',error);
        this.loading = false;
      }
    });
  };
  checkPasswords(form: NgForm):void {
    const password = form.controls['newPassword'];
    const confirmPassword = form.controls['confirmNewPassword'];
 
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword.setErrors(null);
    }
  };

  onSubmit(forgetPasswordForm: NgForm): void {
    if(forgetPasswordForm.valid){
      this.authService.forgetPassword(this.forgetPassword).subscribe({
        next:(response) => {
          if(response.success){
            alert(response.message)
            this.router.navigate(['/signin'])
          }
          else {
            alert(response.message);
          }
        },
        error:(err) => {
          alert(err.error.message);
        }
      });
    }
  }
}
