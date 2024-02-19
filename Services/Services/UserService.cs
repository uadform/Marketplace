using Clients.DTOs.Results;
using Clients.DTOs;
using Clients.Interfaces;
using Core.Models;
using Core.Repositories;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Mvc;


namespace Services.Services
{
    public class UserService
    {
        private readonly IUserClient _userClient;
        private readonly IUserRepository _userRepository;

        public UserService(IUserClient userClient, IUserRepository userRepository)
        {
            _userClient = userClient;
            _userRepository = userRepository;
        }

        public async Task<ActionResult<int>> CreateUserIfNotExists(int externalUserId)
        {
            var userResult = await _userClient.GetUserAsync(externalUserId);
            if (userResult.StatusCode != HttpStatusCode.OK || userResult.Data == null)
            {
                throw new NotFoundException($"User with external ID {externalUserId} not found in external service.");
            }

            int userId = await _userRepository.CreateUserAsync(externalUserId);

            return new OkObjectResult(userId);
        }
    }
}
