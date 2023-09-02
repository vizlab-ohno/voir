//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using voirCommon;

public class voirIsoSurface : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh isosurf;
    int[] gniso;
    double[,,] scal4iso;
    float[,,] scal4isof;
    float[] coordx;
    float[] coordy;
    float[] coordz;

    int ns;
    double isolevel;
    int dn4iso;

    int[] vertex; // [8]
    int[,] connect;

    // ----------------------- For Parallel
    int[,,] tet; // tet[5,4]
    double[,] vertval; //vertval[8]
    float[,,] vertnorm; //vertnorm[8, 3]

    float[,,] vertpos; //(8,3)
    float[,,] tetpos; // (4,3)
    double[,] tetval; // (4)
    float[,,] tetnorm; // (4,3)
    int [] trinum; // 0, 1, 2
    float[,] direc;
    double[,] pos;
    float[,] norm;
    List<Vector3> IsoSurfVert;
    List<Vector3> IsoSurfNorm;
    List<int> IsoSurfIndex;
    List<Color> IsoSurfColor;

    List<Vector3>[] IsoSurfVertT;
    List<Vector3>[] IsoSurfNormT;
    List<int>[] IsoSurfIndexT;
    List<Color>[] IsoSurfColorT;
    
    int[] numSurfVerts;
    //Color vcolor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    // --------------------------

    voirParam param;
    voirData data;
    voirCoord coord;

    voirColor col;
    const int numisomaterial = 5;
    [SerializeField] Material[] mat = new Material[numisomaterial + 1];
    bool coloring;
    bool wireframe;
    int coldatn;

    bool roicalc;
    bool rawdatamode;

    tracking trackd;
    MainControll maincontroll;

    GameObject IsoSurfUI;
    float height;
    float guidz;
    float recz, curz;
    float recv, curv;
    IsosurfUI isosurfui;
    bool pressed;
    Vector3 rcpos;
    float maxv, minv, absmaxv;
    Vector3 center;
    Vector3 headpos;
    Vector3 headfwd;
    Vector3 headrgt;

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
        if (maincontroll.SelectedMethod == VizMethods.IsoSurface1)
        {
            if (trackd.getRGripChange() == 1) {
                maincontroll.SetNone();
                gameObject.SetActive(false);
            }

        switch (trackd.getRTrigChange())
        {
            case 1:
                IsoSurfUI.SetActive(true);
                trackd.getRContData(0, ref rcpos);
                recz = rcpos.y;
                recv = (float)isolevel;
                calc4gui();
                //    voirFunc.Log("x:"+center.x+" y:"+center.y+" z:"+center.z);
                isosurfui.initGUI(center, ns);
                pressed = true;
                break;

            case 0:
                if (pressed)
                {
                    IsoSurfUI.SetActive(true);
                    trackd.getRContData(0, ref rcpos);
                    curz = rcpos.y;

                    float cv = guidz*(curz - recz) + recv;

                    if (cv > maxv) cv = maxv;
                    else if (cv < minv) cv = minv;
                    curv = cv;
                    isosurfui.drawGUI(ns, curv);
                }
                break;

            case -1:
                if (curv > maxv) curv = maxv;
                else if (curv < minv) curv = minv;
                //voirFunc.Log("curv:" + curv+" max:"+maxv+" min:"+minv);
                ReGenerateIsoSurf(curv);
                pressed = false;
                IsoSurfUI.SetActive(false);
                break;
        }
        }
    }

    void InitGUI()
    {
        IsoSurfUI = GameObject.Find("IsoSurfUI");
        isosurfui = GameObject.Find("IsoSurfUI").GetComponent<IsosurfUI>();

        center = Vector3.zero;
        rcpos = Vector3.zero;

        headpos = Vector3.zero;
        headfwd = Vector3.zero;
        headrgt = Vector3.zero;

        pressed = false;
    }

    void calc4gui()
    {
        int nscal = ns;
        trackd.getHeadData(0, ref headpos);
        trackd.getHeadData(1, ref headfwd);
        trackd.getHeadData(3, ref headrgt);

        center.x = headpos.x + 1.0f * headfwd.x + 0.3f * headrgt.x;
        center.z = headpos.z + 1.0f * headfwd.z + 0.3f * headrgt.z;
        center.y = headpos.y;

        //voirFunc.Log("hfd"+trackd.hfd);
        //voirFunc.Log("hright" + trackd.hrt);
        //voirFunc.Log("hup" + trackd.hup);

        maxv = (float)data.voirScalData[nscal].maxvalue;
        minv = (float)data.voirScalData[nscal].minvalue;

        guidz = (float)((maxv-minv)/voirConst.IS_GUI_Z);
        absmaxv = Mathf.Abs(maxv);
        if (absmaxv < Mathf.Abs(minv)) absmaxv = Mathf.Abs(minv);
    }

    void Alloc()
    {
        int maxt = voirConst.MAXTHREADS;
        vertex = new int[8];
        connect = new int[6, 2];

        tet = new int[maxt, 5, 4];
        vertval = new double[maxt, 8];
        vertnorm = new float[maxt, 8, 3];
        vertpos = new float[maxt, 8, 3];
        tetpos = new float[maxt, 4, 3];
        tetnorm = new float[maxt, 4, 3];
        tetval = new double[maxt, 4];

        direc = new float[maxt, 3];
        pos = new double[maxt, 3];
        norm = new float[maxt, 3];

        trinum = new int[maxt];
        numSurfVerts = new int[maxt];

        IsoSurfVert = new List<Vector3>();
        IsoSurfNorm = new List<Vector3>();
        IsoSurfColor = new List<Color>();
        IsoSurfIndex = new List<int>();

        IsoSurfVertT = new List<Vector3> [maxt];
        IsoSurfNormT = new List<Vector3> [maxt];
        IsoSurfColorT = new List<Color> [maxt];
        IsoSurfIndexT = new List<int> [maxt];
        for (int i=0;i<maxt;i++)
        {
            IsoSurfVertT[i] = new List<Vector3>();
            IsoSurfNormT[i] = new List<Vector3>();
            IsoSurfColorT[i] = new List<Color>();
            IsoSurfIndexT[i] = new List<int>();

            trinum[i] = 0;
            numSurfVerts[i] = 0;
        }

        isosurf = new Mesh();
        isosurf.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = isosurf;
    }

    void IsRawdatamode()
    {
        if (coord.gn[0] <= voirConst.IS_GNISX &&
            coord.gn[1] <= voirConst.IS_GNISY && coord.gn[2] <= voirConst.IS_GNISZ)
        {
            rawdatamode = true;
        }
        else
        {
            rawdatamode = false;
        }
        gniso[0] = Math.Min(voirConst.IS_GNISX, coord.gn[0]);
        gniso[1] = Math.Min(voirConst.IS_GNISY, coord.gn[1]);
        gniso[2] = Math.Min(voirConst.IS_GNISZ, coord.gn[2]);
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

        ns = 0;
        GetComponent<MeshRenderer>().material = mat[ns];
        dn4iso = -2;
        gniso = new int[3];
        IsRawdatamode();

        if (param.single)
            scal4isof = new float[gniso[2], gniso[1], gniso[0]];
        else scal4iso = new double[gniso[2], gniso[1], gniso[0]];

        coordx = new float[gniso[0]];
        coordy = new float[gniso[1]];
        coordz = new float[gniso[2]];
        for (int i = 0; i < 8; i++) vertex[i] = i;
        isolevel = (data.voirScalData[0].maxvalue - data.voirScalData[0].minvalue) / 2.0 * 1.2 + data.voirScalData[0].minvalue;
        //isolevel = 0.0;
        //voirFunc.Log("isolevel : "+isolevel);
        connect[0, 0] = 0; connect[0, 1] = 1;
        connect[1, 0] = 0; connect[1, 1] = 2;
        connect[2, 0] = 0; connect[2, 1] = 3;
        connect[3, 0] = 1; connect[3, 1] = 2;
        connect[4, 0] = 1; connect[4, 1] = 3;
        connect[5, 0] = 2; connect[5, 1] = 3;

        IsoDataSet();
        IsoCoordSet();
        MarchingTetra();

        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        //        isosurf.SetColors(vcolor);
        isosurf.SetTriangles(IsoSurfIndex, 0);

        coloring = false;
        coldatn = 0;

        roicalc = false;
        wireframe = false;
    }

    public void IsosurfROI()
    {
        roicalc = true;
        ClearIsoSurf();

        IsRawdatamode();
        IsoDataSet();
        IsoCoordSet();
        MarchingTetra();
        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        if (coloring) isosurf.SetColors(IsoSurfColor);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        if (wireframe) isosurf.SetIndices(isosurf.GetIndices(0), MeshTopology.Lines, 0);
        roicalc = false;
        isosurf.RecalculateBounds();
    }

    public void SetIsoParams(int dn, bool wire)
    {
        SetDataNum(dn);
        wireframe = wire;
        coloring = false;
        GenIsoSurf();
        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        if (wire) isosurf.SetIndices(isosurf.GetIndices(0), MeshTopology.Lines, 0);
        isosurf.RecalculateBounds();
    }

    public void SetCIsoParams(int dn, bool wire, int cdn)
    {
        SetDataNumC(dn, cdn);
        wireframe = wire;
        coloring = true;
        GenIsoSurf();
        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        if (coloring) isosurf.SetColors(IsoSurfColor);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        if (wire) isosurf.SetIndices(isosurf.GetIndices(0), MeshTopology.Lines, 0);
        isosurf.RecalculateBounds();
    }

    void ReGenerateIsoSurf(float level)
    {
        if (coloring) SetDataNumC(ns, coldatn);
        else SetDataNum(ns);
        SetIsoLevel((double)level);
        GenIsoSurf();
        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        if (coloring) isosurf.SetColors(IsoSurfColor);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        if (wireframe) isosurf.SetIndices(isosurf.GetIndices(0), MeshTopology.Lines, 0);
        isosurf.RecalculateBounds();
        //isosurf.RecalculateNormals();
    }

    public void GenerateIsoSurf(int dn, float level, bool wire)
    {
        coloring = false;

        SetDataNum(dn);
        SetIsoLevel((double)level);
        GenIsoSurf();
        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        if (wire) isosurf.SetIndices(isosurf.GetIndices(0), MeshTopology.Lines, 0);
        wireframe = wire;
        isosurf.RecalculateBounds();
        //isosurf.RecalculateNormals();
    }

    public void GenerateIsoSurfC(int dn, int cdn, float level, bool wire)
    {
        coloring = true;

        SetDataNumC(dn, cdn);
        SetIsoLevel((double)level);
        GenIsoSurf();
        isosurf.SetVertices(IsoSurfVert);
        isosurf.SetNormals(IsoSurfNorm);
        if (coloring) isosurf.SetColors(IsoSurfColor);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        if (wire) isosurf.SetIndices(isosurf.GetIndices(0), MeshTopology.Lines, 0);
        wireframe = wire;
        isosurf.RecalculateBounds();
    }

    public void SetDataNum(int ison)
    {
        int nmat = ison % numisomaterial;
        GetComponent<MeshRenderer>().material = mat[nmat];
        ns = ison;
    }

    public void SetDataNumC(int ison, int coln)
    {
        //if (ison == coln)
        //{
        //    coloring = false;
        //    SetDataNum(ison);
        //    return;
        //}
        GetComponent<MeshRenderer>().material = mat[numisomaterial];
        ns = ison;
        coldatn = coln;
    }

    public void SetIsoLevel(double level)
    {
        isolevel = level;
    }

    public void GenIsoSurf()
    {
        IsoDataSet();
        IsoCoordSet();
        MarchingTetra();
    }

    public void ClearIsoSurf()
    {
        int maxt = voirConst.MAXTHREADS;
        for(int i=0;i<maxt;i++) numSurfVerts[i] = 0;
        IsoSurfIndex.Clear();
        isosurf.SetTriangles(IsoSurfIndex, 0);
        IsoSurfVert.Clear();
        IsoSurfNorm.Clear();
        IsoSurfColor.Clear();
        isosurf.SetNormals(IsoSurfNorm);
        //        isosurf.SetColors(vcolor);
        isosurf.SetTriangles(IsoSurfIndex, 0);
        for (int i=0;i<maxt;i++)
        {
            IsoSurfIndexT[i].Clear();
            IsoSurfVertT[i].Clear();
            IsoSurfNormT[i].Clear();
            IsoSurfColorT[i].Clear();
        }
    }

    void MarchingTetra()
    {
        int ni, nj, nk;
        ni = gniso[0]; nj = gniso[1]; nk = gniso[2];
        int maxt = voirConst.MAXTHREADS;
        int loopt;

        int[] st;
        int[] end;
        st = new int[maxt];
        end = new int[maxt];
        ClearIsoSurf();
        if (nk - 1 < maxt) { 
            loopt = nk - 1;
            for(int t=0;t<loopt;t++)
            {
                st[t] = t;
                end[t] = t + 1;
            }
            end[loopt - 1] = nk - 1;
        }
        else
        {
            loopt = maxt;
            int bl = (nk - 1) / maxt;
            for (int t = 0; t < loopt; t++)
            {
                st[t] = t*bl;
                end[t] = (t + 1)*bl;
            }
            end[loopt - 1] = nk - 1;
        }

        Parallel.For(0, loopt, t=>
        {
            for (int k = st[t]; k < end[t]; k++)
            {
                int ok = (k % 2 == 0) ? 1 : -1;
                for (int j = 0; j < nj - 1; j++)
                {
                    int oj = (j % 2 == 0) ? 1 : -1;
                    int ojk = oj * ok;
                    for (int i = 0; i < ni - 1; i++)
                    {
                        setVertVal(t, i, j, k);
                        if (!IsoSurfExistCube(t)) continue;
                        //voirFunc.Log("surfcell = " + i + " " +j + " " +k);
                        setVertPosNorm(t, i, j, k);

                        int oi = (i % 2 == 0) ? 1 : -1;
                        int type = oi * ojk;

                        if (type == 1) typeA(t);
                        else typeB(t);

                        for (int tn = 0; tn < 5; tn++)
                        {
                            setTetData(t, tn);
                            //if (!IsoSurfExistTet(t)) continue;
                            generateIsoSurf(t);
                        }

                    }
                }
            }
        });

        st = null;
        end = null;

        int totalv = 0;
        for (int t = 0; t < loopt; t++)
        {
            IsoSurfVert.AddRange(IsoSurfVertT[t]);
            IsoSurfVertT[t].Clear();

            IsoSurfNorm.AddRange(IsoSurfNormT[t]);
            IsoSurfNormT[t].Clear();

            if (coloring)
            {
                IsoSurfColor.AddRange(IsoSurfColorT[t]);
                IsoSurfColorT[t].Clear();
            }

            foreach (int index in IsoSurfIndexT[t])
            {
                int mindex = index + totalv;
                IsoSurfIndex.Add(mindex);
            }
            IsoSurfIndexT[t].Clear();
            totalv += numSurfVerts[t];

        }
    }

    bool findisovtx(int t, int v1, int v2){
        double dif1 = 0.0, dif2 = 0.0;
        if (tetval[t, v1] > isolevel && tetval[t, v2] < isolevel)
        {
            dif1 = tetval[t, v1] - isolevel;
            dif2 = isolevel - tetval[t, v2];
        }
        else if (tetval[t, v1] < isolevel && tetval[t, v2] > isolevel)
        {
            dif1 = isolevel - tetval[t, v1];
            dif2 = tetval[t, v2] - isolevel;
        } else { voirFunc.Error("Marching Cubes..."); }
        
        SetPosNorm(t, v1, v2, dif1, dif2);
        AddVtx(t);
            //voirFunc.Log("Num Triangle Verts (List) = " + IsoSurfVert.Count);
            //nv++;
        return true;
    }

    void SetPosNorm(int t, int v1, int v2, double dif1, double dif2){
            direc[t, 0] = tetpos[t, v2, 0] - tetpos[t, v1, 0];
            direc[t, 1] = tetpos[t, v2, 1] - tetpos[t, v1, 1];
            direc[t, 2] = tetpos[t, v2, 2] - tetpos[t, v1, 2];
            pos[t, 0] = (tetpos[t, v1, 0] + direc[t, 0] * dif1 / (dif1 + dif2));
            pos[t, 1] = (tetpos[t, v1, 1] + direc[t, 1] * dif1 / (dif1 + dif2));
            pos[t, 2] = (tetpos[t, v1, 2] + direc[t, 2] * dif1 / (dif1 + dif2));

            if (dif1 < 0.0)
            {
                pos[t, 0] = tetpos[t, v1, 0];
                pos[t, 1] = tetpos[t, v1, 1];
                pos[t, 2] = tetpos[t, v1, 2];
            } else if (dif1 / (dif1 + dif2) > 1.0) {
                pos[t, 0] = tetpos[t, v2, 0];
                pos[t, 1] = tetpos[t, v2, 1];
                pos[t, 2] = tetpos[t, v2, 2];
            }
            norm[t, 0] = (float)((tetnorm[t, v1, 0] * dif2 + tetnorm[t, v2, 0] * dif1));
            norm[t, 1] = (float)((tetnorm[t, v1, 1] * dif2 + tetnorm[t, v2, 1] * dif1));
            norm[t, 2] = (float)((tetnorm[t, v1, 2] * dif2 + tetnorm[t, v2, 2] * dif1));
    }

    void AddVtx(int t){
        voirFunc.Normalize(t, norm);
        IsoSurfVertT[t].Add(new Vector3((float)pos[t, 0], (float)pos[t, 2], (float)pos[t, 1]));
        IsoSurfNormT[t].Add(new Vector3(norm[t, 0], norm[t, 2], norm[t, 1]));

        if (coloring)
        {
            double sval = 0.0;
            Color vcolor = new Color(0.0f,0.0f,0.0f,1.0f);
            if (coldatn != ns) {
                data.getScalVal(t, coldatn, pos, out sval);//COLORING
                col.getScalColor(coldatn, sval, ref vcolor);
            } else
            {
                col.getScalColor(ns, isolevel, ref vcolor);
            }
//          IsoSurfColorT[t].Add(new Color(vcolor[0], vcolor[1], vcolor[2], 1.0f));
            vcolor[3] = 1.0f;
            IsoSurfColorT[t].Add(vcolor);
        }
        numSurfVerts[t]++;
    }

    void generateIsoSurf(int t)
    {
        int nv = 0;
        int v1, v2;

        int [] vtxtype = new int [4]{0,0,0,0};
        int mtil = 0, ltil = 0, eqil = 0;
        for(int i=0;i<4;i++)
        {
            if(tetval[t, i] > isolevel)
            {
                vtxtype[i] = 1;
                mtil++;
            }
            else if(tetval[t, i] < isolevel)
            {
                vtxtype[i] = -1;
                ltil++;
            }
            else
            {
                vtxtype[i] = 0;
                eqil++;
            }
        }

        if(mtil == 4 || ltil == 4 || eqil == 4){
            vtxtype = null;
            return;
        }

        if(eqil == 0){
            for (int i = 0; i < 6; i++) {
                v1 = connect[i, 0]; v2 = connect[i, 1];
                if(vtxtype[v1]*vtxtype[v2] != -1) continue;
                if(findisovtx(t, v1, v2)) nv++;
            }
        } else if(eqil == 1){
            int zv = 0;
            for(int j=0;j<4;j++){
                if(vtxtype[j] == 0){
                    zv = j;
                    for(int i=0;i<3;i++){
                        pos[t, i] = tetpos[t, zv, i];
                        norm[t, i] = (float)tetnorm[t, zv, i];
                    }
                    AddVtx(t);
                    nv++;
                    break;
                }
            }

            for(int i=0;i<6;i++){
                v1 = connect[i, 0]; v2 = connect[i, 1];
                if(v1 == zv || v2 == zv) continue;
                if(vtxtype[v1]*vtxtype[v2] != -1) continue;
                if(findisovtx(t, v1, v2)) nv++;
            }

        } else if(eqil == 2){
            int [] zv = new int [2];
            int nzv = 0;
            for(int j=0;j<4;j++){
                if(vtxtype[j] == 0){
                    zv[nzv] = j;
                    for(int i=0;i<3;i++){
                        pos[t, i] = tetpos[t, zv[nzv], i];
                        norm[t, i] = (float)tetnorm[t, zv[nzv], i];
                    }
                    nzv++;
                    AddVtx(t);
                    nv++;
                    if(nzv==2) break;
                }
            }
            
            for(int i=0;i<6;i++){
                v1 = connect[i, 0]; v2 = connect[i, 1];
                if(v1 == zv[0] || v2 == zv[0]) continue;
                if(v1 == zv[1] || v2 == zv[1]) continue;
                if(vtxtype[v1]*vtxtype[v2] != -1) continue;
                if(findisovtx(t, v1, v2)) nv++;
            }
            zv = null;
        } else if(eqil == 3){
            int [] zv = new int [3];
            int nzv = 0;
            for(int j=0;j<4;j++){
                if(vtxtype[j] == 0){
                    zv[nzv] = j;
                    for(int i=0;i<3;i++){
                        pos[t, i] = tetpos[t, zv[nzv], i];
                        norm[t, i] = (float)tetnorm[t, zv[nzv], i];
                    }
                    nzv++;
                    AddVtx(t);
                    nv++;
                    if(nzv==3) break;
                }
            }
            zv = null;
        } else {
            voirFunc.Error("marching tet: eqil is wrong -> "+eqil);
        }

        vtxtype = null;

        switch(nv){
            case 0:
                break;
            
            case 1:
                IsoSurfVertT[t].RemoveAt(numSurfVerts[t] - 1);
                IsoSurfNormT[t].RemoveAt(numSurfVerts[t] - 1);
                if (coloring)
                    IsoSurfColorT[t].RemoveAt(numSurfVerts[t] - 1);
                numSurfVerts[t]--;
                break;

            case 2:
                for(int i=0;i<2;i++){
                    IsoSurfVertT[t].RemoveAt(numSurfVerts[t] - 1);
                    IsoSurfNormT[t].RemoveAt(numSurfVerts[t] - 1);
                    if (coloring)
                        IsoSurfColorT[t].RemoveAt(numSurfVerts[t] - 1);
                    numSurfVerts[t]--;
                }
                break;

            case 3:// 1 triangle
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 3);
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 2);
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 1);
                break;

            case 4: // 2 triangles
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 4);
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 3);
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 2);

                IsoSurfIndexT[t].Add(numSurfVerts[t] - 3);
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 2);
                IsoSurfIndexT[t].Add(numSurfVerts[t] - 1);
                break;

            default:
                voirFunc.Log("nv = " + nv);
                break;

        }

    }

/*
    void generateIsoSurf(int t)
    {
        int nv = 0;
        double dif1 = 0.0, dif2 = 0.0;
        int v1, v2;
        double sval;
        for (int i = 0; i < 6; i++) {
            v1 = connect[i, 0]; v2 = connect[i, 1];
            sval = tetval[t, v1];
            if ((tetval[t, v1] >= isolevel && tetval[t, v2] >= isolevel) || (tetval[t, v1] <= isolevel && tetval[t, v2] <= isolevel))
            {
                //doesn't exist
            }
            else
            {
                if (tetval[t, v1] >= isolevel && tetval[t, v2] < isolevel)
                {
                    dif1 = tetval[t, v1] - isolevel;
                    dif2 = isolevel - tetval[t, v2];
                }
                else if (tetval[t, v1] < isolevel && tetval[t, v2] >= isolevel)
                {
                    dif1 = isolevel - tetval[t, v1];
                    dif2 = tetval[t, v2] - isolevel;
                } else { voirFunc.Error("Marching Cubes..."); }
                direc[t, 0] = tetpos[t, v2, 0] - tetpos[t, v1, 0];
                direc[t, 1] = tetpos[t, v2, 1] - tetpos[t, v1, 1];
                direc[t, 2] = tetpos[t, v2, 2] - tetpos[t, v1, 2];
                pos[t, 0] = (tetpos[t, v1, 0] + direc[t, 0] * dif1 / (dif1 + dif2));
                pos[t, 1] = (tetpos[t, v1, 1] + direc[t, 1] * dif1 / (dif1 + dif2));
                pos[t, 2] = (tetpos[t, v1, 2] + direc[t, 2] * dif1 / (dif1 + dif2));

                if (dif1 < 0.0)
                {
                    pos[t, 0] = tetpos[t, v1, 0];
                    pos[t, 1] = tetpos[t, v1, 1];
                    pos[t, 2] = tetpos[t, v1, 2];
                    sval = tetval[t, v1];
                } else if (dif1 / (dif1 + dif2) > 1.0) {
                    pos[t, 0] = tetpos[t, v2, 0];
                    pos[t, 1] = tetpos[t, v2, 1];
                    pos[t, 2] = tetpos[t, v2, 2];
                    sval = tetval[t, v2];
                }
                norm[t, 0] = (float)((tetnorm[t, v1, 0] * dif2 + tetnorm[t, v2, 0] * dif1));
                norm[t, 1] = (float)((tetnorm[t, v1, 1] * dif2 + tetnorm[t, v2, 1] * dif1));
                norm[t, 2] = (float)((tetnorm[t, v1, 2] * dif2 + tetnorm[t, v2, 2] * dif1));
                voirFunc.Normalize(t, norm);
                IsoSurfVertT[t].Add(new Vector3((float)pos[t, 0], (float)pos[t, 2], (float)pos[t, 1]));
                IsoSurfNormT[t].Add(new Vector3(norm[t, 0], norm[t, 2], norm[t, 1]));
                if (coloring)
                {
                    Color vcolor = new Color(0.0f,0.0f,0.0f,1.0f);
                    if (coldatn != ns) {
                        data.getScalVal(t, coldatn, pos, out sval);//COLORING
                        col.getScalColor(coldatn, sval, ref vcolor);
                    } else
                    {
                        col.getScalColor(ns, isolevel, ref vcolor);
                    }
//                    IsoSurfColorT[t].Add(new Color(vcolor[0], vcolor[1], vcolor[2], 1.0f));
                    vcolor[3] = 1.0f;
                    IsoSurfColorT[t].Add(vcolor);
                }
                //voirFunc.Log("Num Triangle Verts (List) = " + IsoSurfVert.Count);
                nv++;
                numSurfVerts[t]++;
            }
        }

        if (nv == 3)
        { // 1 triangle
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 3);
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 2);
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 1);
        }
        else if (nv == 4)
        { // 2 triangles
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 4);
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 3);
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 2);

            IsoSurfIndexT[t].Add(numSurfVerts[t] - 3);
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 2);
            IsoSurfIndexT[t].Add(numSurfVerts[t] - 1);
        } else voirFunc.Log("nv = " + nv);

    }
*/
    bool IsoSurfExistTet(int t) {
        bool exist = true;
        if (tetval[t, 0] > isolevel && tetval[t, 1] > isolevel && tetval[t, 2] > isolevel && tetval[t, 3] > isolevel) exist = false;
        else if (tetval[t, 0] < isolevel && tetval[t, 1] < isolevel && tetval[t, 2] < isolevel && tetval[t, 3] < isolevel) exist = false;
        else if (tetval[t, 0] == isolevel && tetval[t, 1] == isolevel && tetval[t, 2] == isolevel && tetval[t, 3] == isolevel) exist = false;

        return exist;
    }

    void setTetData(int t, int n) // n:tetnumber 0~4
    {
        for (int i = 0; i < 4; i++) // i: vertex of tetra 0~3
        {
            //voirFunc.Log("n="+n+" i="+i+" tet[n,i]="+tet[n, i]);
            tetpos[t, i, 0] = vertpos[t, tet[t, n, i], 0];
            tetpos[t, i, 1] = vertpos[t, tet[t, n, i], 1];
            tetpos[t, i, 2] = vertpos[t, tet[t, n, i], 2];
            tetnorm[t, i, 0] = vertnorm[t, tet[t, n, i], 0];
            tetnorm[t, i, 1] = vertnorm[t, tet[t, n, i], 1];
            tetnorm[t, i, 2] = vertnorm[t, tet[t, n, i], 2];
            tetval[t, i] = vertval[t, tet[t, n, i]];
        }
    }

    void setVertPosNorm(int t, int i, int j, int k)
    {
        vertpos[t, 0, 0] = coordx[i]; vertpos[t, 0, 1] = coordy[j]; vertpos[t, 0, 2] = coordz[k];
        vertpos[t, 1, 0] = coordx[i + 1]; vertpos[t, 1, 1] = coordy[j]; vertpos[t, 1, 2] = coordz[k];
        vertpos[t, 2, 0] = coordx[i]; vertpos[t, 2, 1] = coordy[j + 1]; vertpos[t, 2, 2] = coordz[k];
        vertpos[t, 3, 0] = coordx[i + 1]; vertpos[t, 3, 1] = coordy[j + 1]; vertpos[t, 3, 2] = coordz[k];
        vertpos[t, 4, 0] = coordx[i]; vertpos[t, 4, 1] = coordy[j]; vertpos[t, 4, 2] = coordz[k + 1];
        vertpos[t, 5, 0] = coordx[i + 1]; vertpos[t, 5, 1] = coordy[j]; vertpos[t, 5, 2] = coordz[k + 1];
        vertpos[t, 6, 0] = coordx[i]; vertpos[t, 6, 1] = coordy[j + 1]; vertpos[t, 6, 2] = coordz[k + 1];
        vertpos[t, 7, 0] = coordx[i + 1]; vertpos[t, 7, 1] = coordy[j + 1]; vertpos[t, 7, 2] = coordz[k + 1];

        if (param.single)
        {
            if (!rawdatamode)
            {
                getScalNormf(t, i, j, k, vertnorm, 0); getScalNormf(t, i + 1, j, k, vertnorm, 1); getScalNormf(t, i, j + 1, k, vertnorm, 2); getScalNormf(t, i + 1, j + 1, k, vertnorm, 3);
                getScalNormf(t, i, j, k + 1, vertnorm, 4); getScalNormf(t, i + 1, j, k + 1, vertnorm, 5); getScalNormf(t, i, j + 1, k + 1, vertnorm, 6); getScalNormf(t, i + 1, j + 1, k + 1, vertnorm, 7);
            }
            else
            {
                getScalNormRawf(t, i, j, k, vertnorm, 0); getScalNormRawf(t, i + 1, j, k, vertnorm, 1); getScalNormRawf(t, i, j + 1, k, vertnorm, 2); getScalNormRawf(t, i + 1, j + 1, k, vertnorm, 3);
                getScalNormRawf(t, i, j, k + 1, vertnorm, 4); getScalNormRawf(t, i + 1, j, k + 1, vertnorm, 5); getScalNormRawf(t, i, j + 1, k + 1, vertnorm, 6); getScalNormRawf(t, i + 1, j + 1, k + 1, vertnorm, 7);
            }
        }
        else
        {
            if (!rawdatamode)
            {
                getScalNormd(t, i, j, k, vertnorm, 0); getScalNormd(t, i + 1, j, k, vertnorm, 1); getScalNormd(t, i, j + 1, k, vertnorm, 2); getScalNormd(t, i + 1, j + 1, k, vertnorm, 3);
                getScalNormd(t, i, j, k + 1, vertnorm, 4); getScalNormd(t, i + 1, j, k + 1, vertnorm, 5); getScalNormd(t, i, j + 1, k + 1, vertnorm, 6); getScalNormd(t, i + 1, j + 1, k + 1, vertnorm, 7);
            }
            else
            {
                getScalNormRawd(t, i, j, k, vertnorm, 0); getScalNormRawd(t, i + 1, j, k, vertnorm, 1); getScalNormRawd(t, i, j + 1, k, vertnorm, 2); getScalNormRawd(t, i + 1, j + 1, k, vertnorm, 3);
                getScalNormRawd(t, i, j, k + 1, vertnorm, 4); getScalNormRawd(t, i + 1, j, k + 1, vertnorm, 5); getScalNormRawd(t, i, j + 1, k + 1, vertnorm, 6); getScalNormRawd(t, i + 1, j + 1, k + 1, vertnorm, 7);

            }
        }
    }

    bool IsoSurfExistCube(int t) {
        bool exist = true;
        if (vertval[t, 0] > isolevel && vertval[t, 1] > isolevel && vertval[t, 2] > isolevel && vertval[t, 3] > isolevel &&
            vertval[t, 4] > isolevel && vertval[t, 5] > isolevel && vertval[t, 6] > isolevel && vertval[t, 7] > isolevel) exist = false;
        else if (vertval[t, 0] < isolevel && vertval[t, 1] < isolevel && vertval[t, 2] < isolevel && vertval[t, 3] < isolevel &&
            vertval[t, 4] < isolevel && vertval[t, 5] < isolevel && vertval[t, 6] < isolevel && vertval[t, 7] < isolevel) exist = false;
        else if (vertval[t, 0] == isolevel && vertval[t, 1] == isolevel && vertval[t, 2] == isolevel && vertval[t, 3] == isolevel &&
            vertval[t, 4] == isolevel && vertval[t, 5] == isolevel && vertval[t, 6] == isolevel && vertval[t, 7] == isolevel) exist = false;

        return exist;
    }

    void setVertVal(int t, int i, int j, int k)
    {
        if (param.single) {
            vertval[t, 0] = scal4isof[k, j, i]; vertval[t, 1] = scal4isof[k, j, i + 1];
            vertval[t, 2] = scal4isof[k, j + 1, i]; vertval[t, 3] = scal4isof[k, j + 1, i + 1];
            vertval[t, 4] = scal4isof[k + 1, j, i]; vertval[t, 5] = scal4isof[k + 1, j, i + 1];
            vertval[t, 6] = scal4isof[k + 1, j + 1, i]; vertval[t, 7] = scal4isof[k + 1, j + 1, i + 1];
        }
        else
        {
            vertval[t, 0] = scal4iso[k, j, i]; vertval[t, 1] = scal4iso[k, j, i + 1];
            vertval[t, 2] = scal4iso[k, j + 1, i]; vertval[t, 3] = scal4iso[k, j + 1, i + 1];
            vertval[t, 4] = scal4iso[k + 1, j, i]; vertval[t, 5] = scal4iso[k + 1, j, i + 1];
            vertval[t, 6] = scal4iso[k + 1, j + 1, i]; vertval[t, 7] = scal4iso[k + 1, j + 1, i + 1];
        }
    }

    void typeA(int t) //vertex[8] tet[5,4]
    {
        tet[t, 0, 0] = vertex[4]; tet[t, 0, 1] = vertex[0]; tet[t, 0, 2] = vertex[1]; tet[t, 0, 3] = vertex[2];
        tet[t, 1, 0] = vertex[6]; tet[t, 1, 1] = vertex[4]; tet[t, 1, 2] = vertex[7]; tet[t, 1, 3] = vertex[2];
        tet[t, 2, 0] = vertex[2]; tet[t, 2, 1] = vertex[1]; tet[t, 2, 2] = vertex[7]; tet[t, 2, 3] = vertex[4];
        tet[t, 3, 0] = vertex[5]; tet[t, 3, 1] = vertex[4]; tet[t, 3, 2] = vertex[1]; tet[t, 3, 3] = vertex[7];
        tet[t, 4, 0] = vertex[7]; tet[t, 4, 1] = vertex[2]; tet[t, 4, 2] = vertex[1]; tet[t, 4, 3] = vertex[3];
    }

    void typeB(int t)
    {
        tet[t, 0, 0] = vertex[4]; tet[t, 0, 1] = vertex[0]; tet[t, 0, 2] = vertex[5]; tet[t, 0, 3] = vertex[6];
        tet[t, 1, 0] = vertex[6]; tet[t, 1, 1] = vertex[0]; tet[t, 1, 2] = vertex[3]; tet[t, 1, 3] = vertex[2];
        tet[t, 2, 0] = vertex[5]; tet[t, 2, 1] = vertex[0]; tet[t, 2, 2] = vertex[3]; tet[t, 2, 3] = vertex[6];
        tet[t, 3, 0] = vertex[5]; tet[t, 3, 1] = vertex[0]; tet[t, 3, 2] = vertex[1]; tet[t, 3, 3] = vertex[3];
        tet[t, 4, 0] = vertex[7]; tet[t, 4, 1] = vertex[6]; tet[t, 4, 2] = vertex[5]; tet[t, 4, 3] = vertex[3];
    }

    void getScalNormd(int t, int nx, int ny, int nz, float[,,] vect, int vn)
    {
        double dsx, dsy, dsz;

        double dx = (coordx[gniso[0] - 1] - coordx[0]) / (gniso[0] - 1) * 0.99;
        double dy = (coordy[gniso[1] - 1] - coordy[0]) / (gniso[1] - 1) * 0.99;
        double dz = (coordz[gniso[2] - 1] - coordz[0]) / (gniso[2] - 1) * 0.99;

        if (nx == 0)
        {
            dsx = scal4iso[nz, ny, 1] - scal4iso[nz, ny, 0];
        }
        else if (nx == gniso[0] - 1)
        {
            dsx = scal4iso[nz, ny, gniso[0] - 1] - scal4iso[nz, ny, gniso[0] - 2];
        }
        else
        {
            dsx = scal4iso[nz, ny, nx + 1] - scal4iso[nz, ny, nx - 1];
            dx = dx * 2.0;
        }
        vect[t, vn, 0] = -(float)(dsx / dx);

        if (ny == 0)
        {
            dsy = scal4iso[nz, 1, nx] - scal4iso[nz, 0, nx];
        }
        else if (ny == gniso[1] - 1)
        {
            dsy = scal4iso[nz, gniso[1] - 1, nx] - scal4iso[nz, gniso[1] - 2, nx];
        }
        else
        {
            dsy = scal4iso[nz, ny + 1, nx] - scal4iso[nz, ny - 1, nx];
            dy = dy * 2.0;
        }
        vect[t, vn, 1] = -(float)(dsy / dy);

        if (nz == 0)
        {
            dsz = scal4iso[1, ny, nx] - scal4iso[0, ny, nx];
        }
        else if (nz == gniso[2] - 1)
        {
            dsz = scal4iso[gniso[2] - 1, ny, nx] - scal4iso[gniso[2] - 2, ny, nx];
        }
        else
        {
            dsz = scal4iso[nz + 1, ny, nx] - scal4iso[nz - 1, ny, nx];
            dz = dz * 2.0;
        }
        vect[t, vn, 2] = -(float)(dsz / dz);

    }

    void getScalNormf(int t, int nx, int ny, int nz, float[,,] vect, int vn)
    {
        float dsx, dsy, dsz;

        float dx = (coordx[gniso[0] - 1] - coordx[0]) / (gniso[0] - 1) * 0.99f;
        float dy = (coordy[gniso[1] - 1] - coordy[0]) / (gniso[1] - 1) * 0.99f;
        float dz = (coordz[gniso[2] - 1] - coordz[0]) / (gniso[2] - 1) * 0.99f;

        if (nx == 0)
        {
            dsx = scal4isof[nz, ny, 1] - scal4isof[nz, ny, 0];
        }
        else if (nx == gniso[0] - 1)
        {
            dsx = scal4isof[nz, ny, gniso[0] - 1] - scal4isof[nz, ny, gniso[0] - 2];
        }
        else
        {
            dsx = scal4isof[nz, ny, nx + 1] - scal4isof[nz, ny, nx - 1];
            dx = dx * 2.0f;
        }
        vect[t, vn, 0] = -dsx / dx;

        if (ny == 0)
        {
            dsy = scal4isof[nz, 1, nx] - scal4isof[nz, 0, nx];
        }
        else if (ny == gniso[1] - 1)
        {
            dsy = scal4isof[nz, gniso[1] - 1, nx] - scal4isof[nz, gniso[1] - 2, nx];
        }
        else
        {
            dsy = scal4isof[nz, ny + 1, nx] - scal4isof[nz, ny - 1, nx];
            dy = dy * 2.0f;
        }
        vect[t, vn, 1] = -dsy / dy;

        if (nz == 0)
        {
            dsz = scal4isof[1, ny, nx] - scal4isof[0, ny, nx];
        }
        else if (nz == gniso[2] - 1)
        {
            dsz = scal4isof[gniso[2] - 1, ny, nx] - scal4isof[gniso[2] - 2, ny, nx];
        }
        else
        {
            dsz = scal4isof[nz + 1, ny, nx] - scal4isof[nz - 1, ny, nx];
            dz = dz * 2.0f;
        }
        vect[t, vn, 2] = -dsz / dz;

    }


    void getScalNormRawd(int t, int nx, int ny, int nz, float[,,] vect, int vn)
    {
        double dx, dy, dz, dsx, dsy, dsz;
        //		bool uniform = true;
        // uniform = coord.uniform;

        if (nx == 0)
        {
            dsx = scal4iso[nz, ny, 1] - scal4iso[nz, ny, 0];
            dx = coordx[1] - coordx[0];
        }
        else if (nx == gniso[0] - 1)
        {
            dsx = scal4iso[nz, ny, gniso[0] - 1] - scal4iso[nz, ny, gniso[0] - 2];
            dx = coordx[gniso[0] - 1] - coordx[gniso[0] - 2];
        }
        else
        {
            dsx = scal4iso[nz, ny, nx + 1] - scal4iso[nz, ny, nx - 1];
            dx = coordx[nx + 1] - coordx[nx - 1];
        }
        vect[t, vn, 0] = -(float)(dsx / dx);

        if (ny == 0)
        {
            dsy = scal4iso[nz, 1, nx] - scal4iso[nz, 0, nx];
            dy = coordy[1] - coordy[0];
        }
        else if (ny == gniso[1] - 1)
        {
            dsy = scal4iso[nz, gniso[1] - 1, nx] - scal4iso[nz, gniso[1] - 2, nx];
            dy = coordy[gniso[1] - 1] - coordy[gniso[1] - 2];
        }
        else
        {
            dsy = scal4iso[nz, ny + 1, nx] - scal4iso[nz, ny - 1, nx];
            dy = coordy[ny + 1] - coordy[ny - 1];
        }
        vect[t, vn, 1] = -(float)(dsy / dy);

        if (nz == 0)
        {
            dsz = scal4iso[1, ny, nx] - scal4iso[0, ny, nx];
            dz = coordz[1] - coordz[0];
        }
        else if (nz == gniso[2] - 1)
        {
            dsz = scal4iso[gniso[2] - 1, ny, nx] - scal4iso[gniso[2] - 2, ny, nx];
            dz = coordz[gniso[2] - 1] - coordz[gniso[2] - 2];
        }
        else
        {
            dsz = scal4iso[nz + 1, ny, nx] - scal4iso[nz - 1, ny, nx];
            dz = coordz[nz + 1] - coordz[nz - 1];
        }
        vect[t, vn, 2] = -(float)(dsz / dz);

    }

    void getScalNormRawf(int t, int nx, int ny, int nz, float[,,] vect, int vn)
    {
        float dx, dy, dz, dsx, dsy, dsz;
        //		bool uniform = true;
        // uniform = coord.uniform;

        if (nx == 0)
        {
            dsx = scal4isof[nz, ny, 1] - scal4isof[nz, ny, 0];
            dx = coordx[1] - coordx[0];
        }
        else if (nx == gniso[0] - 1)
        {
            dsx = scal4isof[nz, ny, gniso[0] - 1] - scal4isof[nz, ny, gniso[0] - 2];
            dx = coordx[gniso[0] - 1] - coordx[gniso[0] - 2];
        }
        else
        {
            dsx = scal4isof[nz, ny, nx + 1] - scal4isof[nz, ny, nx - 1];
            dx = coordx[nx + 1] - coordx[nx - 1];
        }
        vect[t, vn, 0] = -dsx / dx;

        if (ny == 0)
        {
            dsy = scal4isof[nz, 1, nx] - scal4isof[nz, 0, nx];
            dy = coordy[1] - coordy[0];
        }
        else if (ny == gniso[1] - 1)
        {
            dsy = scal4isof[nz, gniso[1] - 1, nx] - scal4isof[nz, gniso[1] - 2, nx];
            dy = coordy[gniso[1] - 1] - coordy[gniso[1] - 2];
        }
        else
        {
            dsy = scal4isof[nz, ny + 1, nx] - scal4isof[nz, ny - 1, nx];
            dy = coordy[ny + 1] - coordy[ny - 1];
        }
        vect[t, vn, 1] = -dsy / dy;

        if (nz == 0)
        {
            dsz = scal4isof[1, ny, nx] - scal4isof[0, ny, nx];
            dz = coordz[1] - coordz[0];
        }
        else if (nz == gniso[2] - 1)
        {
            dsz = scal4isof[gniso[2] - 1, ny, nx] - scal4isof[gniso[2] - 2, ny, nx];
            dz = coordz[gniso[2] - 1] - coordz[gniso[2] - 2];
        }
        else
        {
            dsz = scal4isof[nz + 1, ny, nx] - scal4isof[nz - 1, ny, nx];
            dz = coordz[nz + 1] - coordz[nz - 1];
        }
        vect[t, vn, 2] = -dsz / dz;

    }

    void IsoCoordSet()
    {
        double dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / (gniso[0] - 1) * 0.99;
        double dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / (gniso[1] - 1) * 0.99;
        double dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (gniso[2] - 1) * 0.99;
        double cx = coord.coordx[coord.minic[0]];
        double cy = coord.coordy[coord.minic[1]];
        double cz = coord.coordz[coord.minic[2]];
        if (rawdatamode)
        {
            for (int i = coord.minic[0]; i <= coord.maxic[0]; i++) coordx[i - coord.minic[0]] = (float)coord.coordx[i];
            for (int j = coord.minic[1]; j <= coord.maxic[1]; j++) coordy[j - coord.minic[1]] = (float)coord.coordy[j];
            for (int k = coord.minic[2]; k <= coord.maxic[2]; k++) coordz[k - coord.minic[2]] = (float)coord.coordz[k];

        }
        else
        {

            for (int i = 0; i < gniso[0]; i++) coordx[i] = (float)(cx + i * dx);
            for (int j = 0; j < gniso[1]; j++) coordy[j] = (float)(cy + j * dy);
            for (int k = 0; k < gniso[2]; k++) coordz[k] = (float)(cz + k * dz);
        }

        if (coord.minic[0] == 0) coordx[0] = (float)(cx + dx * 0.005);
        if (coord.minic[1] == 0) coordy[0] = (float)(cy + dy * 0.005);
        if (coord.minic[2] == 0) coordz[0] = (float)(cz + dz * 0.005);
        if (coord.maxic[0] == coord.gn_[0] - 1) coordx[gniso[0]-1] = (float)(coord.coordx[coord.maxic[0]] - dx * 0.005);
        if (coord.maxic[1] == coord.gn_[1] - 1) coordy[gniso[1]-1] = (float)(coord.coordy[coord.maxic[1]] - dy * 0.005);
        if (coord.maxic[2] == coord.gn_[2] - 1) coordz[gniso[2]-1] = (float)(coord.coordz[coord.maxic[2]] - dz * 0.005);
    }

    void IsoDataSet()
    {
        //int ii, jj, kk;
        if (ns < 0) voirFunc.Error("IsoDataSet: ns is negative");
        if (ns == dn4iso && roicalc == false) return;
        if (rawdatamode)
        {
            if (param.single)
            {
                //for (int k = coord.minic[2]; k <= coord.maxic[2]; k++)
                Parallel.For(coord.minic[2], coord.maxic[2]+1, k =>
                {
                    for (int j = coord.minic[1]; j <= coord.maxic[1]; j++)
                    {
                        for (int i = coord.minic[0]; i <= coord.maxic[0]; i++)
                        {
                            int ii = i - coord.minic[0];
                            int jj = j - coord.minic[1];
                            int kk = k - coord.minic[2];
                            scal4isof[kk, jj, ii] = data.getScalValf(ns, k, j, i);
                            //Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
                        }
                    }
                });
            }

            else
            {
                //for (int k = coord.minic[2]; k <= coord.maxic[2]; k++)
                Parallel.For(coord.minic[2], coord.maxic[2]+1, k =>
                {
                    for (int j = coord.minic[1]; j <= coord.maxic[1]; j++)
                    {
                        for (int i = coord.minic[0]; i <= coord.maxic[0]; i++)
                        {
                            int ii = i - coord.minic[0];
                            int jj = j - coord.minic[1];
                            int kk = k - coord.minic[2];
                            scal4iso[kk, jj, ii] = data.getScalVald(ns, k, j, i);
                            //Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
                        }
                    }
                });
            }
        }
        else
        {
            double dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / (gniso[0] - 1) * 0.99;
            double dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / (gniso[1] - 1) * 0.99;
            double dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (gniso[2] - 1) * 0.99;
            double cx = coord.coordx[coord.minic[0]] + dx * 0.005;
            double cy = coord.coordy[coord.minic[1]] + dy * 0.005;
            double cz = coord.coordz[coord.minic[2]] + dz * 0.005;
            //double val = 0.0;
            //float valf = 0.0f;
            //double[] pos = new double[3];
            int maxt = voirConst.MAXTHREADS;
            int loopt;
            int[] st;
            int[] end;
            st = new int[maxt];
            end = new int[maxt];

            if (gniso[2] < maxt)
            {
                loopt = gniso[2];
                for (int t = 0; t < loopt; t++)
                {
                    st[t] = t;
                    end[t] = t + 1;
                }
                end[loopt - 1] = gniso[2];
            }
            else
            {
                loopt = maxt;
                int bl = gniso[2] / maxt;
                for (int t = 0; t < loopt; t++)
                {
                    st[t] = t * bl;
                    end[t] = (t + 1) * bl;
                }
                end[loopt - 1] = gniso[2];
            }


            double[,] pos = new double[loopt, 3];
            if (param.single)
            {
                //for (int k = 0; k<gniso[2]; k++)
                Parallel.For(0, loopt, t=>
                {
                    float valf = 0.0f;
                    for (int k = st[t]; k < end[t]; k++)
                    {
                        pos[t, 2] = cz + k * dz;
                        for (int j = 0; j < gniso[1]; j++)
                        {
                            pos[t, 1] = cy + j * dy;
                            for (int i = 0; i < gniso[0]; i++)
                            {
                                pos[t, 0] = cx + i * dx;
                                if (data.getScalValf(t, ns, pos, out valf))
                                {
                                    scal4isof[k, j, i] = valf;
                                } else
                                {
                                    scal4isof[k, j, i] = 0.0f;
                                    voirFunc.Log("isodata: "+i+" "+j+" "+k+" set zero");
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                voirFunc.Log("isosurf data recalc");
                //for (int t = 0; t < loopt; t++)
                Parallel.For(0, loopt, t=>
                {
                    //voirFunc.Log("st:"+st[t]+" - end:"+end[t]);
                    double val = 0.0;
                    for (int k = st[t]; k < end[t]; k++)
                    {
                        pos[t,2] = cz + k * dz;
                        for (int j = 0; j < gniso[1]; j++)
                        {
                            pos[t,1] = cy + j * dy;
                            for (int i = 0; i < gniso[0]; i++)
                            {
                                pos[t,0] = cx + i * dx;
                                if (data.getScalVal(t, ns, pos, out val))
                                {
                                    scal4iso[k, j, i] = val;
                                }
                                else
                                {
                                    scal4iso[k, j, i] = 0.0;
                                    voirFunc.Log("isodata: " + i + " " + j + " " + k + " set zero");
                                }
                            }
                        }
                    }
                });
            }
            pos = null;
        }
        dn4iso = ns;
    }

    void setscal4iso(int t, int ns, double [,] pos, int i, int j, int k)
    {
        double val;
        if (data.getScalVal(t, ns, pos, out val))
        {
            scal4iso[k, j, i] = val;
        }

    }

    void IsoDataSet_old()
    {
        int ii, jj, kk;
        if (ns < 0) voirFunc.Error("IsoDataSet: ns is negative");
        if (ns == dn4iso && roicalc == false) return;
        if (rawdatamode)
        {
            if (param.single)
            {
                for (int k = coord.minic[2]; k <= coord.maxic[2]; k++)
                {
                    for (int j = coord.minic[1]; j <= coord.maxic[1]; j++)
                    {
                        for (int i = coord.minic[0]; i <= coord.maxic[0]; i++)
                        {
                            ii = i - coord.minic[0];
                            jj = j - coord.minic[1];
                            kk = k - coord.minic[2];
                            scal4isof[kk, jj, ii] = data.getScalValf(ns, k, j, i);
                            //Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
                        }
                    }
                }
            }

            else
            {
                for (int k = coord.minic[2]; k <= coord.maxic[2]; k++)
                {
                    for (int j = coord.minic[1]; j <= coord.maxic[1]; j++)
                    {
                        for (int i = coord.minic[0]; i <= coord.maxic[0]; i++)
                        {
                            ii = i - coord.minic[0];
                            jj = j - coord.minic[1];
                            kk = k - coord.minic[2];
                            scal4iso[kk, jj, ii] = data.getScalVald(ns,k,j,i);
                        //Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
                        }
                    }
                }
            }
        }
		else
		{
    	    double dx = (coord.coordx[coord.maxic[0]] - coord.coordx[coord.minic[0]]) / (gniso[0] - 1) * 0.99;
            double dy = (coord.coordy[coord.maxic[1]] - coord.coordy[coord.minic[1]]) / (gniso[1] - 1) * 0.99;
            double dz = (coord.coordz[coord.maxic[2]] - coord.coordz[coord.minic[2]]) / (gniso[2] - 1) * 0.99;
            double cx = coord.coordx[coord.minic[0]] + dx * 0.005;
            double cy = coord.coordy[coord.minic[1]] + dy * 0.005;
            double cz = coord.coordz[coord.minic[2]] + dz * 0.005;
            double val = 0.0;
            float valf = 0.0f;
            double[] pos = new double[3];

            if(param.single)
            {
                for (int k = 0; k<gniso[2]; k++)
                {
                    pos[2] = cz + k* dz;
                    for (int j = 0; j<gniso[1]; j++)
                    {
                        pos[1] = cy + j* dy;
                        for (int i = 0; i<gniso[0]; i++)
                        {
                            pos[0] = cx + i* dx;
                            if (data.getScalValf(ns, pos, out valf))
                            {
                                scal4isof[k, j, i] = valf;
                            //Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
                            }
                        }
                    }
                }
            }
            else
            {
                voirFunc.Log("isosurf data recalc");
                for (int k = 0; k < gniso[2]; k++)
                {
                    pos[2] = cz + k * dz;
                    for (int j = 0; j < gniso[1]; j++)
                    {
                        pos[1] = cy + j * dy;
                        for (int i = 0; i < gniso[0]; i++)
                        {
                            pos[0] = cx + i * dx;
                            if (data.getScalVal(ns, pos, out val))
                            {
                                scal4iso[k, j, i] = val;
                                //Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
                            }
                        }
                    }
                }
            }
			pos = null;
		}
        dn4iso = ns;
    }

}
