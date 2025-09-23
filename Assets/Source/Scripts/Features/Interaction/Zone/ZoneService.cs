using System;
using System.Collections.Generic;

public sealed class ZoneService : IZoneService
{
    private readonly HashSet<string> _open = new(StringComparer.Ordinal);

    public bool IsOpen(string key) => string.IsNullOrEmpty(key) || _open.Contains(key);

    public void Open(string key)
    {
        if (string.IsNullOrEmpty(key) || _open.Contains(key)) return;
        _open.Add(key);
        ZoneOpened?.Invoke(key);
    }

    public void OpenMany(IEnumerable<string> keys)
    {
        foreach (var k in keys) Open(k);
    }

    public void Close(string key)
    {
        if (string.IsNullOrEmpty(key) || !_open.Remove(key)) return;
        ZoneClosed?.Invoke(key);
    }

    public event Action<string> ZoneOpened;
    public event Action<string> ZoneClosed;
}