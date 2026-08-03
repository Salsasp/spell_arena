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
	private readonly Dictionary<ulong, DamageText> activeTextByTarget = new();

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
		DismissActiveText(targetNode);
		SpawnDamageText(targetNode, verticalOffset, text, textModifiers, damage);
	}

	public void RegisterActiveText(Node2D targetNode, DamageText damageText)
	{
		if (damageText == null || targetNode == null || !GodotObject.IsInstanceValid(targetNode))
		{
			return;
		}

		activeTextByTarget[targetNode.GetInstanceId()] = damageText;
	}

	public void UnregisterActiveText(DamageText damageText)
	{
		if (damageText == null)
		{
			return;
		}

		foreach (KeyValuePair<ulong, DamageText> entry in activeTextByTarget)
		{
			if (entry.Value != damageText)
			{
				continue;
			}

			activeTextByTarget.Remove(entry.Key);
			return;
		}
	}

	private void DismissActiveText(Node2D targetNode)
	{
		if (targetNode == null || !GodotObject.IsInstanceValid(targetNode))
		{
			return;
		}

		ulong targetId = targetNode.GetInstanceId();
		if (!activeTextByTarget.TryGetValue(targetId, out DamageText existing))
		{
			return;
		}

		activeTextByTarget.Remove(targetId);
		if (GodotObject.IsInstanceValid(existing))
		{
			existing.Dismiss();
		}
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
		bool isDamaging = RunStatBuilder.HasDamagingText(modifierManager.ActiveModifiers.Values);
		damageText.IsDamaging = isDamaging;
		damageText.Text = text;
		damageText.Damage = damage;
		damageText.TextModifiers = textModifiers;
		damageText.verticalOffset = verticalOffset;
		damageText.spawnAboveTarget = targetNode;
		damageText.projectileSpawner = projectileSpawner;
		damageText.canvasLayer = canvasLayer;
		damageText.textManagerComponent = this;
		canvasLayer.CallDeferred(Node.MethodName.AddChild, damageText);
	}

	public static string GenerateBBText(string text, List<string> textModifiers)
	{
		if (textModifiers == null || textModifiers.Count == 0)
		{
			return text;
		}

		string prefix = "";
		string suffix = "";
		foreach (string modifier in textModifiers)
		{
			// Color is unique in that it takes a provided value
			if (modifier.StartsWith("color="))
			{
				string color = modifier.Substring("color=".Length);
				prefix = $"[color={color}]{prefix}";
				suffix = $"{suffix}[/color]";
			}
			else
			{
				prefix = $"[{modifier}]{prefix}";
				suffix = $"{suffix}[/{modifier}]";
			}
		}

		return prefix + text + suffix;
	}
}
