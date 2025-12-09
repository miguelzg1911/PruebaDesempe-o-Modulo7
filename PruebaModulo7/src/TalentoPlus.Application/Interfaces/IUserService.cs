using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User> Create(User student);
    Task<bool> Update(int id, User student);
    Task<bool> Delete(int id);
}