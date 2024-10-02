using CivicaShoppingAppApi.Data.Implementation;
using CivicaShoppingAppApi.Data;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AutoFixture;
using System.Diagnostics.Metrics;
using System.Threading;

namespace CivicaShoppingAppApiTests.Repository
{
    public class ApiAuthRepositoryTests
    {
        // UpdateUser
        [Fact]
        public void UpdateUser_ReturnsTrue()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Name = "TestName",
                LoginId = "loginId",
                Email = "xyz@gmail.com",
                Phone = "9090909090"
            };
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new AuthRepository(mockAppDbContext.Object);

            // Act
            var actual = target.UpdateUser(user);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(p => p.Update(user), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateUser_ReturnsFalse()
        {
            // Arrange
            var loginId = "non_existing_login_id";
            var email = "non_existing_email@example.com";
            var usersData = new List<User>
            { }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            var target = new AuthRepository(mockAppDbContext.Object);
            User user = null;

            //var target = new AuthRepository(mockDbContext.Object);

            // Act
            var actual = target.UpdateUser(user);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void RegisterUser_ReturnsTrue_WhenUserIsNotNull()
        {
            // Arrange
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<User>>();
            var user = new User
            {
                Name = "TestName",
                LoginId = "loginId",
                Email = "xyz@gmail.com",
                Phone = "9090909090"
            };
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.RegisterUser(user);

            // Assert
            Assert.True(result); // Expecting user registration to succeed
            mockDbSet.Verify(m => m.Add(user), Times.Once);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void RegisterUser_ReturnsFalse_WhenUserIsNull()
        {
            // Arrange
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<User>>();
            User user = null;
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.RegisterUser(user);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.Add(It.IsAny<User>()), Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void UserExists_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            var loginId = "existing_login_id";
            var email = "existing_email@example.com";
            var usersData = new List<User>
        {
            new User { LoginId = loginId, Email = email },

        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.UserExists(loginId, email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UserExists_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var loginId = "non_existing_login_id";
            var email = "non_existing_email@example.com";
            var usersData = new List<User>
            { }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.UserExists(loginId, email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAllSecurityQuestions_ReturnsQuestions_WhenQuestionsExist()
        {
            // Arrange
            var questions = new List<SecurityQuestion>
        {
            new SecurityQuestion
            {
                   SecurityQuestionId = 1,
                   Question = "Question"
            },
            new SecurityQuestion {
                    SecurityQuestionId = 2,
                    Question = "Questions"
            }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<SecurityQuestion>>();
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.Provider).Returns(questions.Provider);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.Expression).Returns(questions.Expression);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.ElementType).Returns(questions.ElementType);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.GetEnumerator()).Returns(questions.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.SecurityQuestions).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var actual = target.GetAllSecurityQuestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(questions.Count(), actual.Count());
            mockDbContext.Verify(c => c.SecurityQuestions, Times.Once);
        }

        [Fact]
        public void GetAllUsers_ReturnsCorrectUsers_WhenUsersExists_SearchIsNull()
        {
            string sortOrder = "asc";
            var users = new List<User>
              {
                  new User{UserId=1, Name="User 1"},
                  new User{UserId=2, Name="User 2"},
                  new User{UserId=3, Name="User 3"},

              }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAppDbContext.Object);
            //Act
            var actual = target.GetAllUsers(1, 2, null, sortOrder);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
        }
           [Fact]
        public void GetAllUsers_ReturnsCorrectUsers_WhenUsersExists_SearchIsNotNull()
        {
            string sortOrder = "desc";
            var users = new List<User>
              {
                  new User{UserId=1, Name="User 1", LoginId="xyz"},
                  new User{UserId=2, Name="User 2", LoginId="xyz"},
                  new User{UserId=3, Name="User 3", LoginId="xyz"},

              }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAppDbContext.Object);
            //Act
            var actual = target.GetAllUsers(1, 2, "User", sortOrder);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
        } 
        
        [Fact]
        public void GetAllUsers_ReturnsCorrectUsers_WhenUsersExists_SearchIsNotNull_defaultOrder()
        {
            string sortOrder = "drfgh";
            var users = new List<User>
              {
                  new User{UserId=1, Name="User 1", LoginId="xyz"},
                  new User{UserId=2, Name="User 2", LoginId="xyz"},
                  new User{UserId=3, Name="User 3", LoginId="xyz"},

              }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAppDbContext.Object);
            //Act
            var actual = target.GetAllUsers(1, 2, "User", sortOrder);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
        }

        [Fact]
        public void TotalUsers_ReturnsCount_WhenUsersExistWhenSearchIsNotNull()
        {
            string search = "user";
            var users = new List<User> {
                new User {UserId = 1,Name="User 1", LoginId="user"},
                new User {UserId = 2,Name="User 2", LoginId="user"}
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalUsers(search);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(users.Count(), actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Users, Times.Once);

        }

        [Fact]
        public void GetUser_WhenUserIsNull()
        {
            //Arrange
            var id = 1;
            var users = new List<User>().AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockAbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAbContext.Object);
            //Act
            var actual = target.GetUser(id);
            //Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.Users, Times.Once);

        }

        [Fact]
        public void GetUser_WhenUserIsNotNull()
        {
            //Arrange
            var id = 1;
            var contacts = new List<User>()
            {
              new User { UserId = 1, Name = "User 1" },
                new User { UserId = 2, Name = "User 2" },
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(contacts.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(contacts.Expression);
            mockAbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAbContext.Object);
            //Act
            var actual = target.GetUser(id);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.Users, Times.Once);

        }

        [Fact]
        public void DeleteUser_ReturnsTrue()
        {
            // Arrange
            var contactId = 1;
            var contact = new User { UserId = contactId };
            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.Users.Find(contactId)).Returns(contact);
            var target = new AuthRepository(mockContext.Object);
            // Act
            var result = target.DeleteUser(contactId);

            // Assert
            Assert.True(result);
            mockContext.Verify(c => c.Users.Remove(contact), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            mockContext.Verify(c => c.Users.Find(contactId), Times.Once);

        }

        [Fact]
        public void DeleteUser_ReturnsFalse()
        {
            //Arrange
            var id = 1;
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new AuthRepository(mockAbContext.Object);

            //Act
            var actual = target.DeleteUser(id);
            //Assert
            Assert.False(actual);
            mockAbContext.VerifyGet(c => c.Users, Times.Once);
        }

        [Fact]
        public void ValidateUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var username = "existing_username";
            var userData = new List<User>
        {
            new User { LoginId = username },
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.ValidateUser(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.LoginId);
        }

        [Fact]
        public void ValidateUser_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var username = "non_existing_username";
            var userData = new List<User>
            { }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.ValidateUser(username);

            // Assert
            Assert.Null(result);
        }






    }
}
