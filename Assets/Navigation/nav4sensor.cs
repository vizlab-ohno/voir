//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nav4sensor : MonoBehaviour
{
    [SerializeField] GameObject XRCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 campos = XRCamera.transform.position;
        Quaternion camrot = XRCamera.transform.rotation;
        transform.position = campos;
        transform.rotation = camrot;

    }
}
