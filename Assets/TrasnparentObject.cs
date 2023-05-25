using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;


public class TrasnparentObject : MonoBehaviour
{

    public GameObject object1;
    public GameObject object2;
    public float transparency = 0.5f;
    public float maxDistance = 10f;

    [SerializeField] private Collider[] colliders;
    [SerializeField] private Renderer[] renderers;

    private void Start()
    {
        colliders = new Collider[100];
        renderers = new Renderer[100];
    }

    private void Update()
    {


        // Calculate the distance between the two objects
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);

        // Check only the objects within a certain distance range of the two specified objects
        int numColliders = Physics.OverlapSphereNonAlloc(object2.transform.position, maxDistance, colliders);
        int numRenderers = 0;
        for (int i = 0; i < numColliders; i++)
        {
            Renderer renderer = colliders[i].GetComponent<Renderer>();
            
            if (renderer != null)
            {
                // Check if the object is between the two specified objects
                Vector3 objectPosition = renderer.transform.position;
                float distanceToObject2 = Vector3.Distance(objectPosition, object2.transform.position);

                if (distanceToObject2 <= distance + 0.1f)
                {
                    // The object is between the two specified objects, so add it to the array of renderers
                    renderers[numRenderers] = renderer;
                    numRenderers++;
                }
            }
        }

        //Debug.Log("Numero ", renderers);

        // Loop through all the renderers that are between the two specified objects
        for (int i = 0; i < numRenderers; i++)
        {
            Renderer renderer = renderers[i];
            TrasnparencyController trasnparency = renderer.gameObject.GetComponent<TrasnparencyController>();
            if (trasnparency == null) {
                break;
            }

            if (renderer != null && renderer.gameObject != object1 && renderer.gameObject != object2)
            {
                foreach (Material material in renderer.materials)
                {
                    // Check if the material has a color property
                    if (material.HasProperty("_Color") )
                    {
                       
                        //Color color = material.color;
                        //// Set the alpha value of the color to the transparency value
                        //color.a = trasnparency.transparencyData.normalTransparency;
                        //material.color = color;
                        trasnparency.isBetween = false;

                       // Debug.Log("bateu");
                    }
                }
                if (renderer.gameObject != object1 && renderer.gameObject != object2)
                {
                    Debug.Log(renderer.gameObject.name);
                }
            }
        }


         

        // Cast a ray between the two objects and get all the colliders that intersect with the ray
        Ray ray = new Ray(object1.transform.position, object2.transform.position - object1.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray, distance);

        // Loop through all the colliders that were hit by the raycast
        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            TrasnparencyController trasnparency = hit.collider.gameObject.GetComponent<TrasnparencyController>();

            if (trasnparency == null) {
                Debug.Log("TransparancyController not found in: " + hit.collider.gameObject.name);
            }


            if (renderer != null)
            {
                Material material = renderer.material;

                // Check if the material has a color property
                if (material.HasProperty("_Color") && hit.collider.gameObject != object1 && hit.collider.gameObject != object2 && trasnparency != null)
                {
                   // Debug.Log(hit.collider.gameObject.name);

                    trasnparency.isBetween = true;
                    
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        // Draw the ray between object1 and object2
        Gizmos.color = Color.green;
        Gizmos.DrawLine(object1.transform.position, object2.transform.position);

        // Draw a sphere gizmo representing the number of colliders
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(object2.transform.position, maxDistance);
        //Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        //Gizmos.DrawSphere(object2.transform.position, maxDistance);
    }
}
