using Godot;

// Shifts the camera's view toward whichever screen edge the mouse is near,
// giving a soft "edge pan" effect. Response near each edge is shaped by an
// easing curve (custom Curve resource, or a smoothstep default) and the
// resulting offset is smoothed over time so the pan never snaps.
public partial class CameraPanComponent : Component
{
	[Export] public float MaxPanDistance = 90f;
	[Export] public float EdgeMargin = 160f;
	[Export] public float SmoothingSpeed = 6f;
	[Export] public Curve PanCurve;
	public Camera2D ParentCamera;

	public override void _Ready()
	{
		ParentCamera = GetParent<Camera2D>();
	}

	public override void _Process(double delta)
	{
		Vector2 targetOffset = ComputeTargetOffset();
		float weight = 1f - Mathf.Exp(-SmoothingSpeed * (float)delta);
		ParentCamera.Offset = ParentCamera.Offset.Lerp(targetOffset, weight);
	}

	private Vector2 ComputeTargetOffset()
	{
		Viewport viewport = GetViewport();
		Vector2 viewportSize = viewport.GetVisibleRect().Size;
		Vector2 mousePos = viewport.GetMousePosition();

		float panX = ComputeAxisPan(mousePos.X, viewportSize.X);
		float panY = ComputeAxisPan(mousePos.Y, viewportSize.Y);

		return new Vector2(panX, panY);
	}

	private float ComputeAxisPan(float mouseCoord, float axisSize)
	{
		float margin = Mathf.Min(EdgeMargin, axisSize / 2f);
		if (margin <= 0f)
		{
			return 0f;
		}

		float distanceFromNearEdge = mouseCoord;
		float distanceFromFarEdge = axisSize - mouseCoord;

		if (distanceFromNearEdge < margin)
		{
			float t = 1f - Mathf.Clamp(distanceFromNearEdge / margin, 0f, 1f);
			return -ApplyCurve(t) * MaxPanDistance;
		}

		if (distanceFromFarEdge < margin)
		{
			float t = 1f - Mathf.Clamp(distanceFromFarEdge / margin, 0f, 1f);
			return ApplyCurve(t) * MaxPanDistance;
		}

		return 0f;
	}

	private float ApplyCurve(float t)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (PanCurve != null)
		{
			return PanCurve.Sample(t);
		}

		// Smoothstep: eases in gently near the dead zone, accelerates toward the edge.
		return t * t * (3f - 2f * t);
	}
}
