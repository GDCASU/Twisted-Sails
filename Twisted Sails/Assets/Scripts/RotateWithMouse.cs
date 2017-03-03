using UnityEngine;
using System.Collections;

/* Programmer:  Nizar Kury
 * Date:        November 17, 2016
 * Description: Modified code found on here: http://answers.unity3d.com/questions/177391/drag-to-rotate-gameobject.html
 *              Clicking on an object and moving the mouse will rotate said object. This is primarily used for the ship 
 *              select screen. It gives players a better understanding of the design for the ship. We can also add a
 *              panel where the chat is for statistics (?)
*/

public class RotateWithMouse : MonoBehaviour {

    public float _sensitivity; // how sensitive the rotation is to the mouse-- .4 is standard
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;

    void Start()
    {
        _rotation = Vector3.zero;
    }

    void Update()
    {
        if (_isRotating)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);

            // apply rotation
            _rotation.y = -(_mouseOffset.x) * _sensitivity;

            // rotate
            transform.Rotate(_rotation);

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    public void OnMouseDown()
    {
        Debug.Log("clicked");
        // rotating flag
        _isRotating = true;

        // store mouse
        _mouseReference = Input.mousePosition;
    }

    public void OnMouseUp()
    {
        // rotating flag
        _isRotating = false;
    } 
}
