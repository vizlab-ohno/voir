//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************

using System;
using UnityEngine;
using System.Collections;

namespace voirCommon
{
	public static class voirFunc
	{
		public static double innerPro(double[] a, double[] b)
		{
			double c;
			c = a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
			return c;
		}

		public static float innerPro(float[] a, float[] b)
		{
			float c;
			c = a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
			return c;
		}

		public static float Normalize(float[] a)
		{
			float c;
			c = Mathf.Sqrt(a[0]*a[0]+a[1]*a[1]+a[2]*a[2]);
			//voirFunc.Log("c="+c);
			if(c!=0.0f)
            {
				a[0] /= c; a[1] /= c;a[2] /= c;
				return c;
            }
            else { return 0.0f; }
		}

		public static float Normalize(int t, float[,] a)
		{
			float c;
			c = Mathf.Sqrt(a[t,0] * a[t, 0] + a[t, 1] * a[t, 1] + a[t, 2] * a[t, 2]);
			//voirFunc.Log("c="+c);
			if (c != 0.0f)
			{
				a[t, 0] /= c; a[t, 1] /= c; a[t, 2] /= c;
				return c;
			}
			else { return 0.0f; }
		}

		public static double Normalize(double[] a)
		{
			double c;
			c = Math.Sqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]);
			if (c != 0.0)
			{
				a[0] /= c; a[1] /= c; a[2] /= c;
				return c;
			}
			else { return 0.0; }
		}

		public static void crossPro(double[] a, double[] b, double[] c)
		{
			c[0] = a[1] * b[2] - a[2] * b[1];
			c[1] = a[2] * b[0] - a[0] * b[2];
			c[2] = a[0] * b[1] - a[1] * b[0];
		}

		public static void crossPro(float[] a, float[] b, float[] c)
		{
			c[0] = a[1] * b[2] - a[2] * b[1];
			c[1] = a[2] * b[0] - a[0] * b[2];
			c[2] = a[0] * b[1] - a[1] * b[0];
		}

		public static void Error(string mssg)
        {
			Log(mssg);
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_STANDALONE
			UnityEngine.Application.Quit();
		#endif
		}

		public static void Log(string mssg)
		{
		#if UNITY_EDITOR
			Debug.Log(mssg);
		#elif UNITY_STANDALONE
			Console.WriteLine(mssg);
		#endif
		}

		public static double rad2deg(double rad)
		{
			return rad / voirConst.PI * 180.0;
		}

		public static double deg2rad(double deg)
		{
			return deg / 180.0 * voirConst.PI;
		}
		// Data(right handed): x,y,z <-> Unity(left handed): x,z,y
		// Transform right handed system to left handed system
		public static void Right2Left(double[] r, double[] l)
		{
			l[0] = r[0];
			l[1] = r[2];
			l[2] = r[1];
		}

		// Transform right handed system to left handed system
		public static void Left2Right(double[] l, double[] r)
		{
			r[0] = l[0];
			r[1] = l[2];
			r[2] = l[1];
		}
	}

}