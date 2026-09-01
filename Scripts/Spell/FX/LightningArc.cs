using Godot;

/// <summary>
/// A single one-shot bolt of lightning drawn between two points. The jagged path is
/// built with recursive midpoint displacement and regenerated a few times over the
/// bolt's short life, which produces the flickering shape; LightningArc.gdshader
/// handles the hot core, glow and the charge flowing along it.
/// </summary>
public partial class LightningArc : Node2D
{
	[Export] public Line2D Bolt;
	[Export] public PointLight2D ImpactLight;
	[Export] public GpuParticles2D ImpactSparks;

	/// <summary> Path resolution. n levels produce (2^n)+1 points. </summary>
	private const int SubdivisionLevels = 4;
	/// <summary> Initial sideways wander, as a fraction of the distance covered. </summary>
	private const float DisplacementRatio = 0.16f;
	private const float MaxDisplacement = 28f;
	private const float Lifetime = 0.22f;
	private const float StrikeDuration = 0.05f;
	private const float ReshapeInterval = 0.045f;
	private const float ImpactLightEnergy = 1.8f;

	private readonly RandomNumberGenerator rng = new();
	private ShaderMaterial boltMaterial;
	private Vector2 boltStart;
	private Vector2 boltEnd;
	private float elapsed;
	private float nextReshape;

	public override void _Ready()
	{
		boltMaterial = Bolt.Material as ShaderMaterial;
		SetProcess(false);
	}

	/// <summary> Aims the bolt between two global positions. Call after adding to the tree. </summary>
	public void Setup(Vector2 from, Vector2 to)
	{
		rng.Randomize();
		GlobalPosition = from;
		boltStart = Vector2.Zero;
		boltEnd = ToLocal(to);

		boltMaterial?.SetShaderParameter("seed", rng.Randf() * 100f);
		boltMaterial?.SetShaderParameter("progress", 0f);
		boltMaterial?.SetShaderParameter("fade", 1f);

		Reshape();

		if (ImpactLight != null)
		{
			ImpactLight.Position = boltEnd;
			ImpactLight.Energy = ImpactLightEnergy;
		}

		if (ImpactSparks != null)
		{
			ImpactSparks.Position = boltEnd;
			ImpactSparks.Restart();
		}

		SetProcess(true);
	}

	public override void _Process(double delta)
	{
		elapsed += (float)delta;

		if (elapsed >= Lifetime)
		{
			QueueFree();
			return;
		}

		float progress = Mathf.Min(elapsed / StrikeDuration, 1f);
		float decay = Mathf.Clamp((elapsed - StrikeDuration) / (Lifetime - StrikeDuration), 0f, 1f);
		float fade = 1f - (decay * decay);

		boltMaterial?.SetShaderParameter("progress", progress);
		boltMaterial?.SetShaderParameter("fade", fade);

		if (ImpactLight != null)
		{
			// The impact only lights up once the bolt has actually reached the target
			ImpactLight.Energy = ImpactLightEnergy * fade * progress;
		}

		nextReshape -= (float)delta;
		if (nextReshape <= 0f)
		{
			nextReshape = ReshapeInterval;
			Reshape();
		}
	}

	private void Reshape()
	{
		int pointCount = (1 << SubdivisionLevels) + 1;
		Vector2[] points = new Vector2[pointCount];
		points[0] = boltStart;
		points[pointCount - 1] = boltEnd;

		Vector2 span = boltEnd - boltStart;
		if (span.LengthSquared() <= 0f)
		{
			Bolt.Points = points;
			return;
		}

		Vector2 normal = span.Normalized().Orthogonal();
		float displacement = Mathf.Min(span.Length() * DisplacementRatio, MaxDisplacement);

		for (int step = pointCount - 1; step > 1; step /= 2)
		{
			int half = step / 2;
			for (int i = half; i < pointCount; i += step)
			{
				Vector2 midpoint = (points[i - half] + points[i + half]) * 0.5f;
				points[i] = midpoint + (normal * rng.RandfRange(-displacement, displacement));
			}

			displacement *= 0.5f;
		}

		Bolt.Points = points;
	}
}
