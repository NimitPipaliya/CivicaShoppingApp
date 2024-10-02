import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { UsersListComponent } from './components/users-list/users-list.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { ProductListComponent } from './components/products/product-list/product-list.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { AddProductComponent } from './components/products/add-product/add-product.component';
import { UpdateProductComponent } from './components/products/update-product/update-product.component';
import { ChangepasswordComponent } from './components/auth/changepassword/changepassword.component';
import { ForgetpasswordComponent } from './components/auth/forgetpassword/forgetpassword.component';
import { OrderSummaryComponent } from './components/order/order-summary/order-summary.component';
import { UserOrderReportComponent } from './components/reports/user-order-report/user-order-report.component';
import { GetQuantityOfProductsComponent } from './components/reports/get-quantity-of-products/get-quantity-of-products.component';
import { BuyProductsComponent } from './components/cart/buy-products/buy-products.component';
import { ShoppingCartComponent } from './components/cart/shopping-cart/shopping-cart.component';
import { ProductSalesComponent } from './components/reports/product-sales/product-sales.component';
import { OrderSummaryReportComponent } from './components/order/order-summary-report/order-summary-report.component';
import { OrderHistoryComponent } from './components/order/order-history/order-history.component';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

const routes: Routes = [
  {path:'',redirectTo:'home',pathMatch:'full'},
  {path:'home',component:HomeComponent},
  { path: 'privacy', component: PrivacyComponent },
  { path: 'signin', component: SigninComponent },
    { path: 'users', component: UsersListComponent,canActivate:[adminGuard] },
    { path: 'signup', component: SignupComponent },
    {path:'products',component:ProductListComponent,canActivate:[adminGuard]},
    {path:'products/product-detail/:id',component:ProductDetailsComponent,canActivate:[adminGuard]},
    {path:'products/add-product',component:AddProductComponent,canActivate:[adminGuard]},
    {path:'products/update-product/:id',component:UpdateProductComponent,canActivate:[adminGuard]},
    {path: 'forgetpassword', component:ForgetpasswordComponent},
    {path: 'changepassword', component:ChangepasswordComponent,canActivate:[authGuard]},
    //User order Report
    {path: 'userOrderReport/:id', component:UserOrderReportComponent, canActivate:[adminGuard]},
    {path: 'order-summary/:orderNumber', component:OrderSummaryComponent,canActivate:[authGuard]},
    {path: 'getquantityofproducts', component:GetQuantityOfProductsComponent,canActivate:[adminGuard]},
    {path: 'buyProducts', component: BuyProductsComponent},
    {path:'cart',component: ShoppingCartComponent,canActivate:[authGuard]},
    {path:'productSales',component: ProductSalesComponent,canActivate:[adminGuard]},
    {path: 'order-summary-report/:orderNumber', component:OrderSummaryReportComponent,canActivate:[authGuard]},
    {path: 'order-history', component:OrderHistoryComponent,canActivate:[authGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
