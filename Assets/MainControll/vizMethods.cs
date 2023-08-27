//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VizMethods{
    None,
    //Scalar Visualization
    IsoSurface1,
    IsoSurface2,
    OrthoSlicerXY1,
    OrthoSlicerYZ1,
    OrthoSlicerXZ1,
    OrthoSlicerXY2,
    OrthoSlicerYZ2,
    OrthoSlicerXZ2,
    LocalSlicer,
    //Vector Visualization
    StreamLines,
    LocalArrows,
    Snow,
    FlashLight,
    //Others
    SnapShot,
    Obj,
    ROI
}
