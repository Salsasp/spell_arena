using Godot;
using System.Collections.Generic;

public partial class DamageText : Node2D
{
	[Export] public Area2D area2D;
	[Export] public LifetimeComponent lifetimeComponent;
	[Export] public RichTextLabel label;
	[Export] public CollisionShape2D textCollider;

	[Export] public float FadeInDuration = 0.15f;
	[Export] public float RiseDuration = 0.2f;
	[Export] public float RiseDistance = 14f;
	[Export] public float FadeOutDuration = 0.3f;

	public bool IsDamaging = false;
	public string Text = "";
	public int Damage;
	public ProjectileSpawner projectileSpawner;
	public Node2D spawnAboveTarget;
	public int verticalOffset;
	public List<string> TextModifiers;
	public CanvasLayer canvasLayer;
	public TextManagerComponent textManagerComponent;

	private float elapsedTime;
	private float fadeOutElapsed;
	private bool isFadingOut;
	private bool isDismissed;

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
		if (IsDamaging || isDismissed)
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

		float dt = (float)delta;

		if (isFadingOut)
		{
			fadeOutElapsed += dt;
			float fadeT = FadeOutDuration > 0f
				? Mathf.Clamp(fadeOutElapsed / FadeOutDuration, 0f, 1f)
				: 1f;
			label.Modulate = new Color(1f, 1f, 1f, 1f - fadeT);
			UpdateLabelPosition(GetAnimatedWorldPosition());
			if (fadeT >= 1f)
			{
				label.QueueFree();
				QueueFree();
			}

			return;
		}

		elapsedTime += dt;

		float fadeInAlpha = FadeInDuration > 0f
			? Mathf.Clamp(elapsedTime / FadeInDuration, 0f, 1f)
			: 1f;
		label.Modulate = new Color(1f, 1f, 1f, fadeInAlpha);
		UpdateLabelPosition(GetAnimatedWorldPosition());

		if (lifetimeComponent.TimedOut)
		{
			isFadingOut = true;
			fadeOutElapsed = 0f;
		}
	}

	public void Dismiss()
	{
		if (isDismissed)
		{
			return;
		}

		isDismissed = true;
		textManagerComponent?.UnregisterActiveText(this);

		if (label != null && GodotObject.IsInstanceValid(label))
		{
			label.QueueFree();
		}

		QueueFree();
	}

	public override void _ExitTree()
	{
		textManagerComponent?.UnregisterActiveText(this);
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
		label.Modulate = new Color(1f, 1f, 1f, 0f);
		label.Reparent(canvasLayer);

		elapsedTime = 0f;
		fadeOutElapsed = 0f;
		isFadingOut = false;
		textManagerComponent?.RegisterActiveText(spawnAboveTarget, this);
		UpdateLabelPosition(GetAnimatedWorldPosition());
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

	private Vector2 GetAnimatedWorldPosition()
	{
		Vector2 headTop = GetHeadTopWorldPosition();

		float riseT = RiseDuration > 0f
			? Mathf.Clamp(elapsedTime / RiseDuration, 0f, 1f)
			: 1f;
		riseT = 1f - (1f - riseT) * (1f - riseT);

		float yOffset = verticalOffset + (1f - riseT) * RiseDistance;
		return new Vector2(headTop.X, headTop.Y + yOffset);
	}

	private Vector2 GetSpawnWorldPosition()
	{
		Vector2 headTop = GetHeadTopWorldPosition();
		return new Vector2(headTop.X, headTop.Y + verticalOffset);
	}

	private Vector2 GetHeadTopWorldPosition()
	{
		AnimatedSprite2D animSprite = spawnAboveTarget.GetNodeOrNull<AnimatedSprite2D>("EnemySprite")
			?? spawnAboveTarget.GetNodeOrNull<AnimatedSprite2D>("PlayerSprite");
		if (animSprite?.SpriteFrames != null)
		{
			Texture2D frameTexture = animSprite.SpriteFrames.GetFrameTexture(animSprite.Animation, animSprite.Frame);
			if (frameTexture != null)
			{
				float halfHeight = GetVisualHalfHeight(frameTexture.GetSize(), animSprite.GlobalTransform.Scale);
				Vector2 spritePos = animSprite.GlobalPosition;
				return new Vector2(spritePos.X, spritePos.Y - halfHeight);
			}
		}

		Sprite2D sprite = spawnAboveTarget.GetNodeOrNull<Sprite2D>("EnemySprite")
			?? spawnAboveTarget.GetNodeOrNull<Sprite2D>("PlayerSprite");
		if (sprite?.Texture != null)
		{
			float halfHeight = GetVisualHalfHeight(sprite.Texture.GetSize(), sprite.GlobalTransform.Scale);
			Vector2 spritePos = sprite.GlobalPosition;
			return new Vector2(spritePos.X, spritePos.Y - halfHeight);
		}

		CollisionShape2D collider = spawnAboveTarget.GetNodeOrNull<CollisionShape2D>("CollisionShape2D")
			?? spawnAboveTarget.GetNodeOrNull<CollisionShape2D>("Collider");
		if (collider?.Shape is RectangleShape2D rect)
		{
			float halfHeight = GetVisualHalfHeight(rect.Size, collider.GlobalTransform.Scale);
			Vector2 colliderPos = collider.GlobalPosition;
			return new Vector2(colliderPos.X, colliderPos.Y - halfHeight);
		}

		Vector2 bodyPos = spawnAboveTarget.GlobalPosition;
		return new Vector2(bodyPos.X, bodyPos.Y - 16f);
	}

	private static float GetVisualHalfHeight(Vector2 size, Vector2 globalScale)
	{
		return Mathf.Abs(size.Y * globalScale.Y) * 0.5f;
	}
}
