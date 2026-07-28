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
		Vector2 pos = currentNode.GlobalPosition;
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

	public static int GetDirectionIndex8D(Vector2 vec)
    {
        if (vec == Vector2.Zero)
            return 0;

        float angle = vec.Angle();
        int index = Mathf.RoundToInt(angle / (Mathf.Pi / 4.0f));
        index = ((index % 8) + 8) % 8;
        return index;
    }

}