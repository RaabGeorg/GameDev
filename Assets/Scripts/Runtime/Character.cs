using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class Character : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Audio Settings")]
    [Range(0f, 1f)] [SerializeField] private float runningVolume = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float jumpVolume = 0.5f;

    private AudioSource runningSoundSource;
    private Animator animator;
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;
    private bool isJumping = false;
    private float jumpCooldownTimer;
    private AudioSource musicSource;

    [Header("Movement Settings")]
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float characterSpeed;
    [SerializeField] private float dampening;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float platformRayDistance;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Character Stats")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    private Vector3 characterMovement;
    private Vector3 jumpVelocity;
    private Vector3 platformVelocity;
    private Vector3 characterGravity;

    private void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
        this.animator = this.GetComponent<Animator>();
        this.runningSoundSource = gameObject.AddComponent<AudioSource>();
        this.runningSoundSource.playOnAwake = false;
        this.runningSoundSource.loop = true;
        this.runningSoundSource.outputAudioMixerGroup = sfxMixerGroup;
        this.musicSource = gameObject.AddComponent<AudioSource>();
        this.musicSource.clip = backgroundMusic;
        this.musicSource.loop = true;      
        this.musicSource.playOnAwake = true; 
        this.musicSource.outputAudioMixerGroup = sfxMixerGroup; 
    
        this.musicSource.Play();
        
        this.currentHealth = this.maxHealth;
        
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }

        if (this.controller.isGrounded && !this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;
            
            if (jumpSound != null)
            {
                GameObject go = new GameObject("JumpSoundTemp");
                go.transform.position = transform.position;
                AudioSource source = go.AddComponent<AudioSource>();
                source.clip = jumpSound;
                source.volume = jumpVolume;
                source.outputAudioMixerGroup = sfxMixerGroup;
                source.Play();
                Destroy(go, jumpSound.length);
            }
        }

        if (this.jumpVelocity.y > 0.0f)
            this.jumpVelocity.y -= Time.fixedDeltaTime;
        else
            this.jumpVelocity = Vector3.zero;

        this.jumpCooldownTimer -= Time.fixedDeltaTime;
    }

    private void HandlePlatforms()
    {
        this.platformVelocity = Vector3.zero;
        if(this.controller.isGrounded 
            && Physics.Raycast(this.transform.position, Vector3.down, out var hit, this.platformRayDistance, LayerMask.GetMask("Platforms"))) {
            var platformObject = hit.collider.gameObject;
            var movingPlatform = platformObject.GetComponent<MovingPlatform>();
            if(movingPlatform != null)
            {
                this.platformVelocity = movingPlatform.GetVelocity();
            }
        }
    }

    void SetAnimationState(Vector2 inputMovement) {
        this.animator.SetBool("isJumping", this.isJumping);
        this.animator.SetBool("isRunning", inputMovement != Vector2.zero);
        this.animator.SetFloat("movementForward", inputMovement.magnitude);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHead"))
        {
            
            BasicEnemy enemy = other.GetComponentInParent<BasicEnemy>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
    }

    private void FixedUpdate()
    {
        this.HandleJumping();
        this.HandlePlatforms();

        var inputMovement = this.moveAction.ReadValue<Vector2>();

        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;

        inputRightDirection.y = 0f;
        inputForwardDirection.y = 0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        //Since we do not use the physics system, we have to simulate gravity ourselves
        if (this.controller.isGrounded) {
            this.characterGravity.y = 0.0f;
        }

        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;
        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;
        this.characterMovement += inputRightDirection * inputMovement.x * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * this.characterSpeed * Time.fixedDeltaTime;

        this.characterMovement *= (1.0f - this.dampening);

        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;

        if(characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero) {
            this.transform.forward = characterForward.normalized;
        }

        this.controller.Move(this.characterMovement + this.platformVelocity * Time.fixedDeltaTime);
        
        this.SetAnimationState(inputMovement);

        if (this.controller.isGrounded && inputMovement != Vector2.zero && !this.isJumping)
        {
            if (!this.runningSoundSource.isPlaying)
            {
                this.runningSoundSource.clip = runningSound;
                this.runningSoundSource.volume = runningVolume;
                this.runningSoundSource.pitch = 0.3f;
                this.runningSoundSource.Play();
            }
        }
        else
        {
            if (this.runningSoundSource.isPlaying)
                this.runningSoundSource.Stop();
        }
    }
    
    public void InflictDamage(float amount) {
        this.currentHealth -= amount ;
        this.currentHealth = Mathf .Clamp( this.currentHealth, 0.0f, this.maxHealth);
    }
    

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
        
}
