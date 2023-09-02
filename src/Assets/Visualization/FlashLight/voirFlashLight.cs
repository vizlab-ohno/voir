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

public class voirFlashLight : MonoBehaviour
{
    [SerializeField] GameObject ptclObj;
    GameObject[] ptcls;// = new GameObject[5];

    MeshFilter MeshFilter;
    Mesh snow;
    Vector3[] snow_pos; // x,z,y
    //int[] snow_index;
    Color[] snow_color;
    voirParam param;
    voirCoord coord;
    voirData data;
    Vector3 hfd;
    Vector3 hup;

    int nvec;
    double[] val; // vector value of a position
    float[] valf; // vector value of a position
    double[] pos; // a position

    float[] fldirec;
    float[] xax;
    float[] yax;
    float[] lxax;
    float[] lyax;
    float [] flbase;
    float[] rpos; //relative position
    tracking trackd;
    MainControll maincontroll;

    float costheta, tantheta;

    void Awake()
    {
        Alloc();
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
        InitPtcls();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();
        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        calcp();

        if (maincontroll.SelectedMethod != VizMethods.FlashLight) return;
 
        if (trackd.getRGripChange() == 1)
        {
            maincontroll.SetNone();
            gameObject.SetActive(false);
        }

        if (trackd.RTrigState())
        {
            Vector3 pos = Vector3.zero;
            Vector3 dir = Vector3.up;
            trackd.getRContData(0, ref pos);
            trackd.getRContData(1, ref dir);
            updatedb(pos, dir);
        }
 
    }

    void InitPtcls()
    {
        ptcls = new GameObject[voirConst.FL_NUM_SNOW];
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());

        for (int i = 0; i < voirConst.FL_NUM_SNOW; i++)
        {
            ptcls[i]
                = Instantiate(ptclObj, new Vector3(0.0f, 0.0f, 0.0f),
                Quaternion.identity) as GameObject;
            ptcls[i].transform.SetParent(transform);
            ptcls[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            ptcls[i].GetComponent<Transform>().localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
        }
    }

    public void updatedb(Vector3 bas, Vector3 dir)
    {
		fldirec[0] = dir.x; fldirec[1] = dir.z; fldirec[2] = dir.y;
        voirFunc.Normalize(fldirec);
        flbase[0] = bas.x; flbase[1] = bas.z; flbase[2] = bas.y;
        //voirFunc.Log("x:"+flbase[0]+" y:"+flbase[1]+" z:"+flbase[2]);
        //voirFunc.Log("x:" + fldirec[0] + " y:" + fldirec[1] + " z:" + fldirec[2]);
    }
    
    public void GetFLPos(double [] pos)
    {
        pos[0] = (double)flbase[0];
        pos[1] = (double)flbase[1];
        pos[2] = (double)flbase[2];
    }
    
    public void FlashLightROI(double[] pos)
    {
        flbase[0] = (float)pos[0];
        flbase[1] = (float)pos[1];
        flbase[2] = (float)pos[2];
        calcp();
    }

    void calcp()
    {
        double h = coord.h * voirConst.DH;
        double maxvalue;
        if (nvec < 0)
        {
            voirFunc.Error("flash calc: nvec < 0");
        }
        int iter = (int)coord.relatelen;
        if (iter == 0) iter = 1;

        maxvalue = data.voirVectData[nvec].maxvalue;
        double dt = h/maxvalue;
        trackd.getHeadData(voirConst.TD_FORWARD, ref hfd);
        trackd.getHeadData(voirConst.TD_UP, ref hup);
        Quaternion prot = Quaternion.LookRotation(hfd, hup);
        if (param.single)
        {
            for (int i = 0; i < voirConst.FL_NUM_SNOW; i++)
            {
                for (int k = 0; k < iter* voirConst.SL_CALC_ACC; k++)
                {
                    pos[0] = (double)snow_pos[i][0];
                    pos[1] = (double)snow_pos[i][2];
                    pos[2] = (double)snow_pos[i][1];
                    if (!data.getVectValf(nvec, pos, valf) || !insideFLight(pos))
                    {
                        place_snow(pos);
                        snow_pos[i][0] = (float)pos[0];
                        snow_pos[i][1] = (float)pos[2];
                        snow_pos[i][2] = (float)pos[1];
                        continue;
                    }
                    pos[0] = pos[0] + valf[0] * dt;
                    pos[1] = pos[1] + valf[1] * dt;
                    pos[2] = pos[2] + valf[2] * dt;

                    snow_pos[i][0] = (float)pos[0];
                    snow_pos[i][1] = (float)pos[2];
                    snow_pos[i][2] = (float)pos[1];
                }
                ptcls[i].SetActive(true);
                //y,z -> z,y
                ptcls[i].GetComponent<Transform>().localPosition
                    = new Vector3(snow_pos[i][0], snow_pos[i][1], snow_pos[i][2]);

                ptcls[i].GetComponent<Transform>().localRotation = prot;
            }

        }
        else
        {
            for (int i = 0; i < voirConst.FL_NUM_SNOW; i++)
            {
                for (int k = 0; k < iter* voirConst.SL_CALC_ACC; k++)
                {
                    pos[0] = (double)snow_pos[i][0];
                    pos[1] = (double)snow_pos[i][2];
                    pos[2] = (double)snow_pos[i][1];
                    if (!data.getVectVal(nvec, pos, val) || !insideFLight(pos))
                    {
                        place_snow(pos);
                        snow_pos[i][0] = (float)pos[0];
                        snow_pos[i][1] = (float)pos[2];
                        snow_pos[i][2] = (float)pos[1];
                        continue;
                    }
                    pos[0] = pos[0] + val[0] * dt;
                    pos[1] = pos[1] + val[1] * dt;
                    pos[2] = pos[2] + val[2] * dt;

                    snow_pos[i][0] = (float)pos[0];
                    snow_pos[i][1] = (float)pos[2];
                    snow_pos[i][2] = (float)pos[1];
                }
                ptcls[i].SetActive(true);
                //y,z -> z,y
                ptcls[i].GetComponent<Transform>().localPosition
                    = new Vector3(snow_pos[i][0], snow_pos[i][1], snow_pos[i][2]);

                ptcls[i].GetComponent<Transform>().localRotation = prot;
            }
        }

    }


    bool insideFLight(double [] pos)
    {
        float cost, length;
        rpos[0] = (float)pos[0] - flbase[0];
        rpos[1] = (float)pos[1] - flbase[1];
        rpos[2] = (float)pos[2] - flbase[2];
        length = voirFunc.innerPro(rpos, fldirec);
        if(length > 0.0f && length <= voirConst.FL_LLENGTH)
        {
            cost = length/voirFunc.Normalize(rpos);
            //if (cost < Math.Cos(voirConst.FL_THETA)) return false;
            if (cost < costheta) return false;
        }
        else { return false; }

        return true;
    }

    void place_snow(double[] spos)
    {
        float z, r, phi;
        float tmp;
        z = UnityEngine.Random.Range(0.0f, 1.0f);
        r = UnityEngine.Random.Range(0.0f, 1.0f);
        phi = UnityEngine.Random.Range(0.0f, (float)(2.0*voirConst.PI));
        z = Mathf.Pow(z, 0.3333333f);
        r = Mathf.Sqrt(r);

        z = z * voirConst.FL_LLENGTH;
        r = r * z * tantheta;//(float)Math.Tan(voirConst.FL_THETA);

        voirFunc.crossPro(fldirec, xax, lyax);
        tmp = voirFunc.Normalize(lyax);
        if ( tmp <= (float)1.0E-2){
            voirFunc.crossPro(fldirec, yax, lxax);
            voirFunc.Normalize(lxax);
            voirFunc.crossPro(lxax, fldirec, lyax);
            voirFunc.Normalize(lyax);
        }else
        {
            voirFunc.crossPro(yax, fldirec, lxax);
            voirFunc.Normalize(lxax);
            voirFunc.crossPro(fldirec, lxax, lyax);
            voirFunc.Normalize(lyax);
        }

        float rcosp, rsinp;
        rcosp = r * Mathf.Cos(phi); rsinp = r * Mathf.Sin(phi);

        spos[0] = (double)lxax[0] * rcosp + lyax[0] * rsinp + z * fldirec[0] + flbase[0];
        spos[1] = (double)lxax[1] * rcosp + lyax[1] * rsinp + z * fldirec[1] + flbase[1];
        spos[2] = (double)lxax[2] * rcosp + lyax[2] * rsinp + z * fldirec[2] + flbase[2];
        //spos[0] = (double)lxax[0] * r * Mathf.Cos(phi) + lyax[0] * r * Mathf.Sin(phi) + z * fldirec[0] + flbase[0];
        //spos[1] = (double)lxax[1] * r * Mathf.Cos(phi) + lyax[1] * r * Mathf.Sin(phi) + z * fldirec[1] + flbase[1];
        //spos[2] = (double)lxax[2] * r * Mathf.Cos(phi) + lyax[2] * r * Mathf.Sin(phi) + z * fldirec[2] + flbase[2];
    }

    void Alloc()
    {
        snow = new Mesh();
        //GetComponent<MeshFilter>().mesh = snow;
        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = snow;
        snow_pos = new Vector3[voirConst.FL_NUM_SNOW];
        //snow_index = new int[voirConst.FL_NUM_SNOW];
        snow_color = new Color[voirConst.FL_NUM_SNOW];

//        if(param.single) valf = new float[3];
//        else val = new double[3];

        pos = new double[3];
        fldirec = new float[3];
        flbase = new float[3];
        xax = new float[3]; lxax = new float[3];
        yax = new float[3]; lyax = new float[3];
        rpos = new float[3];
        hfd = Vector3.zero;
        hup = Vector3.zero;
        for (int i=0;i<3;i++)
        {
//            val[i] = 0.0;
            pos[i] = 0.0;
            fldirec[i] = 0.0f;
            flbase[i] = 0.0f;
            rpos[i] = 0.0f;
            lxax[i] = 0.0f;
            lyax[i] = 0.0f;
        }
        fldirec[1] = 1.0f;
        flbase[1] = -2.0f;
        xax[0] = 1.0f; xax[1] = 0.0f; xax[2] = 0.0f;
        yax[0] = 0.0f; yax[1] = 1.0f; yax[2] = 0.0f;

        for (int i = 0; i < voirConst.FL_NUM_SNOW; i++)
        {
            //snow_index[i] = i;
            snow_color[i] = new Color(0.75f, 0.75f, 1.0f, 1.0f);
        }
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());
    }

    void Init()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        data = GameObject.Find("Data").GetComponent<voirData>();

        costheta = (float)Math.Cos(voirConst.FL_THETA);
        tantheta = (float)Math.Tan(voirConst.FL_THETA);

        if (param.single) valf = new float[3];
        else val = new double[3];

        for (int i = 0; i < voirConst.FL_NUM_SNOW; i++)
        {
            place_snow(pos);
            snow_pos[i] = new Vector3((float)pos[0], (float)pos[2], (float)pos[1]);
        }

        nvec = 0;

    }

    public void ClearLight()
    {
        for (int i = 0; i < voirConst.FL_NUM_SNOW; i++)
        {
            place_snow(pos);
            snow_pos[i][0] = (float)pos[0];
            snow_pos[i][1] = (float)pos[2];
            snow_pos[i][2] = (float)pos[1];
        }
    }

    public void SetVectNum(int n)
    {
        nvec = n;
    }

}
