using Clients.DTOs;
using Clients.DTOs.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clients.Interfaces
{
    public interface IUserClient
    {
        Task<BaseClientResult<UserInfoDto>> GetUserAsync(int externalUserId);
    }
}
