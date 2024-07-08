# VRCOSCRouter

Simple OSC-Router for OSC-Programs that do not use OSC-Query.
Can also translate Endpoint names for programs that have hard-coded parameters.

## Usage

Place as many `xxx.json` files as you want in the same folder as the executable.
Each file represents a OSC-Program you want to route.

### Example `Program.json`

```json
{
  "Name": "Program name",
  "Ip": "127.0.0.1",
  "SendPort": 8000,
  "ReceivePort": 8001,
  "Endpoints": [
    {
      "ProgramEndpoint": "/avatar/parameters/What_The_Program_Is_Sending",
      "VrcEndpoint": "/avatar/parameters/Avatar_Parameter",
      "Type": "System.Single/System.String/etc."
    }
  ]
}
```