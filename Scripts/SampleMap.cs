using Godot;

public partial class SampleMap : Map
{
	[Export] private Material TerrainMaterial;
	[Export] private Material WaterMaterial;
	[Export] private Material GrassMaterial1;
	[Export] private Material GrassMaterial2;
	[Export] private Material GrassMaterial3;
	[Export] private Mesh GrassMesh;
	
	int Size = 16;
	
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
					
					ChunksPerMap = (Size, Size);
					
					StepSetup = true;
				}
				
				NewTerrainChunk(GenX, GenY, "Terrain", 16, 1f, false, true, false, 0f, 0f, TerrainMaterial, 256f);
				NewTerrainChunk(GenX, GenY, "Water", 1, 16f, false, false, false, 0f, 0f, WaterMaterial, 256f);
				break;
			case 1:
				if (!StepSetup)
				{
					//LoadingScreen.Text("Foliagizing");
					//Console.Log(Name, "Foliagizing " + GenTime() + "s");
					
					ChunksPerMap = (Size, Size);
					
					StepSetup = true;
				}
				
				NewFoliageChunk(GenX, GenY, 
					"Grass", 
					16, 1f, 
					1f, 0.5f, 
					GrassMesh, GrassMaterial1, 64f);
				break;
			case 2:
				NewFoliageChunk(GenX, GenY, 
					"Grass", 
					12, 1.333f, 
					1f, 1f, 
					GrassMesh, GrassMaterial2, 128f);
				break;
			case 3:
				NewFoliageChunk(GenX, GenY, 
					"Grass", 
					8, 2f, 
					1f, 1.5f, 
					GrassMesh, GrassMaterial3, 256f);
				break;
			case 4:
				//NewFoliageChunk(GenX, GenY, "Grass", 8, 2f, (1f, 1f), (0.25f, 2f), GrassMesh, GrassMaterial, 240f);
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
				//TerrainHeight -= SampleWater(Sp) * 1.5f;
				//TerrainHeight += GameManager.SamplePerlinNoise(Seed + 2, Sp, 12f, 0.1f) * SampleWater(Sp);
				
				// terrain
				//TerrainHeight += SampleTerrain(Sp) * (1f - SampleNotWater(Sp));
				
				/////////////////////////////////////////////
				
				/*float bumps = GameManager.SamplePerlinNoise(Seed + 11, Sp, 5f, 1f);
				float bumpsNoise = 0f;//SampleNoise(sP, 0.05f, 2f);
				
				float platforms = GameManager.SamplePerlinNoise(Seed + 11, Sp, 10f, 1f);
				float platformsNoise = 0f;//SampleNoise(sP, 0.075f, 1.5f);
				
				float waterNoise = GameManager.SamplePerlinNoise(Seed + 1, Sp, 0.666f, 1f);
				float water = waterNoise;
				float wMin = 0.48f;
				float wMax = 0.52f;
				float wMid = (wMin + wMax) / 2f;
				float depth = 40f;
				if (water > wMin && water < wMax)
				{
					float tempWater = water - wMid;
					if (tempWater < 0f) tempWater *= -1f;
					water = tempWater * -depth + (wMid - wMin) * depth;
					
					platforms /= 2f;
					bumps /= 2f;
				}	
				else water = 0f;
				
				float waterCarve = waterNoise;
				float wcDiff = 0.025f;
				float wcMin = wMin - wcDiff;
				float wcMax = wMax + wcDiff;
				float wcDepth = depth;
				if (waterCarve > wcMin && waterCarve < wcMax)
				{
					float tempWater = waterCarve - wMid;
					if (tempWater < 0f) tempWater *= -1f;
					waterCarve = tempWater * -wcDepth + (wMid - wcMin) * wcDepth;
				}
				else waterCarve = 0f;
				
				//TerrainHeight = Mathf.Clamp((bumps - water) + (platforms - water), -1f, 128f);
				TerrainHeight = (bumps - water) + (platforms - water);*/
				
				/*float Water = GameManager.SamplePerlinNoise(Seed + 1, Sp, 0.666f, 1f);
				float WaterSliceStart = 0.48f;
				float WaterSliceEnd = 0.52f;
				float WaterSliceMiddle = (WaterSliceStart + WaterSliceEnd) * 0.5f;
				float depth = 40f;
				if (water > wMin && water < wMax)
				{
					float tempWater = water - WaterSliceMiddle;
					if (tempWater < 0f) tempWater *= -1f;
					water = tempWater * -depth + (WaterSliceMiddle - WaterSliceStart) * depth;
					
					platforms /= 2f;
					bumps /= 2f;
				}	
				else water = 0f;*/
				break;
			case "Water":
				TerrainHeight = -0.5f;
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
				return Height > -0.75f;
		}
		
		return false;
	}
}
