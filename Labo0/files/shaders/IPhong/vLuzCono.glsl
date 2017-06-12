// FRAGMENT SHADER.
#version 330



//Matrices
uniform mat4 projMat;
uniform mat4 viewMatrix;
uniform mat4 modelMat;


in vec2 TexCoord;
in vec3 vPos;
in vec3 vNormal;

out vec2 f_TexCoord;
out vec3 fragPos;
out vec3 fragNormal;

void main(){
	
	fragPos = vPos ;
	fragNormal = vNormal;
	gl_Position = projMat * viewMatrix * modelMat * vec4(vPos, 1.0);
	//f_TexCoord = vec2(TexCoord.s, 1 - TexCoord.t);
	f_TexCoord = TexCoord;
}

