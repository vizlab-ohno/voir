//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using voirCommon;

public class voirCamera : MonoBehaviour
{
    Camera cam;

    [SerializeField] AudioClip shutter;
    AudioSource aSource;

    bool bgw, flash;
    float ftime;
    int fcount;

    void Awake()
    {
        cam = GetComponent<Camera>();
        aSource = GetComponent<AudioSource>();
        bgw = false;
        flash = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Camera Movement
        if (bgw)
        {
            if (!flash && fcount + 2 < Time.frameCount)
            {
                ftime = Time.time;
                flash = true;
                cam.backgroundColor = Color.white;
            }

            if (flash)
            {
                if (ftime + 0.1f < Time.time)
                {
                    cam.backgroundColor = Color.black;
                    bgw = false;
                    flash = false;
                }
            }
        }
    }

    public void ShutterSound()// For SnapShot
    {
        aSource.PlayOneShot(shutter);
        bgw = true;
        fcount = Time.frameCount;
    }

}
