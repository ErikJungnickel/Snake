using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

/// <summary>
/// Author: Erik Jungnickel - http://backyard-dev.de
/// </summary>
public class SnakeController : MonoBehaviour
{
    public GameObject partPrefab;
    public GameObject foodPrefab;
    public GameObject mapCubePrefab;
    public GameObject floorTilePrefab;

    //this basically controls the speed of the snake
    public float frameLength = 0.5f;
    float timeSinceLastFrame = 0;

    //food spawn rate
    public float numFramesForFoodSpawn = 5;
    float numFrames;

    List<GameObject> parts;
    List<GameObject> foods;

    Vector3 dir;
    Vector3 actualDir;

    int score = 0;
    public Text scoreText;

    public int MapWidth = 40;
    public int MapHeight = 20;

    bool paused = false;

    // Use this for initialization
    void Start()
    {
        CreateMap();

        numFrames = numFramesForFoodSpawn; //Spawn an initial food

        parts = new List<GameObject>();
        foods = new List<GameObject>();

        GameObject head = (GameObject)Instantiate(partPrefab, Vector3.zero, Quaternion.identity);
        parts.Add(head);

        dir = Vector3.up;
        actualDir = dir;

        Grow();
        Grow();

        //Register for input events
        InputController inputCtrl = GameObject.FindObjectOfType<InputController>();
        inputCtrl.dirChanged += inputCtrl_dirChanged;
        inputCtrl.stateChanged += inputCtrl_stateChanged;
    }

    /// <summary>
    /// This will just create the map boundaries and the floor tiles.
    /// If this was a more complex game we should move this to a seperate class.
    /// </summary>
    void CreateMap()
    {
        for (int x = -MapWidth / 2; x <= MapWidth / 2; x++)
        {
            Instantiate(mapCubePrefab, new Vector3(x, -MapHeight / 2, 0), Quaternion.identity);
            Instantiate(mapCubePrefab, new Vector3(x, MapHeight / 2, 0), Quaternion.identity);
            for (int y = -MapHeight / 2; y < MapHeight / 2; y++)
            {
                Instantiate(floorTilePrefab, new Vector3(x, y, floorTilePrefab.transform.position.z), Quaternion.identity);
            }
        }
        for (int y = -MapHeight / 2; y < MapHeight / 2; y++)
        {
            Instantiate(mapCubePrefab, new Vector3(-MapWidth / 2, y, 0), Quaternion.identity);
            Instantiate(mapCubePrefab, new Vector3(MapWidth / 2, y, 0), Quaternion.identity);
        }

    }

    void inputCtrl_dirChanged(Vector3 dir)
    {
        if ((this.actualDir + dir) == Vector3.zero)
            return;
        this.dir = dir;
    }

    void inputCtrl_stateChanged(bool paused)
    {
        this.paused = paused;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (parts != null && parts.Count > 0) //make sure everything is setup
            {
                timeSinceLastFrame += Time.deltaTime;
                if (timeSinceLastFrame >= frameLength)
                {
                    timeSinceLastFrame = 0;
                    numFrames++;

                    MoveSnake();

                    if (CheckCollision(parts[0].transform.position))
                    {
                        //Gameover
                        Thread.Sleep(2000);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //just reload the scene
                    }

                    //check for food "collisions". 
                    foreach (GameObject food in foods)
                    {
                        if (food.transform.position.Equals(parts[0].transform.position))
                        {
                            //Remove the food, grow the snake and increase the score
                            foods.Remove(food);
                            Destroy(food);
                            Grow();
                            score++;
                            scoreText.text = "Score: " + score;
                            break;
                        }
                    }
                }
                if (numFrames >= numFramesForFoodSpawn)
                {
                    numFrames = 0;

                    SpawnFood();
                }
            }
        }
    }

    /// <summary>
    /// Moves the snake in the desired direction.
    /// </summary>
    void MoveSnake()
    {
        for (int i = parts.Count - 1; i > 0; i--)
        {
            GameObject prev = parts[i - 1];
            parts[i].transform.position = prev.transform.position;
        }

        parts[0].transform.position += dir;

        actualDir = dir;
    }

    /// <summary>
    /// Get a random position and spawn food
    /// </summary>
    void SpawnFood()
    {
        int x = Random.Range(-MapWidth / 2 + 1, MapWidth / 2 - 1);
        int y = Random.Range(-MapHeight / 2 + 1, MapHeight / 2 - 1);

        GameObject food = (GameObject)Instantiate(foodPrefab, new Vector3(x, y, 0), Quaternion.identity);
        foods.Add(food);
    }

    /// <summary>
    /// Checks collisions with the map boundaries and the snake
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool CheckCollision(Vector3 pos)
    {
        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].transform.position.Equals(pos))
                return true;
        }

        if (pos.x >= MapWidth / 2 || pos.x <= -MapWidth / 2
            || pos.y >= MapHeight / 2 || pos.y <= -MapHeight / 2)
            return true;

        return false;
    }

    /// <summary>
    /// Adds a new part to the end of the snake
    /// </summary>
    void Grow()
    {
        Vector3 newPos;
        if (parts.Count == 1)
        {
            newPos = parts[0].transform.position - dir;
            GameObject newPart = (GameObject)Instantiate(partPrefab, newPos, Quaternion.identity);
            parts.Add(newPart);
        }
        else
        {
            GameObject lastPart = parts[parts.Count - 1];
            GameObject lastPartMinusOne = parts[parts.Count - 2];

            Vector3 diff = lastPart.transform.position - lastPartMinusOne.transform.position;

            newPos = lastPart.transform.position + diff;
            GameObject newPart = (GameObject)Instantiate(partPrefab, newPos, Quaternion.identity);
            parts.Add(newPart);
        }
    }
}
