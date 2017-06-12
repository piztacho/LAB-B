// FRAGMENT SHADER PHONG.
#version 330

in vec3 vNE; 
in vec3 vVE;
in vec3 vLE; 

//Material Attrib
uniform vec3 ka;
uniform vec3 kd;
uniform vec3 ks;
uniform float CoefEsp;

uniform sampler2D gSampler;

out vec4 fColor;


uniform vec3 LightIa;
uniform vec3 LightId;
uniform vec3 LightIs;

in vec2 TexCoordOut;

void main(){

 // calcular los distintos vectores y normalizarlos
 vec3 N = normalize(vNE);
 vec3 L = normalize(vLE);
 vec3 R = reflect(L, N);
 vec3 V = normalize(vVE);
 vec3 H = normalize(L+V);


 // Calc térm difuso+espec de Phong
float difuso = max(dot(L,N), 0.0);
float specPhong = pow(max(dot(R, V), 0.0), CoefEsp);

if (dot(L,N) < 0.0) {
	specPhong = 0.0;
	}

vec4 Textura = texture(gSampler, TexCoordOut);
vec3 termTextura = vec3(Textura);

//vec3 colorV = ka + kd * difuso + termTextura + ks * specPhong ;

float distancia = length(vLE);
float a= 0.1;
float b= 0.001;
float c= 0.00001;
float atenuacion = min( 1.0 / (a+b*distancia + c*distancia*distancia),1.0);


vec3 colorV = ( ka * LightIa +   kd * LightId * difuso * atenuacion * termTextura + ks * specPhong * atenuacion * LightIs );

fColor = vec4( colorV, 1.0 );



}
