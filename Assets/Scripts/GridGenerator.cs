using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridGenerator : MonoBehaviour
{
    /* Generator Script
     * This is the core of the demonstration and generates step by step the whole level for the user to see
     * The generation is done in the start function and does so in this order:
     * Populate the grid through Random Walkers
     * Instantiate floor tiles with stones and rock objects
     * Place all the dungeon walls
     * Fill out outside of the grid and instantiate colliders
     * Then populate the grid with player, camera, treasure
     * As well as Coin Traps, Enemy Camps and Enemies
     */

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
    public GameObject colliderWall;
    public GameObject colliderContainer;
    [Header("Floor Tiles")]
    public List<GameObject> floorTiles;
    public GameObject FloorTileContainer;
    public GameObject DirtContainer;
    public List<GameObject> FloorObjects;
    [Header("Walls")]
    public List<GameObject> Walls;
    public GameObject WallsContainer;
    [Header("Player")]
    public GameObject playerCharacter;
    public GameObject playerCamera;
    public GameObject treasureObject;
    [Header("Coin Traps")]
    public GameObject coin;
    public GameObject trap;
    public GameObject coinTrapContainer;
    [Header("Enemy Camp")]
    public GameObject campCorner;
    public GameObject campWall;
    public GameObject barrel;
    public GameObject enemy;
    public GameObject enemyCampContainer;


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
        int coinAmount = Random.Range(2, 5);
        int coinCounter = 0;
        while(coinCounter < coinAmount)
        {
            SpawnCoinTrap();
            coinCounter++;
        }
        int campAmount = Random.Range(1, 10);
        int campCounter = 0;
        while (campCounter < campAmount)
        {
            SpawnEnemyCamp();
            campCounter++;
        }
        int enemyCount = Random.Range(10, 21);
        for(int i =0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    //This Function Creates Random Walkers from their class and carves out a dungeon space within a 2-dimensional boolean grid
    private void PopulateGrid()
    {
 
        //Create RandomWalker List
        randomWalkers = new List<RandomWalker> 
        {
            //Guaranteed 5 Walkers
            new RandomWalker(1, gridSize / 2, gridSize, grid),
            new RandomWalker(gridSize / 2, 1, gridSize, grid),
            new RandomWalker(gridSize - 2, gridSize / 2, gridSize, grid),
            new RandomWalker(gridSize / 2, gridSize - 2, gridSize, grid),
            new RandomWalker(gridSize / 2, gridSize / 2, gridSize, grid)
        };

        //Let the walkers carve out a space
        bool finishedSteps = true;
        while (finishedSteps)
        {
            //Move the walkers
            for (int i = randomWalkers.Count - 1; i >= 0; i--)
            {
                if (randomWalkers[i].stepCount < 100 && randomWalkers[i] != null)
                {
                    randomWalkers[i].moveOnGrid();
                    if (randomWalkers[i].stepCount == 51 && randomWalkers.Count < gridSize)
                    {
                        randomWalkers.Add(new RandomWalker((int)randomWalkers[i].position.x, (int)randomWalkers[i].position.y, gridSize, grid));
                    }
                }
            }

            finishedSteps &= randomWalkers[randomWalkers.Count - 1].stepCount < 100;
        }

        //Update the grid
        /*
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 tilePosition = gridObjects[i, j].transform.position;
                if (grid[i, j] == true)
                {
                    gridObjects[i, j] = Instantiate(floorTile, tilePosition, Quaternion.identity, transform);
                }
                else
                {
                    gridObjects[i, j] = Instantiate(basicFloorTile, tilePosition, Quaternion.identity, transform);
                }
            }
        }
        */

        //Clear the walkers
        randomWalkers.Clear();
    }

    //This function spawns the appropriate dungeon tiles at the correct positions according to the generated grid
    // Also has a chance to instantiate rocks or stones at free spaces
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
                if(Random.value < 0.05f)
                {
                    Instantiate(FloorObjects[Random.Range(0, FloorObjects.Count)], position, Quaternion.identity, FloorTileContainer.transform);
                }
            }
        }
    }

    //Uses the marching cube algorithm to decided which walls should be instantiated. The list of walls has to be prepared exactly within the engine inspector
    private void PlaceWalls()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                //If dungeon space place no block
                if (grid[i, j] == true) { continue; }

                //Marching Cube Algorithm
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
                DestroyObject(i,j);
                GameObject Wall = Instantiate(Walls[WallIndex], position, Walls[WallIndex].transform.rotation, WallsContainer.transform);
            }
        }
    }

    //This function fills outside of the generated dungeon with dirt blocks. It instantiate blocks with a 10% size of the original grid on each side
    //Also spawns colliders and positions them around the grid to keep the player on the map
    private void FillOutsideWithDirt()
    {
        //Dirt blocks
        for (int i = 0; i < gridSize * 1.2; i++)
        {
            for (int j = 0; j < gridSize * 1.2; j++)
            {
                if (i > 0.1f * gridSize && j > 0.1f * gridSize && i < 1.1f * gridSize - 1 && j < 1.1f * gridSize - 1)
                {
                    continue;
                }
                float dirtWidth = Walls[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
                float dirtHeight = Walls[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
                Vector3 position = new Vector3(i * dirtWidth - (int)(gridSize * 0.1f), 0f, j * dirtHeight - (int)(gridSize * 0.1f));
                GameObject dirt = Instantiate(Walls[0], position, Quaternion.identity, DirtContainer.transform);
            }
        }

        //Colliders
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(colliderWall, colliderContainer.transform);
            wall.GetComponent<BoxCollider>().size = new Vector3(gridSize * 1.2f, 10f, 2f);
            wall.transform.rotation = Quaternion.Euler(0f, i * 90f, 0f);
            switch (i)
            {
                case 0:
                    wall.transform.position = new Vector3((meshWidth * gridSize * 0.6f) - (gridSize * 1.2f * 0.1f), 0f, 0f - (gridSize * 1.2f * 0.1f));
                    break;
                case 1:
                    wall.transform.position = new Vector3((meshWidth * gridSize * 1.2f) - (gridSize * 1.2f * 0.1f), 0f, (meshHeight * gridSize * 0.6f) - (gridSize * 1.2f * 0.1f));
                    break;
                case 2:
                    wall.transform.position = new Vector3((meshWidth * gridSize * 0.6f) - (gridSize * 1.2f * 0.1f), 0f, (meshHeight * gridSize * 1.2f) - (gridSize * 1.2f * 0.1f));
                    break;
                case 3:
                    wall.transform.position = new Vector3(0f - (gridSize * 1.2f * 0.1f), 0f, (meshHeight * gridSize * 0.6f) - (gridSize * 1.2f * 0.1f));
                    break;
            }
        }
    }


    //This functions spawns the player at an appropriate location
    //Then spawns the treasure at an appropriate location with a minimum of 3/4 grid size distance
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
            if (canSpawn) { DestroyObject(x, y); }
        }
        Instantiate(playerCamera, Vector3.zero, playerCamera.transform.rotation);
        GameObject player = Instantiate(playerCharacter, new Vector3(meshWidth * x, 0f, meshHeight * y), playerCharacter.transform.rotation);
        canSpawn = false;
        while (!canSpawn)
        {
            x = Random.Range(0, gridSize);
            y = Random.Range(0, gridSize);
            canSpawn = grid[x, y];
            if (canSpawn) { DestroyObject(x, y); }
            Vector3 treasurePosition = new Vector3(x*meshWidth, 0f, y*meshHeight);
            float distanceToPlayer = (player.transform.position - treasurePosition).magnitude;
            if (distanceToPlayer < 3f/4f * gridSize * meshWidth) { canSpawn = false; }
        }
        Instantiate(treasureObject, new Vector3(meshWidth * x, 0f, meshHeight * y), playerCharacter.transform.rotation);
    }

    //This function spawns a coin at a proper position
    //Then continues spawning on of three spike trap patterns around the  coin
    private void SpawnCoinTrap()
    {
        //Coin
        bool canSpawn = false;
        int x = 0;
        int y = 0;
        int whileCounter = 0;
        while (!canSpawn)
        {
            whileCounter++;
            x = Random.Range(0, gridSize);
            y = Random.Range(0, gridSize);
            canSpawn = grid[x, y];
            if (canSpawn) { DestroyObject(x, y); }
            if(whileCounter >= 100) { return; }
        }
        Instantiate(coin, new Vector3(meshWidth * x, 0.2f, meshHeight * y), coin.transform.rotation, coinTrapContainer.transform);


        //Spike Trap Pattern
        float rValue = Random.value;
        switch (rValue)
        {
            case <= 0.33f:
                //Pattern1
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (i == 0 && j == 0) { continue; }
                        if (x + i < 0 || y + j < 0 || x + i >= gridSize || y + j >= gridSize) { continue; }
                        if (grid[x + i, y + j] == false) { continue; }
                        DestroyObject(x +i, y+j);
                        Instantiate(trap, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), trap.transform.rotation, coinTrapContainer.transform);
                    }
                }
                break;
            case <= 0.66f:
                //Pattern2
                for (int i = -2; i < 3; i++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        if (i == 0 && j == 0) { continue; }
                        if (x + i < 0 || y + j < 0 || x + i >= gridSize || y + j >= gridSize) { continue; }
                        if (grid[x + i, y + j] == false) { continue; }
                        DestroyObject(x + i, y + j);
                        if (Random.value < 0.5f) 
                        {
                            Instantiate(trap, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), trap.transform.rotation, coinTrapContainer.transform);                            
                        }
                    }
                }
                break;
            case <= 1f:
                //Pattern3
                for (int i = -2; i < 3; i++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        if (i == 0 && j == 0) { continue; }
                        if (x + i < 0 || y + j < 0 || x + i >= gridSize || y + j >= gridSize) { continue; }
                        if (grid[x + i, y + j] == false) { continue; }
                        if((Mathf.Abs(i) == 2 && j == 0) || (Mathf.Abs(i) == 1 && Mathf.Abs(j) == 1) || (i == 0 && Mathf.Abs(j) == 2)) {continue; }
                        DestroyObject(x + i, y + j);
                        Instantiate(trap, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), trap.transform.rotation, coinTrapContainer.transform);
                    }
                }
                break;
        }
    }


    //This function generates enemy camps by finding a proper location first, then spawning corners, walls, coins and barrels and three enemies around
    private void SpawnEnemyCamp()
    {
        //Find Location
        bool canSpawn = false;
        int x = 0;
        int y = 0;
        int whileCounter = 0;
        while (!canSpawn)
        {
            whileCounter++;
            x = Random.Range(0, gridSize);
            y = Random.Range(0, gridSize);
            canSpawn = grid[x, y];
            if(canSpawn)
            {
                RaycastHit[] allHits = Physics.BoxCastAll(new Vector3(meshWidth * x, 0f, meshHeight * y), new Vector3(2.5f, 2.5f, 2.5f), transform.forward);
                foreach(RaycastHit hit in allHits)
                {
                    if(hit.collider.gameObject.tag == "Object")
                    {
                        float distanceToObject = (hit.collider.gameObject.transform.position - new Vector3(meshWidth * x, 0f, meshHeight * y)).magnitude;
                        if(distanceToObject < 5 * meshWidth)
                        {
                            canSpawn = false;
                        }
                    }
                }
            }
            if (canSpawn) { DestroyObject(x, y); }
            if (whileCounter >= 100f) { return; }
        }
        //Spawn Barrel at centre
        Instantiate(barrel, new Vector3(meshWidth * x + Random.Range(-meshWidth, meshWidth), 0f, meshHeight * y + Random.Range(-meshHeight, meshHeight)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), enemyCampContainer.transform);
        //Walls
        int startingPointI = Random.Range(-4, 0);
        int endPointI = Random.Range(2, 5);
        int startingPointJ = Random.Range(-4, 0);
        int endPointJ = Random.Range(2, 5);

        for (int i = startingPointI; i < endPointI; i++)
        {
            for (int j = startingPointJ; j < endPointJ; j++)
            {
                DestroyObject(x + i, y + j);
                //Corner Pillars
                if(i == startingPointI && j == startingPointJ)
                {
                    Instantiate(campCorner, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f,0f,0f), enemyCampContainer.transform);
                    continue;
                }
                else if( i == startingPointI && j == endPointJ-1)
                {
                    Instantiate(campCorner, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 90f, 0f), enemyCampContainer.transform);
                    continue;
                }
                else if(i == endPointI-1 && j == startingPointJ)
                {
                    Instantiate(campCorner, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 270f, 0f), enemyCampContainer.transform);
                    continue;
                }
                else if (i == endPointI-1 && j == endPointJ-1)
                {
                    Instantiate(campCorner, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 180f, 0f), enemyCampContainer.transform);
                    continue;
                }
                //Walls
                if(i == startingPointI && j != 0)
                {
                    Instantiate(campWall, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 90f, 0f), enemyCampContainer.transform);
                }
                if (i == endPointI-1 && j != 0)
                {
                    Instantiate(campWall, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 270f, 0f), enemyCampContainer.transform);
                }
                if (j == startingPointJ && i != 0 )
                {
                    Instantiate(campWall, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 0f, 0f), enemyCampContainer.transform);
                }
                if (j == endPointJ-1 && i != 0)
                {
                    Instantiate(campWall, new Vector3(meshWidth * x + i, 0f, meshHeight * y + j), Quaternion.Euler(0f, 180f, 0f), enemyCampContainer.transform);
                }

                //Objects
                float randomValue = Random.value;
                switch (randomValue)
                {
                    case < 0.2f:
                        //Barrel

                        Instantiate(barrel, new Vector3(meshWidth * x + i + Random.Range(-meshWidth/2,meshWidth/2), 0f, meshHeight * y + j + Random.Range(-meshHeight/2, meshHeight/2)), Quaternion.Euler(0f,Random.Range(0f,360f),0f), enemyCampContainer.transform);
                        break;
                    case < 0.4f:
                        //Coin
                        Instantiate(coin, new Vector3(meshWidth * x + i, 0.2f, meshHeight * y + j), coin.transform.rotation, enemyCampContainer.transform);
                        break;                    
                }
            }
        }

        //Place Enemies
        //1 Inside Camp
        int xOffset = Random.Range(-4, 4);
        int yOffset = Random.Range(-4, 4);
        DestroyObject(x+xOffset, y+yOffset);
        Instantiate(enemy, new Vector3(meshWidth * x + xOffset, 0.2f, meshHeight * y + yOffset), Quaternion.Euler(0f, 180f, 0f), enemyCampContainer.transform);
        //2 Outside
        xOffset = Random.Range(-6, -4);
        yOffset = Random.Range(-6, -4);
        DestroyObject(x + xOffset, y + yOffset);
        Instantiate(enemy, new Vector3(meshWidth * x + xOffset, 0.2f, meshHeight * y + yOffset), Quaternion.Euler(0f, 180f, 0f), enemyCampContainer.transform);
        xOffset = Random.Range(5, 7);
        yOffset = Random.Range(5, 7);
        DestroyObject(x + xOffset, y + yOffset);
        Instantiate(enemy, new Vector3(meshWidth * x + xOffset, 0.2f, meshHeight * y + yOffset), Quaternion.Euler(0f, 180f, 0f), enemyCampContainer.transform);
    }

    //This function spawns enemies at appropriate locations within the dungeon
    private void SpawnEnemy()
    {
        bool canSpawn = false;
        int x = 0;
        int y = 0;
        while (!canSpawn)
        {
            x = Random.Range(0, gridSize);
            y = Random.Range(0, gridSize);
            canSpawn = grid[x, y];
            if (canSpawn) { DestroyObject(x, y); }
        }
        Instantiate(enemy, new Vector3(x * meshWidth, 0f, y * meshHeight), enemy.transform.rotation,enemyCampContainer.transform);
    }

    //This function checks if there are any objects and destroys them
    // It takes the x and y coordinates within the grid and does a physics cast to find colliders
    //If they are walls or objects, it deletes that object
    private void DestroyObject(int x, int y)
    {
        RaycastHit[] foundObjects = Physics.SphereCastAll(new Vector3(meshWidth * x, 0f, meshHeight * y), 0.1f, transform.forward,0.1f);
        foreach (RaycastHit hit in foundObjects)
        {
            //Return falls if it finds an object
            if(hit.collider.gameObject.tag == "Object" || hit.collider.gameObject.tag == "Wall")
            {
                Destroy(hit.collider.gameObject);
            }            
        }
    }

    //The Random Walker Class
    //Keeps track of it's location and moves randomly on the grid with the moveOnGrid function
    //Activates the current position with the Activate position function
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
