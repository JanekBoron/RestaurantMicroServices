using Restaurant.Services.Email.Models;
using Restaurant.Services.Email.Messages;

namespace Restaurant.Services.Email.Repository
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
