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

    void CreateCube()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++) { 
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
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1, 0)));
        else if (Input.GetKeyDown(KeyCode.S))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, -1, 0)));
        else if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, -1)));
        else if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1)));
        else if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1, 0, 0)));
        else if (Input.GetKeyDown(KeyCode.X))
            StartCoroutine(Rotate(BackPieces, new Vector3(-1, 0, 0)));
    }

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
}
