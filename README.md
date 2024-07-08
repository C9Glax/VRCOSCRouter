# VRCOSCRouter

Simple OSC-Router for OSC-Programs that do not use OSC-Query.

## Example `Program.json`

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