using UnityEngine;

public class RackParts : MonoBehaviour, ICheckedInteractable<PositionalHandApply>, ICheckedInteractable<InventoryApply>
{

	public GameObject rackPrefab;

	public bool WillInteract(PositionalHandApply interaction, NetworkSide side)
	{
		if (!DefaultWillInteract.Default(interaction, side))
		{
			return false;
		}

		if (Validations.HasItemTrait(interaction.HandObject, CommonTraits.Instance.Wrench))
		{
			return true;
		}

		// Must be constructing the rack somewhere empty
		var vector = interaction.WorldPositionTarget.RoundToInt();
		if (!MatrixManager.IsPassableAt(vector, vector, false))
		{
			return false;
		}

		return true;
	}

	public bool WillInteract(InventoryApply interaction, NetworkSide side)
	{
		if (!DefaultWillInteract.Default(interaction, side))
		{
			return false;
		}

		if (interaction.TargetObject != gameObject
		    || !Validations.HasItemTrait(interaction.HandObject, CommonTraits.Instance.Wrench))
		{
			return false;
		}

		return true;
	}

	public void ServerPerformInteraction(PositionalHandApply interaction)
	{
		if (Validations.HasItemTrait(interaction.HandObject, CommonTraits.Instance.Wrench))
		{
			SoundManager.PlayNetworkedAtPos("Wrench", interaction.WorldPositionTarget, 1f);
			Spawn.ServerPrefab("Metal", interaction.WorldPositionTarget.RoundToInt(), transform.parent, count: 1,
				scatterRadius: Spawn.DefaultScatterRadius, cancelIfImpassable: true);
			Despawn.ServerSingle(gameObject);

			return;
		}

		var progressFinishAction = new ProgressCompleteAction(() =>
			{
				Chat.AddExamineMsgFromServer(interaction.Performer,
						"You assemble a rack.");
				Spawn.ServerPrefab(rackPrefab, interaction.WorldPositionTarget.RoundToInt(),
					interaction.Performer.transform.parent);
				var handObj = interaction.HandObject;
				Inventory.ServerDespawn(interaction.HandSlot);
			}
		);

		var bar = UIManager.ServerStartProgress(ProgressAction.Construction, interaction.WorldPositionTarget.RoundToInt(),
			5f, progressFinishAction, interaction.Performer);
		if (bar != null)
		{
			Chat.AddExamineMsgFromServer(interaction.Performer, "You start constructing a rack...");
		}
	}

	public void ServerPerformInteraction(InventoryApply interaction)
	{
		SoundManager.PlayNetworkedAtPos("Wrench", interaction.Performer.WorldPosServer(), 1f);
		Spawn.ServerPrefab("Metal", interaction.Performer.WorldPosServer().CutToInt(), transform.parent, count: 1,
			scatterRadius: Spawn.DefaultScatterRadius, cancelIfImpassable: true);
		Inventory.ServerDespawn(interaction.HandSlot);
	}
}
