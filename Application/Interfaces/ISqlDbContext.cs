﻿using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces;

public interface ISqlDbContext
{
    DatabaseFacade datbase { get; }

    DbSet<Attachment> Attachments { get; set; }

    DbSet<Conversation> Conversations { get; set; }

    DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }

    DbSet<Message> Messages { get; set; }

    DbSet<Pricing> Pricings { get; set; }

    DbSet<Role> Roles { get; set; }

   // DbSet<SenderType> SenderTypes { get; set; }

   // DbSet<ServiceModel> ServiceModels { get; set; }

    DbSet<User> Users { get; set; }

    DbSet<UserRequest> UserRequests { get; set; }

    DbSet<WalletTransaction> WalletTransactions { get; set; }
}
