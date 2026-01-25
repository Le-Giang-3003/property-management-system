using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IPaymentDisputeRepository
    {
        Task<PaymentDispute?> GetByIdAsync(int id);
        Task<IEnumerable<PaymentDispute>> GetAllAsync();
        Task<IEnumerable<PaymentDispute>> GetByRaisedByAsync(int userId);
        Task AddAsync(PaymentDispute dispute);
        Task UpdateAsync(PaymentDispute dispute);
    }
}
