using Godot;
using System;

public partial class Player : Node3D
{
	private float DeltaTime;

	private GameManager GameManager;
	private InputManager InputManager;
	private Console Console;
	private RandomNumberGenerator Rng;
	private Hud Hud;

	public Vector3 LastPos;
	public Vector3 DeltaPos;
	public float MoveSpeed;
	
	public (float Current, float Target, float Idle, float Walk) Speed;
	public (Vector3 XYZ, Vector3 XYZTarget, Vector3 XYZActual) Velocity;
	public (Vector3 XYZ, Vector3 XYZTarget) LandHang;
	
	public Vector3 InputDirection = Vector3.Zero;
	public Vector3 GlobalInputDirection = Vector3.Zero;
	
	public Vector2 CurrentCameraRot = Vector2.Zero;
	public Vector2 LastCameraRot = Vector2.Zero;
	public Vector2 DeltaCameraRot = Vector2.Zero;
	
	[Export] public CharacterBody3D Controller;
	
	[ExportCategory("Body")]
	[Export] public Node3D BodyRotY;
	[Export] public CollisionShape3D Collider;
	public SphereShape3D ColliderShape;
	[Export] public Node3D CameraTargetPos;
	
	[ExportCategory("Camera")]
	[Export] public Node3D PlayerView;
	[Export] public Node3D CameraMotionGlobal;
	[Export] public Node3D CamRotY;
	[Export] public Node3D CamRotX;
	[Export] public Node3D CameraMotionLocal;
	[Export] public Camera3D PlayerCamera;
	[Export] public Node3D CameraOrientation;
	[Export] public Node3D SpreadR;
	[Export] public Node3D Director;
	
	// flags
	public bool CanMove;
	public bool CanLook;
	
	public bool IsMenu;
	public bool IsConsole;
	
	public bool IsMoving;
	public bool IsMovementInput;
	
	public override void _Ready()
	{
		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);
		InputManager = GameManager.InputManager;
		Console = GameManager.Console;
		Rng = GameManager.Rng;
		Hud = GameManager.Hud;
		
		ColliderShape = (SphereShape3D)Collider.Shape;
		
		Speed.Current = 0f;
		Speed.Target = 0f;
		Speed.Idle = 0f; 
		Speed.Walk = 24f;
		
		Velocity.XYZ = Vector3.Zero;
		Velocity.XYZTarget = Vector3.Zero;
		Velocity.XYZActual = Vector3.Zero;
		
		LandHang.XYZ = Vector3.Zero;
		LandHang.XYZTarget = Vector3.Zero;
	}
	
	public override void _Process(double delta)
	{
		DeltaTime = GameManager.GlobalDeltaTime;
		
		Flags();
		
		Movement();
		
		Camera();
		
		Actions();
	}
	
	private void Flags()
	{
		IsConsole = GameManager.IsConsoleOpen();
		
		CanMove = !IsConsole && !IsMenu;
		
		CanLook = !IsConsole && !IsMenu;
		
		IsMovementInput = InputManager.IsMovementInput();
		
		IsMoving = CanMove && IsMovementInput;
	}
	
	private void Movement()
	{
		if (CanMove && IsMovementInput)
		{
			InputDirection = InputManager.GetMovementInputVector().Normalized();
			GlobalInputDirection = ((BodyRotY.GlobalBasis.X * InputDirection.X) + (-BodyRotY.GlobalBasis.Z * InputDirection.Z) + Vector3.Up * InputDirection.Y).Normalized();
		}
		else
		{
			InputDirection = Vector3.Zero;
			GlobalInputDirection = Vector3.Zero;
		}

		DeltaPos = Controller.GlobalPosition - LastPos;
		MoveSpeed = Controller.GlobalPosition.DistanceTo(LastPos);
		LastPos = Controller.GlobalPosition;
		
		if (IsMovementInput) Speed.Current = Speed.Walk;
		else Speed.Current = Speed.Idle;
		Speed.Current *= GameManager.GlobalTimeScale;
		
		Velocity.XYZTarget = GlobalInputDirection * Speed.Current;
		Velocity.XYZ = Velocity.XYZ.Lerp(Velocity.XYZTarget, DeltaTime * 2f);
		Controller.Velocity = Velocity.XYZ;
		Controller.MoveAndSlide();
	}
	
	private void Camera()
	{
		// update rotation
		if (CanLook)
		{
			CamRotY.Rotation = new Vector3(0f, CamRotY.Rotation.Y - Mathf.DegToRad(InputManager.MouseDelta().X * InputManager.Sensitivity), 0f);
			CamRotX.Rotation = new Vector3(CamRotX.Rotation.X - Mathf.DegToRad(InputManager.MouseDelta().Y * InputManager.Sensitivity), 0f, 0f);
			CamRotX.Rotation = new Vector3(Mathf.Clamp(CamRotX.Rotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f)), 0f, 0f);
		}
		BodyRotY.Rotation = new Vector3(0f, CamRotY.Rotation.Y, 0f);
		
		// delta
		CurrentCameraRot = new Vector2(Mathf.RadToDeg(CamRotX.Rotation.X), Mathf.RadToDeg(CamRotY.Rotation.Y));
		DeltaCameraRot = LastCameraRot - CurrentCameraRot;
		LastCameraRot = CurrentCameraRot;
		
		// update position
		PlayerView.GlobalPosition = PlayerView.GlobalPosition.Lerp(
			CameraTargetPos.GlobalPosition + LandHang.XYZ
			, DeltaTime * 16f);
		
		// global camera motion
		CameraMotionGlobal.Rotation = CameraMotionGlobal.Rotation.Lerp(
			new Vector3(
				Mathf.DegToRad(Velocity.XYZ.Z * 0.25f), 
				0f, 
				Mathf.DegToRad(Velocity.XYZ.X * -0.25f)), DeltaTime * 8f);
	}
	
	private void Actions()
	{
		
	}
	
	// --------------------------------------------------------------------------------------------------------------------- //
	// -------------------------------------------------- utility section -------------------------------------------------- //
	// --------------------------------------------------------------------------------------------------------------------- //
	
	public void TeleportTo(Vector3 NewPosition)
	{
		LastPos = NewPosition;
		Controller.GlobalPosition = NewPosition;
		PlayerView.GlobalPosition = CameraTargetPos.GlobalPosition;
	}
}
