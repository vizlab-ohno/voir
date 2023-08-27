//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using voirCommon;

public class voirStlSurfObj : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh sobj;
    int numtri, numvtx;
    int[] sindex;
    Vector3[] vertex;
    Vector3[] normal;
    byte[] bufx;
    byte[] bufy;
    byte[] bufz;
    float [,] vertex_;
    float[] pos;
    float[] npos;
    voirParam param;
    voirCoord coord;

    // Start is called before the first frame update
    void Start()
    {
        //return;
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        Init();
        if (!param.sobj) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Init()
    {
        numtri = 0;
        numvtx = 0;
        if (!param.sobj) return;
        AllocRead();
        ScaleVertex();
        SetMesh();
    }

    public void SetMesh()
    {
        if (!param.sobj) return;
        sobj.SetVertices(vertex);
        sobj.SetNormals(normal);
        sobj.SetTriangles(sindex, 0);
    }

    public void ScaleVertex()
    {
        if (!param.sobj) return;
        for (int i = 0; i < numvtx; i++)
        {
            pos[0] = vertex_[i, 0];
            pos[1] = vertex_[i, 1];
            pos[2] = vertex_[i, 2];
            
            coord.MapPos4Obj(pos, npos);
            
            vertex[i].x = npos[0];
            vertex[i].z = npos[1];
            vertex[i].y = npos[2];
        }        
    }

    void AllocRead()
    {
        pos = new float[3];
        npos = new float[3];
        sobj = new Mesh();
        sobj.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = sobj;

        voirFunc.Log(param.surfacefile);
        FileStream fs = new FileStream(param.surfacefile, FileMode.Open, FileAccess.Read);
        long size;
        var buf = new byte[80];
        fs.Read(buf, 0, 80);

        size = fs.Read(buf, 0, 4);
        if (size != 4)
        {
            voirFunc.Error("read failed. number of trianbles");
        }
        numtri = (int)BitConverter.ToUInt32(buf, 0);
        numvtx = numtri * 3;
        buf = null;

        buf = new byte[numtri*(sizeof(float)*12+2)]; // normal(=3) 3xVertex(=9) + 2bytes
        voirFunc.Log("Num of Triangles = "+numtri+", Num of Vertex = "+numvtx);

        size = fs.Read(buf, 0, numtri*(12 * sizeof(float)+2));
        if (size != numtri * (12 * sizeof(float) + 2))
        {
            voirFunc.Log("size = "+size + " != "+ (numtri * (12 * sizeof(float) + 2)+"(filesize)"));
            voirFunc.Error("read failed. surface");
        }

        fs.Dispose();

        vertex = new Vector3[numvtx];
        normal = new Vector3[numvtx];
        vertex_ = new float [numvtx, 3];
        sindex = new int[numtri * 3];
        for (int i = 0; i < numvtx; i++)
        {
            sindex[i] = i;
            vertex[i] = new Vector3(0,0,0);
            normal[i] = new Vector3(0,0,0);
        }

        int v = 0;
        int tridatsize = 50;
        for (int j = 0; j < numtri; j++)
        {
            v = j * 3;
            normal[v].x = BitConverter.ToSingle(buf, tridatsize * j);
            normal[v].z = BitConverter.ToSingle(buf, tridatsize * j+4);
            normal[v].y = BitConverter.ToSingle(buf, tridatsize * j+8);

            normal[v+1].x = normal[v].x;
            normal[v+1].z = normal[v].z;
            normal[v+1].y = normal[v].y;

            normal[v + 2].x = normal[v].x;
            normal[v + 2].z = normal[v].z;
            normal[v + 2].y = normal[v].y;

            vertex_[v, 0] = BitConverter.ToSingle(buf, tridatsize * j + 12);
            vertex_[v, 1] = BitConverter.ToSingle(buf, tridatsize * j + 16);
            vertex_[v, 2] = BitConverter.ToSingle(buf, tridatsize * j + 20);

            vertex_[v+1, 0] = BitConverter.ToSingle(buf, tridatsize * j + 24);
            vertex_[v+1, 1] = BitConverter.ToSingle(buf, tridatsize * j + 28);
            vertex_[v+1, 2] = BitConverter.ToSingle(buf, tridatsize * j + 32);

            vertex_[v + 2, 0] = BitConverter.ToSingle(buf, tridatsize * j + 36);
            vertex_[v + 2, 1] = BitConverter.ToSingle(buf, tridatsize * j + 40);
            vertex_[v + 2, 2] = BitConverter.ToSingle(buf, tridatsize * j + 44);

        }

        buf = null;
    }
}
