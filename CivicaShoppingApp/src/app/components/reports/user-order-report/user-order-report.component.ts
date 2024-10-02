import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserOrderReport } from 'src/app/models/UserOrderReport.model';
import { AuthService } from 'src/app/services/auth.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-user-order-report',
  templateUrl: './user-order-report.component.html',
  styleUrls: ['./user-order-report.component.css']
})
export class UserOrderReportComponent implements OnInit {
  loading: boolean = false;
  userId !: number;
  username!: string;
  name!: string;
  allOrders: UserOrderReport[] = [];

  flag: string = "desc";
  //pagination
  pageNumber: number = 1;
  pageSize: number = 6;
  totalItems: number = 0;
  totalPages: number = 0;



  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService,
    private authService: AuthService,
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.userId = params['id'];

      this.loadOrderReportCount(this.userId);
      this.getUserById(this.userId);
    });
  }

  loadOrderReportCount(id: number): void {
    this.loading = true;
    this.orderService.totalOrderByUser(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.totalItems = response.data;
          this.totalPages = Math.ceil(this.totalItems / this.pageSize);
          this.loadOrderReport();
        }
        else {
          console.error('Failed to fetch data ', response.message);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching data : ', error);
        this.loading = false;
      }
    })
  }

  loadOrderReport(): void {
    this.loading = true;
    this.orderService.getUserOrderReport(this.userId, this.pageNumber, this.pageSize, this.flag).subscribe({
      next: (response) => {
        if (response.success) {
          this.allOrders = response.data;
        }
        else {
          console.error('Failed to fetch data ', response.message);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching data : ', error);
        this.loading = false;
      }
    })
  }

  getUserById(userId: number) {
    this.authService.getUserById(userId).subscribe({
      next: (response) => {
        if (response.success) {
          this.username = response.data.loginId;
          this.name  = response.data.name;
        }
        else {
          console.error('Failed to fetch data ', response.message);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching data : ', error);
        this.loading = false;
      }
    })
  }

  onClickSortWithDate(): void {
    this.loading = true;
    if (this.flag == 'asc') {
      this.flag = 'desc';
    }
    else if (this.flag == 'desc') {
      this.flag = 'asc';
    }

    this.loadOrderReport();
  }


  changePage(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.loadOrderReportCount(this.userId);
  }

  changePageSize(pageSize: number): void {
    this.pageSize = pageSize;
    this.pageNumber = 1; // Reset to first page
    this.totalPages = Math.ceil(this.totalItems / this.pageSize); // Recalculate total pages
    this.loadOrderReportCount(this.userId);
  }

  goToFirstPage(): void {
    if (this.pageNumber > 1) {
      this.changePage(1);
    }
  }

  goToLastPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.changePage(this.totalPages);
    }
  }

}
