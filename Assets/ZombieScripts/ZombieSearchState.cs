using UnityEngine;

public class ZombieSearchState : ZombieBaseState
{
    private float timeSinceLastRandomMove = 0f;
    private float timeToNextRandomMove = 2f; // Tiempo entre movimientos aleatorios

    public override void EnterState(ZombieController zombie)
    {
        zombie.SetBool("isAttacking", false);
        zombie.SetBool("isDead", false);
        zombie.SetBool("isWalking", true); // Asegurar que isWalking sea true al entrar en el estado
        zombie.StartMoving();
    }

    public override void UpdateState(ZombieController zombie)
    {
        if (zombie.player != null && Vector3.Distance(zombie.transform.position, zombie.player.position) < 10.0f)
        {
            zombie.TransitionToState(zombie.attackState); // Si detecta al jugador cerca, pasar al estado de ataque
        }
        else
        {
            // Si ha pasado suficiente tiempo desde el último movimiento aleatorio, moverse a un punto aleatorio
            if (Time.time >= timeSinceLastRandomMove + timeToNextRandomMove)
            {
                zombie.MoveToRandomPoint();
                timeSinceLastRandomMove = Time.time;
            }

            // Mantener isWalking como true mientras está en movimiento
            zombie.SetBool("isWalking", true);
        }
    }
}
