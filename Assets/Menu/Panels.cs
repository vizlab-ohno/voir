//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using voirCommon;

public class Panels : MonoBehaviour
{
    // Params outside script
    int scalnum;
    int vectnum;
    string[] scallabel;
    string[] vectlabel;

    const int FirstPNUM = 6;
    const int ObjPNUM = 2;
    const int SVizPNUM = 5;
    const int VVizPNUM = 4;
    const int ISub1PNUM = 2;
    const int ISub2PNUM = 2;
    const int SSub1PNUM = 3;
    const int SSub2PNUM = 2;
    const int SSub3PNUM = 2;

    public GameObject pfplane;
    public GameObject pftext;

    public GameObject FirstParent;
    public GameObject[] FirstPanels;
    public GameObject[] FirstTexts;

    public GameObject ScalParent;
    public GameObject[] ScalPanels;
    public GameObject[] ScalTexts;

    public GameObject VectParent;
    public GameObject[] VectPanels;
    public GameObject[] VectTexts;

    public GameObject ObjParent;
    public GameObject[] ObjPanels;
    public GameObject[] ObjTexts;

    public GameObject SVizParent;
    public GameObject[] ScalVizPanels;
    public GameObject[] ScalVizTexts;

    public GameObject VVizParent;
    public GameObject[] VectVizPanels;
    public GameObject[] VectVizTexts;

    public GameObject ISub1Parent;
    public GameObject[] ISub1Panels;
    public GameObject[] ISub1Texts;

    public GameObject ISub2Parent;
    public GameObject[] ISub2Panels;
    public GameObject[] ISub2Texts;

    public GameObject CScalParent;
    public GameObject[] CScalPanels;
    public GameObject[] CScalTexts;

    public GameObject SSub1Parent;
    public GameObject[] SSub1Panels;
    public GameObject[] SSub1Texts;

    public GameObject SSub2Parent;
    public GameObject[] SSub2Panels;
    public GameObject[] SSub2Texts;

    public GameObject SSub3Parent;
    public GameObject[] SSub3Panels;
    public GameObject[] SSub3Texts;
    voirParam param;

    [SerializeField] Material white;
    // Start is called before the first frame update
    void Start()
    {
        Alloc();
        CreateFirstPanels();
        CreateScalPanels();
        CreateVectPanels();
        CreateObjPanels();
        CreateSVizPanels();
        CreateVVizPanels();
        CreateISub1Panels();
        CreateISub2Panels();
        CreateCScalPanels();
        CreateSSub1Panels();
        CreateSSub2Panels();
        CreateSSub3Panels();
    }

    public void ClearPanelColor()
    {
        if (FirstParent.activeInHierarchy)
        {
            for (int i = 0; i < FirstPNUM; i++)
            {
                FirstPanels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (ScalParent.activeInHierarchy)
        {
            for (int i = 0; i < scalnum; i++)
            {
                ScalPanels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (VectParent.activeInHierarchy)
        {
            for (int i = 0; i < vectnum; i++)
            {
                VectPanels[i].GetComponent<Renderer>().material = white;
            }
        }


        if (ObjParent.activeInHierarchy)
        {
            for (int i = 0; i < ObjPNUM; i++)
            {
                ObjPanels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (SVizParent)
        {
            for (int i = 0; i < SVizPNUM; i++)
            {
                ScalVizPanels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (VVizParent)
        {
            for (int i = 0; i < VVizPNUM; i++)
            {
                VectVizPanels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (ISub1Parent)
        {
            for (int i = 0; i < ISub1PNUM; i++)
            {
                ISub1Panels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (ISub2Parent)
        {
            for (int i = 0; i < ISub2PNUM; i++)
            {
                ISub2Panels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (CScalParent)
        {
            for (int i = 0; i < scalnum; i++)
            {
                CScalPanels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (SSub1Parent)
        {
            for (int i = 0; i < SSub1PNUM; i++)
            {
                SSub1Panels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (SSub2Parent)
        {
            for (int i = 0; i < SSub2PNUM; i++)
            {
                SSub2Panels[i].GetComponent<Renderer>().material = white;
            }
        }

        if (SSub3Parent)
        {
            for (int i = 0; i < SSub3PNUM; i++)
            {
                SSub3Panels[i].GetComponent<Renderer>().material = white;
            }
        }
    }

    void Alloc()
    {
        param = GameObject.Find("Param").GetComponent<voirParam>();
        scalnum = param.nscal;
        vectnum = param.nvect;

        //voirFunc.Log("nscal:"+scalnum+" nvect:"+vectnum);

        FirstPanels = new GameObject[FirstPNUM];
        FirstTexts = new GameObject[FirstPNUM];

        scallabel = new string[scalnum];
        for (int i = 0; i < scalnum; i++)
            scallabel[i] = param.scallabel[i];

        ScalPanels = new GameObject[scalnum];
        ScalTexts = new GameObject[scalnum];

        vectlabel = new string[vectnum];
        for (int i = 0; i < vectnum; i++)
            vectlabel[i] = param.vectlabel[i];

        VectPanels = new GameObject[vectnum];
        VectTexts = new GameObject[vectnum];

        ObjPanels = new GameObject[ObjPNUM];
        ObjTexts = new GameObject[ObjPNUM];

        ScalVizPanels = new GameObject[SVizPNUM];
        ScalVizTexts = new GameObject[SVizPNUM];

        VectVizPanels = new GameObject[VVizPNUM];
        VectVizTexts = new GameObject[VVizPNUM];

        ISub1Panels = new GameObject[ISub1PNUM];
        ISub1Texts = new GameObject[ISub1PNUM];

        ISub2Panels = new GameObject[ISub2PNUM];
        ISub2Texts = new GameObject[ISub2PNUM];

        CScalPanels = new GameObject[scalnum];
        CScalTexts = new GameObject[scalnum];

        SSub1Panels = new GameObject[SSub1PNUM];
        SSub1Texts = new GameObject[SSub1PNUM];

        SSub2Panels = new GameObject[SSub2PNUM];
        SSub2Texts = new GameObject[SSub2PNUM];

        SSub3Panels = new GameObject[SSub3PNUM];
        SSub3Texts = new GameObject[SSub3PNUM];
    }

    void CreateFirstPanels()
    {
        for (int i = 0; i < FirstPNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            FirstPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            FirstPanels[i].transform.SetParent(FirstParent.transform);
            FirstPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            FirstPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            FirstTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            FirstTexts[i].transform.SetParent(FirstParent.transform); //parent = parent.transform;
            FirstTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            FirstTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        FirstPanels[0].name = "Scalar";
        FirstTexts[0].name = "ScalarT";
        FirstTexts[0].GetComponent<TextMeshPro>().text = "Scalar";

        FirstPanels[1].name = "Vector";
        FirstTexts[1].name = "VectorT";
        FirstTexts[1].GetComponent<TextMeshPro>().text = "Vector";

        FirstPanels[2].name = "ROI";
        FirstTexts[2].name = "ROIT";
        FirstTexts[2].GetComponent<TextMeshPro>().text = "Extract ROI";

        FirstPanels[3].name = "Obj";
        FirstTexts[3].name = "ObjT";
        FirstTexts[3].GetComponent<TextMeshPro>().text = "Objs";

        FirstPanels[4].name = "Shot";
        FirstTexts[4].name = "ShotT";
        FirstTexts[4].GetComponent<TextMeshPro>().text = "Snap Shot";

        FirstPanels[5].name = "Quit";
        FirstTexts[5].name = "QuitT";
        FirstTexts[5].GetComponent<TextMeshPro>().text = "Quit";

        FirstParent.SetActive(false);
    }

    void CreateScalPanels()
    {
        for (int i = 0; i < scalnum; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            ScalPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            ScalPanels[i].transform.SetParent(ScalParent.transform);
            ScalPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ScalPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            ScalTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            ScalTexts[i].transform.SetParent(ScalParent.transform); //parent = parent.transform;
            ScalTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ScalTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);

            ScalPanels[i].name = "Scal" + i;
            ScalTexts[i].name = "ScalT" + i;
            ScalTexts[i].GetComponent<TextMeshPro>().text = scallabel[i];
        }

        ScalParent.SetActive(false);
    }

    void CreateVectPanels()
    {
        for (int i = 0; i < vectnum; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            VectPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            VectPanels[i].transform.SetParent(VectParent.transform);
            VectPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            VectPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            VectTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            VectTexts[i].transform.SetParent(VectParent.transform); //parent = parent.transform;
            VectTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            VectTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);

            VectPanels[i].name = "Vect" + i;
            VectTexts[i].name = "VectT" + i;
            VectTexts[i].GetComponent<TextMeshPro>().text = vectlabel[i];
        }

        VectParent.SetActive(false);
    }

    void CreateObjPanels()
    {

        for (int i = 0; i < ObjPNUM; i++)
        {
            float y = -0.35f * i;
            ObjPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            ObjPanels[i].transform.SetParent(ObjParent.transform);
            ObjPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ObjPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            ObjTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            ObjTexts[i].transform.SetParent(ObjParent.transform);
            ObjTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ObjTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        ObjPanels[0].name = "LineObj";
        ObjTexts[0].name = "LineObjT";
        ObjTexts[0].GetComponent<TextMeshPro>().text = "Line Object";

        ObjPanels[1].name = "SurfObj";
        ObjTexts[1].name = "SurfObjT";
        ObjTexts[1].GetComponent<TextMeshPro>().text = "Surface Object";

        ObjParent.SetActive(false);
    }

    void CreateSVizPanels()
    {
        for (int i = 0; i < SVizPNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            ScalVizPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            ScalVizPanels[i].transform.SetParent(SVizParent.transform);
            ScalVizPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ScalVizPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            ScalVizTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            ScalVizTexts[i].transform.SetParent(SVizParent.transform); //parent = parent.transform;
            ScalVizTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ScalVizTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        ScalVizPanels[0].name = "Iso1";
        ScalVizTexts[0].name = "Iso1T";
        ScalVizTexts[0].GetComponent<TextMeshPro>().text = "Isosurf1";

        ScalVizPanels[1].name = "Iso2";
        ScalVizTexts[1].name = "Iso2T";
        ScalVizTexts[1].GetComponent<TextMeshPro>().text = "Isosurf2";

        ScalVizPanels[2].name = "Slice1";
        ScalVizTexts[2].name = "Slice1T";
        ScalVizTexts[2].GetComponent<TextMeshPro>().text = "Slicer1";

        ScalVizPanels[3].name = "Slice2";
        ScalVizTexts[3].name = "Slice2T";
        ScalVizTexts[3].GetComponent<TextMeshPro>().text = "Slicer2";

        ScalVizPanels[4].name = "LSlice";
        ScalVizTexts[4].name = "LSliceT";
        ScalVizTexts[4].GetComponent<TextMeshPro>().text = "Local Slicer";

        SVizParent.SetActive(false);
    }

    void CreateVVizPanels()
    {
        for (int i = 0; i < VVizPNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            VectVizPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            VectVizPanels[i].transform.SetParent(VVizParent.transform);
            VectVizPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            VectVizPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            VectVizTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            VectVizTexts[i].transform.SetParent(VVizParent.transform); //parent = parent.transform;
            VectVizTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            VectVizTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        VectVizPanels[0].name = "SLines";
        VectVizTexts[0].name = "SlinesT";
        VectVizTexts[0].GetComponent<TextMeshPro>().text = "Field Lines";

        VectVizPanels[1].name = "LArrows";
        VectVizTexts[1].name = "LArrowsT";
        VectVizTexts[1].GetComponent<TextMeshPro>().text = "Local Arrows";

        VectVizPanels[2].name = "FLight";
        VectVizTexts[2].name = "FLightT";
        VectVizTexts[2].GetComponent<TextMeshPro>().text = "Flash Light";

        VectVizPanels[3].name = "Snow";
        VectVizTexts[3].name = "SnowT";
        VectVizTexts[3].GetComponent<TextMeshPro>().text = "Hotaru";

        VVizParent.SetActive(false);
    }

    void CreateISub1Panels()
    {

        for (int i = 0; i < ISub1PNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            ISub1Panels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            ISub1Panels[i].transform.SetParent(ISub1Parent.transform);
            ISub1Panels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ISub1Panels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            ISub1Texts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            ISub1Texts[i].transform.SetParent(ISub1Parent.transform);
            ISub1Texts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ISub1Texts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        ISub1Panels[0].name = "IsoS";
        ISub1Texts[0].name = "IsoST";
        ISub1Texts[0].GetComponent<TextMeshPro>().text = "Surface";

        ISub1Panels[1].name = "IsoW";
        ISub1Texts[1].name = "IsoWT";
        ISub1Texts[1].GetComponent<TextMeshPro>().text = "Wire Frame";

        ISub1Parent.SetActive(false);
    }

    void CreateISub2Panels()
    {

        for (int i = 0; i < ISub2PNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            ISub2Panels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            ISub2Panels[i].transform.SetParent(ISub2Parent.transform);
            ISub2Panels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ISub2Panels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            ISub2Texts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            ISub2Texts[i].transform.SetParent(ISub2Parent.transform);
            ISub2Texts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            ISub2Texts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        ISub2Panels[0].name = "IColor";
        ISub2Texts[0].name = "IColorT";
        ISub2Texts[0].GetComponent<TextMeshPro>().text = "Coloring";

        ISub2Panels[1].name = "IMono";
        ISub2Texts[1].name = "IMonoT";
        ISub2Texts[1].GetComponent<TextMeshPro>().text = "Mono";

        ISub2Parent.SetActive(false);
    }

    void CreateCScalPanels()
    {
        for (int i = 0; i < scalnum; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            CScalPanels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            CScalPanels[i].transform.SetParent(CScalParent.transform);
            CScalPanels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            CScalPanels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            CScalTexts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            CScalTexts[i].transform.SetParent(CScalParent.transform); //parent = parent.transform;
            CScalTexts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            CScalTexts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);

            CScalPanels[i].name = "CScal" + i;
            CScalTexts[i].name = "CScalT" + i;
            CScalTexts[i].GetComponent<TextMeshPro>().text = scallabel[i];
        }

        CScalParent.SetActive(false);

    }

    void CreateSSub1Panels()
    {

        for (int i = 0; i < SSub1PNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            SSub1Panels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            SSub1Panels[i].transform.SetParent(SSub1Parent.transform);
            SSub1Panels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            SSub1Panels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            SSub1Texts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            SSub1Texts[i].transform.SetParent(SSub1Parent.transform);
            SSub1Texts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            SSub1Texts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        SSub1Panels[0].name = "XY";
        SSub1Texts[0].name = "XYT";
        SSub1Texts[0].GetComponent<TextMeshPro>().text = "X-Y Plane";

        SSub1Panels[1].name = "YZ";
        SSub1Texts[1].name = "YZT";
        SSub1Texts[1].GetComponent<TextMeshPro>().text = "Y-Z Plane";

        SSub1Panels[2].name = "ZX";
        SSub1Texts[2].name = "ZXT";
        SSub1Texts[2].GetComponent<TextMeshPro>().text = "Z-X Plane";

        SSub1Parent.SetActive(false);
    }

    void CreateSSub2Panels()
    {

        for (int i = 0; i < SSub2PNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            SSub2Panels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            SSub2Panels[i].transform.SetParent(SSub2Parent.transform);
            SSub2Panels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            SSub2Panels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            SSub2Texts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            SSub2Texts[i].transform.SetParent(SSub2Parent.transform);
            SSub2Texts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            SSub2Texts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        SSub2Panels[0].name = "Plane";
        SSub2Texts[0].name = "PlaneT";
        SSub2Texts[0].GetComponent<TextMeshPro>().text = "Plane";

        SSub2Panels[1].name = "Carpet";
        SSub2Texts[1].name = "CarpedT";
        SSub2Texts[1].GetComponent<TextMeshPro>().text = "Carpet";

        SSub2Parent.SetActive(false);
    }

    void CreateSSub3Panels()
    {

        for (int i = 0; i < SSub3PNUM; i++)
        {
            float y = -voirConst.MN_PANEL_VSEP * i;
            SSub3Panels[i] = Instantiate(pfplane, Vector3.zero, Quaternion.identity);
            SSub3Panels[i].transform.SetParent(SSub3Parent.transform);
            SSub3Panels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            SSub3Panels[i].transform.localPosition = new Vector3(0.0f, y, 0.01f);

            SSub3Texts[i] = Instantiate(pftext, Vector3.zero, Quaternion.identity);
            SSub3Texts[i].transform.SetParent(SSub3Parent.transform);
            SSub3Texts[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            SSub3Texts[i].transform.localPosition = new Vector3(0.0f, y, 0.0f);
        }

        SSub3Panels[0].name = "SliceS";
        SSub3Texts[0].name = "SliceST";
        SSub3Texts[0].GetComponent<TextMeshPro>().text = "Surface";

        SSub3Panels[1].name = "SliceW";
        SSub3Texts[1].name = "SliceWT";
        SSub3Texts[1].GetComponent<TextMeshPro>().text = "Wire Frame";

        SSub3Parent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
