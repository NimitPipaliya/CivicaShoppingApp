import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-update-product',
  templateUrl: './update-product.component.html',
  styleUrls: ['./update-product.component.css']
})
export class UpdateProductComponent {

  loading: boolean = false;
  productForm!: FormGroup;
  productId !: number;
  
    constructor(
    private productService: ProductService, 
    private fb: FormBuilder,
    private router : Router,
    private route : ActivatedRoute
  ){ }

  ngOnInit(): void {
    this.route.params.subscribe((params) =>{
      this.productId = params['id'];
      this.loadProductData(this.productId);
    });

    this.productForm = this.fb.group({
      productId : [0,Validators.required],
      productName: ['',[Validators.required]],
      productDescription: ['',[Validators.required]],
      quantity: [0,[Validators.required,Validators.min(0)]],
      productPrice: [0,[Validators.required,Validators.min(0)]]
    });
  };

  get formControls() {
    return this.productForm.controls;
  }

  loadProductData(id : number)
  {
    this.loading = true;

    this.productService.getProductById(id).subscribe({
      next:(response) =>{
        if(response.success){
          this.productForm.patchValue({
            productId : response.data.productId,
            productName : response.data.productName,
            productDescription : response.data.productDescription,
            quantity: response.data.quantity,
            productPrice : response.data.productPrice
          });

        }
        else{
          console.error('Failed to fetch product',response.message);
        }
        this.loading = false;

      },
      error:(err) =>{
        alert(err.error.message);
        this.loading = false;

      },
      complete:()=>{
        console.log("completed");
      }
    })

  }
  onSubmit(){
    this.loading = true;

    console.log(this.productForm.value);
    if(this.productForm.valid){
      this.productService.ModifyProduct(this.productForm.value).subscribe({
        next:(response) =>{
          if(response.success){
            this.router.navigate(['/products'])
          }
          else{
            alert(response.message);
          }
          this.loading = false;

        },
        error:(err) =>{
          console.error('Failed to add product: ',err.error.message);
          alert(err.error.message);
          this.loading = false;

        },
        complete:() =>{
          console.log('completed');
        }
      });
    }
  }
}
