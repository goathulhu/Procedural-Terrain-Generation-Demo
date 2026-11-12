using Godot;

public partial class GameManager : Node3D
{
	private float DeltaTime = 0f;
	
	public float GlobalDeltaTime = 0f;
	public float GlobalTimeScale = 1f;

	// references
	public InputManager InputManager;
	public Console Console;
	public Player Player;
	public Hud Hud;

	public RandomNumberGenerator Rng = new RandomNumberGenerator();

	private bool ConsoleOpen;

	[ExportCategory("Fps Display")]
	[Export] private RichTextLabel FpsLabel;
	private float FpsTimer;
	private float CumFps;
	private int HowManyFps;

	[ExportCategory("Map Generation")]
	[Export] public PackedScene DummyMap;
	[Export] public PackedScene TerrainChunkPrefab;
	[Export] public PackedScene FoliageChunkPrefab;
	[Export] public PackedScene PropChunkPrefab;
	[Export] public PackedScene PoiChunkPrefab;
	public Map CurrentMap;

	[ExportCategory("PhysicsCasts")]
	[Export] RayCast3D Ray;
	[Export] ShapeCast3D Sphere;

	// noise
	private FastNoiseLite NoisePerlin = new FastNoiseLite();

	public override void _Ready()
	{
		InputManager = (InputManager)GetTree().Root.FindChild("InputManager", true, false);
		Console = (Console)GetTree().Root.FindChild("Console", true, false);
		Player = (Player)GetTree().Root.FindChild("Player", true, false);
		Hud = (Hud)GetTree().Root.FindChild("Hud", true, false);
		
		NoisePerlin.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Perlin);
		NoisePerlin.SetFractalType(FastNoiseLite.FractalTypeEnum.None);

		var NewMap = DummyMap.Instantiate();
		AddChild(NewMap);
		CurrentMap = (Map)NewMap;
		CurrentMap.Init(this, 696969);
	}

	public override void _Process(double delta)
	{
		GlobalDeltaTime = (float)delta * GlobalTimeScale;
		DeltaTime = GlobalDeltaTime;

		FpsTimer += DeltaTime;
		CumFps += 1f / DeltaTime;
		HowManyFps++;
		if (FpsTimer >= 0.5f)
		{
			FpsLabel.Text = "[color=666666]" + (int)(CumFps / HowManyFps) + "fps " + DeltaTime * 1000f + "ms ";
			CumFps = 0f;
			HowManyFps = 0;
			FpsTimer = 0f;
		}
	}
	
	public void OpenConsole()
	{
		ConsoleOpen = true;
	}

	public void CloseConsole()
	{
		ConsoleOpen = false;
	}

	public bool IsConsoleOpen()
	{
		return ConsoleOpen;
	}
	
	public (bool DidHit, Vector3 Position, Vector3 Normal, Node Collider) Raycast(Vector3 From, Vector3 To)
	{
		Ray.GlobalPosition = From;
		Ray.TargetPosition = To - From;
		
		Ray.ForceRaycastUpdate();
		
		return (Ray.IsColliding(), Ray.GetCollisionPoint(), Ray.GetCollisionNormal(), (Node)Ray.GetCollider());
	}
	
	public (bool DidHit, Node[] Colliders) Spherecast(Vector3 At, float Radius)
	{
		Sphere.GlobalPosition = At;
		((SphereShape3D)Sphere.Shape).Radius = Radius;
		
		Sphere.ForceShapecastUpdate();
		
		int Count = Sphere.GetCollisionCount();
		Node[] Colliders = new Node[Count];
		for (int i = 0; i < Count; i++)
		{
			Colliders[i] = (Node)Sphere.GetCollider(i);
		}
		
		return (Sphere.IsColliding(), Colliders);
	}

	public float SamplePerlinNoise(int Seed, Vector2 Position, float Scale, float Height)
	{
		NoisePerlin.SetSeed(Seed);

		return ((NoisePerlin.GetNoise2D(Position.X * Scale, Position.Y * Scale) + 1f) * 0.5f) * Height;
	}
}
