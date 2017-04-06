using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationLaser : MonoBehaviour {

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


    // 1 Transform de la caméra
    public Transform cameraRigTransform;
    // 2 - Reference au prefab réticule
    public GameObject teleportReticlePrefab;
    // 3 - Instance du réticule
    private GameObject reticle;
    // 4 - Référence du transform du réticule
    private Transform teleportReticleTransform;
    // 5 - Transform de la position de la tête
    public Transform headTransform;
    // 6 - Contrebalance la différence entre le reticule et le sol ???
    public Vector3 teleportReticleOffset;
    // 7 - Layer où la téléportation est permise
    public LayerMask teleportMask;
    // 8 - True si il est possible de se téléporter
    private bool shouldTeleport;

    public GenerationSourcesSonores1 GSS; // référence vers la source sonore 


    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        // 1 - Instancie un nouveau Laser
        laser = Instantiate(laserPrefab);
        // 2 - stockage de sa position
        laserTransform = laser.transform;
        // 1 - Spawn a new reticle and save a reference to it in reticle.
        reticle = Instantiate(teleportReticlePrefab);
        // 2 - Store the reticle’s transform component.
        teleportReticleTransform = reticle.transform;
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

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1 - Si le touchpad est appuyé
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            // 2 - Tir un laser du controlleur. Si il y a un hit, on stock les coordonnées
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                collidingObject = hit.collider.gameObject;
                hitPoint = hit.point; // Nécessaire?
                ShowLaser(hit);
                if (hit.collider.tag == "CIBLE")
                {
                    GSS.desactivationSource();
                    Debug.Log("Coordonnées CIBLE: X = " + collidingObject.transform.position.x + "  -  Y = " + collidingObject.transform.position.y + "  - Z = " + collidingObject.transform.position.z);
                    Debug.Log("Coordonnées HIT : X= " + hitPoint.x + "  - Y = " + hitPoint.y + "  - Z = " + hitPoint.z);
                }
                // 1 - Montre le réticule
                reticle.SetActive(true);
                // 2 - Pris en compte du contrebalencement entre l'endroit visé et la "vraie" position voulue
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                // 3
                shouldTeleport = true;
            }
        }
        else // 3 - Le Laser est désactivé au relachement du touchpad
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        // 1 - Set the shouldTeleport flag to false when teleportation is in progress.
        shouldTeleport = false;
        // 2 -Hide the reticle.
        reticle.SetActive(false);
        // 3 -Calculate the difference between the positions of the camera rig’s center and the player’s head.
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // 4 -Reset the y-position for the above difference to 0, because the calculation doesn’t consider the vertical position of the player’s head.
        difference.y = 0;
        // 5 - Move the camera rig to the position of the hit point and add the calculated difference. Without the difference, the player would teleport to an incorrect location.
        cameraRigTransform.position = hitPoint + difference;
    }
}
