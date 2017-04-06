using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pour une meilleure compréhension des calculs, se reporter à l'annexe du document d'Etat d'avancement

public class GenerationSourcesSonores1 : MonoBehaviour
{

    public GameObject Source; // Référence au prefab SourceSonore
    public int nombreDeLigne; // nombre de lignes de source sonores
    public int rayonSphere; // rayon de la dimeSphere
    public AudioClip Clip; // Réfrence vers le clip sonore
    private List<GameObject> ListeSource; // Contient l'ensemble des sources sonores qui seront créées
    private Vector3[] Coord; // Coordonnées de chaque source sonore
    private GvrAudioSource As; // Déclaration d'un objet AudioSource Google VR  * private  *
    private int number; // numéro de la source active
    private int nbErreur;
    private float tempsLancement;
    private float [] tempsInterCible;
    public int nbIteration;
    GameObject LastCollidingObject;

    void Start()
    {
        // Instanciation des sources sonores
        LastCollidingObject = null;
        tempsInterCible = new float[nbIteration];
        ListeSource = new List<GameObject>();
        for (int y = 0; y < nombreDeLigne * 3 + 1; y++)
        {
            Instantiate(Source, new Vector3(0, 0, y), Quaternion.identity, this.transform);
        }
        // Chaque source a pour parent l'objet de référence ListeSource
        foreach (Transform child in transform)
        {
            ListeSource.Add(child.gameObject);
        }

        float[] P = CalculPhi();
        Coord = CalculCoord(P);
        rotationSource();
                int j = 0;
        foreach (GameObject g in ListeSource)
        {
            g.transform.position = Coord[j];
            j++;
        }
        activationAuSort();
        nbErreur = 0;
    }


    float[] CalculPhi()
    {
        // Calcul de la position angulaire phi horizontale des sources sonores autour de la demiSphere
        float[] phiCalculate = new float[nombreDeLigne];
        float angleDeg = 360 / nombreDeLigne;
        for (int i = 0; i < nombreDeLigne; i++) // Pour chaque ligne à générer, on calcul l'angle phi
        {
            phiCalculate[i] = angleDeg * i * (float)(Mathf.PI / 180);
        }
        return phiCalculate;
    }

    Vector3[] CalculCoord(float[] phi) // transformer en vector3
    {
        // Calcul des coordonnées angulaire verticale deta selon le rayon de la sphère
        float[] Deta = new float[3];
        Deta[0] = 3 * (float)Mathf.PI / 8;
        Deta[1] = 2 * (float)Mathf.PI / 8;
        Deta[2] = (float)Mathf.PI / 8;
        Vector3[] Coord = new Vector3[phi.Length * 3 + 1]; // Coord a pour taille le nombre de source à générer 
        Coord[0] = new Vector3(0, 9.99f, 0); // Source centrale ****
        int c = 0; // parcours le tableau d'angle deta
        int c2 = 0; // parcours le tableau d'angle phi 
        float x, y, z; // Coordonnées x,y,z
        x = y = z = 0;
        for (int i = 1; i < phi.Length * 3 + 1; i++) // i commence à un pour ne pas écraser Coord [0] 
        {
            // Pour chaque source on calcul les coordonnées x, y z, à partir de phi et deta
            x = rayonSphere * (float)(Mathf.Sin(Deta[c]) * Mathf.Cos(phi[c2]));
            y = rayonSphere * (float)Mathf.Cos(Deta[c]) - 0.05f; // décalage pour uniformisation des sources
            z = rayonSphere * (float)(Mathf.Sin(Deta[c]) * Mathf.Sin(phi[c2]));
            Coord[i] = new Vector3(x, y, z);
            c2 = c == Deta.Length - 1 ? c2 + 1 : c2; // c2 augmente si on a pas parcouru l'ensemble des angle Deta, il revient à 0 sinon (nouvelle ligne)
            c = c == Deta.Length - 1 ? 0 : c + 1; // c reviens à 0 quand on a terminer une ligne
        }
        Coord[0] = new Vector3(0, rayonSphere, 0); // source centrale *****
        return Coord;
    }

    void rotationSource()
    {
        // Permet d'orienter les sources sonores de manières uniformes
        float angleCirconf; // Angle de rotation nécessaire pour que la source soit bien orientée
        for (int k = 0; k < nombreDeLigne / 2; k++)
        {
            if (k == 0) // Première ligne
            {
                angleCirconf = 0;
            }
            else if (k <= (nombreDeLigne / 2))
            {
                angleCirconf = -360 / nombreDeLigne * k; // Lignes situées avant pi/2 par rapport à la ligne centrale (sens trigonométrique)
            }
            else
            {
                angleCirconf = 360 / nombreDeLigne * k; // Ligne situées après pi/2 par rapport à la ligne centrale (sens trigonométrique)
            }
            k = k * 3;// permet de passer de lignes en lignes et de couvrir l'ensemble des sources
            ListeSource[k + 1].transform.rotation = ListeSource[k + 1].transform.rotation;
            ListeSource[k + 1].transform.Rotate(0, angleCirconf, -67.5f); // angle source basse
            ListeSource[k + 2].transform.Rotate(0, angleCirconf, -45f); // angle source moyenne
            ListeSource[k + 3].transform.Rotate(0, angleCirconf, -22.5f); // angle source haute
            // Les lignes de la moitié trigonométrique opposée sont calculés en même temps
            ListeSource[(k + 1) + nombreDeLigne / 2 * 3].transform.Rotate(0, angleCirconf, 67.5f);
            ListeSource[(k + 2) + nombreDeLigne / 2 * 3].transform.Rotate(0, angleCirconf, 45f);
            ListeSource[(k + 3) + nombreDeLigne / 2 * 3].transform.Rotate(0, angleCirconf, 22.5f);
            k = k / 3; // on remet le compteur à sa valeur normale
        }
    }

    // Les coroutines sont nécessaire pour activer et desactiver la lecture de la piste audio
    // après une pause de deux secondes. De plus, ce système règle un problème
    // de pistes continuées à être jouer alors que la source est inactive

    void activationAuSort()
    {
        StopAllCoroutines();// On arrête la coroutine potentiellement lancée
        // activation au sort d'une source sonore
        number = Random.Range(0, nombreDeLigne * 3 + 1); // choix aléatoire de la source à activer
        As = ListeSource[number].GetComponent<GvrAudioSource>(); // Accès à l'objet GVRAudioSource de la source choisie
        As.PlayOneShot(this.Clip);// Lecture
        ListeSource[number].GetComponent<Renderer>().material.color = Color.green; // Coloration en vert (en mode entrainement)
        ListeSource[number].tag = "CIBLE"; // Identification de la source à ciblé
        tempsLancement = Time.time;
        nbIteration--;
    }

    public bool toucherCible(GameObject collidingObject)
    {
        // Détecte si la cible touchée par le laser est l'active
        if (collidingObject == ListeSource[number]) // Si c'est le cas
        {
            Debug.Log("taille tab =" + tempsInterCible.Length + " nbiteration = " + nbIteration);

            tempsInterCible[tempsInterCible.Length-(nbIteration+1)] = Time.time - tempsLancement;
            Debug.Log(tempsInterCible);
            ListeSource[number].GetComponent<Renderer>().material.color = Color.green; // On indique à l'utilisateur qu'il a juste (mode test), coloration verte de la sphère
            //Debug.Log("CIBLE TOUCHE");
            StartCoroutine(desactivationSource()); // desactivation après deux secondes
            return true;
        }
        else
        {
            if (collidingObject.tag == "NOCIBLE" && collidingObject!=LastCollidingObject)
            {
                LastCollidingObject = collidingObject;
                nbErreur++;
            }
            foreach (GameObject g in ListeSource) // sinon erreur, on colore la sphère en rouge pour indiquer une erreur
            {
                if (g == collidingObject)
                    g.GetComponent<Renderer>().material.color = Color.red;
            }
            // Debug.Log("PAS TOUCHE");
            return false;
        }
    }

    public IEnumerator desactivationSource() // desactivation de la source active après un délai de 2s
    {
        As.Stop(); // On arrête la piste audio
        yield return new WaitForSeconds(2); // pause
        foreach (GameObject g in ListeSource)
        {
            g.GetComponent<Renderer>().material.color = Color.white; // On réinitialise la couleur de l'ensemble des sources

        }
        ListeSource[number].tag = "NOCIBLE";
        if (nbIteration > 0)
        {
            activationAuSort();// activation d'une nouvelle source sonore
        }
        else
        {
            afficheTemps();
        }
    }

    public void afficheTemps(){
        for (int i  = 0; i  <tempsInterCible.Length; i ++)
        {
            Debug.Log(tempsInterCible[i]);
            Debug.Log(nbErreur);
        }
    }


}
