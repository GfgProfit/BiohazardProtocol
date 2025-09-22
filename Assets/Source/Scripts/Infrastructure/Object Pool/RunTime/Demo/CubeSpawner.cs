using UnityEngine;
using System.Collections.Generic;

public sealed class CubeSpawner : MonoBehaviour
{
    [Inject] private IPool<Cube> _cubePool;

    private readonly List<Cube> _spawned = new();

    private void Update()
    {
        // Spawn new cube
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }

        // Despawn last cube on mouse click
        if (Input.GetMouseButtonDown(0) && _spawned.Count > 0)
        {
            var last = _spawned[_spawned.Count - 1];
            _spawned.RemoveAt(_spawned.Count - 1);
            _cubePool.Release(last);
        }
    }

    private void Spawn()
    {
        var c = _cubePool.Get();
        c.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        c.transform.rotation = Quaternion.identity;
        _spawned.Add(c);
    }
}