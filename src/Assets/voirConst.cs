using UnityEngine;
using System.Collections;

namespace voirCommon
{
	public static class voirConst
	{
		//ParamFile
		// for win
		//public const string paramfile = "c:\\linux\\voir\\dynamo.jv";
		//public const string paramfile = "Assets/voirdata/dynamo.jv";
		//public const string paramfile = "Assets/voirdata/turb.jv";
		public const string paramfile = "Assets/voirdata/lorenz.jv";

		// for mac
		//public const string paramfile = "/Users/ohno/voir/dynamo.jv";
		//public const string paramfile = "Assets/voirdata/dynamo.jv";

		// Math
		public const double PI = 3.1415926535897931;

		//parallel calc
		public const int MAXTHREADS = 4;

		//num of scalar and vector fields
		public const int MAXNVECT = 3;
		public const int MAXNSCAL = 3;

		public const double DH = 0.1; // For integral of streamlime
		public const double SDH = 0.2; // For integral of snow
		
		public const float BEAM_LEN = 0.7f;

		// Menu
		public const float MN_DIST = 2.0f;
		public const float MN_BEAM_LEN = 9.0f;
		public const float MN_PANEL_VSEP = 0.35f;
		public const int MN_PANEL_MAXCOL = 6;

		//Color
		public const int NCOL = 256;

		//CoordConst
		public const double CD_DDSIZE = 6.0;

		//Local Arrows
		public const double LA_DIAMETER = 1.0;
		public const int LA_DIV = 7;

		//Snow
		public const int SW_NUM_SNOW = 10000;
		public const bool SN_BLINK = true;

		//Flash Light
		public const int FL_NUM_SNOW = 4000;
		public const float FL_LLENGTH = 1.5f; // light length
		public const double FL_THETA = 15.0 / 180.0 * 3.1415926535897931; // light angle

		//Stream Lines
		public const int SL_NUM_VERTS = 20000;
		public const int SL_NUM_LINES = 100;
		public const float SL_BALL_SIZE = 0.1f;
		public const int SL_CALC_ACC = 3;

		// Local Slicer
		public const int LS_DIV = 25;
		public const float LS_WIDTH = 1.0f;

		// Ortho Slicer
		public const int OS_DIV = 150;
		public const int OS_CARPET = 150;
		public const float OS_GUI_X = 0.7f;
		public const float OS_GUI_Y = 0.7f;
		public const float OS_GUI_Z = 0.7f;

		//Isosurface
		public const int IS_GNISX = 150;
		public const int IS_GNISY = 150;
		public const int IS_GNISZ = 150;
		public const float IS_GUI_Z = 0.6f;
		public const float IS_GUI_RAD = 0.075f;

		//Snap Shot
		public const string SS_FILENAME = "voirSnapShot";

		//ROI
		public const int RI_MINX = 75;
		public const int RI_MINY = 75;
		public const int RI_MINZ = 75;
		public const double RI_TRACER_GRID_SIZE = 50.0;

		//TRACKD
		// 0:pos, 1:forward, 2:up, 3:right
		public const int TD_POS = 0;
		public const int TD_FORWARD = 1;
		public const int TD_UP = 2;
		public const int TD_RIGHT = 3;
	}

}
