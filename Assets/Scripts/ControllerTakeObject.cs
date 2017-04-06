using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTakeObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    // 1 - Stock le GameObject avec lequel le controlleur est en collision, pour pouvoir l'attraper
    private GameObject collidingObject;
    // 2 -  Sert de référence au GameObject que le controlleur tiens actuellement
    private GameObject objectInHand;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        // 1 - Empeche un GO d'être potentiellement pris si le joueur tiens déjà quelque chose || pas de RigidBody
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2 - GO potentiellement prenable
        collidingObject = col.gameObject;
    }

    // 1 - Quand il y a collision, on regarde si le GO est prenable
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2 - Similaire à // 1, mais évite les bugs lorsque la collision dure
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3 - Le GO est relâché
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    private void GrabObject()
    {
        // 1 - Le GO est en main, il n'est plus en "colission"
        objectInHand = collidingObject;
        collidingObject = null;
        // 2 - La variable joint permet de joindre le controlleur et le GO saisi
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // 3 - Créer un nouveau joint lié au controleur, et réglage de la force de tenu
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        // 1 - Il faut bien qu'il y est un joint attaché au controlleur
        if (GetComponent<FixedJoint>())
        {
            // 2 - On enleve la connection entre l'objet tenu et le joint, destruction de ce dernier
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 3 - L'objet suis la trajectoire de lancé du controlleur
            //objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // 4 - La référence est remise à nulle
        objectInHand = null;
    }

	void Update () {
        // 1 - Lorsque le controlleur appuis sur la gachette et qu'il y a un GO, l'attrape
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject!=null)
            {
                GrabObject();
            }
        }

        // 2 - Relachement
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
	}
}
