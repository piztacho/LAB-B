// VERTEX SHADER. Simple
// Transforma la posicion.
#version 150

in vec3 vPos;
//in vec3 vColor;

uniform mat4 projMat;
uniform mat4 mvMat;

//out vec4 fragColor;

void main(){
  //fragColor = vec4(vColor, 1.0)
  gl_Position = projMat * mvMat * vec4(vPos, 1.0);
}
