using System;
using System.Collections.Generic;
using System.Text;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        void SetUsersAddress(int userId, Address address);

        void VerifyUser(int userId);
    }
}
