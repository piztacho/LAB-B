using System;
using System.Collections.Generic;
using OpenTK;
using System.Text;
using CGUNS.Shaders;
using OpenTK.Graphics.OpenGL;
using gl = OpenTK.Graphics.OpenGL.GL;


namespace CGUNS.Meshes.FaceVertexList
{
    public class FVLMesh2
    {
        private List<FVLFace> faceList;
        private List<Vector3> vertexList;
        private List<Vector2> texCordList;
        private List<Vector3> vertexNormalList;

        private int[] indices;  //Los indices para formar las caras.
        private int[] indicesNormales;  //Los indices para dibujar las normales
        private Vector3[] vectoresyN;

        private int h_VBO; //Handle del Vertex Buffer Object (posiciones de los vertices)
        private int h_EBO; //Handle del Elements Buffer Object (indices)
        private int h_VAO; //Handle del Vertex Array Object (Configuracion de los dos anteriores)

        private int h_VBON; //Handle del Vertex Array Object para dibujar los normales
        private int h_EBON; //Handle del Elements Buffer Object (indices)
        private int h_VAON; //Handle del Vertex Array Object 

        public FVLMesh2()
        {
            faceList = new List<FVLFace>();
            vertexList = new List<Vector3>();
            vertexNormalList = new List<Vector3>();
            texCordList = new List<Vector2>();

        }

        public List<Vector3> VertexList
        {
            get { return vertexList; }
        }

        public List<FVLFace> FaceList
        {
            get { return faceList; }
        }

        public List<Vector3> VertexNormalList
        {
            get { return vertexNormalList; }
        }

        public List<Vector2> TexCordList
        {
            get { return texCordList; }
        }

        public int VertexCount
        {
            get { return vertexList.Count; }
        }

        public int FaceCount
        {
            get { return faceList.Count; }
        }

        public int NormalCount
        {
            get { return vertexNormalList.Count; }
        }

        public int AddVertex(Vector3 vertex)
        {
            vertexList.Add(vertex);
            return vertexList.Count - 1;
        }

        public int AddVertexNormal(Vector3 normal)
        {
            vertexNormalList.Add(normal);
            return vertexNormalList.Count - 1;
        }

        public int AddTexCord(Vector2 texCord)
        {
            texCordList.Add(texCord);
            return texCordList.Count - 1;
        }

        public int AddFace(FVLFace face)
        {
            faceList.Add(face);
            return faceList.Count - 1;
        }

        public void PrintLists()
        {
            String sender = "FVLMesh.printLists: ";
            FVLFace face;
            List<int> faceVertexes;
            log(sender, "Vertex List has {0} items.", vertexList.Count);
            for (int i = 0; i < vertexList.Count; i++)
            {
                log("", "V[{0}] = ({1}, {2}, {3})", i, vertexList[i].X, vertexList[i].Y, vertexList[i].Z);
            }
            int cantFaces = faceList.Count;
            log(sender, "Face List has {0} items.", cantFaces);
            for (int i = 0; i < cantFaces; i++)
            {
                face = faceList[i];
                faceVertexes = face.VertexIndexes;
                String format = "F[{0}] = ";
                for (int j = 0; j < faceList[i].VertexCount; j++)
                {
                    format = format + " V[" + faceVertexes[j] + "],";
                }
                log("", format, i);
            }
            log(sender, "End!");
        }

        private void log(String sender, String format, params Object[] args)
        {
            Console.Out.WriteLine(sender + format, args);
        }

        public void Build(ShaderProgram sProgram, int inicioIndices = 0, int inicioNormales = 0)
        {
            log("FVLMesh.printLists: ", "llego");
            if (inicioIndices > 0)
                AcomodarIndices(inicioIndices, inicioNormales); //Solo se usa si el obj quiere dividirse en objetos

            CrearVBOs();
            CrearVAO(sProgram);

            CrearVBOsNormales();
            CrearVAONormales(sProgram);
        }

        void AcomodarIndices(int inicioIndices, int inicioNormales)
        { //Se acomodan los indices de los vertices. Si hay normales, tambien se acomodan. Solo se usa para obj dividido en partes
            int cantFaces = faceList.Count;

            for (int f = 0; f < cantFaces; f++)
            {
                FVLFace cara = faceList[f];
                int cuantosVertices = cara.VertexCount;
                for (int j = 0; j < cuantosVertices; j++)
                {
                    cara.VertexIndexes[j] = cara.VertexIndexes[j] - inicioIndices;
                    if (vertexNormalList.Count > 0)
                        cara.NormalIndexes[j] = cara.NormalIndexes[j] - inicioNormales;
                }

            }



        }

        private void CrearVBOs()
        {
            BufferTarget bufferType; //Tipo de buffer (Array: datos, Element: indices)
            IntPtr size;             //Tamanio (EN BYTES!) del buffer.
                                     //Hint para que OpenGl almacene el buffer en el lugar mas adecuado.
                                     //Por ahora, usamos siempre StaticDraw (buffer solo para dibujado, que no se modificara)
            BufferUsageHint hint = BufferUsageHint.StaticDraw;

            //VBO con el atributo "posicion" de los vertices.
            bufferType = BufferTarget.ArrayBuffer;

            size = new IntPtr(VertexList.Count * Vector3.SizeInBytes);
            h_VBO = gl.GenBuffer();  //Le pido un Id de buffer a OpenGL
            gl.BindBuffer(bufferType, h_VBO); //Lo selecciono como buffer de Datos actual.

            Vector3[] posiciones = new Vector3[VertexCount];
            posiciones = VertexList.ToArray();

            Vector3[] normales = new Vector3[VertexNormalList.Count];

            gl.BufferData<Vector3>(bufferType, size, posiciones, hint); //Lo lleno con la info.
            gl.BindBuffer(bufferType, 0); // Lo deselecciono (0: ninguno)

            //VBO con otros atributos de los vertices (color, normal, textura, etc).
            //Se pueden hacer en distintos VBOs o en el mismo.

            //EBO, buffer con los indices.
            bufferType = BufferTarget.ElementArrayBuffer;
            size = new IntPtr(faceList.Count * sizeof(int));
            h_EBO = gl.GenBuffer();
            indices = CarasToIndices();
            size = new IntPtr(indices.Length * sizeof(int));
            gl.BindBuffer(bufferType, h_EBO); //Lo selecciono como buffer de elementos actual.
            gl.BufferData<int>(bufferType, size, indices, hint);
            gl.BindBuffer(bufferType, 0);
        }

        private void CrearVBOsNormales()
        {
            BufferTarget bufferType; //Tipo de buffer (Array: datos, Element: indices)
            IntPtr size;             //Tamanio (EN BYTES!) del buffer.
                                     //Hint para que OpenGl almacene el buffer en el lugar mas adecuado.
                                     //Por ahora, usamos siempre StaticDraw (buffer solo para dibujado, que no se modificara)
            BufferUsageHint hint = BufferUsageHint.StaticDraw;

            Vector3[] vertices = new Vector3[VertexCount];
            vertices = VertexList.ToArray();


            int cantNormales = vertexNormalList.Count;
            if (cantNormales > 0)
            {
                Vector3[] normales = new Vector3[cantNormales];
                normales = vertexNormalList.ToArray();

                ArmarArreglosNormales(vertices, normales); //Arma el VBO y EBO de las normales. Crea vectoresyN e indicesNormales


                bufferType = BufferTarget.ArrayBuffer;
                size = new IntPtr(vectoresyN.Length * Vector3.SizeInBytes);
                h_VBON = gl.GenBuffer();  //Le pido un Id de buffer a OpenGL
                gl.BindBuffer(bufferType, h_VBON); //Lo selecciono como buffer de Datos actual.
                gl.BufferData<Vector3>(bufferType, size, vectoresyN, hint); //Lo lleno con la info.
                gl.BindBuffer(bufferType, 0); // Lo deselecciono (0: ninguno)


                //EBO, buffer con los indicesNormales para las normales.
                bufferType = BufferTarget.ElementArrayBuffer;
                h_EBON = gl.GenBuffer();
                size = new IntPtr(indicesNormales.Length * sizeof(int));
                gl.BindBuffer(bufferType, h_EBON); //Lo selecciono como buffer de elementos actual.
                gl.BufferData<int>(bufferType, size, indicesNormales, hint);
                gl.BindBuffer(bufferType, 0);

            }



        }

        private void ArmarArreglosNormales(Vector3[] posiciones, Vector3[] normales)
        {/*Arma el arreglo con los vectores que va al VBO para normales y el arreglo con indices de normales para el EBO */

            int cantFaces = faceList.Count;

            vectoresyN = new Vector3[cantFaces * 6]; //3 vertices y 3 normales por cara
            indicesNormales = new int[cantFaces * 6];

            int i = 0, IndiceV, IndiceN;
            for (int f = 0; f < cantFaces; f++)
            {
                FVLFace cara = faceList[f];
                int[] verticesCara = cara.IndicesDeCara();
                int[] normalesCara = cara.IndicesDeNormales();
                int cuantosVertices = cara.VertexCount;
                for (int j = 0; j < cuantosVertices; j++)
                {
                    IndiceV = verticesCara[j];
                    IndiceN = normalesCara[j];
                    vectoresyN[i] = posiciones[IndiceV];
                    vectoresyN[i + 1] = normales[IndiceN] * 5 + posiciones[IndiceV];
                    i = i + 2;
                }
            }
            //Organizo el arreglo que va en VBON poniendo primero todos los vertices y despues todos los vertices+normales

            for (i = 0; i < indicesNormales.Length; i++)
            {
                indicesNormales[i] = i;
            }


        }

        private int[] CarasToIndices()
        {

            int cantFaces = faceList.Count;

            int[] arrayIndices = new int[cantFaces * 3]; //OJO solo si  TODAS las caras son triágulos

            int i = 0;
            for (int f = 0; f < cantFaces; f++)
            {
                FVLFace cara = faceList[f];
                int[] indicesCara = cara.IndicesDeCara();
                int cuantosVertices = cara.VertexCount;
                for (int j = 0; j < cuantosVertices; j++)
                {
                    arrayIndices[i] = indicesCara[j];
                    i++;
                }

            }
            return arrayIndices;
        }
        private void CrearVAO(ShaderProgram sProgram)
        {
            // Indice del atributo a utilizar. Este indice se puede obtener de tres maneras:
            // Supongamos que en nuestro shader tenemos un atributo: "in vec3 vPos";
            // 1. Dejar que OpenGL le asigne un indice cualquiera al atributo, y para consultarlo hacemos:
            //    attribIndex = gl.GetAttribLocation(programHandle, "vPos") DESPUES de haberlo linkeado.
            // 2. Nosotros le decimos que indice queremos que le asigne, utilizando:
            //    gl.BindAttribLocation(programHandle, desiredIndex, "vPos"); ANTES de linkearlo.
            // 3. Nosotros de decimos al preprocesador de shader que indice queremos que le asigne, utilizando
            //    layout(location = xx) in vec3 vPos;
            //    En el CODIGO FUENTE del shader (Solo para #version 330 o superior)      
            int attribIndex;
            int cantComponentes; //Cantidad de componentes de CADA dato.
            VertexAttribPointerType attribType; // Tipo de CADA una de las componentes del dato.
            int stride; //Cantidad de BYTES que hay que saltar para llegar al proximo dato. (0: Tightly Packed, uno a continuacion del otro)
            int offset; //Offset en BYTES del primer dato.
            BufferTarget bufferType; //Tipo de buffer.

            // 1. Creamos el VAO
            h_VAO = gl.GenVertexArray(); //Pedimos un identificador de VAO a OpenGL.
            gl.BindVertexArray(h_VAO);   //Lo seleccionamos para trabajar/configurar.

            //2. Configuramos el VBO de posiciones.
            attribIndex = sProgram.GetVertexAttribLocation("vPos"); //Yo lo saco de mi clase ProgramShader.
            cantComponentes = 3;   // 3 componentes (x, y, z)
            attribType = VertexAttribPointerType.Float; //Cada componente es un Float.
            stride = 0;  //Los datos estan uno a continuacion del otro.
            offset = 0;  //El primer dato esta al comienzo. (no hay offset).
            bufferType = BufferTarget.ArrayBuffer; //Buffer de Datos.

            gl.EnableVertexAttribArray(attribIndex); //Habilitamos el indice de atributo.
            gl.BindBuffer(bufferType, h_VBO); //Seleccionamos el buffer a utilizar.
            gl.VertexAttribPointer(attribIndex, cantComponentes, attribType, false, stride, offset);//Configuramos el layout (como estan organizados) los datos en el buffer.

            // 2.a.El bloque anterior se repite para cada atributo del vertice (color, normal, textura..)

            // 3. Configuramos el EBO a utilizar. (como son indices, no necesitan info de layout)
            bufferType = BufferTarget.ElementArrayBuffer;
            gl.BindBuffer(bufferType, h_EBO);

            // 4. Deseleccionamos el VAO.
            gl.BindVertexArray(0);
        }

        private void CrearVAONormales(ShaderProgram sProgram)
        {

            int attribIndex;
            int cantComponentes; //Cantidad de componentes de CADA dato.
            VertexAttribPointerType attribType; // Tipo de CADA una de las componentes del dato.
            int stride; //Cantidad de BYTES que hay que saltar para llegar al proximo dato. (0: Tightly Packed, uno a continuacion del otro)
            int offset; //Offset en BYTES del primer dato.
            BufferTarget bufferType; //Tipo de buffer.

            // 1. Creamos el VAO
            h_VAON = gl.GenVertexArray(); //Pedimos un identificador de VAO a OpenGL.
            gl.BindVertexArray(h_VAON);   //Lo seleccionamos para trabajar/configurar.

            //2. Configuramos el VBO de posiciones.
            attribIndex = sProgram.GetVertexAttribLocation("vPos"); //Yo lo saco de mi clase ProgramShader.
            cantComponentes = 3;   // 3 componentes (x, y, z)
            attribType = VertexAttribPointerType.Float; //Cada componente es un Float.
            stride = 0;  //Los datos estan uno a continuacion del otro.
            offset = 0;  //El primer dato esta al comienzo. (no hay offset).
            bufferType = BufferTarget.ArrayBuffer; //Buffer de Datos.

            gl.EnableVertexAttribArray(attribIndex); //Habilitamos el indice de atributo.
            gl.BindBuffer(bufferType, h_VBON); //Seleccionamos el buffer a utilizar.
            gl.VertexAttribPointer(attribIndex, cantComponentes, attribType, false, stride, offset);//Configuramos el layout (como estan organizados) los datos en el buffer.

            // 2.a.El bloque anterior se repite para cada atributo del vertice (color, normal, textura..)

            // 3. Configuramos el EBO a utilizar. (como son indices, no necesitan info de layout)
            bufferType = BufferTarget.ElementArrayBuffer;
            gl.BindBuffer(bufferType, h_EBON);

            // 4. Deseleccionamos el VAO.
            gl.BindVertexArray(0);
        }

        public void Dibujar(ShaderProgram sProgram)
        {

            PrimitiveType primitive; //Tipo de Primitiva a utilizar (triangulos, strip, fan, quads, ..)
            int offset; // A partir de cual indice dibujamos?
            int count;  // Cuantos?
            DrawElementsType indexType; //Tipo de los indices.

            primitive = PrimitiveType.Triangles;  //Usamos triangulos.
            offset = 0;  // A partir del primer indice.
            count = indices.Length; // Todos los indices.
            indexType = DrawElementsType.UnsignedInt; //Los indices son enteros sin signo.

            gl.BindVertexArray(h_VAO); //Seleccionamos el VAO a utilizar.
            gl.DrawElements(primitive, count, indexType, offset); //Dibujamos utilizando los indices del VAO.
            gl.BindVertexArray(0); //Deseleccionamos el VAO
        }


        public void DibujarNormales(ShaderProgram sProgram)
        {

            PrimitiveType primitive; //Tipo de Primitiva a utilizar (triangulos, strip, fan, quads, ..)
            int offset; // A partir de cual indice dibujamos?
            int count;  // Cuantos?
            DrawElementsType indexType; //Tipo de los indices.

            primitive = PrimitiveType.Lines;  //Usamos puntos
                                              //    primitive = PrimitiveType.Points;
            offset = 0;  // A partir del primer indice.  
            count = indicesNormales.Length;
            indexType = DrawElementsType.UnsignedInt; //Los indices son enteros sin signo.


            //Se dibuja el VAO de las normales


            gl.BindVertexArray(h_VAON); //Seleccionamos el VAO a utilizar.
            gl.DrawElements(primitive, count, indexType, offset); //Dibujamos utilizando los indices del VAO.
            gl.BindVertexArray(0); //Deseleccionamos el VAO

        }


    }
}
