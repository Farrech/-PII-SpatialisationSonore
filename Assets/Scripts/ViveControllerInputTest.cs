﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerInputTest : MonoBehaviour {

    // 1 - Référence à l'objet tracké (ici un controlleur)
    private SteamVR_TrackedObject trackedObj;
    // 2 - Propriété Device : accès au controlleur. L'index de TrackedObject retourne le controlleur en input
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    void Update()
    {
        // 1
        if (Controller.GetAxis() != Vector2.zero)
        {
            Debug.Log(gameObject.name + Controller.GetAxis());
        }

        // 2
        if (Controller.GetHairTriggerDown())
        {
            Debug.Log(gameObject.name + " Trigger Press");
        }

        // 3
        if (Controller.GetHairTriggerUp())
        {
            Debug.Log(gameObject.name + " Trigger Release");
        }

        // 4
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log(gameObject.name + " Grip Press");
        }

        // 5
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log(gameObject.name + " Grip Release");
        }
    }

    void Awake() // 3 - Quand le script est chargé, trackedObj fait référence au composant (script) SteamVR_TrackedObject attaché au controller
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
}
