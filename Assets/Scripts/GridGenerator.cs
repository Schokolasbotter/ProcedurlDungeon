using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class GridGenerator : MonoBehaviour
{
    //Grid Generation
    private bool[,] grid;
    private GameObject[,] gridObjects;
    private float meshWidth;
    private float meshHeight;
    private List<RandomWalker> randomWalkers;

    [Header("Grid Generation")]
    public int gridSize = 100;
    public GameObject basicFloorTile;
    public GameObject floorTile;
    [Header("Floor Tiles")]
    public List<GameObject> floorTiles;
    public GameObject FloorTileContainer;
    public GameObject DirtContainer;
    [Header("Walls")]
    public List<GameObject> Walls;
    public GameObject WallsContainer;
    [Header("Player")]
    public GameObject playerCharacter;
    public GameObject playerCamera;
    public GameObject treasureObject;


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

        PopulateGrid();
        CreateFloorTiles();
        PlaceWalls();
        FillOutsideWithDirt();
        SpawnPlayer();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PopulateGrid()
    {
        //Create RandomWalker List
        randomWalkers = new List<RandomWalker>();

        //Visualize Grid
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = new Vector3(i * meshWidth, 0f, j * meshHeight);
                GameObject tile = Instantiate(basicFloorTile, position, Quaternion.identity, transform);
                gridObjects[i, j] = tile;
            }
        }

        //Guaranteed 5 Walkers
        randomWalkers.Add(new RandomWalker(1, gridSize / 2, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize / 2, 1, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize - 2, gridSize / 2, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize / 2, gridSize - 2, gridSize, grid));
        randomWalkers.Add(new RandomWalker(gridSize / 2, gridSize / 2, gridSize, grid));

        for (int i = 31; i <= gridSize; i += 10)
        {
            int randomX = (int)(Random.Range(1, gridSize) * gridSize);
            int randomY = (int)(Random.Range(1, gridSize) * gridSize);
            //randomWalkers.Add(new RandomWalker(randomX, randomY, gridSize, grid));

        }

        bool finishedSteps = true;
        while (finishedSteps)
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

    private void CreateFloorTiles()
    {
        for (int i = 0; i < gridSize * 1.2; i++)
        {
            for (int j = 0; j < gridSize * 1.2; j++)
            {
                float floorTileWidth = floorTiles[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                float floorTileHeight = floorTiles[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
                Vector3 position = new Vector3(i * floorTileWidth - (int)(gridSize * 0.1f), 0f, j * floorTileHeight - (int)(gridSize * 0.1f));
                GameObject tile = Instantiate(floorTiles[Random.Range(0, floorTiles.Count)], position, Quaternion.identity, FloorTileContainer.transform);
            }
        }
    }

    private void PlaceWalls()
    {
        //WallsContainer.transform.localPosition = new Vector3(0f, Walls[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.y/2,0f);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                //If dungeon space place no block
                if (grid[i, j] == true) { continue; }

                //Debug.Log($"I: {i}\t J: {j}");
                int WallIndex = 0;
                if (j + 1 < gridSize)
                {
                    if (grid[i, j + 1]) { WallIndex += 1; }
                }
                if (i + 1 < gridSize)
                {
                    if (grid[i + 1, j]) { WallIndex += 2; }
                }
                if (j - 1 >= 0)
                {
                    if (grid[i, j - 1]) { WallIndex += 4; }
                }
                if (i - 1 >= 0)
                {
                    if (grid[i - 1, j]) { WallIndex += 8; }
                }

                float floorTileWidth = floorTiles[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                float floorTileHeight = floorTiles[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
                Vector3 position = new Vector3(i * floorTileWidth, 0f, j * floorTileHeight);
                GameObject Wall = Instantiate(Walls[WallIndex], position, Walls[WallIndex].transform.rotation, WallsContainer.transform);

            }
        }
    }

    private void FillOutsideWithDirt()
    {
        for (int i = 0; i < gridSize * 1.2; i++)
        {
            for (int j = 0; j < gridSize * 1.2; j++)
            {
                if (i > 0.1 * gridSize && j > 0.1 * gridSize && i < 1.1f * gridSize - 1 && j < 1.1f * gridSize - 1)
                {
                    continue;
                }
                float dirtWidth = Walls[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                float dirtHeight = Walls[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
                Vector3 position = new Vector3(i * dirtWidth - (int)(gridSize * 0.1f), 0f, j * dirtHeight - (int)(gridSize * 0.1f));
                GameObject dirt = Instantiate(Walls[0], position, Quaternion.identity, DirtContainer.transform);
            }
        }
    }


    private void SpawnPlayer()
    {
        bool canSpawn = false;
        int x = 0;
        int y = 0;
        while (!canSpawn)
        {
            x = Random.Range(0, gridSize);
            y = Random.Range(0, gridSize);
            canSpawn = grid[x, y];
        }
        Instantiate(playerCamera, Vector3.zero, playerCamera.transform.rotation);
        Instantiate(playerCharacter, new Vector3(meshWidth * x, 0f, meshHeight * y), playerCharacter.transform.rotation);
        canSpawn = false;
        while (!canSpawn)
        {
            x = Random.Range(0, gridSize);
            y = Random.Range(0, gridSize);
            canSpawn = grid[x, y];
        }
        Instantiate(treasureObject, new Vector3(meshWidth * x, 0f, meshHeight * y), playerCharacter.transform.rotation);
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
                    if (position.x - 1 <= 0) { break; }
                    position.x--;
                    break;
                case < 0.5f:
                    if (position.x + 1 >= gridSize-1) { break; }
                    position.x++;
                    break;
                case < 0.75f:
                    if (position.y - 1 <= 0) { break; }
                    position.y--;
                    break;
                case <= 1f:
                    if (position.y + 1 >= gridSize-1) { break; }
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
