using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public bool mutateMutations = true;
    public GameObject agentPrefab;
    public bool isUser = false;
    public bool canEat = true;
    public float viewDistance = 20;
    public float size = 1.0f;
    public float energy = 20;
    public float energyGained = 10;
    public float reproductionEnergyGained = 1;
    public float reproductionEnergy = 0;
    public float reproductionEnergyThreshold = 10;
    public float FB = 0;
    public float LR = 0;
    public int numberOfChildren = 1;
    private bool isMutated = false;
    float elapsed = 0f;
    public float lifeSpan = 0f;
    public static List<float> creatureLifespans = new List<float>();

    public float[] distances = new float[6];

    public float mutationAmount = 0.8f;
    public float mutationChance = 0.2f; 
    public NN nn;
    public Movement movement;

    float relativeFoodX;
    float relativeFoodZ;

    private List<GameObject> edibleFoodList = new List<GameObject>();

    public bool isDead = false;

    void Awake()
    {
        nn = gameObject.GetComponent<NN>();
        movement = gameObject.GetComponent<Movement>();
        distances = new float[17];

        this.name = "Agent";
    }
    public static void ResetLifespans()
    {
        creatureLifespans.Clear();
    }
    void FixedUpdate()
    {
        //only do this once
        if(!isMutated)
        {
            //call mutate function to mutate the neural network
            MutateCreature();
            this.transform.localScale = new Vector3(size,size,size);
            isMutated = true;
            energy = 20;
        }

        ManageEnergy();

        // This group of comments is for the old food detection system
        // //use the FindClosestFood function to find the closest food object
        // GameObject closestFood = FindClosestFood();

        // //if food is found, set the relative x and z coordinates of the food to the agent
        // if (closestFood != null)
        // {
        //         relativeFoodX = this.transform.position.x - closestFood.transform.position.x;
        //         relativeFoodZ = this.transform.position.z - closestFood.transform.position.z;
        // }

        // //get the angle between the agents local rotation and the food
        // float angle = Vector3.SignedAngle(transform.forward, new Vector3(relativeFoodX, 0, relativeFoodZ), Vector3.up);

        // //get the distance between the agent and the food
        // float distance = Mathf.Sqrt((Mathf.Pow(relativeFoodX, 2) + Mathf.Pow(relativeFoodZ, 2)));

        // //Setup inputs for neural network
        // //get the global rotation of the agent
        // float [] inputsToNN = {relativeFoodX, relativeFoodZ, transform.rotation.eulerAngles.y};

        //float [] inputsToNN ={(angle), distance/50};


        // This section of code is for the new food detection system (Raycasts)
        // Set up a variable to store the number of raycasts to use
        int numRaycasts = 16;

        // Set up a variable to store the angle between raycasts
        float angleBetweenRaycasts = 15;

        // Use multiple raycasts to detect food objects
        RaycastHit hit;
        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = ((2 * i + 1 - numRaycasts) * angleBetweenRaycasts / 2);
            // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 rayDirection = rotation * transform.forward * -1;
            // Increase the starting point of the raycast by 0.1 units
            Vector3 rayStart = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(rayStart, rayDirection, out hit, viewDistance))
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red);
                if (hit.transform.gameObject.tag == "Food")
                {
                    // Use the length of the raycast as the distance to the food object
                    distances[i] = hit.distance/viewDistance;
                }
                else
                {
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[i] = 1;
                }
            }
            else
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(rayStart, rayDirection * viewDistance, Color.red);
                // If no food object is detected, set the distance to the maximum length of the raycast
                distances[i] = 1;
            }
        }

        // Setup inputs for the neural network
        float [] inputsToNN = distances;

        // Get outputs from the neural network
        float [] outputsFromNN = nn.Brain(inputsToNN);

        //Store the outputs from the neural network in variables
        FB = outputsFromNN[0];
        LR = outputsFromNN[1];

        //if the agent is the user, use the inputs from the user instead of the neural network
        if (isUser)
        {
            FB = Input.GetAxis("Vertical");
            LR = Input.GetAxis("Horizontal")/10;
        }

        //Move the agent
        movement.Move(FB, LR);
    }

    //this function gets called whenever the agent collides with a trigger. (Which in this case is the food)
    void OnTriggerEnter(Collider col)
    {
        Transform parentObject = col.gameObject.transform.parent; 

        //if the agent collides with a food object, it will eat it and gain energy.
        if (col.gameObject.tag == "Food" && canEat)
        {
            energy += energyGained;
            reproductionEnergy += reproductionEnergyGained;
            Destroy(parentObject.gameObject);
        }
    }

    public void ManageEnergy()
    {
        elapsed += Time.deltaTime;
        lifeSpan += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;

            //subtract 1 energy per second
            energy -= 1f;

            //if agent has enough energy to reproduce, reproduce
            if (reproductionEnergy >= reproductionEnergyThreshold)
            {
                reproductionEnergy = 0;
                Reproduce();
            }
        }

        //Starve
        float agentY = this.transform.position.y;
        if (energy <= 0 || agentY < -10)
        {
            // Add the lifespan of the creature to the list
            creatureLifespans.Add(lifeSpan);

            this.transform.Rotate(0, 0, 180);
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 3.5f, this.transform.position.z);
            Destroy(this.gameObject,3);
            GetComponent<Movement>().enabled = false;
        }

    }

    private void MutateCreature()
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
        //replicate
        for (int i = 0; i< numberOfChildren; i ++) // I left this here so I could possibly change the number of children a parent has at a time.
        {
            //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
            GameObject child = Instantiate(agentPrefab, new Vector3(
                (float)this.transform.position.x + Random.Range(-10, 11), 
                0.75f, 
                (float)this.transform.position.z+ Random.Range(-10, 11)), 
                Quaternion.identity);
            
            //copy the parent's neural network to the child
            child.GetComponent<NN>().layers = GetComponent<NN>().copyLayers();
        }
        reproductionEnergy = 0;

    }
}
