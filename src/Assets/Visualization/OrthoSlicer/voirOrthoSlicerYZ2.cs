//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirOrthoSlicerYZ2 : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh sliceryz;
    Vector3[] verts;
    Vector3[] norm;
    Color[] vcolor;
    int[] tindex;
    double[] pos;
    double dy, dz;
    int numz, numy;
    int I; // Slicer's X
    int sn;
    voirParam param;
    voirCoord coord;
    voirData data;
    voirColor col;
    float carpet;
    float[,] height;
    float[] ny;
    float[] nz;
    bool rawdatamode;

    tracking trackd;
    MainControll maincontroll;
    GameObject SlicerUI;
    Vector3 center;
    float scaley, scalez;
    float guidx;
    float recx, curx;
    int recix, curix;
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
        if (maincontroll.SelectedMethod != VizMethods.OrthoSlicerYZ2) return;

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
                recix = I;
                recx = rcpos.x;
                calcScale();
                pressed = true;
                break;

            case 0:
                if (pressed)
                {
                    SlicerUI.SetActive(true);
                    trackd.getRContData(voirConst.TD_POS, ref rcpos);
                    curx = rcpos.x;
                    int cix = (int)((curx - recx) / guidx) + recix;
                    if (cix > coord.maxic[0]) cix = coord.maxic[0];
                    else if (cix < coord.minic[0]) cix = coord.minic[0];
                    curix = cix;
                        //voirFunc.Log("z:"+((curz - recz) / guidz));
                        //voirFunc.Log("cz:" + curz + " rz" + recz+" curiz:"+curiz);
                    center.x = (float)coord.coordx[curix];
                    slicerui.GUIYZ(center, scaley, scalez);
                }
                break;

            case -1:
                if (curix > coord.maxic[0]) curix = coord.maxic[0];
                else if (curix < coord.minic[0]) curix = coord.minic[0];
                voirFunc.Log("curiz " + curix);
                ReGenerateSlice(curix);
                pressed = false;
                SlicerUI.SetActive(false);
                break;
        }

    }

    void calcScale()
    {
        scaley = (float)((coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / 1.0);
        scalez = (float)((coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / 1.0);
    }

    void InitGUI()
    {
        SlicerUI = GameObject.Find("OrthoSlicerUI");
        slicerui = GameObject.Find("OrthoSlicerUI").GetComponent<OrthoSlicerUI>();

        center = Vector3.zero;
        rcpos = Vector3.zero;

        center.x = (float)(coord.coordx[I]);
        center.z = (float)((coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / 2.0 + coord.coordy[coord.minic[1]]);
        center.y = (float)((coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / 2.0 + coord.coordz[coord.minic[2]]);
        scaley = (float)((coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / 1.0);
        scalez = (float)((coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / 1.0);

        guidx = (float)(voirConst.OS_GUI_X / (coord.maxic[0] - coord.minic[0] + 1));
        wireframe = false; carpetplot = false;
        pressed = false;
    }

    void SetMesh()
    {
        sliceryz.SetVertices(verts);
        sliceryz.SetColors(vcolor);
        sliceryz.SetNormals(norm);
        sliceryz.SetTriangles(tindex, 0);
        sliceryz.RecalculateBounds();
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
        ny = new float[3];
        nz = new float[3];

        sliceryz = new Mesh();
        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = sliceryz;
    }

    void ReGenerateSlice(int i)
    {
        I = i;
        setSliceX(I);
        if (carpetplot)
        {
            float cmag = voirConst.OS_CARPET / 2.0f;
            carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
            setVertColorYZC();
        }
        else
        {
            setVertColorYZ();
        }
        SetMesh();
        if (wireframe) sliceryz.SetIndices(sliceryz.GetIndices(0), MeshTopology.Lines, 0);
    }

    public void SetParams(int n, bool carp, bool wire)
    {
        sn = n;
        setSliceX(I);
        if (carp)
        {
            float cmag = voirConst.OS_CARPET / 2.0f;
            carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
            setVertColorYZC();
        }
        else
        {
            setVertColorYZ();
        }
        SetMesh();
        if (wire) sliceryz.SetIndices(sliceryz.GetIndices(0), MeshTopology.Lines, 0);
        carpetplot = carp;
        wireframe = wire;
    }


    public void GenerateSlice(int n, int x, bool wire)
    {
        I = x;
        sn = n;
        setSliceX(x);
        setVertColorYZ();
        SetMesh();
        if (wire) sliceryz.SetIndices(sliceryz.GetIndices(0), MeshTopology.Lines, 0);
    }

    public void GenerateSliceC(int n, int x, int cmag, bool wire)
    {
        I = x;
        sn = n;
        carpet = (float)(coord.dx[0] + coord.dx[1] + coord.dx[2]) / 3.0f * cmag;
        setSliceX(x);
        setVertColorYZC();
        SetMesh();
        if (wire) sliceryz.SetIndices(sliceryz.GetIndices(0), MeshTopology.Lines, 0);
    }

    public int XPos()
    {
        return I;
    }

    void IsRawdatamode()
    {
        if (coord.gn[1] <= voirConst.OS_DIV && coord.gn[2] <= voirConst.OS_DIV)
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
        I = (coord.maxic[0] - coord.minic[0]) / 2;

        if (coord.gn[1] > voirConst.OS_DIV) numy = voirConst.OS_DIV;
        else numy = coord.gn[1];

        if (coord.gn[2] > voirConst.OS_DIV) numz = voirConst.OS_DIV;
        else numz = coord.gn[2];

        dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / (numy - 1);
        dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (numz - 1);

        IsRawdatamode();
        setVertIndex();
        setVertPosYZ(I);
        setVertColorYZ();
        SetMesh();

        carpet = 0.0f;
    }

    public bool SlicerYZROI()
    {
        bool exist = true;
        if (I > coord.maxic[0] || I < coord.minic[0])
        {
            I = (coord.maxic[0] - coord.minic[0]) / 2 + coord.minic[0];
            exist = false;
        }

        if (coord.gn[1] > voirConst.OS_DIV) numy = voirConst.OS_DIV;
        else numy = coord.gn[1];

        if (coord.gn[2] > voirConst.OS_DIV) numz = voirConst.OS_DIV;
        else numz = coord.gn[2];

        dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / (numy - 1);
        dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (numz - 1);

        sliceryz.Clear();
        IsRawdatamode();
        setVertIndex();
        setVertPosYZ(I);
        ReGenerateSlice(I);
        //        setSliceX(I);
        //        setVertColorYZ();
        //        SetMesh();

        return exist;
    }

    void setVertIndex()
    {
        int n = 0;
        tindex = null;
        tindex = new int[(numz - 1) * (numy - 1) * 2 * 3];
        for (int k = 0; k < numz - 1; k++)
        {
            for (int j = 0; j < numy - 1; j++)
            {
                tindex[n] = j + k * numy;
                tindex[n + 1] = j + numy + k * numy;
                tindex[n + 2] = j + 1 + k * numy;
                n += 3;
                tindex[n] = j + numy + k * numy;
                tindex[n + 1] = j + 1 + numy + k * numy;
                tindex[n + 2] = j + 1 + k * numy;
                n += 3;
            }
        }
    }

    void setVertPosYZ(int i)
    {
        int vn;
        if (!rawdatamode)
        {
            for (int k = 0; k < numz; k++)
            {
                for (int j = 0; j < numy; j++)
                {
                    vn = j + numy * k;
                    if (coord.gn[1] == numy) verts[vn][2] = (float)coord.coordy[coord.minic[1] + j];
                    else verts[vn][2] = (float)(coord.coordy[coord.minic[1]] + j * dy);

                    if (coord.gn[2] == numz) verts[vn][1] = (float)coord.coordz[coord.minic[2] + k];
                    else verts[vn][1] = (float)(coord.coordz[coord.minic[2]] + k * dz);

                    verts[vn][0] = (float)coord.coordx[i];

                    // for safety
                    if (j == 0) verts[vn][2] = (float)(coord.coordy[coord.minic[1]] + 0.01 * coord.dx[1]);
                    if (j == numy - 1) verts[vn][2] = (float)(coord.coordy[coord.maxic[1]] - 0.01 * coord.dx[1]);

                    if (k == 0) verts[vn][1] = (float)(coord.coordz[coord.minic[2]] + 0.01 * coord.dx[2]);
                    if (k == numz - 1) verts[vn][1] = (float)(coord.coordz[coord.maxic[2]] - 0.01 * coord.dx[2]);

                    if (i == 0) verts[vn][0] = (float)(coord.coordx[coord.minic[0]] + 0.01 * coord.dx[0]);
                    if (i == coord.gn[0] - 1) verts[vn][0] = (float)(coord.coordx[coord.maxic[0]] - 0.01 * coord.dx[0]);

                }
            }
        }
        else
        {
            for (int k = 0; k < numz; k++)
            {
                for (int j = 0; j < numy; j++)
                {
                    vn = j + numy * k;
                    verts[vn][2] = (float)(coord.coordy[coord.minic[1] + j]);
                    verts[vn][1] = (float)(coord.coordz[coord.minic[2] + k]);
                    verts[vn][0] = (float)coord.coordx[i];
                }
            }
        }
    }

    void setSliceX(int i)
    {
        int vn;
        for (int k = 0; k < numz; k++)
        {
            for (int j = 0; j < numy; j++)
            {
                vn = j + numy * k;
                verts[vn][0] = (float)coord.coordx[i];

                // for safety
                if (!rawdatamode)
                {
                    if (i == coord.minic[0]) verts[vn][0] += (float)(0.01 * coord.dx[0]);
                    if (i == coord.maxic[0]) verts[vn][0] -= (float)(0.01 * coord.dx[0]);
                }
            }
        }
    }

    void setVertColorYZ()
    {
        int vn;
        bool inside;
        double sval;
        float svalf;
        if (param.single)
        {
            for (int k = 0; k < numz; k++)
            {
                for (int j = 0; j < numy; j++)
                {
                    vn = j + numy * k;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    norm[vn][0] = 1.0f;
                    norm[vn][1] = 0.0f;
                    norm[vn][2] = 0.0f;

                    if (!rawdatamode)
                    {
                        inside = data.getScalValf(sn, pos, out svalf);
                    }
                    else
                    {
                        inside = true;
                        svalf = data.getScalValf(sn, k + coord.minic[2], j + coord.minic[1], I);
                    }

                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        voirFunc.Error("SlicerYZ: position coloring error");
                    }

                }
            }

        }
        else
        {
            for (int k = 0; k < numz; k++)
            {
                for (int j = 0; j < numy; j++)
                {
                    vn = j + numy * k;
                    pos[0] = (double)verts[vn][0];
                    pos[1] = (double)verts[vn][2];
                    pos[2] = (double)verts[vn][1];

                    norm[vn][0] = 1.0f;
                    norm[vn][1] = 0.0f;
                    norm[vn][2] = 0.0f;

                    if (!rawdatamode)
                    {
                        inside = data.getScalVald(sn, pos, out sval);
                    }
                    else
                    {
                        inside = true;
                        sval = data.getScalVald(sn, k + coord.minic[2], j + coord.minic[1], I);
                    }

                    if (inside)
                    {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                    }
                    else
                    {
                        //voirFunc.Log("x:" + pos[0] + " i: " + I + " coordx[I]:" + coord.coordx[I]);
                        voirFunc.Error("SlicerYZ: position coloring error");
                    }

                }
            }
        }
    }

    void setVertColorYZC()
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
                for (int j = 0; j < numy; j++)
                {
                    vn = j + numy * k;
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
                        svalf = data.getScalValf(sn, k + coord.minic[2], j + coord.minic[1], I);
                    }

                    if (inside)
                    {
                        sval = (double)svalf;
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                        height[k, j] = svalf;
                    }
                    else
                    {
                        voirFunc.Error("SlicerYZ: position coloring error");
                    }

                }
            }

        }
        else
        {
            for (int k = 0; k < numz; k++)
            {
                for (int j = 0; j < numy; j++)
                {
                    vn = j + numy * k;
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
                        sval = data.getScalVald(sn, k + coord.minic[2], j + coord.minic[1], I);
                    }

                    if (inside)
                    {
                        col.getScalColor(sn, sval, ref vcolor[vn]);
                        vcolor[vn][3] = 1.0f;
                        height[k, j] = (float)sval;
                    }
                    else
                    {
                        voirFunc.Error("SlicerYZ: position coloring error");
                    }

                }
            }
        }

        for (int k = 0; k < numz; k++)
        {
            for (int j = 0; j < numy; j++)
            {
                vn = j + numy * k;
                height[k, j] = (-0.5f + (height[k, j] - minval) / dif) * carpet;
                verts[vn][0] += height[k, j];
            }
        }

        for (int k = 0; k < numz; k++)
        {
            for (int j = 0; j < numy; j++)
            {
                vn = j + numy * k;
                if (j == 0)
                {
                    ny[0] = verts[vn + 1][2] - verts[vn][2];
                    ny[2] = -(height[k, j + 1] - height[k, j]);
                    ny[1] = 0.0f;
                }
                else if (j == numy - 1)
                {
                    ny[0] = verts[vn][2] - verts[vn - 1][2];
                    ny[2] = -(height[k, j] - height[k, j - 1]);
                    ny[1] = 0.0f;
                }
                else
                {
                    ny[0] = verts[vn + 1][2] - verts[vn - 1][2];
                    ny[2] = -(height[k, j + 1] - height[k, j - 1]);
                    ny[1] = 0.0f;
                }

                if (k == 0)
                {
                    nz[0] = verts[vn + numz][1] - verts[vn][1];
                    nz[2] = 0.0f;
                    nz[1] = -(height[k + 1, j] - height[k, j]);
                }
                else if (k == numz - 1)
                {
                    nz[0] = verts[vn][1] - verts[vn - numz][1];
                    nz[2] = 0.0f;
                    nz[1] = -(height[k, j] - height[k - 1, j]);
                }
                else
                {
                    nz[0] = verts[vn + numz][1] - verts[vn - numz][1];
                    nz[2] = 0.0f;
                    nz[1] = -(height[k + 1, j] - height[k - 1, j]);
                }

                voirFunc.Normalize(ny);
                voirFunc.Normalize(nz);
                ny[0] += nz[0];
                ny[1] += nz[1];
                ny[2] += nz[2];
                voirFunc.Normalize(ny);
                norm[vn][0] = ny[0];
                norm[vn][1] = ny[1];
                norm[vn][2] = ny[2];

            }
        }

    }

}
