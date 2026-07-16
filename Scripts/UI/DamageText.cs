using Godot;
using System.Collections.Generic;

public partial class DamageText : Node2D
{
	[Export] public Area2D area2D;
	[Export] public LifetimeComponent lifetimeComponent;
	[Export] public RichTextLabel label;
	[Export] public CollisionShape2D textCollider;

	public bool IsDamaging = false;
	public string Text = "";
	public int Damage;
	public ProjectileSpawner projectileSpawner;
	public Node2D spawnAboveTarget;
	public int verticalOffset;
	public List<string> TextModifiers;
	public CanvasLayer canvasLayer;

	[Export] public float TrackDelay = 0.2f;


	public override void _Ready()
	{
		if (IsDamaging)
		{
			SpawnDamagingText();
		}
		else
		{
			SpawnText();
		}
	}

	public override void _Process(double delta)
	{
		if (IsDamaging)
		{
			return;
		}

		if (label == null)
		{
			return;
		}

		if (spawnAboveTarget == null || !GodotObject.IsInstanceValid(spawnAboveTarget))
		{
			label.QueueFree();
			QueueFree();
			return;
		}
		UpdateLabelPosition(GetSpawnWorldPosition());

		if (lifetimeComponent.TimedOut)
		{
			label.QueueFree();
			QueueFree();
		}
	}

	public void SpawnDamagingText()
	{
		if (projectileSpawner == null || spawnAboveTarget == null)
		{
			GD.PrintErr("Error with public field in DamageText");
			return;
		}

		if (string.IsNullOrEmpty(Text))
		{
			return;
		}

		area2D.Visible = false;
		label.Visible = false;

		int damagePerLetter = Mathf.Max(1, Damage / Text.Length);
		const int letterSpacing = 12;
		Vector2 anchorPos = GetSpawnWorldPosition();
		float totalWidth = letterSpacing * (Text.Length - 1);
		float startX = anchorPos.X - totalWidth * 0.5f;
		Vector2 basePos = new Vector2(startX, anchorPos.Y);

		for (int i = 0; i < Text.Length; i++)
		{
			string projText = TextManagerComponent.GenerateBBText(Text[i].ToString(), TextModifiers);
			Vector2 spawnPos = basePos + new Vector2(i * letterSpacing, 0);
			projectileSpawner.SpawnTextProjectile(spawnPos, Vector2.Zero, projText, damagePerLetter);
		}

		QueueFree();
	}

	public void SpawnText()
	{
		if (spawnAboveTarget == null || label == null)
		{
			GD.PrintErr("DamageText missing spawn target or label.");
			return;
		}

		if (canvasLayer == null)
		{
			GD.PrintErr("DamageText missing CanvasLayer reference.");
			return;
		}

		area2D.Visible = false;

		label.BbcodeEnabled = true;
		label.Text = TextManagerComponent.GenerateBBText(Text, TextModifiers);
		label.MouseFilter = Control.MouseFilterEnum.Ignore;
		label.Visible = true;
		label.Reparent(canvasLayer);
		UpdateLabelPosition(GetSpawnWorldPosition());
	}

	private void UpdateLabelPosition(Vector2 worldPos)
	{
		if (label == null || spawnAboveTarget == null || !GodotObject.IsInstanceValid(spawnAboveTarget))
		{
			return;
		}

		Vector2 canvasPos = GetViewport().GetCanvasTransform() * worldPos;
		canvasPos.X -= label.Size.X * 0.5f;
		label.Position = canvasPos;
	}

	private Vector2 GetSpawnWorldPosition()
	{
		Vector2 pos = spawnAboveTarget.GlobalPosition;
		float halfHeight = GetTargetHalfHeight();
		return new Vector2(pos.X, pos.Y - halfHeight + verticalOffset);
	}

	private float GetTargetHalfHeight()
	{
		Sprite2D sprite = spawnAboveTarget.GetNodeOrNull<Sprite2D>("EnemySprite")
			?? spawnAboveTarget.GetNodeOrNull<Sprite2D>("PlayerSprite");
		if (sprite?.Texture != null)
		{
			Vector2 textureSize = sprite.Texture.GetSize();
			Vector2 globalScale = sprite.GlobalTransform.Scale;
			return Mathf.Abs(textureSize.Y * globalScale.Y) * 0.5f;
		}

		CollisionShape2D collider = spawnAboveTarget.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collider?.Shape is RectangleShape2D rect)
		{
			Vector2 globalScale = collider.GlobalTransform.Scale;
			return Mathf.Abs(rect.Size.Y * globalScale.Y) * 0.5f;
		}

		return 16f;
	}
}
