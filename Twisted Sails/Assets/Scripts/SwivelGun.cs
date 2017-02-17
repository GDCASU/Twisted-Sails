using UnityEngine;
using System.Collections;

// The SwivelGun class has a single method that sets the rotation of the object it is attached to equal 
// to the transforms rotation in the xz-plane. 
//
// Edward Borroughs 
// Version 2/1/2016

// Update:      Edward Borroughs
// Date:        February 16, 2016
// Description: The SwivelGun class now extends BroadsideCannonFireNetworked and can be treated as
//              a broadside cannon with more functionality.

public class SwivelGun : BroadsideCannonFireNetworked {

    public void updateRotation(Transform objTransform)
    {
        this.transform.rotation = Quaternion.LookRotation(Vector3.up, new Vector3(objTransform.forward.x, 0, objTransform.forward.z));
    }
}
