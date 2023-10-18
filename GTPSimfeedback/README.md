# SpaceMonkey SimFeedback


SpaceMonkey SimFeedback is a SimFeedback plugin for the SpaceMonkey Telemetry Provider.

### Setup

1. Install SpaceMonkey Telemetry Provider and follow instructions here. https://github.com/PHARTGAMES/SpaceMonkey
2. Install SpaceMonkey SimFeedback into your root SimFeedback folder using the installer here. https://github.com/PHARTGAMES/SpaceMonkey/raw/main/GTPSimfeedback/Installer/SpaceMonkeySFX-SetupFiles/SpaceMonkeySFX.msi


### Configuration

1. Edit the SMConfig.txt file in a text editor 
2. The options are as follows
- udpPort: This is the port that SpaceMonkey SimFeedback will recieve UDP telemetry through when recieveUDP is set to true within this same file.
- receiveUDP: When this value is true SpaceMonkey SimFeedback will recieve UDP telemetry on the port specified by udpPort in this same file.
- receiveMMF: When this value is true SpaceMonkey SimFeedback will recieve telemetry through a Memory Mapped File named "GenericTelemetryProviderFiltered" using the global mutex "GenericTelemetryProviderMutex".
- integrated: When this value is true SpaceMonkey Telemetry Provider will launch automatically when pressing Start on a compatible profile and receive telemetry through a callback interface.


### Usage (integrated:true)

1. Launch SimFeedback as administrator.
2. Select the SpaceMonkey compatible profile in SimFeedback and press Start to load the SpaceMonkey window.
3. Configure SpaceMonkey as described here https://github.com/PHARTGAMES/SpaceMonkey


### Usage (integrated:false)

1. Launch SimFeedback
2. Launch SpaceMonkeyStart.exe as administrator.
2. Select the SpaceMonkey compatible profile in SimFeedback and press Start.
3. Configure SpaceMonkey as described here https://github.com/PHARTGAMES/SpaceMonkey


To use SpaceMonkey in conjunction with other supported software follow the guide here https://github.com/PHARTGAMES/SpaceMonkey
