import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, NgForm, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}.model';
import { SecurityQuestion } from 'src/app/models/security-question.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  loading : boolean = false;
  signUpForm !: FormGroup;
  questions !: SecurityQuestion[] ;
  constructor(private formBuilder : FormBuilder,private authService : AuthService,  private router : Router){}
  ngOnInit(): void {
    this.signUpForm = this.formBuilder.group({
      salutation : [''],
      name: ['', [Validators.required, Validators.minLength(2)]],
      loginId: ['', [Validators.required, Validators.minLength(2)]],
      phone: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(12)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&].{8,}$/)]],
      confirmPassword: ['', [Validators.required]],
      birthDate: ['', [Validators.required, this.validateBirthdate]],
      gender:['',Validators.required],
      securityQuestionId : [,[Validators.required,this.questionValidator]],
      answer: ['', [Validators.required]],
    }, {
      validator: this.passwordMatchValidator // Custom validator for password match
    });
    this.loadQuestions();
  }

  get formControl(){
    return this.signUpForm.controls;
   }


  questionValidator(control:any){
    return control.value=='' ? {invalidQuestion:true}: null
  }
  passwordMatchValidator(form: FormGroup): { [key: string]: any } | null {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
  
    const passwordsMatch = password === confirmPassword;
    console.log('Passwords Match:', passwordsMatch);
  
    return passwordsMatch ? null : { passwordMismatch: true };
  }

  validateBirthdate(control: AbstractControl): ValidationErrors | null {
    const selectedDate = new Date(control.value);
    const currentDate = new Date();
  
    // Set hours, minutes, seconds, and milliseconds to 0 to compare only the date part
    selectedDate.setHours(0, 0, 0, 0);
    currentDate.setHours(0, 0, 0, 0);
  
    if (selectedDate > currentDate) {
      return { invalidBirthDate: true };
    }
    return null;
   }

  loadQuestions(): void{
    this.loading = true;
    this.authService.getAllQuestions().subscribe({
      next:(response: ApiResponse<SecurityQuestion[]>) =>{
        if(response.success){
          this.questions = response.data;
        }
        else{
          console.error('Failed to fetch questions ', response.message);
        }
        this.loading = false;
      },error:(error)=>{
        console.error('Error fetching questions : ',error);
        this.loading = false;
      }
    });
  };

  onSubmit(): void {
    if (this.signUpForm.valid) {
      console.log(this.signUpForm.value);

      this.authService.signUp(this.signUpForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            alert('User registered successfully!!');
            this.router.navigate(['/home']);
          } else {
            alert(response.message);
          }
        },
        error: (err) => {
          alert(err.error.message);
        }
      });
    }
  }

}
