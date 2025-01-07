using System.Collections.Generic;
using UnityEngine;

public class BeastController : MonoBehaviour
{
    public static BeastController Instance { get; private set; }

    [Header("Other GameObjects")]
    public CeilingFan ceilingFan;
    [Space(25)]

    [Tooltip("Each value for this equals 1 second, so the higher, the less the beast moves. Example: at 1, the beast tries a move every 1 second")]
    public int movementSpeed = 10;
    [Range(0, 20), Tooltip("A 1-20 variable that dictates the probability that the beast will move on a given tick")] public int aggression = 1;
    public bool closetBeast = false;

    [Space(25)]
    public List<BeastLocation> beastLocations = new List<BeastLocation>();
    public List<GameObject> closetStages = new List<GameObject>();

    private int lastMovementSpeed = -100;

    private float movementTick = 0;
    private float closetTick = 0;

    public string CurrentRoom
    {
        get
        {
            return currentRoom;
        }
    }

    private string currentRoom = "Kitchen";
    private int currentClosetStage;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance.gameTime >= GameManager.Night.beastStartTime)
        {
            MovementTick();
            ClosetTick();
        }

        if (lastMovementSpeed == -100)
            lastMovementSpeed = movementSpeed;
    }

    void MovementTick()
    {
        movementTick += Time.deltaTime;

        if (movementTick > ((1 * movementSpeed) / GameManager.beastSpeedMultiplier))
        {
            movementTick = 0f;
            if (Random.Range(0, 20) <= aggression)
                Movement();
        }
    }

    void ClosetTick()
    {
        if (!closetBeast)
            return;

        ceilingFan.flickerRate = (ceilingFan.startFlickerRate * currentClosetStage) * 4;

        if (!Closet.Instance.Open && currentClosetStage == 1 && !closetStages[0].activeSelf)
            closetStages[0].SetActive(true);

        if (!Closet.Instance.Open_ || Closet.Instance.Open)
            closetTick += Time.deltaTime;
        else if (!Closet.Instance.Open)
        {
            closetTick += Time.deltaTime;

            if (closetTick > 2f)
            {
                closetTick = 0f;

                if (Random.Range(0, 20) > 12)
                    ClosetMovement(-1);
            }
        }

        if (closetTick > ((3 * movementSpeed) / GameManager.beastSpeedMultiplier))
        {
            closetTick = 0f;
            if (Random.Range(0, 20) <= aggression)
            {
                if (currentClosetStage < 3)
                    ClosetMovement();
                else if (currentClosetStage == 3)
                    AttemptClosetJumpscare();
            }
        }
    }

    void ClosetMovement(int forceDirection = 0)
    {
        if (forceDirection == 0)
        {
            int movementDirection = Random.Range(0, 5);
            if (movementDirection < 4)
            {
                // Forward movement
                currentClosetStage++;
            }
            else if (movementDirection == 5)
            {
                // Backward movement
                currentClosetStage--;
            }
        }
        else
            currentClosetStage--;

        currentClosetStage = Mathf.Clamp(currentClosetStage, 1, 3);

        if (Closet.Instance.Open_)
            BlackScreen.Instance.FlashBlack(.1f);
        else if (currentClosetStage == 3 && Random.Range(0, 2) == 1)
            Closet.Instance.doorKnocking.Play();

        foreach (GameObject closetPos in closetStages)
            closetPos.SetActive(false);

        closetStages[currentClosetStage - 1].SetActive(true);

        if (currentClosetStage - 1 == 0 && forceDirection != 0)
            closetStages[0].SetActive(false);
    }

    void AttemptClosetJumpscare()
    {
        DeathScreen.Instance.KillPlayer("Closet");
    }

    void Movement()
    {
        // Make other room's camera disabled
        foreach (BeastLocation location in beastLocations)
            if (location.LocationName == currentRoom)
                foreach (Camera cam in location.seeingCameras)
                {
                    if (SecurityCameras.Instance.camerasDisabled.ContainsKey(cam) || SecurityCameras.Instance.camerasBroken.ContainsKey(cam))
                        continue;

                    int rand = Random.Range(0, 20);

                    if (rand > 4)
                        SecurityCameras.Instance.DisableCamera(cam, 1.5f);
                    else if (rand <= 4)
                        SecurityCameras.Instance.BreakCamera(cam);
                }

        List<string> availableLocations = new List<string>();

        switch (currentRoom)
        {
            case "Kitchen":
                availableLocations = new List<string>()
                {
                    "ComputerRoom",
                    "LivingRoom",
                };
                break;
            case "LivingRoom":
                availableLocations = new List<string>()
                {
                    "Kitchen",
                    "HallwayStairs",
                    "Outside"
                };
                break;
            case "HallwayStairs":
                availableLocations = new List<string>()
                {
                    "LivingRoom",
                    "Outside",
                    "Hallway",
                };
                break;
            case "Hallway":
                availableLocations = new List<string>()
                {
                    "HallwayStairs",
                    "Door", // Supposed to be two btw
                    "Door",
                };
                break;
            case "Door":
                if (!Door.Instance.Open)
                {
                    if (Random.Range(1, 20) > 2)
                        DeathScreen.Instance.KillPlayer("Door");
                }
                else if (Door.Instance.Open && Door.Instance.Open_)
                {
                    if (Random.Range(1, 20) > 2)
                        DeathScreen.Instance.KillPlayer("Door");
                }
                return;
            case "Outside":
                availableLocations = new List<string>()
                {
                    "LivingRoom",
                    "Porch",
                    "HallwayStairs",
                };

                if (Window.Instance.Open)
                    for (int i = 0; i < 3; i++)
                        availableLocations.Add("Window");

                break;
            case "Window":

                return;
            case "ComputerRoom":
                availableLocations = new List<string>()
                {
                    "Porch",
                    "Kitchen"
                };
                break;
            case "Porch":
                availableLocations = new List<string>()
                {
                    "Outside",
                    "ComputerRoom"
                };
                break;
        }

        availableLocations.Add(currentRoom);

        string newLocation = availableLocations[Random.Range(0, availableLocations.Count)];
        currentRoom = newLocation;

        switch (currentRoom)
        {
            case "Door":
                movementSpeed = lastMovementSpeed / 5;
                break;
            default:
                movementSpeed = lastMovementSpeed;
                break;
        }

        foreach (BeastLocation beastLocation in beastLocations)
            foreach (GameObject beastPosition in beastLocation.possibleBeastLocations)
                beastPosition.SetActive(false);

        foreach (BeastLocation beastLocation in beastLocations)
        {
            bool currentLocation = beastLocation.LocationName == currentRoom;
            if (currentLocation)
            {
                GameObject randomLocation = beastLocation.possibleBeastLocations[Random.Range(0, beastLocation.possibleBeastLocations.Count)];
                randomLocation.SetActive(true);
            }
        }
    }

    public void ForceIntoRoom(string roomName)
    {
        currentRoom = roomName;

        foreach (BeastLocation beastLocation in beastLocations)
        {
            bool currentLocation = beastLocation.LocationName == currentRoom;
            if (!currentLocation)
            {
                foreach (GameObject beastPosition in beastLocation.possibleBeastLocations)
                    beastPosition.SetActive(false);
            }
            else
            {
                GameObject randomLocation = beastLocation.possibleBeastLocations[Random.Range(0, beastLocation.possibleBeastLocations.Count)];
                randomLocation.SetActive(true);
            }
        }
    }
}

[System.Serializable]
public struct BeastLocation
{
    public string LocationName;
    public List<GameObject> possibleBeastLocations;
    public List<Camera> seeingCameras;
}