using GlaxOSC;
using Microsoft.Extensions.Logging;
using BuildSoft.OscCore;

namespace VRCOSCRouter;

public class OscProgram
{
     public string ServiceName;
     public readonly int SendPort;
     public readonly int ReceivePort;
     private readonly OSC _vrcOscClient;
     private readonly OscServer _programOscReceiver;
     private readonly OscClient _programOscSender;
     public bool _keepRunning = true;
     private readonly Thread _listenThread;
     private const int UpdateInterval = 10;
     private readonly ILogger? _logger;
     internal readonly List<Endpoint> Endpoints;

     public OscProgram(int sendPort, int receivePort, List<Endpoint> endpoints, string? serviceName = null, string ip = "127.0.0.1", ILogger? logger = null)
     {
          this._logger = logger;
          this.ServiceName = serviceName ?? $"VRCOSCRouter_{RandomString(4)}";

          this.Endpoints = endpoints;
          
          this.SendPort = sendPort;
          this.ReceivePort = receivePort;
          this._vrcOscClient = new OSC(this.ServiceName, endpoints.ToDictionary(e => e.VrcEndpoint, e => e.Type), ip, sendPort, logger: logger);

          _programOscReceiver = new OscServer(sendPort);
          foreach (KeyValuePair<string, Type> endpoint in endpoints.ToDictionary(e => e.ProgramEndpoint, e=> e.Type))
               this._programOscReceiver.TryAddMethod(endpoint.Key, values => HandleProgramOscMessage(endpoint.Key, endpoint.Value, values));
          
          _programOscSender = new OscClient("127.0.0.1", this.ReceivePort);
          this._vrcOscClient.OnParameterChangeEvent += HandleVrcOscMessage;
          
          this._listenThread = new(Listen);
          this._listenThread.Start();
          
          this._logger?.LogInformation($"Started {this.ServiceName}." +
                                      $"\n\tProgramSend: {this.SendPort}" +
                                      $"\n\tProgramReceive: {this.ReceivePort}" +
                                      $"\n\tVRCOSC-Port: {this._vrcOscClient.OSCPort}" +
                                      $"\n\tEndpoints:\n\t\t{string.Join("\n\t\t", this.Endpoints.Select(e => $"({e.Type.Name}) {e.ProgramEndpoint} -> {e.VrcEndpoint}"))}");
     }

     private void HandleVrcOscMessage(string endpoint, object? oldvalue, object? newvalue)
     {
          if (newvalue is null)
               return;
          
          if (this.Endpoints.All(e => e.VrcEndpoint != endpoint))
               return;
          string programEndpoint = this.Endpoints.First(e => e.VrcEndpoint == endpoint).ProgramEndpoint;
          
          Type type = newvalue.GetType();
          if (type == typeof(int))
               this._programOscSender.Send(endpoint, (int)newvalue);
          else if (type == typeof(long))
               this._programOscSender.Send(endpoint, (long)newvalue);
          else if (type == typeof(float))
               this._programOscSender.Send(endpoint, (float)newvalue);
          else if (type == typeof(double))
               this._programOscSender.Send(endpoint, (double)newvalue);
          else if (type == typeof(string))
               this._programOscSender.Send(endpoint, (string)newvalue);
          else if (type == typeof(char))
               this._programOscSender.Send(endpoint, (char)newvalue);
          else if (type == typeof(bool))
               this._programOscSender.Send(endpoint, (bool)newvalue);
          
          this._logger?.LogDebug($"[{this.ServiceName}] VRC '{endpoint}' - {newvalue} -> Program {programEndpoint}");
     }

     private void HandleProgramOscMessage(string endpoint, Type type, OscMessageValues values)
     {
          if (this.Endpoints.All(e => e.ProgramEndpoint != endpoint))
               return;
          string vrcEndpoint = this.Endpoints.First(e => e.ProgramEndpoint == endpoint).VrcEndpoint;

          object o = "fail";
          if (type == typeof(int))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadIntElement(0));
               o = values.ReadIntElement(0);
          }
          else if (type == typeof(long))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadInt64Element(0));
               o = values.ReadInt64Element(0);
          }
          else if (type == typeof(float))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadFloatElement(0));
               o = values.ReadFloatElement(0);
          }
          else if (type == typeof(double))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadFloat64Element(0));
               o = values.ReadFloat64Element(0);
          }
          else if (type == typeof(string))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadStringElement(0));
               o = values.ReadStringElement(0);
          }
          else if (type == typeof(char))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadAsciiCharElement(0));
               o = values.ReadAsciiCharElement(0);
          }
          else if (type == typeof(bool))
          {
               this._vrcOscClient.Client.Send(vrcEndpoint, values.ReadBooleanElement(0));
               o = values.ReadBooleanElement(0);
          }
          
          this._logger?.LogDebug($"[{this.ServiceName}] Program '{endpoint}' - {o} -> VRC {vrcEndpoint}");
     }

     private void Listen()
     {
          this._programOscReceiver.Start();
          while (_keepRunning)
          {
               this._programOscReceiver.Update();
               Thread.Sleep(UpdateInterval);
          }
     }
     
     private static string RandomString(int length)
     {
          const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
          var random = new Random();
          return new string(Enumerable.Repeat(chars, length)
               .Select(s => s[random.Next(s.Length)]).ToArray());
     }
}