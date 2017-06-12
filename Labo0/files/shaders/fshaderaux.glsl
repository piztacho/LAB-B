// FRAGMENT SHADER.
//
#version 150

//in vec4 fragColor;

uniform vec4 figureColor;

out vec4 fColor;

void main(){
  //fColor = fragColor;
  fColor = figureColor;
}
