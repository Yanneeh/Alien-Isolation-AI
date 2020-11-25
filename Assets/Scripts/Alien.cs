using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alien : MonoBehaviour
{
    private GameObject player;
    private string state = "Idle";
    private Vector3 targetPosition = Vector3.zero;
    private bool seenPlayer;
    private float soundMagnitude;
    private float time = 0;
    private float seenCounter = 5;

    public NavMeshAgent navMeshAgent;
    public int senseTime = 60;
    public int attackRange = 1;
    public int randomRange = 50;
    public int soundTreshold = 10;



    public Vector3 getRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int t = Random.Range(0, navMeshData.indices.Length - 3);

        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        point = Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;

    }
    
    // Perceptions.

    void look()
    {

        // Raycast stuff.

        seenPlayer = false;

    }

    void listen()
    {
        // Sphere stuff.

        soundMagnitude = 0f;

    }

    void sensePlayerPosition()
    {
        time += Time.deltaTime;

        if (time > senseTime)
        {
            Debug.Log("Sensing player");
            targetPosition = player.transform.position;

            time = 0;
        }
    }

    void chooseRandomPosition()
    {
        //targetPosition = Random.insideUnitSphere * randomRange;
        //targetPosition.y = transform.position.y;

        //NavMeshPathStatus status = navMeshAgent.pathStatus;

        //Debug.Log(status);

        targetPosition = getRandomLocation();

    }

    // Actions.
    void move()
    {
        // Navmesh set destination.
        navMeshAgent.SetDestination(targetPosition);
    }

    void attack()
    {

    }


    // State machine.

    void changeState()
    {
        switch (state)
        {
            case "Idle":
                if (targetPosition != Vector3.zero)
                {
                    state = "Search";
                    break;
                }

                if (seenPlayer || soundMagnitude > soundTreshold)
                {
                    state = "Chase";
                    break;
                }       

                break;

            case "Search":

                if ((targetPosition - transform.position).magnitude < 2f)
                {
                    state = "Idle";
                    break;
                }

                if (seenPlayer || soundMagnitude > soundTreshold)
                {
                    state = "Chase";
                    break;
                }

                break;

            case "Chase":
                if ((player.transform.position - transform.position).magnitude < attackRange)
                {
                    state = "Attack";
                    break;
                }

                if (!seenPlayer || soundMagnitude < soundTreshold)
                {
                    state = "Chase";
                    break;
                }

                break;

            case "Attack":
                if (player == null)
                {
                    state = "Idle";
                    break;
                }
                break;
        }
    }

    void executeState()
    {

        Debug.Log(state);

        //Debug.Log(transform.position);
        //Debug.Log(targetPosition);

        switch (state)
        {
            case "Idle":
                chooseRandomPosition();
                Debug.Log(targetPosition);
                break;
            case "Search":
                move();
                break;
            case "Chase":
                targetPosition = player.transform.position + 5 * Random.insideUnitSphere;
                move();
                break;
            case "Attack":
                attack();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        sensePlayerPosition();
        look();
        listen();

        changeState();
        executeState();
    }
}
