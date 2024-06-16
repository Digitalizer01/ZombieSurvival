using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    public float speed = 6.0f;
    public float rotationSpeed = 700.0f;
    public float maxHorizontalRotationSpeed = 180.0f; // Límite para la velocidad de rotación horizontal
    public float jumpHeight = 2.0f; // Altura del salto
    public float jumpHorizontalSpeed = 6.0f; // Velocidad horizontal del salto
    public float gravity = 20.0f;
    public float runMultiplier = 1.5f; // Multiplier para la velocidad de carrera
    public float shootDistance = 50.0f; // Distancia del disparo
    public int damage = 10; // Daño del disparo
    public float fireRate = 0.1f; // Velocidad de disparo en modo de metralleta
    public AudioClip shootSound; // Sonido de disparo
    public AudioClip footstepSound; // Sonido de los pasos

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private Animator animator;
    private AudioSource audioSource;
    private AudioSource footstepAudioSource; // AudioSource para los sonidos de los pasos
    private PlayerHealth playerHealth;
    private PlayerAmmo playerAmmo;

    private bool isShooting = false; // Flag para verificar si se está disparando
    private bool canJump = true; // Flag para verificar si se puede saltar

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>(); // Referencia al script de salud del jugador
        playerAmmo = GetComponent<PlayerAmmo>(); // Referencia al script de munición del jugador

        footstepAudioSource = gameObject.AddComponent<AudioSource>(); // Añadir AudioSource para los pasos
        footstepAudioSource.spatialBlend = 0; // Configurar el sonido como 2D (no espacial)
        footstepAudioSource.volume = 0.5f; // Ajustar el volumen según sea necesario
        footstepAudioSource.clip = footstepSound; // Asignar el clip de sonido de los pasos
    }

    void Update()
    {
        if (animator.GetBool("Dead")) // Verifica si el jugador está muerto
        {
            return; // Si está muerto, no hace nada más en Update
        }

        // Si el personaje no está disparando, permitir movimiento
        if (!isShooting)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            bool isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0; // Verifica si la entrada vertical es positiva
            float currentSpeed = isRunning ? speed * runMultiplier : speed;

            if (controller.isGrounded)
            {
                // Si el personaje está en el suelo, permitir movimiento
                moveDirection = new Vector3(h, 0, v);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= currentSpeed;

                // Reproducir sonido de pasos si se está moviendo
                if (footstepSound != null && (h != 0 || v != 0))
                {
                    if (!footstepAudioSource.isPlaying)
                    {
                        footstepAudioSource.Play();
                    }
                }
                else
                {
                    footstepAudioSource.Stop();
                }

                if (canJump && Input.GetButtonDown("Jump"))
                {
                    // Calcular la dirección del salto basado en la altura deseada
                    float jumpVerticalSpeed = Mathf.Sqrt(2 * jumpHeight * gravity);
                    moveDirection.y = jumpVerticalSpeed;

                    // Aplicar velocidad horizontal al salto
                    Vector3 horizontalJumpDirection = moveDirection.normalized * jumpHorizontalSpeed;
                    horizontalJumpDirection.y = jumpVerticalSpeed;
                    moveDirection = horizontalJumpDirection;

                    animator.SetTrigger("Roll"); // Activa el trigger "Roll" en el animator al saltar
                    canJump = false; // No permitir otro salto hasta que toque el suelo
                }
            }
            else
            {
                // En el aire, mantener el movimiento lateral pero no modificar la dirección Y excepto por gravedad
                Vector3 airControl = new Vector3(h, 0, v);
                moveDirection += transform.TransformDirection(airControl) * (currentSpeed * 0.1f);
            }

            if (h != 0 || v != 0)
            {
                animator.SetFloat("Speed", isRunning ? 1.0f : 0.5f);

                // Ensure the character only rotates on the Y axis
                Vector3 direction = new Vector3(moveDirection.x, 0, moveDirection.z);
                if (direction.magnitude > 0.1f)
                {
                    float targetRotationSpeed = Mathf.Clamp(rotationSpeed, -maxHorizontalRotationSpeed, maxHorizontalRotationSpeed);
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, targetRotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
            }

            // Apply gravity manually
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the character controller
            CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);

            // Check if the character is grounded
            if ((flags & CollisionFlags.Below) != 0)
            {
                canJump = true; // Permitir saltar nuevamente si el personaje está en el suelo
            }
        }

        // Handle shooting
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            isShooting = true;
            InvokeRepeating("Shoot", 0f, fireRate); // Comienza a disparar según la velocidad de disparo
            animator.SetBool("Aiming", true);
        }
        else if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isShooting = false;
            CancelInvoke("Shoot"); // Detiene el disparo continuo
            animator.SetBool("Aiming", false);
        }
    }

    void Shoot()
    {
        if (playerAmmo.GetAmmoAmount() > 0)
        {
            // Solo dispara si se está presionando el botón del ratón
            if (isShooting)
            {
                if (shootSound != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }

                Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, shootDistance))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        // Handle damage to the enemy
                        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(damage);
                        }
                    }
                }
                playerAmmo.ReduceAmmo(1);
            }
        }
    }

    // Método para curar al jugador
    public void HealPlayer(int amount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
        }
    }

    // Método para recargar munición
    public void AddAmmo(int amount)
    {
        if (playerAmmo != null)
        {
            playerAmmo.AddAmmo(amount);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthBox"))
        {
            HealthPickup healthPickup = other.GetComponent<HealthPickup>();
            if (healthPickup != null)
            {
                HealPlayer(healthPickup.healAmount);
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("AmmoBox"))
        {
            AmmoPickup ammoPickup = other.GetComponent<AmmoPickup>();
            if (ammoPickup != null)
            {
                AddAmmo(ammoPickup.ammoAmount);
                Destroy(other.gameObject);
            }
        }
    }
}
