import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderItem } from 'src/app/models/order-item.model';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-order-summary',
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.css']
})
export class OrderSummaryComponent implements OnInit {
  orderNumber!: number;
  orderItems: OrderItem[] = [];
  loading: boolean = false;

  constructor(private orderService: OrderService, private router: Router, private route: ActivatedRoute) {
    
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.orderNumber = params['orderNumber'];
    });
    this.loadOrderItems(this.orderNumber);
  }

  loadOrderItems(orderNumber: number) {
    this.loading = true;
    this.orderService.getOrderSummary(orderNumber).subscribe({
      next: (response) => {
        if(response.success) {
          console.log(response.data)
          this.orderItems = response.data;
        } else {
          console.error('Failed to fetch order summary', response.message);
        }
        this.loading= false;
      },
      error: (error) => {
        console.error('Error fetching order summary', error);
        this.orderItems = [];
        this.loading = false;
      }
    })
  }
}
