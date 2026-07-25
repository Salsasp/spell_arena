using Godot;

public partial class Util
{
	public enum OrbitTypes
{
	fireball,
	enemy
}

public static Enemy FindNearestEnemy(Node2D currentNode)
	{
		Vector2 pos = currentNode.GetParent<Node2D>().GlobalPosition;
		Enemy nearest = null;
		float minDistSq = float.MaxValue;

		foreach (Node node in currentNode.GetTree().GetNodesInGroup("enemies"))
		{
			if (node is not Enemy enemy || !GodotObject.IsInstanceValid(enemy))
			{
				continue;
			}

			float distSq = pos.DistanceSquaredTo(enemy.GlobalPosition);
			if (distSq < minDistSq)
			{
				minDistSq = distSq;
				nearest = enemy;
			}
		}

		return nearest;
	}
}