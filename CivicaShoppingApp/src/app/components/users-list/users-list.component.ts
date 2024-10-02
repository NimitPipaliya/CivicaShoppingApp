import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {
  users: User[] | undefined;
  username:string |null|undefined;
  totalUsers!: number;
  pageSize = 2;
  currentPage = 1;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  totalPages: number[] = [];
  sortOrder: string = 'asc';
  searchQuery: string = '';

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef,private route : Router) { }
   
  ngOnInit() {
    this.loadUsers();
    this.totalUserCount();
    this.authService.isAuthenticated().subscribe((authState:boolean)=>{
      this.isAuthenticated=authState;
      this.cdr.detectChanges();
     });
    //  this.authService.getUsername().subscribe((username:string |null|undefined)=>{
    //   this.username=username;
    //   this.cdr.detectChanges();
    //  });
  }

  totalUserCount(search?: string) {
    this.authService.fetchUserCount(search)
    .subscribe({
      next: (response: ApiResponse<number>) => {
        if(response.success)
          {
            this.totalUsers = response.data;
            console.log(this.totalUsers);
            this.calculateTotalPages();

          }
          else{
            console.error('Failed to fetch users', response.message);
          }
      },
      error:(error => {
        console.error('Failed to fetch users', error);
        this.loading = false;
      })
    });
  }

  loadUsers(search?: string) {
    this.loading = true;
    this.authService.getAllUsers(this.currentPage, this.pageSize, this.sortOrder,search)
      .subscribe({
        next:(response: ApiResponse<User[]>) => {
          if(response.success){
            this.users = response.data;
            console.log(response.data);
          }
          else{
            console.error('Failed to fetch users', response.message);
          }
          this.loading = false;

        },
        error:(error => {
          console.error('Failed to fetch users', error);
          this.loading = false;
        })
      });
  }

  calculateTotalPages() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalUsers / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.loadUsers(this.searchQuery);
  }

  onPageSizeChange() {
    this.currentPage = 1; // Reset to first page when page size changes
    this.loadUsers(this.searchQuery);
    this.totalUserCount(this.searchQuery);
  }

  onShowAll() {
    this.currentPage = 1;
    this.totalUserCount(this.searchQuery);
    this.loadUsers(this.searchQuery);
  }

  deleteUser(userId: number) {
    if (confirm('Are you sure you want to delete this user?')) {
      this.authService.deleteUser(userId).subscribe(() => {
        this.loadUsers(); 
        this.totalUserCount(); 
        this.calculateTotalPages();

      // Check if the current page should be adjusted
      if (this.currentPage > 1 && this.users && this.users.length === 1) {

        this.onPageChange(this.currentPage - 1);
      }
      });
    }
  }

  // {
  //   this.sortOrder = 'asc'
  //   this.currentPage = 1;
  //   this.totalUserCount(this.searchQuery);
  //   this.loadUsers(this.searchQuery);
  // }

  // sortDesc()
  // {
  //   this.sortOrder = 'desc'
  //   this.currentPage = 1;
  //   this.totalUserCount(this.searchQuery);
  //   this.loadUsers(this.searchQuery);
  // }

  onClickSort(): void{
    this.loading = true;
    if(this.sortOrder == 'asc'){
      this.sortOrder = 'desc';
    }
    else if(this.sortOrder == 'desc'){
      this.sortOrder = 'asc';
    }
    this.currentPage = 1;
    this.totalUserCount(this.searchQuery);
    this.loadUsers(this.searchQuery);
  }

  searchUsers() {
    if(this.searchQuery && this.searchQuery.length>2)
 {   this.currentPage = 1;
    this.loadUsers(this.searchQuery);
    this.totalUserCount(this.searchQuery);
 }
 else
 {
  this.currentPage = 1;
    this.loadUsers();
    this.totalUserCount();
 }
  }

  clearSearch() {
    this.currentPage = 1;
    this.searchQuery = '';
    this.loadUsers(this.searchQuery);
    this.totalUserCount(this.searchQuery);
  }


}
