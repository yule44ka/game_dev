using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour

{
    Vector3 localRotation;
    bool cameraDisabled = false, rotateDisabled = false;
    public CubeManager cubeManager;
    List<GameObject> pieces = new List<GameObject>(), planes = new List<GameObject>();

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(0)) {

            // Cube movements by mouse
            if (!rotateDisabled)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    cameraDisabled = true;

                    if (pieces.Count < 2 &&
                        !pieces.Exists(x => x == hit.collider.transform.parent.gameObject) &&
                        hit.transform.parent.gameObject != cubeManager.gameObject)
                    {
                        pieces.Add(hit.collider.transform.parent.gameObject);
                        planes.Add(hit.collider.gameObject);
                    }

                    else if (pieces.Count == 2)
                    {
                        cubeManager.
                    }
                }


                // Camera movements by mouse
                if (!cameraDisabled)
                {
                    localRotation.x += Input.GetAxis("Mouse X") * 15;
                    localRotation.y += Input.GetAxis("Mouse Y") * -15;
                    localRotation.y = Mathf.Clamp(localRotation.y, -50, 50);
                }
            }
            Quaternion qt = Quaternion.Euler(localRotation.y, localRotation.x, 0);
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, qt, Time.deltaTime * 15);

        }
    } 
}
