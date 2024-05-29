using Ntier.DAL.Entities;
using Ntier.DTO.DTO;
using Ntier.DTO.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsersAsync(int pageSize , int pageIndex);

        Task<User?> GetUserByIdAsync(string id);
        Task<User?> AddUserAsync( UserRegisterDTO userDTO );
        Task<User?> CheckUserAsync( UserLoginDTO userLoginDTO );

        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task RemoveRefreshTokenAsync(string userId); 

        Task<RefreshToken> GetRefreshTokenAsync(string userId);
        Task UpdateUserInformation(UserDTO userDTO);
    }
}
