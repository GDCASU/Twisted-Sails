using UnityEngine;
using System.Collections;

//The only purpose of this script is to have a component always facing the camera
//Currently only used to have health bars above other ships be always visible
public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}