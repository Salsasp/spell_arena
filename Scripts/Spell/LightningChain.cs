using Godot;
using System.Collections.Generic;

/// <summary>
/// Resolves the lightning wand's innate arc chain. Every hop is found, damaged and
/// drawn within a single frame, so the lightning appears to connect through a cluster
/// of enemies at once. This deliberately does not touch the projectile - unlike the
/// chain_spells modifier, which redirects the projectile itself one bounce at a time.
/// </summary>
public static class LightningChain
{
	private const int BASE_JUMPS = 3;
	/// <summary> Extra enemies reached per stack beyond the first. </summary>
	private const int JUMPS_PER_EXTRA_STACK = 1;
	private const float ARC_RADIUS = 180f;
	/// <summary> Share of the projectile's damage dealt by the first arc at one stack. </summary>
	private const float BASE_ARC_DAMAGE_RATIO = 0.5f;
	/// <summary> Added to that share per stack beyond the first. </summary>
	private const float ARC_DAMAGE_RATIO_PER_EXTRA_STACK = 0.25f;
	/// <summary> Damage retained by each arc relative to the one before it. </summary>
	private const float ARC_FALLOFF = 0.7f;
	private const string ARC_SCENE_PATH = "res://Scenes/Spells/FX/LightningArc.tscn";

	private static PackedScene arcScene;

	/// <summary> How many enemies the chain reaches at the given stack count. </summary>
	public static int GetJumpCount(int copies)
	{
		return BASE_JUMPS + (ExtraStacks(copies) * JUMPS_PER_EXTRA_STACK);
	}

	/// <summary> Share of the projectile's damage dealt by the first arc at the given stack count. </summary>
	public static float GetFirstArcDamageRatio(int copies)
	{
		return BASE_ARC_DAMAGE_RATIO + (ExtraStacks(copies) * ARC_DAMAGE_RATIO_PER_EXTRA_STACK);
	}

	private static int ExtraStacks(int copies)
	{
		return Mathf.Max(0, copies - 1);
	}

	/// <param name="copies">Equipped stacks of the innate. Each stack past the first adds a jump and raises arc damage.</param>
	public static void Strike(Node2D origin, DamageComponent damage, int copies = 1)
	{
		if (origin == null || damage == null)
		{
			return;
		}

		int maxJumps = GetJumpCount(copies);
		float ratio = GetFirstArcDamageRatio(copies);

		SceneTree tree = origin.GetTree();
		if (tree == null)
		{
			return;
		}

		HashSet<Node2D> struck = [origin];
		Vector2 arcFrom = origin.GlobalPosition;

		for (int jump = 0; jump < maxJumps; jump++)
		{
			Enemy target = FindNearestUnstruck(tree, arcFrom, struck);
			if (target == null)
			{
				return;
			}

			// Captured before damage lands, since a lethal hit queues the enemy for deletion
			Vector2 arcTo = target.GlobalPosition;
			struck.Add(target);

			SpawnArc(tree, arcFrom, arcTo);

			bool isCrit = damage.IsCriticalHit();
			int arcDamage = Mathf.Max(1, Mathf.RoundToInt(damage.CalculateDamage(isCrit) * ratio));
			target.ApplyDamage(arcDamage, isCrit, true, false);

			arcFrom = arcTo;
			ratio *= ARC_FALLOFF;
		}
	}

	private static Enemy FindNearestUnstruck(SceneTree tree, Vector2 from, HashSet<Node2D> struck)
	{
		Enemy nearest = null;
		float nearestDistance = ARC_RADIUS;

		foreach (Node node in tree.GetNodesInGroup("enemies"))
		{
			if (node is not Enemy enemy || enemy.IsQueuedForDeletion() || struck.Contains(enemy))
			{
				continue;
			}

			float distance = from.DistanceTo(enemy.GlobalPosition);
			if (distance < nearestDistance)
			{
				nearest = enemy;
				nearestDistance = distance;
			}
		}

		return nearest;
	}

	private static void SpawnArc(SceneTree tree, Vector2 from, Vector2 to)
	{
		arcScene ??= GD.Load<PackedScene>(ARC_SCENE_PATH);
		if (arcScene == null || tree.CurrentScene == null)
		{
			return;
		}

		LightningArc arc = arcScene.Instantiate<LightningArc>();
		tree.CurrentScene.AddChild(arc);
		arc.Setup(from, to);
	}
}
