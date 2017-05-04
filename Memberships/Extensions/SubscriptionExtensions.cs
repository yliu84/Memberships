﻿using Memberships.Entities;
using Memberships.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Memberships.Extensions
{
    public static class SubscriptionExtensions
    {
        public static async Task<int> GetSubscriptionIdByResitrationCode(this IDbSet<Subscription> subscription, string code)
        {
            try
            {
                if(subscription == null || code == null || code.Equals(string.Empty))
                {
                    return Int32.MinValue;
                }

                var subscriptionId = await (from s in subscription
                                            where s.RegistrationCode.Equals(code)
                                            select s.Id).FirstOrDefaultAsync();

                return subscriptionId;
            }
            catch
            {
                return Int32.MinValue;
            }
        }
    
        public static async Task Register(this IDbSet<UserSubscription> userSubscription, int subscriptionId, string userId)
        {
            try
            {
                if (userSubscription == null || subscriptionId.Equals(Int32.MinValue) || userId.Equals(string.Empty))
                    return;

                var exist = await Task.Run<int>(
                    () => userSubscription.CountAsync(s => s.SubscriptionId.Equals(subscriptionId) &&
                        s.UserId.Equals(userId))) > 0;

                if (!exist)
                    await Task<UserSubscription>.Run(() => userSubscription.Add(
                        new UserSubscription
                        {
                            UserId = userId,
                            SubscriptionId = subscriptionId,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.MaxValue
                        }));
            }
            catch
            {

            }
            


        }
    
        public static async Task<bool> RegisterUserSubscriptionCode(string userId, string code)
        {
            try
            {
                var db = ApplicationDbContext.Create();
                var id = await db.Subscriptions.GetSubscriptionIdByResitrationCode(code);

                if (id <= 0)
                    return false;

                await db.UserSubscriptions.Register(id, userId);

                if (db.ChangeTracker.HasChanges())
                    await db.SaveChangesAsync();

                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}