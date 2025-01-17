﻿using UnityEngine;

public static class EffectsFactory
{

	private static GameObject fireTile;

	private static GameObject smallBloodTile;
	private static GameObject mediumBloodTile;
	private static GameObject largeBloodTile;
	private static GameObject largeAshTile;
	private static GameObject smallAshTile;
	private static GameObject waterTile;

	private static GameObject smallXenoBloodTile;
	private static GameObject medXenoBloodTile;
	private static GameObject largeXenoBloodTile;

	private static void EnsureInit()
	{
		if (fireTile == null)
		{
			//Do init stuff
			fireTile = Resources.Load("FireTile") as GameObject;
			smallBloodTile = Resources.Load("SmallBloodSplat") as GameObject;
			mediumBloodTile = Resources.Load("MediumBloodSplat") as GameObject;
			largeBloodTile = Resources.Load("LargeBloodSplat") as GameObject;
			largeAshTile = Resources.Load("LargeAsh") as GameObject;
			smallAshTile = Resources.Load("SmallAsh") as GameObject;
			waterTile = Resources.Load("WaterSplat") as GameObject;
			smallXenoBloodTile = Resources.Load("SmallXenoBloodSplat") as GameObject;
			medXenoBloodTile = Resources.Load("MedXenoBloodSplat") as GameObject;
			largeXenoBloodTile = Resources.Load("LargeXenoBloodSplat") as GameObject;
		}
	}

	//FileTiles are client side effects only, no need for network sync (triggered by same event on all clients/server)
	public static void SpawnFireTileClient(float fuelAmt, Vector3 localPosition, Transform parent)
	{
		EnsureInit();
		//ClientSide pool spawn
		GameObject fireObj = Spawn.ClientPrefab(fireTile, Vector3.zero).GameObject;
		//Spawn tiles need to be placed in a local matrix:
		fireObj.transform.parent = parent;
		fireObj.transform.localPosition = localPosition;
		FireTile fT = fireObj.GetComponent<FireTile>();
		fT.StartFire(fuelAmt);
	}

	public static void BloodSplat(Vector3 worldPos, BloodSplatSize splatSize, BloodSplatType bloodColorType)
	{
		EnsureInit();
		GameObject chosenTile = null;
		switch (bloodColorType)
		{
			case BloodSplatType.red:
				switch (splatSize)
				{
					case BloodSplatSize.small:
						chosenTile = smallBloodTile;
						break;
					case BloodSplatSize.medium:
						chosenTile = mediumBloodTile;
						break;
					case BloodSplatSize.large:
						chosenTile = largeBloodTile;
						break;
					case BloodSplatSize.Random:
						int rand = Random.Range(0, 3);
						BloodSplat(worldPos, (BloodSplatSize)rand, bloodColorType);
						return;
				}
				break;
			case BloodSplatType.green:
				switch (splatSize)
				{
					case BloodSplatSize.small:
						chosenTile = smallXenoBloodTile;
						break;
					case BloodSplatSize.medium:
						chosenTile = medXenoBloodTile;
						break;
					case BloodSplatSize.large:
						chosenTile = largeXenoBloodTile;
						break;
					case BloodSplatSize.Random:
						int rand = Random.Range(0, 3);
						BloodSplat(worldPos, (BloodSplatSize)rand, bloodColorType);
						return;
				}
				break;
		}

		if (chosenTile != null)
		{
			Spawn.ServerPrefab(chosenTile, worldPos,
				MatrixManager.AtPoint(Vector3Int.RoundToInt(worldPos), true).Objects);
		}
	}

	/// <summary>
	/// Creates ash at the specified tile position
	/// </summary>
	/// <param name="worldTilePos"></param>
	/// <param name="large">if true, spawns the large ash pile, otherwise spawns the small one</param>
	public static void Ash(Vector2Int worldTilePos, bool large)
	{
		EnsureInit();
		Spawn.ServerPrefab(large ? largeAshTile : smallAshTile, worldTilePos.To3Int(),
			MatrixManager.AtPoint(worldTilePos.To3Int(), true).Objects);
	}

	public static void WaterSplat(Vector3 worldPos)
	{
		EnsureInit();
		Spawn.ServerPrefab(waterTile, worldPos,
			MatrixManager.AtPoint(Vector3Int.RoundToInt(worldPos), true).Objects, Quaternion.identity);
	}
}