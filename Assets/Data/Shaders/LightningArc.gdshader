shader_type canvas_item;
render_mode blend_add, unshaded;

/*
 * Lightning arc shader for the lightning wand's innate chain.
 *
 * Expects to sit on a Line2D using TEXTURE_STRETCH, so UV.x runs along the bolt
 * from source to target and UV.y runs across its width. The jagged path itself is
 * generated as Line2D geometry (see LightningArc.cs); this shader only handles the
 * hot core, the coloured glow falloff, the charge flowing along the bolt, and the
 * strike/fade animation.
 */

group_uniforms colour;
uniform vec4 core_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 glow_color : source_color = vec4(0.45, 0.65, 1.0, 1.0);
/** Higher values make the white centre thinner and hotter */
uniform float core_power : hint_range(1.0, 48.0) = 16.0;
/** Lower values spread the coloured glow further out from the centre */
uniform float glow_power : hint_range(0.5, 8.0) = 2.0;
uniform float glow_strength : hint_range(0.0, 4.0) = 1.5;
group_uniforms;

group_uniforms flow;
/** How fast charge races from source to target */
uniform float flow_speed : hint_range(0.0, 40.0) = 14.0;
/** Number of bright/dim bands along the bolt */
uniform float flow_scale : hint_range(1.0, 64.0) = 16.0;
uniform float flow_strength : hint_range(0.0, 1.0) = 0.5;
group_uniforms;

group_uniforms animation;
/** Leading edge of the strike, driven from 0 to 1 as the bolt lands */
uniform float progress : hint_range(0.0, 1.0) = 1.0;
uniform float strike_softness : hint_range(0.001, 0.5) = 0.12;
/** Overall brightness, driven down to 0 as the bolt dies */
uniform float fade : hint_range(0.0, 1.0) = 1.0;
/** Per-instance offset so simultaneous bolts don't flicker in lockstep */
uniform float seed = 0.0;
group_uniforms;

float hash11(float n)
{
	return fract(sin(n * 127.1 + seed * 311.7) * 43758.5453123);
}

float value_noise(float x)
{
	float i = floor(x);
	float f = fract(x);
	f = f * f * (3.0 - 2.0 * f);
	return mix(hash11(i), hash11(i + 1.0), f);
}

void fragment()
{
	// 1.0 along the centre line of the ribbon, falling to 0.0 at either edge
	float across = 1.0 - clamp(abs(UV.y - 0.5) * 2.0, 0.0, 1.0);

	float flow = value_noise(UV.x * flow_scale - TIME * flow_speed);
	float brightness = mix(1.0 - flow_strength, 1.0 + flow_strength, flow);

	// Wipes the bolt in from source to target so it reads as a strike, not a pop-in
	float strike = smoothstep(progress, progress - strike_softness, UV.x);

	float core = pow(across, core_power);
	float glow = pow(across, glow_power) * glow_strength;

	vec3 colour = (core_color.rgb * core + glow_color.rgb * glow) * brightness;
	float alpha = clamp(core + glow, 0.0, 1.0) * strike * fade;

	COLOR = vec4(colour, alpha);
}
