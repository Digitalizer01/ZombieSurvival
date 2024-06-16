using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    public float attackInterval = 1.0f; // Intervalo entre ataques
    public int attackDamage = 10; // Daño por ataque
    public float closeDistance = 2.0f; // Distancia para considerar que el jugador está cerca
    public Color closeColor = Color.green; // Color cuando el jugador está cerca
    public Color farColor = Color.red; // Color cuando el jugador está lejos
    public float rotationSpeed = 700.0f; // Valor de ejemplo, ajusta según sea necesario

    private float lastAttackTime;

    public override void EnterState(ZombieController zombie)
    {
        zombie.SetBool("isWalking", true);
        zombie.SetBool("isDead", false);
        zombie.StartMoving();
    }

    public override void UpdateState(ZombieController zombie)
    {
        // Lógica de movimiento y ataque
        if (Vector3.Distance(zombie.transform.position, zombie.player.position) > closeDistance)
        {
            zombie.SetBool("isAttacking", false);
            zombie.MoveToPlayer();
        }
        else
        {
            zombie.StopMoving();
            zombie.SetBool("isWalking", false);
            zombie.SetBool("isAttacking", true);
            AttackPlayer(zombie);
        }

        // Dibujar la línea entre el jugador y el zombie
        DrawLineBetweenPlayerAndZombie(zombie);

        // Verificar si el jugador está fuera del rango de ataque y transicionar al estado de búsqueda
        if (Vector3.Distance(zombie.transform.position, zombie.player.position) > closeDistance)
        {
            zombie.TransitionToState(zombie.searchState);
        }
    }

    private void AttackPlayer(ZombieController zombie)
    {
        // Calcular la dirección hacia el jugador
        Vector3 directionToPlayer = zombie.player.position - zombie.transform.position;

        // Rotar el zombie para que mire al jugador
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        zombie.transform.rotation = Quaternion.RotateTowards(zombie.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Realizar el ataque si ha pasado el intervalo desde el último ataque
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;
            PlayerHealth playerHealth = zombie.player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Zombie attacks player!");
            }
        }
    }

    private void DrawLineBetweenPlayerAndZombie(ZombieController zombie)
    {
        // Determinar el color de la línea según la distancia entre el jugador y el zombie
        Color lineColor = Vector3.Distance(zombie.transform.position, zombie.player.position) <= closeDistance ? closeColor : farColor;

        // Dibujar la línea
        Debug.DrawLine(zombie.transform.position, zombie.player.position, lineColor);
    }
}
