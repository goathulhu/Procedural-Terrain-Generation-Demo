using Godot;

public partial class DummyMap : Map
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
					
					ChunksPerMap = (8, 8);
					
					StepSetup = true;
				}

				NewTerrainChunk(GenX, GenY, "Terrain", 8, 2f, false, true, false, 0f, 0f, TerrainMaterial, 256f);
				break;
			case 1:
				if (!StepSetup)
				{
					//LoadingScreen.Text("Foliagizing");
					//Console.Log(Name, "Foliagizing " + GenTime() + "s");
					
					ChunksPerMap = (8, 8);
					
					StepSetup = true;
				}
				
				NewFoliageChunk(GenX, GenY, "Grass", 32, 0.5f, (1f, 1f), (1f, 1.25f), GrassMesh, GrassMaterial, 64f);
				break;
			case 2:
				//NewFoliageChunk(GenX, GenY, "Grass", 32, 0.5f, (1f, 1f), (0.75f, 1f), GrassMesh, Grass2Material, 64f);
				break;
			case 3:
				//NewFoliageChunk(GenX, GenY, "Grass", 48, 0.333f, (1f, 1f), (0.5f, 0.75f), GrassMesh, Grass3Material, 128f);
				break;
			case 4:
				//NewFoliageChunk(GenX, GenY, "Grass", 64, 0.25f, (1f, 1f), (0.25f, 0.5f), GrassMesh, Grass4Material, 256f);
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
				TerrainHeight += GameManager.SamplePerlinNoise(Seed + 1, Sp, 8f, 8f);
				break;
		}
		
		return TerrainHeight;
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
