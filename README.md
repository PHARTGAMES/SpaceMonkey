# SpaceMonkey - Open Source Telemetry Provider.

SpaceMonkey was created to be a telemetry interface for games with no native telemetry support. 

SpaceMonkey mimics the functionality of the Codemasters Dirt 4 custom udp format which allows SpaceMonkey to be used via the UDP protocol by any software that already supports Dirt 4 and Dirt Rally 2.0 custom udp.
SpaceMonkey also optionally writes telemetry to a Memory Mapped File as an alternative to UDP.

SpaceMonkey contains telemetry visualisation and filtering functionality.

SpaceMonkey supports XINPUT, currently mapped to standard gamepad inputs for steering (left stick), accelerator(right trigger), and brake(left trigger). Clutch and axis assignments coming in a future version. Use the XBOX 360 controller emulator to map your direct input devices to an XINPUT gamepad. https://www.x360ce.com/

SpaceMonkey has been tested with SimCommander 4 (Accuforce v2) and SimFeedback (motion) and should work with any software that supports Dirt 4 Custom UDP.

### Supported Games

- Dirt 5
- Wreckfest
- BeamNG.Drive
- GTA 5
- Digital Combat Simulator
- Nascar Heat 4/5 (MonsterGames)
- All American Racing (MonsterGames)
- Sprint Car Racing (MonsterGames)
- WRC 7/8/9
- Richard Burns Rally (NGP 6)
- STAR WARS: Squadrons (Coming v1.0.4)
- Warplanes: WW1 (Coming v1.0.4)
- VTOL VR (Coming v1.0.4)
- IL-2 Sturmovik (Coming v1.0.4)


## Installation

The latest installer for SpaceMonkey is here (v1.0.3)

https://github.com/PHARTGAMES/SpaceMonkey/raw/main/GenericTelemetryProvider/Installer/SpaceMonkeyTP-SetupFiles/SpaceMonkeyTP.msi

Install anywhere to your local drive.


## Main Interface


![Main Interface](https://github.com/PHARTGAMES/SpaceMonkey/blob/main/Documentation/MainInterface.png?raw=true))]

1. Game selection buttons; press one to load the interface for the selected game.
2. Filters Button; press this to load the filters interface.
3. Config selection; Choose or duplicate/rename the main configuration parameters. Configs are stored in the Configs folder. Ideally a config file will be created for each game as they often have different requirements for filtering.
4. Telemetry Ouput; Choose how you want telemetry to be output from SpaceMonkey and configure the UDP out parameters.
5. Packet formatting; Choose the packet format configuration file or specify "Packet Format Destinations"; these are folders that the packet format configuration file is copied to. Packet formats are specified in the PacketFormats folder and conform to the Dirt 4 Custom UDP specification. https://www.scribd.com/document/350826037/UDP-output-setup
6. Filter Config and Hotkey. Extra filter configs can be created by copying a filter config in the Filters folder. The hotkey can be used to pause and resume telemetry globally when the app is not in focus.

All changes to config options are saved as they are changed.


---
# Games

## DIRT5

### Usage

1. Load Dirt 5 and get to the main menu with the vehicle visible.
2. Go to the main interface of Space Monkey and select a main config for DIRT5.
3. Go to the main interface of Space Monkey and press the DIRT5 button.
4. Click the Initialize! button in the SpaceMonkey Dirt5UI and wait for the game's memory to be scanned. If successful the status message will change to Success! and you will see debug output in the text box.
5. SpaceMonkey will now be outputting telemetry.


---
## Wreckfest

### Usage

1. Load Wreckfest in 32 bit mode.
2. Setup a session and get to the stage where you have the start race option on the left and the selected vehicle in the centre of the screen.
3. Go to the main interface of Space Monkey and select a main config for Wreckfest.
4. Go to the main interface of Space Monkey and press the Wreckfest button.
5. Select the player number from the drop down. In single player this is always 00. In multiplayer this is the order of your player in the lobby at the point that you join the lobby. Check as soon as you enter the lobby or you may get the wrong number.
6. Click the Initialize button and wait for the game's memory to be scanned. If successful the status message will change to Success! and you will see debug output in the text box.
7. Space Monkey will now be outputting telemetry.

---
## BeamNG.Drive

### Setup

1. Within BeamNG.drive 'Options > OTHER', set the following options:

- MotionSim enabled [x]
- IP: 127.0.0.1
- Port: 4444
- Update rate: 50
- Acceleration Smoothing: X, Y, Z all set to 0. (This is a personal preference, however these values are also programatically smoothed within the plugin code.)

### Usage

1. Go to the main interface of Space Monkey and select a main config for BeamNG Drive.
2. Go to the main interface of Space Monkey and press the BeamNG Drive button.
3. Specify the UDP receive port specified in the setup step (Default 4444)
4. Press the Initialize! button.
5. Space Monkey will now wait for a connection from BeamNG.Drive and output telemetry automatically. You can launch BeamNG.Drive and any other software at this point.

---
## GTAV

### Setup

1. Install ScripthookV: https://www.gta5-mods.com/tools/script-hook-v
2. Download and install Community Script Hook V .NET: https://www.gta5-mods.com/tools/scripthookv-net
3. Run the "SMTP GTA Plugin.msi" installer in the GTAV subfolder of SpaceMonkey. Install into your GTAV root folder, the default location is appropriate for a default steam installation, just need to change the drive letter.

### Usage

1. Go to the main interface of Space Monkey and select a main config for GTAV.
2. Go to the main interface of Space Monkey and press the GTAV button.
3. Press the Initialize! button.
4. Space Monkey will now wait for a connection from GTAV and output telemetry automatically. You can launch GTAV and any other software at this point.

---
## Digital Combat Simulator

### Setup

1. Backup any Export.lua files in your C:\Users\USER_NAME\Saved Games\DCS\Scripts\
2. Run the "SMTP DCS Export Plugin.msi" installer in the DCS subfolder of SpaceMonkey. Install into your C:\Users\USER_NAME\Saved Games\DCS\Scripts folder, the default destination should be appropriate.
3. If you are using multiple export files, you can remove the SpaceMonkey Export.lua and add a "dofile(lfs.writedir()..[[Scripts\ExportSM.lua]])" entry to your main Export.lua.

### Usage

1. Go to the main interface of Space Monkey and select a main config for DCS.
2. Go to the main interface of Space Monkey and press the DCS button.
3. Specify the UDP receive port specified in the setup step (Default 6666). If you want to recieve on a different port you will also need to edit the Export.lua file installed to C:\Users\USER_NAME\Saved Games\DCS\Scripts\ and change the port.
4. Press the Initialize! button.
5. Space Monkey will now wait for a connection from DCS and output telemetry automatically. You can launch DCS and any other software at this point.

---


## Nascar Heat 4/5, All American Racing and Sprint Car Racing.

### Usage

1. Load the game.
2. Go to the main interface of Space Monkey and select a main config for MonsterGames..
3. Go to the main interface of Space Monkey and press the Nascar Heat 4/5..etc.. button which will load the MonsterGamesUI.
4. In the Monster Games UI, click the Initialize button and wait for a message box to appear in the top left corner of the game window stating that SpaceMonkey is injected.
5. Space Monkey will now be outputting telemetry. Load any other software at this point.

---
## WRC 7/8/9

### Warning

This feature only outputs Sway, Heave and Surge accelerations and may not perform optimally. As such, the output does not have enough information to run an Accuforce wheel in FFB Foundation mode through SimCommander. 
It uses a dll provided by the user Motion4All on RaceDepartment.

### Setup

1. Extract the contents of the "WRCInjection.zip" file in the WRC subfolder of SpaceMonkey into your WRC 7/8/9 game folder.
2. Run the Install.bat file.

### Usage

1. Go to the main interface of Space Monkey and select a main config for WRC.
2. Go to the main interface of Space Monkey and press the WRC 7/8/9 button.
3. Press the Initialize! button.
4. Space Monkey will now wait for a connection from WRC and output telemetry automatically. You can launch WRC and any other software at this point.

---
## Richard Burns Rally (NGP 6)

### Setup

1. Ensure you have NGP 6 installed in your Richard Burns Rally installation.
2. Edit the RichardBurnsRally.ini file next to the RichardBurnsRally_SSE.exe and ensure that these settings under the [NGP] section look like this:

udpTelemetry=1

udpTelemetryAddress=192.168.50.194

udpTelemetryPort=6775

You may be able to use 127.0.0.1 if you enable loopback from your internal ip, however you can just resolve your IPv4 using ipconfig.exe on a command prompt.

3. In the game make sure that options/plugins/ngp6/udp telemetry is turned on (u to toggle), but it should be if the RichardBurnsRally.ini is setup correctly.

### Usage

1. Go to the main interface of Space Monkey and select a main config for Richard Burns Rally.
2. Go to the main interface of Space Monkey and press the Richard Burns Rally button.
3. Specify the UDP receive port specified in the setup step (Default 6775)
4. Press the Initialize! button.
5. Space Monkey will now wait for a connection from Richard Burns Rally and output telemetry automatically. You can launch Richard Burns Rally and any other software at this point.

---
## STAR WARS: Squadrons

### Warning

This only works offline and with the Steam version of the game. You must uninstall EasyAntiCheat for Space Monkey to work. You need to block connections to the game in your firewall so that it can't tell the servers that you are playing with EasyAntiCheat disabled. Follow the Setup steps.

### Setup

1. Setup firewall rules to block udp and tcp from both the starwarssquadrons.exe and starwarssquadrons_launcher.exe files.
2. In the STAR WARS: Squadrons game folder there is a subfolder named EasyAntiCheat. Run EasyAntiCheat_Setup.exe and choose uninstall.
3. In the SpaceMonkey install folder there is a subfolder name Squadrons. Copy the steam_appid.txt from this folder into the STAR WARS: Squadrons game folder.

### Usage

1. Launch STAR WARS: Squadrons by running the starwarssquadrons.exe located in the game install folder.
2. Go to the main interface of Space Monkey and select a main config for Squadrons.
3. Go to the main interface of Space Monkey and press the STAR WARS: Squadrons button which will load the Squadrons UI.
4. Press the Initialize! button.
5. You can now switch back to STAR WARS: Squadrons and you should have telemetry once you start a game.

---

## Warplanes WW1

### Usage

1. Load the game.
2. Go to the main interface of Space Monkey and select a main config for Warplanes WW1..
3. Go to the main interface of Space Monkey and press the Warplanes WW1 button which will load the Warplanes WW1 UI.
4. In the Warplanes WW1 UI, click the Initialize button .
5. Space Monkey will now be outputting telemetry. Load any other software at this point.

---


## VTOL VR

### Usage

1. Load the game.
2. Go to the main interface of Space Monkey and select a main config for VTOL VR..
3. Go to the main interface of Space Monkey and press the VTOL VR button which will load the VTOL VR UI.
4. In the VTOL VR UI, click the Initialize button .
5. Space Monkey will now be outputting telemetry. Load any other software at this point.

---

## IL-2 Sturmovik

### Setup

1. Edit the file data/startup.cfg located within the IL-2 Sturmovik game folder and add the following:

[KEY = motiondevice]

addr = "127.0.0.1"

decimation = 1

enable = true

port = 4321

[END]

Simulation produces 50Hz rate data output (output 50 samples per second) of in-game player body's state: orientation, rotation speed (spin) and acceleration (if game mission has user-controlled body). To reduce UDP messages output rate the above setup section contains an integer setting “decimation”: UDP_output_rate = Data_output_rate / decimation The default setup makes UDP output rate at the simulation's rate and is equal 50Hz.

Some people may find better results with "decimation = 2".

### Usage

1. Go to the main interface of Space Monkey and select a main config for IL-2 Sturmovik.
2. Go to the main interface of Space Monkey and press the IL-2 Sturmovik button.
3. Specify the UDP receive port specified in the setup step (Default 4321)
4. Press the Initialize! button.
5. Space Monkey will now wait for a connection from IL-2 Sturmovik and output telemetry automatically. You can launch IL-2 Sturmovik and any other software at this point.

---



# SimCommander 4

### Setup
1. Create a new game entry under settings with the following settings. Point the Game Exe at the SpaceMonkeyTP.exe in the location you installed it to previously.
![SC4Game](https://github.com/PHARTGAMES/SpaceMonkey/blob/main/Documentation/SC4Game.PNG?raw=true))]

2. For car games, you have the option to create a duplicate of an existing Dirt Rally 2.0 or Dirt 4 profile, for Aircraft games you can duplicate an X-Plane profile. You can also create a profile from scratch, then change it's settings as follows.
![SC4Profile](https://github.com/PHARTGAMES/SpaceMonkey/blob/main/Documentation/SC4Profile.PNG?raw=true))]

### Usage

1. Click the launch button on your new profile, SpaceMonkey will load.
2. Perform the Usage steps for the game you wish to use, as described in this document.


---

# SimFeedback

Setup instructions here

https://github.com/PHARTGAMES/SpaceMonkey/blob/main/GTPSimfeedback/README.md

---

# MMF Support

The mutex used by SpaceMonkey is named "GenericTelemetryProviderMutex"

The MMF used by SpaceMonkey is named "GenericTelemetryProviderFiltered"

Example MMF usage here https://github.com/PHARTGAMES/SpaceMonkey/blob/main/GTPSimfeedback/GTPTelemetryProvider.cs

---

# Known Issues

1. Sometimes SpaceMonkey will not close correctly. In the case that SC4 thinks the app is still running or running the app doesn't create a new window, find the SpaceMonkeyTP.exe process in task manager and end the process manually.

---

# Support

SpaceMonkeyTelemetry@gmail.com

---

# Authors

PEZZALUCIFER
