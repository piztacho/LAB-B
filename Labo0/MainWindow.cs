using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK; //La matematica
using OpenTK.Graphics.OpenGL;
using gl = OpenTK.Graphics.OpenGL.GL;
using CGUNS.Shaders;
using CGUNS.Cameras;
using CGUNS.Meshes;
using CGUNS.Parsers;
using CGUNS;
using System.Drawing.Imaging;

namespace Labo0
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private ShaderProgram sProgram,sProgramGizmo, sProgramPhong, sProgramText, sProgramLuz; //Nuestro programa de shaders.
        //private Cube myCube; //Nuestro objeto a dibujar.
        private const float DEG2RAD = (float)(Math.PI / 180.0); //Para pasar de grados a radianes
        private CGUNS.Meshes.FaceVertexList.FVLMesh mesa,drone,avion, plano;
        private CGUNS.Meshes.FaceVertexList.FVLMesh drone2;
        private Material material;
        private int textAvion, textPlano;
        private Light luz;
        private Light[] luces;
        Vector4 posL = new Vector4(0.0f, 8.0f, 1.0f, 0.0f);
        Vector3[] colorLuz = new Vector3[] { new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f) };
        int indiceColor = 0;
        private CamQuaternion myCamera;  //Camara
        private Rectangle viewport; //Viewport a utilizar (Porcion del glControl en donde voy a dibujar).

        private int normales = 0; // se usa para dibujar o no las normales

        private float rotar_helice = 0;


        private void glControl3_Load(object sender, EventArgs e)
        {
            logContextInfo(); //Mostramos info de contexto.
            SetupShaders(); //Creamos los shaders y el programa de shader


            mesa = ObjFileParser.parseFile("ModelosOBJ/ModernTable.obj");
            mesa.Build(sProgramPhong); //Construyo los objetos OpenGL 
            avion = ObjFileParser.parseFile("ModelosOBJ/avionJuguete.obj");
            avion.Build(sProgramText); //Construyo los objetos OpenGL 
            drone = ObjFileParser.parseFile("ModelosOBJ/drone2.obj");
            drone.Build(sProgramPhong); //Construyo los objetos OpenGL
            plano = ObjFileParser.parseFile("ModelosOBJ/plano.obj");
            plano.Build(sProgramText); //Construyo los objetos OpenGL 
        








            GL.ActiveTexture(TextureUnit.Texture0);
            textPlano = CargarTextura("files/Texturas/piedra.jpg");

            textAvion = CargarTextura("files/Texturas/madera.jpg");

           



       
                                          // GL.ActiveTexture(TextureUnit.Texture1);
                                          // texIronMan = CargarTextura("files/Texturas/madera.jpg");
                                          //texIronMan = CargarTextura("files/Texturas/madera.jpg");

          //  plano.Build(sProgramText);
            //Configuracion de la Luz
            luz = new Light();
            luz.Enabled = 1;
            luz.gizmo.Build(sProgramGizmo);
           

            luces = new Light[1];
            luces[0] = new Light();

            luces[0].Ispecular = new Vector3(1.0f, 1.0f, 1.0f);
            luces[0].ConeAngle = 180.0f;
            luces[0].ConeDirection = new Vector3(0.0f, -1.0f, 0.0f);
            luces[0].Enabled = 1;
            luces[0].gizmo.Build(sProgramGizmo);
           


            /*
            int cantVertices = 0;
            int cantNormales = 0;
                        
            for (int i=0; i< IronMan2.Length; i++)
            {
                IronMan2[i].Build(sProgram, cantVertices, cantNormales);
                cantVertices += IronMan2[i].VertexCount;
                cantNormales += IronMan2[i].NormalCount;
            }
            */

            myCamera = new CamQuaternion(); //Creo una camara.
            

            gl.ClearColor(Color.LightGray); //Configuro el Color de borrado.
            gl.Enable(EnableCap.DepthTest);
            gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //De cada poligono solo dibujo las lineas de contorno (wireframe).
        
          gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); 
        }

        private void glControl3_Paint(object sender, PaintEventArgs e)
        {
            Matrix4 modelMatrix = Matrix4.Identity; //Por ahora usamos la identidad.
            Matrix4 viewMatrix = myCamera.getViewMatrix();
            Matrix4 projMatrix = myCamera.getProjectionMatrix();
            Matrix4 mvMatrix = Matrix4.Mult(viewMatrix, modelMatrix);
            Matrix4 MVP = Matrix4.Mult(mvMatrix, projMatrix);

           // Vector4 figColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Matrix4 MAcumulada = Matrix4.Identity;
            Matrix4 Escalado = Matrix4.CreateScale(0.5f);

            Matrix3 mnMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(mvMatrix)));
            

            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); //Borramos el contenido del glControl.

            gl.Viewport(viewport); //Especificamos en que parte del glControl queremos dibujar.

            luz.Position = posL;
            luz.Iambient = colorLuz[indiceColor];
            luz.Idiffuse = colorLuz[indiceColor];          

            luces[0].Position = posL;
            luces[0].Iambient = colorLuz[indiceColor];
            luces[0].Idiffuse = colorLuz[indiceColor];


            //Dibujamos la Luz
            sProgramGizmo.Activate();
            sProgramGizmo.SetUniformValue("projMat", projMatrix);
            sProgramGizmo.SetUniformValue("viewMatrix", viewMatrix);
            sProgramGizmo.SetUniformValue("modelMat", modelMatrix);
            luz.gizmo.Dibujar(sProgramGizmo); 
            for (int i = 0; i < luces.Length; i++)
                luces[i].gizmo.Dibujar(sProgramGizmo);
            sProgramGizmo.Deactivate();
            
            
            //Dibujamos el objeto1 (mesa).
            sProgramPhong.Activate();

            sProgramPhong.SetUniformValue("projMat", projMatrix);
            sProgramPhong.SetUniformValue("viewMatrix", viewMatrix);
            sProgramPhong.SetUniformValue("modelMat", modelMatrix);
            sProgramPhong.SetUniformValue("MNormal", mnMatrix);

            material = new Material();
            material = Material.Brass;
            sProgramPhong.SetUniformValue("ka", material.Kambient);
            sProgramPhong.SetUniformValue("kd", material.Kdiffuse);
            sProgramPhong.SetUniformValue("ks", material.Kspecular);
            sProgramPhong.SetUniformValue("CoefEsp", material.Shininess);
            MAcumulada = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(0.0f, -9f, 0.0f) * modelMatrix;
            sProgramPhong.SetUniformValue("modelMat", MAcumulada);
            sProgramPhong.SetUniformValue("posL", luz.Position);
            sProgramPhong.SetUniformValue("LightIa", luz.Iambient);
            sProgramPhong.SetUniformValue("LightId", luz.Idiffuse);
            sProgramPhong.SetUniformValue("LightIs", luz.Ispecular);

            mesa.Dibujar(sProgramPhong);
            sProgramPhong.Deactivate();
            

            /*
            //Dibujamos el objeto1 (Drone).
            sProgramPhong.Activate();

            sProgramPhong.SetUniformValue("projMat", projMatrix);
            sProgramPhong.SetUniformValue("viewMatrix", viewMatrix);
            sProgramPhong.SetUniformValue("modelMat", modelMatrix);
            sProgramPhong.SetUniformValue("MNormal", mnMatrix);

            material = new Material();
            material = Material.Gold;
            sProgram.SetUniformValue("ka", material.Kambient);
            sProgram.SetUniformValue("kd", material.Kdiffuse);
            sProgram.SetUniformValue("ks", material.Kspecular);
            sProgram.SetUniformValue("CoefEsp", material.Shininess);
            MAcumulada = Matrix4.Identity;
            MAcumulada = Matrix4.CreateScale(5f) * Matrix4.CreateTranslation(0.7f,-1.7f,0.0f) ;
            sProgram.SetUniformValue("modelMat", MAcumulada);
            sProgram.SetUniformValue("posL", luz.Position);
            sProgram.SetUniformValue("LightIa", luz.Iambient);
            sProgram.SetUniformValue("LightId", luz.Idiffuse);
            sProgram.SetUniformValue("LightIs", luz.Ispecular);

            IronMan.Dibujar(sProgram);
            sProgram.Deactivate();
            */

            /*
            ////////Plano con Textura y Luz 
                      
            sProgramLuz.Activate();
            sProgramLuz.SetUniformValue("projMat", projMatrix);
            sProgramLuz.SetUniformValue("viewMatrix", viewMatrix);
            sProgramLuz.SetUniformValue("modelMat", modelMatrix);
            sProgramLuz.SetUniformValue("MNormal", mnMatrix);

            material = new Material();
            material = Material.Obsidian;
            sProgramLuz.SetUniformValue("material.Ka", material.Kambient);
            sProgramLuz.SetUniformValue("material.Kd", material.Kdiffuse);
            sProgramLuz.SetUniformValue("material.Ks", material.Kspecular);
            sProgramLuz.SetUniformValue("material.Shininess", material.Shininess);
                
         
            sProgramLuz.SetUniformValue("numLights", luces.Length);
            for (int i = 0; i < luces.Length; i++)
            {
                sProgramLuz.SetUniformValue("allLights[" + i + "].position", luces[i].Position);
                sProgramLuz.SetUniformValue("allLights[" + i + "].Ia", luces[i].Iambient);
                sProgramLuz.SetUniformValue("allLights[" + i + "].Id", luces[i].Idiffuse);
                sProgramLuz.SetUniformValue("allLights[" + i + "].Is", luces[i].Ispecular);
                sProgramLuz.SetUniformValue("allLights[" + i + "].coneAngle", luces[i].ConeAngle);
                sProgramLuz.SetUniformValue("allLights[" + i + "].coneDirection", luces[i].ConeDirection);
                sProgramLuz.SetUniformValue("allLights[" + i + "].enabled", luces[i].Enabled);
            }

            GL.BindTexture(TextureTarget.Texture2D, textPlano);

            MAcumulada = Matrix4.Identity;
            MAcumulada = Matrix4.CreateScale(8f) * Matrix4.CreateTranslation(0.0f, -9f, 0.0f);
            sProgramLuz.SetUniformValue("modelMat", MAcumulada);

            plano.Dibujar(sProgramLuz);
            sProgramLuz.Deactivate();
            */

            
            //Dibujamos el Drone.
            sProgramPhong.Activate(); //Activamos el programa de shaders
                                      //Seteamos los valores uniformes.

            sProgramPhong.SetUniformValue("projMat", projMatrix);
            sProgramPhong.SetUniformValue("viewMatrix", viewMatrix);
            sProgramPhong.SetUniformValue("modelMat", modelMatrix);
            sProgramPhong.SetUniformValue("MNormal", mnMatrix);
          

            material = new Material();
            material = Material.WhitePlastic;
            sProgramPhong.SetUniformValue("ka", material.Kambient);
            sProgramPhong.SetUniformValue("kd", material.Kdiffuse);
            sProgramPhong.SetUniformValue("ks", material.Kspecular);
            sProgramPhong.SetUniformValue("CoefEsp", material.Shininess);
            MAcumulada = Matrix4.Identity;
            MAcumulada = Matrix4.CreateScale(0.05f) * Matrix4.CreateTranslation(4.7f, -1.7f, 0.0f);
            sProgramPhong.SetUniformValue("modelMat", MAcumulada);            
            sProgramPhong.SetUniformValue("posL", luz.Position);
            sProgramPhong.SetUniformValue("LightIa", luz.Iambient);
            sProgramPhong.SetUniformValue("LightId", luz.Idiffuse);
            sProgramPhong.SetUniformValue("LightIs", luz.Ispecular);

            drone.Dibujar(sProgramPhong);
            sProgramPhong.Deactivate(); //Desactivamos el programa de shader.
            
            
            
            // Dibujamos el Plano
            sProgramText.Activate();
            sProgramText.SetUniformValue("projMat", projMatrix);
            sProgramText.SetUniformValue("viewMatrix", viewMatrix);
            sProgramText.SetUniformValue("modelMat", modelMatrix);
            sProgramText.SetUniformValue("MNormal", mnMatrix);
            material = new Material();
            material = Material.Silver;
            sProgramText.SetUniformValue("ka", material.Kambient);
            sProgramText.SetUniformValue("kd", material.Kdiffuse);
            sProgramText.SetUniformValue("ks", material.Kspecular);
            sProgramText.SetUniformValue("CoefEsp", material.Shininess);


            sProgramText.SetUniformValue("posL", luz.Position);
            sProgramText.SetUniformValue("LightIa", luz.Iambient);
            sProgramText.SetUniformValue("LightId", luz.Idiffuse);
            sProgramText.SetUniformValue("LightIs", luz.Ispecular);         

            MAcumulada = Matrix4.Identity;
            MAcumulada = Matrix4.CreateScale(8f) * Matrix4.CreateTranslation(0.0f, -9f, 0.0f);
            sProgramText.SetUniformValue("modelMat", MAcumulada);

            GL.BindTexture(TextureTarget.Texture2D, textPlano); //Activamos la textura

            plano.Dibujar(sProgramText);
            sProgramText.Deactivate();
            

            
            //Dibujamos el Avion
            sProgramText.Activate();
            sProgramText.SetUniformValue("projMat", projMatrix);
            sProgramText.SetUniformValue("viewMatrix", viewMatrix);
            sProgramText.SetUniformValue("modelMat", modelMatrix);
            sProgramText.SetUniformValue("MNormal", mnMatrix);

            material = new Material();
            material = Material.Silver;
            sProgramText.SetUniformValue("ka", material.Kambient);
            sProgramText.SetUniformValue("kd", material.Kdiffuse);
            sProgramText.SetUniformValue("ks", material.Kspecular);
            sProgramText.SetUniformValue("CoefEsp", material.Shininess);             


            MAcumulada = Matrix4.Identity;
            MAcumulada = Matrix4.CreateScale(0.17f) * Matrix4.CreateTranslation(-2.8f, 0.0f, 0.0f);
            sProgramText.SetUniformValue("modelMat", MAcumulada);

            sProgramText.SetUniformValue("posL", luz.Position);
            sProgramText.SetUniformValue("LightIa", luz.Iambient);
            sProgramText.SetUniformValue("LightId", luz.Idiffuse);
            sProgramText.SetUniformValue("LightIs", luz.Ispecular);

            GL.BindTexture(TextureTarget.Texture2D, textAvion);

            avion.Dibujar(sProgramText);
            sProgramText.Deactivate();
            

            /*
            //Dibujamos el IronMan4              
            sProgramText.Activate();
            sProgramText.SetUniformValue("projMat", projMatrix);
            sProgramText.SetUniformValue("viewMatrix", viewMatrix);
            sProgramText.SetUniformValue("modelMat", modelMatrix);
            sProgramText.SetUniformValue("MNormal", mnMatrix);
            material = new Material();
            material = Material.Gold;
            sProgramText.SetUniformValue("ka", material.Kambient);
            sProgramText.SetUniformValue("kd", material.Kdiffuse);
            sProgramText.SetUniformValue("ks", material.Kspecular);
            sProgramText.SetUniformValue("CoefEsp", material.Shininess);

            MAcumulada = Matrix4.Identity;
            MAcumulada = Matrix4.CreateScale(5f) * Matrix4.CreateTranslation(-7.3f, -1.7f, 0.0f);
            sProgramText.SetUniformValue("modelMat", MAcumulada);

            sProgramText.SetUniformValue("posL", luz.Position);
            sProgramText.SetUniformValue("LightIa", luz.Iambient);
            sProgramText.SetUniformValue("LightId", luz.Idiffuse);
            sProgramText.SetUniformValue("LightIs", luz.Ispecular);

            GL.BindTexture(TextureTarget.Texture2D, texIronMan);

            IronMan4.Dibujar(sProgramText);
            sProgramText.Deactivate();


            /*
            //Dibujamos el objeto2 (IronMan2)
            MAcumulada = Matrix4.Identity;
            //figColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            MAcumulada = Matrix4.CreateScale(1.5f) ;
                      
            sProgram.SetUniformValue("modelMat", MAcumulada);
           // sProgram.SetUniformValue("figureColor", figColor);

            
            for (int i = 0; i < IronMan2.Length; i++)
            {
                IronMan2[i].Dibujar(sProgram);
            }
            
            /*
            for (int i=0; i<drone.Length; i++)
            {
                if (i != 2)
                {
                    drone[i].Dibujar(sProgram);
                }
                else
                {
                    figColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                    //  MAcumulada = Matrix4.CreateScale(0.1f)  * Matrix4.CreateRotationY(-90 * DEG2RAD) * Matrix4.CreateRotationZ(rotar_helice * DEG2RAD) * Matrix4.CreateTranslation(0.0f, -0.8f, 0.0f);
                    MAcumulada = Matrix4.CreateScale(0.05f * 1.5f) * Matrix4.CreateTranslation(-1.8f, 0.0f, 1.6f) * Matrix4.CreateRotationY(rotar_helice * DEG2RAD) * Matrix4.CreateTranslation(1.8f, 0.0f, -1.6f) * Matrix4.CreateTranslation(0.0f, -1.7f, 0.0f) ;
                    sProgram.SetUniformValue("figureColor", figColor);
                    sProgram.SetUniformValue("modelMat", MAcumulada);
                    drone[i].Dibujar(sProgram);
                    MAcumulada = Matrix4.Identity;
                    figColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                    MAcumulada = Matrix4.CreateScale(0.05f * 1.5f) * Matrix4.CreateTranslation(0.0f, -1.7f, 0.0f);
                    //MAcumulada = Matrix4.CreateScale(0.1f) * Matrix4.CreateRotationY(-90 * DEG2RAD) * Matrix4.CreateTranslation(0.0f, -0.8f, 0.0f);
                    sProgram.SetUniformValue("modelMat", MAcumulada);
                    sProgram.SetUniformValue("figureColor", figColor);

                }
            }
            


            //Dibujamos el objeto3 (drone2).
            MAcumulada = Matrix4.Identity;
            figColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            sProgram.SetUniformValue("figureColor", figColor);
            MAcumulada = Matrix4.CreateScale(0.05f) * Matrix4.CreateRotationY(90* DEG2RAD) *  Matrix4.CreateTranslation(7.0f, -1.8f, 2.5f) * modelMatrix;
            sProgram.SetUniformValue("modelMat", MAcumulada);
            for (int i = 0; i < drone.Length; i++)
            {
                drone[i].Dibujar(sProgram);
            }
            */

            /*
            switch (normales)
            {   
                
                case 0: //no dibuja ninguna
                    break;

                case 1: //dibuja normales 
                    sProgram.Activate();

                    sProgram.SetUniformValue("projMat", projMatrix);
                    sProgram.SetUniformValue("viewMatrix", viewMatrix);
                    sProgram.SetUniformValue("modelMat", modelMatrix);
                    sProgram.SetUniformValue("MNormal", mnMatrix);

                    material = new Material();
                    material = Material.WhitePlastic;
                    sProgram.SetUniformValue("ka", material.Kambient);
                    sProgram.SetUniformValue("kd", material.Kdiffuse);
                    sProgram.SetUniformValue("ks", material.Kspecular);
                    sProgram.SetUniformValue("CoefEsp", material.Shininess);
                    MAcumulada = Matrix4.Identity;
                    MAcumulada = Matrix4.CreateScale(5f) * Matrix4.CreateTranslation(0.7f, -1.7f, 0.0f);
                    sProgram.SetUniformValue("modelMat", MAcumulada);
                    sProgram.SetUniformValue("posL", luz.Position);
                    sProgram.SetUniformValue("LightIa", luz.Iambient);
                    sProgram.SetUniformValue("LightId", luz.Idiffuse);
                    sProgram.SetUniformValue("LightIs", luz.Ispecular);

                    drone.DibujarNormales(sProgram);
                    sProgram.Deactivate();
                    break;

                 /*   
                case 2: // Dibujo normales Drone 1
                    MAcumulada = Matrix4.Identity;
                    figColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                    MAcumulada = Matrix4.CreateScale(0.05f) * Matrix4.CreateRotationY(90 * DEG2RAD) * Matrix4.CreateTranslation(7.0f, -1.8f, 2.5f) * modelMatrix;
                    sProgram.SetUniformValue("modelMat", MAcumulada);
                    sProgram.SetUniformValue("figureColor", figColor);
                    //sProgram.SetUniformValue("mvMat", MAcumulada);
                    for (int i = 0; i < drone.Length; i++)
                    {
                        drone[i].DibujarNormales(sProgram);
                    }
                    break;

                case 3: // Dibujo normales Drone 2
                    MAcumulada = Matrix4.Identity;
                    figColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                    MAcumulada = Matrix4.CreateScale(0.05f * 1.5f) * Matrix4.CreateTranslation(0.0f, -1.8f, 0.0f) * modelMatrix;

                    sProgram.SetUniformValue("modelMat", MAcumulada);
                    sProgram.SetUniformValue("figureColor", figColor);
                    //sProgram.SetUniformValue("mvMat", MAcumulada);
                    for (int i = 0; i < drone.Length; i++)
                    {
                        drone[i].DibujarNormales(sProgram);
                    }
                    break;

                   
            }
         */



            glControl3.SwapBuffers(); //Intercambiamos buffers frontal y trasero, para evitar flickering.
        }

        private int CargarTextura(String imagenTex)
        {
            int texId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texId);


            Bitmap bitmap = new Bitmap(Image.FromFile(imagenTex));

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                             ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            return texId;

        }

        private void glControl3_Resize(object sender, EventArgs e)
        {   //Actualizamos el viewport para que dibuje en el centro de la pantalla.
            Size size = glControl3.Size;
            if (size.Width < size.Height)
            {
                viewport.X = 0;
                viewport.Y = (size.Height - size.Width) / 2;
                viewport.Width = size.Width;
                viewport.Height = size.Width;
            }
            else
            {
                viewport.X = (size.Width - size.Height) / 2;
                viewport.Y = 0;
                viewport.Width = size.Height;
                viewport.Height = size.Height;
            }
            glControl3.Invalidate(); //Invalidamos el glControl para que se redibuje.(llama al metodo Paint)
        }

        private void SetupShaders()
        {
            //Lo hago con mis clases, que encapsulan el manejo de shaders.
            //1. Creamos los shaders, a partir de archivos.
            String vShaderFile = "files/shaders/IPhong/vgouraud.glsl";
            String fShaderFile = "files/shaders/IPhong/fgouraud.glsl";
           
            Shader vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            Shader fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            vShader.Compile();
            fShader.Compile();
            //3. Creamos el Programa de shader con ambos.
            sProgram = new ShaderProgram();
            sProgram.AddShader(vShader);
            sProgram.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgram.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();

            //Shader para los Gizmos
            //1. Creamos los shaders, a partir de archivos.
            vShaderFile = "files/shaders/Basic/vshader1.glsl";
            fShaderFile = "files/shaders/Basic/fshader1.glsl";
            vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            vShader.Compile();
            fShader.Compile();
            //3. Creamos el Programa de shader con ambos.
            sProgramGizmo = new ShaderProgram();
            sProgramGizmo.AddShader(vShader);
            sProgramGizmo.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgramGizmo.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();

            //Shader para Luz cono
            //1. Creamos los shaders, a partir de archivos.
            vShaderFile = "files/shaders/IPhong/vLuzCono.glsl";
            fShaderFile = "files/shaders/IPhong/fLuzCono.glsl";
            vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            vShader.Compile();
            fShader.Compile();
            //3. Creamos el Programa de shader con ambos.
            sProgramLuz = new ShaderProgram();
            sProgramLuz.AddShader(vShader);
            sProgramLuz.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgramLuz.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();

            //Shader para Textura
            //1. Creamos los shaders, a partir de archivos.
            vShaderFile = "files/TextShaders/IPhong/vPhong.glsl";
            fShaderFile = "files/TextShaders/IPhong/fPhong.glsl";
           // vShaderFile = "files/TextShaders/IPhong/vgouraud.glsl";
           // fShaderFile = "files/TextShaders/IPhong/fgouraud.glsl";
            vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            vShader.Compile();
            fShader.Compile();
            //3. Creamos el Programa de shader con ambos.
            sProgramText = new ShaderProgram();
            sProgramText.AddShader(vShader);
            sProgramText.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgramText.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();

            //Shader para Phong
            //1. Creamos los shaders, a partir de archivos.
            vShaderFile = "files/shaders/IPhong/vPhong.glsl";
            fShaderFile = "files/shaders/IPhong/fPhong.glsl";
            vShader = new Shader(ShaderType.VertexShader, vShaderFile);
            fShader = new Shader(ShaderType.FragmentShader, fShaderFile);
            //2. Los compilamos
            vShader.Compile();
            fShader.Compile();
            //3. Creamos el Programa de shader con ambos.
            sProgramPhong = new ShaderProgram();
            sProgramPhong.AddShader(vShader);
            sProgramPhong.AddShader(fShader);
            //4. Construimos (linkeamos) el programa.
            sProgramPhong.Build();
            //5. Ya podemos eliminar los shaders compilados. (Si no los vamos a usar en otro programa)
            vShader.Delete();
            fShader.Delete();
        }

        private void logContextInfo()
        {
            String version, renderer, shaderVer, vendor;//, extensions;
            version = gl.GetString(StringName.Version);
            renderer = gl.GetString(StringName.Renderer);
            shaderVer = gl.GetString(StringName.ShadingLanguageVersion);
            vendor = gl.GetString(StringName.Vendor);
            //extensions = gl.GetString(StringName.Extensions);
            log("========= CONTEXT INFORMATION =========");
            log("Renderer:       {0}", renderer);
            log("Vendor:         {0}", vendor);
            log("OpenGL version: {0}", version);
            log("GLSL version:   {0}", shaderVer);
            //log("Extensions:" + extensions);
            log("===== END OF CONTEXT INFORMATION =====");

        }
        private void log(String format, params Object[] args)
        {
            System.Diagnostics.Debug.WriteLine(String.Format(format, args), "[CGUNS]");
        }

        private void glControl3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            e.IsInputKey = true;
        }

        private void glControl3_KeyPressed(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.S:
                    myCamera.Abajo();
                    break;

                case Keys.Up:
                case Keys.W:
                    myCamera.Arriba();
                    break;

                case Keys.Right:
                case Keys.D:
                    myCamera.Derecha();
                    break;

                case Keys.Left:
                case Keys.A:
                    myCamera.Izquierda();
                    break;

                case Keys.Add:
                    myCamera.Acercar(0.5f);
                    break;

                case Keys.Subtract:
                    myCamera.Alejar(0.5f);
                    break;

                case Keys.N:
                    normales = (normales == 0) ? 1 : 0;
                    break;

                case Keys.D0:
                    normales = 0;
                    break;

                case Keys.D1:
                case Keys.M:
                    normales = 1; //Dibuja normales 
                    break;

                case Keys.D2:
                    normales = 2; //Dibuja normales 
                    break;

                case Keys.D3:
                    normales = 3; //Dibuja normales 
                    break;

                case Keys.G:
                    rotar_helice += 10; // acumula valor para rotar la helice
                    break;
                case Keys.I:
                    posL.Z = posL.Z - 0.5f;
                    break;
                case Keys.J:
                    posL.X = posL.X - 0.5f;
                    break;
                case Keys.K:
                    posL.Z = posL.Z + 0.5f;
                    break;
                case Keys.L:
                    posL.X = posL.X + 0.5f;
                    break;
                case Keys.O:
                    posL.Y = posL.Y + 0.5f;
                    break;
                case Keys.P:
                    posL.Y = posL.Y - 0.5f;
                    break;
                case Keys.Q:
                    indiceColor = (indiceColor + 1) % 4;
                   
                    break;


            }

            glControl3.Invalidate(); //Notar que renderizamos para CUALQUIER tecla que sea presionada.
        }
    }
}
