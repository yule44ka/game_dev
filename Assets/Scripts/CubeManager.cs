using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject CubePiece;
    Transform CubeTransf;
    List<GameObject> AllCubePieces = new List<GameObject>();
    GameObject CubeCenter;
    bool canRotate = true;

    // Rotation vectors
    Vector3[] RotationVectors =
    {
        new Vector3 (0, 1, 0), new Vector3 (0, -1, 0),
        new Vector3(0, 0, -1), new Vector3(0, 0, 1),
        new Vector3(1, 0, 0), new Vector3(-1, 0, 0)
    };

    //Planes of cube
    List<GameObject> UpPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
        }
    }

    List<GameObject> DownPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
        }
    }

    List<GameObject> FrontPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
        }
    }

    List<GameObject> BackPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -2);
        }
    }

    List<GameObject> LeftPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
        }
    }

    List<GameObject> RightPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 2);
        }
    }

    // Inner plane of cube
    List<GameObject> FrontHorizontalPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CubeTransf = transform;
        CreateCube();
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate) 
            CheckInput();
    }

    //Restart game
    void CreateCube()
    {
        foreach (GameObject go in AllCubePieces)
        {
            DestroyImmediate(go);
        }

        AllCubePieces.Clear();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++) 
            { 
                for (int z = 0; z < 3; z++)
                {
                    GameObject gameObj = Instantiate(CubePiece, CubeTransf, false);
                    gameObj.transform.localPosition = new Vector3(-x, -y, z);
                    gameObj.GetComponent<CubePieceManager>().SetColor(-x, -y, z);
                    AllCubePieces.Add(gameObj);
                }
            }
        }
        CubeCenter = AllCubePieces[13];
    }

    void CheckInput()
    {   
        if (Input.GetKeyDown(KeyCode.E)) // Restart game
            CreateCube();
    }

    // Rotation
    IEnumerator Rotate(List<GameObject> pieces, Vector3 rotationVec, int speed = 5)
    {
        canRotate = false;
        int angle = 0;
        while (angle < 90)
        {
            foreach (GameObject piece in pieces)
            {
                piece.transform.RotateAround(CubeCenter.transform.position, rotationVec, speed);
            }
            angle += speed;
            yield return null;
        }
        canRotate = true;
    }

    // How rotate
    public void DetectRotate(List<GameObject> pieces, List<GameObject> planes)
    {
        if (!canRotate)
        {
            return;
        }

        else if (FrontHorizontalPieces.Exists(x => x == pieces[0]) &&
                 FrontHorizontalPieces.Exists(x => x == pieces[1]))
        {
            StartCoroutine(Rotate(FrontHorizontalPieces, new Vector3(0, 1 * DetectUpMiddleDown(pieces), 0)));
        }

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), UpPieces))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1 * DetectUpMiddleDown(pieces), 0)));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), DownPieces))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, 1 * DetectUpMiddleDown(pieces), 0)));

        else if (DetectSide(planes, new Vector3(0, 0, 1), new Vector3(0, 1, 0), FrontPieces))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1 * DetectFrontMiddleBack(pieces), 0, 0)));

        else if (DetectSide(planes, new Vector3(0, 0, 1), new Vector3(0, 1, 0), BackPieces))
            StartCoroutine(Rotate(BackPieces, new Vector3(1 * DetectFrontMiddleBack(pieces), 0, 0)));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 1, 0), LeftPieces))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, 1 * DetectLeftMiddleRightSign(pieces))));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 1, 0), RightPieces))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1 * DetectLeftMiddleRightSign(pieces))));
    }

    // What rotate
    bool DetectSide(List<GameObject> planes, Vector3 fDirection, Vector3 sDirection, List<GameObject> side)
    {
        GameObject centralPiece = side.Find(x => x.GetComponent<CubePieceManager>().Planes.FindAll(y => y.activeInHierarchy).Count == 1);

        List<RaycastHit> hit1 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, fDirection)),
                         hit2 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, fDirection)),
                         hit1_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -fDirection)),
                         hit2_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -fDirection)),

                         hit3 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, sDirection)),
                         hit4 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, sDirection)),
                         hit3_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -sDirection)),
                         hit4_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -sDirection));

        return hit1.Exists(x => x.collider.gameObject == centralPiece) ||
            hit2.Exists(x => x.collider.gameObject == centralPiece) ||
            hit1_m.Exists(x => x.collider.gameObject == centralPiece) ||
            hit2_m.Exists(x => x.collider.gameObject == centralPiece) ||

            hit3.Exists(x => x.collider.gameObject == centralPiece) ||
            hit4.Exists(x => x.collider.gameObject == centralPiece) ||
            hit3_m.Exists(x => x.collider.gameObject == centralPiece) ||
            hit4_m.Exists(x => x.collider.gameObject == centralPiece);
    } 
    
    // In what direction rotate
    float DetectLeftMiddleRightSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.y) != Mathf.Round(pieces[0].transform.position.y))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sign = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
            else
                sign = Mathf.Round(pieces[1].transform.position.y) - Mathf.Round(pieces[0].transform.position.y);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.y) == -2)
                sign = Mathf.Round(pieces[1].transform.position.x) - Mathf.Round(pieces[0].transform.position.x);
            else
                sign = Mathf.Round(pieces[0].transform.position.x) - Mathf.Round(pieces[1].transform.position.x);
        }

        return sign;
    }

    float DetectFrontMiddleBack(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.y) == 0)
                sign = Mathf.Round(pieces[1].transform.position.z) - Mathf.Round(pieces[0].transform.position.z);
            else
                sign = Mathf.Round(pieces[0].transform.position.z) - Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sign = Mathf.Round(pieces[1].transform.position.y) - Mathf.Round(pieces[0].transform.position.y);
            else
                sign = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
        }

        return sign;
    }

    float DetectUpMiddleDown(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sign = Mathf.Round(pieces[1].transform.position.z) - Mathf.Round(pieces[0].transform.position.z);
            else
                sign = Mathf.Round(pieces[0].transform.position.z) - Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sign = Mathf.Round(pieces[0].transform.position.x) - Mathf.Round(pieces[1].transform.position.x);
            else
                sign = Mathf.Round(pieces[1].transform.position.x) - Mathf.Round(pieces[0].transform.position.x);
        }

        return sign;
    }

}
