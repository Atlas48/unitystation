﻿using UnityEngine;
using Mirror;


public abstract class TileInteraction
{
	protected readonly GameObject gameObject;
	protected readonly string hand;
	protected readonly GameObject originator;
	protected Vector3 position;

	public TileInteraction(GameObject gameObject, GameObject originator, Vector3 position, string hand)
	{
		this.gameObject = gameObject;
		this.originator = originator;
		this.position = position;
		this.hand = hand;
	}

	public void Interact(bool isServer)
	{
		if (isServer)
		{
			ServerAction();
		}
		else
		{
			ClientAction();
		}
	}

	public abstract void ClientAction();

	[Server]
	public abstract void ServerAction();
}