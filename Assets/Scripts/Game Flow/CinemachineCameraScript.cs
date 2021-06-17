using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineCameraScript : MonoBehaviour
{
    public void setFollowPlayer(Transform transform)
    {
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = transform;
    }
}
