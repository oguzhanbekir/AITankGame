using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AForge.Fuzzy;
using System;

public class enemyAI : MonoBehaviour
{
    float distance2, speed2;
    Transform player;
    NavMeshAgent agent;
    public Transform[] wayPoints;
    Vector3[] wayPointPos = new Vector3[3];
    int currentWayPointIndex=0;
    public Transform rayOrigin;
    Animator fsm;
    float shootFreq=10f;
    // Start is called before the first frame update

    //distance
    FuzzySet fsNear, fsMed, fsFar;
    LinguisticVariable lvDistance;

    //speed
    FuzzySet fsSlow, fsMedium, fsFast;
    LinguisticVariable lvSpeed;

    Database database;
    InferenceSystem infSystem;


    void Start()
    {

        for(int i = 0; i < wayPointPos.Length; i++)
        {
            wayPointPos[i] = wayPoints[i].position;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fsm = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(wayPointPos[currentWayPointIndex]);

        StartCoroutine("CheckPlayer");

        Initialize();
    }
    public void Initialize()
    {
        SetDistanceFuzzySets();
        SetSpeedFuzzySets();
        AddToDataBase();
    }

    private void AddToDataBase()
    {
        database = new Database();
        database.AddVariable(lvDistance);
        database.AddVariable(lvSpeed);

        infSystem = new InferenceSystem(database, new CentroidDefuzzifier(120));
        infSystem.NewRule("Rule 1", "IF Distance IS Near THEN Speed IS Slow");
        infSystem.NewRule("Rule 2", "IF Distance IS Med THEN Speed IS Medium");
        infSystem.NewRule("Rule 3", "IF Distance IS Far THEN Speed IS Fast");

    }

    private void SetSpeedFuzzySets()
    {
        fsSlow = new FuzzySet("Slow", new TrapezoidalFunction(30, 50, TrapezoidalFunction.EdgeType.Right));
        fsMed = new FuzzySet("Medium", new TrapezoidalFunction(30, 50, 80, 100));
        fsFast = new FuzzySet("Fast", new TrapezoidalFunction(80, 100, TrapezoidalFunction.EdgeType.Left));
        lvSpeed = new LinguisticVariable("Speed", 0, 120);
        lvSpeed.AddLabel(fsSlow);
        lvSpeed.AddLabel(fsMed);
        lvSpeed.AddLabel(fsFast);
    }

    private void SetDistanceFuzzySets()
    {
        fsNear = new FuzzySet("Near",new TrapezoidalFunction(20,40,TrapezoidalFunction.EdgeType.Right));
        fsMed = new FuzzySet("Med", new TrapezoidalFunction(20, 40, 50, 70));
        fsFar = new FuzzySet("Far", new TrapezoidalFunction(50, 70, TrapezoidalFunction.EdgeType.Left));
        lvDistance = new LinguisticVariable("Distance", 0, 100);
        lvDistance.AddLabel(fsNear);
        lvDistance.AddLabel(fsMed);
        lvDistance.AddLabel(fsFar);
        
    }


    private void Evaluate()
    {
        Vector3 dir = (player.position - transform.position);
        distance2 = dir.magnitude;
        dir.Normalize();
        infSystem.SetInput("Distance", distance2);
        speed2 = infSystem.Evaluate("Speed");
        transform.position += dir * speed2 * Time.deltaTime*0.2f;
        agent.SetDestination(player.position);
    }

    IEnumerator CheckPlayer()
    {
        CheckDistance();
        CheckVisibility();
        CheckDistanceFromCurrentWaypoint();
        
        yield return new WaitForSeconds(0.1f);
        yield return CheckPlayer();
    }
    private void CheckDistanceFromCurrentWaypoint()
    {
        float distance = Vector3.Distance(wayPointPos[currentWayPointIndex], rayOrigin.position);

        fsm.SetFloat("distanceFromCurrentWayPoint", distance);
    }

    private void CheckDistance()
    {
        // float distance = (player.position - transform.position).magnitude;
        float distance = Vector3.Distance(player.position, rayOrigin.position);
 

        fsm.SetFloat("distance", distance);
    }

    private void CheckVisibility()
    {
       // Debug.Log("Test");
        float maxDistance=20;
        Vector3 direction = (player.position - rayOrigin.position).normalized;
        Debug.DrawRay(rayOrigin.position, direction * maxDistance, Color.red);
        if (Physics.Raycast(rayOrigin.position, direction, out RaycastHit info, maxDistance))
        {
            if (info.transform.tag=="Player")
            {
                fsm.SetBool("isVisible", true);
            }   
            else
                fsm.SetBool("isVisible", false);
        }
        else
         fsm.SetBool("isVisible", false);

    }

    public void SetNewWayPoint()
    {
        switch (currentWayPointIndex)
        {
            case 0:
                currentWayPointIndex = 1;
                break;
            case 1:
                currentWayPointIndex = 2;
                break;
            case 2:
                currentWayPointIndex = 0;
                break;
        }
        agent.SetDestination(wayPointPos[currentWayPointIndex]);
    }

    public void Shoot()
    {
        GetComponent<shootBehaviour>().Shoot(shootFreq);
    }
    public void Chase()
    {
        Evaluate();
        
    }
    public void Patrol()
    {
        agent.SetDestination(wayPointPos[currentWayPointIndex]);
    }

    public void SetLookRotation()
    {
        Vector3 dir = (player.position - transform.position).normalized;

        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation,rotation,0.2f);
    }
   
}
