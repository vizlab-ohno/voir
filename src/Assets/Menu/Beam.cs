//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class Beam : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3[] line;
    Vector3 rpos;
    Vector3 rfd;
    tracking trackd;

    Panels panels;
    Menu menu;

    [SerializeField] Material white;
    [SerializeField] Material cyan;
    // Start is called before the first frame update
    void Start()
    {
        Alloc();
    }

    void Alloc()
    {
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        panels = GameObject.Find("Panels").GetComponent<Panels>();
        menu = GameObject.Find("Menu").GetComponent<Menu>();

        line = new Vector3[2];
        for(int i=0;i<2;i++) line[i] = new Vector3(0,0,0);
        rpos = Vector3.zero;
        rfd = Vector3.zero;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void drawBeam()
    {
        line[0].x = rpos.x;
        line[0].y = rpos.y;
        line[0].z = rpos.z;

        line[1].x = rpos.x + voirConst.MN_BEAM_LEN * rfd.x;
        line[1].y = rpos.y + voirConst.MN_BEAM_LEN * rfd.y;
        line[1].z = rpos.z + voirConst.MN_BEAM_LEN * rfd.z;

        lineRenderer.SetPositions(line);
    }

    void selection()
    {
        RaycastHit hit;
        Ray ray = new Ray(rpos, rfd);

        //change color
        panels.ClearPanelColor();
        if (Physics.Raycast(ray, out hit, voirConst.MN_BEAM_LEN))
        {
            hit.collider.gameObject.GetComponent<Renderer>().material = cyan;//.color = Color.cyan;

            string sdname = null;
            if (trackd.getRTrigChange() == -1)
            {
                //hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                sdname = hit.collider.gameObject.name;
                int col = -1, row = -1;
                if (sdname != null) {
                    SearchPanel(sdname, ref col, ref row);
                    //Debug.Log(sdname+" : col = "+col+", row = "+row);
                    if (col !=-1 && row !=-1) {
                        menu.ItemSelected(col, row);
                        hit.collider.gameObject.GetComponent<Renderer>().material= white;//.color = Color.white;
                    }
                }

            }
        }
    }

    void SearchPanel(string pname, ref int scol, ref int srow)
    {
        int col = menu.PanelCol;
        //Col 0
        scol = 0;
        if(pname == "Scalar")
        {
            srow = 0;
            menu.selectFirst(srow);
            return;
        } else if (pname == "Vector")
        {
            srow = 1;
            menu.selectFirst(srow);
            return;
        }
        else if (pname == "ROI")
        {
            srow = 2;
            menu.selectFirst(srow);
            return;
        }
        else if (pname == "Obj")
        {
            srow = 3;
            menu.selectFirst(srow);
            return;
        }
        else if (pname == "Shot")
        {
            srow = 4;
            menu.selectFirst(srow);
            return;
        }
        else if (pname == "Quit")
        {
            srow = 5;
            menu.selectFirst(srow);
            return;
        }


        //Col 1
        scol = 1;
        int nscal = 3;
        for(int i=0;i<nscal;i++)
        {
            if(pname == "Scal" + i)
            {
                srow = i;
                menu.selectScal(srow);
                return;
            }
        }

        int nvect = 3;
        for (int i = 0; i < nvect; i++)
        {
            if (pname == "Vect" + i)
            {
                srow = i;
                menu.selectVect(srow);
                return;
            }
        }

        if (pname == "LineObj")
        {
            srow = 0;
            menu.selectObj(srow);
            return;
        }
        else if (pname == "SurfObj")
        {
            srow = 1;
            menu.selectObj(srow);
            return;
        }

        //Col 2
        scol = 2;
        if (pname == "Iso1")
        {
            srow = 0;
            menu.selectSViz(srow);
            return;
        }
        else if (pname == "Iso2")
        {
            srow = 1;
            menu.selectSViz(srow);
            return;
        }
        else if (pname == "Slice1")
        {
            srow = 2;
            menu.selectSViz(srow);
            return;
        }
        else if (pname == "Slice2")
        {
            srow = 3;
            menu.selectSViz(srow);
            return;
        }
        else if (pname == "LSlice")
        {
            srow = 4;
            menu.selectSViz(srow);
            return;
        }

        if (pname == "SLines")
        {
            srow = 0;
            menu.selectVViz(srow);
            return;
        }
        else if (pname == "LArrows")
        {
            srow = 1;
            menu.selectVViz(srow);
            return;
        }
        else if (pname == "FLight")
        {
            srow = 2;
            menu.selectVViz(srow);
            return;
        }
        else if (pname == "Snow")
        {
            srow = 3;
            menu.selectVViz(srow);
            return;
        }

        //Col 3
        scol = 3;
        if (pname == "IsoS")
        {
            srow = 0;
            menu.selectISub1(srow);
            return;
        }
        else if (pname == "IsoW")
        {
            srow = 1;
            menu.selectISub1(srow);
            return;
        }
        else if (pname == "XY")
        {
            srow = 0;
            menu.selectSSub1(srow);
            return;
        }
        else if (pname == "YZ")
        {
            srow = 1;
            menu.selectSSub1(srow);
            return;
        }
        else if (pname == "ZX")
        {
            srow = 2;
            menu.selectSSub1(srow);
            return;
        }

        //Col 4
        scol = 4;
        if (pname == "IColor")
        {
            srow = 0;
            menu.selectISub2(srow);
            return;
        }
        else if (pname == "IMono")
        {
            srow = 1;
            menu.selectISub2(srow);
            return;
        }
        else if (pname == "Plane")
        {
            srow = 0;
            menu.selectSSub2(srow);
            return;
        }
        else if (pname == "Carpet")
        {
            srow = 1;
            menu.selectSSub2(srow);
            return;
        }

        //Col 5
        scol = 5;
        if (pname == "SliceS")
        {
            srow = 0;
            menu.selectSSub3(srow);
            return;
        }
        else if (pname == "SliceW")
        {
            srow = 1;
            menu.selectSSub3(srow);
            return;
        }

        for (int i = 0; i < nscal; i++)
        {
            if (pname == "CScal" + i)
            {
                srow = i;
                menu.selectCScal(srow);
                return;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        trackd.getRContData(voirConst.TD_POS, ref rpos);
        trackd.getRContData(voirConst.TD_FORWARD, ref rfd);

        drawBeam();
        selection();
    }
}
