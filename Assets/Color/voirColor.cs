//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirColor : MonoBehaviour
{
	public float [,,] scalcolor;
	public float [,,] vectcolor;
    voirData data;
    // Start is called before the first frame update
    void Awake()
    {
        data = GameObject.Find("Data").GetComponent<voirData>();
    }

    void Start()
    {
        Alloc();
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void Alloc()
    {
        scalcolor = new float [data.nscal, voirConst.NCOL, 5]; //VRGBA
		vectcolor = new float [data.nvect, voirConst.NCOL, 4]; //VRGB
    }
    
    void SetColor()
    {
        for(int i=0;i<data.nscal;i++) setScalColor(i);
        for(int i=0;i<data.nvect;i++) setVectColor(i);
    }

	private void generateColor(double val, out float r, out float g, out float b){ // val:0.0-1.0
		double pi = Math.PI;
		double x;
		if(val < 0.0) val = 0.0;
		else if(val > 1.0) val = 1.0;
		x = 2.0*pi*val;
		
		r = (float)((1.0+Math.Cos(pi+x/2.0))/2.0);
		g = (float)((1.0+Math.Cos(pi+x))/2.0);
		b = (float)((1.0+Math.Cos(x/2.0))/2.0);
	}

	public void setScalColor(int sn){
        double maxv, minv;
        maxv = data.voirScalData[sn].maxvalue;
        minv = data.voirScalData[sn].minvalue;
		double dc = (maxv-minv)/(voirConst.NCOL-1);
		float r,g,b,a;
		double val;
		a = 0.2f;
		for(int i=0;i<voirConst.NCOL;i++){
			val = dc*i/(maxv-minv);
			generateColor(val, out r, out g, out b);
			scalcolor[sn, i, 0] = (float)(dc*i+minv);
			scalcolor[sn, i, 1] = r;
			scalcolor[sn, i, 2] = g;
			scalcolor[sn, i, 3] = b;
			scalcolor[sn, i, 4] = a;
			//if(sn==0)Debug.Log("val:"+val+" r:"+r+" g:"+g+" b:"+b+"a:"+a);
		}
	}

	public void setVectColor(int vn){
        double maxv, minv;
        maxv = data.voirVectData[vn].maxvalue;
        minv = data.voirVectData[vn].minvalue;
		double dc = (maxv-minv)/(voirConst.NCOL-1);
		float r,g,b;
		double val;
		for(int i=0;i<voirConst.NCOL;i++){
			val = dc*i/(maxv-minv);
			generateColor(val, out r, out g, out b);
			vectcolor[vn, i, 0] = (float)(dc*i+minv);
			vectcolor[vn, i, 1] = r;
			vectcolor[vn, i, 2] = g;
			vectcolor[vn, i, 3] = b;
		}
	}

	public void getScalColor(int sn, double sval, ref Color color){
		float min, max, val;
		float dv, dc;
		int idc;
		min = scalcolor[sn, 0, 0]; max = scalcolor[sn, voirConst.NCOL-1, 0];
		val = (float)sval;
		if(val <= min){
			color[0] = scalcolor[sn, 0, 1];
			color[1] = scalcolor[sn, 0, 2];
			color[2] = scalcolor[sn, 0, 3];
			color[3] = scalcolor[sn, 0, 4];
			return;
		} else if(val >= max){
			color[0] = scalcolor[sn, voirConst.NCOL-1, 1];
			color[1] = scalcolor[sn, voirConst.NCOL-1, 2];
			color[2] = scalcolor[sn, voirConst.NCOL-1, 3];
			color[3] = scalcolor[sn, voirConst.NCOL-1, 4];		
			return;
		}
		dv = (max-min)/(voirConst.NCOL-1);
		dc = (val - min)/dv;
		idc = (int)dc;
		dc = dc-idc;
		if(idc == voirConst.NCOL - 1)
        {
			idc = voirConst.NCOL - 2;
			dc = 0.99999999f;
		}
		color[0] = dc*scalcolor[sn, idc+1, 1] + (1.0f-dc)*scalcolor[sn, idc, 1];
		color[1] = dc*scalcolor[sn, idc+1, 2] + (1.0f-dc)*scalcolor[sn, idc, 2];
		color[2] = dc*scalcolor[sn, idc+1, 3] + (1.0f-dc)*scalcolor[sn, idc, 3];
		color[3] = dc*scalcolor[sn, idc+1, 4] + (1.0f-dc)*scalcolor[sn, idc, 4];
	}
	
	public void getVectColor(int vn, double vval, ref Color color){
		float min, max, val;
		float dv, dc;
		int idc;
		min = vectcolor[vn, 0, 0]; max = vectcolor[vn, voirConst.NCOL-1, 0];
		val = (float)vval;
		if(val <= min){
			color[0] = vectcolor[vn, 0, 1];
			color[1] = vectcolor[vn, 0, 2];
			color[2] = vectcolor[vn, 0, 3];
			color[3] = 1.0f;
			return;
		} else if(val >= max){
			color[0] = vectcolor[vn, voirConst.NCOL-1, 1];
			color[1] = vectcolor[vn, voirConst.NCOL-1, 2];
			color[2] = vectcolor[vn, voirConst.NCOL-1, 3];
			color[3] = 1.0f;
			return;
		}
		dv = (max-min)/(voirConst.NCOL-1);
		dc = (val - min)/dv;
		idc = (int)dc;
		dc = dc-idc;
		if (idc == voirConst.NCOL - 1)
		{
			idc = voirConst.NCOL - 2;
			dc = 0.99999999f;
		}
		color[0] = dc*vectcolor[vn, idc+1, 1] + (1.0f-dc)*vectcolor[vn, idc, 1];
		color[1] = dc*vectcolor[vn, idc+1, 2] + (1.0f-dc)*vectcolor[vn, idc, 2];
		color[2] = dc*vectcolor[vn, idc+1, 3] + (1.0f-dc)*vectcolor[vn, idc, 3];
		color[3] = 1.0f;
	}

}
