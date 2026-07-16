using System.Collections.Generic;
using Godot;

public partial class TextManagerComponent : Component
{
	[Export] public int CRIT_TEXT_DURATION = 1;
	[Export] public PackedScene DamageTextPrefab;
	[Export] public ProjectileSpawner projectileSpawner;
	[Export] public NodePath ModifierManagerPath { get; set; } = "../Player/ModifierManager";
	[Export] public NodePath CanvasLayerPath { get; set; } = "../CanvasLayer";

	private ModifierManager modifierManager;
	private CanvasLayer canvasLayer;

	public override void _Ready()
	{
		base._Ready();
		modifierManager = ModifierManager.Instance ?? GetNodeOrNull<ModifierManager>(ModifierManagerPath);
		canvasLayer = GetNodeOrNull<CanvasLayer>(CanvasLayerPath);
	}

	public void InitComponents(ModifierManager manager)
	{
		modifierManager = manager;
	}

	public void DrawAbove(Node2D targetNode, int verticalOffset, string text, List<string> textModifiers, int damage = 0)
	{
		SpawnDamageText(targetNode, verticalOffset, text, textModifiers, damage);
	}

	public void InstantiateDamageText(Node2D targetNode, int verticalOffset, string text, List<string> textModifiers, int damage = 0)
	{
		SpawnDamageText(targetNode, verticalOffset, text, textModifiers, damage);
	}

	private void SpawnDamageText(Node2D targetNode, int verticalOffset, string text, List<string> textModifiers, int damage)
	{
		if (DamageTextPrefab == null)
		{
			GD.PrintErr($"{Name}: DamageTextPrefab is not set.");
			return;
		}

		if (modifierManager == null)
		{
			GD.PrintErr($"{Name}: ModifierManager not available.");
			return;
		}

		if (canvasLayer == null)
		{
			GD.PrintErr($"{Name}: CanvasLayer not available.");
			return;
		}

		DamageText damageText = DamageTextPrefab.Instantiate<DamageText>();
		bool isDamaging = modifierManager.ActiveModifiers.ContainsKey("damaging_text");
		damageText.IsDamaging = isDamaging;
		damageText.Text = text;
		damageText.Damage = damage;
		damageText.TextModifiers = textModifiers;
		damageText.verticalOffset = verticalOffset;
		damageText.spawnAboveTarget = targetNode;
		damageText.projectileSpawner = projectileSpawner;
		damageText.canvasLayer = canvasLayer;
		canvasLayer.CallDeferred(Node.MethodName.AddChild, damageText);
	}

	public static string GenerateBBText(string text, List<string> textModifiers)
	{
		if (textModifiers == null || textModifiers.Count == 0)
		{
			return text;
		}

		string result = text;
		foreach (string modifier in textModifiers)
		{
			if (modifier.StartsWith("color="))
			{
				string color = modifier.Substring("color=".Length);
				result = $"[color={color}]{result}[/color]";
			}
		}

		return result;
	}
}
