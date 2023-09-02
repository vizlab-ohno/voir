//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class RoiUI : MonoBehaviour
{
    GameObject ROI;
    voirParam param;
    voirCoord coord;
    voirROI roi;

    int[] c1;
    int[] c2;
    int[] oc1;
    int[] oc2;
    float[] fm1;
    float[] fm2;

    int[] prev_c1;
    int[] prev_c2;
    int xmax, xmin, ymax, ymin, zmax, zmin;

    GameObject[] RFrame;
    LineRenderer[] RlineRenderer;
    LineRenderer beamRenderer;
    Vector3[] line;
    tracking trackd;
    MainControll maincontroll;
    Vector3[] contrdata;
    double[] tip;
    double[] dd;
    bool pressed;

    void Awake()
    {
        Init();
        Alloc();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (maincontroll.SelectedMethod != VizMethods.ROI) return;
        DrawBeam();
        DrawFrame();
    }

    public bool StartGUI()
    {
        if(!coord.insideWData(tip)) return false;

        trackd.getRContData(voirConst.TD_POS, ref contrdata[0]);
        trackd.getRContData(voirConst.TD_FORWARD, ref contrdata[1]);
        updatedb(contrdata[0], contrdata[1]);
        coord.getFact(tip, dd, oc1);

        for(int i=0;i<3;i++) c2[i] = c1[i] = oc2[i] = oc1[i];
        pressed = true;

        for(int i=0;i<12;i++)
        {
            RlineRenderer[i].startColor = Color.yellow;
            RlineRenderer[i].endColor = Color.yellow;
            RlineRenderer[i].positionCount = 2;
        }
        return true;
    }

    public bool CalcGUI()
    {
        if (!pressed || !coord.insideWData(tip)) return false;

        trackd.getRContData(voirConst.TD_POS, ref contrdata[0]);
        trackd.getRContData(voirConst.TD_FORWARD, ref contrdata[1]);
        updatedb(contrdata[0], contrdata[1]);
        coord.getFact(tip, dd, oc2);

        for (int i = 0; i < 3; i++)
        {
            if (oc1[i] > oc2[i])
            {
                c1[i] = oc2[i];
                c2[i] = oc1[i];
            } else
            {
                c1[i] = oc1[i];
                c2[i] = oc2[i];
            }
        }

        int rx = c2[0] - c1[0] + 1;
        int ry = c2[1] - c1[1] + 1;
        int rz = c2[2] - c1[2] + 1;

        // X-Direction
        if (rx < voirConst.RI_MINX)
        {
            for (int i = 0; i < 4; i++)
            {
                RlineRenderer[i].startColor = Color.yellow;
                RlineRenderer[i].endColor = Color.yellow;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                RlineRenderer[i].startColor = Color.cyan;
                RlineRenderer[i].endColor = Color.cyan;
            }
        }

        // Y-Direction
        if (ry < voirConst.RI_MINY)
        {
            for (int i = 4; i < 8; i++)
            {
                RlineRenderer[i].startColor = Color.yellow;
                RlineRenderer[i].endColor = Color.yellow;
            }
        }
        else
        {
            for (int i = 4; i < 8; i++)
            {
                RlineRenderer[i].startColor = Color.cyan;
                RlineRenderer[i].endColor = Color.cyan;
            }
        }

        // Z-Direction
        if (rz < voirConst.RI_MINZ)
        {
            for (int i = 8; i < 12; i++)
            {
                RlineRenderer[i].startColor = Color.yellow;
                RlineRenderer[i].endColor = Color.yellow;
            }
        }
        else
        {
            for (int i = 8; i < 12; i++)
            {
                RlineRenderer[i].startColor = Color.cyan;
                RlineRenderer[i].endColor = Color.cyan;
            }
        }

        return true;
    }

    public bool EndGUI(ref int [] cmin, ref int [] cmax)
    {
        if (!pressed || !coord.insideWData(tip)) return false;

        trackd.getRContData(voirConst.TD_POS, ref contrdata[0]);
        trackd.getRContData(voirConst.TD_FORWARD, ref contrdata[1]);
        updatedb(contrdata[0], contrdata[1]);

        coord.getFact(tip, dd, oc2);

        for (int i = 0; i < 3; i++)
        {
            if (oc1[i] > oc2[i])
            {
                c1[i] = oc2[i];
                c2[i] = oc1[i];
            }
            else
            {
                c1[i] = oc1[i];
                c2[i] = oc2[i];
            }
        }

        int rx = c2[0] - c1[0] + 1;
        int ry = c2[1] - c1[1] + 1;
        int rz = c2[2] - c1[2] + 1;

        pressed = false;

        if (rx < voirConst.RI_MINX || ry < voirConst.RI_MINY || rz < voirConst.RI_MINZ)
        {
            for(int i=0;i<3;i++)
            {
                cmin[i] = 0;
                cmax[i] = 0;
            }

            for(int i=0;i<12;i++)
                RlineRenderer[i].positionCount = 0;

            //            voirFunc.Log("ROI failed "+pressed);
            return false;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                cmin[i] = c1[i];
                cmax[i] = c2[i];
                prev_c1[i] = c1[i];
                prev_c2[i] = c2[i];
            }
//            voirFunc.Log("ROI success " + pressed);
            return true;
        }
    }

    void Init()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        ROI = GameObject.Find("ROI");
        roi = ROI.GetComponent<voirROI>();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();
        beamRenderer = gameObject.AddComponent<LineRenderer>();

    }

    void Alloc()
    {
        contrdata = new Vector3[2];
        line = new Vector3[2];
        for(int i=0;i<2;i++){
            contrdata[i] = new Vector3(0,0,0);
            line[i] = new Vector3(0,0,0);
        }

        tip = new double[3];
        dd = new double[3];

        c1 = new int[3];
        c2 = new int[3];
        fm1 = new float[3];
        fm2 = new float[3];
        oc1 = new int[3];
        oc2 = new int[3];
        prev_c1 = new int[3];
        prev_c2 = new int[3];

        RlineRenderer = new LineRenderer[12];
        RFrame = new GameObject[12];

        for (int i = 0; i < 12; i++)
        {
            RFrame[i] = new GameObject("RFrame" + i);
            RFrame[i].transform.SetParent(transform);
            RFrame[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            //RlineRenderer[i] = new LineRenderer();
            RlineRenderer[i] = RFrame[i].AddComponent<LineRenderer>();
            RlineRenderer[i].useWorldSpace = false;
            RlineRenderer[i].material = new Material(Shader.Find("Sprites/Default"));

            RlineRenderer[i].startWidth = 0.01f;
            RlineRenderer[i].endWidth = 0.01f;

            RlineRenderer[i].positionCount = 0;
            RlineRenderer[i].loop = false;
        }

        beamRenderer.startWidth = 0.01f;
        beamRenderer.endWidth = 0.01f;
        beamRenderer.startColor = Color.red;
        beamRenderer.endColor = Color.red;
        beamRenderer.positionCount = 0;
        beamRenderer.material = new Material(Shader.Find("Sprites/Default"));

        InitVariables();
    }

    void DrawBeam()
    {
        trackd.getRContData(voirConst.TD_POS, ref contrdata[0]);
        trackd.getRContData(voirConst.TD_FORWARD, ref contrdata[1]);

        updatedb(contrdata[0], contrdata[1]);
        beamRenderer.positionCount = 2;

        if(coord.insideWData(tip))
        {
            beamRenderer.startColor = Color.cyan;
            beamRenderer.endColor = Color.green;
        }
        else
        {
            beamRenderer.startColor = Color.red;
            beamRenderer.endColor = Color.red;
        }

    }

    void updatedb(Vector3 basp, Vector3 dir)
    {
        //
        tip[0] = dir.x * voirConst.BEAM_LEN + basp[0];
        tip[1] = dir.z * voirConst.BEAM_LEN + basp[2];
        tip[2] = dir.y * voirConst.BEAM_LEN + basp[1];

        line[0].x = basp.x;
        line[0].y = basp.y;
        line[0].z = basp.z;

        line[1].x = (float)tip[0];
        line[1].y = (float)tip[2];
        line[1].z = (float)tip[1];

        beamRenderer.SetPositions(line);
    }

    void InitVariables()
    {
        xmin = 0;
        xmax = coord.gn_[0] - 1;

        ymin = 0;
        ymax = coord.gn_[1] - 1;

        zmin = 0;
        zmax = coord.gn_[2] - 1;

        c1[0] = xmin; c2[0] = xmax;
        c1[1] = ymin; c2[1] = ymax;
        c1[2] = zmin; c2[2] = zmax;

        pressed = false;
    }

    void DrawFrame()
    {
        if (!pressed)
        {
            for (int i = 0; i < 12; i++)
            {
                RlineRenderer[i].positionCount = 0;
            }
            return;
        }
        else
        {
            for (int i = 0; i < 12; i++)
            {
                RlineRenderer[i].positionCount = 2;
            }
        }

        //voirFunc.Log("c1 " + c1[0] + " " + c1[1] + " " + c1[2]+ ", c2 " + c2[0] + " " + c2[1] + " " + c2[2]);
        fm1[0] = (float)coord.coordx[c1[0]];
        fm2[0] = (float)coord.coordx[c2[0]];
        fm1[1] = (float)coord.coordy[c1[1]];
        fm2[1] = (float)coord.coordy[c2[1]];
        fm1[2] = (float)coord.coordz[c1[2]];
        fm2[2] = (float)coord.coordz[c2[2]];

        for (int i = 0; i < 12; i++)
        {
            switch (i)
            {
                // xmin -> xmax
                case 0:
                    Vector3[] line0 = new Vector3[] {new Vector3(fm1[0], fm1[2], fm1[1]), new Vector3(fm2[0], fm1[2], fm1[1])};
                    RlineRenderer[i].SetPositions(line0);
                    break;

                case 1:
                    Vector3[] line1 = new Vector3[]{new Vector3(fm1[0], fm2[2], fm1[1]), new Vector3(fm2[0], fm2[2], fm1[1])};
                    RlineRenderer[i].SetPositions(line1);
                    break;

                case 2:
                    Vector3[] line2 = new Vector3[]{new Vector3(fm1[0], fm1[2], fm2[1]), new Vector3(fm2[0], fm1[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line2);
                    break;

                case 3:
                    Vector3[] line3 = new Vector3[]{new Vector3(fm1[0], fm2[2], fm2[1]), new Vector3(fm2[0], fm2[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line3);
                    break;

                // ymin->ymax
                case 4:
                    Vector3[] line4 = new Vector3[]{new Vector3(fm1[0], fm2[2], fm1[1]), new Vector3(fm1[0], fm2[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line4);
                    break;

                case 5:
                    Vector3[] line5 = new Vector3[]{new Vector3(fm2[0], fm2[2], fm1[1]), new Vector3(fm2[0], fm2[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line5);
                    break;

                case 6:
                    Vector3[] line6 = new Vector3[]{new Vector3(fm1[0], fm1[2], fm1[1]), new Vector3(fm1[0], fm1[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line6);
                    break;

                case 7:
                    Vector3[] line7 = new Vector3[]{new Vector3(fm2[0], fm1[2], fm1[1]), new Vector3(fm2[0], fm1[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line7);
                    break;

                // zmin->zmax
                case 8:
                    Vector3[] line8 = new Vector3[]{new Vector3(fm1[0], fm1[2], fm1[1]), new Vector3(fm1[0], fm2[2], fm1[1])};
                    RlineRenderer[i].SetPositions(line8);
                    break;

                case 9:
                    Vector3[] line9 = new Vector3[]{new Vector3(fm2[0], fm1[2], fm1[1]), new Vector3(fm2[0], fm2[2], fm1[1])};
                    RlineRenderer[i].SetPositions(line9);
                    break;

                case 10:
                    Vector3[] line10 = new Vector3[]{new Vector3(fm1[0], fm1[2], fm2[1]), new Vector3(fm1[0], fm2[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line10);
                    break;

                case 11:
                    Vector3[] line11 = new Vector3[]{new Vector3(fm2[0], fm1[2], fm2[1]), new Vector3(fm2[0], fm2[2], fm2[1])};
                    RlineRenderer[i].SetPositions(line11);
                    break;

            }
            RlineRenderer[i].positionCount = 2;
            //Debug.Log(i);
        }

    }

}
