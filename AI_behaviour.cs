using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI_behaviour : MonoBehaviour
{
    public int numRays = 36; // Number of rays to cast in a circle
    public float maxRayDistance = 10f;
    public float[][] floatInputVectors = new float[36][];
    public string foodTag = "Food"; // Tag of the food object
    private Color hitPlayerColor = Color.red;
    private Color hitFoodColor = Color.green;
    private Color hitNothingColor = Color.grey;

    public GameObject creature;
    public bool canEat = true;
    public bool mutateMutations = true;
    public float FB = 0;
    public float LR = 0;
    public int numberOfChildren = 1;
    private bool isMutated = false;
    float elapsed = 0f;
    public float lifeSpan = 0f;
    public bool isDead = false;

    public float maxHealth = 100f; // Maximum health value
    public float currentHealth = 100f; // Current health value
    public int attackAmount;
    private float previousHealth; 

    public float maxEnergy = 100f; // Maximum health value
    public float currentEnergy = 100f; // Current health value
    private float energyGained = 25f; //how much they gain from eating Food
    private float previousEnergy; 
    

    public float mutationAmount = 0.8f;
    public float mutationChance = 0.2f; 
    public NN nn;
    public Movement movement;
    public ObjectTracker tracker;
    List<double[]> inputVectors = new List<double[]>();


    private Collider2D hostCollider; // Reference to the host object's collider
    void Awake()
    {
        
        // Initialize previousHealth to see if health changed at all
        previousHealth = currentHealth;
        previousEnergy = currentEnergy;


        nn = gameObject.GetComponent<NN>();
        movement = gameObject.GetComponent<Movement>();
        tracker = gameObject.GetComponent<ObjectTracker>();


        this.name = "Agent";


        hostCollider = GetComponent<Collider2D>(); // Assuming the collider is attached to the same GameObject

    }
    void FixedUpdate()
    {
        
        
        // Check if the energy has changed
        if (currentEnergy != previousEnergy)
        {
            // Update the previous health value
            previousEnergy = currentEnergy;
        }
        //only do this once
        if(!isMutated)
        {
            //call mutate function to mutate the neural network
            MutateCreature();
            this.transform.localScale = new Vector3(1,1,1); // possible idea for changing size
            isMutated = true;
            currentEnergy = maxEnergy;
        }

        ManageEnergy();

        Rays_update();
        // Flatten the floatInputVectors array of arrays into a single-dimensional array
        float[] flattenedInput = floatInputVectors.SelectMany(innerArray => innerArray).ToArray();

        float[] outputsFromNN = nn.Brain(flattenedInput);

        FB = outputsFromNN[0];
        LR = outputsFromNN[1];

        movement.Move(FB, LR);
    }


    void OnTrigger2D(Collider2D col)
    {
        // Check if the collision involves the object you want to decrease the variable.
        if (col.gameObject.CompareTag(foodTag) && canEat)
        {
            Debug.Log("it works");
            currentEnergy += energyGained;
            
            Destroy(col.gameObject);
        }

    
    }

    void Rays_update()
    {
        // Define the number of object classes, e.g., food and player
        int numObjectClasses = 3;

            

        for (int i = 0; i < numRays; i++)
        {
            float angle = i * 360f / numRays;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            Vector3 rayStart = hostCollider.bounds.center + (Vector3)direction * (hostCollider.bounds.extents.x + 0.01f);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, maxRayDistance);

            Vector3 rayEnd = hit.collider ? hit.point : (Vector2)rayStart + direction * maxRayDistance;

            // the "1" is for the length of the ray
            double[] inputVector = new double[1 + numObjectClasses]; // Input vector

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(foodTag))
                {
                    Debug.DrawLine(rayStart, rayEnd, hitFoodColor);
                    // Use the length of the raycast as the distance to the food object
                    inputVector[0] = hit.distance / maxRayDistance; // Distance
                    inputVector[1] = 1.0; // Food identification
                }
                else if (hit.collider.CompareTag("Creature"))
                {
                    Debug.DrawLine(rayStart, rayEnd, hitPlayerColor);

                    inputVector[0] = hit.distance / maxRayDistance; // Distance
                    inputVector[2] = 1.0; // Creature identification
                }
                else 
                {
                    Debug.DrawLine(rayStart, rayEnd, hitPlayerColor);

                    inputVector[0] = hit.distance / maxRayDistance; // Distance
                    inputVector[3] = 1.0; // enemy identification
                }
            }
            else
            {
                Debug.DrawLine(rayStart, rayEnd, Color.white);
                inputVector[0] = 1.0; // Maximum distance (normalized)
            }

            // Store the input vector for this ray in the 'inputVectors' list or array.
            // You can use a list or array to collect all input vectors for later processing with your neural network.
            //inputVectors[i] = inputVector;

            // Convert the 'inputVector' from double to float
            floatInputVectors[i] = System.Array.ConvertAll(inputVector, x => (float)x);
                
        }
    }

    void ManageEnergy()
    {
        elapsed += Time.deltaTime;
        lifeSpan += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;

            // Subtract 1 energy per second
            currentEnergy -= 1f;
            

        }

        // Starve
        if (currentEnergy <= 0)
        {           
            Destroy(this.gameObject,3); 
            GetComponent<Movement>().enabled = false;
        }
    }
    void MutateCreature()
    {
        if(mutateMutations)
        {
            mutationAmount += Random.Range(-1.0f, 1.0f)/100;
            mutationChance += Random.Range(-1.0f, 1.0f)/100;
        }

        //make sure mutation amount and chance are positive using max function
        mutationAmount = Mathf.Max(mutationAmount, 0);
        mutationChance = Mathf.Max(mutationChance, 0);

        nn.MutateNetwork(mutationAmount, mutationChance);
    }
    public void Reproduce()
    {
        
        if (currentEnergy >= 25f)
        {
            numberOfChildren = 2;
        }
            
        else
        {
            numberOfChildren = 1;
        }
            
        for (int j = 0; j < numberOfChildren; j++)
        {
            // Ensure agentList[i] is not null and it has a valid transform
            if (this.gameObject != null && this.transform != null)
            {
                // Ensure creature and NN component are assigned
                if (creature != null && GetComponent<NN>() != null)
                {
                    // Instantiate child GameObject
                    GameObject child = Instantiate(creature, new Vector3(
                        this.transform.position.x + Random.Range(-10, 11),
                        this.transform.position.y + Random.Range(-10, 11),
                        0), Quaternion.identity);

                                
                    //copy the parent's neural network to the child
                    child.GetComponent<NN>().layers = GetComponent<NN>().copyLayers();

                }
                else
                {
                    Debug.LogWarning("Creature or NN component is null.");
                }
            }
        }

        Destroy(gameObject, 3); 
        GetComponent<Movement>().enabled = false;


    }
}
