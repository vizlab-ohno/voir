//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class tracking : MonoBehaviour
{
    UnityEngine.XR.InputDevice Head;
    UnityEngine.XR.InputDevice RDevice;
    UnityEngine.XR.InputDevice LDevice;

    //[SerializeField] GameObject XRCamera;

    bool foundr, foundl, foundh, dfound;
    public bool rtrig, ltrig;
    bool rgrip, lgrip;
    Vector2 rjoys, ljoys;
    public Vector3 rpos, lpos, hpos;
    Vector3 rfd, lfd, hfd;
    Vector3 rup, lup, hup;
    Vector3 rrt, lrt, hrt;
    Quaternion rquat, lquat, hquat;
    //Quaternion Rot;
    List<UnityEngine.XR.InputDevice> XRInDevs;

    bool prev_rtstate;
    bool prev_ltstate;
    bool prev_rgstate;
    bool prev_lgstate;

    void Start()
    {
        initvar();
    }

    void initvar()
    {
        prev_rtstate = false;
        prev_ltstate = false;
        prev_rgstate = false;
        prev_lgstate = false;

        dfound = false; foundr = false; foundl = false; foundh = false;
        rtrig = false; ltrig = false;
        rgrip = false; lgrip = false;
        rjoys = Vector2.zero; ljoys = Vector2.zero;
        rpos = Vector3.zero; lpos = Vector3.zero; hpos = Vector3.zero;
        rquat = Quaternion.Euler(0, 0, 0); lquat = Quaternion.Euler(0, 0, 0); hquat = Quaternion.Euler(0, 0, 0);
        //Rot = Quaternion.Euler(0, 0, 0);
        rfd = Vector3.zero; lfd = Vector3.zero; hfd = Vector3.zero;
        rup = Vector3.zero; lup = Vector3.zero; hup = Vector3.zero;
        rrt = Vector3.zero; lrt = Vector3.zero; hrt = Vector3.zero;
        XRInDevs = new List<UnityEngine.XR.InputDevice>();
        //TDObj = GameObject.Find("Trackd");
    }

    void finddevices()
    {
        XRInDevs.Clear();
        UnityEngine.XR.InputDevices.GetDevices(XRInDevs);
        int ndevs = 0;
        ndevs = XRInDevs.Count;
        if(ndevs == 0) return;

        dfound = false; foundr = false; foundl = false; foundh = false;
        foreach (var device in XRInDevs)
        {
            //Debug.Log("device "+ device.characteristics.ToString());
            //Debug.Log("device "+ device.role.ToString());
            //switch(device.role.ToString())
            switch(device.characteristics.ToString())
            {
                //case "RightHanded":
                case "HeldInHand, TrackedDevice, Controller, Right":
                    RDevice = device;
                    foundr = true;
                    Debug.Log("RightController:" + device.characteristics.ToString());
                    break;

                //case "LeftHanded":
                case "HeldInHand, TrackedDevice, Controller, Left":
                    LDevice = device;
                    foundl = true;
                    Debug.Log("LeftController:" + device.characteristics.ToString());
                    break;

                //case "Generic":
                case "HeadMounted, TrackedDevice":
                    Head = device;
                    foundh = true;
                    Debug.Log("HeadSet:" + device.characteristics.ToString());
                    break;

                default:
                    Debug.Log("device:" + device.characteristics.ToString());
                    break;
            }
            //Debug.Log("device "+ device.role.ToString());
        }

        if (foundr && foundl && foundh)
        {
            dfound = true;
        }

        return;
    }

    void updateHData()
    {
        Head.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out hpos);
        Head.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out hquat);
        //Debug.Log(hpos);
    }

    void updateRData()
    {
        RDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rtrig);
        RDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out rgrip);
        RDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out rpos);
        RDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out rquat);
        RDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rjoys);
    }

    void updateLData()
    {
        LDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out ltrig);
        LDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out lgrip);
        LDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out lpos);
        LDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out lquat);
        LDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out ljoys);
    }

    void headConvert()
    {
        //Head
        Vector3 forward = hquat * Vector3.forward;
        Vector3 up = hquat * Vector3.up;
        Vector3 right = hquat * Vector3.right;


        Quaternion adj = Quaternion.AngleAxis(-2f, right.normalized);
        hfd = adj * forward;
        hup = adj * up;
        hrt = right;

        forward = hpos + hfd;
        up = hpos + hup;
        right = hpos + hrt;

        hpos = transform.TransformPoint(hpos);
        forward = transform.TransformPoint(forward);
        up = transform.TransformPoint(up);
        right = transform.TransformPoint(right);

        hfd = forward - hpos;
        hup = up - hpos;
        hrt = right - hpos;

        hfd.Normalize();
        hup.Normalize();
        hrt.Normalize();

    }

    void rightConvert()
    {
        //Right Controller
        Vector3 forward = rquat * Vector3.forward;
        Vector3 up = rquat * Vector3.up;
        Vector3 right = rquat * Vector3.right;


        Quaternion adj = Quaternion.AngleAxis(58f, right.normalized);
        rfd = adj * forward;
        rup = adj * up;
        rrt = right;

        forward = rpos + rfd;
        up = rpos + rup;
        right = rpos + rrt;

        rpos = transform.TransformPoint(rpos);
        forward = transform.TransformPoint(forward);
        up = transform.TransformPoint(up);
        right = transform.TransformPoint(right);

        rfd = forward - rpos;
        rup = up - rpos;
        rrt = right - rpos;

        rfd.Normalize();
        rup.Normalize();
        rrt.Normalize();

    }

    void leftConvert()
    {
        //Left Controller
        Vector3 forward = lquat * Vector3.forward;
        Vector3 up = lquat * Vector3.up;
        Vector3 right = lquat * Vector3.right;


        Quaternion adj = Quaternion.AngleAxis(58f, right.normalized);
        lfd = adj * forward;
        lup = adj * up;
        lrt = right;

        forward = lpos + lfd;
        up = lpos + lup;
        right = lpos + lrt;

        lpos = transform.TransformPoint(lpos);
        forward = transform.TransformPoint(forward);
        up = transform.TransformPoint(up);
        right = transform.TransformPoint(right);

        lfd = forward - lpos;
        lup = up - lpos;
        lrt = right - lpos;

        lfd.Normalize();
        lup.Normalize();
        lrt.Normalize();
    }

    public int getRTrigChange()
    {
        int state = 0;
        bool RT = rtrig;
        if (RT == true && prev_rtstate == false) // Pressed
        {
            state = 1;
        } else if(RT == true && prev_rtstate == true) // No Change
        {
            state = 0;
        } else if (RT == false && prev_rtstate == false) // No Change
        {
            state = 0;
        }
        else if (RT == false && prev_rtstate == true) // Released
        {
            state = -1;
        }

        prev_rtstate = RT;
        return state;
    }

    public int getLTrigChange()
    {
        int state = 0;
        bool LT = ltrig;
        if (LT == true && prev_ltstate == false) // Pressed
        {
            state = 1;
        }
        else if (LT == true && prev_ltstate == true) // No Change
        {
            state = 0;
        }
        else if (LT == false && prev_ltstate == false) // No Change
        {
            state = 0;
        }
        else if (LT == false && prev_ltstate == true) // Released
        {
            state = -1;
        }

        prev_ltstate = LT;
        return state;
    }

    public int getRGripChange()
    {
        int state = 0;
        bool RG = rgrip;
        if (RG == true && prev_rgstate == false) // Pressed
        {
            state = 1;
        }
        else if (RG == true && prev_rgstate == true) // No Change
        {
            state = 0;
        }
        else if (RG == false && prev_rgstate == false) // No Change
        {
            state = 0;
        }
        else if (RG == false && prev_rgstate == true) // Released
        {
            state = -1;
        }

        prev_rgstate = RG;
        return state;
    }

    public int getLGripChange()
    {
        int state = 0;
        bool LG = lgrip;
        if (LG == true && prev_lgstate == false) // Pressed
        {
            state = 1;
        }
        else if (LG == true && prev_lgstate == true) // No Change
        {
            state = 0;
        }
        else if (LG == false && prev_lgstate == false) // No Change
        {
            state = 0;
        }
        else if (LG == false && prev_lgstate == true) // Released
        {
            state = -1;
        }

        prev_lgstate = lgrip;
        return state;
    }

    // 0:pos, 1:forward, 2:up, 3:right
    public void getHeadData(int kind, ref Vector3 vec)
    {
        switch(kind)
        {
            case 0:
                vec.x = hpos.x;
                vec.y = hpos.y;
                vec.z = hpos.z;
//                Debug.Log(hpos);
                break;

            case 1:
                vec.x = hfd.x;
                vec.y = hfd.y;
                vec.z = hfd.z;
//                Debug.Log(hfd);
                break;

            case 2:
                vec.x = hup.x;
                vec.y = hup.y;
                vec.z = hup.z;
                break;

            case 3:
                vec.x = hrt.x;
                vec.y = hrt.y;
                vec.z = hrt.z;
                break;

            default:

                break;
        }
    }

    public void getRContData(int kind, ref Vector3 vec)
    {
        switch (kind)
        {
            case 0:
                vec.x = rpos.x;
                vec.y = rpos.y;
                vec.z = rpos.z;
                break;

            case 1:
                vec.x = rfd.x;
                vec.y = rfd.y;
                vec.z = rfd.z;
                break;

            case 2:
                vec.x = rup.x;
                vec.y = rup.y;
                vec.z = rup.z;
                break;

            case 3:
                vec.x = rrt.x;
                vec.y = rrt.y;
                vec.z = rrt.z;
                break;

            default:

                break;
        }
    }

    public void getLContData(int kind, ref Vector3 vec)
    {
        switch (kind)
        {
            case 0:
                vec.x = lpos.x;
                vec.y = lpos.y;
                vec.z = lpos.z;
                break;

            case 1:
                vec.x = lfd.x;
                vec.y = lfd.y;
                vec.z = lfd.z;
                break;

            case 2:
                vec.x = lup.x;
                vec.y = lup.y;
                vec.z = lup.z;
                break;

            case 3:
                vec.x = lrt.x;
                vec.y = lrt.y;
                vec.z = lrt.z;
                break;

            default:

                break;
        }
    }


    public bool RTrigState()
    {
        return rtrig;
    }

    public bool LTrigState()
    {
        return ltrig;
    }

    public bool RGripState()
    {
        return rgrip;
    }

    public bool LGripState()
    {
        return lgrip;
    }

    public float RJoyStickX()
    {
        return rjoys.x;
    }

    public float RJoyStickY()
    {
        return rjoys.y;
    }

    public float LJoyStickX()
    {
        return ljoys.x;
    }

    public float LJoyStickY()
    {
        return ljoys.y;
    }

    void Update()
    {
        Debug.Log("Update Tracking Data:"+dfound);
        if (!dfound)
        {
            finddevices();
            return;
        }

        updateHData();
        updateRData();
        updateLData();

        headConvert();
        rightConvert();
        leftConvert();

        //Rot = XRCamera.transform.rotation;
    }


}
