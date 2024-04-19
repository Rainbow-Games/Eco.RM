using Eco.Gameplay.Players;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;

namespace Eco.RM.PowerRework.PowerNetworks
{
    [Eco]
    public enum PowerAuthTypes
    {
        [LocDisplayName("None"), LocDescription("No authorization.")]
        None,
        [LocDisplayName("User"), LocDescription("User can use the network but not manage it.")]
        User,
        [LocDisplayName("Owner"), LocDescription("Owners can use the network and manage other users permissions.")]
        Owner,
    }
    //Holds authorization data for a power network
    [Serialized]
    public class PowerAuthData
    {
        private readonly Dictionary<User, PowerAuthTypes> Authorizations = new();
        public PowerAuthTypes GetUserAuthLevel(User user)
        {
            if (!Authorizations.TryGetValue(user, out var auth)) return PowerAuthTypes.None;
            return auth;
        }
        public string AuthorizeUser(User current, User affected, PowerAuthTypes auth)
        {
            if (!Authorizations.TryGetValue(current, out var currentAuth) || currentAuth != PowerAuthTypes.Owner) return $"You must be an owner of the grid to manage users.";
            var affectedExists = Authorizations.TryGetValue(affected, out var affectedAuth);
            if (affectedAuth == PowerAuthTypes.Owner) return $"You are not authorized to manage this user.";
            if (auth == PowerAuthTypes.None && affectedExists)
            {
                Authorizations.Remove(affected);
                return $"removed authorization from {affected.Name}";
            }
            Authorizations[affected] = auth;
            return $"authorized {affected.Name} as {auth}";
        }
        public PowerAuthData(User creator)
        {
            Authorizations[creator] = PowerAuthTypes.Owner;
        }
        
    }
}
