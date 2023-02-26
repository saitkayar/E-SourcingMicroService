﻿using Esourcing.Sourcing.Entities;
using Esourcing.Sourcing.Settings.Interfaces;
using MongoDB.Driver;

namespace Esourcing.Sourcing.Data
{
    public class SourcingContext : ISourcingContext
    {
        public SourcingContext(ISourcingDatabaseSettings settings)
        {
            var client= new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DataBaseName);
            Auctions=database.GetCollection<Auction>(nameof(Auction));
            Bids = database.GetCollection<Bid>(nameof(Bid));

            SourcingContextSeed.SeedData(Auctions);
        }
        public IMongoCollection<Auction> Auctions { get; }

        public IMongoCollection<Bid> Bids { get; }
    }
}
