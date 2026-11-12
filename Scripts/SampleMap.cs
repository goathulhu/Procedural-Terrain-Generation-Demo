using Godot;

public partial class SampleMap : Map
{
	[Export] private Material TerrainMaterial;
	
	[Export] private Material GrassMaterial;
	[Export] private Mesh GrassMesh;
	
	public override void Setup()
	{
		
	}
	
	protected override void Progress()
	{
		switch (GenStep)
		{
			case 0:
				if (!StepSetup)
				{
					Console.Log(Name, "Starting Map Generation");
					Console.Log(Name, "Seed " + Seed);
					Console.Log(Name, "Building Terrain " + GenTime() + "s");
					
					ChunksPerMap = (16, 16);
					
					StepSetup = true;
				}

				NewTerrainChunk(GenX, GenY, "Terrain", 8, 2f, false, true, false, 0f, 0f, TerrainMaterial, 256f);
				break;
			case 1:
				if (!StepSetup)
				{
					//LoadingScreen.Text("Foliagizing");
					//Console.Log(Name, "Foliagizing " + GenTime() + "s");
					
					ChunksPerMap = (16, 16);
					
					StepSetup = true;
				}
				
				NewFoliageChunk(GenX, GenY, "Grass", 32, 0.5f, (1f, 1f), (1f, 0.5f), GrassMesh, GrassMaterial, 32f);
				break;
			case 2:
				NewFoliageChunk(GenX, GenY, "Grass", 32, 0.5f, (1f, 1f), (0.5f, 1f), GrassMesh, GrassMaterial, 64f);
				break;
			case 3:
				NewFoliageChunk(GenX, GenY, "Grass", 32, 0.5f, (1f, 1f), (0.5f, 1.5f), GrassMesh, GrassMaterial, 96f);
				break;
			case 4:
				NewFoliageChunk(GenX, GenY, "Grass", 32, 0.5f, (1f, 1f), (0.25f, 2f), GrassMesh, GrassMaterial, 128f);
				break;
			case 5:
				/*if (!StepSetup)
				{
					LoadingScreen.Text("Propulating");
					Console.Log(Name, "Propulating " + GenTime() + "s");
					StepSetup = true;
				}

				NewPropChunk(GenX, GenY);*/
				break;
			default:
				GenStop();
				break;
		}
	}
	
	public override float GetTerrainHeightAt(string Type, Vector2 Sp)
	{
		float TerrainHeight = 0f;
		
		switch (Type)
		{
			case "Terrain":
				//TerrainHeight += GameManager.SamplePerlinNoise(Seed + 1, Sp, 8f, 8f);
				// swamp
				TerrainHeight -= SampleWater(Sp) * 1.5f;
				TerrainHeight += GameManager.SamplePerlinNoise(Seed + 2, Sp, 12f, 0.1f) * SampleWater(Sp);

				// terrain
				TerrainHeight += SampleTerrain(Sp) * (1f - SampleNotWater(Sp));
				break;
		}
	
		return TerrainHeight;
	}
	
	private float SampleTerrain(Vector2 Sp)
	{
		return (Mathf.Clamp(GameManager.SamplePerlinNoise(Seed + 11, Sp, 0.5f, 1f), 0.45f, 0.55f) - 0.45f) * 64f;
	}

	private float SampleWater(Vector2 Sp)
	{
		return ((Mathf.Clamp(GameManager.SamplePerlinNoise(Seed + 1, Sp, 0.666f, 1f), 0.499f, 0.501f) - 0.499f) * 500f) *
			   ((Mathf.Clamp(GameManager.SamplePerlinNoise(Seed + 2, Sp, 2f, 1f), 0.499f, 0.501f) - 0.499f) * 500f);
	}

	private float SampleNotWater(Vector2 Sp)
	{
		return ((Mathf.Clamp(GameManager.SamplePerlinNoise(Seed + 1, Sp, 0.666f, 1f), 0.45f, 0.55f) - 0.45f) * 10f) *
			((Mathf.Clamp(GameManager.SamplePerlinNoise(Seed + 2, Sp, 2f, 1f), 0.45f, 0.55f) - 0.45f) * 10f);
	}

	private float SampleBiome(Vector2 Sp)
	{
		return GameManager.SamplePerlinNoise(Seed + 69, Sp, 4f, 1f);
	}
	
	protected override bool GetDoFoliageAt(string Type, Vector2 Sp, float Angle, float Height)
	{
		switch (Type)
		{
			case "Grass":
				return true;
		}
		
		return false;
	}
}
