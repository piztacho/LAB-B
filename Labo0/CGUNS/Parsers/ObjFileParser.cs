using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using CGUNS.Meshes.FaceVertexList;
using OpenTK;

namespace CGUNS.Parsers
{
    public class ObjFileParser
    {
        private const String VERTEX = "v";
        private const String FACE = "f";
        private const String NORMAL = "vn";
        private const String TEXCORD = "vt";
        private const Char COMMENT = '#';
        private static Char[] SEPARATORS = { ' ', '/' };
        private const Char OTROOBJ = 'o';
        private static bool logEnabled = false;


        public static FVLMesh parseFile(String fileName)
        {
            FVLMesh mesh = new FVLMesh();
            String line;
            String[] lineSplit;
            String sender = "ObjFileParser.parseFile: ";
            info(sender, "Opening file: {0}", fileName);
            StreamReader file = new StreamReader(fileName);
            info(sender, "OK. Reading all the lines of the file...");
            line = file.ReadLine();
            while (line != null)
            {
                line = line.Trim(); //Saco espacios en blanco.
                if ((line.Length != 0) && (!line[0].Equals(COMMENT))) //Si no es comentario
                {
                    lineSplit = line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                    if (lineSplit[0].Equals(VERTEX))
                    {
                        parseVertex(mesh, lineSplit);
                    }
                    else if (lineSplit[0].Equals(NORMAL))
                    {
                        parseNormal(mesh, lineSplit);
                    }

                    else if (lineSplit[0].Equals(FACE))
                    {
                        parseFace(mesh, line);//HERE!!
                    }

                    else if (lineSplit[0].Equals(TEXCORD))
                    {
                        parseTexCord(mesh, lineSplit);
                    }


                    else {
                        log(sender, "Not supported instruction: {0}", lineSplit[0]);
                    }
                }
                line = file.ReadLine();
            }
            file.Close();
            info(sender, "FINISHED!");
            return mesh;
        }






        public static FVLMesh[] parseFileDividido(String fileName)
        {
            int cant = 0;

            FVLMesh[] objetos;

            String line;
            String[] lineSplit;
            String sender = "ObjFileParser.parseFile: ";
            info(sender, "Opening file: {0}", fileName);
            StreamReader file = new StreamReader(fileName);
            info(sender, "OK. Reading all the lines of the file...");

            //SE cuenta la cantidad de objetos que hay
            line = file.ReadLine();
            while (line != null)
            {
                if (line[0].Equals(OTROOBJ))
                    cant++;
                line = file.ReadLine();
            }
            info(sender, "cant es " + cant);
            objetos = new FVLMesh[cant];
            file.Close();
            file = new StreamReader(fileName);


            line = file.ReadLine();
            while (!line[0].Equals(OTROOBJ) && line != null)
                line = file.ReadLine(); //Se busca hasta donde empiezan a listarse los vertices del 1er objeto




            for (int i = 0; i < cant; i++)
            { //Se crea un mesh para cada uno de los objetos
                FVLMesh mesh = new FVLMesh();

                line = file.ReadLine();
                while (line != null && (!line[0].Equals(OTROOBJ)))
                {

                    line = line.Trim(); //Saco espacios en blanco.                

                    if ((line.Length != 0) && (!line[0].Equals(COMMENT)) && (!line[0].Equals(OTROOBJ))) //Si no es comentario
                    {
                        lineSplit = line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                        if (lineSplit[0].Equals(VERTEX))
                        {
                            parseVertex(mesh, lineSplit);
                        }
                        else if (lineSplit[0].Equals(NORMAL))
                        {
                            parseNormal(mesh, lineSplit);
                        }

                        else if (lineSplit[0].Equals(FACE))
                        {
                            parseFace(mesh, line);//HERE!!
                        }

                        else if (lineSplit[0].Equals(TEXCORD))
                        {
                            parseTexCord(mesh, lineSplit);
                        }


                        else {
                            log(sender, "Not supported instruction: {0}", lineSplit[0]);
                        }
                    }

                    if (!line[0].Equals(OTROOBJ) && line != null)
                        line = file.ReadLine();



                }
                objetos[i] = mesh;
            }
            file.Close();
            info(sender, "FINISHED!");
            return objetos;
        }






        public static void parseVertex(FVLMesh mesh, String[] args)
        {
            String sender = "ObjFileParser.parseVertex: ";
            Vector3 vertex = new Vector3();
            switch (args.Length)
            {
                case 2:
                    log(sender, "Vertex definition must be (x,y,z [,w]). Found only x.");
                    vertex.X = float.Parse(args[1], System.Globalization.NumberStyles.Number);
                    vertex.Y = 0.0f;
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 3:
                    log(sender, "Vertex definition must be (x,y,z [,w]). Found only x, y.");
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = 0.0f;
                    //vertex.W = 1.0f;
                    break;
                case 4:
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    //vertex.W = 1.0f;
                    break;
                case 5:
                    log(sender, "Found (x, y, z, w). Discarding w component.");
                    vertex.X = float.Parse(args[1], CultureInfo.InvariantCulture);
                    vertex.Y = float.Parse(args[2], CultureInfo.InvariantCulture);
                    vertex.Z = float.Parse(args[3], CultureInfo.InvariantCulture);
                    //vertex.W = float.Parse(args[4], System.Globalization.NumberStyles.Number);
                    break;
                default:
                    break;
            }
            mesh.AddVertex(vertex);
            //vertexCount++;
        }

        public static void parseNormal(FVLMesh mesh, String[] args)
        {
            Vector3 vertex = new Vector3();
            vertex.X = float.Parse(args[1], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            vertex.Y = float.Parse(args[2], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            vertex.Z = float.Parse(args[3], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            mesh.AddVertexNormal(vertex);
        }

        public static void parseTexCord(FVLMesh mesh, String[] args)
        {
            Vector2 texCord = new Vector2();
            texCord.X = float.Parse(args[1], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            texCord.Y = float.Parse(args[2], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
            mesh.AddTexCord(texCord);
        }


        public static void parseFace(FVLMesh mesh, string line)
        {
            FVLFace face = new FVLFace();

            int i = 2; // componente 1 = f , comp 2 = ' '
            String vertex;
            String texCord;
            String normal;

            while (i < line.Length)
            {
                vertex = "";
                texCord = "";
                normal = "";
                while (i < line.Length && line[i] != ' ' && line[i] != '/')
                {
                    vertex = vertex + line[i];
                    i++;
                }

                if (i < line.Length && line[i] != ' ')
                {
                    i++;
                    if (line[i] != '/')
                    {
                        while (i < line.Length && line[i] != ' ' && line[i] != '/')
                        {
                            texCord = texCord + line[i];
                            i++;
                        }
                    }
                    i++;

                    if (i < line.Length && line[i] != '/')
                    {
                        while (i < line.Length && line[i] != ' ' && line[i] != '/')
                        {
                            normal = normal + line[i];
                            i++;
                        }
                    }
                }

                i++;
                face.AddVertex(Int32.Parse(vertex, NumberStyles.Integer) - 1);
                if (!normal.Equals(""))
                {
                    face.AddNormal(Int32.Parse(normal, NumberStyles.Integer) - 1);
                }
                if (!texCord.Equals(""))
                {
                    face.AddTexCord(Int32.Parse(texCord, NumberStyles.Integer) - 1);
                }
            }

            mesh.AddFace(face);
        }


        private static void log(String sender, String format, params Object[] args)
        {
            if (logEnabled)
            {
                System.Console.WriteLine(sender + format, args);
            }
        }
        private static void info(String sender, String format, params Object[] args)
        {
            System.Console.WriteLine(sender + format, args);
        }

    }
}
