using System;
using System.Collections.Generic;

public interface IZoneService
{
    bool IsOpen(string key);
    void Open(string key);
    void Close(string key);
    void OpenMany(IEnumerable<string> keys);

    event Action<string> ZoneOpened;
    event Action<string> ZoneClosed;
}