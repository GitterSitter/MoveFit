using System;
using Newtonsoft.Json;

namespace TestApp
{
    public class UserFriends
    {


        public string Id { get; set; }

        [JsonProperty(PropertyName = "userlink1")]
        public string UserLink1 { get; set; }

        [JsonProperty(PropertyName = "userlink2")]
        public string UserLink2 { get; set; }

        [JsonProperty(PropertyName = "friendrequest")]
        public bool FriendRequest { get; set; }

        [JsonProperty(PropertyName = "isaccepted")]
        public bool IsAccepted { get; set; }

        [JsonProperty(PropertyName = "isdeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted { get; set; }



    }

    public class UserFriendsWrapper : Java.Lang.Object
    {
        public UserFriendsWrapper(UserFriends item)
        {
            UserFriends = item;
        }

        public UserFriends UserFriends { get; private set; }
    }
}

