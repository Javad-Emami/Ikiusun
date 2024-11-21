using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DatabaseFacade datbase { get; }

    DbSet<Attachment> Attachments { get; set; }

    DbSet<Conversation> Conversations { get; set; }

    DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }

    DbSet<Deposite> Deposites { get; set; }

    DbSet<Message> Messages { get; set; }

    DbSet<Otp> Otps { get; set; }

    DbSet<Pricing> Pricings { get; set; }

    DbSet<Role> Roles { get; set; }

    DbSet<SenderType> SenderTypes { get; set; }

    DbSet<ServiceModel> ServiceModels { get; set; }

    DbSet<User> Users { get; set; }

    DbSet<UserRequest> UserRequests { get; set; }

    DbSet<Wallet> Wallets { get; set; }

    DbSet<Withdrawal> Withdrawals { get; set; }
}
