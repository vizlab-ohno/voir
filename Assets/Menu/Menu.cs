//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class Menu : MonoBehaviour
{
    public GameObject First;
    public GameObject Scal;
    public GameObject Vect;
    public GameObject Obj;
    public GameObject SViz;
    public GameObject VViz;
    public GameObject ISub1;
    public GameObject ISub2;
    public GameObject CScal;
    public GameObject SSub1;
    public GameObject SSub2;
    public GameObject SSub3;

    public GameObject SdCol0; // selected
    public GameObject SdCol1; // selected
    public GameObject SdCol2; // selected
    public GameObject SdCol3; // selected
    public GameObject SdCol4; // selected
    public GameObject SdCol5; // selected

    public int PanelCol;
    MainControll maincontroll;

    // Start is called before the first frame update
    void Start()
    {
        PanelCol = -1;
        turnoffAllSdCols();
        Init();
    }

    void Init()
    {
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();
    }


    void turnoffAllSdCols()
    {
        SdCol0.SetActive(false);
        SdCol1.SetActive(false);
        SdCol2.SetActive(false);
        SdCol3.SetActive(false);
        SdCol4.SetActive(false);
        SdCol5.SetActive(false);
    }

    public void ItemSelected(int col, int row)
    {
        float y = - row * voirConst.MN_PANEL_VSEP;
        switch(col)
        {
            case 0:
                turnoffAllSdCols();
                SdCol0.SetActive(true);
                SdCol0.transform.localPosition = new Vector3(0.0f, y, 0.015f);
                break;

            case 1:
                SdCol2.SetActive(false);
                SdCol3.SetActive(false);
                SdCol4.SetActive(false);
                SdCol5.SetActive(false);
                SdCol1.SetActive(true);
                SdCol1.transform.localPosition = new Vector3(0.0f, y, 0.015f);
                break;

            case 2:
                SdCol3.SetActive(false);
                SdCol4.SetActive(false);
                SdCol5.SetActive(false);
                SdCol2.SetActive(true);
                SdCol2.transform.localPosition = new Vector3(0.0f, y, 0.015f);
                break;

            case 3:
                SdCol4.SetActive(false);
                SdCol5.SetActive(false);
                SdCol3.SetActive(true);
                SdCol3.transform.localPosition = new Vector3(0.0f, y, 0.015f);
                break;

            case 4:
                SdCol5.SetActive(false);
                SdCol4.SetActive(true);
                SdCol4.transform.localPosition = new Vector3(0.0f, y, 0.015f);
                break;
            case 5:
                SdCol5.SetActive(true);
                SdCol5.transform.localPosition = new Vector3(0.0f, y, 0.015f);
                break;

            default:
                turnoffAllSdCols();
                break;
        }
    }

    public void selectFirst(int rn)
    {
        maincontroll.SelectedVal[0] = rn;
        ClearSelectedVal(0);
        switch(rn)
        {
            case 0:
                //Scal
                ActivateScal();
                break;

            case 1:
                //Vect
                ActivateVect();
                break;

            case 2:
                // ROI
                maincontroll.VizSelected();
                break;

            case 3:
                // Obj
                ActivateObj();
                break;

            case 4:
                // Snap Shot
                maincontroll.VizSelected();
                break;

            case 5:
                // Quit
                voirFunc.Log("VOIR ends");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif                
                break;
        }
    }

    public void selectObj(int rn)
    {
        maincontroll.SelectedVal[1] = rn;
        ClearSelectedVal(1);
        maincontroll.VizSelected();
    }

    public void selectScal(int rn)
    {
        maincontroll.SelectedVal[1] = rn;
        ClearSelectedVal(1);
        ActivateSViz();
    }

    public void selectVect(int rn)
    {
        maincontroll.SelectedVal[1] = rn;
        ClearSelectedVal(1);
        ActivateVViz();
    }

    public void selectSViz(int rn)
    {
        maincontroll.SelectedVal[2] = rn;
        ClearSelectedVal(2);
        switch(rn)
        {
            case 0:
                //Isosurface1
                ActivateISub1();
                break;

            case 1:
                //Isosurface2
                ActivateISub1();
                break;

            case 2:
                //OrthoSlicer1
                ActivateSSub1();
                break;

            case 3:
                //OrthoSlicer2
                ActivateSSub1();
                break;

            case 4:
                // Local Slice
                TurnOffPanel(2);
                maincontroll.VizSelected();
                break;
        }
    }

    public void selectVViz(int rn)
    {
        maincontroll.SelectedVal[2] = rn;
        ClearSelectedVal(2);
        switch (rn)
        {
            case 0:
                //Stream LInes
                maincontroll.VizSelected();
                break;

            case 1:
                //Local Arrows
                maincontroll.VizSelected();
                break;

            case 2:
                // Flash Light
                maincontroll.VizSelected();
                break;

            case 3:
                // Snow
                maincontroll.VizSelected();
                break;

        }
    }

    public void selectISub1(int rn)
    {
        maincontroll.SelectedVal[3] = rn;
        ClearSelectedVal(3);
        ActivateISub2();
        switch (rn)
        {
            case 0:
                //Surface
                break;

            case 1:
                //Wire Frame
                break;
        }
    }

    public void selectSSub1(int rn)
    {
        maincontroll.SelectedVal[3] = rn;
        ClearSelectedVal(3);
        ActivateSSub2();
        switch (rn)
        {
            case 0:
                //XY
                break;

            case 1:
                //YZ
                break;

            case 2:
                //ZX
                break;
        }
    }

    public void selectISub2(int rn)
    {
        maincontroll.SelectedVal[4] = rn;
        ClearSelectedVal(4);
        switch (rn)
        {
            case 0:
                //Coloring
                ActivateCScal();
                break;

            case 1:
                //Mono
                TurnOffPanel(4);
                maincontroll.VizSelected();
                break;
        }
    }

    public void selectSSub2(int rn)
    {
        maincontroll.SelectedVal[4] = rn;
        ClearSelectedVal(4);
        ActivateSSub3();
        switch (rn)
        {
            case 0:
                //Plane
                break;

            case 1:
                //Carpet
                break;
        }
    }

    public void selectCScal(int rn)
    {
        maincontroll.SelectedVal[5] = rn;
        ClearSelectedVal(5);
        maincontroll.VizSelected();
    }

    public void selectSSub3(int rn)
    {
        maincontroll.SelectedVal[5] = rn;
        ClearSelectedVal(5);
        switch (rn)
        {
            case 0:
                //Surface
                maincontroll.VizSelected();
                break;

            case 1:
                //Wire Frame
                maincontroll.VizSelected();
                break;
        }
    }


    // Column 0
    public void ActivateMenu()
    {
        PanelCol = 0;
        TurnOffPanel(PanelCol);
        turnoffAllSdCols();
        First.SetActive(true);
    }

    // Column 1
    public void ActivateScal()
    {
        PanelCol = 1;
        TurnOffPanel(PanelCol-1);
        Scal.SetActive(true);
    }

    public void ActivateVect()
    {
        PanelCol = 1;
        TurnOffPanel(PanelCol-1);
        Vect.SetActive(true);
    }

    public void ActivateObj()
    {
        PanelCol = 1;
        TurnOffPanel(PanelCol-1);
        Obj.SetActive(true);
    }

    // Column 2
    public void ActivateSViz()
    {
        PanelCol = 2;
        TurnOffPanel(PanelCol-1);
        SViz.SetActive(true);
    }

    public void ActivateVViz()
    {
        PanelCol = 2;
        TurnOffPanel(PanelCol-1);
        VViz.SetActive(true);
    }

    // Column 3
    public void ActivateISub1()
    {
        PanelCol = 3;
        TurnOffPanel(PanelCol-1);
        ISub1.SetActive(true);
    }

    public void ActivateSSub1()
    {
        PanelCol = 3;
        TurnOffPanel(PanelCol-1);
        SSub1.SetActive(true);
    }

    // Column 4
    public void ActivateISub2()
    {
        PanelCol = 4;
        TurnOffPanel(PanelCol-1);
        ISub2.SetActive(true);
    }

    public void ActivateSSub2()
    {
        PanelCol = 4;
        TurnOffPanel(PanelCol-1);
        SSub2.SetActive(true);
    }

    // Column 5
    public void ActivateCScal()
    {
        PanelCol = 5;
        TurnOffPanel(PanelCol-1);
        CScal.SetActive(true);
    }

    public void ActivateSSub3()
    {
        PanelCol = 5;
        TurnOffPanel(PanelCol-1);
        SSub3.SetActive(true);
    }

    void ClearSelectedVal(int cn)
    {
        for (int i = cn+1; i < voirConst.MN_PANEL_MAXCOL; i++) maincontroll.SelectedVal[i] = -1;
    }

    void TurnOffPanel(int cn)
    {
        switch (cn)
        {
            case 0:
                Scal.SetActive(false);
                Vect.SetActive(false);
                Obj.SetActive(false);

                SViz.SetActive(false);
                VViz.SetActive(false);

                ISub1.SetActive(false);
                SSub1.SetActive(false);

                ISub2.SetActive(false);
                SSub2.SetActive(false);

                CScal.SetActive(false);
                SSub3.SetActive(false);
                break;

            case 1:
                SViz.SetActive(false);
                VViz.SetActive(false);

                ISub1.SetActive(false);
                SSub1.SetActive(false);

                ISub2.SetActive(false);
                SSub2.SetActive(false);

                CScal.SetActive(false);
                SSub3.SetActive(false);
                break;

            case 2:
                ISub1.SetActive(false);
                SSub1.SetActive(false);

                ISub2.SetActive(false);
                SSub2.SetActive(false);

                CScal.SetActive(false);
                SSub3.SetActive(false);
                break;

            case 3:
                ISub2.SetActive(false);
                SSub2.SetActive(false);

                CScal.SetActive(false);
                SSub3.SetActive(false);
                break;

            case 4:
                CScal.SetActive(false);
                SSub3.SetActive(false);
                break;

            case 5:
                break;

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
