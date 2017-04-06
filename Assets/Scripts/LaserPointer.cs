using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    // 1 - Référence au prefab Laser
    public GameObject laserPrefab;
    // 2 - Référence a une instance de Laser
    private GameObject laser;
    // 3 - Stockage des transformations 
    private Transform laserTransform;
    // 4 - Position où la laser touche
    private Vector3 hitPoint;// nécessaire?
    // 1 - Stock le GameObject avec lequel le laser est en collision
    private GameObject collidingObject;

    public GenerationSourcesSonores1 GSS; // référence vers la source sonore 


    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        // 1 - Instancie un nouveau Laser
        laser = Instantiate(laserPrefab);
        // 2 - stockage de sa position
        laserTransform = laser.transform;
    }
    private void ShowLaser(RaycastHit hit)
    {
        // 1 - Montre le laser
        laser.SetActive(true);
        // 2 - Positionne le Laser entre le controlleur et le hitPoint. Lerp => deux position, et le pourcentage à traverser
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // 3 - Pointe le laser vers le hitPoint
        laserTransform.LookAt(hitPoint);
        // 4 - Mis à l'échelle du laser 
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

	
	// Update is called once per frame
	void Update () {
        // 1 - Si le touchpad est appuyé
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            // 2 - Tir un laser du controlleur. Si il y a un hit, on stock les coordonnées
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
            {
                collidingObject=hit.collider.gameObject;
                hitPoint = hit.point; // Nécessaire?
                ShowLaser(hit);
                //if (hit.collider.tag == "CIBLE")
                //{
                GSS.toucherCible(collidingObject);
                    //GSS.desactivationSource();
                    //Debug.Log("Coordonnées CIBLE: X = " + collidingObject.transform.position.x + "  -  Y = " + collidingObject.transform.position.y + "  - Z = " + collidingObject.transform.position.z);
                    //Debug.Log("Coordonnées HIT : X= " + hitPoint.x + "  - Y = " + hitPoint.y + "  - Z = " + hitPoint.z);
               // }
            }
        }
        else // 3 - Le Laser est désactivé au relachement du touchpad
        {
            laser.SetActive(false);
        }
	}
}
