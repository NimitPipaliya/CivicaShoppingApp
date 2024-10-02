import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent {
  loading: boolean = false;
  productForm!: FormGroup;
  
    constructor(
    private productService: ProductService, 
    private fb: FormBuilder,
    private router : Router
  ){ }

  ngOnInit(): void {
    this.productForm = this.fb.group({
      productName: ['',[Validators.required]],
      productDescription: ['',[Validators.required]],
      quantity: [0,[Validators.required,Validators.min(0)]],
      productPrice: [0,[Validators.required,Validators.min(0)]]
    });

  };

  get formControls() {
    return this.productForm.controls;
  }

  onSubmit(){
    console.log(this.productForm.value);
    if(this.productForm.valid){
      this.productService.addProduct(this.productForm.value).subscribe({
        next:(response) =>{
          if(response.success){
            this.router.navigate(['/products'])
          }
          else{
            alert(response.message);
          }
        },
        error:(err) =>{
          console.error('Failed to add product',err.error.message);
          alert(err.error.message);
        },
        complete:() =>{
          console.log('completed');
        }
      });
    }
  }
}
