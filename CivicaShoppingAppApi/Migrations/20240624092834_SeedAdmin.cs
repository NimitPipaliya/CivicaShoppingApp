using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace CivicaShoppingAppApi.Migrations
{
    public partial class SeedAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            byte[] passwordSalt;
            byte[] passwordHash;

            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Admin@123"));
            }

            migrationBuilder.InsertData(
              table: "Users",
              columns: new[] { "Salutation", "Name", "BirthDate", "Age", "LoginId", "Gender", "Email", "Phone", "PasswordHash", "PasswordSalt", "SecurityQuestionId", "Answer", "IsAdmin" },
              values: new object[]
              {
                    "Mr.",
                    "Admin",
                    "2002-04-23 00:00:00.0000000",
                    22,
                    "admin",
                    "M",
                    "admin@gmail.com",
                    "9999999999",
                    passwordHash,
                    passwordSalt,
                    1,
                    "Black",
                    true
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "Users",
               keyColumn: "UserId",
               keyValues: new object[]
               {
                    1
               });
        }
    }
}
