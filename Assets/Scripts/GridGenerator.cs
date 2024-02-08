using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class GridGenerator : MonoBehaviour
{
    public int gridSize = 100;
    private bool[,] grid;
    private GameObject[,] gridObjects;
    private float meshWidth;
    private float meshHeight;
    public GameObject basicFloorTile;
    public GameObject floorTile;
    public List<RandomWalker> randomWalkers;

    // Start is called before the first frame update
    void Start()
    {
        //Get MeshSizes
        meshWidth = basicFloorTile.GetComponent<MeshRenderer>().bounds.size.x;
        meshHeight = basicFloorTile.GetComponent<MeshRenderer>().bounds.size.z;
        //Create Grid
        gridSize = (int)Random.Range(25, 60);
        grid = new bool[gridSize, gridSize];
        gridObjects = new GameObject[gridSize, gridSize];
        //Create RandomWalker List
        randomWalkers = new List<RandomWalker>();

        //Visualize Grid
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = new Vector3(i * meshWidth, 0f, j * meshHeight);
                GameObject tile = Instantiate(basicFloorTile, position, Quaternion.identity, transform);
                gridObjects[i,j] = tile;
            }
        }

        //Guaranteed 5 Walkers
        randomWalkers.Add(new RandomWalker(0, gridSize/2, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize/2, 0, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize-1, gridSize/2, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize/2, gridSize-1, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize/2, gridSize/2, gridSize, grid));
        
        for(int i = 31; i  <= gridSize; i+= 10)
        {
            int randomX = (int)(Random.value * gridSize);
            int randomY = (int)(Random.value * gridSize);
            randomWalkers.Add(new RandomWalker(randomX, randomY, gridSize, grid));

        }

        bool finishedSteps = true;
        while(finishedSteps)
        {
            //Move the walkers
            for (int i = randomWalkers.Count - 1; i >= 0; i--)
            {
                if (randomWalkers[i].stepCount < 100 && randomWalkers[i] != null)
                { 
                    randomWalkers[i].moveOnGrid();
                    if (randomWalkers[i].stepCount == 51 && Random.value < 1.1f && randomWalkers.Count < gridSize)
                    {
                        randomWalkers.Add(new RandomWalker((int)randomWalkers[i].position.x, (int)randomWalkers[i].position.y, gridSize, grid));
                    }
                }
            }

            finishedSteps &= randomWalkers[randomWalkers.Count - 1].stepCount < 100;
        }

        //Update the grid
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] == true && gridObjects[i, j].tag != "Dungeon Floor")
                {
                    Vector3 tilePosition = gridObjects[i, j].transform.position;
                    Destroy(gridObjects[i, j]);
                    gridObjects[i, j] = Instantiate(floorTile, tilePosition, Quaternion.identity, transform);
                }
            }
        }

        randomWalkers.Clear();
    }

    // Update is called once per frame
    void Update()
    {
               
    }

    public class RandomWalker
    {
        public Vector2 position;
        public int gridSize;
        public bool[,] grid;
        public int stepCount;

        public RandomWalker(int inputX, int inputY, int SizeOfGrid, bool[,] gridObject)
        {
            gridSize = SizeOfGrid;
            grid = gridObject;
            position.x = inputX;
            position.y = inputY;
        }

        public void moveOnGrid()
        {
            float randomValue = Random.value;
            switch (randomValue)
            {
                case < 0.25f:
                    if (position.x - 1 < 0) { break; }
                    position.x--;
                    break;
                case < 0.5f:
                    if (position.x + 1 >= gridSize) { break; }
                    position.x++;
                    break;
                case < 0.75f:
                    if (position.y - 1 < 0) { break; }
                    position.y--;
                    break;
                case <= 1f:
                    if (position.y + 1 >= gridSize) { break; }
                    position.y++;
                    break;
            }

           stepCount++;
           ActivatePosition();
        }

        public void ActivatePosition()
        {
            grid[(int)position.x, (int)position.y] = true;
        }
    }
}
