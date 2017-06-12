// FRAGMENT SHADER.
#version 330

//Vertex Attrib
in vec3 vPos;
in vec3 vNormal;
in vec2 TexCoord;

//Light Attrib
uniform vec4 posL;

//Material Attrib
uniform vec3 ka;
uniform vec3 kd;
uniform vec3 ks;
uniform float CoefEsp;

uniform sampler2D gSampler;

//Matrices
uniform mat4 projMat;
uniform mat4 viewMatrix;
uniform mat4 modelMat;
//uniform mat4 MV;
//uniform mat4 MVP;
uniform mat3 MNormal;


out vec3 colorV;

void main(){

		
	mat4 MV = viewMatrix * modelMat;

	//Transformar Normal de EspacioObjeto a EspacioOjo
	vec3 N = normalize(MNormal * vNormal); 

	//Transformar Posicion de EspacioObjeto a EspacioOjo
	vec4 vE = MV * vec4(vPos, 1.0);

	//Calcular Vector Luz en el EspacioOjo
	vec3 posLE = vec3(viewMatrix * posL);
	vec3 vLE = vec3(posLE - vE.xyz);	
	vec3 L = normalize(vLE);

	// Vector del vertice al ojo en Espacio del ojo
	vec3 V = normalize(-vE.xyz);

	vec3 R = normalize(reflect(L, N));
	

	// Calc térm difuso+espec de Phong
	float difuso = max(dot(L,N), 0.0);
	float specPhong = pow(max(dot(R, V), 0.0), CoefEsp);

	if (dot(L,N) < 0.0) {
		specPhong = 0.0;
	}
	vec4 Textura = texture2D(gSampler, TexCoord);
    vec3 termTextura = vec3(Textura);

	colorV = ka + kd * difuso  + ks * specPhong; //+ termTextura
	

	gl_Position = projMat * viewMatrix * modelMat * vec4(vPos, 1.0) + Textura - Textura;

}
