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

public class voirCurveObj : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh sobj;
    int numc, numvtx;
    int[] cnump;
    float [,,] vert_;
    byte[] bufx;
    byte[] bufy;
    byte[] bufz;
    LineRenderer[] drawline;
    GameObject[] Lines;
    voirParam param;
    voirCoord coord;
    [SerializeField] Material linemat;

        // Start is called before the first frame update
    void Start()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        Init();
        if (!param.cobj) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Init()
    {
        numc = 0;
        numvtx = 0;
//        voirFunc.Log("cobj = "+param.cobj);
        if (!param.cobj) return;
        Alloc();
        ReadVertex();
        ScaleVertex();
    }


    void ReadVertex()
    {
        voirFunc.Log("curve obj: ReadVertex");
        voirFunc.Log("curve obj: ReadVertex"+numvtx );
        bufx = new byte[numvtx * sizeof(float)];
        bufy = new byte[numvtx * sizeof(float)];
        bufz = new byte[numvtx * sizeof(float)];
        FileStream fsx = new FileStream(param.curvefile[1], FileMode.Open, FileAccess.Read);
        FileStream fsy = new FileStream(param.curvefile[2], FileMode.Open, FileAccess.Read);
        FileStream fsz = new FileStream(param.curvefile[3], FileMode.Open, FileAccess.Read);
        long size;
        var buf = new byte[4];
        if (param.skip4bytes) fsx.Read(buf, 0, 4);
        size = fsx.Read(bufx, 0, numvtx * sizeof(float));
        if (size != numvtx * sizeof(float))
        {
            voirFunc.Error("read failed. curve coord x");
        }
        //voirFunc.Log("bufx 0123 = "+bufx[0]+bufx[1]+bufx[2]+bufx[3]);

        if (param.skip4bytes) fsy.Read(buf, 0, 4);
        size = fsy.Read(bufy, 0, numvtx * sizeof(float));
        if (size != numvtx * sizeof(float))
        {
            voirFunc.Error("read failed. curve coord y");
        }

        if (param.skip4bytes) fsz.Read(buf, 0, 4);
        size = fsz.Read(bufz, 0, numvtx * sizeof(float));
        if (size != numvtx * sizeof(float))
        {
            voirFunc.Error("read failed. curve coord y");
        }
        fsx.Dispose();
        fsy.Dispose();
        fsz.Dispose();

        int count = 0;
        for (int j = 0; j < numc; j++)
        {
            drawline[j].positionCount = cnump[j];
            for (int i = 0; i < cnump[j]; i++)
            {
                vert_[j,i,0] = BitConverter.ToSingle(bufx, count * sizeof(float));
                vert_[j,i,1] = BitConverter.ToSingle(bufy, count * sizeof(float));
                vert_[j,i,2] = BitConverter.ToSingle(bufz, count * sizeof(float));
                count++;
            }
        }
        bufx = null;
        bufy = null;
        bufz = null;
        buf = null;
        voirFunc.Log("read curve ends");
    }

    public void ScaleVertex()
    {
        double cscale = coord.cscale;
        if (!param.cobj) return;
        for (int j = 0; j < numc; j++)
        {
            drawline[j].positionCount = cnump[j];
            Vector3 [] vert = new Vector3[cnump[j]];
            for (int i = 0; i < cnump[j]; i++)
            {
                vert[i] = new Vector3(0,0,0);
                vert[i][0] = (float)(vert_[j,i,0] * cscale + coord.shift[0] + (1.0 - cscale) * coord.coordx_[0]);
                vert[i][2] = (float)(vert_[j,i,1] * cscale + coord.shift[1] + (1.0 - cscale) * coord.coordy_[0]);
                vert[i][1] = (float)(vert_[j,i,2] * cscale + coord.shift[2] + (1.0 - cscale) * coord.coordz_[0]);
            }
            drawline[j].SetPositions(vert);
        }
        voirFunc.Log("curve obj: ScaleVertex ends");
    }

    void Alloc()
    {
        voirFunc.Log("curve obj: Alloc");
        FileStream fs = new FileStream(param.curvefile[0], FileMode.Open, FileAccess.Read);
        voirFunc.Log(param.curvefile[0]);
        long size;
        var buf = new byte[4];
        if (param.skip4bytes) fs.Read(buf, 0, 4);
        size = fs.Read(buf, 0, 1 * sizeof(int));
        if (size != 4)
        {
            voirFunc.Error("read failed. curve index 1");
        }
        numc = BitConverter.ToInt32(buf, 0);

        buf = null;
        buf = new byte[numc * sizeof(int)];
        voirFunc.Log("Num of Curves = " + numc);

        cnump = new int[numc];

        size = fs.Read(buf, 0, numc * sizeof(int));
        if (size !=  numc * sizeof(int))
        {
            voirFunc.Error("read failed. curve index 2");
        }

        numvtx = 0;
        int maxp = 0;
        for (int i = 0; i < numc; i++)
        {
            cnump[i] = BitConverter.ToInt32(buf, i * sizeof(int));
            numvtx += cnump[i];
            if (maxp < cnump[i]) maxp = cnump[i];
        }
        fs.Dispose();
        voirFunc.Log("maxp = "+maxp);
        voirFunc.Log("vertex = "+numvtx);
        vert_ = new float [numc, maxp, 3];

        drawline = new LineRenderer[numc];
        Lines = new GameObject[numc];

        buf = null;
        for (int i=0;i<numc;i++)
        {
            Lines[i] = new GameObject("courve v=" + i);
            Lines[i].transform.SetParent(transform);
            Lines[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            //drawline[i] = new LineRenderer();
            drawline[i] = Lines[i].AddComponent<LineRenderer>();
            drawline[i].useWorldSpace = false;
            drawline[i].material = linemat;//new Material(Shader.Find("Sprites/Default"));

            drawline[i].startWidth = 0.01f;
            drawline[i].endWidth = 0.01f;

            drawline[i].positionCount = 0;
            drawline[i].loop = false;

            drawline[i].startColor = Color.white;
            drawline[i].endColor = Color.white;
        }
    }
}
