using UnityEngine;

public class ZombieDeadState : ZombieBaseState
{
    public override void EnterState(ZombieController zombie)
    {
        zombie.SetBool("isWalking", false);
        zombie.SetBool("isAttacking", false);
        zombie.SetBool("isDead", true);
        zombie.StopMoving();
    }

    public override void UpdateState(ZombieController zombie)
    {
        // No hacer nada, el zombie está muerto
    }
}
