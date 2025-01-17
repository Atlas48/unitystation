﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System;


[CreateAssetMenu(fileName = "ContainerData", menuName = "ScriptableObjects/BackpackData", order = 1)]
public class ContainerData : BaseClothData
{
	public EquippedData Sprites;
	public StorageObjectData StorageData;

	public override void  InitializePool()
	{
		if (Spawn.BackpackStoredData.ContainsKey(this.name) && Spawn.BackpackStoredData[this.name] != this)
		{
			Logger.LogError("a ContainerData Has the same name as another one. name " + this.name + ". Please rename one of them to a different name");
		}
		Spawn.BackpackStoredData[this.name] = this;

	}
	public static void getContainerData(List<ContainerData> DataPCD)
	{
		DataPCD.Clear();
		var PCD = Resources.LoadAll<ContainerData>("textures/clothing");
		foreach (var PCDObj in PCD)
		{
			DataPCD.Add(PCDObj);
		}

		//string[] dirs = Directory.GetDirectories(Application.dataPath, "textures/clothing", SearchOption.AllDirectories); //could be changed later not to load everything to save start-up times

		//foreach (string dir in dirs)
		//{
		//	//Should yield For a frame to Increase performance

		//	loadFolder(dir, DataPCD);
		//	foreach (string subdir in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
		//	{
		//		loadFolder(subdir, DataPCD);
		//	}
		//}
	}

	private static void loadFolder(string folderpath, List<ContainerData> DataPCD)
	{
		folderpath = folderpath.Substring(folderpath.IndexOf("Resources", StringComparison.Ordinal) + "Resources".Length);
		foreach (var PCDObj in Resources.LoadAll<ContainerData>(folderpath))
		{
			if (!DataPCD.Contains(PCDObj))
			{
				DataPCD.Add(PCDObj);
			}
		}
	}
}
