using Godot;
using System;

public class Player : KinematicBody
{
    [Export] public float Gravity = -24.8f;
    [Export] public float MaxSpeed = 20.0f;
    [Export] public float JumpSpeed = 18.0f;
    [Export] public float Acceleration = 4.5f;
    [Export] public float Deacceleration = 16.0f;
    [Export] public float MaxSlopeAngle = 40.0f;
    [Export] public float MouseSensitivity = 0.05f;

    private Vector3 _velocity = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;

    private Spatial _rotationHelper;
    private Camera _camera;

    private bool _isPlaying = false;

    private float _setGrav;
    private float _setSpeed;
    
    public override void _Ready()
    {
        //_setGrav = Gravity;
        //_setSpeed = MaxSpeed;
        //Gravity = 0.0f;
        //MaxSpeed = 0.0f;

        _rotationHelper = GetNode<Spatial>("RotationHelper");
        _camera = _rotationHelper.GetNode<Camera>("Camera");

        GetNode<Control>("../HUD").Connect("StartGame", this, nameof(_On_Game_Started));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_isPlaying)
        {
            ProcessInput();
            ProcessMovement(delta);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseEvent && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            _rotationHelper.RotateX(Mathf.Deg2Rad(-mouseEvent.Relative.y * MouseSensitivity));
            RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

            Vector3 cameraRotation = _rotationHelper.RotationDegrees;
            cameraRotation.x = Mathf.Clamp(cameraRotation.x, -85, 85);
            _rotationHelper.RotationDegrees = cameraRotation;
        }
    }

    private void ProcessInput()
    {
        _direction = Vector3.Zero;
        
        Vector2 inputMovementVector = Vector2.Zero;
        if (Input.IsActionPressed("MoveForward"))
            inputMovementVector.y -= 1;
        if (Input.IsActionPressed("MoveBack"))
            inputMovementVector.y += 1;
        if (Input.IsActionPressed("MoveLeft"))
            inputMovementVector.x -= 1;
        if (Input.IsActionPressed("MoveRight"))
            inputMovementVector.x += 1;
        inputMovementVector = inputMovementVector.Normalized();
        
        Transform cameraTransform = _camera.GlobalTransform;

        _direction += cameraTransform.basis.z * inputMovementVector.y;
        _direction += cameraTransform.basis.x * inputMovementVector.x;

        if (IsOnFloor())
        {
            if (Input.IsActionPressed("MoveUp"))
            {
                _velocity.y = JumpSpeed;
            }
        }

        if (Input.IsActionPressed("Escape") && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }

        if (Input.IsMouseButtonPressed(1) && Input.GetMouseMode() == Input.MouseMode.Visible)
        {
            Input.SetMouseMode(Input.MouseMode.Captured);
        }
    }

    private void ProcessMovement(float delta)
    {
        _direction.y = 0;
        _direction = _direction.Normalized();

        _velocity.y += delta * Gravity;

        Vector3 horizontalVelocity = _velocity;
        horizontalVelocity.y = 0;

        Vector3 target = _direction * MaxSpeed;
        float acceleration = (_direction.Dot(horizontalVelocity) > 0) ? Acceleration : Deacceleration;

        horizontalVelocity = horizontalVelocity.LinearInterpolate(target, acceleration * delta);

        _velocity.x = horizontalVelocity.x;
        _velocity.z = horizontalVelocity.z;
        _velocity = MoveAndSlide(_velocity, Vector3.Up, false, 4, Mathf.Deg2Rad(MaxSlopeAngle));
    }

    private void _On_Game_Started()
    {
        //Gravity = _setGrav;
        //MaxSpeed = _setSpeed;

        _isPlaying = true;
        
        Input.SetMouseMode(Input.MouseMode.Captured);
    }
}
