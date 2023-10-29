using TShockAPI.Configuration;

namespace TShock_Gatekeeper
{
    public class GatekeeperConfig
    {
        //Enable this if you want this plugin to run
        public bool Enabled = true;

        public string panicModeKickMessage = "Kicked : Kicked by panic mode.";

        //Enable this if you want only staff members with the permission can join your server
        public bool MaintenanceMode = false;
        public string staffGroupsPermission = "gatekeeper.staff";
        public string MMKickMessage = "Forbidden : The server is under maintenance.";

        //Enable this if you want only registered users can join your server
        public bool AllowRegisteredOnly = false;
        public string AROKickMessage = "Forbidden : Only registered accounts are allowed to join the server.";

        //Enable this if you want only certain groups with permission can join your server
        public bool AllowOnlyCertainGroups = false;
        public string allowedGroupsPermission = "gatekeeper.group";
        public string AOCGKickMessage = "Forbidden : Only certain permitted groups are allowed to join the server.";

        //Enable this if you want only certain users can join your server
        public bool AllowOnlyCertainUsers = false;
        public int[] allowedUsersIds = { 1, 2, 3 };
        public string AOCUKickMessage = "Forbidden : Only certain permitted users are allowed to join the server.";

        //Enable this if you want to limit the number of connection per IP to your server
        //For example when you don't want too many certain players alt account joined at the same time
        public bool AllowMultipleConnectionPerIP = false;
        public int MaximumNumberofConnectionPerIP = 2;
        public string AMCPIMessage = $"Forbidden : High concentration of players with identical IP addresses detected.";

        //Add a fun Custom Join Message
        //{ userid, "custom message"}
        //Wildcard, [playername]
        public bool EnableCustomUserJoinMessage = false;
        public Dictionary<int, string> customMessage = new Dictionary<int, string>{
            { 1111, "A very cool looking player joined the world!"},
            { 2222, "[playername] from the [playergroup] guild descended to this filthy mortal world."},
        };

    }
    public class GKConfig : ConfigFile<GatekeeperConfig>
    {
    }
}