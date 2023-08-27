//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class MainControll : MonoBehaviour
{
    tracking trackd;
    GameObject Menu;
    Menu menu;
    GameObject Panels;
    GameObject Visualization;

    Vector3 hpos;
    Vector3 hforward;
    Vector3 ppos;
    Vector3 hup;
    Vector3 hright;
    int first;
    //
    public VizMethods SelectedMethod;
    //GameObject
    GameObject StreamLines;
    GameObject LocalArrows;
    GameObject Snow;
    GameObject FlashLight;
    GameObject IsoSurface;
    GameObject IsoSurface2;
    GameObject OrthoSlicerXY;
    GameObject OrthoSlicerXZ;
    GameObject OrthoSlicerYZ;
    GameObject OrthoSlicerXY2;
    GameObject OrthoSlicerXZ2;
    GameObject OrthoSlicerYZ2;
    GameObject LocalSlicer;
    GameObject SurfaceObj;
    GameObject CurveObj;
    GameObject ROI;
    GameObject SnapShot;
    // GUI
    GameObject ROIUI;
    //Script
    voirStreamLines streamlines;
    voirArrows larrows;
    voirSnow snow;
    voirFlashLight flight;
    voirLocalSlicer lslicer;
    voirIsoSurface isosurf;
    voirIsoSurface2 isosurf2;
    voirOrthoSlicerXY slicerxy;
    voirOrthoSlicerXY2 slicerxy2;
    voirOrthoSlicerYZ sliceryz;
    voirOrthoSlicerYZ2 sliceryz2;
    voirOrthoSlicerXZ slicerxz;
    voirOrthoSlicerXZ2 slicerxz2;

    public int[] SelectedVal;

    void Awake()
    {
        SelectedMethod = VizMethods.None;
        first = 1;
        Init();
        Alloc();
    }

    // Start is called before the first frame update
    void Start()
    {
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        Menu = GameObject.Find("Menu");
        Visualization = GameObject.Find("Visualization");
        menu = GameObject.Find("Menu").GetComponent<Menu>();
        Panels = GameObject.Find("Panels");
        hpos = new Vector3(0,0,0);
        hforward = new Vector3(0, 0, 0);
        ppos = new Vector3(0, 0, 0);
        hup = new Vector3(0, 0, 0);
        hright = new Vector3(0, 0, 0);

        TurnOffUI();
    }

    // Update is called once per frame
    void ToggleMenu()
    {
        if (Menu.activeSelf)
        {
            Panels.transform.localPosition = Vector3.zero;
            Menu.SetActive(false);
            Visualization.SetActive(true);
        }
        else
        {
            SelectedMethod = VizMethods.None;
            TurnOffUI();
            //SnapShot.SetActive(false);
            trackd.getHeadData(0, ref hpos);
            trackd.getHeadData(1, ref hforward);
            trackd.getHeadData(2, ref hup);
            trackd.getHeadData(3, ref hright);
            ppos.x = hpos.x + voirConst.MN_DIST * hforward.x + 0.5f * hright.x - 0.5f * hup.x;
            ppos.y = hpos.y + voirConst.MN_DIST * hforward.y + 0.5f * hright.y - 0.5f * hup.y;
            ppos.z = hpos.z + voirConst.MN_DIST * hforward.z + 0.5f * hright.z - 0.5f * hup.z;
            Visualization.SetActive(false);
            Menu.SetActive(true);
            hup.x = 0;
            hup.y = 1;
            hup.z = 0;
            hforward.y = 0;
            Quaternion prot = Quaternion.LookRotation(hforward, hup);
            Panels.transform.rotation = prot;
            Panels.transform.localPosition = ppos;
            menu.ActivateMenu();
        }
            
    }

    void Update()
    {
        if (first == 1)
        {
            Menu.SetActive(false);
            first = 0;
        }
        //Debug.Log(trackd.getLTrigChange());
        if (trackd.getLTrigChange()==1)
        {
            ToggleMenu();
        }

       
    }

    public void SetNone()
    {
        SelectedMethod = VizMethods.None;
    }

    void TurnOffUI()
    {
        SnapShot.SetActive(false);
        ROIUI.SetActive(false);
        ROI.SetActive(false);
    }

    public void VizSelected()
    {
        VizMethods method = VizMethods.None;
        ToggleMenu();

        if (SelectedVal[0] == 0)
        {
            //Scalar
            switch (SelectedVal[2])
            {
                case 0:
                    method = VizMethods.IsoSurface1;
                    break;

                case 1:
                    method = VizMethods.IsoSurface2;
                    break;

                case 2:
                    switch (SelectedVal[3])
                    {
                        case 0:
                            method = VizMethods.OrthoSlicerXY1;
                            break;
                        case 1:
                            method = VizMethods.OrthoSlicerYZ1;
                            break;
                        case 2:
                            method = VizMethods.OrthoSlicerXZ1;
                            break;
                    }
                    break;

                case 3:
                    switch (SelectedVal[3])
                    {
                        case 0:
                            method = VizMethods.OrthoSlicerXY2;
                            break;
                        case 1:
                            method = VizMethods.OrthoSlicerYZ2;
                            break;
                        case 2:
                            method = VizMethods.OrthoSlicerXZ2;
                            break;
                    }
                    break;

                case 4:
                    method = VizMethods.LocalSlicer;
                    break;

            }
        }
        else if (SelectedVal[0] == 1)
        {
            //Vector
            switch (SelectedVal[2])
            {
                case 0:
                    method = VizMethods.StreamLines;
                    break;

                case 1:
                    method = VizMethods.LocalArrows;
                    break;

                case 2:
                    method = VizMethods.FlashLight;
                    break;

                case 3:
                    method = VizMethods.Snow;
                    break;

            }
        }
        else if (SelectedVal[0] == 2)
        {
            //Region of Interest
            method = VizMethods.ROI;
        }
        else if (SelectedVal[0] == 3)
        {
            //Obj
            SelectedMethod = VizMethods.Obj;
            ROIUI.SetActive(false);
            ROI.SetActive(false);
            switch(SelectedVal[1])
            {
                case 0:
                    //Curve
                    if (CurveObj.activeSelf)
                        CurveObj.SetActive(false);
                    else
                        CurveObj.SetActive(true);
                    break;
                case 1:
                    //Surface
                    if (SurfaceObj.activeSelf)
                        SurfaceObj.SetActive(false);
                    else
                        SurfaceObj.SetActive(true);
                    break;
            }
            return;

        }
        else if (SelectedVal[0] == 4)
        {
            //Snap Shot
            SelectedMethod = VizMethods.SnapShot;
            SnapShot.SetActive(true);
            ROIUI.SetActive(false);
            ROI.SetActive(false);
            return;
        }

        ActivateUI(method);
    }

    void ActivateUI(VizMethods method)
    {
        //Isosurf
        bool wire = false;
        bool carpet = false;

        TurnOffUI();
        //        voirFunc.Log("selected:" + vizselect.value);
        //method = FindSelectedViz();
        switch (method)
        {
            case VizMethods.None:
                SelectedMethod = VizMethods.None;
                break;

            case VizMethods.IsoSurface1:
                SelectedMethod = VizMethods.IsoSurface1;
                IsoSurface.SetActive(true);
                if (SelectedVal[3] == 1) wire = true; // surface or wire
                if (SelectedVal[4] == 0)// color or mono
                {
                    isosurf.SetCIsoParams(SelectedVal[1], wire, SelectedVal[5]);
                }
                else
                {
                    isosurf.SetIsoParams(SelectedVal[1], wire);
                }
                break;

            case VizMethods.IsoSurface2:
                SelectedMethod = VizMethods.IsoSurface2;
                IsoSurface2.SetActive(true);
                if (SelectedVal[3] == 1) wire = true; // surface or wire
                if (SelectedVal[4] == 0)// color or mono
                {
                    isosurf2.SetCIsoParams(SelectedVal[1], wire, SelectedVal[5]);
                }
                else
                {
                    isosurf2.SetIsoParams(SelectedVal[1], wire);
                }
                break;

            case VizMethods.OrthoSlicerXY1:
                SelectedMethod = VizMethods.OrthoSlicerXY1;
                OrthoSlicerXY.SetActive(true);
                if (SelectedVal[4] == 1) carpet = true;
                if (SelectedVal[5] == 1) wire = true;
                slicerxy.SetParams(SelectedVal[1], carpet, wire);
                break;

            case VizMethods.OrthoSlicerXZ1:
                SelectedMethod = VizMethods.OrthoSlicerXZ1;
                OrthoSlicerXZ.SetActive(true);
                if (SelectedVal[4] == 1) carpet = true;
                if (SelectedVal[5] == 1) wire = true;
                slicerxz.SetParams(SelectedVal[1], carpet, wire);
                break;

            case VizMethods.OrthoSlicerYZ1:
                SelectedMethod = VizMethods.OrthoSlicerYZ1;
                OrthoSlicerYZ.SetActive(true);
                if (SelectedVal[4] == 1) carpet = true;
                if (SelectedVal[5] == 1) wire = true;
                sliceryz.SetParams(SelectedVal[1], carpet, wire);
                break;

            case VizMethods.OrthoSlicerXY2:
                SelectedMethod = VizMethods.OrthoSlicerXY2;
                OrthoSlicerXY2.SetActive(true);
                if (SelectedVal[4] == 1) carpet = true;
                if (SelectedVal[5] == 1) wire = true;
                slicerxy2.SetParams(SelectedVal[1], carpet, wire);
                break;

            case VizMethods.OrthoSlicerXZ2:
                SelectedMethod = VizMethods.OrthoSlicerXZ2;
                OrthoSlicerXZ2.SetActive(true);
                if (SelectedVal[4] == 1) carpet = true;
                if (SelectedVal[5] == 1) wire = true;
                slicerxz2.SetParams(SelectedVal[1], carpet, wire);
                break;

            case VizMethods.OrthoSlicerYZ2:
                SelectedMethod = VizMethods.OrthoSlicerYZ2;
                OrthoSlicerYZ2.SetActive(true);
                if (SelectedVal[4] == 1) carpet = true;
                if (SelectedVal[5] == 1) wire = true;
                sliceryz2.SetParams(SelectedVal[1], carpet, wire);
                break;

            case VizMethods.LocalSlicer:
                SelectedMethod = VizMethods.LocalSlicer;
                LocalSlicer.SetActive(true);
                lslicer.SetScalNum(SelectedVal[1]);
                break;

            case VizMethods.StreamLines:
                SelectedMethod = VizMethods.StreamLines;
                StreamLines.SetActive(true);
                streamlines.SetVect(SelectedVal[1]);
                break;

            case VizMethods.LocalArrows:
                SelectedMethod = VizMethods.LocalArrows;
                LocalArrows.SetActive(true);
                //larrows.Toggle(SelectedVal[1]);
                larrows.SetVectNum(SelectedVal[1]);
                break;

            case VizMethods.Snow:
                SelectedMethod = VizMethods.Snow;
                Snow.SetActive(true);
                snow.ClearSnow();
                snow.SetVectNum(SelectedVal[1]);
                break;

            case VizMethods.FlashLight:
                SelectedMethod = VizMethods.FlashLight;
                FlashLight.SetActive(true);
                flight.SetVectNum(SelectedVal[1]);
                break;

            case VizMethods.ROI:
                SelectedMethod = VizMethods.ROI;
                ROIUI.SetActive(true);
                ROI.SetActive(true);
                break;
        }
    }

    void Alloc()
    {
        SelectedVal = new int[voirConst.MN_PANEL_MAXCOL];
        for (int i = 0; i < voirConst.MN_PANEL_MAXCOL; i++) SelectedVal[i] = -1;
    }

    void Init()
    {
        StreamLines = GameObject.Find("StreamLines");
        streamlines = StreamLines.GetComponent<voirStreamLines>();
        LocalArrows = GameObject.Find("LocalArrows");
        larrows = LocalArrows.GetComponent<voirArrows>();
        Snow = GameObject.Find("Snow");
        snow = Snow.GetComponent<voirSnow>();
        FlashLight = GameObject.Find("FlashLight");
        flight = FlashLight.GetComponent<voirFlashLight>();
        IsoSurface = GameObject.Find("IsoSurface");
        isosurf = IsoSurface.GetComponent<voirIsoSurface>();
        IsoSurface2 = GameObject.Find("IsoSurface2");
        isosurf2 = IsoSurface2.GetComponent<voirIsoSurface2>();
        OrthoSlicerXY = GameObject.Find("OrthoSlicerXY");
        OrthoSlicerXZ = GameObject.Find("OrthoSlicerXZ");
        OrthoSlicerYZ = GameObject.Find("OrthoSlicerYZ");
        OrthoSlicerXY2 = GameObject.Find("OrthoSlicerXY2");
        OrthoSlicerXZ2 = GameObject.Find("OrthoSlicerXZ2");
        OrthoSlicerYZ2 = GameObject.Find("OrthoSlicerYZ2");

        slicerxy = OrthoSlicerXY.GetComponent<voirOrthoSlicerXY>();
        slicerxy2 = OrthoSlicerXY2.GetComponent<voirOrthoSlicerXY2>();
        sliceryz = OrthoSlicerYZ.GetComponent<voirOrthoSlicerYZ>();
        sliceryz2 = OrthoSlicerYZ2.GetComponent<voirOrthoSlicerYZ2>();
        slicerxz = OrthoSlicerXZ.GetComponent<voirOrthoSlicerXZ>();
        slicerxz2 = OrthoSlicerXZ2.GetComponent<voirOrthoSlicerXZ2>();

        LocalSlicer = GameObject.Find("LocalSlicer");
        lslicer = LocalSlicer.GetComponent<voirLocalSlicer>();
        SurfaceObj = GameObject.Find("SurfaceObj");
        CurveObj = GameObject.Find("CurveObj");
        SnapShot = GameObject.Find("SnapShot");
        ROI = GameObject.Find("ROI");

        ROIUI = GameObject.Find("ROIUI");
    }
}
