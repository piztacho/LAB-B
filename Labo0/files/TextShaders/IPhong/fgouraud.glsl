// FRAGMENT SHADER.
#version 330



in vec3 colorV;

out vec4 fColor;

void main(){
	
	fColor =  vec4(colorV, 1.0) ;
}
