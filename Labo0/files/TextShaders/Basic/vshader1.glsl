// VERTEX SHADER. Simple con Textura
#version 330

in vec3 vPos;
in vec2 TexCoord;
in vec3 vNormal;

uniform mat4 projMat;
uniform mat4 viewMatrix;
uniform mat4 modelMat;

out vec2 TexCoordOut;

void main(){
	vec4 alPedo = vec4(vNormal,1.0);
	TexCoordOut = TexCoord;
	gl_Position = projMat * viewMatrix * modelMat * vec4(vPos, 1.0)+alPedo-alPedo;

}
