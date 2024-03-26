using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


// this code is a bit janky and could be optimized.
//This class is optional and is what I was using to make the graphs in the simulation
public class ObjectTracker : MonoBehaviour  
{
    public GameObject[] foodList;
    public GameObject[] agentList;

    private int currentRound = 0;    
    private float time = 0f;
    private float RoundLength = 10f;

    private string logFilePath = "Assets/simulation_log.txt"; // Change path if necessary
    private int[] info = new int[2];

    // Initialize the round counter text
    void Start()
    {

        IncrementRound();
    }

    // Update the round counter text
    void UpdateRoundCounter()
    {
        info[0] = foodList.Length;
        info[1] = agentList.Length;
        // Calculate average lifespan
        float totalLifespan = 0f;
        foreach (float lifespan in Creature.creatureLifespans)
        {
            totalLifespan += lifespan;
        }

        float averageLifespan = 0f;
        if (Creature.creatureLifespans.Count > 0)
        {
            averageLifespan = totalLifespan / Creature.creatureLifespans.Count;
        }
        using (StreamWriter writer = File.AppendText(logFilePath))
        {
            writer.WriteLine("iteraton:" + currentRound);
            writer.WriteLine("Logging data for current round: Food = " + info[0] + " Agents = " + info[1] + " Average life span = " + averageLifespan);
        }
        
        if (currentRound == 1 ||
            currentRound == 10 ||
            currentRound == 100 ||
            currentRound == 250 ||
            currentRound == 500 ||
            currentRound == 1000 ||
            currentRound == 2500 ||
            currentRound == 5000 ||
            currentRound == 10000 ||
            currentRound == 20000)
        {
            Debug.Log("Iteration: " + currentRound); 
            // Log data or perform actions based on the value of currentRound
            Debug.Log("Logging data for current round: Food = " + info[0] + " Agents = " + info[1] + " Average life span = " + averageLifespan);
        }
        
    }

    // Increment the round
    public void IncrementRound()
    {
        currentRound++;
        foodList = GameObject.FindGameObjectsWithTag("Food");
        agentList = GameObject.FindGameObjectsWithTag("Agent");
        UpdateRoundCounter();
    }

    private void FixedUpdate()
    {
        time+=Time.fixedDeltaTime;

        //if time save data point
        if(time >= RoundLength)
        {
            time = 0f;



            IncrementRound();
            
             
        }
    }
}
