# SpaceMonkey - Open Source Telemetry Provider.

SpaceMonkey was created to be a telemetry interface for games with no native telemetry support. 

SpaceMonkey mimics the functionality of the Codemasters Dirt 4 custom udp format which allows SpaceMonkey to be used via the UDP protocol by any software that already supports Dirt 4 and Dirt Rally 2.0 custom udp.
SpaceMonkey also optionally writes telemetry to a Memory Mapped File as an alternative to UDP.

SpaceMonkey contains telemetry visualisation and filtering functionality.

SpaceMonkey supports XINPUT, currently mapped to standard gamepad inputs for steering (left stick), accelerator(right trigger), and brake(left trigger). Clutch and axis assignments coming in a future version. Use the XBOX 360 controller emulator to map your direct input devices to an XINPUT gamepad. https://www.x360ce.com/

### Supported Games

- Dirt 5
- Wreckfest
- BeamNG.Drive
- GTA 5
- Digital Combat Simulator


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
## DIRT5

### Usage

1. Load Dirt 5 and get to the main menu with the vehicle visible.
2. Go to the main interface of Space Monkey and press the DIRT5 button.
3. Click the Initialize! button in the SpaceMonkey Dirt5UI and wait for the game's memory to be scanned. If successful the status message will change to Success! and you will see debug output in the text box.
4. SpaceMonkey will now be outputting telemetry.


---
## Wreckfest

### Usage

1. Load Wreckfest in 32 bit mode.
2. Setup a session and get to the stage where you have the start race option on the left and the selected vehicle in the centre of the screen.
3. Go to the main interface of Space Monkey and press the Wreckfest button.
4. Select the player number from the drop down. In single player this is always 00. In multiplayer this is the order of your player in the lobby at the point that you join the lobby. Check as soon as you enter the lobby or you may get the wrong number.
5. Click the Initialize button and wait for the game's memory to be scanned. If successful the status message will change to Success! and you will see debug output in the text box.
6. Space Monkey will now be outputting telemetry.

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

1. Go to the main interface of Space Monkey and press the BeamNG Drive button.
2. Specify the UDP receive port specified in the setup step (Default 4444)
3. Press the Initialize! button.
4. Space Monkey will now wait for a connection from BeamNG.Drive and output telemetry automatically. You can launch BeamNG.Drive and any other software at this point.

---
## GTAV

### Setup

1. Install ScripthookV: https://www.gta5-mods.com/tools/script-hook-v
2. Download and install Community Script Hook V .NET: https://www.gta5-mods.com/tools/scripthookv-net
3. Run the "SMTP GTA Plugin.msi" installer in the GTAV subfolder of SpaceMonkey. Install into your GTAV root folder, the default location is appropriate for a default steam installation, just need to change the drive letter.

### Usage

1. Go to the main interface of Space Monkey and press the GTAV button.
2. Press the Initialize! button.
3. Space Monkey will now wait for a connection from GTAV and output telemetry automatically. You can launch GTAV and any other software at this point.

---
## Digital Combat Simulator

### Setup

1. Backup any Export.lua files in your C:\Users\USER_NAME\Saved Games\DCS\Scripts\
2. Run the "SMTP DCS Export Plugin.msi" installer in the DCS subfolder of SpaceMonkey. Install into your C:\Users\USER_NAME\Saved Games\DCS\Scripts folder, the default destination should be appropriate.

### Usage

1. Go to the main interface of Space Monkey and press the DCS button.
2. Specify the UDP receive port specified in the setup step (Default 6666). If you want to recieve on a different port you will also need to edit the Export.lua file installed to C:\Users\USER_NAME\Saved Games\DCS\Scripts\ and change the port.
3. Press the Initialize! button.
4. Space Monkey will now wait for a connection from DCS and output telemetry automatically. You can launch DCS and any other software at this point.

---


## SimCommander 4

## Setup
1. Create a new game entry under settings with the following settings. Point the Game Exe at the SpaceMonkeyTP.exe in the location you installed it to previously.
![SC4Game](https://github.com/PHARTGAMES/SpaceMonkey/blob/main/Documentation/SC4Game.PNG?raw=true))]

2. You have the option to create a duplicate of an existing Dirt Rally 2.0 or Dirt 4 profile, or you can create a profile from scratch, then change it's settings as follows.
![SC4Profile](https://github.com/PHARTGAMES/SpaceMonkey/blob/main/Documentation/SC4Profile.PNG?raw=true))]

## Usage

1. Click the launch button on your new profile, SpaceMonkey will load.
2. Perform the Usage steps for the game you wish to use, as described in this document.






