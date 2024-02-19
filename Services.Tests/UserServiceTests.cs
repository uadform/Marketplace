using AutoFixture.AutoMoq;
using AutoFixture;
using Clients.DTOs.Results;
using Clients.DTOs;
using Clients.Interfaces;
using Contracts.Exceptions;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Services.Tests
{
    public class UserServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<IUserClient> _userClientMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userClientMock = _fixture.Freeze<Mock<IUserClient>>();
            _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
            _userService = new UserService(_userClientMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateUserIfNotExists_ExternalUserFound_ReturnsOkWithUserId()
        {
            // Arrange
            var externalUserId = _fixture.Create<int>();
            var expectedUserId = _fixture.Create<int>();
            _userClientMock.Setup(client => client.GetUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new BaseClientResult<UserInfoDto> { StatusCode = HttpStatusCode.OK, Data = _fixture.Create<UserInfoDto>() });
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedUserId);

            // Act
            var result = await _userService.CreateUserIfNotExists(externalUserId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedUserId);

            _userClientMock.Verify(client => client.GetUserAsync(externalUserId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(externalUserId), Times.Once);
        }

        [Fact]
        public async Task CreateUserIfNotExists_ExternalUserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var externalUserId = _fixture.Create<int>();
            _userClientMock.Setup(client => client.GetUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new BaseClientResult<UserInfoDto> { StatusCode = HttpStatusCode.NotFound });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _userService.CreateUserIfNotExists(externalUserId));
            exception.Message.Should().Be($"User with external ID {externalUserId} not found in external service.");

            _userClientMock.Verify(client => client.GetUserAsync(externalUserId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
