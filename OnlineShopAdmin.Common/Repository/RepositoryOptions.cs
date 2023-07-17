﻿using Microsoft.EntityFrameworkCore;

namespace OnlineShopAdmin.Common.Repository
{
    public class RepositoryOptions<TDbContext> where TDbContext : DbContext
    {
        public int RelatedPropertiesMaxDepth { get; set; } = 3;

        public SaveChangesStrategy SaveChangesStrategy { get; set; } = SaveChangesStrategy.PerUnitOfWork;
    }

    public enum SaveChangesStrategy
    {
        PerOperation,
        PerUnitOfWork
    }
}