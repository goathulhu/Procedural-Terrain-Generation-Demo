using Godot;

public partial class InputManager : Node
{
	private float DeltaTime = 0f;

	private GameManager GameManager;

	private InputBind[] Binds;

	private float DoubleTapThreshold = 0.2f;

	public float Sensitivity = 0.05f;
	public float TabletSensitivity = 0.0025f;

	private bool DoDebug = false;

	private Vector2 LastMouseInputValue = Vector2.Zero;
	private Vector2 CurrentMouseInputValue = Vector2.Zero;
	private Vector2 DeltaMouseInputValue = Vector2.Zero;
	
	private float LastMouseWheelValue = 0f;
	private float CurrentMouseWheelValue = 0f;
	private float DeltaMouseWheelValue = 0f;

	public override void _Ready()
	{
		SetBinds();

		GameManager = (GameManager)GetTree().Root.FindChild("GameManager", true, false);

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion EventMouseMotion)
			CurrentMouseInputValue += EventMouseMotion.Relative;
		
		if (@event is InputEventMouseButton EventMouseButton)
		{
			if (EventMouseButton.ButtonIndex == MouseButton.WheelUp)
				CurrentMouseWheelValue += 1f;
			if (EventMouseButton.ButtonIndex == MouseButton.WheelDown)
				CurrentMouseWheelValue -= 1f;
		}
	}

	public override void _Process(double delta)
	{
		DeltaTime = GameManager.GlobalDeltaTime;

		DeltaMouseInputValue = CurrentMouseInputValue - LastMouseInputValue;
		LastMouseInputValue = CurrentMouseInputValue;

		for (int i = 0; i < Binds.Length; i++)
		{
			Binds[i].IsInput = false;
			for (int j = 0; j < Binds[i].Keys.Length; j++)
			{
				if (Binds[i].Keys[j].KeyBind != Key.None)
					if (Input.IsKeyPressed(Binds[i].Keys[j].KeyBind))
						Binds[i].IsInput = true;
				if (Binds[i].Keys[j].ButtonBind != MouseButton.None)
					if (Input.IsMouseButtonPressed(Binds[i].Keys[j].ButtonBind))
						Binds[i].IsInput = true;
			}

			Binds[i].Down = !Binds[i].Hold && Binds[i].IsInput; // check if the action has just been pressed
			Binds[i].Up = Binds[i].Hold && !Binds[i].IsInput; // check if the action has just stopped
			Binds[i].Hold = Binds[i].IsInput; // check if the action is being pressed
			Binds[i].DoubleTap =
				Binds[i].DoubleTapTimer > 0f && Binds[i].Down; // check if the action has been double tapped

			if (Binds[i].Down) Binds[i].DoubleTapTimer = DoubleTapThreshold; // starts the doubletap timer
			if (Binds[i].DoubleTap) Binds[i].DoubleTapTimer = 0f; // resets the doubletap timer
			if (Binds[i].DoubleTapTimer > 0f) Binds[i].DoubleTapTimer -= DeltaTime; // updates the doubletap timer

			if (DoDebug)
			{
				//if (Binds[i].Down) GameManager.Console.Log("\\/ " + Binds[i].Action);
				//if (Binds[i].Hold) GameManager.Console.Log("|| " + Binds[i].Action);
				//if (Binds[i].DoubleTap) GameManager.Console.Log("-- " + Binds[i].Action);
				//if (Binds[i].Up) GameManager.Console.Log("/\\ " + Binds[i].Action);
			}
		}
	}

	public bool Down(string Action)
	{
		foreach (InputBind BIND in Binds)
		{
			if (BIND.Action == Action) return BIND.Down;
		}

		return false;
	}

	public bool Hold(string Action)
	{
		foreach (InputBind BIND in Binds)
		{
			if (BIND.Action == Action) return BIND.Hold;
		}

		return false;
	}

	public bool Up(string Action)
	{
		foreach (InputBind BIND in Binds)
		{
			if (BIND.Action == Action) return BIND.Up;
		}

		return false;
	}

	public bool DoubleTap(string Action)
	{
		foreach (InputBind BIND in Binds)
		{
			if (BIND.Action == Action) return BIND.DoubleTap;
		}

		return false;
	}

	public float MouseWheelDelta()
	{
		return DeltaMouseWheelValue;
	}

	public Vector2 MouseDelta()
	{
		return DeltaMouseInputValue;
	}

	public Vector3 GetMovementInputVector()
	{
		Vector3 MovementInputVector = Vector3.Zero;

		if (Hold("forward")) MovementInputVector.Z += 1f;
		if (Hold("left")) MovementInputVector.X -= 1f;
		if (Hold("backward")) MovementInputVector.Z -= 1f;
		if (Hold("right")) MovementInputVector.X += 1f;
		if (Hold("up")) MovementInputVector.Y += 1f;
		if (Hold("down")) MovementInputVector.Y -= 1f;

		return MovementInputVector;
	}

	public bool IsMovementInput()
	{
		return Hold("forward") || Hold("left") || Hold("backward") || Hold("right") || Hold("up") || Hold("down");
	}

	private void SetBinds()
	{
		Binds = new InputBind[]
		{
			// static
			new InputBind("mwup", MouseButton.WheelUp),
			new InputBind("mwdown", MouseButton.WheelDown),
			new InputBind("arrowup", Key.Up),
			new InputBind("arrowdown", Key.Down),
			new InputBind("arrowleft", Key.Left),
			new InputBind("arrowright", Key.Right),
			new InputBind("return", Key.Enter),
			new InputBind("esc", Key.Escape),
			new InputBind("lmb", MouseButton.Left),
			new InputBind("rmb", MouseButton.Right),
			new InputBind("mmb", MouseButton.Middle),
			
			// rebindable
			new InputBind("primary", MouseButton.Left),
			new InputBind("secondary", MouseButton.Right),
			new InputBind("forward", Key.W),
			new InputBind("left", Key.A),
			new InputBind("backward", Key.S),
			new InputBind("right", Key.D),
			new InputBind("up", Key.Space),
			new InputBind("down", Key.C).AddBind(Key.Ctrl).AddBind(MouseButton.Xbutton2),
		};
	}
}
