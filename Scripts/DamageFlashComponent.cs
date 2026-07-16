using Godot;

public partial class DamageFlashComponent : Component
{
	[Export] public NodePath FlashTargetPath { get; set; }
	[Export] public NodePath HealthComponentPath { get; set; } = "HealthComponent";
	[Export] public float FlashDuration { get; set; } = 0.12f;
	[Export] public Color FlashColor { get; set; } = new(1.8f, 0.45f, 0.45f, 1f);

	private CanvasItem flashTarget;
	private HealthComponent healthComponent;
	private Color originalModulate;
	private int lastHealth;
	private Tween flashTween;

	public override void _Ready()
	{
		base._Ready();

		if (FlashTargetPath == null || FlashTargetPath.IsEmpty)
		{
			GD.PrintErr($"{owner.Name}/{Name}: FlashTargetPath must be set in the scene.");
			return;
		}

		flashTarget = owner.GetNodeOrNull<CanvasItem>(FlashTargetPath);
		healthComponent = owner.GetNodeOrNull<HealthComponent>(HealthComponentPath);

		if (flashTarget == null)
		{
			GD.PrintErr($"{owner.Name}/{Name}: no CanvasItem found at '{FlashTargetPath}'.");
			return;
		}

		if (healthComponent == null)
		{
			GD.PrintErr($"{owner.Name}/{Name}: no HealthComponent found at '{HealthComponentPath}'.");
			return;
		}

		originalModulate = flashTarget.Modulate;
		lastHealth = healthComponent.GetCurrentHealth();
		healthComponent.HealthChanged += OnHealthChanged;
	}

	private void OnHealthChanged(int newHealth)
	{
		if (flashTarget == null)
		{
			return;
		}

		if (newHealth < lastHealth)
		{
			Flash();
		}

		lastHealth = newHealth;
	}

	private void Flash()
	{
		if (flashTween != null && flashTween.IsValid())
		{
			flashTween.Kill();
		}

		flashTarget.Modulate = FlashColor;
		flashTween = CreateTween();
		flashTween.TweenProperty(flashTarget, "modulate", originalModulate, FlashDuration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
	}
}
