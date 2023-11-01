using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TShock_Gatekeeper
{
    [ApiVersion(2, 1)]
    public class TShock_Gatekeeper : TerrariaPlugin
    {
        public override string Author => "hdseventh";
        public override string Description => "Server Gatekeeper for TShock";
        public override string Name => "Gatekeeper TShock  Plugin";

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public const string configFolder = "Gatekeeper";
        private static string ConfigPath = Path.Combine(TShock.SavePath, configFolder, "GatekeeperConfig.json");

        public static GKConfig Config { get; set; } = new GKConfig();

        public const string tag = "[Gatekeeper] ";
        public static bool panicMode = false;

        public TShock_Gatekeeper(Main game) : base(game) { }

        public override void Initialize()
        {

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoinAsync);
        }

        private static void OnInitialize(EventArgs args)
        {
            if (!File.Exists(Path.Combine(TShock.SavePath, configFolder)))
            {
                Directory.CreateDirectory(Path.Combine(TShock.SavePath, configFolder));
            }

            bool writeConfig = true;
            if (File.Exists(ConfigPath))
            {
                Config.Read(ConfigPath, out writeConfig);
            }

            if (writeConfig)
            {
                Console.WriteLine(tag + "Config file not found, creating a new one...");
                Config.Write(ConfigPath);
            }

            //Commands
            Commands.ChatCommands.Add(new Command("gatekeeper.admin.main", MainCommand, "gatekeeper", "gk")); 
        }

        private static void MainCommand(CommandArgs args)
        {
            var player = args.Player;

            if (args.Parameters.Count < 1)
            {
                player.SendInfoMessage("[Gatekeeper] Commands List:");
                player.SendInfoMessage("{0}gk panic - Temporarily enables all gates except for Maintenance mode. Use this command again to disable it.", Commands.Specifier);
                player.SendInfoMessage("{0}gk [kickAll/ka] - Kick all players except you.", Commands.Specifier);
                player.SendInfoMessage("{0}gk [kickUsers/ku] - Kick all players except you and your staffs.", Commands.Specifier);
                player.SendInfoMessage("{0}gk [kickOrdinary/ko] - Kick all players except you, your staffs, permitted users, and permitted groups.", Commands.Specifier);
                player.SendInfoMessage("{0}gk [kickUnregistered/kun] - Kick all unregistered players.", Commands.Specifier);
                player.SendInfoMessage("{0}gk status - Show Gatekeeper's status.", Commands.Specifier);
                player.SendInfoMessage("{0}gk reload - Reload Gatekeeper's config.", Commands.Specifier);
                return;
            }

            string param = args.Parameters[0].ToLower();
            switch (param)
            {
                case "panic":
                    if (panicMode)
                    {
                        panicMode = false;
                        player.SendSuccessMessage(tag + "Panic Mode has been deactivated!");
                    }
                    else
                    {
                        panicMode = true;
                        player.SendSuccessMessage(tag + "Panic Mode has been activated!");
                    }
                    player.SendSuccessMessage(tag + "Run this command again to toggle panic mode.");
                    return;

                case "kickAll":
                case "ka":
                    if (TShock.Utils.GetActivePlayerCount() < 1)
                    {
                        player.SendErrorMessage(tag + "There's no other players beside you.");
                        return;
                    };
                    for (int i = 0; i < TShock.Utils.GetActivePlayerCount(); i++)
                    {
                        if (TShock.Players[i].Name != player.Name)
                        {
                            TShock.Players[i].Disconnect(Config.Settings.panicModeKickMessage);
                            return;
                        }
                    }
                    player.SendSuccessMessage(tag + "All players has been kicked.");
                    return;

                case "kickUsers":
                case "ku":
                    if (TShock.Utils.GetActivePlayerCount() < 1)
                    {
                        player.SendErrorMessage(tag + "There's no other players beside you.");
                        return;
                    };
                    for (int i = 0; i < TShock.Utils.GetActivePlayerCount(); i++)
                    {
                        if (!TShock.Players[i].Group.HasPermission(Config.Settings.staffGroupsPermission))
                        {
                            TShock.Players[i].Disconnect(Config.Settings.panicModeKickMessage);
                            return;
                        }
                    }
                    player.SendSuccessMessage(tag + "All players except you and your staffs has been kicked.");
                    return;

                case "kickOrdinary":
                case "ko":
                    if (TShock.Utils.GetActivePlayerCount() < 1)
                    {
                        player.SendErrorMessage(tag + "There's no other players beside you.");
                        return;
                    };
                    for (int i = 0; i < TShock.Utils.GetActivePlayerCount(); i++)
                    {
                        if (!Config.Settings.allowedUsersIds.Contains(TShock.Players[i].Account.ID) &&
                            (!TShock.Players[i].Group.HasPermission(Config.Settings.staffGroupsPermission) ||
                            !TShock.Players[i].Group.HasPermission(Config.Settings.allowedGroupsPermission)))
                        {
                            TShock.Players[i].Disconnect(Config.Settings.panicModeKickMessage);
                            return;
                        }
                    }
                    player.SendSuccessMessage(tag + "All players except you, your staffs, allowed players, and allowed groups has been kicked.");
                    return;

                case "kickUnregistered":
                case "kun":
                    if(TShock.Utils.GetActivePlayerCount() < 1)
                    {
                        player.SendErrorMessage(tag + "There's no other players beside you.");
                        return;
                    };
                    for (int i = 0; i < TShock.Utils.GetActivePlayerCount(); i ++)
                    {
                        if (TShock.UserAccounts.GetUserAccountByName(TShock.Players[i].Name) == null || 
                            TShock.UserAccounts.GetUserAccountsByName(TShock.Players[i].Name).Count < 1)
                        {
                            TShock.Players[i].Disconnect(Config.Settings.panicModeKickMessage);
                            return;
                        }
                    }
                    player.SendSuccessMessage(tag + "All unregistered players has been kicked.");
                    return;

                case "status":
                    player.SendInfoMessage("[Gatekeeper] Status:");
                    player.SendInfoMessage("Gatekeeper plugin status : {0}", (Config.Settings.Enabled)?"Enabled":"Disabled");
                    player.SendInfoMessage("Maintenance Mode : {0}", (Config.Settings.MaintenanceMode) ? "Enabled" : "Disabled");
                    player.SendInfoMessage("Panic Mode : {0}", (panicMode) ? "Enabled" : "Disabled");
                    player.SendInfoMessage("Allow Registered Only : {0}", (Config.Settings.AllowRegisteredOnly) ? "Enabled" : "Disabled");
                    player.SendInfoMessage("Allow Certain Groups Only : {0}", (Config.Settings.AllowOnlyCertainGroups) ? "Enabled" : "Disabled");
                    player.SendInfoMessage("Allow Certain Users Only : {0}", (Config.Settings.AllowOnlyCertainUsers) ? "Enabled" : "Disabled");
                    player.SendInfoMessage("Allow Multiple Connection per IP: {0}", (Config.Settings.AllowMultipleConnectionPerIP) ? "Enabled" : "Disabled");
                    player.SendInfoMessage("Costum Join Message : {0}", (Config.Settings.EnableCustomUserJoinMessage) ? "Enabled" : "Disabled");
                    return;

                case "reload":
                    bool writeConfig = true;
                    if (File.Exists(ConfigPath))
                    {
                        Config.Read(ConfigPath, out writeConfig);
                    }

                    if (writeConfig)
                    {
                        Console.WriteLine(tag + "Config file not found, creating a new one...");
                        player.SendErrorMessage(tag + "Config file not found, creating a new one...");
                        Config.Write(ConfigPath);
                    }
                    else
                    {
                        player.SendInfoMessage(tag + "Gatekeeper has been reloaded.");
                    }
                    return;

                default:
                    player.SendInfoMessage("Gatekeeper Commands List:");
                    player.SendInfoMessage("{0}gk panic [on/off] - [On]Temporarily enables all gates except for Maintenance mode.", Commands.Specifier);
                    player.SendInfoMessage("{0}gk [kickAll/ka] - Kick all players except you.", Commands.Specifier);
                    player.SendInfoMessage("{0}gk [kickUsers/ku] - Kick all players except you and your staffs.", Commands.Specifier);
                    player.SendInfoMessage("{0}gk [kickOrdinary/ko] - Kick all players except you, your staffs, permitted users, and permitted groups.", Commands.Specifier);
                    player.SendInfoMessage("{0}gk [kickUnregistered/kun] - Kick all unregistered players.", Commands.Specifier);
                    player.SendInfoMessage("{0}gk status - Show Gatekeeper's status.", Commands.Specifier);
                    player.SendInfoMessage("{0}gk reload - Reload Gatekeeper's config.", Commands.Specifier);
                    return;
            }
        }

        private void OnJoinAsync(JoinEventArgs args)
        {
            if (TShock.Players[args.Who] == null || args.Handled)
                return;

            var player = TShock.Players[args.Who];

            if (!Config.Settings.Enabled)
                return;
            //Main Gate
            if (Config.Settings.MaintenanceMode)
            {
                if (!player.Group.HasPermission(Config.Settings.staffGroupsPermission))
                {
                    player.Disconnect(Config.Settings.MMKickMessage);
                }
            }
            else
            {
                //First Gate
                if (Config.Settings.AllowRegisteredOnly || panicMode)
                {
                    if (TShock.UserAccounts.GetUserAccountByName(player.Name) == null ||
                        TShock.UserAccounts.GetUserAccountsByName(player.Name).Count < 1
                        )
                    {
                        player.Disconnect(Config.Settings.AROKickMessage);
                        return;
                    }
                }
                //Second Gate
                if (Config.Settings.AllowOnlyCertainGroups || panicMode)
                {
                    if (!player.Group.HasPermission(Config.Settings.allowedGroupsPermission))
                    {
                        player.Disconnect(Config.Settings.AOCGKickMessage);
                    }
                }
                //Third Gate
                if (Config.Settings.AllowOnlyCertainUsers || panicMode)
                {
                    if (!Config.Settings.allowedUsersIds.Contains(player.Account.ID))
                    {
                        player.Disconnect(Config.Settings.AOCUKickMessage);
                    }
                }

                if (Config.Settings.AllowMultipleConnectionPerIP || panicMode)
                {
                    int SameIP = 0;
                    foreach (TSPlayer ply in TShock.Players)
                    {
                        if (player.IP == ply.IP)
                        {
                            SameIP++;
                        }
                    }
                    if (SameIP > Config.Settings.MaximumNumberofConnectionPerIP)
                    {
                        player.Disconnect(Config.Settings.AMCPIMessage);
                        return;
                    }
                }
            }

            if (Config.Settings.EnableCustomUserJoinMessage)
            {
                if (Config.Settings.customMessage.ContainsKey(player.Account.ID))
                {
                    string message = Config.Settings.customMessage[player.Account.ID];
                    message = message.Replace("[playername]", player.Name);
                    message = message.Replace("[playergroup]", player.Group.Name);
                    TSPlayer.All.SendMessage(message, Color.LimeGreen);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoinAsync);
            }
            base.Dispose(disposing);
        }
    }
}