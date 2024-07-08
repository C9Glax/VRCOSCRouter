namespace VRCOSCRouter;

public struct ProgramInfo(string name, string ip, int sendPort, int receivePort, List<Endpoint> endpoints)
{
    public string Name = name, Ip = ip;
    public int SendPort = sendPort, ReceivePort = receivePort;
    public List<Endpoint> Endpoints = endpoints;
}