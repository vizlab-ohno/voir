//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirOrthoSlicerXY : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh slicerxy;
    Vector3[] verts;
    Vector3[] norm;
    Color [] vcolor;
    int [] tindex;
    double [] pos;
    double dx, dy;
    int numy, numx;
    int K; // Slicer's Z
    int sn;
    voirParam param;
    voirCoord coord;
    voirData data;
    voirColor col;
    float carpet;
    float[,] height;
    float[] nx;
    float[] ny;
    bool rawdatamode;

    tracking trackd;
    MainControll maincontroll;
    GameObject SlicerUI;
    Vector3 center;
    float scalex, scaley;
    float guidz;
    float recz, curz;
    int reciz,curiz;
    OrthoSlicerUI slicerui;
    bool pressed;
    Vector3 rcpos;
    bool wireframe, carpetplot;

    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        Alloc();
        Init();
        InitGUI();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (maincontroll.SelectedMethod != VizMethods.OrthoSlicerXY1) return;
        if (trackd.getRGripChange() == 1)
        {
            maincontroll.SetNone();
            gameObject.SetActive(false);
        }

        switch(trackd.getRTrigChange())
        {
            case 1:
                SlicerUI.SetActive(true);
                trackd.getRContData(voirConst.TD_POS, ref rcpos);
                reciz = K;
                recz = rcpos.y;
                calcScale();
                pressed = true;
                break;

            case 0:
                if (pressed) {
                    SlicerUI.SetActive(true);
                    trackd.getRContData(voirConst.TD_POS, ref rcpos);
                    curz = rcpos.y;
                    int ciz = (int)((curz - recz) / guidz) + reciz;
                    if (ciz > coord.maxic[2]) ciz = coord.maxic[2];
                    else if (ciz < coord.minic[2]) ciz = coord.minic[2];
                    curiz = ciz;
                        //voirFunc.Log("z:"+((curz - recz) / guidz));
                        //voirFunc.Log("cz:" + curz + " rz" + recz+" curiz:"+curiz);
                    center.y = (float)coord.coordz[curiz];
                    slicerui.GUIXY(center, scalex, scaley);
                }
                break;

            case -1:
                if (curiz > coord.maxic[2]) curiz = coord.maxic[2];
                else if (curiz < coord.minic[2]) curiz = coord.minic[2];
                voirFunc.Log("curiz " + curiz);
                ReGenerateSlice(curiz);
                pressed = false;
                SlicerUI.SetActive(false);
                break;
        }

    }

    void calcScale()
    {
        scalex = (float)(coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]);
        scaley = (float)(coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]);
    }

    void InitGUI()
    {
        SlicerUI = GameObject.Find("OrthoSlicerUI");
        slicerui = GameObject.Find("OrthoSlicerUI").GetComponent<OrthoSlicerUI>();

        center = Vector3.zero;
        rcpos = Vector3.zero;

        center.x = (float)((coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / 2.0 + coord.coordx[coord.minic[0]]);
        center.z = (float)((coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / 2.0 + coord.coordy[coord.minic[1]]);
        center.y = (float)(coord.coordz[K]);

        calcScale();

        guidz = (float)(voirConst.OS_GUI_Z/(coord.maxic[2] - coord.minic[2]+1));
        wireframe = false; carpetplot = false;
        pressed = false;
    }

    void SetMesh()
    {
        slicerxy.SetVertices(verts);
        slicerxy.SetColors(vcolor);
        slicerxy.SetNormals(norm);
        slicerxy.SetTriangles(tindex, 0);
        slicerxy.RecalculateBounds();
    }

    void Alloc()
    {
        verts = new Vector3[voirConst.OS_DIV*voirConst.OS_DIV];
        norm = new Vector3[voirConst.OS_DIV*voirConst.OS_DIV];
        vcolor = new Color[voirConst.OS_DIV*voirConst.OS_DIV];
        for(int i=0;i<voirConst.OS_DIV*voirConst.OS_DIV;i++){
            verts[i] = new Vector3(0f,0f,0f);
            norm[i] = new Vector3(0f,0f,0f);
            vcolor[i] = new Color(0f,0f,0f,1f);
        }

        pos = new double [3];
        height = new float[voirConst.OS_DIV, voirConst.OS_DIV];
        nx = new float[3];
        ny = new float[3];

        slicerxy = new Mesh();
        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = slicerxy;
    }

    void ReGenerateSlice(int k)
    {
        K = k;
        setSliceZ(K);
        if (carpetplot)
        {
            float cmag = voirConst.OS_CARPET / 2.0f;
            carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
            setVertColorXYC();
        }
        else
        {
            setVertColorXY();
        }
        SetMesh();
        if (wireframe) slicerxy.SetIndices(slicerxy.GetIndices(0), MeshTopology.Lines, 0);
    }

    public void SetParams(int n, bool carp, bool wire)
    {
        sn = n;
        setSliceZ(K);
        if (carp) {
            float cmag = voirConst.OS_CARPET / 2.0f;
            carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
            setVertColorXYC();
        }
        else
        {
            setVertColorXY();
        }
        SetMesh();
        if (wire) slicerxy.SetIndices(slicerxy.GetIndices(0), MeshTopology.Lines, 0);
        carpetplot = carp;
        wireframe = wire;
    }

    public void GenerateSlice(int n, int z, bool wire)
    {
        K = z;
        sn = n;
        setSliceZ(z);
        setVertColorXY();
        SetMesh();
        if(wire) slicerxy.SetIndices(slicerxy.GetIndices(0), MeshTopology.Lines, 0);
    }

    public void GenerateSliceC(int n, int z, int cmag, bool wire)
    {
        K = z;
        sn = n;
        carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
        setSliceZ(z);
        setVertColorXYC();
        SetMesh();
        if (wire) slicerxy.SetIndices(slicerxy.GetIndices(0), MeshTopology.Lines, 0);
    }

    public int ZPos()
    {
        return K;
    }

    void IsRawdatamode()
    {
        if (coord.gn[0] <= voirConst.OS_DIV && coord.gn[1] <= voirConst.OS_DIV)
        {
            rawdatamode = true;
        }
        else
        {
            rawdatamode = false;
        }
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
        sn = 0;
        K = (coord.maxic[2] - coord.minic[2]) / 2; ;

        if (coord.gn[0] > voirConst.OS_DIV) numx = voirConst.OS_DIV;
        else numx = coord.gn[0];

        if(coord.gn[1] > voirConst.OS_DIV) numy = voirConst.OS_DIV;
        else numy = coord.gn[1];

        dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]])/(numx-1);
        dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]])/(numy-1);

        IsRawdatamode();
        setVertIndex();
        setVertPosXY(K);
        setVertColorXY();
        SetMesh();

        carpet = 0.0f;

    }

    public bool SlicerXYROI()
    {
        bool exist = true;
        if (K > coord.maxic[2] || K < coord.minic[2])
        {
            K = (coord.maxic[2] - coord.minic[2]) / 2 + coord.minic[2];
            exist = false;
        }

        if (coord.gn[0] > voirConst.OS_DIV) numx = voirConst.OS_DIV;
        else numx = coord.gn[0];

        if (coord.gn[1] > voirConst.OS_DIV) numy = voirConst.OS_DIV;
        else numy = coord.gn[1];

        dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / (numx - 1);
        dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / (numy - 1);

        slicerxy.Clear();
        IsRawdatamode();
        setVertIndex();
        setVertPosXY(K);
        ReGenerateSlice(K);
        return exist;
    }

    void setVertIndex()
    {
        int n = 0;
        tindex = null;
        tindex = new int[(numx - 1) * (numy - 1) * 2 * 3];
        for (int j=0;j<numy-1;j++)
        {
            for(int i=0;i<numx-1;i++)
            {
                tindex[n] = i + j*numx;
                tindex[n+1] = i+numx + j*numx;
                tindex[n+2] = i+1 + j*numx;
                n += 3;
                tindex[n] = i+numx + j*numx;
                tindex[n+1] = i+1+numx + j*numx;
                tindex[n+2] = i+1 + j*numx;
                n += 3;
            }
        }
    }
    
    void setVertPosXY(int k)
    {
        int vn;
        if (!rawdatamode)
        {
            for (int j = 0; j < numy; j++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * j;
                    if (coord.gn[0] == numx) verts[vn][0] = (float)coord.coordx[coord.minic[0] + i];
                    else verts[vn][0] = (float)(coord.coordx[coord.minic[0]] + i * dx);

                    if (coord.gn[1] == numy) verts[vn][2] = (float)coord.coordy[coord.minic[1] + j];
                    else verts[vn][2] = (float)(coord.coordy[coord.minic[1]] + j * dy);

                    verts[vn][1] = (float)coord.coordz[k];

                    // for safety
                    if (i == 0) verts[vn][0] = (float)(coord.coordx[coord.minic[0]] + 0.01 * coord.dx[0]);
                    if (i == numx - 1) verts[vn][0] = (float)(coord.coordx[coord.maxic[0]] - 0.01 * coord.dx[0]);

                    if (j == 0) verts[vn][2] = (float)(coord.coordy[coord.minic[1]] + 0.01 * coord.dx[1]);
                    if (j == numy - 1) verts[vn][2] = (float)(coord.coordy[coord.maxic[1]] - 0.01 * coord.dx[1]);

                    if (k == 0) verts[vn][1] = (float)(coord.coordz[coord.minic[2]] + 0.01 * coord.dx[2]);
                    if (k == coord.gn[2] - 1) verts[vn][1] = (float)(coord.coordz[coord.maxic[2]] - 0.01 * coord.dx[2]);

                }
            }
        }
        else {
            for (int j = 0; j < numy; j++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * j;
                    verts[vn][0] = (float)(coord.coordx[coord.minic[0]+i]);
                    verts[vn][2] = (float)(coord.coordy[coord.minic[1]+j]);
                    verts[vn][1] = (float)coord.coordz[k];
                }
            }

        }
    }

    void setSliceZ(int k)
    {
        int vn;
        for (int j = 0; j < numy; j++)
        {
            for (int i = 0; i < numx; i++)
            {
                vn = i + numx * j;
                verts[vn][1] = (float)coord.coordz[k];

                // for safety
                if (!rawdatamode)
                {
                    if (k == coord.minic[2]) verts[vn][1] += (float)(0.01 * coord.dx[2]);
                    if (k == coord.maxic[2]) verts[vn][1] -= (float)(0.01 * coord.dx[2]);
                }
            }
        }
    }

    void setVertColorXY()
    {
        int vn;
        bool inside;
        double sval;
        float svalf;
        if (param.single)
        {
            for (int j = 0; j < numy; j++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * j;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    norm[vn][0] = 0.0f;
                    norm[vn][1] = 1.0f;
                    norm[vn][2] = 0.0f;

                    if (!rawdatamode)
                    {
                        inside = data.getScalValf(sn, pos, out svalf);
                    }
                    else {
                        inside = true;
                        svalf = data.getScalValf(sn, K, j + coord.minic[1], i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXY: position coloring error");
                    }

                }
            }

        }
        else
        {
            for (int j = 0; j < numy; j++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * j;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    norm[vn][0] = 0.0f;
                    norm[vn][1] = 1.0f;
                    norm[vn][2] = 0.0f;

                    if (!rawdatamode)
                    {
                        inside = data.getScalVald(sn, pos, out sval);
                    }
                    else
                    {
                        inside = true;
                        sval = data.getScalVald(sn, K, j + coord.minic[1], i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        //voirFunc.Log(pos[0]+" "+pos[1]+" "+pos[2]+" "+coord.coordx[coord.minic[0]]
                        //    +" " + coord.coordx[coord.maxic[0]]
                        //    +" " + coord.coordy[coord.minic[1]] + " " + coord.coordy[coord.maxic[1]]);
                        voirFunc.Error("SlicerXY: position coloring error");
                    }

                }
            }
        }
    }

    void setVertColorXYC()
    {
        int vn;
        bool inside;
        double sval;
        float svalf;
        float maxval, minval, dif;
        maxval = (float)data.voirScalData[sn].maxvalue;
        minval = (float)data.voirScalData[sn].minvalue;
        dif = maxval - minval;
        if (param.single)
        {
            for (int j = 0; j < numy; j++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * j;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    if (!rawdatamode)
                    {
                        inside = data.getScalValf(sn, pos, out svalf);
                    }
                    else
                    {
                        inside = true;
                        svalf = data.getScalValf(sn, K, j + coord.minic[1], i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                        height[j, i] = svalf;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXY: position coloring error");
                    }

                }
            }

        }
        else
        {
            for (int j = 0; j < numy; j++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * j;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    if (!rawdatamode)
                    {
                        inside = data.getScalVald(sn, pos, out sval);
                    }
                    else
                    {
                        inside = true;
                        sval = data.getScalVald(sn, K, j + coord.minic[1], i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                        height[j, i] = (float)sval;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXY: position coloring error");
                    }

                }
            }
        }

        for (int j = 0; j < numy; j++)
        {
            for (int i = 0; i < numx; i++)
            {
                vn = i + numx * j;
                height[j,i] = (-0.5f + (height[j, i] - minval) / dif) * carpet;
                verts[vn][1] += height[j,i];
            }
        }

        for (int j = 0; j < numy; j++)
        {
            for (int i = 0; i < numx; i++)
            {
                vn = i + numx * j;
                if (i == 0)
                {
                    nx[0] = -(height[j, i + 1] - height[j, i]);
                    nx[2] = 0.0f;
                    nx[1] = verts[vn+1][0] - verts[vn][0];
                }
                else if(i==numx-1)
                {
                    nx[0] = -(height[j, i] - height[j, i-1]);
                    nx[2] = 0.0f;
                    nx[1] = verts[vn][0] - verts[vn-1][0];
                }
                else
                {
                    nx[0] = -(height[j, i+1] - height[j, i - 1]);
                    nx[2] = 0.0f;
                    nx[1] = verts[vn + 1][0] - verts[vn-1][0];
                }

                if (j == 0)
                {
                    ny[0] = 0.0f;
                    ny[2] = -(height[j + 1, i] - height[j, i]);
                    ny[1] = verts[vn + numx][2] - verts[vn][2];
                }
                else if (j == numy - 1)
                {
                    ny[0] = 0.0f;
                    ny[2] = -(height[j, i] - height[j - 1, i]);
                    ny[1] = verts[vn][2] - verts[vn-numx][2];
                }
                else
                {
                    ny[0] = 0.0f;
                    ny[2] = -(height[j + 1, i] - height[j - 1, i]);
                    ny[1] = verts[vn + numx][2] - verts[vn-numx][2];
                }

                voirFunc.Normalize(nx);
                voirFunc.Normalize(ny);
                nx[0] += ny[0];
                nx[1] += ny[1];
                nx[2] += ny[2];
                voirFunc.Normalize(nx);
                norm[vn][0] = nx[0];
                norm[vn][1] = nx[1];
                norm[vn][2] = nx[2];

            }
        }

    }

}
