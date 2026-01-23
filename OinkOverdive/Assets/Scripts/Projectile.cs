using FishNet.Object;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Vector2 _dir;
    private float _speed;
    private bool _initialized;

    [Server]
    public void InitServer(Vector2 dir, float speed)
    {
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        dir.Normalize();

        ApplyInit(dir, speed);

        // Jetzt, wo es gespawnt ist, wird das sauber an alle verteilt
        InitObserversRpc(dir, speed);
    }

    [ObserversRpc(BufferLast = true)]
    private void InitObserversRpc(Vector2 dir, float speed)
    {
        ApplyInit(dir, speed);
    }

    private void ApplyInit(Vector2 dir, float speed)
    {
        _dir = dir;
        _speed = speed;
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized)
            return;

        transform.position += (Vector3)(_dir * _speed * Time.deltaTime);
    }
}


