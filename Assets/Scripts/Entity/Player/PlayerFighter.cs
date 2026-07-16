using UnityEngine;
using static CameraManager;

public class PlayerFighter : FighterEntity
{
    [Header("Player Specifics")]
    public Transform cameraTransform;
    public bool canMove;

    // --- VARIABLES DE DASH (Necesarias para DashState) ---
    [Header("Dash Settings")]
    public float dashSpeed = 20f;      
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;
    public float dashForceStop = 0.1f;

    [HideInInspector] public float dashTimer;
    [HideInInspector] public float dashCooldownTimer;

    // Estado único
    public DashState DashState;

    // Input 
    private PlayerControls controls;
    private Vector2 rawInput;
    private bool dashPressed;
    private bool attackPressed;
    private bool interactPressed;
    private float dashCooldownCounter;

    private bool attackBuffer;
    private float bufferWindow = 0.2f; // Tiempo antes de terminar el ataque donde aceptamos input
    private float bufferTimer;

    public void ClearBuffer() => attackBuffer = false;

    protected override void Awake()
    {
        base.Awake();
        DashState = new DashState(this);
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => rawInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => rawInput = Vector2.zero;
        controls.Gameplay.Attack.performed += ctx => attackPressed = true;
        controls.Gameplay.Dash.performed += ctx => dashPressed = true;
        controls.Gameplay.Interact.performed += ctx => interactPressed = true;
    }

    void OnEnable()
    {
        controls?.Enable();
    }

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance != null)
        {
            GameManager.instance.InitialGameStart += HandleGameStart;
            GameManager.instance.InitialGameEnd += HandleGameEnd;
        }
    }

    void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.InitialGameStart -= HandleGameStart;
            GameManager.instance.InitialGameEnd -= HandleGameEnd;
        }

        controls?.Disable();
    }

    private void HandleGameStart() => canMove = true;
    private void HandleGameEnd() => canMove = false;

    protected override void Update()
    {
        base.Update();

        HandlePlayerActions();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    public override Vector3 GetMovementInput()
    {
        if (rawInput.sqrMagnitude < 0.1f) return Vector3.zero;

        if (!canMove) return Vector3.zero;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;

        return (forward.normalized * rawInput.y + right.normalized * rawInput.x).normalized;
    }

    public override bool GetAttackInput()
    {
        if (!canMove) return false;

        if (attackPressed)
        {
            attackPressed = false;
            attackBuffer = true;
            bufferTimer = bufferWindow;
        }

        if (attackBuffer)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0) attackBuffer = false;
        }

        return attackBuffer;
    }

    public override void ConsumeAttackInput()
    {
        attackBuffer = false;
        bufferTimer = 0;
    }

    private void HandlePlayerActions()
    {
        if (currentState != null && !currentState.CanBeInterrupted) return;

        // Caso 1: DASH
        if (dashPressed && dashCooldownTimer <= 0 && controller.isGrounded)
        {
            dashPressed = false;
            ChangeState(DashState);
            return;
        }

        if (interactPressed)
        {
            // CheckDistanceToBox()...
            // ChangeState(InteractState);
        }
    }

    public void StartDashCooldown()
    {
        dashCooldownCounter = dashCooldown;
    }
}