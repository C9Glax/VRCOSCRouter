namespace VRCOSCRouter;

public struct Endpoint(string vrcEndpoint, string programEndpoint, Type type)
{
    public readonly string VrcEndpoint = vrcEndpoint, ProgramEndpoint = programEndpoint;
    public readonly Type Type = type;
}