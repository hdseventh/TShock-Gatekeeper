# TShock-Gatekeeper
TShock gatekeeper, i don't really know what to put here. This is literally a gatekeeper, if you have the "pass" you can enter the server else you'll be kicked by the gatekeeper.
This is just a glorified whitelist for my terraria server "KodokMan" a year ago (now it is closed). Pretty useful when your server are under attack by persistent cheaters at 3 AM in the morning. This will hold them from joining your server while you sleep and let your 8 AM self handle them.

## How does this plugin work?
There's a few option you can enable in the config file, but basically this plugin will kick anyone who did not have the "pass" from entering your server. The "pass" are :
- Registered User = The plugin will kick everyone who are not registered yet upon joining. But it will allow registered users to enter.
- Staff Only = The plugin will kick everyone who are not a staff upon joining. It recognize staff accounts by checking if they have the necessary permission.
- Certain Groups Only = The plugin will kick everyone who aren't in any allowed groups upon joining. You can add allowed groups in the config file.
- Certain Users Only = The plugin will kick everyone who aren't allowed upon joining. You can add allowed users account id in the config file.
- There's also a few other fun function, but you can explore them yourself in the config file or even better in the source code.

## Config file
Config file location is at /tshock/Gatekeeper/

## Commands
- `/gk` - Gatekeeper main command -Permission need `gatekeeper.admin`
- `/gk panic` - Temporarily enables all gates except for Maintenance mode. Use this command again to disable it
- `/gk [kickAll/ka]` - Kick all players except you.
- `/gk [kickUsers/ku]` - Kick all players except you and your staffs.
- `/gk [kickOrdinary/ko` - Kick all players except you, your staffs, permitted users, and permitted groups.
- `/gk [kickUnregistered/kun]` - Kick all unregistered players.
- `/gk status` - Show Gatekeeper's status.
- `/gk reload` - Reload Gatekeeper's config.
