using LanguageExt.Common;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Application.Users;
using LiveOrderService.Domain.Users;
using LiveOrderService.src.Application.Users;
using Moq;
using Xunit;
using Assert = Xunit.Assert;
using TheoryAttribute = Xunit.TheoryAttribute;

public class UserUnitTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CancellationToken _cancellationToken;

    public UserUnitTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _cancellationToken = CancellationToken.None;
    }

    #region CreateUserCommandHandler Tests

    [Fact]
    public async Task CreateUserCommandHandler_WithValidData_ShouldCreateUserSuccessfully()
    {
        var command = new CreateUserCommand("testuser", "password");
        var user = new User("testuser", "password");
        _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(new Result<User>(user));

        var handler = new CreateUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("testuser", result.Match(user => user.Username, _ => string.Empty));
        
        _userRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<User>(u => u.Username == "testuser" && u.VerifyPassword("password"))), 
            Times.Once);
    }

    [Fact]
    public async Task CreateUserCommandHandler_WhenRepositoryFails_ShouldReturnFailure()
    {
        var command = new CreateUserCommand("testuser", "password");
        var exception = new Exception("Database connection error");
        _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(new Result<User>(exception));

        var handler = new CreateUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal("Database connection error", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("username", "")]
    [InlineData("", "")]
    public async Task CreateUserCommandHandler_WithInvalidData_ShouldFail(string username, 
                                                                    string password)
    {
        var command = new CreateUserCommand(username, password);
        var handler = new CreateUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.False(result.IsSuccess);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region UpdateUserCommandHandler Tests

    [Fact]
    public async Task UpdateUserCommandHandler_WithValidData_ShouldUpdateUserSuccessfully()
    {
        var command = new UpdateUserCommand(1, "updateduser", "newpassword");
        var existingUser = new User("testuser", "password") { Id = 1 };
        
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Result<User>(existingUser));
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(new Result<bool>(true));

        var handler = new UpdateUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.True(result.IsSuccess);
        
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(
            It.Is<User>(u => u.Id == 1 && u.Username == "updateduser" 
                        && u.VerifyPassword("newpassword"))), 
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_WhenUserNotFound_ShouldReturnFailure()
    {
        var command = new UpdateUserCommand(1, "updateduser", "newpassword");
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Result<User>(new Exception("User not found")));

        var handler = new UpdateUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
        
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_WhenUpdateFails_ShouldReturnFailure()
    {
        var command = new UpdateUserCommand(1, "updateduser", "newpassword");
        var existingUser = new User("testuser", "password") { Id = 1 };
        
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Result<User>(existingUser));
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(new Result<bool>(new Exception("Update failed")));

        var handler = new UpdateUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal("Update failed", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
    }

    #endregion

    #region DeleteUserCommandHandler Tests

    [Fact]
    public async Task DeleteUserCommandHandler_WithValidId_ShouldDeleteUserSuccessfully()
    {
        var command = new DeleteUserCommand(1);
        _userRepositoryMock.Setup(repo => repo.DeleteAsync(1))
            .ReturnsAsync(new Result<bool>(true));

        var handler = new DeleteUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.True(result.IsSuccess);
        _userRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteUserCommandHandler_WhenUserNotFound_ShouldReturnFailure()
    {
        var command = new DeleteUserCommand(1);
        _userRepositoryMock.Setup(repo => repo.DeleteAsync(1))
            .ReturnsAsync(new Result<bool>(new Exception("User not found")));

        var handler = new DeleteUserCommandHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(command, _cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
    }

    #endregion

    #region GetUserByIdQueryHandler Tests

    [Fact]
    public async Task GetUserByIdQueryHandler_WithValidId_ShouldReturnUser()
    {
        var query = new GetUserByIdQuery(1);
        var user = new User("testuser", "password") { Id = 1 };
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Result<User>(user));

        var handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        var result = await handler.Handle(query, _cancellationToken);

        Assert.True(result.IsSuccess);
        result.Match(
            userDto => {
                Assert.Equal<uint>(1, userDto.Id);
                Assert.Equal("testuser", userDto.Username);
                return userDto;
            },
            _ => null!
        );
        
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetUserByIdQuery(1);
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Result<User>(new Exception("User not found")));

        var handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
    }

    #endregion

    #region GetUserByUsernameQueryHandler Tests

    [Fact]
    public async Task GetUserByUsernameQueryHandler_WithValidUsername_ShouldReturnUser()
    {
        // Arrange
        var query = new GetUserByUsernameQuery("testuser");
        var user = new User("testuser", "password") { Id = 1 };
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("testuser"))
            .ReturnsAsync(new Result<User>(user));

        var handler = new GetUserByUsernameQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        result.Match(
            userDto => {
                Assert.Equal<uint>(1, userDto.Id);
                Assert.Equal("testuser", userDto.Username);
                return userDto;
            },
            _ => null!
        );
        
        _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync("testuser"), Times.Once);
    }

    [Fact]
    public async Task GetUserByUsernameQueryHandler_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetUserByUsernameQuery("nonexistentuser");
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("nonexistentuser"))
            .ReturnsAsync(new Result<User>(new Exception("User not found")));

        var handler = new GetUserByUsernameQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
    }

    [Fact]
    public async Task GetUserByUsernameQueryHandler_WithEmptyUsername_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetUserByUsernameQuery("");
        var handler = new GetUserByUsernameQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region GetAllUsersQueryHandler Tests

    [Fact]
    public async Task GetAllUsersQueryHandler_WhenUsersExist_ShouldReturnAllUsers()
    {
        // Arrange
        var query = new GetAllUsersQuery();
        var users = new List<User> { 
            new User("testuser1", "password1") { Id = 1 }, 
            new User("testuser2", "password2") { Id = 2 } 
        };
        
        _userRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new Result<IEnumerable<User>>(users));

        var handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        
        result.Match(
            userDtos => {
                Assert.Equal(2, userDtos.Count());
                Assert.Contains(userDtos, dto => dto.Username == "testuser1");
                Assert.Contains(userDtos, dto => dto.Username == "testuser2");
                return userDtos;
            },
            _ => null!
        );
        
        _userRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersQueryHandler_WhenNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllUsersQuery();
        var emptyList = new List<User>();
        
        _userRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new Result<IEnumerable<User>>(emptyList));

        var handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        
        result.Match(
            userDtos => {
                Assert.Empty(userDtos);
                return userDtos;
            },
            _ => null!
        );
    }

    [Fact]
    public async Task GetAllUsersQueryHandler_WhenRepositoryFails_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetAllUsersQuery();
        _userRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new Result<IEnumerable<User>>(new Exception("Database error")));

        var handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Database error", result.Match(
            _ => string.Empty,
            ex => ex.Message
        ));
    }

    #endregion
}