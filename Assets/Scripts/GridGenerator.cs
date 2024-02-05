using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private RandomWalker randomWalker;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Get MeshSizes
        meshWidth = basicFloorTile.GetComponent<MeshRenderer>().bounds.size.x;
        meshHeight = basicFloorTile.GetComponent<MeshRenderer>().bounds.size.z;
        //Create Grid

        grid = new bool[gridSize, gridSize];
        gridObjects = new GameObject[gridSize, gridSize];

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

        randomWalker = new RandomWalker(gridSize);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0.5f && randomWalker != null)
        {
            randomWalker.moveOnGrid();
            randomWalker.ActivatePosition(grid);
            if(randomWalker.stepCount > 50)
            {
                randomWalker = null;
            }
            timer = 0;
        }

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i,j] == true && gridObjects[i,j].tag != "Dungeon Floor")
                {
                    Vector3 tilePosition = gridObjects[i, j].transform.position;
                    Destroy(gridObjects[i, j]);
                    gridObjects[i,j] = Instantiate(floorTile, tilePosition , Quaternion.identity, transform);
                }
            }
        }

        timer += Time.deltaTime;
    }

    public class RandomWalker
    {
        public Vector2 position;
        public int gridSize;
        public int stepCount = 0;
        public RandomWalker(int SizeOfGrid)
        {
            gridSize = SizeOfGrid;
            position.x = Random.Range(0, gridSize);
            position.y = Random.Range(0, gridSize);
        }

        public void moveOnGrid()
        {
            Debug.Log("Walk " + Time.time);
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
                case < 1f:
                    if (position.y + 1 >= gridSize) { break; }
                    position.y++;
                    break;
            }

            stepCount++;
        }

        public void ActivatePosition(bool[,] grid)
        {
            grid[(int)position.x, (int)position.y] = true;
        }
    }
}
