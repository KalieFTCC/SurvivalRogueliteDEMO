using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Kalie Kirch 11/27/2022
public class Generation : MonoBehaviour
{
    //changable values for procedural generation
    public int mapWidth = 7;
    public int mapHeight = 7;
    public int roomsToGenerate = 12;

    private int roomCount;
    private bool roomsCreated;

    private Vector2 firstRoomPosition;

    //creates a 2d array map
    private bool[,] map;
    //keeps track of the room prefabs
    public GameObject roomPrefab;

    //keeps track of the rooms
    private List<Room> roomObjects = new List<Room>();

    public static Generation instance;

    private void Awake()
    {
        //initializes instance 
        instance = this;
    }

    private void Start()
    {
        Random.InitState(74729283);
        Generate();
    }

    //called at the start of the game, starts the room generation process
    public void Generate() 
    {
        map = new bool[mapWidth, mapHeight];
        //center of the map
        CheckRoom(3, 3, 0, Vector2.zero, true);
        IntializeRooms();
        //converts tile coordinates into global coordinates, using the player game object/prefab
        FindObjectOfType<Player>().transform.position = firstRoomPosition * 12;
    }

    //checks to see if a room can be placed here
    void CheckRoom(int x, int y, int remaining, Vector2 generalDirection, bool firstRoom = false) 
    {
        //starts with one room in the center and moves and generates according to the generalDirection variable
        //loops until remaining integer is zero
        
        //Impossible game states
        if (roomCount >= roomsToGenerate)
            return;
        if (x < 0 || x > mapWidth - 1 || y < 0 || y > mapHeight - 1)
            return;
        if (firstRoom == false && remaining == 0)
            return;
        if (map[x, y] == true)
            return;
        //populates the array
        if (firstRoom == true)
            firstRoomPosition = new Vector2(x, y);
        roomCount++;
        map[x, y] = true;

        //turnary conditions 
        bool north = Random.value > (generalDirection == Vector2.up ? 0.2f : 0.8f);
        bool south = Random.value > (generalDirection == Vector2.down ? 0.2f : 0.8f);
        bool east = Random.value > (generalDirection == Vector2.right ? 0.2f : 0.8f);
        bool west = Random.value > (generalDirection == Vector2.left ? 0.2f : 0.8f);

        //amount possible to generate divided by 4, one for each direction
        int maxRemaining = roomsToGenerate / 4;

        if (north || firstRoom)
            //you can use turnary conditions inside method calls. This method basically calculates if it's at the +
            //first room it gives it the first remaining, otherwise (colon) it gives remaining - 1
            //then, if it's the first room, you want to give it vector.up, and if its not, have the generation continue
            //in the direction of the generalDirection
            CheckRoom(x, y + 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.up : generalDirection);
        if (south || firstRoom)
            CheckRoom(x, y -1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.down : generalDirection);
        if (east || firstRoom)
            CheckRoom(x + 1, y, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.right : generalDirection);
        if (west || firstRoom)
            CheckRoom(x - 1, y, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.left : generalDirection);
    }

    void IntializeRooms() 
    {
        //if the rooms have been made, this method will just return to nothing
        if (roomsCreated)
            return;
        
        roomsCreated = true;
        //goes through both x and y coordinates in the map array
        for (int x = 0; x < mapWidth; ++x) 
        {
            for (int y = 0; y < mapHeight; ++y) 
            {
                if (map[x, y] == false)
                    continue;

                GameObject roomObj = Instantiate(roomPrefab, new Vector3(x, y, 0) * 12, Quaternion.identity);
                Room room = roomObj.GetComponent<Room>();
                //creates gates and walls for each room based on their position
                if(y < mapHeight - 1 && map[x, y +1] == true) 
                {
                    room.northDoor.gameObject.SetActive(true);
                    room.northWall.gameObject.SetActive(false);
                }
                if (y > 0 && map[x, y - 1] == true) 
                {
                    room.southDoor.gameObject.SetActive(true);
                    room.southWall.gameObject.SetActive(false);
                }
                //my terrible procedural generation error that broke everything was I forgot to put a -1 LOL
                //I spent weeks trying to figure out what I did wrong
                if (x < mapWidth -1 && map[x + 1, y] == true) 
                {
                    room.eastDoor.gameObject.SetActive(true);
                    room.eastWall.gameObject.SetActive(false);
                }
                if (x > 0 && map[x - 1, y] == true) 
                {
                    room.westDoor.gameObject.SetActive(true);
                    room.westWall.gameObject.SetActive(false);
                }

                if (firstRoomPosition != new Vector2(x, y))
                    room.GenerateInterior();

                roomObjects.Add(room);
            }
        }

        CalculateKeyExit();
    }

    void CalculateKeyExit() 
    {
        
    }
}
