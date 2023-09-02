//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirOrthoSlicerXZ2 : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh slicerxz;
    Vector3[] verts;
    Vector3[] norm;
    Color[] vcolor;
    int[] tindex;
    double[] pos;
    double dx, dz;
    int numz, numx;
    int J; // Slicer's Y
    int sn;
    voirParam param;
    voirCoord coord;
    voirData data;
    voirColor col;
    float carpet;
    float[,] height;
    float[] nx;
    float[] nz;
    bool rawdatamode;

    tracking trackd;
    MainControll maincontroll;
    GameObject SlicerUI;
    Vector3 center;
    float scalex, scalez;
    float guidy;
    float recy, cury;
    int reciy, curiy;
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
        if (maincontroll.SelectedMethod != VizMethods.OrthoSlicerXZ2) return;

        if (trackd.getRGripChange() == 1)
        {
            maincontroll.SetNone();
            gameObject.SetActive(false);
        }

        switch (trackd.getRTrigChange())
        {
            case 1:
                SlicerUI.SetActive(true);
                trackd.getRContData(voirConst.TD_POS, ref rcpos);
                reciy = J;
                recy = rcpos.z;
                calcScale();
                pressed = true;
                break;

            case 0:
                if (pressed)
                {
                    SlicerUI.SetActive(true);
                    trackd.getRContData(voirConst.TD_POS, ref rcpos);
                    cury = rcpos.z;
                    int ciy = (int)((cury - recy) / guidy) + reciy;
                    if (ciy > coord.maxic[1]) ciy = coord.maxic[1];
                    else if (ciy < coord.minic[1]) ciy = coord.minic[1];
                    curiy = ciy;
                        //voirFunc.Log("z:"+((curz - recz) / guidz));
                        //voirFunc.Log("cz:" + curz + " rz" + recz+" curiz:"+curiz);
                    center.z = (float)coord.coordy[curiy];
                    slicerui.GUIXZ(center, scalex, scalez);
                }
                break;

            case -1:
                if (curiy > coord.maxic[1]) curiy = coord.maxic[1];
                else if (curiy < coord.minic[1]) curiy = coord.minic[1];
                voirFunc.Log("curiz " + curiy);
                ReGenerateSlice(curiy);
                pressed = false;
                SlicerUI.SetActive(false);
                break;
        }

    }

    void calcScale()
    {
        scalex = (float)(coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]);
        scalez = (float)(coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]);
    }

    void InitGUI()
    {
        SlicerUI = GameObject.Find("OrthoSlicerUI");
        slicerui = GameObject.Find("OrthoSlicerUI").GetComponent<OrthoSlicerUI>();

        center = Vector3.zero;
        rcpos = Vector3.zero;

        center.x = (float)((coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / 2.0 + coord.coordx[coord.minic[0]]);
        center.z = (float)(coord.coordy[J]);
        center.y = (float)((coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / 2.0 + coord.coordz[coord.minic[2]]);

        calcScale();

        guidy = (float)(voirConst.OS_GUI_Y / (coord.maxic[1] - coord.minic[1] + 1));
        wireframe = false; carpetplot = false;
        pressed = false;
    }

    void SetMesh()
    {
        slicerxz.SetVertices(verts);
        slicerxz.SetColors(vcolor);
        slicerxz.SetNormals(norm);
        slicerxz.SetTriangles(tindex, 0);
        slicerxz.RecalculateBounds();
        //slicerxz.RecalculateNormals();
    }

    void Alloc()
    {
        verts = new Vector3[voirConst.OS_DIV * voirConst.OS_DIV];
        norm = new Vector3[voirConst.OS_DIV * voirConst.OS_DIV];
        vcolor = new Color[voirConst.OS_DIV * voirConst.OS_DIV];
        for(int i=0;i<voirConst.OS_DIV * voirConst.OS_DIV;i++){
            verts[i] = new Vector3(0f,0f,0f);
            norm[i] = new Vector3(0f,0f,0f);
            vcolor[i] = new Color(0f,0f,0f,1f);
        }
        pos = new double[3];
        height = new float[voirConst.OS_DIV, voirConst.OS_DIV];
        nx = new float[3];
        nz = new float[3];

        slicerxz = new Mesh();
        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = slicerxz;
    }

    void ReGenerateSlice(int j)
    {
        J = j;
        setSliceY(J);
        if (carpetplot)
        {
            float cmag = voirConst.OS_CARPET / 2.0f;
            carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
            setVertColorXZC();
        }
        else
        {
            setVertColorXZ();
        }
        SetMesh();
        if (wireframe) slicerxz.SetIndices(slicerxz.GetIndices(0), MeshTopology.Lines, 0);
    }

    public void SetParams(int n, bool carp, bool wire)
    {
        sn = n;
        setSliceY(J);
        if (carp)
        {
            float cmag = voirConst.OS_CARPET / 2.0f;
            carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
            setVertColorXZC();
        }
        else
        {
            setVertColorXZ();
        }
        SetMesh();
        if (wire) slicerxz.SetIndices(slicerxz.GetIndices(0), MeshTopology.Lines, 0);
        carpetplot = carp;
        wireframe = wire;
    }

    public void GenerateSlice(int n, int y, bool wire)
    {
        J = y;
        sn = n;
        setSliceY(y);
        setVertColorXZ();
        SetMesh();
        if (wire) slicerxz.SetIndices(slicerxz.GetIndices(0), MeshTopology.Lines, 0);
    }

    public void GenerateSliceC(int n, int y, int cmag, bool wire)
    {
        J = y;
        sn = n;
        carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
        setSliceY(y);
        setVertColorXZC();
        SetMesh();
        if (wire) slicerxz.SetIndices(slicerxz.GetIndices(0), MeshTopology.Lines, 0);
    }

    public int YPos()
    {
        return J;
    }

    void IsRawdatamode()
    {
        if (coord.gn[0] <= voirConst.OS_DIV && coord.gn[2] <= voirConst.OS_DIV)
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
        J = (coord.maxic[1] - coord.minic[1]) / 2;

        if (coord.gn[0] > voirConst.OS_DIV) numx = voirConst.OS_DIV;
        else numx = coord.gn[0];

        if (coord.gn[2] > voirConst.OS_DIV) numz = voirConst.OS_DIV;
        else numz = coord.gn[2];

        dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / (numx - 1);
        dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (numz - 1);

        IsRawdatamode();
        setVertIndex();
        setVertPosXZ(J);
        setVertColorXZ();
        SetMesh();

        carpet = 0.0f;
    }

    public bool SlicerXZROI()
    {
        bool exist = true;
        if (J > coord.maxic[1] || J < coord.minic[1])
        {
            J = (coord.maxic[1] - coord.minic[1]) / 2 + coord.minic[1];
            exist = false;
        }

        if (coord.gn[0] > voirConst.OS_DIV) numx = voirConst.OS_DIV;
        else numx = coord.gn[0];

        if (coord.gn[2] > voirConst.OS_DIV) numz = voirConst.OS_DIV;
        else numz = coord.gn[2];

        dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / (numx - 1);
        dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (numz - 1);

        slicerxz.Clear();
        IsRawdatamode();
        setVertIndex();
        setVertPosXZ(J);
        ReGenerateSlice(J);
        //        setSliceY(J);
        //        setVertColorXZ();
        //        SetMesh();

        return exist;
    }

    void setVertIndex()
    {
        int n = 0;
        tindex = null;
        tindex = new int[(numz - 1) * (numx - 1) * 2 * 3];
        for (int k = 0; k < numz - 1; k++)
        {
            for (int i = 0; i < numx - 1; i++)
            {
                tindex[n] = i + k * numx;
                tindex[n + 1] = i + numx + k * numx;
                tindex[n + 2] = i + 1 + k * numx;
                n += 3;
                tindex[n] = i + numx + k * numx;
                tindex[n + 1] = i + 1 + numx + k * numx;
                tindex[n + 2] = i + 1 + k * numx;
                n += 3;
            }
        }
    }

    void setVertPosXZ(int j)
    {
        int vn;
        if (!rawdatamode)
        {
            for (int k = 0; k < numz; k++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * k;
                    if (coord.gn[0] == numx) verts[vn][0] = (float)coord.coordx[coord.minic[0] + i];
                    else verts[vn][0] = (float)(coord.coordx[coord.minic[0]] + i * dx);

                    if (coord.gn[2] == numz) verts[vn][1] = (float)coord.coordz[coord.minic[2] + k];
                    else verts[vn][1] = (float)(coord.coordz[coord.minic[2]] + k * dz);

                    verts[vn][2] = (float)coord.coordy[j];

                    // for safety
                    if (i == 0) verts[vn][0] = (float)(coord.coordx[coord.minic[0]] + 0.01 * coord.dx[0]);
                    if (i == numx - 1) verts[vn][0] = (float)(coord.coordx[coord.maxic[0]] - 0.01 * coord.dx[0]);

                    if (k == 0) verts[vn][1] = (float)(coord.coordz[coord.minic[2]] + 0.01 * coord.dx[2]);
                    if (k == numz - 1) verts[vn][1] = (float)(coord.coordz[coord.maxic[2]] - 0.01 * coord.dx[2]);

                    if (j == 0) verts[vn][2] = (float)(coord.coordy[coord.minic[1]] + 0.01 * coord.dx[1]);
                    if (j == coord.gn[1] - 1) verts[vn][2] = (float)(coord.coordy[coord.maxic[1]] - 0.01 * coord.dx[1]);

                }
            }
        }
        else
        {
            for (int k = 0; k < numz; k++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * k;
                    verts[vn][0] = (float)(coord.coordx[coord.minic[0] + i]);
                    verts[vn][1] = (float)(coord.coordz[coord.minic[2] + k]);
                    verts[vn][2] = (float)coord.coordy[j];
                }
            }
        }
    }

    void setSliceY(int j)
    {
        int vn;
        for (int k = 0; k < numz; k++)
        {
            for (int i = 0; i < numx; i++)
            {
                vn = i + numx * k;
                verts[vn][2] = (float)coord.coordy[j];

                // for safety
                if (!rawdatamode)
                {
                    if (j == coord.minic[1]) verts[vn][2] += (float)(0.01 * coord.dx[1]);
                    if (j == coord.maxic[1]) verts[vn][2] -= (float)(0.01 * coord.dx[1]);
                }
            }
        }
    }

    void setVertColorXZ()
    {
        int vn;
        bool inside;
        double sval;
        float svalf;
        if (param.single)
        {
            for (int k = 0; k < numz; k++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * k;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    norm[vn][0] = 0.0f;
                    norm[vn][1] = 0.0f;
                    norm[vn][2] = 1.0f;

                    if (!rawdatamode)
                    {
                        inside = data.getScalValf(sn, pos, out svalf);
                    }
                    else
                    {
                        inside = true;
                        svalf = data.getScalValf(sn, k + coord.minic[2], J, i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXZ: position coloring error");
                    }

                }
            }

        }
        else
        {
            for (int k = 0; k < numz; k++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * k;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    norm[vn][0] = 0.0f;
                    norm[vn][1] = 0.0f;
                    norm[vn][2] = 1.0f;

                    if (!rawdatamode)
                    {
                        inside = data.getScalVald(sn, pos, out sval);
                    }
                    else
                    {
                        inside = true;
                        sval = data.getScalVald(sn, k + coord.minic[2], J, i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXZ: position coloring error");
                    }

                }
            }
        }
    }

    void setVertColorXZC()
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
            for (int k = 0; k < numz; k++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * k;
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
                        svalf = data.getScalValf(sn, k + coord.minic[2], J, i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                        height[k, i] = svalf;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXZ: position coloring error");
                    }

                }
            }

        }
        else
        {
            for (int k = 0; k < numz; k++)
            {
                for (int i = 0; i < numx; i++)
                {
                    vn = i + numx * k;
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
                        sval = data.getScalVald(sn, k + coord.minic[2], J, i + coord.minic[0]);
                    }

                    if (inside)
                    {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                        height[k, i] = (float)sval;
                    }
                    else
                    {
                        voirFunc.Error("SlicerXZ: position coloring error");
                    }

                }
            }
        }

        for (int k = 0; k < numz; k++)
        {
            for (int i = 0; i < numx; i++)
            {
                vn = i + numx * k;
                height[k, i] = (-0.5f + (height[k, i] - minval) / dif) * carpet;
                verts[vn][2] += height[k, i];
            }
        }

        for (int k = 0; k < numz; k++)
        {
            for (int i = 0; i < numx; i++)
            {
                vn = i + numx * k;
                if (i == 0)
                {
                    nx[0] = -(height[k, i + 1] - height[k, i]);
                    nx[2] = verts[vn + 1][0] - verts[vn][0];
                    nx[1] = 0.0f;
                }
                else if (i == numx - 1)
                {
                    nx[0] = -(height[k, i] - height[k, i - 1]);
                    nx[2] = verts[vn][0] - verts[vn - 1][0];
                    nx[1] = 0.0f;
                }
                else
                {
                    nx[0] = -(height[k, i + 1] - height[k, i - 1]);
                    nx[2] = verts[vn + 1][0] - verts[vn - 1][0];
                    nx[1] = 0.0f;
                }

                if (k == 0)
                {
                    nz[0] = 0.0f;
                    nz[2] = verts[vn + numx][1] - verts[vn][1];
                    nz[1] = -(height[k + 1, i] - height[k, i]);
                }
                else if (k == numz - 1)
                {
                    nz[0] = 0.0f;
                    nz[2] = verts[vn][1] - verts[vn - numx][1];
                    nz[1] = -(height[k, i] - height[k - 1, i]);
                }
                else
                {
                    nz[0] = 0.0f;
                    nz[2] = verts[vn + numx][1] - verts[vn - numx][1];
                    nz[1] = -(height[k + 1, i] - height[k - 1, i]);
                }

                voirFunc.Normalize(nx);
                voirFunc.Normalize(nz);
                nx[0] += nz[0];
                nx[1] += nz[1];
                nx[2] += nz[2];
                voirFunc.Normalize(nx);
                norm[vn][0] = nx[0];
                norm[vn][1] = nx[1];
                norm[vn][2] = nx[2];

            }
        }

    }
}
