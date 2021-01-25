using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.XR.ARFoundation; 

public class ARPlacement : MonoBehaviour 
{ 
    public GameObject arObjectToSpawn; // You tell the program what prefab to copy and place down 
    private GameObject spawnedObject;  // Program keeps track of the object it spawned   
    public GameObject placementIndicator; // The indicator that tells you where you can place 

    private Pose PlacementPose;  
    private ARRaycastManager aRRaycastManager; // Manages AR  
    private bool placementPoseIsValid = false; 

    // Start is called before the first frame update 
    void Start() 
    { 
        spawnedObject = null; // Doesn't exist at the start 
        aRRaycastManager = FindObjectOfType<ARRaycastManager>(); 
    } 

    void Update() 
    { 
        // If there is not already a created object, and the placement is valid, and the user touches the screen 
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        { 
            ARPlaceObject(); // Creates a new spawnedObject 
        // Placement is valid and user touches screen, but a prefab has already been created 
        } 
        else if (spawnedObject != null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        { 
            spawnedObject.transform.position = PlacementPose.position; // Move the prefab  
            spawnedObject.transform.rotation = PlacementPose.rotation; // Rotate the prefab 

        } 
        UpdatePlacementPose(); // Calls the UpdatePlacementPose function  
        UpdatePlacementIndicator(); // Calls the UpdatePlacementIndicator 
    } 

    // Sends a raycast from the phone camera, checks if it hits anything that is a plane 
    void UpdatePlacementPose() 
    { 
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)); 
        var hits = new List<ARRaycastHit>(); 
        
        aRRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes); 
        placementPoseIsValid = hits.Count > 0; // If there are valid raycasts, set to true 
        if (placementPoseIsValid) 
        {  
            PlacementPose = hits[0].pose; // The position and rotation from the phone camera to the real world 
        } 
    } 

    // Moves the Target indicator per frame 
    void UpdatePlacementIndicator() 
    { 
        if (placementPoseIsValid) 
        { // If it's allowed to be placed 
            placementIndicator.SetActive(true); // Sets the indicator to render 
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation); 
        } 
        else 
        { // Invalid spot to place 
            placementIndicator.SetActive(false); // Stops the indicator being rendered 
        } 
    } 

    // This function creates a new copy of the provided prefab 
    void ARPlaceObject() 
    { 
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation); 
    } 
} 
