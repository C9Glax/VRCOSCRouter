// See https://aka.ms/new-console-template for more information

using GlaxLogger;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VRCOSCRouter;

Logger logger = new (LogLevel.Debug, consoleOut: Console.Out);
logger.LogInformation("Starting VRCOSCRouter...");

string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.json");
List<OscProgram> programs = new();
foreach (string filePath in files)
{
  string fileContent = File.ReadAllText(filePath);
  if(!fileContent.Contains("Name")
     || !fileContent.Contains("Ip")
     || !fileContent.Contains("SendPort")
     || !fileContent.Contains("ReceivePort")
     || !fileContent.Contains("Endpoints"))
    continue;
  ProgramInfo pi = JsonConvert.DeserializeObject<ProgramInfo>(fileContent);
  programs.Add(new OscProgram(pi.SendPort, pi.ReceivePort, pi.Endpoints, pi.Name, pi.Ip, logger));
}

while(!Console.KeyAvailable && Console.ReadKey().Key != ConsoleKey.Escape)
  Thread.Sleep(10);