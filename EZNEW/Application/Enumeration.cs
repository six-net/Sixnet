using System;

namespace EZNEW.Application
{
    /// <summary>
    /// Defines application type
    /// </summary>
    [Serializable]
    public enum ApplicationType
    {
        WebSite = 110,
        WebAPI = 120,
        WindowsService = 130,
        Console = 140,
        WindowsForm = 150,
        ApplicationService = 160
    }

    /// <summary>
    /// Defines application status
    /// </summary>
    [Serializable]
    public enum ApplicationStatus
    {
        Ready = 200,
        Starting = 205,
        Running = 210,
        Paused = 215,
        Stoped = 220,
        Closed = 225
    }
}
