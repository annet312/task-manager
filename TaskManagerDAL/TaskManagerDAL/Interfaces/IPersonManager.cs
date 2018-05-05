using System;
using TaskManagerDAL.Entities;

namespace TaskManagerDAL.Interfaces
{
    public interface IPersonManager : IDisposable
    {
        void Create(Person item, string TeamName);
    }
}
