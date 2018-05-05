using IdentityDAL.Entities;
using System;
namespace IdentityDAL.Interfaces
{
    interface IUserManager : IDisposable
    {
        void Create(UserProfile item);
    }
}
