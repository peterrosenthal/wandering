// shader inspired by https://gitlab.com/arcane_hive/terrain-generator-godot
shader_type spatial;

uniform float slope_factor = 8.0;

uniform sampler2D grass_texture;
uniform vec2 grass_scale;

uniform sampler2D dirt_texture;
uniform vec2 dirt_scale;

varying float height_val;
varying vec3 normal;

void vertex() {
	height_val = VERTEX.y;
	normal = NORMAL;
}

float get_slope_of_terrain(float height_normal) {
	float slope = 1.0 - height_normal;
	slope *= slope;
	return (slope*slope_factor);
}

void fragment() {
	vec3 dirt = vec3(texture(dirt_texture, UV * dirt_scale).rgb) * 0.35;
	vec3 grass = vec3(texture(grass_texture, UV * grass_scale).rgb) * 0.5;
	
	float slope = clamp(get_slope_of_terrain(normal.y), 0.0, 1.0);
	
	ALBEDO = mix(grass, dirt, slope);
}