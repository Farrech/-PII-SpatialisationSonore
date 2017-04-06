using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandesSpheriques : MonoBehaviour
{
    // Ce script permet de diriger la sphère périphérique selon les interactions de l'utilisateur 
    // avec la sphère de commande

    // Le facteur d'offset récupéré du script PrisObjetControlleur permet au joueur de choisir 
    // d'augmenter ou de diminuer l'interprétation de distance du système de commande :
    // Plus il sera fort, plus une variation de la position de la sphère de contrôle entrainera un déplacement
    // important de la sphère périphérique

    public Transform headTransform; // transform de la tête
    public Transform userSphereTrans; // transform de la sphère représentant l'utilisateur (SU)
    public Transform commandSphereTrans;// transform de la sphère de commande (SC)
    public Transform periphericSphereTrans;// transform de la sphère périphérique (SP)
    public ControllerTakeObjectOffset facteurOffSet; // récupère le facteur d'offset : décalage plus ou moins grand de la SP par rapport à HEAD
    // Voir PriseObjetControlleur 



    void Update()
    {
        periphericSphereTrans.transform.position = periphericMove(); // déplacement de la sphère selon les calculs effectués
        //Debug.Log("OffsetFromCommandesSpheres: "+ facteurOffSet);
    }

    Vector3 periphericMove()
    {
        // calcul la position de la SP en fonction de la distance SC-SU
        Vector3 offSet = calculOffset();
        // ajout de du facteur d'offSet choisi par l'utilisateur
        Vector3 nouvellePosition = new Vector3(headTransform.position.x + offSet.x * facteurOffSet.offSet, headTransform.position.y + offSet.y * facteurOffSet.offSet, headTransform.position.z + offSet.z * facteurOffSet.offSet);
        return nouvellePosition;
    }

    Vector3 calculOffset()
    {
        //calcul SC-SU
        Vector3 offSet = new Vector3(
            commandSphereTrans.position.x - userSphereTrans.position.x,
            commandSphereTrans.position.y - userSphereTrans.position.y,
            commandSphereTrans.position.z - userSphereTrans.position.z);
        // Debug.Log("OffSet = X: " + offSet.x + "  Y=: " + offSet.y + "  Z=  " + offSet.z);
        return offSet;
    }

}
