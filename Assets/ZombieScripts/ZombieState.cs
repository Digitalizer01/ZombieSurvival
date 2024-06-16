using UnityEngine;

public abstract class ZombieBaseState
{
    public abstract void EnterState(ZombieController zombie);
    public abstract void UpdateState(ZombieController zombie);
}
