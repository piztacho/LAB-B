// VERTEX SHADER. Simple

#version 330

//in vec3 vCol;
uniform vec4 figureColor;
uniform mat4 projMat;
uniform mat4 viewMatrix;
uniform mat4 modelMat;

in vec3 vPos;
//in vec3 vNormal;
//out vec4 fragColor;

void main(){
// fragColor = figureColor;
 //fragColor = vec4(vPos.z +0.75, 0.6, 0.0, 1.0);
// fragColor = vec4(0, 1, 0, 1);
 //fragColor = figureColor + vec4(vNormal, 1.0) * 0;
  gl_Position = projMat * viewMatrix * modelMat * vec4(vPos, 1.0);
}
