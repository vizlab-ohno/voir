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
using System.Text;
using voirCommon;

public class voirCoord : MonoBehaviour
{
	// "_" means "original"
	public int[] gn_;
	public int[] gn;
	public bool uniform;
    public double cscale;
	public double pscale;
    public double [] shift;
	public double[] coordx_, coordy_, coordz_;
	public double[] coordx, coordy, coordz;
	public double[] dx_, dx;
	public int[] minic, maxic, prev_minic, prev_maxic;
	public double[] minc_, maxc_, minc, maxc, prev_minc;
	public double h;
	public float relatelen;

	voirParam param;
	
	void Awake()
    {
		InitCoord();
    }

	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {

	}

	public void ReInitCoord(int [] newminic, int [] newgridsize)
    {
		for(int i = 0; i < 3; i++)
		{
			prev_minic[i] = minic[i];
			prev_maxic[i] = maxic[i];
		}

		for (int i = 0; i < 3; i++)
		{
			gn[i] = newgridsize[i];
			minic[i] = newminic[i];
			maxic[i] = newminic[i] + gn[i] - 1;
		}

		ScaleCoord();

	}

	public void InitCoord()
	{
		gn = new int[] { -1, -1, -1 };
		gn_ = new int[] { -1, -1, -1 };
		uniform = false;
		dx = new double[] { 0.0, 0.0, 0.0 }; dx_ = new double[] { 0.0, 0.0, 0.0 };
		minc = new double[] { 0.0, 0.0, 0.0 }; minc_ = new double[] { 0.0, 0.0, 0.0 };
		maxc = new double[] { 0.0, 0.0, 0.0 }; maxc_ = new double[] { 0.0, 0.0, 0.0 };
		prev_minc = new double[] { 0.0, 0.0, 0.0 };
		cscale = 1.0;
		pscale = 1.0;
        shift = new double[3] { 0.0, 0.0, 0.0};
		minic = new int[3]; prev_minic = new int[3];
		maxic = new int[3]; prev_maxic = new int[3];
		minc = new double[3]; maxc = new double[3];
		h = 1.0;

		int[] n = new int[3];
		param = GameObject.Find("Param").GetComponent<voirParam>();
		n[0] = param.n1; n[1] = param.n2; n[2] = param.n3;
		setInitGridSize(n);
		setUniform(param.uniform);
		if (param.uniform)
		{
			double[] tdx;
			double[] corner;
			tdx = new double[3]; corner = new double[3];
			tdx[0] = param.dx; tdx[1] = param.dy; tdx[2] = param.dz;
			corner[0] = param.coordxc; corner[1] = param.coordyc; corner[2] = param.coordzc;
			setInitUniCoord(tdx, corner);
		}
		else
		{
			setInitRectCoord(param.coordfile);
		}
		ScaleCoord();
	}

	public void setInitGridSize(int[] n)
	{
		int n1, n2, n3;
		n1 = n[0]; n2 = n[1]; n3 = n[2];
		if (n1 > 0)
		{
			gn[0] = n1;
			gn_[0] = n1;
			coordx = new double[n1];
			coordx_ = new double[n1];
		}
		else
		{
			voirFunc.Error("Grid size x is strange "+n1);
		}

		if (n2 > 0)
		{
			gn[1] = n2;
			gn_[1] = n2;
			coordy = new double[n2];
			coordy_ = new double[n2];
		}
		else
		{
			voirFunc.Error("Grid size y is strange "+n2);
		}

		if (n3 > 0)
		{
			gn[2] = n3;
			gn_[2] = n3;
			coordz = new double[n3];
			coordz_ = new double[n3];
		}
		else
		{
			voirFunc.Error("Grid size z is strange "+n3);
		}
        for (int i=0;i<3;i++)
        {
			minic[i] = 0;
			prev_minic[i] = 0;
			maxic[i] = gn[i] - 1;
			prev_maxic[i] = gn[i] - 1;
		}

	}

	public void setUniform(bool uni)
	{
		uniform = uni;
	}

	public void setInitUniCoord(double[] d_, double[] cx)
	{
		if (!uniform) return;
		for (int i = 0; i < 3; i++)
		{
			dx_[i] = d_[i];
			minc_[i] = cx[i];
			maxc_[i] = minc_[i] + (gn[i] - 1) * dx_[i];
			dx[i] = dx_[i];
			minc[i] = minc_[i];
			maxc[i] = maxc_[i];
		}

		if (dx_[0] < 0.0 || dx_[1] < 0.0 || dx_[2] < 0.0)
		{
			voirFunc.Error("dx is strange "+dx_[0]+" "+dx_[1]+" "+dx_[2]);
		}

		// x coord
		for (int i = 0; i < gn[0]; i++)
		{
			coordx_[i] = minc_[0] + i * dx_[0];
			coordx[i] = coordx_[i];
		}

		// y coord
		for (int i = 0; i < gn[1]; i++)
		{
			coordy_[i] = minc_[1] + i * dx_[1];
			coordy[i] = coordy_[i];
		}

		// z coord
		for (int i = 0; i < gn[2]; i++)
		{
			coordz_[i] = minc_[2] + i * dx_[2];
			coordz[i] = coordz_[i];
		}

	}

	public void setInitRectCoord(string[] file)
	{
		if (uniform) return;

		//int maxn = gn.Max();
		double d;
		int maxn = gn[0];
		if (maxn < gn[1]) maxn = gn[1];
		if (maxn < gn[2]) maxn = gn[2];
		long size;
		byte[] buf;
		buf = new byte[maxn * sizeof(double)];
		for (int j = 0; j < 3; j++)
		{
			FileStream fs = new FileStream(file[j], FileMode.Open, FileAccess.Read);
			if(param.skip4bytes)fs.Read(buf, 0, 4);
			size = fs.Read(buf, 0, gn[j] * sizeof(double));
			if (size != gn[j] * sizeof(double))
			{
				voirFunc.Error("read failed");
			}

			fs.Dispose();
			//			Console.WriteLine("read ends");

			switch (j)
			{
				case 0:
					for (int i = 0; i < gn[j]; i++)
					{
						coordx_[i] = BitConverter.ToDouble(buf, i * sizeof(double));
						coordx[i] = coordx_[i];
					}
					minc_[0] = coordx_[0]; maxc_[0] = coordx_[gn[0] - 1];

					dx_[0] = coordx_[1] - coordx_[0];
					for (int i = 2; i < gn[j]; i++)
					{
						d=coordx_[i]-coordx_[i-1];
						if (d < dx_[0]) dx_[0] = d;
					}
					dx[0] = dx_[0];

					break;

				case 1:
					for (int i = 0; i < gn[j]; i++)
					{
						coordy_[i] = BitConverter.ToDouble(buf, i * sizeof(double));
						coordy[i] = coordy_[i];
					}
					minc_[1] = coordy_[1]; maxc_[1] = coordy_[gn[1] - 1];

					dx_[1] = coordy_[1] - coordy_[0];
					for (int i = 2; i < gn[j]; i++)
					{
						d = coordy_[i] - coordy_[i - 1];
						if (d < dx_[1]) dx_[1] = d;
					}
					dx[1] = dx_[1];
					break;

				case 2:
					for (int i = 0; i < gn[j]; i++)
					{
						coordz_[i] = BitConverter.ToDouble(buf, i * sizeof(double));
						coordz[i] = coordz_[i];
					}
					minc_[2] = coordz_[2]; maxc_[2] = coordz_[gn[2] - 1];
					dx_[2] = coordz_[1] - coordz_[0];
					for (int i = 2; i < gn[j]; i++)
					{
						d = coordz_[i] - coordz_[i - 1];
						if (d < dx_[2]) dx_[2] = d;
					}
					dx[2] = dx_[2];
					break;

				default:
					voirFunc.Error("j is strange:: setRectCoord");
					break;
			}
		}
		buf = null;

	}

	public void ScaleCoord()
	{
		double scale, scalex, scaley, scalez;
		double lenx, leny, lenz;

		pscale = cscale;
		lenx = coordx_[maxic[0]] - coordx_[minic[0]];
		leny = coordy_[maxic[1]] - coordy_[minic[1]];
		lenz = coordz_[maxic[2]] - coordz_[minic[2]];

		scalex = voirConst.CD_DDSIZE / lenx; scaley = voirConst.CD_DDSIZE / leny; scalez = voirConst.CD_DDSIZE / lenz;
		scale = scalex;
		if (scale > scaley) scale = scaley;
		if (scale > scalez) scale = scalez;
		//voirFunc.Log("scale"+cscale+" -> "+scale);
		//voirFunc.Log("minic" + minic[0] + " " + minic[1] + " " + minic[2]);
		//voirFunc.Log("maxic"+maxic[0]+" "+maxic[1]+" "+maxic[2]);
		cscale = scale;

		prev_minc[0] = coordx[0];
		minc[0] = -lenx * scale / 2.0;

		prev_minc[1] = coordy[0];
		minc[1] = -leny * scale / 2.0;

		prev_minc[2] = coordz[0];
		minc[2] = -lenz * scale / 2.0;

		//voirFunc.Log("dx" + dx[0] + " " + dx[1] + " " + dx[2]);
		for (int i = 0; i < 3; i++)
		{
			dx[i] = dx_[i] * scale;
		}

		//voirFunc.Log("new dx" + dx[0] + " " + dx[1] + " " + dx[2]);
		if (uniform)
		{
			for (int i = 0; i < 3; i++)
			{
				maxc[i] = minc[i] + (maxic[i] - minic[i]) * dx[i];
			}
			for (int i = 0; i < gn_[0]; i++) coordx[i] = minc[0] + dx[0] * (i - minic[0]);
			for (int i = 0; i < gn_[1]; i++) coordy[i] = minc[1] + dx[1] * (i - minic[1]);
			for (int i = 0; i < gn_[2]; i++) coordz[i] = minc[2] + dx[2] * (i - minic[2]);
		}
		else
		{
			for (int i = 0; i < gn_[0]; i++) coordx[i] = minc[0] + (coordx_[i] - coordx_[minic[0]]) * scale;
			for (int i = 0; i < gn_[1]; i++) coordy[i] = minc[1] + (coordy_[i] - coordy_[minic[1]]) * scale;
			for (int i = 0; i < gn_[2]; i++) coordz[i] = minc[2] + (coordz_[i] - coordz_[minic[2]]) * scale;
			maxc[0] = coordx[maxic[0]]; maxc[1] = coordy[maxic[1]]; maxc[2] = coordz[maxic[2]];

			double d;
			dx[0] = coordx[minic[0] + 1] - coordx[minic[0]];
			for (int i = minic[0]; i < maxic[0]; i++)
            {
				d = coordx[i+1] - coordx[i];
				if (d < dx[0]) dx[0] = d;
            }

			dx[1] = coordy[minic[1] + 1] - coordy[minic[1]];
			for (int i = minic[1]; i < maxic[1]; i++)
			{
				d = coordy[i + 1] - coordy[i];
				if (d < dx[1]) dx[1] = d;
			}

			dx[2] = coordz[minic[2] + 1] - coordz[minic[2]];
			for (int i = minic[2]; i < maxic[2]; i++)
			{
				d = coordz[i + 1] - coordz[i];
				if (d < dx[2]) dx[2] = d;
			}
		}

		shift[0] = coordx[0] - coordx_[0];
		shift[1] = coordy[0] - coordy_[0];
		shift[2] = coordz[0] - coordz_[0];

		h = dx[0];
		if (h > dx[1]) h = dx[1];
		if (h > dx[2]) h = dx[2];
	}

	public bool insideData(double[] pos)
	{
		if (coordx[minic[0]] > pos[0] || coordx[maxic[0]] < pos[0]) return false;
		if (coordy[minic[1]] > pos[1] || coordy[maxic[1]] < pos[1]) return false;
		if (coordz[minic[2]] > pos[2] || coordz[maxic[2]] < pos[2]) return false;

		return true;
	}

	public bool insideData(int t, double[,] pos)
	{
		if (coordx[minic[0]] > pos[t, 0] || coordx[maxic[0]] < pos[t, 0]) return false;
		if (coordy[minic[1]] > pos[t, 1] || coordy[maxic[1]] < pos[t, 1]) return false;
		if (coordz[minic[2]] > pos[t, 2] || coordz[maxic[2]] < pos[t, 2]) return false;

		return true;
	}

	public bool insideWData(double[] pos)
	{
		if (coordx[0] > pos[0] || coordx[gn_[0]-1] < pos[0]) return false;
		if (coordy[0] > pos[1] || coordy[gn_[1]-1] < pos[1]) return false;
		if (coordz[0] > pos[2] || coordz[gn_[2]-1] < pos[2]) return false;

		return true;
	}

	public bool MapPos(double[] pos)
	{
		bool inside;
		double x_, y_, z_;
		x_ = (pos[0] - prev_minc[0]) / pscale + coordx_[0];
		y_ = (pos[1] - prev_minc[1]) / pscale + coordy_[0];
		z_ = (pos[2] - prev_minc[2]) / pscale + coordz_[0];

		pos[0] = (1.0 - cscale) * coordx_[0] + cscale * x_ + shift[0];
		pos[1] = (1.0 - cscale) * coordy_[1] + cscale * y_ + shift[1];
		pos[2] = (1.0 - cscale) * coordz_[2] + cscale * z_ + shift[2];

		inside = insideData(pos);

		return inside;
	}

	public void MapPos4Obj(float[] pos, float[] npos)
	{
		npos[0] = (float)((1.0 - cscale) * coordx_[0] + cscale * pos[0] + shift[0]);
		npos[1] = (float)((1.0 - cscale) * coordy_[1] + cscale * pos[1] + shift[1]);
		npos[2] = (float)((1.0 - cscale) * coordz_[2] + cscale * pos[2] + shift[2]);
	}

	public bool insideData(float[] pos)
	{
		if (coordx[minic[0]] > pos[0] || coordx[maxic[0]] < pos[0]) return false;
		if (coordy[minic[1]] > pos[1] || coordy[maxic[1]] < pos[1]) return false;
		if (coordz[minic[2]] > pos[2] || coordz[maxic[2]] < pos[2]) return false;

		return true;
	}

	public bool insideData(Vector3 pos)
	{
		if (coordx[minic[0]] > pos[0] || coordx[maxic[0]] < pos[0]) return false;
		if (coordy[minic[1]] > pos[1] || coordy[maxic[1]] < pos[1]) return false;
		if (coordz[minic[2]] > pos[2] || coordz[maxic[2]] < pos[2]) return false;

		return true;
	}

	public void getFact(double[] pos, double[] dd, int[] id)
	{
		int px, py, pz;
		double dpx, dpy, dpz;
		if (uniform)
		{
			dpx = (pos[0] - coordx[0]) / dx[0];
			px = (int)dpx;
			dd[0] = dpx - px;
			id[0] = px;

			dpy = (pos[1] - coordy[0]) / dx[1];
			py = (int)dpy;
			dd[1] = dpy - py;
			id[1] = py;

			dpz = (pos[2] - coordz[0]) / dx[2];
			pz = (int)dpz;
			dd[2] = dpz - pz;
			id[2] = pz;

		}
		else
		{
			px = maxic[0] - 1;
			for (int i = gn_[0] - 1; i < 0; i--)
			{
				if (pos[0] >= coordx[i])
				{
					px = i;
					break;
				}
			}
			dd[0] = (pos[0] - coordx[px]) / (coordx[px + 1] - coordx[px]);
			id[0] = px;

			py = maxic[1] - 1;
			for (int i = gn_[1] - 1; i < 0; i--)
			{
				if (pos[1] >= coordy[i])
				{
					py = i;
					break;
				}
			}
			dd[1] = (pos[1] - coordy[py]) / (coordy[py + 1] - coordy[py]);
			id[1] = py;

			pz = maxic[2] - 1;
			for (int i = gn_[2] - 1; i <0; i--)
			{
				if (pos[2] >= coordz[i])
				{
					pz = i;
					break;
				}
			}
			dd[2] = (pos[2] - coordz[pz]) / (coordz[pz + 1] - coordz[pz]);
			id[2] = pz;
		}
		for (int i=0;i<3;i++) {
			if (id[i] == gn_[i] - 1)
			{
				id[i] = gn_[i] - 2;
				dd[i] = 0.99999999999999;
			}
		}
	}

	public void getFact(int t, double[,] pos, double[,] dd, int[,] id)
	{
		int px, py, pz;
		double dpx, dpy, dpz;
		if (uniform)
		{
			dpx = (pos[t, 0] - coordx[0]) / dx[0];
			px = (int)dpx;
			dd[t, 0] = dpx - px;
			id[t, 0] = px;

			dpy = (pos[t, 1] - coordy[0]) / dx[1];
			py = (int)dpy;
			dd[t, 1] = dpy - py;
			id[t, 1] = py;

			dpz = (pos[t, 2] - coordz[0]) / dx[2];
			pz = (int)dpz;
			dd[t, 2] = dpz - pz;
			id[t, 2] = pz;

		}
		else
		{
			px = maxic[0] - 1;
			for (int i = gn_[0] - 1; i >= 0; i--)
			{
				if (pos[t, 0] >= coordx[i])
				{
					px = i;
					break;
				}
			}
			dd[t, 0] = (pos[t, 0] - coordx[px]) / (coordx[px + 1] - coordx[px]);
			id[t, 0] = px;

			py = maxic[1] - 1;
			for (int i = gn_[1] - 1; i >= 0; i--)
			{
				if (pos[t, 1] >= coordy[i])
				{
					py = i;
					break;
				}
			}
			dd[t, 1] = (pos[t, 1] - coordy[py]) / (coordy[py + 1] - coordy[py]);
			id[t, 1] = py;

			pz = maxic[2] - 1;
			for (int i = gn_[2] - 1; i >= 0; i--)
			{
				if (pos[t, 2] >= coordz[i])
				{
					pz = i;
					break;
				}
			}
			dd[t, 2] = (pos[t, 2] - coordz[pz]) / (coordz[pz + 1] - coordz[pz]);
			id[t, 2] = pz;
		}
		for (int i = 0; i < 3; i++)
		{
			if (id[t, i] == gn_[i] - 1)
			{
				id[t, i] = gn_[i] - 2;
				dd[t, i] = 0.99999999999999;
			}
		}
	}

	public bool getCoordUni()
	{
		return uniform;
	}

}
