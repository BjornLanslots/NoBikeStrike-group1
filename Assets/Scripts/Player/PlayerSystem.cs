using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Utility;

//spawns, initializes and manages avatar at runtime
public class PlayerSystem : MonoBehaviour
{
    public enum InputMode
    {
        Flat,
        VR,
        Suite,
        None
    }

    public enum ControlMode
    {
        Driver,
        Passenger,
        Cyclist,
        HostAI
    }

    public enum VehicleType
    {
        MDV,
        AV,
        Bicycle
    }

    public InputMode PlayerInputMode;
    [SerializeField]
    PlayerAvatar _AvatarPrefab;
    [SerializeField]
    PlayerAvatar[] _AvatarPrefabDriver;
    // Add cyclist prefab references
    [SerializeField]
    PlayerAvatar _AvatarPrefabCyclist;  // New cyclist prefab


    [NonSerialized]
    public PlayerAvatar LocalPlayer;
    public PlayerAvatar PedestrianPrefab => _AvatarPrefab;
    

    // Avatars contains both Drivers and Pedestrians (in arbitrary order)
    [NonSerialized]
    public List<PlayerAvatar> Avatars = new List<PlayerAvatar>();
    [NonSerialized]
    public List<PlayerAvatar> Cars = new List<PlayerAvatar>();
    [NonSerialized]
    public List<PlayerAvatar> Pedestrians = new List<PlayerAvatar>();
    [NonSerialized]
    public List<PlayerAvatar> Passengers = new List<PlayerAvatar>();
    [NonSerialized]
    public List<PlayerAvatar> Cyclists = new List<PlayerAvatar>();  // Store cyclist avatars ????????????????????????

    PlayerAvatar[] Player2Avatar = new PlayerAvatar[UNetConfig.MaxPlayers];
    public PlayerAvatar GetAvatar(int player) => Player2Avatar[player];

    HMIManager _hmiManager;
    void Awake()
    {
        _hmiManager = FindObjectOfType<HMIManager>();
    }

    public void ActivatePlayerAICar()
    {
        var aiCar = LocalPlayer.GetComponent<AICar>();
        var tracker = LocalPlayer.GetComponent<WaypointProgressTracker>();
        Assert.IsNotNull(tracker);
        aiCar.enabled = true;
        tracker.enabled = true;
        foreach(var waypoint in tracker.Circuit.Waypoints)
        {
            var speedSettings = waypoint.GetComponent<SpeedSettings>();
            if (speedSettings != null)
            {
                speedSettings.targetAICar = aiCar;
            }
        }
    }

    // Modify GetAvatarPrefab to handle the Cyclist role
    PlayerAvatar GetAvatarPrefab(SpawnPointType type, int carIdx)
    {
        switch (type)
        {
            case SpawnPointType.PlayerControlledPedestrian:
                return _AvatarPrefab;
            case SpawnPointType.PlayerControlingCar:
            case SpawnPointType.PlayerInAIControlledCar:
                return _AvatarPrefabDriver[carIdx];
            case SpawnPointType.PlayerControlledCyclist:  // Add cyclist spawn point type
                return _AvatarPrefabCyclist;
            default:
                Assert.IsFalse(true, $"Invalid SpawnPointType: {type}");
                return null;
        }
    }

    // Modify SpawnLocalPlayer to handle the Cyclist role
    public void SpawnLocalPlayer(SpawnPoint spawnPoint, int player, ExperimentRoleDefinition role)
    {
        bool isPassenger = spawnPoint.Type == SpawnPointType.PlayerInAIControlledCar;
        bool isCyclist = spawnPoint.Type == SpawnPointType.PlayerControlledCyclist; // Detect if cyclist

        LocalPlayer = SpawnAvatar(spawnPoint, GetAvatarPrefab(spawnPoint.Type, role.carIdx), player, role);

        if (isCyclist)
        {
            LocalPlayer.Initialize(false, PlayerInputMode, ControlMode.Cyclist, VehicleType.Bicycle, spawnPoint.CameraIndex);
            // Apply any cyclist-specific logic here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        else if (isPassenger)
        {
            LocalPlayer.Initialize(false, PlayerInputMode, ControlMode.Passenger, spawnPoint.VehicleType, spawnPoint.CameraIndex);

            var waypointFollow = LocalPlayer.GetComponent<WaypointProgressTracker>();
            Assert.IsNotNull(waypointFollow);
            waypointFollow.Init(role.AutonomousPath);
            LocalPlayer.gameObject.layer = LayerMask.NameToLayer(role.AutonomousIsYielding ? "Yielding" : "Car");

            var hmiControl = LocalPlayer.GetComponent<ClientHMIController>();
            hmiControl.Init(_hmiManager);
        }
        else
        {
            LocalPlayer.Initialize(false, PlayerInputMode, ControlMode.Driver, spawnPoint.VehicleType, spawnPoint.CameraIndex);
        }
    }

    public void SpawnRemotePlayer(SpawnPoint spawnPoint, int player, ExperimentRoleDefinition role)
    {
        var remotePlayer = SpawnAvatar(spawnPoint, GetAvatarPrefab(spawnPoint.Type, role.carIdx), player, role);
        remotePlayer.Initialize(true, InputMode.None, ControlMode.HostAI, spawnPoint.VehicleType);
    }

    // Add new avatar collection for cyclists
    public List<PlayerAvatar> GetAvatarsOfType(AvatarType type)
    {
        switch (type)
        {
            case AvatarType.Pedestrian:
                return Pedestrians;
            case AvatarType.Driver:
                return Cars;
            case AvatarType.Cyclist:  // Add cyclist type
                return Cyclists;
            default:
                Assert.IsFalse(true, $"No avatar collection for type {type}");
                return null;
        }
    }

    PlayerAvatar SpawnAvatar(SpawnPoint spawnPoint, PlayerAvatar prefab, int player, ExperimentRoleDefinition role)
    {
        var avatar = GameObject.Instantiate(prefab);
        avatar.transform.position = spawnPoint.position;
        avatar.transform.rotation = spawnPoint.rotation;
        var cameraSetup = spawnPoint.Point.GetComponent<CameraSetup>();
        if (cameraSetup != null)
        {
            foreach (var cam in avatar.GetComponentsInChildren<Camera>())
            {
                cam.fieldOfView = cameraSetup.fieldOfView;
                cam.transform.localRotation = Quaternion.Euler(cameraSetup.rotation);
            }
        }
        Avatars.Add(avatar);
        GetAvatarsOfType(avatar.Type).Add(avatar);
        Player2Avatar[player] = avatar;
        if (role.HoodHMI != null)
        {
            _hmiManager.AddHMI(avatar.HMISlots.Spawn(HMISlot.Hood, role.HoodHMI));
        }
        if (role.TopHMI != null)
        {
            _hmiManager.AddHMI(avatar.HMISlots.Spawn(HMISlot.Top, role.TopHMI));
        }
        if (role.WindshieldHMI != null)
        {
            _hmiManager.AddHMI(avatar.HMISlots.Spawn(HMISlot.Windshield, role.WindshieldHMI));
        }
        return avatar;
    }

    List<AvatarPose> _poses = new List<AvatarPose>();
    public List<AvatarPose> GatherPoses()
    {
        _poses.Clear();
        foreach (var avatar in Avatars)
        {
            _poses.Add(avatar.GetPose());
        }
        return _poses;
    }

    public void ApplyPoses(List<AvatarPose> poses)
    {
        for (int i = 0; i < Avatars.Count; i++)
        {
            var avatar = Avatars[i];
            if (avatar != LocalPlayer)
            {
                Avatars[i].ApplyPose(poses[i]);
            }
        }
    }

    //displays controler selection GUI
    public void SelectModeGUI()
    {

            GUILayout.Label($"Mode: {PlayerInputMode}");
            if (GUILayout.Button("Suite mode"))
            {
                PlayerInputMode = InputMode.Suite;
            }
            if (GUILayout.Button("HMD mode"))
            {
                PlayerInputMode = InputMode.VR;
            }
            if (GUILayout.Button("Keyboard mode"))
            {
                PlayerInputMode = InputMode.Flat;
            }
    }

    public void SelectMode(InputMode inputMode)
    {
        PlayerInputMode = inputMode;
    }
}
