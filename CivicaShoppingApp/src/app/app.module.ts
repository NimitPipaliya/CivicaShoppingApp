import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { UsersListComponent } from './components/users-list/users-list.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { AuthService } from './services/auth.service';
import { SignupComponent } from './components/auth/signup/signup.component';
import { ProductListComponent } from './components/products/product-list/product-list.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { AddProductComponent } from './components/products/add-product/add-product.component';
import { UpdateProductComponent } from './components/products/update-product/update-product.component';
import { ChangepasswordComponent } from './components/auth/changepassword/changepassword.component';
import { ForgetpasswordComponent } from './components/auth/forgetpassword/forgetpassword.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OrderSummaryComponent } from './components/order/order-summary/order-summary.component';
import { UserOrderReportComponent } from './components/reports/user-order-report/user-order-report.component';
import { GetQuantityOfProductsComponent } from './components/reports/get-quantity-of-products/get-quantity-of-products.component';
import { BuyProductsComponent } from './components/cart/buy-products/buy-products.component';
import { ShoppingCartComponent } from './components/cart/shopping-cart/shopping-cart.component';
import { ProductSalesComponent } from './components/reports/product-sales/product-sales.component';
import { OrderSummaryReportComponent } from './components/order/order-summary-report/order-summary-report.component';
import { OrderHistoryComponent } from './components/order/order-history/order-history.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PrivacyComponent,
    SigninComponent,
    UsersListComponent,
    SignupComponent,
    ProductListComponent,
    ProductDetailsComponent,
    AddProductComponent,
    UpdateProductComponent,
    ChangepasswordComponent,
    ForgetpasswordComponent,
    UserOrderReportComponent,
    OrderSummaryComponent,
    GetQuantityOfProductsComponent,
    BuyProductsComponent,
    ShoppingCartComponent,
    ProductSalesComponent,
    ShoppingCartComponent,
    OrderSummaryReportComponent,
    OrderHistoryComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule
  ],
  providers: [
    AuthService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
