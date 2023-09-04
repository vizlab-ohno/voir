using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;
using System;

public class voirSnow : MonoBehaviour
{
    [SerializeField] GameObject ptclObj;
    GameObject[] ptcls;// = new GameObject[5];

    MeshFilter MeshFilter;
    Mesh snow;
    Vector3[] snow_pos; // x,z,y
    Vector3 hfd;
    Vector3 hup;
    //int[] snow_index;
    Color[] snow_color;
    voirParam param;
    voirCoord coord;
    voirData data;
    int nvec;
    double[] val; // vector value of a position
    float[] valf; // vector value of a position
    double[] pos; // a position

    tracking trackd;
    MainControll maincontroll;

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

        if (maincontroll.SelectedMethod != VizMethods.Snow) return;
        if (trackd.getRGripChange() == 1)
        {
            maincontroll.SetNone();
            gameObject.SetActive(false);

        }
    }

    private void InitPtcls()
    {
        ptcls = new GameObject[voirConst.SW_NUM_SNOW];
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());

        for (int i = 0; i < voirConst.SW_NUM_SNOW; i++)
        {
            ptcls[i]
                = Instantiate(ptclObj, new Vector3(0.0f, 0.0f, 0.0f),
                Quaternion.identity) as GameObject;
            ptcls[i].transform.SetParent(transform);
            ptcls[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            ptcls[i].GetComponent<Transform>().localScale = new Vector3(0.005f, 0.005f, 0.005f);
        }
    }

    void calcp()
    {
        double h = coord.h * voirConst.DH;
        double maxvalue;
        if (nvec < 0)
        {
            voirFunc.Error("snow calc: nvec < 0");
        }
        int iter = (int)coord.relatelen;
        if (iter == 0) iter = 1;
        maxvalue = data.voirVectData[nvec].maxvalue;
        double dt = h / maxvalue;
        trackd.getHeadData(voirConst.TD_FORWARD, ref hfd);
        trackd.getHeadData(voirConst.TD_UP, ref hup);
        Quaternion prot = Quaternion.LookRotation(hfd, hup);
        if (param.single)
        {
            for (int i = 0; i < voirConst.SW_NUM_SNOW; i++)
            {
                for (int k = 0; k < iter* voirConst.SL_CALC_ACC; k++)
                {
                    pos[0] = (double)snow_pos[i][0];
                    pos[1] = (double)snow_pos[i][2];
                    pos[2] = (double)snow_pos[i][1];
                    if (!data.getVectValf(nvec, pos, valf))
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
            for (int i = 0; i < voirConst.SW_NUM_SNOW; i++)
            {
                for (int k = 0; k < iter* voirConst.SL_CALC_ACC; k++)
                {
                    pos[0] = (double)snow_pos[i][0];
                    pos[1] = (double)snow_pos[i][2];
                    pos[2] = (double)snow_pos[i][1];
                    //bool outofv = false;
                    if (!data.getVectVal(nvec, pos, val))
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
        if(voirConst.SN_BLINK){
            float t = Time.time;
            double tpi = 2.0*voirConst.PI;
            double wt = tpi*0.5*t;
            double ph = tpi/(voirConst.SW_NUM_SNOW-1);
            for (int i = 0; i < voirConst.SW_NUM_SNOW; i++){
                ptcls[i].GetComponent<Renderer>().material.color 
               = new Color(1,1,1,(float)((Math.Sin((ph*i+wt))+1.0)/2.0));
            }
        }

    }

    void place_snow(double[]spos)
    {
        spos[0] = UnityEngine.Random.Range((float)coord.coordx[coord.minic[0]], (float)coord.coordx[coord.maxic[0]]);
        spos[1] = UnityEngine.Random.Range((float)coord.coordx[coord.minic[1]], (float)coord.coordx[coord.maxic[1]]);
        spos[2] = UnityEngine.Random.Range((float)coord.coordx[coord.minic[2]], (float)coord.coordx[coord.maxic[2]]);
    }

    void Alloc()
    {
        snow = new Mesh();
        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = snow;
        snow_pos = new Vector3[voirConst.SW_NUM_SNOW];
        //snow_index = new int[voirConst.SW_NUM_SNOW];
        snow_color = new Color[voirConst.SW_NUM_SNOW];
//        if (param.single) valf = new float[3];
//        else val = new double[3];
        pos = new double[3];

        for (int i = 0; i < voirConst.SW_NUM_SNOW; i++)
        {
           // snow_index[i] = i;
            snow_color[i] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        hfd = Vector3.zero;
        hup = Vector3.zero;
    }

    void Init()
    {
        //voirFunc.Log("Snow Init");
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        data = GameObject.Find("Data").GetComponent<voirData>();

        if (param.single) valf = new float[3];
        else val = new double[3];

        for (int i = 0; i < voirConst.SW_NUM_SNOW; i++)
        {
            place_snow(pos);
            snow_pos[i] = new Vector3((float)pos[0],(float)pos[2], (float)pos[1]);
        }

        nvec = 0;
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());
    }

    public void ClearSnow()
    {
        //voirFunc.Log("ClearSnow");
        for (int i = 0; i < voirConst.SW_NUM_SNOW; i++)
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
