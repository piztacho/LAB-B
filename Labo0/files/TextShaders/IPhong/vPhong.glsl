// FRAGMENT SHADER.
#version 330

//Vertex Attrib
in vec3 vPos;
in vec3 vNormal;
in vec2 TexCoord;

//Matrices
uniform mat4 projMat;
uniform mat4 viewMatrix;
uniform mat4 modelMat;
uniform mat3 MNormal;

//Light Attrib
uniform vec4 posL; // Posicion de la luz en coordenadas del mundo

// Examples of variables passed from vertex to fragment shader
out vec3 vNE; //vector normal en espacio del ojo
out vec3 vLE; // vector de dir de luz
out vec3 vVE; // vector de vista (al ojo)

//Out Textura
out vec2 TexCoordOut;

void main(){	
	
	TexCoordOut = TexCoord; 

	vec4 Text = vec4(TexCoord,1.0,1.0);
	//Transformar Normal de EspacioObjeto a EspacioOjo
	vNE = normalize(MNormal * vNormal); 

	mat4 MV = viewMatrix * modelMat;
	
	//Transformar Posicion de EspacioObjeto a EspacioOjo
	vec4 vE = MV * vec4(vPos, 1.0);

	//Calcular Vector Luz en el EspacioOjo
	vec3 posLE = vec3(viewMatrix * posL);
	vLE = vec3(posLE - vE.xyz);	
	
	// Vector del vertice al ojo en Espacio del ojo
	vVE = normalize(-vE.xyz); 
	
	
	gl_Position = projMat * viewMatrix * modelMat * vec4(vPos, 1.0); 

}
