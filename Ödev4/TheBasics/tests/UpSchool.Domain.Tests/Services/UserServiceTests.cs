using System.Linq.Expressions;
using FakeItEasy;
using UpSchool.Domain.Data;
using UpSchool.Domain.Entities;
using UpSchool.Domain.Services;

namespace UpSchool.Domain.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetUser_ShouldGetUserWithCorrectId()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();

            Guid userId = new Guid("8f319b0a-2428-4e9f-b7c6-ecf78acf00f9");

            var cancellationSource = new CancellationTokenSource();

            var expectedUser = new User()
            {
                Id = userId
            };

            A.CallTo(() =>  userRepositoryMock.GetByIdAsync(userId, cancellationSource.Token))
                .Returns(Task.FromResult(expectedUser));

            IUserService userService = new UserManager(userRepositoryMock);

            var user = await userService.GetByIdAsync(userId, cancellationSource.Token);

            Assert.Equal(expectedUser, user);
        }
        
        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenEmailIsEmptyOrNull()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();

            var cancellationSource = new CancellationTokenSource();

            var userFirst = new User
            {
                Id=Guid.NewGuid(),
                FirstName = "Çiğdem",
                LastName = "Çakır",
                Age = 25,
                Email = string.Empty,
            };
            var userSecond = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Çiğdem",
                LastName = "Çakır",
                Age = 25,
                Email = null,
            };

            IUserService userService = new UserManager(userRepositoryMock);

            await Assert.ThrowsAsync<ArgumentException>(()=> userService.AddAsync
                (userFirst.FirstName, userFirst.LastName, userFirst.Age, userFirst.Email,cancellationSource.Token));
            
            await Assert.ThrowsAsync<ArgumentException>(()=> userService.AddAsync
                (userSecond.FirstName, userSecond.LastName, userSecond.Age, userSecond.Email,cancellationSource.Token));
        }
        
        [Fact]
        public async Task AddAsync_ShouldReturn_CorrectUserId()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();

            var cancellationSource = new CancellationTokenSource();

            Guid userId = new Guid("8f319b0a-2428-4e9f-b7c6-ecf78acf00f9");
            
            var expectedUser = new User()
            {
                Id = userId
            };
            
            A.CallTo(() =>  userRepositoryMock.AddAsync(expectedUser, cancellationSource.Token))
                .Returns(Task.FromResult(1));

            IUserService userService = new UserManager(userRepositoryMock);

            var userGuid = await userService.AddAsync("Çiğdem", "Çakır", 25, "cigdem@gmail.com", cancellationSource.Token);

            Assert.NotEqual(Guid.Empty, userGuid);
            
            Assert.NotNull(userGuid);
        }
        
        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var userRepositoryMock = A.Fake<IUserRepository>();

            var cancellationSource = new CancellationTokenSource();

            Guid userId = Guid.NewGuid();

            A.CallTo(() => userRepositoryMock.DeleteAsync(A<Expression<Func<User, bool>>>.Ignored, cancellationSource.Token))
                .Returns(Task.FromResult(1));

            IUserService userService = new UserManager(userRepositoryMock);

            // Act
            var result = await userService.DeleteAsync(userId, cancellationSource.Token);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenUserDoesntExists()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();

            var cancellationSource = new CancellationTokenSource();

            IUserService userService = new UserManager(userRepositoryMock);

            await Assert.ThrowsAsync<ArgumentException>(() => userService.DeleteAsync(Guid.Empty, cancellationSource.Token));;
        }
        
        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenUserIdIsEmpty()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();
        
            var cancellationSource = new CancellationTokenSource();
        
            IUserService userService = new UserManager(userRepositoryMock);
            
            var user = new User()
            {
                Id = Guid.Empty
            };

            await Assert.ThrowsAsync<ArgumentException>(() => userService.UpdateAsync(user, cancellationSource.Token));
        }
        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenUserEmailEmptyOrNull()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();

            var cancellationSource = new CancellationTokenSource();

            IUserService userService = new UserManager(userRepositoryMock);

            var userFirst = new User()
            {
                Id = Guid.NewGuid(),
                Email = null,
            };
            
            var userSecond = new User()
            {
                Id = Guid.NewGuid(),
                Email = String.Empty
            };
            
            await Assert.ThrowsAsync<ArgumentException>(() => userService.UpdateAsync(userFirst, cancellationSource.Token));
            
            await Assert.ThrowsAsync<ArgumentException>(() => userService.UpdateAsync(userSecond, cancellationSource.Token));
        }
        
        [Fact]
        public async Task GetAllAsync_ShouldReturn_UserListWithAtLeastTwoRecords()
        {
            var userRepositoryMock = A.Fake<IUserRepository>();

            var cancelletionSource = new CancellationTokenSource();

            List<User> userList = new List<User>
            {
                new User { Id = Guid.NewGuid() },
                new User { Id = Guid.NewGuid() },
            };

            A.CallTo(() => userRepositoryMock.GetAllAsync(cancelletionSource.Token))
                .Returns(Task.FromResult(userList));

            IUserService userService = new UserManager(userRepositoryMock);

            var result = await userService.GetAllAsync(cancelletionSource.Token);

            Assert.True(result.Count >= 2);
        }
        
    }
}
