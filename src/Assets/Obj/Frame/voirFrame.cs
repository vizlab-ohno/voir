//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using voirCommon;

public class voirFrame : MonoBehaviour
{
    GameObject[] Frame;
    LineRenderer[] lineRenderer;

    GameObject[] FrameW;
    LineRenderer[] lineRendererW;
    float[] cmin;
    float[] cmax;
    float[] cminw;
    float[] cmaxw;
    float[] hdx;
    voirCoord coord;

    void Awake()
    {
        Alloc();
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        DrawFrame();
        DrawFrameW();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawFrame()
    {
        for (int i = 0; i < 12; i++)
        {
            switch (i)
            {
                // xmin -> xmax
                case 0:
                    lineRenderer[i].startColor = Color.red;
                    lineRenderer[i].endColor = Color.red;

                    var line0 = new Vector3[]
                    {
                        new Vector3(cmin[0], cmin[2], cmin[1]),
                        new Vector3(cmax[0], cmin[2], cmin[1])
                    };
                    lineRenderer[i].SetPositions(line0);
                    break;

                case 1:
                    lineRenderer[i].startColor = Color.red;
                    lineRenderer[i].endColor = Color.red;

                    var line1 = new Vector3[]
                    {
                        new Vector3(cmin[0], cmax[2], cmin[1]),
                        new Vector3(cmax[0], cmax[2], cmin[1])
                    };
                    lineRenderer[i].SetPositions(line1);
                    break;

                case 2:
                    lineRenderer[i].startColor = Color.red;
                    lineRenderer[i].endColor = Color.red;

                    var line2 = new Vector3[]
                    {
                        new Vector3(cmin[0], cmin[2], cmax[1]),
                        new Vector3(cmax[0], cmin[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line2);
                    break;

                case 3:
                    lineRenderer[i].startColor = Color.red;
                    lineRenderer[i].endColor = Color.red;

                    var line3 = new Vector3[]
                    {
                        new Vector3(cmin[0], cmax[2], cmax[1]),
                        new Vector3(cmax[0], cmax[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line3);
                    break;


                // ymin->ymax
                case 4:
                    lineRenderer[i].startColor = Color.green;
                    lineRenderer[i].endColor = Color.green;

                    var line4 = new Vector3[]
                    {
                        new Vector3(cmin[0], cmax[2], cmin[1]),
                        new Vector3(cmin[0], cmax[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line4);
                    break;


                case 5:
                    lineRenderer[i].startColor = Color.green;
                    lineRenderer[i].endColor = Color.green;

                    var line5 = new Vector3[]
                    {
                        new Vector3(cmax[0], cmax[2], cmin[1]),
                        new Vector3(cmax[0], cmax[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line5);
                    break;


                case 6:
                    lineRenderer[i].startColor = Color.green;
                    lineRenderer[i].endColor = Color.green;

                    var line6 = new Vector3[]
                    {
                        new Vector3(cmin[0], cmin[2], cmin[1]),
                        new Vector3(cmin[0], cmin[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line6);
                    break;


                case 7:
                    lineRenderer[i].startColor = Color.green;
                    lineRenderer[i].endColor = Color.green;

                    var line7 = new Vector3[]
                    {
                        new Vector3(cmax[0], cmin[2], cmin[1]),
                        new Vector3(cmax[0], cmin[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line7);
                    break;

                // zmin->zmax
                case 8:
                    lineRenderer[i].startColor = Color.blue;
                    lineRenderer[i].endColor = Color.blue;

                    var line8 = new Vector3[]
                    {
                    new Vector3(cmin[0], cmin[2], cmin[1]),
                    new Vector3(cmin[0], cmax[2], cmin[1])
                    };
                    lineRenderer[i].SetPositions(line8);
                    break;

                case 9:
                    lineRenderer[i].startColor = Color.blue;
                    lineRenderer[i].endColor = Color.blue;

                    var line9 = new Vector3[]
                    {
                    new Vector3(cmax[0], cmin[2], cmin[1]),
                    new Vector3(cmax[0], cmax[2], cmin[1])
                    };
                    lineRenderer[i].SetPositions(line9);
                    break;

                case 10:
                    lineRenderer[i].startColor = Color.blue;
                    lineRenderer[i].endColor = Color.blue;

                    var line10 = new Vector3[]
                    {
                    new Vector3(cmin[0], cmin[2], cmax[1]),
                    new Vector3(cmin[0], cmax[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line10);
                    break;

                case 11:
                    lineRenderer[i].startColor = Color.blue;
                    lineRenderer[i].endColor = Color.blue;

                    var line11 = new Vector3[]
                    {
                    new Vector3(cmax[0], cmin[2], cmax[1]),
                    new Vector3(cmax[0], cmax[2], cmax[1])
                    };
                    lineRenderer[i].SetPositions(line11);
                    break;

            }
            lineRenderer[i].positionCount = 2;
            //Debug.Log(i);
        }

    }

    void DrawFrameW()
    {
        for (int i = 0; i < 12; i++)
        {
            switch (i)
            {
                // xmin -> xmax
                case 0:
                    lineRendererW[i].startColor = Color.magenta;
                    lineRendererW[i].endColor = Color.magenta;

                    var line0 = new Vector3[]
                    {
                        new Vector3(cminw[0], cminw[2], cminw[1]),
                        new Vector3(cmaxw[0], cminw[2], cminw[1])
                    };
                    lineRendererW[i].SetPositions(line0);
                    break;

                case 1:
                    lineRendererW[i].startColor = Color.magenta;
                    lineRendererW[i].endColor = Color.magenta;

                    var line1 = new Vector3[]
                    {
                        new Vector3(cminw[0], cmaxw[2], cminw[1]),
                        new Vector3(cmaxw[0], cmaxw[2], cminw[1])
                    };
                    lineRendererW[i].SetPositions(line1);
                    break;

                case 2:
                    lineRendererW[i].startColor = Color.magenta;
                    lineRendererW[i].endColor = Color.magenta;

                    var line2 = new Vector3[]
                    {
                        new Vector3(cminw[0], cminw[2], cmaxw[1]),
                        new Vector3(cmaxw[0], cminw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line2);
                    break;

                case 3:
                    lineRendererW[i].startColor = Color.magenta;
                    lineRendererW[i].endColor = Color.magenta;

                    var line3 = new Vector3[]
                    {
                        new Vector3(cminw[0], cmaxw[2], cmaxw[1]),
                        new Vector3(cmaxw[0], cmaxw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line3);
                    break;


                // ymin->ymax
                case 4:
                    lineRendererW[i].startColor = Color.yellow;
                    lineRendererW[i].endColor = Color.yellow;

                    var line4 = new Vector3[]
                    {
                        new Vector3(cminw[0], cmaxw[2], cminw[1]),
                        new Vector3(cminw[0], cmaxw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line4);
                    break;


                case 5:
                    lineRendererW[i].startColor = Color.yellow;
                    lineRendererW[i].endColor = Color.yellow;

                    var line5 = new Vector3[]
                    {
                        new Vector3(cmaxw[0], cmaxw[2], cminw[1]),
                        new Vector3(cmaxw[0], cmaxw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line5);
                    break;


                case 6:
                    lineRendererW[i].startColor = Color.yellow;
                    lineRendererW[i].endColor = Color.yellow;

                    var line6 = new Vector3[]
                    {
                        new Vector3(cminw[0], cminw[2], cminw[1]),
                        new Vector3(cminw[0], cminw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line6);
                    break;


                case 7:
                    lineRendererW[i].startColor = Color.yellow;
                    lineRendererW[i].endColor = Color.yellow;

                    var line7 = new Vector3[]
                    {
                        new Vector3(cmaxw[0], cminw[2], cminw[1]),
                        new Vector3(cmaxw[0], cminw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line7);
                    break;

                // zmin->zmax
                case 8:
                    lineRendererW[i].startColor = Color.cyan;
                    lineRendererW[i].endColor = Color.cyan;

                    var line8 = new Vector3[]
                    {
                    new Vector3(cminw[0], cminw[2], cminw[1]),
                    new Vector3(cminw[0], cmaxw[2], cminw[1])
                    };
                    lineRendererW[i].SetPositions(line8);
                    break;

                case 9:
                    lineRendererW[i].startColor = Color.cyan;
                    lineRendererW[i].endColor = Color.cyan;

                    var line9 = new Vector3[]
                    {
                    new Vector3(cmaxw[0], cminw[2], cminw[1]),
                    new Vector3(cmaxw[0], cmaxw[2], cminw[1])
                    };
                    lineRendererW[i].SetPositions(line9);
                    break;

                case 10:
                    lineRendererW[i].startColor = Color.cyan;
                    lineRendererW[i].endColor = Color.cyan;

                    var line10 = new Vector3[]
                    {
                    new Vector3(cminw[0], cminw[2], cmaxw[1]),
                    new Vector3(cminw[0], cmaxw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line10);
                    break;

                case 11:
                    lineRendererW[i].startColor = Color.cyan;
                    lineRendererW[i].endColor = Color.cyan;

                    var line11 = new Vector3[]
                    {
                    new Vector3(cmaxw[0], cminw[2], cmaxw[1]),
                    new Vector3(cmaxw[0], cmaxw[2], cmaxw[1])
                    };
                    lineRendererW[i].SetPositions(line11);
                    break;

            }
            lineRendererW[i].positionCount = 2;
            //Debug.Log(i);
        }

    }

    void Alloc()
    {
        lineRenderer = new LineRenderer[12];
        Frame = new GameObject[12];
        lineRendererW = new LineRenderer[12];
        FrameW = new GameObject[12];

        for (int i = 0; i < 12; i++)
        {
            Frame[i] = new GameObject("Frame" + i);
            Frame[i].transform.SetParent(transform);
            Frame[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            //lineRenderer[i] = new LineRenderer();
            lineRenderer[i] = Frame[i].AddComponent<LineRenderer>();
            lineRenderer[i].useWorldSpace = false;
            lineRenderer[i].material = new Material(Shader.Find("Sprites/Default"));

            lineRenderer[i].startWidth = 0.01f;
            lineRenderer[i].endWidth = 0.01f;

            lineRenderer[i].loop = false;
        }

        for (int i = 0; i < 12; i++)
        {
            FrameW[i] = new GameObject("FrameW" + i);
            FrameW[i].transform.SetParent(transform);
            FrameW[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            //lineRendererW[i] = new LineRenderer();
            lineRendererW[i] = FrameW[i].AddComponent<LineRenderer>();
            lineRendererW[i].useWorldSpace = false;
            lineRendererW[i].material = new Material(Shader.Find("Sprites/Default"));

            lineRendererW[i].startWidth = 0.01f;
            lineRendererW[i].endWidth = 0.01f;

            lineRendererW[i].loop = false;
        }
        cmin = new float[3];
        cmax = new float[3];
        cminw = new float[3];
        cmaxw = new float[3];
        hdx = new float[3];
    }

    void CoordSet()
    {
        cmin[0] = (float)coord.coordx[coord.minic[0]];
        cmax[0] = (float)coord.coordx[coord.maxic[0]];
        cmin[1] = (float)coord.coordy[coord.minic[1]];
        cmax[1] = (float)coord.coordy[coord.maxic[1]];
        cmin[2] = (float)coord.coordz[coord.minic[2]];
        cmax[2] = (float)coord.coordz[coord.maxic[2]];

        cminw[0] = (float)coord.coordx[0];
        cmaxw[0] = (float)coord.coordx[coord.gn_[0] - 1];
        cminw[1] = (float)coord.coordy[0];
        cmaxw[1] = (float)coord.coordy[coord.gn_[1] - 1];
        cminw[2] = (float)coord.coordz[0];
        cmaxw[2] = (float)coord.coordz[coord.gn_[2] - 1];

        hdx[0] = (float)coord.dx[0] * 0.5f; hdx[1] = (float)coord.dx[1] * 0.5f; hdx[2] = (float)coord.dx[2] * 0.5f;
    }

    void Init()
    {
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        CoordSet();
    }

    public void FrameROI()
    {
        CoordSet();
        DrawFrame();
        DrawFrameW();
    }
}
