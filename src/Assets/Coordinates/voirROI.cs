//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using voirCommon;

public class voirROI : MonoBehaviour
{
    int[] roimin;
    int[] roimax;
    int[] roign;
    int[] cmin;
    int[] cmax;

    double[] pos; // temp
    //For StreamLines
    double[,,] seeds;

    voirParam param;
    voirCoord coord;
    voirData data;

    GameObject IsoSurface;
    GameObject IsoSurface2;
    voirIsoSurface isosurf;
    voirIsoSurface2 isosurf2;

    GameObject OrthoSlicerXY;
    GameObject OrthoSlicerXZ;
    GameObject OrthoSlicerYZ;
    voirOrthoSlicerXY slicerxy;
    voirOrthoSlicerXZ slicerxz;
    voirOrthoSlicerYZ sliceryz;

    GameObject OrthoSlicerXY2;
    GameObject OrthoSlicerXZ2;
    GameObject OrthoSlicerYZ2;
    voirOrthoSlicerXY2 slicerxy2;
    voirOrthoSlicerXZ2 slicerxz2;
    voirOrthoSlicerYZ2 sliceryz2;

    GameObject LocalSlicer;
    voirLocalSlicer localslicer;

    GameObject StreamLines;
    voirStreamLines streamlines;

    GameObject FlashLight;
    voirFlashLight flashlight;

    GameObject LocalArrows;
    voirArrows localarrows;

    GameObject SurfObj;
    GameObject CurvObj;
    GameObject Frame;
    voirStlSurfObj surfobj;
    voirCurveObj curvobj;
    voirFrame frame;
    
    RoiUI roigui;
    GameObject ROIUI;

    tracking trackd;
    MainControll maincontroll;
    bool pressed;

    void Awake()
    {
        Init();
        Alloc();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //voirFunc.Log("ROI pressed "+pressed);
        if (maincontroll.SelectedMethod == VizMethods.ROI)
        {
            if (trackd.getRGripChange() == 1)
            {
                maincontroll.SetNone();
                ROIUI.SetActive(false);
                pressed = false;
                gameObject.SetActive(false);
            }

            
            switch (trackd.getRTrigChange())
            {
                case 1:
                    if(roigui.StartGUI())
                        pressed = true;
                    //voirFunc.Log("Pressed : ROI");
                    break;

                case 0:
                    if (pressed)
                    {
                        //voirFunc.Log("being pressed : ROI");
                        roigui.CalcGUI();
                    }
                    break;

                case -1:
                    if (pressed)
                    {
                        pressed = false;
                        //voirFunc.Log("Released : ROI");
                        if (roigui.EndGUI(ref cmin, ref cmax))
                        {
                            ROI(cmin, cmax);
                            maincontroll.SetNone();
                            ROIUI.SetActive(false);
                            gameObject.SetActive(false);
                        }
                    }
                    //voirFunc.Log("cmin"+cmin[0]+" "+cmin[1]+" "+cmin[2]
                    //    +" cmax " + cmax[0] + " " + cmax[1] + " " + cmax[2]);
                    break;
            }
        }
    }

    void Init()
    {
        pressed = false;

        roimin = new int[3];
        roimax = new int[3];
        roign = new int[3];

        cmin = new int[3];
        cmax = new int[3];
        pos = new double[3];

        param = GameObject.Find("Param").GetComponent<voirParam>();
        coord = GameObject.Find("Coord").GetComponent<voirCoord>();
        data = GameObject.Find("Data").GetComponent<voirData>();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();

        //voirFunc.Log("nvect:"+data.nvect);
        seeds = new double[param.nvect, voirConst.SL_NUM_LINES, 3];

        isosurf = GameObject.Find("IsoSurface").GetComponent<voirIsoSurface>();
        isosurf2 = GameObject.Find("IsoSurface2").GetComponent<voirIsoSurface2>();
        IsoSurface = GameObject.Find("IsoSurface");
        IsoSurface2 = GameObject.Find("IsoSurface2");

        slicerxy = GameObject.Find("OrthoSlicerXY").GetComponent<voirOrthoSlicerXY>();
        slicerxz = GameObject.Find("OrthoSlicerXZ").GetComponent<voirOrthoSlicerXZ>();
        sliceryz = GameObject.Find("OrthoSlicerYZ").GetComponent<voirOrthoSlicerYZ>();
        OrthoSlicerXY = GameObject.Find("OrthoSlicerXY");
        OrthoSlicerXZ = GameObject.Find("OrthoSlicerXZ");
        OrthoSlicerYZ = GameObject.Find("OrthoSlicerYZ");

        slicerxy2 = GameObject.Find("OrthoSlicerXY2").GetComponent<voirOrthoSlicerXY2>();
        slicerxz2 = GameObject.Find("OrthoSlicerXZ2").GetComponent<voirOrthoSlicerXZ2>();
        sliceryz2 = GameObject.Find("OrthoSlicerYZ2").GetComponent<voirOrthoSlicerYZ2>();
        OrthoSlicerXY2 = GameObject.Find("OrthoSlicerXY2");
        OrthoSlicerXZ2 = GameObject.Find("OrthoSlicerXZ2");
        OrthoSlicerYZ2 = GameObject.Find("OrthoSlicerYZ2");

        localslicer = GameObject.Find("LocalSlicer").GetComponent<voirLocalSlicer>();
        LocalSlicer = GameObject.Find("LocalSlicer");

        streamlines = GameObject.Find("StreamLines").GetComponent<voirStreamLines>();
        StreamLines = GameObject.Find("StreamLines");

        flashlight = GameObject.Find("FlashLight").GetComponent<voirFlashLight>();
        FlashLight = GameObject.Find("FlashLight");

        localarrows = GameObject.Find("LocalArrows").GetComponent<voirArrows>();
        LocalArrows = GameObject.Find("LocalArrows");

        surfobj = GameObject.Find("SurfaceObj").GetComponent<voirStlSurfObj>();
        curvobj = GameObject.Find("CurveObj").GetComponent<voirCurveObj>();
        frame = GameObject.Find("Frame").GetComponent<voirFrame>();
        SurfObj = GameObject.Find("SurfaceObj");
        CurvObj = GameObject.Find("CurveObj");
        Frame = GameObject.Find("Frame");

        roigui = GameObject.Find("ROIUI").GetComponent<RoiUI>();
        ROIUI = GameObject.Find("ROIUI");

        VarsSet();
        CalcRVol();
    }

    void Alloc()
    {
    }


    void VarsSet()
    {
        for (int i = 0; i < 3; i++)
        {
            roimin[i] = coord.minic[i];
            roimax[i] = coord.maxic[i];
            roign[i] = coord.gn[i];
        }
    }

    void CalcRVol()
    {
        coord.relatelen = (float)((coord.gn[0]+coord.gn[1]+coord.gn[2]) / voirConst.RI_TRACER_GRID_SIZE / 3.0);
    }

    bool ROI(int [] c1, int [] c2)
    {

        for(int i=0;i<3;i++) roign[i] = Math.Abs(c1[i] - c2[i])+1;

        if(roign[0] < voirConst.RI_MINX || roign[1] < voirConst.RI_MINY || roign[2] < voirConst.RI_MINZ)
        {
            voirFunc.Log("Region size is too small: voirROI:ROI"+roign[0]+" "+roign[1]+" "+roign[2]);
            return false;
        }

        for (int i=0;i<3;i++)
        {
            if (c1[i] > c2[i])
            {
                roimin[i] = c2[i];
                roimax[i] = c1[i];
            }
            else
            {
                roimin[i] = c1[i];
                roimax[i] = c2[i];
            }
            roign[i] = roimax[i] - roimin[i] + 1;
        }
//        voirFunc.Log("roimin:"+roimin[0]+" "+roimin[1]+" "+roimin[2]);
//        voirFunc.Log("roimax:" + roimax[0] + " " + roimax[1] + " " + roimax[2]);
//        voirFunc.Log("roign:" + roign[0] + " " + roign[1] + " " + roign[2]);

        coord.ReInitCoord(roimin, roign);
        VarsSet();

//        voirFunc.Log("roimin:" + roimin[0] + " " + roimin[1] + " " + roimin[2]);
//        voirFunc.Log("roimax:" + roimax[0] + " " + roimax[1] + " " + roimax[2]);
//        voirFunc.Log("roign:" + roign[0] + " " + roign[1] + " " + roign[2]);

        // Scalar Visualization
        if(data.nscal>0){
          IsosurfROI();
          OrthoSlicerROI();
          LocalSlicerROI();
        }

        //Vector Visualization
        if(data.nvect > 0){
          StreamLinesROI();
          LocalArrowsROI();
          FlashLightROI();
        }

        //Object
        ObjROI();

        CalcRVol();

        return true;

    }

    void IsosurfROI()
    {
        bool isoact;
        isoact = IsoSurface.activeSelf;
        IsoSurface.SetActive(true);
        isosurf.IsosurfROI();
        IsoSurface.SetActive(isoact);

        isoact = IsoSurface2.activeSelf;
        IsoSurface2.SetActive(true);
        isosurf2.IsosurfROI();
        IsoSurface2.SetActive(isoact);
    }

    void OrthoSlicerROI()
    {
        bool sliceact, exist;
        // XY Slice
        sliceact = OrthoSlicerXY.activeSelf;
        OrthoSlicerXY.SetActive(true);
        exist = slicerxy.SlicerXYROI();
        if (!sliceact || !exist) OrthoSlicerXY.SetActive(false);
        // XZ Slice
        sliceact = OrthoSlicerXZ.activeSelf;
        OrthoSlicerXZ.SetActive(true);
        exist = slicerxz.SlicerXZROI();
        if (!sliceact || !exist) OrthoSlicerXZ.SetActive(false);
        // YZ Slice
        sliceact = OrthoSlicerYZ.activeSelf;
        OrthoSlicerYZ.SetActive(true);
        exist = sliceryz.SlicerYZROI();
        if (!sliceact || !exist) OrthoSlicerYZ.SetActive(false);


        // XY Slice2
        sliceact = OrthoSlicerXY2.activeSelf;
        OrthoSlicerXY2.SetActive(true);
        exist = slicerxy2.SlicerXYROI();
        if (!sliceact || !exist) OrthoSlicerXY2.SetActive(false);
        // XZ Slice 2
        sliceact = OrthoSlicerXZ2.activeSelf;
        OrthoSlicerXZ2.SetActive(true);
        exist = slicerxz2.SlicerXZROI();
        if (!sliceact || !exist) OrthoSlicerXZ2.SetActive(false);
        // YZ Slice 2
        sliceact = OrthoSlicerYZ2.activeSelf;
        OrthoSlicerYZ2.SetActive(true);
        exist = sliceryz2.SlicerYZROI();
        if (!sliceact || !exist) OrthoSlicerYZ2.SetActive(false);

    }

    void LocalSlicerROI()
    {
        bool sliceact, exist;
        sliceact = LocalSlicer.activeSelf;
        LocalSlicer.SetActive(true);
        localslicer.GetPos(pos);
        //voirFunc.Log("lslice:" + pos[0] + " " + pos[1] + " " + pos[2]);
        exist = coord.MapPos(pos);
        //voirFunc.Log("lslice:" + exist+" "+pos[0]+" "+pos[1]+" "+pos[2]);
        if (!exist)
        {
            LocalSlicer.SetActive(false);
        }
        else
        {
            localslicer.LocalSlicerROI(pos);
            LocalSlicer.SetActive(sliceact);
        }
    }

    void StreamLinesROI()
    {
        bool slineact;
        slineact = StreamLines.activeSelf;
        StreamLines.SetActive(true);
        streamlines.CalcSkip();
        for (int j=0;j<data.nvect;j++)
        {
            int ncalc = streamlines.GetNcalc(j);
            bool cycle = streamlines.GetCycle(j);
            int nlines = ncalc;
            if (cycle) nlines = voirConst.SL_NUM_LINES;
            int nnl = 0;
            for(int i=0;i<nlines;i++)
            {
                streamlines.GetSeeds(j,i,pos);
                voirFunc.Log("stream:"+i+" " + pos[0] + " " + pos[1] + " " + pos[2]);
                if (coord.MapPos(pos))
                {
                    seeds[j, nnl, 0] = pos[0];
                    seeds[j, nnl, 1] = pos[1];
                    seeds[j, nnl, 2] = pos[2];
                    nnl++;
                    voirFunc.Log("stream:" + i + " " + pos[0] + " " + pos[1] + " " + pos[2]);
                }
            }
            streamlines.ClearLines(j);
            for(int i = 0; i < nnl; i++)
            {
                pos[0] = seeds[j, i, 0];
                pos[1] = seeds[j, i, 1];
                pos[2] = seeds[j, i, 2];
                streamlines.StartLine(j, pos, true);
            }
        }

        StreamLines.SetActive(slineact);
    }

    void FlashLightROI()
    {
        bool flact;
        flact = FlashLight.activeSelf;
        FlashLight.SetActive(true);
        flashlight.GetFLPos(pos);
        //voirFunc.Log("flashlight:" + pos[0] + " " + pos[1] + " " + pos[2]);
        bool exist = coord.MapPos(pos);
        //voirFunc.Log("flashlight:" + exist + " " + pos[0] + " " + pos[1] + " " + pos[2]);
        flashlight.FlashLightROI(pos);
        FlashLight.SetActive(flact);
    }

    void LocalArrowsROI()
    {
        bool laact, exist;
        laact = LocalArrows.activeSelf;
        LocalArrows.SetActive(true);
        localarrows.GetPos(pos);
        exist = coord.MapPos(pos);
        if (!exist)
        {
            LocalArrows.SetActive(false);
        }
        else
        {
            localarrows.LocalArrowsROI(pos);
            LocalArrows.SetActive(laact);
        }
    }

    public void ObjROI()
    {
        bool objact;
        objact = SurfObj.activeSelf;
        if(!objact) SurfObj.SetActive(true);
        surfobj.ScaleVertex();
        surfobj.SetMesh();
        SurfObj.SetActive(objact);

        objact = CurvObj.activeSelf;
        if (!objact) CurvObj.SetActive(true);
        curvobj.ScaleVertex();
        CurvObj.SetActive(objact);

        objact = Frame.activeSelf;
        if (!objact) Frame.SetActive(true);
        frame.FrameROI();
        Frame.SetActive(objact);
    }

}
