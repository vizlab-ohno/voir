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

public class voirParam : MonoBehaviour
{
	public string file;
	public bool skip4bytes;
	public bool single;
    public bool sobj, cobj;
	public int n1, n2, n3;
	public int nvect, nscal;
	public bool uniform;
	public double coordxc, coordyc, coordzc;
	public double dx, dy, dz;
	public string[] coordfile;
	public string[] scallabel;
	public string[] vectlabel;
	public string[] scalfile;
	public string[,] vectfile;
    public string[] curvefile;
    //public string[] surfacefile;
	public string surfacefile;


	void Awake()
    {

#if UNITY_EDITOR
		ParamRead(voirConst.paramfile);
#else
		var arg = System.Environment.GetCommandLineArgs();
		int argc = arg.Length;
		for (var i = 0; i < argc; i++)
		{
			voirFunc.Log(arg[i]);
		}
		ParamRead(arg[1]);
#endif
	}
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}


	void ParamRead(string file)
	{
		scallabel = new string[voirConst.MAXNSCAL];
		vectlabel = new string[voirConst.MAXNVECT];
		scalfile = new string[voirConst.MAXNSCAL];
		vectfile = new string[voirConst.MAXNVECT, 3];
		coordfile = new string[3];
        curvefile = new string[4]; // index, coordx, coordy, coordz
        //surfacefile = new string[7]; // index, coordx, coordy, coordz, ormx, normy, normz

        InitParam();
		Parse(file);

		voirFunc.Log("n1, n2, n3 = "+n1+","+n2 + "," + n3);

		voirFunc.Log("nvec = "+nvect+", nscal = "+nscal);

		voirFunc.Log("coord xfile = " + coordfile[0]);
		voirFunc.Log("coord yfile = " + coordfile[1]);
		voirFunc.Log("coord zfile = " + coordfile[2]);

		//vector file
		for (int j = 0; j < voirConst.MAXNVECT; j++)
		{
			voirFunc.Log("vect lable "+j+"="+vectlabel[j]);
			for (int i = 0; i < 3; i++)
			{
				voirFunc.Log("vect file "+i+"="+vectfile[j, i]);
			}
		}

		//scalar file
		for (int i = 0; i < voirConst.MAXNSCAL; i++)
		{
			voirFunc.Log("scal label "+i+"="+scallabel[i]);
			voirFunc.Log("scal file "+i+"="+scalfile[i]);
		}

	}

	void InitParam()
	{
		n1 = -1; n2 = -1; n3 = -1;
		uniform = false;
		skip4bytes = false;
		single = false;
        sobj = false; cobj = false;
		dx = -1; dy = -1; dz = -1;
		nvect = 0; nscal = 0;
		coordxc = 0.0; coordyc = 0.0; coordzc = 0.0;
	}

	void ParseLine(string line)
	{
		string[] originaltokens = line.Split(' ');
		var stlist = new List<string>();
		foreach (string s in originaltokens) {
			s.Trim();
			if (!String.IsNullOrEmpty(s)) {
				stlist.Add(s);
			}
		}
		string[] tokens;
		tokens = stlist.ToArray(); 

		int tnum = tokens.Length;


		//voirFunc.Log(tokens[0] +"-"+tokens[tnum-1]);
		//vector file
		char[] direc = new char[3] { 'X', 'Y', 'Z' };
		for (int j = 0; j < voirConst.MAXNVECT; j++)
		{
			string vlabel = string.Format("VECT{0}_LABEL", j);
			if (vlabel == tokens[0])
			{
				vectlabel[j] = tokens[tnum - 1].Replace("@s"," ");
//				voirFunc.Log(tokens[0] +"="+tokens[tnum-1]);
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				string vfile = string.Format("VECT{0}{1}", j, direc[i]);
				//voirFunc.Log(vfile);
				if (vfile == tokens[0])
				{
					vectfile[j, i] = tokens[tnum - 1];
//					voirFunc.Log(tokens[0] +"="+tokens[tnum-1]);
					return;
				}
			}
		}

		//scalar file
		for (int i = 0; i < voirConst.MAXNSCAL; i++)
		{
			string slabel = string.Format("SCAL{0}_LABEL", i);
			if (slabel == tokens[0])
			{
				scallabel[i] = tokens[tnum - 1].Replace("@s", " ");
//				voirFunc.Log(tokens[0] +"="+tokens[tnum-1]);
				return;
			}

			string sfile = string.Format("SCAL{0}", i);
			if (sfile == tokens[0])
			{
				scalfile[i] = tokens[tnum - 1];
//				voirFunc.Log(tokens[0] +"="+tokens[tnum-1]);
				return;
			}
		}

		switch (tokens[0])
		{
			case "GRIDSIZE":
//				voirFunc.Log(tokens[tnum-1]);
				n1 = int.Parse(tokens[1]);
				n2 = int.Parse(tokens[2]);
				n3 = int.Parse(tokens[3]);
				break;

			case "CORNER":
//				voirFunc.Log(tokens[tnum-1]);
				coordxc = double.Parse(tokens[1]);
				coordyc = double.Parse(tokens[2]);
				coordzc = double.Parse(tokens[3]);
				break;

			case "DX":
//				voirFunc.Log(tokens[tnum-1]);
				dx = double.Parse(tokens[1]);
				dy = double.Parse(tokens[2]);
				dz = double.Parse(tokens[3]);
				break;

			case "UNIFORM":
//				voirFunc.Log(tokens[tnum-1]);
				uniform = true;
				break;

			case "SKIP4BYTES":
				//				voirFunc.Log(tokens[tnum-1]);
				skip4bytes = true;
				break;

			case "SINGLEPRECISION":
				//				voirFunc.Log(tokens[tnum-1]);
				single = true;
				break;

			case "NSCAL":
//				voirFunc.Log(tokens[tnum-1]);
				nscal = int.Parse(tokens[tnum - 1]);
				break;

			case "NVEC":
//				voirFunc.Log(tokens[tnum-1]);
				nvect = int.Parse(tokens[tnum - 1]);
				break;

			case "COORDFILES":
//				voirFunc.Log(tokens[tnum-1]);
				coordfile[0] = tokens[1];
				coordfile[1] = tokens[2];
				coordfile[2] = tokens[3];
				break;

            case "SURFACEFILES":
				//              voirFunc.Log(tokens[tnum-1]);
				surfacefile = tokens[1];
				sobj = true;
                break;

            case "CURVEFILES":
                //              voirFunc.Log(tokens[tnum-1]);
                curvefile[0] = tokens[1];
                curvefile[1] = tokens[2];
                curvefile[2] = tokens[3];
                curvefile[3] = tokens[4];
                cobj = true;
                break;

            case "INCLUDE":
				Parse(tokens[1]);
				break;
		}


	}

	void Parse(string file)
	{

		voirFunc.Log("File = " + file);

		StreamReader sr = new StreamReader(file);

		string line = null;
		while ((line = sr.ReadLine()) != null)
		{
			string line1 = line.Replace("\t", " ");
			string tline = line1.Trim();
			voirFunc.Log(tline);
			if (tline.StartsWith("#") || String.IsNullOrWhiteSpace(tline))
			{
				continue;
			}
			ParseLine(tline);
		}

		sr.Close();

	}
}
