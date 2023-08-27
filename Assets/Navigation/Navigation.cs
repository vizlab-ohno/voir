//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using voirCommon;

public class Navigation : MonoBehaviour
{
    [SerializeField] GameObject XRCamera;
    Vector3 rfd;
    Vector2 rjoy;

    tracking trackd;
    // Start is called before the first frame update
    void Start()
    {
        init();
        trackd = GameObject.Find("TrackdData").GetComponent<tracking>();
    }

    // Update is called once per frame
    void Update()
    {
        trackd.getRContData(voirConst.TD_FORWARD, ref rfd);
        float joyx, joyy;
        joyx = trackd.RJoyStickX();
        joyy = trackd.RJoyStickY();

        if (Mathf.Abs(joyy) > 0.75)
        {
            Vector3 campos = XRCamera.transform.position;
            campos += rfd * joyy * Time.deltaTime;
            XRCamera.transform.position = campos;
        }

        if (Mathf.Abs(joyx) > 0.75)
        {
            Quaternion camrot = Quaternion.Euler(0f, 45f * Time.deltaTime * joyx, 0f);
            XRCamera.transform.rotation = camrot * XRCamera.transform.rotation;
        }

    }

    void init()
    {
        rfd = Vector3.zero;
        rjoy = Vector2.zero;
    }
}
