//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using System;
using System.IO;
using System.Text;
using voirCommon;

public class vectdata
{
	public double[,,] vectx;
	public double[,,] vecty;
	public double[,,] vectz;
	public float[,,] vectxf;
	public float[,,] vectyf;
	public float[,,] vectzf;
	public double minvalue, maxvalue;
    public double minxvalue, maxxvalue;
    public double minyvalue, maxyvalue;
    public double minzvalue, maxzvalue;

	public void alloc(int []gn, bool single)
    {
		if (single) allocf(gn);
		else allocd(gn);
    }

    void allocd(int[] gn)
	{
		vectx = new double[gn[2], gn[1], gn[0]];
		vecty = new double[gn[2], gn[1], gn[0]];
		vectz = new double[gn[2], gn[1], gn[0]];
	}

	void allocf(int[] gn)
	{
		vectxf = new float[gn[2], gn[1], gn[0]];
		vectyf = new float[gn[2], gn[1], gn[0]];
		vectzf = new float[gn[2], gn[1], gn[0]];
	}

	public void minmax(bool single)
    {
		if (single) minmaxf();
		else minmaxd();
    }

	void minmaxd()
	{
		int n3 = vectx.GetLength(0);
		int n2 = vectx.GetLength(1);
		int n1 = vectx.GetLength(2);
		double[] maxarray = new double[n3];
		double[] minarray = new double[n3];
		double[] maxxarray = new double[n3];
		double[] minxarray = new double[n3];
		double[] maxyarray = new double[n3];
		double[] minyarray = new double[n3];
		double[] maxzarray = new double[n3];
		double[] minzarray = new double[n3];

		//for (int k = 0; k < n3; k++)
		Parallel.For(0, n3, k =>
		{
			double min = 1.0E+20, max = -1.0E+20;
			double minx = 1.0E+20, maxx = -1.0E+20;
			double miny = 1.0E+20, maxy = -1.0E+20;
			double minz = 1.0E+20, maxz = -1.0E+20;
			double val;

			for (int j = 0; j < n2; j++)
			{
				for (int i = 0; i < n1; i++)
				{
					val = Math.Sqrt(vectx[k, j, i] * vectx[k, j, i] +
						vecty[k, j, i] * vecty[k, j, i] +
						vectz[k, j, i] * vectz[k, j, i]);
					if (val < min) min = val;
					if (val > max) max = val;

					if (vectx[k, j, i] < minx) minx = vectx[k, j, i];
					if (vectx[k, j, i] > maxx) maxx = vectx[k, j, i];

					if (vecty[k, j, i] < miny) miny = vecty[k, j, i];
					if (vecty[k, j, i] > maxy) maxy = vecty[k, j, i];

					if (vectz[k, j, i] < minz) minz = vectz[k, j, i];
					if (vectz[k, j, i] > maxz) maxz = vectz[k, j, i];
				}
			}
			minarray[k] = min; maxarray[k] = max;
			minxarray[k] = minx; maxxarray[k] = maxx;
			minyarray[k] = miny; maxxarray[k] = maxy;
			minzarray[k] = minz; maxxarray[k] = maxz;
		});

		double min = minarray[0];
		double max = maxarray[0];
		double minx = minxarray[0];
		double maxx = maxxarray[0];
		double miny = minyarray[0];
		double maxy = maxyarray[0];
		double minz = minzarray[0];
		double maxz = maxzarray[0];
		for (int k = 1; k < n3; k++)
		{
			if (minarray[k] < min) min = minarray[k];
			if (maxarray[k] > max) max = maxarray[k];

			if (minxarray[k] < minx) minx = minxarray[k];
			if (maxxarray[k] > maxx) maxx = maxxarray[k];

			if (minyarray[k] < miny) miny = minyarray[k];
			if (maxyarray[k] > maxy) maxy = maxyarray[k];

			if (minzarray[k] < minz) minz = minzarray[k];
			if (maxzarray[k] > maxz) maxz = maxzarray[k];
		}
		maxarray = null; minarray = null;
		maxxarray = null; minxarray = null;
		maxyarray = null; minyarray = null;
		maxzarray = null; minzarray = null;

		minvalue = min; maxvalue = max;
		minxvalue = minx; maxxvalue = maxx;
		minyvalue = miny; maxyvalue = maxy;
		minzvalue = minz; maxzvalue = maxz;

		voirFunc.Log("vect min=" + minvalue + ", max=" + maxvalue);
	}


	void minmaxf()
	{
		int n3 = vectxf.GetLength(0);
		int n2 = vectxf.GetLength(1);
		int n1 = vectxf.GetLength(2);
		float[] maxarray = new float[n3];
		float[] minarray = new float[n3];
		float[] maxxarray = new float[n3];
		float[] minxarray = new float[n3];
		float[] maxyarray = new float[n3];
		float[] minyarray = new float[n3];
		float[] maxzarray = new float[n3];
		float[] minzarray = new float[n3];

		//for (int k = 0; k < n3; k++)
		Parallel.For(0, n3, k =>
		{
			float min = (float)1.0E+20, max = (float)(-1.0E+20);
			float minx = (float)1.0E+20, maxx = (float)(-1.0E+20);
			float miny = (float)1.0E+20, maxy = (float)(-1.0E+20);
			float minz = (float)1.0E+20, maxz = (float)(-1.0E+20);
			float val;

			for (int j = 0; j < n2; j++)
			{
				for (int i = 0; i < n1; i++)
				{
					val = Mathf.Sqrt(vectxf[k, j, i] * vectxf[k, j, i] +
						vectyf[k, j, i] * vectyf[k, j, i] +
						vectzf[k, j, i] * vectzf[k, j, i]);
					if (val < min) min = val;
					if (val > max) max = val;

					if (vectxf[k, j, i] < minx) minx = vectxf[k, j, i];
					if (vectxf[k, j, i] > maxx) maxx = vectxf[k, j, i];

					if (vectyf[k, j, i] < miny) miny = vectyf[k, j, i];
					if (vectyf[k, j, i] > maxy) maxy = vectyf[k, j, i];

					if (vectzf[k, j, i] < minz) minz = vectzf[k, j, i];
					if (vectzf[k, j, i] > maxz) maxz = vectzf[k, j, i];
				}
			}
			minarray[k] = min; maxarray[k] = max;
			minxarray[k] = minx; maxxarray[k] = maxx;
			minyarray[k] = miny; maxxarray[k] = maxy;
			minzarray[k] = minz; maxxarray[k] = maxz;
		});

		float min = minarray[0];
		float max = maxarray[0];
		float minx = minxarray[0];
		float maxx = maxxarray[0];
		float miny = minyarray[0];
		float maxy = maxyarray[0];
		float minz = minzarray[0];
		float maxz = maxzarray[0];
		for (int k = 1; k < n3; k++)
		{
			if (minarray[k] < min) min = minarray[k];
			if (maxarray[k] > max) max = maxarray[k];

			if (minxarray[k] < minx) minx = minxarray[k];
			if (maxxarray[k] > maxx) maxx = maxxarray[k];

			if (minyarray[k] < miny) miny = minyarray[k];
			if (maxyarray[k] > maxy) maxy = maxyarray[k];

			if (minzarray[k] < minz) minz = minzarray[k];
			if (maxzarray[k] > maxz) maxz = maxzarray[k];
		}
		maxarray = null; minarray = null;
		maxxarray = null; minxarray = null;
		maxyarray = null; minyarray = null;
		maxzarray = null; minzarray = null;

		minvalue = (double)min; maxvalue = (double)max;
		minxvalue = (double)minx; maxxvalue = (double)maxx;
		minyvalue = (double)miny; maxyvalue = (double)maxy;
		minzvalue = (double)minz; maxzvalue = (double)maxz;

		voirFunc.Log("vect min=" + minvalue + ", max=" + maxvalue);
	}
};

public class scaldata
{
	public double[,,] scal;
	public float[,,] scalf;

	public double minvalue, maxvalue;
	public double average, stdev;

	public void alloc(int [] gn, bool single)
	{
		if (single) allocf(gn);
		else allocd(gn);
	}

	void allocd(int[] gn)
	{
		scal = new double[gn[2], gn[1], gn[0]];
	}

	void allocf(int[] gn)
	{
		scalf = new float[gn[2], gn[1], gn[0]];
	}

	public void minmax(bool single, bool uniform, double [] coordx, double [] coordy, double [] coordz)
    {
		if (single) minmaxf(uniform, coordx, coordy, coordz);
		else minmaxd(uniform, coordx, coordy, coordz);
    }

	void minmaxd(bool uniform, double [] coordx, double [] coordy, double [] coordz)
	{
		int n3 = scal.GetLength(0);
		int n2 = scal.GetLength(1);
		int n1 = scal.GetLength(2);

		double[] maxarray = new double[n3];
		double[] minarray = new double[n3];
		double[] sumarray = new double[n3];
		double[] sumsarray = new double[n3];

		for (int k = 0; k < n3; k++) 
		{ 
			sumarray[k] = 0.0;
			sumsarray[k] = 0.0;
		}

		if (uniform)
		{
			Parallel.For(0, n3, k =>
			{
				double min = 1.0E+20, max = -1.0E+20;
				double val;
				for (int j = 0; j < n2; j++)
				{
					for (int i = 0; i < n1; i++)
					{
						val = scal[k, j, i];
						if (val < min) min = val;
						if (val > max) max = val;
						sumarray[k] += val;
						sumsarray[k] += val*val;
					}
				}
				maxarray[k] = max;
				minarray[k] = min;
			});
		}
		else {
			Parallel.For(0, n3, k =>
			{
				double min = 1.0E+20, max = -1.0E+20;
				double val;
				double dx, dy, dz, dv;

				if (k == 0) dz = (coordz[1] - coordz[0]) / 2.0;
				else if (k == n3 - 1) dz = (coordz[n3 - 1] - coordz[n3 - 2]) / 2.0;
				else dz = (coordz[k+1] - coordz[k - 1])/2.0;

				for (int j = 0; j < n2; j++)
				{
					if (j == 0) dy = (coordy[1] - coordy[0]) / 2.0;
					else if (j == n2 - 1) dy = (coordy[n2 - 1] - coordy[n2 - 2]) / 2.0;
					else dy = (coordy[j + 1] - coordy[j - 1]) / 2.0;

					for (int i = 0; i < n1; i++)
					{
						if (i == 0) dx = (coordx[1] - coordx[0]) / 2.0;
						else if (i == n1 - 1) dx = (coordx[n1 - 1] - coordx[n1 - 2]) / 2.0;
						else dx = (coordx[i + 1] - coordx[i - 1]) / 2.0;

						dv = dx * dy * dz;
						val = scal[k, j, i];
						if (val < min) min = val;
						if (val > max) max = val;
						sumarray[k] += val*dv;
						sumsarray[k] += val * val*dv;
					}
				}
				maxarray[k] = max;
				minarray[k] = min;
			});
		}

		double min = minarray[0];
		double max = maxarray[0];
		double sum = sumarray[0];
		double sums = sumsarray[0];
		for (int k = 1; k < n3; k++)
		{
			if (minarray[k] < min) min = minarray[k];
			if (maxarray[k] > max) max = maxarray[k];
			sum += sumarray[k];
			sums += sumsarray[k];
		}
		maxarray = null; minarray = null;
		sumarray = null; sumsarray = null;
		minvalue = min;
		maxvalue = max;

		if (uniform)
		{
			average = sum / ((double)n1*n2*n3);
			sums = sums / ((double)n1 * n2 * n3);
		}
		else
		{
			double vol = ((coordx[n1 - 1] - coordx[0]) * (coordy[n2 - 1] - coordy[0]) * (coordz[n3 - 1] - coordz[0]));
			average = sum / vol;
			sums = sums / vol;
		}
		stdev = Math.Sqrt(sums - average * average);
		voirFunc.Log("scal min = "+minvalue+", max = "+maxvalue);
		voirFunc.Log("scal average = " + average + ", standard deviation = " + stdev);
	}

	void minmaxf(bool uniform, double[] coordx, double[] coordy, double[] coordz)
	{
		int n3 = scalf.GetLength(0);
		int n2 = scalf.GetLength(1);
		int n1 = scalf.GetLength(2);
		float [] maxarray = new float[n3];
		float [] minarray = new float[n3];
		double [] sumarray = new double[n3];
		double [] sumsarray = new double[n3];

		for (int k = 0; k < n3; k++)
		{
			sumarray[k] = 0.0;
			sumsarray[k] = 0.0;
		}

		//for (int k = 0; k < n3; k++)
		if (uniform)
		{
			Parallel.For(0, n3, k =>
			{
				float min = (float)1.0E+20;
				float max = (float)(-1.0E+20);
				float val;
				for (int j = 0; j < n2; j++)
				{
					for (int i = 0; i < n1; i++)
					{
						val = scalf[k, j, i];
						if (val < min) min = val;
						if (val > max) max = val;
						sumarray[k] += val;
						sumsarray[k] += val * val;
					}
				}
				maxarray[k] = max;
				minarray[k] = min;
			});
		}
		else {

			Parallel.For(0, n3, k =>
			{
				float min = (float)1.0E+20, max = (float)(-1.0E+20);
				float val;
				double dx, dy, dz, dv;

				if (k == 0) dz = (coordz[1] - coordz[0]) / 2.0;
				else if (k == n3 - 1) dz = (coordz[n3 - 1] - coordz[n3 - 2]) / 2.0;
				else dz = (coordz[k + 1] - coordz[k - 1]) / 2.0;

				for (int j = 0; j < n2; j++)
				{
					if (j == 0) dy = (coordy[1] - coordy[0]) / 2.0;
					else if (j == n2 - 1) dy = (coordy[n2 - 1] - coordy[n2 - 2]) / 2.0;
					else dy = (coordy[j + 1] - coordy[j - 1]) / 2.0;

					for (int i = 0; i < n1; i++)
					{
						if (i == 0) dx = (coordx[1] - coordx[0]) / 2.0;
						else if (i == n1 - 1) dx = (coordx[n1 - 1] - coordx[n1 - 2]) / 2.0;
						else dx = (coordx[i + 1] - coordx[i - 1]) / 2.0;

						dv = dx * dy * dz;
						val = scalf[k, j, i];
						if (val < min) min = val;
						if (val > max) max = val;
						sumarray[k] += val * dv;
						sumsarray[k] += val * val * dv;
					}
				}
				maxarray[k] = max;
				minarray[k] = min;
			});
		}


		float min = minarray[0];
		float max = maxarray[0];
		double sum = sumarray[0];
		double sums = sumsarray[0];
		for (int k=1;k<n3;k++)
        {
			if (minarray[k] < min) min = minarray[k];
			if (maxarray[k] > max) max = maxarray[k];
			sum += sumarray[k];
			sums += sumsarray[k];
		}
		maxarray = null; minarray = null;
		sumarray = null; sumsarray = null;
		minvalue = (double)min;
		maxvalue = (double)max;

		if (uniform)
		{
			average = sum / ((double)n1 * n2 * n3);
			sums = sums / ((double)n1 * n2 * n3);
		}
		else
		{
			double vol = ((coordx[n1 - 1] - coordx[0]) * (coordy[n2 - 1] - coordy[0]) * (coordz[n3 - 1] - coordz[0]));
			average = sum / vol;
			sums = sums / vol;
		}

		stdev = Math.Sqrt(sums - average * average);
		voirFunc.Log("scal min = " + minvalue + ", max = " + maxvalue);
		voirFunc.Log("scal average = " + average + ", standard deviation = " + stdev);
	}

};


public class voirData : MonoBehaviour
{
	public int nvect, nscal;
	public vectdata[] voirVectData;
	public scaldata[] voirScalData;
	byte[] buf;
	public int[] gn;
	int[] id;
	double[] dd;
	int[,] tid;
	double[,] tdd;
	bool single;
	voirParam param;
	voirCoord coord;
	

	void Awake() 
    {
		Init();
    }

    // Start is called before the first frame update
    void Start()
    {

	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public void Init()
	{
		nvect = 0; nscal = 0;
		gn = new int[3];
		id = new int[3];
		dd = new double[3];
		tid = new int[voirConst.MAXTHREADS, 3];
		tdd = new double[voirConst.MAXTHREADS, 3];

		param = GameObject.Find("Param").GetComponent<voirParam>();
		coord = GameObject.Find("Coord").GetComponent<voirCoord>();
		single = param.single;
		setDataParam(param.nscal, param.nvect, param.n1, param.n2, param.n3);
		allocData();
		readData(param.scalfile, param.vectfile);
	}

	public void setDataParam(int sn, int vn, int nx, int ny, int nz)
	{
		nscal = sn; nvect = vn;
		gn[0] = nx; gn[1] = ny; gn[2] = nz;
	}

	public void allocData()
	{
		voirFunc.Log("Data alloc ns="+nscal+", nv="+nvect+", nx="+gn[0]+", ny="+gn[1]+", nz="+gn[2]);
		voirScalData = new scaldata[nscal];
		voirVectData = new vectdata[nvect];

		//		Console.WriteLine("scal dada");
		for (int i = 0; i < nscal; i++)
		{
			voirScalData[i] = new scaldata();
			voirScalData[i].alloc(gn, single);
			voirFunc.Log("scalar"+i+" alloced.");
		}
		//		Console.WriteLine("vect dada");
		for (int i = 0; i < nvect; i++)
		{
			voirVectData[i] = new vectdata();
			voirVectData[i].alloc(gn, single);
			voirFunc.Log("vector" + i + " alloced.");
		}
		voirFunc.Log("Data alloc ends");
	}


	public void readScalData(int ns, string[] file)
	{
		FileStream fs = new FileStream(file[ns], FileMode.Open, FileAccess.Read);
		voirFunc.Log(ns+":"+file[ns]);
		long size;
		int sizeofelement;
		if (single)
			sizeofelement = sizeof(float);
		else sizeofelement = sizeof(double);
		if (param.skip4bytes) fs.Read(buf, 0, 4);
		size = fs.Read(buf, 0, gn[0] * gn[1] * gn[2] * sizeofelement);
		if (size != gn[0] * gn[1] * gn[2] * sizeofelement)
		{
			voirFunc.Error("read failed. scal :"+ns);
		}
		//Debug.Log("read size="+size);
		fs.Dispose();
		//		Console.WriteLine("read ends");
		if (single)
		{
			Parallel.For(0, gn[2], k =>
			//for (int k = 0; k < gn[2]; k++)
			{
				for (int j = 0; j < gn[1]; j++)
				{
					for (int i = 0; i < gn[0]; i++)
					{
						voirScalData[ns].scalf[k, j, i]
							= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
						//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
					}
				}
			});
		}
		else
		{
			Parallel.For(0, gn[2], k =>
			//for (int k = 0; k < gn[2]; k++)
			{
				for (int j = 0; j < gn[1]; j++)
				{
					for (int i = 0; i < gn[0]; i++)
					{
						voirScalData[ns].scal[k, j, i]
							= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
						//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
					}
				}
			});
		}
	}


	public void readVectData(int nv, string[,] file)
	{

		for (int l = 0; l < 3; l++)
		{
			FileStream fs = new FileStream(file[nv, l], FileMode.Open, FileAccess.Read);
			voirFunc.Log(nv +" "+l+ ":" + file[nv,l]);
			long size;
			if(param.skip4bytes) fs.Read(buf, 0, 4);
			int sizeofelement;
			if (single)
				sizeofelement = sizeof(float);
			else sizeofelement = sizeof(double);

			size = fs.Read(buf, 0, gn[0] * gn[1] * gn[2] * sizeofelement);
			if (size != gn[0] * gn[1] * gn[2] * sizeofelement)
			{
				voirFunc.Error("read failed. vect :" + nv+" "+l);
			}

			fs.Dispose();
			//			Console.WriteLine("read ends");

			switch (l)
			{
				case 0:
				if (single)
				{
					Parallel.For(0, gn[2], k =>
					//for (int k = 0; k < gn[2]; k++)
					{
						for (int j = 0; j < gn[1]; j++)
						{
							for (int i = 0; i < gn[0]; i++)
							{
								voirVectData[nv].vectxf[k, j, i]
									= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
									//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
							}
						}
					});
				}
				else {
						Parallel.For(0, gn[2], k =>
						//for (int k = 0; k < gn[2]; k++)
						{
							for (int j = 0; j < gn[1]; j++)
							{
								for (int i = 0; i < gn[0]; i++)
								{
									voirVectData[nv].vectx[k, j, i]
										= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
								//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
								}
							}
						});

				}
				break;

				case 1:
					if (single) {
						Parallel.For(0, gn[2], k =>
						//for (int k = 0; k < gn[2]; k++)
						{
							for (int j = 0; j < gn[1]; j++)
							{
								for (int i = 0; i < gn[0]; i++)
								{
									voirVectData[nv].vectyf[k, j, i]
										= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
								//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
								}
							}
						});
					}
					else
					{
						Parallel.For(0, gn[2], k =>
						//for (int k = 0; k < gn[2]; k++)
						{
							for (int j = 0; j < gn[1]; j++)
							{
								for (int i = 0; i < gn[0]; i++)
								{
									voirVectData[nv].vecty[k, j, i]
										= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
								//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
								}
							}
						});
					}
				break;

				case 2:
					if (single)
					{
						Parallel.For(0, gn[2], k =>
						//for (int k = 0; k < gn[2]; k++)
						{
							for (int j = 0; j < gn[1]; j++)
							{
								for (int i = 0; i < gn[0]; i++)
								{
									voirVectData[nv].vectzf[k, j, i]
										= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
									//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
								}
							}
						});
					}
					else {
						Parallel.For(0, gn[2], k =>
						//for (int k = 0; k < gn[2]; k++)
						{
							for (int j = 0; j < gn[1]; j++)
							{
								for (int i = 0; i < gn[0]; i++)
								{
									voirVectData[nv].vectz[k, j, i]
										= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
									//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
								}
							}
						});
					}
					break;

				default:
					voirFunc.Error("i is strange:: Read Vect Data");
					break;
			}
		}
	}

	public void readScalDataM2(int ns, string[] file) //More than 2GB (< 4GB)
	{
		FileStream fs = new FileStream(file[ns], FileMode.Open, FileAccess.Read);
		voirFunc.Log(ns + ":" + file[ns]);
		int rsize, hgn2, rgn2, sizeofelement, dsize;
		if (single)
			sizeofelement = sizeof(float);
		else sizeofelement = sizeof(double);
		if (param.skip4bytes) fs.Read(buf, 0, 4);

		hgn2 = gn[2] / 2;
		rgn2 = gn[2] - hgn2;
		voirFunc.Log("hgn2 = "+hgn2+", rgn2="+rgn2);
		for (int l = 0; l < 2; l++)
		{
			if (l == 0)
			{
				dsize = (int)(gn[0] * gn[1] * hgn2 * (long)sizeofelement);
			}
			else
			{
				dsize = (int)(gn[0] * gn[1] * rgn2 * (long)sizeofelement);
			}
			if (dsize < 0) voirFunc.Error("readscaldatam2, dsize="+dsize);
			rsize = fs.Read(buf, 0, dsize);

			if (rsize != dsize)
			{
				voirFunc.Error("read failed. scal :" + ns);
			}
			//Debug.Log("read size="+size);
			//		Console.WriteLine("read ends");
			int kmax;
			if (l == 0) kmax = hgn2;
			else kmax = rgn2;

			if (single)
			{
				Parallel.For(0, kmax, k =>
				//for (int k = 0; k < kmax; k++)
				{

					for (int j = 0; j < gn[1]; j++)
					{
						for (int i = 0; i < gn[0]; i++)
						{
							voirScalData[ns].scalf[k + hgn2 * l, j, i]
								= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
							//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
						}
					}
				});
			}
			else
			{
				Parallel.For(0, kmax, k =>
				//for (int k = 0; k < kmax; k++)
				{
					for (int j = 0; j < gn[1]; j++)
					{
						for (int i = 0; i < gn[0]; i++)
						{
							voirScalData[ns].scal[k + hgn2, j, i]
									= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
								//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
							}
					}
				});
			}
			//);
		}
		fs.Dispose();
	}

	public void readVectDataM2(int nv, string[,] file)
	{

		for (int l = 0; l < 3; l++)
		{
			FileStream fs = new FileStream(file[nv, l], FileMode.Open, FileAccess.Read);
			voirFunc.Log(nv + " " + l + ":" + file[nv, l]);
			int rsize, hgn2, rgn2, sizeofelement, dsize;

			if (param.skip4bytes) fs.Read(buf, 0, 4);
			if (single)
				sizeofelement = sizeof(float);
			else sizeofelement = sizeof(double);
			hgn2 = gn[2] / 2;
			rgn2 = gn[2] - hgn2;
			for (int ll = 0; ll < 2; ll++)
			{
				if (ll == 0)
				{
					dsize = (int)(gn[0] * gn[1] * hgn2 * (long)sizeofelement);
				}
				else
				{
					dsize = (int)(gn[0] * gn[1] * rgn2 * (long)sizeofelement);
				}
				if (dsize < 0) voirFunc.Error("readvectdatam2");

				rsize = fs.Read(buf, 0, dsize);
				if (rsize != dsize)
				{
					voirFunc.Error("read failed. vect :" + nv + " " + l);
				}

				//			Console.WriteLine("read ends");

				int kmax;
				if (ll == 0) kmax = hgn2;
				else kmax = rgn2;

				switch (l)
				{
					case 0:
						if (single)
						{
							//for (int k = 0; k < kmax; k++)
							Parallel.For(0, kmax, k =>
							{
								
								for (int j = 0; j < gn[1]; j++)
								{
									for (int i = 0; i < gn[0]; i++)
									{
										voirVectData[nv].vectxf[k+ll*hgn2, j, i]
											= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
										//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
									}
								}
							});
						}
						else
						{
							Parallel.For(0, kmax, k =>
							//for (int k = 0; k < kmax; k++)
							{
								for (int j = 0; j < gn[1]; j++)
								{
									for (int i = 0; i < gn[0]; i++)
									{
										voirVectData[nv].vectx[k + ll * hgn2, j, i]
											= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
										//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
									}
								}
							});

						}                   //);
						break;

					case 1:
						if (single)
						{
							Parallel.For(0, kmax, k =>
							//for (int k = 0; k < kmax; k++)
							{
								
								for (int j = 0; j < gn[1]; j++)
								{
									for (int i = 0; i < gn[0]; i++)
									{
										voirVectData[nv].vectyf[k + ll * hgn2, j, i]
											= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
										//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
									}
								}
							});
						}
						else
						{
							Parallel.For(0, kmax, k =>
							//for (int k = 0; k < kmax; k++)
							{
								for (int j = 0; j < gn[1]; j++)
								{
									for (int i = 0; i < gn[0]; i++)
									{
										voirVectData[nv].vecty[k + ll * hgn2, j, i]
											= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
										//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
									}
								}
							});
						}
						
						break;

					case 2:
						if (single)
						{
							Parallel.For(0, kmax, k =>
							//for (int k = 0; k < kmax; k++)
							{
								for (int j = 0; j < gn[1]; j++)
								{
									for (int i = 0; i < gn[0]; i++)
									{
										voirVectData[nv].vectzf[k + ll * hgn2, j, i]
											= BitConverter.ToSingle(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(float));
										//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
									}
								}
							});
						}
						else
						{
							Parallel.For(0, kmax, k =>
							//for (int k = 0; k < kmax; k++)
							{
								for (int j = 0; j < gn[1]; j++)
								{
									for (int i = 0; i < gn[0]; i++)
									{
										voirVectData[nv].vectz[k + ll * hgn2, j, i]
											= BitConverter.ToDouble(buf, (i + j * gn[0] + k * gn[0] * gn[1]) * sizeof(double));
										//Console.WriteLine("data[{0},{1},{2}]={3}",k,j,i, data[k,j,i]);
									}
								}
							});
						}                   //);
						break;

					default:
						voirFunc.Error("i is strange:: Read Vect Data");
						break;
				}
			}
			fs.Dispose();

		}
	}

	public void readData(string[] scalfile, string[,] vectfile)
	{
		int elesize;
		bool LD;
		if (single) elesize = sizeof(float);
		else elesize = sizeof(double);
		long dsize = (long)gn[0] * gn[1] * gn[2] * elesize;

		if (dsize > Int32.MaxValue) {
			buf = new byte[Int32.MaxValue];
			LD = true; }
		else {
			buf = new byte[gn[0] * gn[1] * gn[2] * elesize];
			LD = false;
		}
		//		Console.WriteLine("scal read");
		for (int s = 0; s < nscal; s++)
		{
			if (!LD) readScalData(s, scalfile);
			else readScalDataM2(s, scalfile);

			voirScalData[s].minmax(single, param.uniform, coord.coordx_, coord.coordy_, coord.coordz_);
			voirFunc.Log("scal[" + s + "] has been read.");
		}

		//		Console.WriteLine("vect read");
		for (int v = 0; v < nvect; v++)
		{
			if(!LD) readVectData(v, vectfile);
			else readVectDataM2(v, vectfile);

			voirVectData[v].minmax(single);
			voirFunc.Log("vect[" + v + "] has been read.");
		}
		buf = null;

	}

	void setIntpolCof(out double c1, out double c2, out double c3, out double c4, out double c5, out double c6, out double c7, out double c8, double[] dd)
	{
		c1 = (1.0 - dd[0]) * (1.0 - dd[1]) * (1.0 - dd[2]);
		c2 = dd[0] * (1.0 - dd[1]) * (1.0 - dd[2]);
		c3 = (1.0 - dd[0]) * dd[1] * (1.0 - dd[2]);
		c4 = dd[0] * dd[1] * (1.0 - dd[2]);
		c5 = (1.0 - dd[0]) * (1.0 - dd[1]) * dd[2];
		c6 = dd[0] * (1.0 - dd[1]) * dd[2];
		c7 = (1.0 - dd[0]) * dd[1] * dd[2];
		c8 = dd[0] * dd[1] * dd[2];
	}

	void setIntpolCof(int t, out double c1, out double c2, out double c3, out double c4, out double c5, 
		out double c6, out double c7, out double c8, double[,] dd)
	{
		c1 = (1.0 - dd[t, 0]) * (1.0 - dd[t, 1]) * (1.0 - dd[t, 2]);
		c2 = dd[t, 0] * (1.0 - dd[t, 1]) * (1.0 - dd[t, 2]);
		c3 = (1.0 - dd[t, 0]) * dd[t, 1] * (1.0 - dd[t, 2]);
		c4 = dd[t, 0] * dd[t, 1] * (1.0 - dd[t, 2]);
		c5 = (1.0 - dd[t, 0]) * (1.0 - dd[t, 1]) * dd[t, 2];
		c6 = dd[t, 0] * (1.0 - dd[t, 1]) * dd[t, 2];
		c7 = (1.0 - dd[t, 0]) * dd[t, 1] * dd[t, 2];
		c8 = dd[t, 0] * dd[t, 1] * dd[t, 2];
	}

	public double getScalVald(int ns, int k, int j, int i)
    {
		return voirScalData[ns].scal[k, j, i];
	}

	public bool getScalVald(int ns, double[] pos, out double val)
	{
		if (ns < 0 || ns >= nscal + 4 * nvect) voirFunc.Error("getScalVal: ns is strainge. ns=" + ns);
		bool inside = coord.insideData(pos);
		val = 0.0;
		if (!inside)
		{
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(pos, dd, id);
		setIntpolCof(out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, dd);
		val = c1 * voirScalData[ns].scal[id[2], id[1], id[0]] +
			c2 * voirScalData[ns].scal[id[2], id[1], id[0] + 1] +
			c3 * voirScalData[ns].scal[id[2], id[1] + 1, id[0]] +
			c4 * voirScalData[ns].scal[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirScalData[ns].scal[id[2] + 1, id[1], id[0]] +
			c6 * voirScalData[ns].scal[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirScalData[ns].scal[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirScalData[ns].scal[id[2] + 1, id[1] + 1, id[0] + 1];
		return true;
	}

	public float getScalValf(int ns, int k, int j, int i)
	{
		return voirScalData[ns].scalf[k, j, i];
	}

	public bool getScalValf(int ns, double[] pos, out float val)
	{
		if (ns < 0 || ns >= nscal + 4 * nvect) voirFunc.Error("getScalVal: ns is strainge. ns=" + ns);
		bool inside = coord.insideData(pos);
		val = 0.0f;
		if (!inside)
		{
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(pos, dd, id);
		setIntpolCof(out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, dd);
		double vald;
		vald = c1 * voirScalData[ns].scalf[id[2], id[1], id[0]] +
			c2 * voirScalData[ns].scalf[id[2], id[1], id[0] + 1] +
			c3 * voirScalData[ns].scalf[id[2], id[1] + 1, id[0]] +
			c4 * voirScalData[ns].scalf[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirScalData[ns].scalf[id[2] + 1, id[1], id[0]] +
			c6 * voirScalData[ns].scalf[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirScalData[ns].scalf[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirScalData[ns].scalf[id[2] + 1, id[1] + 1, id[0] + 1];
		val = (float)vald;
		return true;
	}

	public bool getScalValf(int t, int ns, double[,] pos, out float val)
	{
		if (ns < 0 || ns >= nscal + 4 * nvect) voirFunc.Error("getScalVal: ns is strainge. ns=" + ns);
		bool inside = coord.insideData(t, pos);
		val = 0.0f;
		if (!inside)
		{
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(t, pos, tdd, tid);
		setIntpolCof(t, out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, tdd);
		double vald;
		vald = c1 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1], tid[t, 0]] +
			c2 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1], tid[t, 0] + 1] +
			c3 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1] + 1, tid[t, 0]] +
			c4 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1] + 1, tid[t, 0] + 1] +
			c5 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1], tid[t, 0]] +
			c6 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1], tid[t, 0] + 1] +
			c7 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1] + 1, tid[t, 0]] +
			c8 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1] + 1, tid[t, 0] + 1];
		val = (float)vald;
		return true;
	}

	public bool getScalVal(int ns, double[] pos, out double val)
	{
		if (ns < 0 || ns >= nscal+4*nvect) voirFunc.Error("getScalVal: ns is strainge. ns="+ns);
		bool inside = coord.insideData(pos);
		val = 0.0;
		if (!inside)
		{
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(pos, dd, id);
		setIntpolCof(out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, dd);
		//if (ns < nscal) {
		if (single)
		{
			val = c1 * voirScalData[ns].scalf[id[2], id[1], id[0]] +
				c2 * voirScalData[ns].scalf[id[2], id[1], id[0] + 1] +
				c3 * voirScalData[ns].scalf[id[2], id[1] + 1, id[0]] +
				c4 * voirScalData[ns].scalf[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirScalData[ns].scalf[id[2] + 1, id[1], id[0]] +
				c6 * voirScalData[ns].scalf[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirScalData[ns].scalf[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirScalData[ns].scalf[id[2] + 1, id[1] + 1, id[0] + 1];
        }
        else
        {
			val = c1 * voirScalData[ns].scal[id[2], id[1], id[0]] +
				c2 * voirScalData[ns].scal[id[2], id[1], id[0] + 1] +
				c3 * voirScalData[ns].scal[id[2], id[1] + 1, id[0]] +
				c4 * voirScalData[ns].scal[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirScalData[ns].scal[id[2] + 1, id[1], id[0]] +
				c6 * voirScalData[ns].scal[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirScalData[ns].scal[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirScalData[ns].scal[id[2] + 1, id[1] + 1, id[0] + 1];

		}
		return true;
	}

	public bool getScalVal(int t, int ns, double[,] pos, out double val)
	{
		if (ns < 0 || ns >= nscal + 4 * nvect) voirFunc.Error("getScalVal: ns is strainge. ns=" + ns);
		bool inside = coord.insideData(t, pos);
		val = 0.0;
		if (!inside)
		{
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(t, pos, tdd, tid);
		setIntpolCof(t, out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, tdd);
		//if (ns < nscal) {
		if (single)
		{
			val = c1 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1], tid[t, 0]] +
				c2 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1], tid[t, 0] + 1] +
				c3 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1] + 1, tid[t, 0]] +
				c4 * voirScalData[ns].scalf[tid[t, 2], tid[t, 1] + 1, tid[t, 0] + 1] +
				c5 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1], tid[t, 0]] +
				c6 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1], tid[t, 0] + 1] +
				c7 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1] + 1, tid[t, 0]] +
				c8 * voirScalData[ns].scalf[tid[t, 2] + 1, tid[t, 1] + 1, tid[t, 0] + 1];
		}
		else
		{
			val = c1 * voirScalData[ns].scal[tid[t, 2], tid[t, 1], tid[t, 0]] +
				c2 * voirScalData[ns].scal[tid[t, 2], tid[t, 1], tid[t, 0] + 1] +
				c3 * voirScalData[ns].scal[tid[t, 2], tid[t, 1] + 1, tid[t, 0]] +
				c4 * voirScalData[ns].scal[tid[t, 2], tid[t, 1] + 1, tid[t, 0] + 1] +
				c5 * voirScalData[ns].scal[tid[t, 2] + 1, tid[t, 1], tid[t, 0]] +
				c6 * voirScalData[ns].scal[tid[t, 2] + 1, tid[t, 1], tid[t, 0] + 1] +
				c7 * voirScalData[ns].scal[tid[t, 2] + 1, tid[t, 1] + 1, tid[t, 0]] +
				c8 * voirScalData[ns].scal[tid[t, 2] + 1, tid[t, 1] + 1, tid[t, 0] + 1];

		}
		return true;
	}

	public bool getVectValf(int nv, double[] pos, float[] val)
	{
		double v;
		bool inside = coord.insideData(pos);
		if (!inside)
		{
			val[0] = 0.0f;
			val[1] = 0.0f;
			val[2] = 0.0f;
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(pos, dd, id);
		setIntpolCof(out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, dd);
		if (nv < 0) voirFunc.Log("nv is negative"+nv);
		v = c1 * voirVectData[nv].vectxf[id[2], id[1], id[0]] +
			c2 * voirVectData[nv].vectxf[id[2], id[1], id[0] + 1] +
			c3 * voirVectData[nv].vectxf[id[2], id[1] + 1, id[0]] +
			c4 * voirVectData[nv].vectxf[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirVectData[nv].vectxf[id[2] + 1, id[1], id[0]] +
			c6 * voirVectData[nv].vectxf[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirVectData[nv].vectxf[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirVectData[nv].vectxf[id[2] + 1, id[1] + 1, id[0] + 1];

		val[0] = (float)v;

		v = c1 * voirVectData[nv].vectyf[id[2], id[1], id[0]] +
			c2 * voirVectData[nv].vectyf[id[2], id[1], id[0] + 1] +
			c3 * voirVectData[nv].vectyf[id[2], id[1] + 1, id[0]] +
			c4 * voirVectData[nv].vectyf[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirVectData[nv].vectyf[id[2] + 1, id[1], id[0]] +
			c6 * voirVectData[nv].vectyf[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirVectData[nv].vectyf[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirVectData[nv].vectyf[id[2] + 1, id[1] + 1, id[0] + 1];

		val[1] = (float)v;

		v = c1 * voirVectData[nv].vectzf[id[2], id[1], id[0]] +
			c2 * voirVectData[nv].vectzf[id[2], id[1], id[0] + 1] +
			c3 * voirVectData[nv].vectzf[id[2], id[1] + 1, id[0]] +
			c4 * voirVectData[nv].vectzf[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirVectData[nv].vectzf[id[2] + 1, id[1], id[0]] +
			c6 * voirVectData[nv].vectzf[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirVectData[nv].vectzf[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirVectData[nv].vectzf[id[2] + 1, id[1] + 1, id[0] + 1];

		val[2] = (float)v;

		return true;
	}


	public bool getVectVald(int nv, double[] pos, double[] val)
	{
		bool inside = coord.insideData(pos);
		if (!inside)
		{
			val[0] = 0.0;
			val[1] = 0.0;
			val[2] = 0.0;
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(pos, dd, id);
		setIntpolCof(out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, dd);
		val[0] = c1 * voirVectData[nv].vectx[id[2], id[1], id[0]] +
			c2 * voirVectData[nv].vectx[id[2], id[1], id[0] + 1] +
			c3 * voirVectData[nv].vectx[id[2], id[1] + 1, id[0]] +
			c4 * voirVectData[nv].vectx[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirVectData[nv].vectx[id[2] + 1, id[1], id[0]] +
			c6 * voirVectData[nv].vectx[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirVectData[nv].vectx[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirVectData[nv].vectx[id[2] + 1, id[1] + 1, id[0] + 1];

		val[1] = c1 * voirVectData[nv].vecty[id[2], id[1], id[0]] +
			c2 * voirVectData[nv].vecty[id[2], id[1], id[0] + 1] +
			c3 * voirVectData[nv].vecty[id[2], id[1] + 1, id[0]] +
			c4 * voirVectData[nv].vecty[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirVectData[nv].vecty[id[2] + 1, id[1], id[0]] +
			c6 * voirVectData[nv].vecty[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirVectData[nv].vecty[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirVectData[nv].vecty[id[2] + 1, id[1] + 1, id[0] + 1];

		val[2] = c1 * voirVectData[nv].vectz[id[2], id[1], id[0]] +
			c2 * voirVectData[nv].vectz[id[2], id[1], id[0] + 1] +
			c3 * voirVectData[nv].vectz[id[2], id[1] + 1, id[0]] +
			c4 * voirVectData[nv].vectz[id[2], id[1] + 1, id[0] + 1] +
			c5 * voirVectData[nv].vectz[id[2] + 1, id[1], id[0]] +
			c6 * voirVectData[nv].vectz[id[2] + 1, id[1], id[0] + 1] +
			c7 * voirVectData[nv].vectz[id[2] + 1, id[1] + 1, id[0]] +
			c8 * voirVectData[nv].vectz[id[2] + 1, id[1] + 1, id[0] + 1];

		return true;
	}

	public bool getVectVal(int nv, double[] pos, double[] val)
	{
		bool inside = coord.insideData(pos);
		if (!inside)
		{
			val[0] = 0.0;
			val[1] = 0.0;
			val[2] = 0.0;
			return false;
		}
		double c1, c2, c3, c4, c5, c6, c7, c8;
		coord.getFact(pos, dd, id);
		setIntpolCof(out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8, dd);
		if (single)
		{
			val[0] = c1 * voirVectData[nv].vectxf[id[2], id[1], id[0]] +
				c2 * voirVectData[nv].vectxf[id[2], id[1], id[0] + 1] +
				c3 * voirVectData[nv].vectxf[id[2], id[1] + 1, id[0]] +
				c4 * voirVectData[nv].vectxf[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirVectData[nv].vectxf[id[2] + 1, id[1], id[0]] +
				c6 * voirVectData[nv].vectxf[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirVectData[nv].vectxf[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirVectData[nv].vectxf[id[2] + 1, id[1] + 1, id[0] + 1];

			val[1] = c1 * voirVectData[nv].vectyf[id[2], id[1], id[0]] +
				c2 * voirVectData[nv].vectyf[id[2], id[1], id[0] + 1] +
				c3 * voirVectData[nv].vectyf[id[2], id[1] + 1, id[0]] +
				c4 * voirVectData[nv].vectyf[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirVectData[nv].vectyf[id[2] + 1, id[1], id[0]] +
				c6 * voirVectData[nv].vectyf[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirVectData[nv].vectyf[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirVectData[nv].vectyf[id[2] + 1, id[1] + 1, id[0] + 1];

			val[2] = c1 * voirVectData[nv].vectzf[id[2], id[1], id[0]] +
				c2 * voirVectData[nv].vectzf[id[2], id[1], id[0] + 1] +
				c3 * voirVectData[nv].vectzf[id[2], id[1] + 1, id[0]] +
				c4 * voirVectData[nv].vectzf[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirVectData[nv].vectzf[id[2] + 1, id[1], id[0]] +
				c6 * voirVectData[nv].vectzf[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirVectData[nv].vectzf[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirVectData[nv].vectzf[id[2] + 1, id[1] + 1, id[0] + 1];
        }
        else
        {
			val[0] = c1 * voirVectData[nv].vectx[id[2], id[1], id[0]] +
				c2 * voirVectData[nv].vectx[id[2], id[1], id[0] + 1] +
				c3 * voirVectData[nv].vectx[id[2], id[1] + 1, id[0]] +
				c4 * voirVectData[nv].vectx[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirVectData[nv].vectx[id[2] + 1, id[1], id[0]] +
				c6 * voirVectData[nv].vectx[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirVectData[nv].vectx[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirVectData[nv].vectx[id[2] + 1, id[1] + 1, id[0] + 1];

			val[1] = c1 * voirVectData[nv].vecty[id[2], id[1], id[0]] +
				c2 * voirVectData[nv].vecty[id[2], id[1], id[0] + 1] +
				c3 * voirVectData[nv].vecty[id[2], id[1] + 1, id[0]] +
				c4 * voirVectData[nv].vecty[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirVectData[nv].vecty[id[2] + 1, id[1], id[0]] +
				c6 * voirVectData[nv].vecty[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirVectData[nv].vecty[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirVectData[nv].vecty[id[2] + 1, id[1] + 1, id[0] + 1];

			val[2] = c1 * voirVectData[nv].vectz[id[2], id[1], id[0]] +
				c2 * voirVectData[nv].vectz[id[2], id[1], id[0] + 1] +
				c3 * voirVectData[nv].vectz[id[2], id[1] + 1, id[0]] +
				c4 * voirVectData[nv].vectz[id[2], id[1] + 1, id[0] + 1] +
				c5 * voirVectData[nv].vectz[id[2] + 1, id[1], id[0]] +
				c6 * voirVectData[nv].vectz[id[2] + 1, id[1], id[0] + 1] +
				c7 * voirVectData[nv].vectz[id[2] + 1, id[1] + 1, id[0]] +
				c8 * voirVectData[nv].vectz[id[2] + 1, id[1] + 1, id[0] + 1];
		}

		return true;
	}

	public void getScalGradd(int sn, int[] id, double[] vect)
	{
		int nx, ny, nz;
		double dx, dy, dz, dsx, dsy, dsz;
		bool uniform = true;
		// uniform = coord.uniform;
		nx = id[0]; ny = id[1]; nz = id[2];
		if (uniform)
		{
			dx = coord.dx[0]; dy = coord.dx[1]; dz = coord.dx[2];
			if (id[0] == 0)
			{
				dsx = voirScalData[sn].scal[id[2], id[1], 1] - voirScalData[sn].scal[id[2], id[1], 0];
			}
			else if (id[0] == gn[0] - 1)
			{
				dsx = voirScalData[sn].scal[id[2], id[1], gn[0] - 1] - voirScalData[sn].scal[id[2], id[1], gn[0] - 2];
			}
			else
			{
				dsx = voirScalData[sn].scal[id[2], id[1], id[0] + 1] - voirScalData[sn].scal[id[2], id[1], id[0] - 1];
				dx = dx * 2.0;
			}
			vect[0] = dsx / dx;

			if (id[1] == 0)
			{
				dsy = voirScalData[sn].scal[id[2], 1, id[0]] - voirScalData[sn].scal[id[2], 0, id[0]];
			}
			else if (id[1] == gn[1] - 1)
			{
				dsy = voirScalData[sn].scal[id[2], gn[1] - 1, id[0]] - voirScalData[sn].scal[id[2], gn[1] - 2, id[0]];
			}
			else
			{
				dsy = voirScalData[sn].scal[id[2], id[1] + 1, id[0]] - voirScalData[sn].scal[id[2], id[1] - 1, id[0]];
				dy = dy * 2.0;
			}
			vect[1] = dsy / dy;

			if (id[2] == 0)
			{
				dsz = voirScalData[sn].scal[1, id[1], id[0]] - voirScalData[sn].scal[0, id[1], id[0]];
			}
			else if (id[2] == gn[2] - 1)
			{
				dsz = voirScalData[sn].scal[gn[2] - 1, id[1], id[0]] - voirScalData[sn].scal[gn[2] - 2, id[1], id[0]];
			}
			else
			{
				dsz = voirScalData[sn].scal[id[2] + 1, id[1], id[0]] - voirScalData[sn].scal[id[2] - 1, id[1], id[0]];
				dz = dz * 2.0;
			}
			vect[2] = dsz / dz;

		}
		else
		{
			if (id[0] == 0)
			{
				dsx = voirScalData[sn].scal[id[2], id[1], 1] - voirScalData[sn].scal[id[2], id[1], 0];
				dx = coord.coordx[1] - coord.coordx[0];
			}
			else if (id[0] == gn[0] - 1)
			{
				dsx = voirScalData[sn].scal[id[2], id[1], gn[0] - 1] - voirScalData[sn].scal[id[2], id[1], gn[0] - 2];
				dx = coord.coordx[gn[0] - 1] - coord.coordx[gn[0] - 2];
			}
			else
			{
				dsx = voirScalData[sn].scal[id[2], id[1], id[0] + 1] - voirScalData[sn].scal[id[2], id[1], id[0] - 1];
				dx = coord.coordx[id[0] + 1] - coord.coordx[id[0] - 1];
			}
			vect[0] = dsx / dx;

			if (id[1] == 0)
			{
				dsy = voirScalData[sn].scal[id[2], 1, id[0]] - voirScalData[sn].scal[id[2], 0,id[0]];
				dy = coord.coordy[1] - coord.coordy[0];
			}
			else if (id[1] == gn[1] - 1)
			{
				dsy = voirScalData[sn].scal[id[2], gn[1] - 1, id[0]] - voirScalData[sn].scal[id[2], gn[1] - 2, id[0]];
				dy = coord.coordy[gn[1] - 1] - coord.coordy[gn[1] - 2];
			}
			else
			{
				dsy = voirScalData[sn].scal[id[2], id[1] + 1, id[0]] - voirScalData[sn].scal[id[2], id[1] - 1, id[0]];
				dy = coord.coordy[id[1] + 1] - coord.coordy[id[1] - 1];
			}
			vect[1] = dsy / dy;

			if (id[2] == 0)
			{
				dsz = voirScalData[sn].scal[1, id[1], id[0]] - voirScalData[sn].scal[0, id[1], id[0]];
				dz = coord.coordz[1] - coord.coordz[0];
			}
			else if (id[2] == gn[2] - 1)
			{
				dsz = voirScalData[sn].scal[gn[2] - 1, id[1], id[0]] - voirScalData[sn].scal[gn[2] - 2, id[1], id[0]];
				dz = coord.coordz[gn[2] - 1] - coord.coordz[gn[2] - 2];
			}
			else
			{
				dsz = voirScalData[sn].scal[id[2] + 1, id[1], id[0]] - voirScalData[sn].scal[id[2] - 1, id[1], id[0]];
				dz = coord.coordz[id[2] + 1] - coord.coordz[id[2] - 1];
			}
			vect[2] = dsz / dz;
		}
	}

	public void getScalGradf(int sn, int[] id, double[] vect)
	{
		int nx, ny, nz;
		double dx, dy, dz, dsx, dsy, dsz;
		bool uniform = true;
		// uniform = coord.uniform;
		nx = id[0]; ny = id[1]; nz = id[2];
		if (uniform)
		{
			dx = coord.dx[0]; dy = coord.dx[1]; dz = coord.dx[2];
			if (id[0] == 0)
			{
				dsx = voirScalData[sn].scalf[id[2], id[1], 1] - voirScalData[sn].scalf[id[2], id[1], 0];
			}
			else if (id[0] == gn[0] - 1)
			{
				dsx = voirScalData[sn].scalf[id[2], id[1], gn[0] - 1] - voirScalData[sn].scalf[id[2], id[1], gn[0] - 2];
			}
			else
			{
				dsx = voirScalData[sn].scalf[id[2], id[1], id[0] + 1] - voirScalData[sn].scalf[id[2], id[1], id[0] - 1];
				dx = dx * 2.0;
			}
			vect[0] = dsx / dx;

			if (id[1] == 0)
			{
				dsy = voirScalData[sn].scalf[id[2], 1, id[0]] - voirScalData[sn].scalf[id[2], 0, id[0]];
			}
			else if (id[1] == gn[1] - 1)
			{
				dsy = voirScalData[sn].scalf[id[2], gn[1] - 1, id[0]] - voirScalData[sn].scalf[id[2], gn[1] - 2, id[0]];
			}
			else
			{
				dsy = voirScalData[sn].scalf[id[2], id[1] + 1, id[0]] - voirScalData[sn].scalf[id[2], id[1] - 1, id[0]];
				dy = dy * 2.0;
			}
			vect[1] = dsy / dy;

			if (id[2] == 0)
			{
				dsz = voirScalData[sn].scalf[1, id[1], id[0]] - voirScalData[sn].scalf[0, id[1], id[0]];
			}
			else if (id[2] == gn[2] - 1)
			{
				dsz = voirScalData[sn].scalf[gn[2] - 1, id[1], id[0]] - voirScalData[sn].scalf[gn[2] - 2, id[1], id[0]];
			}
			else
			{
				dsz = voirScalData[sn].scalf[id[2] + 1, id[1], id[0]] - voirScalData[sn].scalf[id[2] - 1, id[1], id[0]];
				dz = dz * 2.0;
			}
			vect[2] = dsz / dz;

		}
		else
		{
			if (id[0] == 0)
			{
				dsx = voirScalData[sn].scalf[id[2], id[1], 1] - voirScalData[sn].scalf[id[2], id[1], 0];
				dx = coord.coordx[1] - coord.coordx[0];
			}
			else if (id[0] == gn[0] - 1)
			{
				dsx = voirScalData[sn].scalf[id[2], id[1], gn[0] - 1] - voirScalData[sn].scalf[id[2], id[1], gn[0] - 2];
				dx = coord.coordx[gn[0] - 1] - coord.coordx[gn[0] - 2];
			}
			else
			{
				dsx = voirScalData[sn].scalf[id[2], id[1], id[0] + 1] - voirScalData[sn].scalf[id[2], id[1], id[0] - 1];
				dx = coord.coordx[id[0] + 1] - coord.coordx[id[0] - 1];
			}
			vect[0] = dsx / dx;

			if (id[1] == 0)
			{
				dsy = voirScalData[sn].scalf[id[2], 1, id[0]] - voirScalData[sn].scalf[id[2], 0, id[0]];
				dy = coord.coordy[1] - coord.coordy[0];
			}
			else if (id[1] == gn[1] - 1)
			{
				dsy = voirScalData[sn].scalf[id[2], gn[1] - 1, id[0]] - voirScalData[sn].scalf[id[2], gn[1] - 2, id[0]];
				dy = coord.coordy[gn[1] - 1] - coord.coordy[gn[1] - 2];
			}
			else
			{
				dsy = voirScalData[sn].scalf[id[2], id[1] + 1, id[0]] - voirScalData[sn].scalf[id[2], id[1] - 1, id[0]];
				dy = coord.coordy[id[1] + 1] - coord.coordy[id[1] - 1];
			}
			vect[1] = dsy / dy;

			if (id[2] == 0)
			{
				dsz = voirScalData[sn].scalf[1, id[1], id[0]] - voirScalData[sn].scalf[0, id[1], id[0]];
				dz = coord.coordz[1] - coord.coordz[0];
			}
			else if (id[2] == gn[2] - 1)
			{
				dsz = voirScalData[sn].scalf[gn[2] - 1, id[1], id[0]] - voirScalData[sn].scalf[gn[2] - 2, id[1], id[0]];
				dz = coord.coordz[gn[2] - 1] - coord.coordz[gn[2] - 2];
			}
			else
			{
				dsz = voirScalData[sn].scalf[id[2] + 1, id[1], id[0]] - voirScalData[sn].scalf[id[2] - 1, id[1], id[0]];
				dz = coord.coordz[id[2] + 1] - coord.coordz[id[2] - 1];
			}
			vect[2] = dsz / dz;
		}
	}
}

