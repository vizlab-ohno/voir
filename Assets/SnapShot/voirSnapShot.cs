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

public class voirSnapShot : MonoBehaviour
{
    voirCamera cam;
    MainControll maincontroll;
    tracking trackd;
    int shotnum;
    float time;

    void Awake()
    {
        shotnum = 0;
        time = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        //voirFunc.Log("SnapShot");
        cam = GameObject.Find("MainCamera").GetComponent<voirCamera>();
        maincontroll = GameObject.Find("MainControll").GetComponent<MainControll>();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //voirFunc.Log("SnapShot: Update");
        if (trackd.getRTrigChange() == 1 && time > 0.1f)
        {
            voirFunc.Log("snapshot" + String.Format("{0:0000}", shotnum) + " is taken");
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/"
                + voirConst.SS_FILENAME + String.Format("{0:0000}", shotnum) + ".png");
            shotnum++;
            cam.ShutterSound();
            time = 0;
        }

        if (trackd.getRGripChange() == 1)
        {
            time = 0;
            maincontroll.SetNone();
            gameObject.SetActive(false);
        }

        time += Time.deltaTime;

    }
}
