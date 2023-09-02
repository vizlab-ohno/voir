//**********************************************
// Copyright (c) 2023 Nobuaki Ohno
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php
//**********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthoSlicerUI : MonoBehaviour
{
    [SerializeField] GameObject sliceObj;
    GameObject[] slices;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        slices = new GameObject[2];
        GameObject obj = transform.parent.gameObject;
        transform.SetParent(obj.GetComponent<Transform>());

        for (int i = 0; i < 2; i++)
        {
            slices[i]
                = Instantiate(sliceObj, new Vector3(0.0f, 0.0f, 0.0f),
                Quaternion.identity) as GameObject;
            slices[i].transform.SetParent(transform);
            slices[i].GetComponent<Transform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            slices[i].GetComponent<Transform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

    }

    public void GUIXY(Vector3 center, float scalex, float scaley)
    {
        slices[0].GetComponent<Transform>().localPosition = new Vector3(center.x, center.y, center.z);
        slices[0].GetComponent<Transform>().localScale = new Vector3(scalex, scaley, 1.0f);
        slices[0].GetComponent<Transform>().localRotation = Quaternion.Euler(90, 0, 0);
        slices[1].GetComponent<Transform>().localPosition = new Vector3(center.x, center.y, center.z);
        slices[1].GetComponent<Transform>().localScale = new Vector3(scalex, scaley, 1.0f);
        slices[1].GetComponent<Transform>().localRotation = Quaternion.Euler(-90, 0, 0);

        float time = Time.time;
        Material mat = slices[0].GetComponent<Renderer>().material;
        Material mat1 = slices[1].GetComponent<Renderer>().material;
        Color col = new Color(1.0f * Mathf.Cos(Mathf.PI * 2.0f * time), 1.0f*Mathf.Sin(Mathf.PI*2.0f*time),0.5f,0.5f);
        mat.color = col;
        mat1.color = col;
    }

    public void GUIXZ(Vector3 center, float scalex, float scalez)
    {
        slices[0].GetComponent<Transform>().localPosition = new Vector3(center.x, center.y, center.z);
        slices[0].GetComponent<Transform>().localScale = new Vector3(scalex, scalez, 1.0f);
        slices[0].GetComponent<Transform>().localRotation = Quaternion.Euler(0, 0, 0);
        slices[1].GetComponent<Transform>().localPosition = new Vector3(center.x, center.y, center.z);
        slices[1].GetComponent<Transform>().localScale = new Vector3(scalex, scalez, 1.0f);
        slices[1].GetComponent<Transform>().localRotation = Quaternion.Euler(0, 180, 0);
        float time = Time.time;
        Material mat = slices[0].GetComponent<Renderer>().material;
        Material mat1 = slices[1].GetComponent<Renderer>().material;
        Color col = new Color(1.0f * Mathf.Cos(Mathf.PI * 2.0f * time), 1.0f * Mathf.Sin(Mathf.PI * 2.0f * time), 0.5f, 0.5f);
        mat.color = col;
        mat1.color = col;
    }

    public void GUIYZ(Vector3 center, float scaley, float scalez)
    {
        slices[0].GetComponent<Transform>().localPosition = new Vector3(center.x, center.y, center.z);
        slices[0].GetComponent<Transform>().localScale = new Vector3(scaley, scalez, 1.0f);
        slices[0].GetComponent<Transform>().localRotation = Quaternion.Euler(0, 90, 0);
        slices[1].GetComponent<Transform>().localPosition = new Vector3(center.x, center.y, center.z);
        slices[1].GetComponent<Transform>().localScale = new Vector3(scaley, scalez, 1.0f);
        slices[1].GetComponent<Transform>().localRotation = Quaternion.Euler(0, -90, 0);
        float time = Time.time;
        Material mat = slices[0].GetComponent<Renderer>().material;
        Material mat1 = slices[1].GetComponent<Renderer>().material;
        Color col = new Color(1.0f * Mathf.Cos(Mathf.PI * 2.0f * time), 1.0f * Mathf.Sin(Mathf.PI * 2.0f * time), 0.5f, 0.5f);
        mat.color = col;
        mat1.color = col;
    }
}
