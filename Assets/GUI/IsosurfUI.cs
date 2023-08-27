//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class IsosurfUI : MonoBehaviour
{
    MeshFilter MeshFilter;
    Mesh gui;

    const int nball = 4;
    const int ndisc = 13;//odd
    const float dt = 2.0f*Mathf.PI / nball;
    [SerializeField] GameObject pfball;
    GameObject [] ballObj;
    GameObject[] discObj;
    voirColor col;
    Color color;

    const int N = 10;
    int sn;
    Vector3[] verts;
    Vector3[] norms;
    int[] index;

    voirData data;
    Vector3 center;
    float minv, maxv, absmaxv;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        Alloc();
        gameObject.SetActive(false);
    }

    void Init()
    {
        col = GameObject.Find("Color").GetComponent<voirColor>();
        data = GameObject.Find("Data").GetComponent<voirData>();
        sn = -1;
    }

    void Alloc()
    {
        color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        ballObj = new GameObject[nball];
        for (int i = 0; i < nball; i++)
        {
            ballObj[i] = Instantiate(pfball, Vector3.zero, Quaternion.identity);
            ballObj[i].transform.SetParent(gameObject.transform);
            ballObj[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ballObj[i].transform.localPosition = Vector3.zero;
        }

        discObj = new GameObject[ndisc];
        for (int i = 0; i < ndisc; i++)
        {
            discObj[i] = Instantiate(pfball, Vector3.zero, Quaternion.identity);
            discObj[i].transform.SetParent(gameObject.transform);
            discObj[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            discObj[i].transform.localPosition = Vector3.zero;
        }

        verts = new Vector3[4 * N];
        norms = new Vector3[4 * N];
        for(int i=0;i<N;i++){
            verts[i] = new Vector3(0,0,0);
            norms[i] = new Vector3(0,0,0);
        }
        center = Vector3.zero;
        index = new int[2 * N * 6]; 

        gui = new Mesh();
        gui.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = gui;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initGUI(Vector3 c, int s)
    {
        center.x = c.x;
        center.y = c.y;
        center.z = c.z;

        sn = s;

        maxv = (float)data.voirScalData[sn].maxvalue;
        minv = (float)data.voirScalData[sn].minvalue;
        absmaxv = Mathf.Abs(maxv);
        if (absmaxv < Mathf.Abs(minv)) absmaxv = Mathf.Abs(minv);

        float mheight; // middle height (ratio)
        float trad, mrad, brad; // ratio
        float tnxz, tny, bnxz, bny;
        if (maxv >= 0 && minv >= 0)
        {
            mheight = 0;
            trad = 1;
            mrad = minv / absmaxv;
            brad = 0;
            tny = -(trad-mrad)* voirConst.IS_GUI_RAD; tnxz = voirConst.IS_GUI_Z;
            float abs = Mathf.Sqrt(tny*tny+tnxz*tnxz);
            tny /= abs; tnxz /= abs;

            bny = tny; bnxz = tnxz;
        }
        else if(maxv <= 0 && minv <= 0)
        {
            mheight = 1;
            trad = 0;
            mrad = maxv / absmaxv;
            brad = 1;
            bny = (brad - mrad) * voirConst.IS_GUI_RAD; bnxz = voirConst.IS_GUI_Z;
            float abs = Mathf.Sqrt(bny * bny + bnxz * bnxz);
            bny /= abs; bnxz /= abs;

            tny = bny; tnxz = bnxz;
        }
        else
        {
            mheight = Mathf.Abs(minv) / (maxv - minv);
            trad = maxv / absmaxv;
            mrad = 0;
            brad = minv / absmaxv;
            tny = -trad * voirConst.IS_GUI_RAD; tnxz = (1.0f - mheight)*voirConst.IS_GUI_Z;
            float abs = Mathf.Sqrt(tny * tny + tnxz * tnxz);
            tny /= abs; tnxz /= abs;

            bny = brad * voirConst.IS_GUI_RAD; bnxz = mheight* voirConst.IS_GUI_Z;
            abs = Mathf.Sqrt(bny * bny + bnxz * bnxz);
            bny /= abs; bnxz /= abs;
        }

        // Drwing Cones
        int n = 0;
        float cy;
        // top
        cy = center.y + voirConst.IS_GUI_Z / 2.0f;
        for (int i = 0; i < N; i++)
        {
            float cx = center.x + voirConst.IS_GUI_RAD * trad * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float cz = center.z + voirConst.IS_GUI_RAD * trad * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            verts[n].x = cx;
            verts[n].y = cy;
            verts[n].z = cz;

            float nx = tnxz * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float nz = tnxz * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            norms[n].x = nx;
            norms[n].y = nz;
            norms[n].z = tny;
            n++;
        }

        // Middle x 2
        cy = center.y - voirConst.IS_GUI_Z / 2.0f + voirConst.IS_GUI_Z * mheight;
        for (int i = 0; i < N; i++)
        {
            float cx = center.x + voirConst.IS_GUI_RAD * mrad * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float cz = center.z + voirConst.IS_GUI_RAD * mrad * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            verts[n].x = cx;
            verts[n].y = cy;
            verts[n].z = cz;

            float nx = tnxz * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float nz = tnxz * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            norms[n].x = nx;
            norms[n].y = nz;
            norms[n].z = tny;
            n++;
        }

        for (int i = 0; i < N; i++)
        {
            float cx = center.x + voirConst.IS_GUI_RAD * mrad * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float cz = center.z + voirConst.IS_GUI_RAD * mrad * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            verts[n].x = cx;
            verts[n].y = cy;
            verts[n].z = cz;

            float nx = bnxz * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float nz = bnxz * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            norms[n].x = nx;
            norms[n].y = nz;
            norms[n].z = bny;
            n++;
        }

        // Bottom
        cy = center.y - voirConst.IS_GUI_Z / 2.0f;
        for (int i = 0; i < N; i++)
        {
            float cx = center.x + voirConst.IS_GUI_RAD * brad * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float cz = center.z + voirConst.IS_GUI_RAD * brad * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            verts[n].x = cx;
            verts[n].y = cy;
            verts[n].z = cz;

            float nx = bnxz * Mathf.Cos(Mathf.PI * 2.0f / N * i);
            float nz = bnxz * Mathf.Sin(Mathf.PI * 2.0f / N * i);
            norms[n].x = nx;
            norms[n].y = nz;
            norms[n].z = bny;
            n++;
        }

        //TOP
        int vn = 0;
        for(int i=0;i<N-1;i++)
        {
            index[vn] = i;
            index[vn+1] = i + 1;
            index[vn+2] = i + N;

            index[vn+3] = i+1;
            index[vn + 4] = i + 1 + N;
            index[vn + 5] = i + N;
            vn += 6;
        }

        index[6*N-6] = N-1;
        index[6 * N - 5] = 0;
        index[6 * N - 4] = 2*N-1;

        index[6 * N - 3] = 0;
        index[6 * N - 2] = 2*N-1;
        index[6 * N - 1] = N;

        //BOTTOM
        vn = 6*N;
        for (int i = 0; i < N - 1; i++)
        {
            index[vn] = i+2*N;
            index[vn + 1] = i + 1 + 2 * N;
            index[vn + 2] = i + N + 2 * N;

            index[vn + 3] = i + 1 + 2 * N;
            index[vn + 4] = i + 1 + N + 2 * N;
            index[vn + 5] = i + N + 2 * N;
            vn += 6;
        }

        index[12 * N - 6] = N - 1 + 2 * N;
        index[12 * N - 5] = 0 + 2 * N;
        index[12 * N - 4] = 2*N-1 + 2 * N;

        index[12 * N - 3] = 0 + 2 * N;
        index[12 * N - 2] = 2 * N - 1 + 2 * N;
        index[12 * N - 1] = N + 2 * N;


        //for (int i = 0; i < 4 * N; i++)
        //    voirFunc.Log("x:"+verts[i].x+ " y:" + verts[i].y+ " z:" + verts[i].z);

        gui.SetVertices(verts);
        gui.SetNormals(norms);
        gui.SetTriangles(index, 0);
        gui.RecalculateBounds();
        gui.RecalculateNormals();

        int avi = (ndisc - 1) / 2;
        for (int i = 0; i < ndisc; i++)
        {
            float val = (float)(data.voirScalData[s].average + (-avi+(double)i)*data.voirScalData[s].stdev);
            float height = (val - minv) / (maxv - minv);
            float rad = Mathf.Abs(val) / absmaxv;
            float discy = center.y - voirConst.IS_GUI_Z / 2.0f + height * voirConst.IS_GUI_Z;
            float discx = center.x;
            float discz = center.z;
            float drad = (voirConst.IS_GUI_RAD + 0.05f) * rad*1.5f;
            float dthick = 0.01f;
            if (val > maxv || val < minv)
            {
                discObj[i].SetActive(false);
            }
            else
            {
                discObj[i].SetActive(true);
            }
            discObj[i].GetComponent<Transform>().localPosition = new Vector3(discx, discy, discz);
            discObj[i].GetComponent<Transform>().localScale = new Vector3(drad, dthick, drad);
            Material mat = discObj[i].GetComponent<Renderer>().material;
            if (avi == i)
            {
                mat.color = Color.red;
            }
            else if (i == avi - 1 || i == avi + 1)
            {
                mat.color = Color.yellow;
            }
            else if (i == avi - 2 || i == avi + 2)
            {
                mat.color = Color.green;
            }
            else
            {
                mat.color = Color.blue;
            }


        }

        voirFunc.Log("Init IsoSurf GUI");
    }

    public void reset()
    {
        sn = -1;
    }

    public void drawGUI(int sn, float val)
    {
        float height = (val - minv) / (maxv - minv);
        bool positive;
        if (val >= 0) positive = true;
        else positive = false;
        float rad = Mathf.Abs(val) / absmaxv;

        float time = Time.time;
        if (!positive) time = -time;
        for (int i=0;i<nball;i++)
        {
            float bally = center.y - voirConst.IS_GUI_Z / 2.0f + height * voirConst.IS_GUI_Z;
            float ballx = center.x + (voirConst.IS_GUI_RAD + 0.05f)* rad * Mathf.Cos(time * Mathf.PI * 2.0f + dt*i);
            float ballz = center.z + (voirConst.IS_GUI_RAD + 0.05f) * rad * Mathf.Sin(time * Mathf.PI * 2.0f + dt * i);
            ballObj[i].GetComponent<Transform>().localPosition = new Vector3(ballx, bally, ballz);
            col.getScalColor(sn, (double)val, ref color);
            Material mat = ballObj[i].GetComponent<Renderer>().material;
            mat.color = color;
        }

    }

}
