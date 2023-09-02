//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirArrows : MonoBehaviour
{
    [SerializeField] GameObject arrowObj;
    double[,] alpos; // arrows' local positions
    double[] apos; // arrow's position
    double[] val; // vector value of arrow's position
    float[] valf; // vector value of arrow's position
    int NumArrows;
    GameObject[] arrows;// = new GameObject[5];
    bool Active;
    double[] tip;
    float[] bas;
    float[] tipf;
    float[] direc;
    int nvec;
    voirParam param;
    voirCoord coord;
    voirData data;

    LineRenderer lineRenderer;
    Vector3[] line;

    Vector3[] contrdata;

    tracking trackd;
    MainControll maincontroll;

    void Awake()
    {
        Init();
        AllocArrows();
        SetPosition();
        InitVariable();
        InitArrows();
    }


    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
     }


    // Update is called once per frame
    void Update()
    {
        if (maincontroll.SelectedMethod != VizMethods.LocalArrows) return;
        
        Beam();
        if (trackd.getRGripChange() == 1)
        {
            maincontroll.SetNone();
            gameObject.SetActive(false);
        }
        if (trackd.RTrigState())
        {
            lineRenderer.positionCount = 2;
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

        if (trackd.RTrigState())
            calc();
    }

    void Init()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        data = GameObject.Find("Data").GetComponent<voirData>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();
    }

    void Beam()
    {
        if (trackd.RTrigState())
        {
            trackd.getRContData(0, ref contrdata[0]);
            trackd.getRContData(1, ref contrdata[1]);

            updatedb(contrdata[0], contrdata[1]);
        }
    }

    public void updatedb(Vector3 basp, Vector3 dir)
    {
        //
        bas[0] = basp.x;
        bas[1] = basp.y;
        bas[2] = basp.z;

        tip[0] = dir.x * voirConst.BEAM_LEN + basp[0];
        tip[1] = dir.z * voirConst.BEAM_LEN + basp[2];
        tip[2] = dir.y * voirConst.BEAM_LEN + basp[1];

        direc[0] = dir.x;
        direc[1] = dir.y;
        direc[2] = dir.z;

        tipf[0] = (float)tip[0];
        tipf[1] = (float)tip[2];
        tipf[2] = (float)tip[1];

        line[0].x = basp.x;
        line[0].y = basp.y;
        line[0].z = basp.z;

        line[1].x = tipf[0];
        line[1].y = tipf[1];
        line[1].z = tipf[2];

        lineRenderer.SetPositions(line);

        //voirFunc.Log("x:"+flbase[0]+" y:"+flbase[1]+" z:"+flbase[2]);
        //voirFunc.Log("x:" + fldirec[0] + " y:" + fldirec[1] + " z:" + fldirec[2]);
    }

    public void GetPos(double [] pos)
    {
        pos[0] = tip[0];
        pos[1] = tip[1];
        pos[2] = tip[2];
    }

    public void LocalArrowsROI(double[] pos)
    {
        tip[0] = pos[0];
        tip[1] = pos[1];
        tip[2] = pos[2];

        // For BEAM
        line[0].x = (float)tip[0] - direc[0] * voirConst.BEAM_LEN;
        line[0].y = (float)tip[2] - direc[1] * voirConst.BEAM_LEN;
        line[0].z = (float)tip[1] - direc[2] * voirConst.BEAM_LEN;

        line[1].x = (float)tip[0];
        line[1].y = (float)tip[2];
        line[1].z = (float)tip[1];

        lineRenderer.SetPositions(line);

        calc();
    }

    void calc()
    {
        if (param.single) calcf();
        else calcd();
    }

    void calcd()
    {
        double maxvalue;
        float scale;
        bool inside;
        if (nvec < 0) {
            voirFunc.Error("arrow calc: nvec < 0");
        }
        //voirFunc.Log("nvec:"+nvec);
        maxvalue = data.voirVectData[nvec].maxvalue;
        for (int i = 0; i < NumArrows; i++)
        {
            apos[0] = tip[0] + alpos[i, 0];
            apos[1] = tip[1] + alpos[i, 1];
            apos[2] = tip[2] + alpos[i, 2];


            inside = data.getVectVal(nvec, apos, val);
            if (inside)
            {
                arrows[i].SetActive(true);
                //y,z -> z,y
                arrows[i].GetComponent<Transform>().localPosition
                    = new Vector3((float)apos[0], (float)apos[2], (float)apos[1]);

                scale = (float)(Math.Sqrt(val[0] * val[0] + val[1] * val[1] + val[2] * val[2]) / maxvalue);
                arrows[i].GetComponent<Transform>().localScale
                    = new Vector3(1.0f, 1.0f, scale);

                arrows[i].GetComponent<Transform>().localRotation
                    = Quaternion.LookRotation(new Vector3((float)val[0], (float)val[2], (float)val[1]));
            }
            else {
                arrows[i].SetActive(false);
            }

            //            Debug.Log(alrot[i,0]+","+ alrot[i, 1] + ","+alrot[i, 2]);
        }

    }

    void calcf()
    {
        float maxvalue;
        float scale;
        bool inside;
        if (nvec < 0)
        {
            voirFunc.Error("arrow calc: nvec < 0");
        }

        maxvalue = (float)data.voirVectData[nvec].maxvalue;
        for (int i = 0; i < NumArrows; i++)
        {
            apos[0] = tip[0] + alpos[i, 0];
            apos[1] = tip[1] + alpos[i, 1];
            apos[2] = tip[2] + alpos[i, 2];


            inside = data.getVectValf(nvec, apos, valf);
            if (inside)
            {
                arrows[i].SetActive(true);
                //y,z -> z,y
                arrows[i].GetComponent<Transform>().localPosition
                    = new Vector3((float)apos[0], (float)apos[2], (float)apos[1]);

                scale = Mathf.Sqrt(valf[0] * valf[0] + valf[1] * valf[1] + valf[2] * valf[2]) / maxvalue;
                arrows[i].GetComponent<Transform>().localScale
                    = new Vector3(1.0f, 1.0f, scale);

                arrows[i].GetComponent<Transform>().localRotation
                    = Quaternion.LookRotation(new Vector3(valf[0], valf[2], valf[1]));
            }
            else
            {
                arrows[i].SetActive(false);
            }

            //            Debug.Log(alrot[i,0]+","+ alrot[i, 1] + ","+alrot[i, 2]);
        }

    }


    void AllocArrows()
    {
        apos = new double[3];
        contrdata = new Vector3[2]{new Vector3(0f,0f,0f), new Vector3(0f,0f,0f)};

        if (param.single) valf = new float[3];
        else val = new double[3];
        bas = new float[3];
        direc = new float[3];

        int narrow = 0;
        double dx = voirConst.LA_DIAMETER / (voirConst.LA_DIV - 1);
        double rad = voirConst.LA_DIAMETER / 2.0;
        // apos is used as temporary variables here
        for (int k = 0; k < voirConst.LA_DIV; k++)
        {
            apos[2] = dx * k - voirConst.LA_DIV / 2.0 * dx + dx / 2.0;
            for (int j = 0; j < voirConst.LA_DIV; j++)
            {
                apos[1] = dx * j - voirConst.LA_DIV / 2.0 * dx + dx / 2.0;
                for (int i = 0; i < voirConst.LA_DIV; i++)
                {
                    apos[0] = dx * i - voirConst.LA_DIV / 2.0 * dx + dx / 2.0;
                    if (apos[0] * apos[0] + apos[1] * apos[1] + apos[2] * apos[2] <= rad*rad)
                    {
                        narrow++;
                    }
                }

            }
        }

        NumArrows = narrow;
        alpos = new double[narrow, 3];
        voirFunc.Log("Num arrows ="+NumArrows);
        line = new Vector3[2]{new Vector3(0f,0f,0f), new Vector3(0f,0f,0f)};
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor= Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void SetPosition()
    {
        double dx = voirConst.LA_DIAMETER / (voirConst.LA_DIV - 1);
        double rad = voirConst.LA_DIAMETER / 2.0;
        int tnum = 0;
        // apos is used as temporary variables here
        for (int k = 0; k < voirConst.LA_DIV; k++)
        {
            apos[2] = dx * k - voirConst.LA_DIV / 2.0 * dx + dx / 2.0;
            for (int j = 0; j < voirConst.LA_DIV; j++)
            {
                apos[1] = dx * j - voirConst.LA_DIV / 2.0 * dx + dx / 2.0;
                for (int i = 0; i < voirConst.LA_DIV; i++)
                {
                    apos[0] = dx * i - voirConst.LA_DIV / 2.0 * dx + dx / 2.0;
                    if (apos[0] * apos[0] + apos[1] * apos[1] + apos[2] * apos[2] <= rad* rad)
                    {
                        alpos[tnum, 0] = apos[0];
                        alpos[tnum, 1] = apos[1];
                        alpos[tnum, 2] = apos[2];
                        tnum++;
                    }
                }

            }
        }

        if(tnum != NumArrows)
        {
            voirFunc.Error("NumArrows is strange");
        }

    }

    public void Toggle(int nv)
    {
        if (nv < 0 || nv >= data.nvect) voirFunc.Log("arrow toggle nv is strange");
        if (Active)
        {
            Active = false;
            nvec = -1;
        }
        else
        {
            nvec = nv;
            Active = true;
        }
    }

    void InitVariable()
    {
        Active = false;
        tip = new double[3];
        tipf = new float[3];
        nvec = 0;
        for(int i=0;i<3;i++)
        {
            tip[i] = 0.0;
            tipf[i] = 0.0f;
        }

    }

    private void InitArrows()
    {
        arrows = new GameObject[NumArrows];
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());

        for (int i = 0; i < NumArrows; i++)
        {
            arrows[i]
                = Instantiate(arrowObj, new Vector3(0.0f, 0.0f, 0.0f),
                Quaternion.identity) as GameObject;
            arrows[i].transform.SetParent(transform);
            arrows[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            arrows[i].GetComponent<Transform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            arrows[i].SetActive(false);
        }
    }
    public void SetVectNum(int n)
    {
        nvec = n;
    }
}
