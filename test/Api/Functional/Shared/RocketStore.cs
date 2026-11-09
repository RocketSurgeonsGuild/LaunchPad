using System.Collections.Concurrent;

namespace Api.Functional;

/// <summary>The servers' in-memory rockets.</summary>
public sealed class RocketStore
{
    private readonly ConcurrentDictionary<Guid, string> _rockets = new();

    public Guid Add(string name)
    {
        var id = Guid.NewGuid();
        _rockets[id] = name;
        return id;
    }

    public string? Find(Guid id) => _rockets.TryGetValue(id, out var name) ? name : null;

    public void Set(Guid id, string name) => _rockets[id] = name;

    public bool Remove(Guid id) => _rockets.TryRemove(id, out _);

    public IEnumerable<(Guid Id, string Name)> All(string? name) => _rockets.Where(r => name is null || r.Value == name).Select(r => (r.Key, r.Value));
}
