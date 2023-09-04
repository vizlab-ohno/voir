using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class LineVerts
{
    int np; // num of points
    int max;
    bool calc;
    bool norm;
    Vector3[] points;
    
    public void Alloc(int num)
    {
        points = new Vector3[num];
        for(int i=0;i<num;i++) points[i] = new Vector3(0f,0f,0f);
        max = num;
    }

    public void init()
    {
        np = 0;
        calc = false;
        norm = false;
    }

    public void clear()
    {
        np = 0;
        calc = false;
    }

    public void start(double [] seed, bool n)
    {
        calc = true;
        np = 0;
        norm = n;
        points[0][0] = (float)seed[0];
        points[0][1] = (float)seed[2];
        points[0][2] = (float)seed[1];
        np=1;
    }

    public bool is_calc()
    {
        return calc;
    }

    public bool is_norm()
    {
        return norm;
    }

    public int num_points()
    {
        return np;
    }

    public bool setPos(double [] p, int it)
    {
        if(!calc || np == max/it) return false;
        points[np].x = (float)p[0];
        points[np].y = (float)p[2];
        points[np].z = (float)p[1];
        np++;
        if(np == max/it) return false;
        return true;
    }

    public void getSeed(double[] p)
    {
        p[0] = points[0].x;
        p[1] = points[0].z;
        p[2] = points[0].y;
    }

    public Vector3[] verts()
    {
        return points;
    }
    
    public void stop_calc()
    {
        calc = false;
    }
};

public class voirStreamLines : MonoBehaviour
{
    LineVerts [,] points;
    double [] xt; double [,,] x;  double [] sltip;
    double [] k1; double [] k2; double [] k3; double [] k4;
    double [] vec_val;
    float[] vec_valf;
    int [] ncalc;
    bool [] cycle;
    bool integrate;
    int iter;
    LineRenderer [,] drawline;
    GameObject[,] Lines;
    public GameObject ballObj;
    GameObject[,] Balls;
    voirParam param;
    voirCoord coord;
    voirData data;
 
    LineRenderer lineRenderer;
    //Vector3[] line;

    LineRenderer beam;
    Vector3[] beampos;
    Vector3[] contrdata;

    tracking trackd;
    MainControll maincontroll;
    int nvect;
    double[] spos;

    void Awake()
    {
        Init();
    }
    // Start is called before the first frame update

    void Start()
    {
        AllocMem();
        AllocObjects();
        CalcSkip();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        calc();
        //calccross();
        Beam();
    }

    void Init()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        data = GameObject.Find("Data").GetComponent<voirData>();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();
        nvect = 0;

        beam = gameObject.AddComponent<LineRenderer>();

        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());
    }

    public int GetNcalc(int n)
    {
        return ncalc[n];
    }

    public bool GetCycle(int n)
    {
        return cycle[n];
    }

    public void GetSeeds(int n, int ln, double [] pos)
    {
        points[n, ln].getSeed(pos);
    }

    public void SetVect(int nv)
    {
        nvect = nv;
    }

    void Beam()
    {
        if (maincontroll.SelectedMethod != VizMethods.StreamLines) return;

        if (trackd.getRTrigChange() == -1)
        {
            trackd.getRContData(0, ref contrdata[0]);
            trackd.getRContData(1, ref contrdata[1]);

            spos[0] = contrdata[0].x + voirConst.BEAM_LEN * contrdata[1].x;
            spos[2] = contrdata[0].y + voirConst.BEAM_LEN * contrdata[1].y;
            spos[1] = contrdata[0].z + voirConst.BEAM_LEN * contrdata[1].z;

            StartLine(nvect, spos, true);
        }

        if(trackd.getRGripChange()==1)
        {
            ClearLines(nvect);
        }

        if (trackd.getLGripChange() == 1)
        {
            ClearALine(nvect);
        }

        if (trackd.RTrigState())
        {
            trackd.getRContData(0, ref contrdata[0]);
            trackd.getRContData(1, ref contrdata[1]);
            
            beampos[0].x = contrdata[0].x;
            beampos[0].y = contrdata[0].y;
            beampos[0].z = contrdata[0].z;

            beampos[1].x = contrdata[0].x + voirConst.BEAM_LEN * contrdata[1].x;
            beampos[1].y = contrdata[0].y + voirConst.BEAM_LEN * contrdata[1].y;
            beampos[1].z = contrdata[0].z + voirConst.BEAM_LEN * contrdata[1].z;

            beam.positionCount = 2;
            beam.SetPositions(beampos);
        }
        else
        {
            beam.positionCount = 0;
        }

    }

    void calccross()
    {
        /*
        if (StreamLinesUI.activeSelf)
        {
            line[0].x = (float)coord.minc[0];
            line[0].y = (float)slpanel.seed[2];
            line[0].z = (float)slpanel.seed[1];

            line[1].x = (float)coord.maxc[0];
            line[1].y = (float)slpanel.seed[2];
            line[1].z = (float)slpanel.seed[1];

            line[2].x = (float)slpanel.seed[0];
            line[2].y = (float)slpanel.seed[2];
            line[2].z = (float)slpanel.seed[1];

            line[3].x = (float)slpanel.seed[0];
            line[3].y = (float)coord.minc[2];
            line[3].z = (float)slpanel.seed[1];

            line[4].x = (float)slpanel.seed[0];
            line[4].y = (float)coord.maxc[2];
            line[4].z = (float)slpanel.seed[1];

            line[5].x = (float)slpanel.seed[0];
            line[5].y = (float)slpanel.seed[2];
            line[5].z = (float)slpanel.seed[1];

            line[6].x = (float)slpanel.seed[0];
            line[6].y = (float)slpanel.seed[2];
            line[6].z = (float)coord.minc[1];

            line[7].x = (float)slpanel.seed[0];
            line[7].y = (float)slpanel.seed[2];
            line[7].z = (float)coord.maxc[1];
            
            lineRenderer.positionCount = 8;
            lineRenderer.SetPositions(line);
            //voirFunc.Log("seed: "+ slpanel.seed[0]+" "+ slpanel.seed[1]+" "+ slpanel.seed[2]);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
        */
    }

    public void CalcSkip()
    {
        iter = (int)(coord.relatelen * 2 + 0.5);
        if (iter < 0) voirFunc.Error("stline; iter < 0");
        //iter = (int)Mathf.Sqrt((float)iter);
        if (iter == 0) iter = 1;
        //iter = 1;
    }

    void AllocObjects()
    {
        drawline = new LineRenderer[data.nvect, voirConst.SL_NUM_LINES];
        Lines = new GameObject[data.nvect, voirConst.SL_NUM_LINES];
        for (int i = 0; i < data.nvect; i++)
        {
            for(int j=0;j<voirConst.SL_NUM_LINES;j++)
            {
                Lines[i,j] = new GameObject("StreamLine v=" + i + "ln="+j);
                Lines[i,j].transform.SetParent(transform);
                Lines[i,j].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

                //drawline[i,j] = new LineRenderer();
                drawline[i,j] = Lines[i,j].AddComponent<LineRenderer>();
                drawline[i,j].useWorldSpace = false;
                drawline[i,j].material = new Material(Shader.Find("Sprites/Default"));

                drawline[i,j].startWidth = 0.005f;
                drawline[i,j].endWidth = 0.005f;

                drawline[i,j].positionCount = 0;
                drawline[i,j].loop = false;

                switch(i%5)
                {
                    case 0:
                        drawline[i,j].startColor = Color.cyan;
                        drawline[i,j].endColor = Color.cyan;
                        break;
                        
                    case 1:
                        drawline[i,j].startColor = Color.magenta;
                        drawline[i,j].endColor = Color.magenta;
                        break;

                    case 2:
                        drawline[i,j].startColor = Color.green;
                        drawline[i,j].endColor = Color.green;
                        break;

                    case 3:
                        drawline[i,j].startColor = Color.yellow;
                        drawline[i,j].endColor = Color.yellow;
                        break;

                    case 4:
                        drawline[i,j].startColor = Color.gray;
                        drawline[i,j].endColor = Color.gray;
                        break;

                        default:
                            voirFunc.Error("StreamLine color is strange");
                            break;
                }
                
                //drawline[i,j].SetPositions(points[i,j].verts());
            }
        }
        
        Balls = new GameObject[data.nvect, voirConst.SL_NUM_LINES];
        for (int i = 0; i < data.nvect; i++)
        {
            for(int j=0;j<voirConst.SL_NUM_LINES;j++)
            {
                Balls[i,j]
                    = Instantiate(ballObj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
                Balls[i,j].transform.SetParent(transform);
                Balls[i,j].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                Balls[i,j].GetComponent<Transform>().localScale = new Vector3(voirConst.SL_BALL_SIZE, voirConst.SL_BALL_SIZE, voirConst.SL_BALL_SIZE);
                
                switch(i%5)
                {
                    case 0:
                        Balls[i,j].GetComponent<Renderer>().material.color = Color.cyan;
                        break;
                        
                    case 1:
                        Balls[i,j].GetComponent<Renderer>().material.color = Color.magenta;
                        break;

                    case 2:
                        Balls[i,j].GetComponent<Renderer>().material.color = Color.green;
                        break;

                    case 3:
                        Balls[i,j].GetComponent<Renderer>().material.color = Color.yellow;
                        break;

                    case 4:
                        Balls[i,j].GetComponent<Renderer>().material.color = Color.gray;
                        break;

                        default:
                            voirFunc.Error("StreamLine ball color is strange");
                            break;
                }
                Balls[i,j].SetActive (false);

            }
        }

    }

    void AllocMem()
    {
        xt = new double [3];
        sltip = new double [3];
        x = new double [data.nvect, voirConst.SL_NUM_LINES, 3];
        ncalc = new int [data.nvect];
        cycle = new bool[data.nvect];
        k1 = new double [3]; k2 = new double [3]; k3 = new double [3]; k4 = new double [3];
        spos = new double[3];

        if (param.single) vec_valf = new float[3];
        else vec_val = new double [3];

        points = new LineVerts[data.nvect, voirConst.SL_NUM_LINES];
        for(int j=0;j<voirConst.SL_NUM_LINES;j++)
        {
            for(int i=0;i<data.nvect;i++)
            {
                points[i,j] = new LineVerts();
                points[i,j].Alloc(voirConst.SL_NUM_VERTS);
                points[i,j].init();
                
            }
        }
        integrate = false;
        for (int i = 0; i < data.nvect; i++)
        {
            ncalc[i] = 0;
            cycle[i] = false;
        }

        beampos = new Vector3[2]{new Vector3(0f,0f,0f), new Vector3(0f,0f,0f)};
        contrdata = new Vector3[2]{new Vector3(0f,0f,0f), new Vector3(0f,0f,0f)};

        beam.startWidth = 0.005f;
        beam.endWidth = 0.005f;
        beam.startColor = Color.cyan;
        beam.endColor = Color.cyan;
        beam.positionCount = 0;
        beam.material = new Material(Shader.Find("Sprites/Default"));

    }

    public void StartLine(int nv, double [] pos, bool norm)
    {
        bool inside;
        inside = coord.insideData(pos);

        if(!inside) return;
        integrate = true;
        points[nv, ncalc[nv]].clear();
        points[nv, ncalc[nv]].start(pos, norm);
        x[nv, ncalc[nv], 0] = pos[0];
        x[nv, ncalc[nv], 1] = pos[1];
        x[nv, ncalc[nv], 2] = pos[2];
        Balls[nv,ncalc[nv]].GetComponent<Transform>().localPosition
             = new Vector3((float)sltip[0], (float)sltip[2], (float)sltip[1]);
        Balls[nv,ncalc[nv]].SetActive (true);
        ncalc[nv]++;
        if(ncalc[nv] == voirConst.SL_NUM_LINES)
        {
            ncalc[nv] = 0;
            cycle[nv] = true;
        }
        //        ncalc[nv] = ncalc[nv]%voirConst.SL_NUM_LINES;
        voirFunc.Log("stream line iter = " + iter);
    }

    public void ClearALine(int nv)
    {
        int cn = ncalc[nv]-1;
        if(cn < 0)
        {
            if (cycle[nv])
            {
                cn = voirConst.SL_NUM_LINES - 1;
                cycle[nv] = false;
            }
            else {
                //nothing to do
                return;
            }
        }
        points[nv, cn].clear();
        drawline[nv, cn].positionCount = 0;
        Balls[nv, cn].SetActive(false);
        ncalc[nv] = cn;
    }

    public void ClearLines(int nv)
    {
        ncalc[nv] = 0;
        for(int i=0;i<voirConst.SL_NUM_LINES;i++){
            points[nv, i].clear();
            Balls[nv,i].SetActive (false);
            drawline[nv, i].positionCount = 0;
        }
    }

    void calc()
    {
        bool calc, cont;
        if (!integrate) return;
        calc = true;
        for (int acc = 0; acc < voirConst.SL_CALC_ACC; acc++) {
            for (int j = 0; j < voirConst.SL_NUM_LINES; j++)
            {
                for (int i = 0; i < data.nvect; i++)
                {
                    if (!points[i, j].is_calc()) continue;
                    sltip[0] = x[i, j, 0]; sltip[1] = x[i, j, 1]; sltip[2] = x[i, j, 2];

                    for (int k = 0; k < iter; k++)
                    {
                        if (param.single) calc = RungeKutta4f(i, sltip, points[i, j].is_norm());
                        else calc = RungeKutta4d(i, sltip, points[i, j].is_norm());
                    }

                    if (calc)
                    {
                        cont = points[i, j].setPos(sltip, iter);
                        drawline[i, j].positionCount = points[i, j].num_points();
                        drawline[i, j].SetPositions(points[i, j].verts());
                        Balls[i, j].GetComponent<Transform>().localPosition
                            = new Vector3((float)sltip[0], (float)sltip[2], (float)sltip[1]);
                        x[i, j, 0] = sltip[0]; x[i, j, 1] = sltip[1]; x[i, j, 2] = sltip[2];
                        if (!cont) points[i, j].stop_calc();
                        //int n=points[i,j].num_points();
                        //voirFunc.Log("np="+n+" x:"+points[i,j].verts()[n-1][0]+" y:"+points[i,j].verts()[n-1][1]+" z:"+points[i,j].verts()[n-1][2]);
                    } else
                    {
                        points[i, j].stop_calc();
                    }
                }
            }
        }
        return;
    }

    bool RungeKutta4d(int nvec, double [] pos, bool normalize)
    {
        //calculating k1
        bool inside;
        double h = coord.h * voirConst.DH;
        double maxvalue = data.voirVectData[nvec].maxvalue;

        xt[0] = pos[0]; xt[1] = pos[1]; xt[2] = pos[2]; 
        //voirFunc.Log("Runge:b4 insidel");
        // calculating k1
        inside = data.getVectVal(nvec, xt, vec_val);
        if(!inside) return false;
        if(normalize) voirFunc.Normalize(vec_val);
        else {
            vec_val[0] /= maxvalue;
            vec_val[1] /= maxvalue;
            vec_val[2] /= maxvalue;
        }
        k1[0] = vec_val[0]*h;
        k1[1] = vec_val[1]*h;
        k1[2] = vec_val[2]*h;

        //calculating k2
        xt[0] = pos[0] + k1[0]/2.0;
        xt[1] = pos[1] + k1[1]/2.0;
        xt[2] = pos[2] + k1[2]/2.0;

        inside = data.getVectVal(nvec, xt, vec_val);
        //voirFunc.Log("x:"+vec_val[0]+" y:"+vec_val[1]+" z:"+vec_val[2]);
        if(!inside) return false;
        if(normalize) voirFunc.Normalize(vec_val);
        else {
            vec_val[0] /= maxvalue;
            vec_val[1] /= maxvalue;
            vec_val[2] /= maxvalue;
        }
        k2[0] = vec_val[0]*h;
        k2[1] = vec_val[1]*h;
        k2[2] = vec_val[2]*h;

        //calculating k3
        xt[0] = pos[0] + k2[0]/2.0;
        xt[1] = pos[1] + k2[1]/2.0;
        xt[2] = pos[2] + k2[2]/2.0;

        inside = data.getVectVal(nvec, xt, vec_val);
        if(!inside) return false;
        if(normalize) voirFunc.Normalize(vec_val);
        else {
            vec_val[0] /= maxvalue;
            vec_val[1] /= maxvalue;
            vec_val[2] /= maxvalue;
        }
        k3[0] = vec_val[0]*h;
        k3[1] = vec_val[1]*h;
        k3[2] = vec_val[2]*h;
   
        // calculating k4
        xt[0] = pos[0] + k3[0];
        xt[1] = pos[1] + k3[1];
        xt[2] = pos[2] + k3[2];
        inside = data.getVectVal(nvec, xt, vec_val);
        if(!inside) return false;
        if(normalize) voirFunc.Normalize(vec_val);
        else {
            vec_val[0] /= maxvalue;
            vec_val[1] /= maxvalue;
            vec_val[2] /= maxvalue;
        }
        k4[0] = vec_val[0]*h;
        k4[1] = vec_val[1]*h;
        k4[2] = vec_val[2]*h;

        pos[0] = pos[0] + (k1[0] + 2.0*k2[0] + 2.0*k3[0]+k4[0])/6.0;
        pos[1] = pos[1] + (k1[1] + 2.0*k2[1] + 2.0*k3[1]+k4[1])/6.0;
        pos[2] = pos[2] + (k1[2] + 2.0*k2[2] + 2.0*k3[2]+k4[2])/6.0;
    
        inside = coord.insideData(pos);
        if(!inside) return false;
        return true;
    }

    bool RungeKutta4f(int nvec, double[] pos, bool normalize)
    {
        //calculating k1
        bool inside;
        double h = coord.h * voirConst.DH;
        float maxvalue = (float)data.voirVectData[nvec].maxvalue;

        xt[0] = pos[0]; xt[1] = pos[1]; xt[2] = pos[2];
        //voirFunc.Log("Runge:b4 insidel");
        // calculating k1
        inside = data.getVectValf(nvec, xt, vec_valf);
        if (!inside) return false;
        if (normalize) voirFunc.Normalize(vec_valf);
        else
        {
            vec_valf[0] /= maxvalue;
            vec_valf[1] /= maxvalue;
            vec_valf[2] /= maxvalue;
        }
        k1[0] = vec_valf[0] * h;
        k1[1] = vec_valf[1] * h;
        k1[2] = vec_valf[2] * h;

        //calculating k2
        xt[0] = pos[0] + k1[0] / 2.0;
        xt[1] = pos[1] + k1[1] / 2.0;
        xt[2] = pos[2] + k1[2] / 2.0;

        inside = data.getVectValf(nvec, xt, vec_valf);
        //voirFunc.Log("x:"+vec_val[0]+" y:"+vec_val[1]+" z:"+vec_val[2]);
        if (!inside) return false;
        if (normalize) voirFunc.Normalize(vec_valf);
        else
        {
            vec_valf[0] /= maxvalue;
            vec_valf[1] /= maxvalue;
            vec_valf[2] /= maxvalue;
        }
        k2[0] = vec_valf[0] * h;
        k2[1] = vec_valf[1] * h;
        k2[2] = vec_valf[2] * h;

        //calculating k3
        xt[0] = pos[0] + k2[0] / 2.0;
        xt[1] = pos[1] + k2[1] / 2.0;
        xt[2] = pos[2] + k2[2] / 2.0;

        inside = data.getVectValf(nvec, xt, vec_valf);
        if (!inside) return false;
        if (normalize) voirFunc.Normalize(vec_valf);
        else
        {
            vec_valf[0] /= maxvalue;
            vec_valf[1] /= maxvalue;
            vec_valf[2] /= maxvalue;
        }
        k3[0] = vec_valf[0] * h;
        k3[1] = vec_valf[1] * h;
        k3[2] = vec_valf[2] * h;

        // calculating k4
        xt[0] = pos[0] + k3[0];
        xt[1] = pos[1] + k3[1];
        xt[2] = pos[2] + k3[2];
        inside = data.getVectValf(nvec, xt, vec_valf);
        if (!inside) return false;
        if (normalize) voirFunc.Normalize(vec_valf);
        else
        {
            vec_valf[0] /= maxvalue;
            vec_valf[1] /= maxvalue;
            vec_valf[2] /= maxvalue;
        }
        k4[0] = vec_valf[0] * h;
        k4[1] = vec_valf[1] * h;
        k4[2] = vec_valf[2] * h;

        pos[0] = pos[0] + (k1[0] + 2.0 * k2[0] + 2.0 * k3[0] + k4[0]) / 6.0;
        pos[1] = pos[1] + (k1[1] + 2.0 * k2[1] + 2.0 * k3[1] + k4[1]) / 6.0;
        pos[2] = pos[2] + (k1[2] + 2.0 * k2[2] + 2.0 * k3[2] + k4[2]) / 6.0;

        inside = coord.insideData(pos);
        if (!inside) return false;
        return true;
    }
}
