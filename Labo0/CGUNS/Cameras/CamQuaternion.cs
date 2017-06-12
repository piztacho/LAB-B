using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace CGUNS.Cameras
{
    /// <summary>
    /// Representa una Camara en coordenadas esfericas.
    /// La camara apunta y orbita alrededor del origen de coordenadas (0,0,0).
    /// El vector "up" de la camara es esl eje "Y" (0,1,0).
    /// La posicion de la camara esta dada por 3 valores: Radio, Theta, Phi.
    /// </summary>
    class CamQuaternion
    {
        private const float DEG2RAD = (float)(Math.PI / 180.0); //Para pasar de grados a radianes

        private Matrix4 projMatrix; //Matriz de Proyeccion.

        private float radius; //Distancia al origen.


        //Valores necesarios para calcular la Matriz de Vista.
        private Vector3 eye;// = new Vector3(0.0f, 0.0f, 0.0f);

        Quaternion cameraRot;// = new Quaternion(0,0,0,1); //tiene la identidad


        public CamQuaternion()
        {
            //Por ahora la matriz de proyeccion queda fija. :)
            float fovy = 50 * DEG2RAD; //50 grados de angulo.
            float aspectRadio = 1; //Cuadrado
            float zNear = 0.1f; //Plano Near
            float zFar = 1000f;  //Plano Far

            //Posicion inicial de la camara.
            radius = 45.0f;
            eye = new Vector3(0.0f, 0.0f, radius);
            cameraRot = new Quaternion(0, 0, 0, 1);

            projMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspectRadio, zNear, zFar);


        }

        /// <summary>
        /// Retorna la Matriz de Projeccion que esta utilizando esta camara.
        /// </summary>
        /// <returns></returns>
        public Matrix4 getProjectionMatrix()
        {
            return projMatrix;
        }
        /// <summary>
        /// Retorna la Matriz de Vista que representa esta camara.
        /// </summary>
        /// <returns></returns>
        public Matrix4 getViewMatrix()
        {

            //Construimos la matriz y la devolvemos.
            eye.Z = radius;
            Matrix4 translacion = Matrix4.CreateTranslation(-eye);
            Matrix4 rotacion = Matrix4.CreateFromQuaternion(cameraRot);
            return rotacion * translacion;
        }


        public void Acercar(float distance)
        {
            if ((distance > 0) && (distance < radius))
            {
                radius = radius - distance;
            }
        }

        public void Alejar(float distance)
        {
            if (distance > 0)
            {
                radius = radius + distance;
            }
        }

        private float deltaTheta = 0.1f;
        private float deltaPhi = 0.1f;

        public void Arriba()
        {
            Quaternion QuatTemporal = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), deltaPhi);
            QuatTemporal.Normalize();
            cameraRot = QuatTemporal * cameraRot;

        }

        public void Abajo()
        {
            Quaternion QuatTemporal = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), -deltaPhi);
            QuatTemporal.Normalize();
            cameraRot = QuatTemporal * cameraRot;

        }

        public void Izquierda()
        {
            Quaternion QuatTemporal = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), deltaTheta);
            QuatTemporal.Normalize();
            cameraRot = cameraRot * QuatTemporal;
        }

        public void Derecha()
        {
            Quaternion QuatTemporal = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -deltaTheta);
            QuatTemporal.Normalize();
            cameraRot = cameraRot * QuatTemporal;
        }


    }
}
