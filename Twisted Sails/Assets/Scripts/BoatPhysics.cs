using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Boat
{
    public class BoatPhysics : MonoBehaviour
    {
        //Drags
        public GameObject underWaterObj;

        //Determines what part of the boat mesh is above water
        private ModifyBoatMesh modifyBoatMesh;

        //Mesh for debugging
        private Mesh underWaterMesh;

        //The boats rigidbody
        private Rigidbody boatRB;

        //The density of the water the boat is traveling in
        private float rhoWater = 1027f;

        void Start()
        {
            //Get the boat's rigidbody
            boatRB = gameObject.GetComponent<Rigidbody>();

            //Init the script that will modify the boat mesh
            modifyBoatMesh = new ModifyBoatMesh(gameObject);

            //Meshes that are below and above the water
            underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
        }

        void Update()
        {
            //Generate the under water mesh
            modifyBoatMesh.GenerateUnderwaterMesh();

            //Display the under water mesh
            modifyBoatMesh.DisplayMesh(underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);
        }

        void FixedUpdate()
        {
            //Add forces to the part of the boat that's below the water
            if (modifyBoatMesh.underWaterTriangleData.Count > 0)
            {
                AddUnderWaterForces();
            }
        }

        //Add all forces that act on the squares below the water
        void AddUnderWaterForces()
        {
            //Get all triangles
            List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;

            for (int i = 0; i < underWaterTriangleData.Count; i++)
            {
                //This triangle
                TriangleData triangleData = underWaterTriangleData[i];

                //Calculate the buoyancy force
                Vector3 buoyancyForce = BuoyancyForce(rhoWater, triangleData);

                //Add the force to the boat
                boatRB.AddForceAtPosition(buoyancyForce, triangleData.center);


                //Debug

                //Normal
                //Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

                //Buoyancy
                //Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
            }
        }

        //The buoyancy force
        private Vector3 BuoyancyForce(float rho, TriangleData triangleData)
        {
            Vector3 buoyancyForce = rho * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

            buoyancyForce.x = 0f;
            buoyancyForce.z = 0f;

            return buoyancyForce;
        }
    }
}