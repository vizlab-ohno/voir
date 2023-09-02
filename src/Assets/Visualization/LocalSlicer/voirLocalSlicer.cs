//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirLocalSlicer : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh lslicer;
    Vector3[] verts;
    Color [] vcolor;
    double [] pos;
    float [] lsdirec; float [] lsbase; float [] tipf;
    float [] right; float [] down; float [] lefttop;
    int [] tindex;
    float [] xax; float [] zax;
    float dx;
    int sn;
    voirParam param;
    voirCoord coord;
    voirData data;
    voirColor col;

    LineRenderer lineRenderer;
    Vector3[] line;

    Vector3[] contrdata;

    tracking trackd;
    MainControll maincontroll;

    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        Alloc();
        Init();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (maincontroll.SelectedMethod != VizMethods.LocalSlicer) return;
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
        lsbase[0] = basp.x;
        lsbase[1] = basp.z;
        lsbase[2] = basp.y;

        lsdirec[0] = dir.x;
        lsdirec[1] = dir.z;
        lsdirec[2] = dir.y;
        voirFunc.Normalize(lsdirec);
    }

    public void GetPos(double [] p)
    {
        for(int i=0;i<3;i++)p[i] = (double)tipf[i];
    }

    public void LocalSlicerROI(double [] p)
    {
        lsbase[0] = (float)p[0] - lsdirec[0] * voirConst.BEAM_LEN;
        lsbase[1] = (float)p[1] - lsdirec[1] * voirConst.BEAM_LEN;
        lsbase[2] = (float)p[2] - lsdirec[2] * voirConst.BEAM_LEN;

        calc();
    }

    void Alloc()
    {
        verts = new Vector3[voirConst.LS_DIV*voirConst.LS_DIV];
        vcolor = new Color[voirConst.LS_DIV*voirConst.LS_DIV];
        for(int i=0;i<voirConst.LS_DIV*voirConst.LS_DIV;i++){
            verts[i] = new Vector3(0f,0f,0f);
            vcolor[i] = new Color(0f,0f,0f,1f);
        }
        lsdirec = new float [3]; lsbase = new float [3]; tipf = new float[3];
        right = new float [3]; down = new float [3]; lefttop = new float [3];
        tindex = new int [(voirConst.LS_DIV-1)*(voirConst.LS_DIV-1)*2*3];
        zax = new float [3]; xax = new float [3];
        pos = new double [3];

        contrdata = new Vector3[2]{new Vector3(0f,0f,0f),new Vector3(0f,0f,0f)};

        lslicer = new Mesh();
        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = lslicer;

        line = new Vector3[2]{new Vector3(0f,0f,0f),new Vector3(0f,0f,0f)};
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Init()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        data = GameObject.Find("Data").GetComponent<voirData>();
        col = GameObject.Find("Color").GetComponent<voirColor>();

        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();

        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());

        xax[0] = 1.0f; xax[1] = 0.0f; xax[2] = 0.0f;
        zax[0] = 0.0f; zax[1] = 0.0f; zax[2] = 1.0f;
        dx = voirConst.LS_WIDTH/(voirConst.LS_DIV-1);
        sn = 0;
        int n = 0;
        for(int k=0;k<(voirConst.LS_DIV-1);k++)
        {
            for(int j=0;j<(voirConst.LS_DIV-1);j++)
            {
                tindex[n] = j + k*voirConst.LS_DIV;
                tindex[n+1] = j+1 + k*voirConst.LS_DIV;
                tindex[n+2] = j+voirConst.LS_DIV + k*voirConst.LS_DIV;
                n+=3;
                tindex[n] = j+voirConst.LS_DIV + k*voirConst.LS_DIV;
                tindex[n+1] = j+1 + k*voirConst.LS_DIV;
                tindex[n+2] = j+1+voirConst.LS_DIV + k*voirConst.LS_DIV;
                n+=3;
            }
        }
        
        lslicer.SetVertices (verts);
        lslicer.SetColors(vcolor);
        lslicer.SetTriangles (tindex, 0);
    }
    
    void calc()
    {
        voirFunc.crossPro(lsdirec, xax, down);
        if(voirFunc.Normalize(down) < 1.0E-10){
            voirFunc.crossPro(zax, lsdirec, right);
            voirFunc.Normalize(right);
            voirFunc.crossPro(lsdirec, right, down);
            voirFunc.Normalize(down);
        } else {
            voirFunc.crossPro(down, lsdirec, right);
            voirFunc.Normalize(right);
        }
        tipf[0] = lsbase[0] + lsdirec[0] * voirConst.BEAM_LEN;
        tipf[1] = lsbase[1] + lsdirec[1] * voirConst.BEAM_LEN;
        tipf[2] = lsbase[2] + lsdirec[2] * voirConst.BEAM_LEN;
        lefttop[0] = tipf[0] - dx*(down[0] + right[0]) * voirConst.LS_DIV / 2.0f;
        lefttop[1] = tipf[1] - dx*(down[1] + right[1]) * voirConst.LS_DIV / 2.0f;
        lefttop[2] = tipf[2] - dx*(down[2] + right[2]) * voirConst.LS_DIV / 2.0f;
        //Debug.Log("y:"+lefttop[1]);
        int vn;
        bool inside;
        double sval=0.0;
        float svalf = 0.0f;
        if (param.single)
        {
            for (int j = 0; j < voirConst.LS_DIV; j++)
            {
                for (int i = 0; i < voirConst.LS_DIV; i++)
                {
                    vn = i + voirConst.LS_DIV * j;
                    verts[vn][0] = lefttop[0] + i * dx * right[0] + j * dx * down[0];
                    verts[vn][2] = lefttop[1] + i * dx * right[1] + j * dx * down[1];
                    verts[vn][1] = lefttop[2] + i * dx * right[2] + j * dx * down[2];

                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];
                    inside = data.getScalValf(sn, pos, out svalf);
                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        //Debug.Log("sval:"+sval+" r:"+vcolor[vn][0]+" g:"+vcolor[vn][1]+" b:"+vcolor[vn][2]+"a:"+vcolor[vn][3]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        vcolor[vn][0] = 0.0f;
                        vcolor[vn][1] = 0.0f;
                        vcolor[vn][2] = 0.0f;
                        vcolor[vn][3] = 1.0f;
                    }

                }
            }

        }
        else {
            for (int j = 0; j < voirConst.LS_DIV; j++)
            {
                for (int i = 0; i < voirConst.LS_DIV; i++)
                {
                    vn = i + voirConst.LS_DIV * j;
                    verts[vn][0] = lefttop[0] + i * dx * right[0] + j * dx * down[0];
                    verts[vn][2] = lefttop[1] + i * dx * right[1] + j * dx * down[1];
                    verts[vn][1] = lefttop[2] + i * dx * right[2] + j * dx * down[2];

                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];
                    inside = data.getScalVal(sn, pos, out sval);
                    if (inside) {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        //Debug.Log("sval:"+sval+" r:"+vcolor[vn][0]+" g:"+vcolor[vn][1]+" b:"+vcolor[vn][2]+"a:"+vcolor[vn][3]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else {
                        vcolor[vn][0] = 0.0f;
                        vcolor[vn][1] = 0.0f;
                        vcolor[vn][2] = 0.0f;
                        vcolor[vn][3] = 1.0f;
                    }

                }
            }
        }
        lslicer.SetVertices (verts);
        lslicer.SetColors(vcolor);
        lslicer.SetTriangles (tindex, 0);
        lslicer.RecalculateBounds();

        line[0].x = lsbase[0];
        line[0].y = lsbase[2];
        line[0].z = lsbase[1];

        line[1].x = tipf[0];
        line[1].y = tipf[2];
        line[1].z = tipf[1];
        lineRenderer.SetPositions(line);

    }

    public void SetScalNum(int n)
    {
        sn = n;
    }

}
