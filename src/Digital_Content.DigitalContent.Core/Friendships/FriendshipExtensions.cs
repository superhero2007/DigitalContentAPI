﻿using Abp;

namespace Digital_Content.DigitalContent.Friendships
{
    public static class FriendshipExtensions
    {
        public static UserIdentifier ToUserIdentifier(this Friendship friendship)
        {
            return new UserIdentifier(friendship.TenantId, friendship.UserId);
        }

        public static UserIdentifier ToFriendIdentifier(this Friendship friendship)
        {
            return new UserIdentifier(friendship.FriendTenantId, friendship.FriendUserId);
        }
    }
}