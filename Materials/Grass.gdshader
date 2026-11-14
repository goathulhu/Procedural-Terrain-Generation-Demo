// NOTE: Shader automatically converted from Godot Engine 4.7.1.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, diffuse_burley, specular_disabled;

uniform vec4 base_color : source_color;
uniform vec4 tip_color : source_color;

uniform float dither_min : hint_range(0.0, 4096.0, 0.01);
uniform float dither_max : hint_range(0.0, 4096.0, 0.01);

void fragment() {
	ALBEDO = mix(tip_color.rgb, base_color.rgb, UV.y);
	
	BACKLIGHT = vec3(1.0f, 1.0f, 1.0f);
	
	{
		// Distance Fade: Pixel Dither
		float fade_distance = length(VERTEX);

		// Use interleaved gradient noise, which is fast but still looks good.
		const vec3 magic = vec3(0.06711056, 0.00583715, 52.9829189);
		float fade = clamp(smoothstep(dither_min, dither_max, fade_distance), 0.0, 1.0);
		// Use a hard cap to prevent a few stray pixels from remaining when past the fade-out distance.
		if (fade < 0.001 || fade < fract(magic.z * fract(dot(FRAGCOORD.xy, magic.xy)))) {
			discard;
		}
	}
}
