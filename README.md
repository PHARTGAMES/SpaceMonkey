# SpaceMonkey - Open Source Telemetry Provider.

SpaceMonkey was created to be a telemetry interface for games with no native telemetry support. 

SpaceMonkey mimics the functionality of the Codemasters Dirt 4 custom udp format which allows SpaceMonkey to be used via the UDP protocol by any software that already supports Dirt 4 and Dirt Rally 2.0 custom udp.
SpaceMonkey also optionally writes telemetry to a Memory Mapped File as an alternative to UDP.

SpaceMonkey contains telemetry visualisation and filtering functionality.

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
