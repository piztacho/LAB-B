// FRAGMENT SHADER.
#version 330

uniform sampler2D gSampler;

in vec2 TexCoordOut;

out vec4 fColor;

void main(){
	fColor = texture2D(gSampler, TexCoordOut);
}
