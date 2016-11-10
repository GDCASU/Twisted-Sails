using UnityEngine;
using System.Collections;

//The only purpose of this script is to have a component always facing the camera
//Currently only used to have health bars above other ships be always visible

//Developer: Kyle Aycock
//Date: 11/3/2016
//Fixed so that text is oriented towards the camera properly.
//Previously any billboarded text appeared mirrored.
public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward,Vector3.up);
    }
}