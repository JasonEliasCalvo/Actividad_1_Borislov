using UnityEngine;
using static CameraManager;

public class PlayerFighter : FighterEntity
{
    [Header("Player Specifics")]
    public Transform cameraTransform;

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

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

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

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;

        return (forward.normalized * rawInput.y + right.normalized * rawInput.x).normalized;
    }

    public override bool GetAttackInput()
    {
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

        // Caso 2: INTERACTUAR (Tirar cajas, etc)
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

    private void HandleMovement()
    {
        if (CameraManager.instance.currentStyle == CameraStyle.Basic || CameraManager.instance.currentStyle == CameraStyle.Topdown)
        {
            //Vector3 finalVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
            //controller.Move(finalVelocity * Time.deltaTime);
        }
        else if (CameraManager.instance.currentStyle == CameraStyle.Combat)
        {

        }
    }
}