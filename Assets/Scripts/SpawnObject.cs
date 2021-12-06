using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class SpawnObject : MonoBehaviour
{
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    ARRaycastManager rayManager;
    ARPlaneManager planeManager;
    List<GameObject> spawnedObjects = new List<GameObject>();
    GameObject placablePrefab;
    GameObject currentObject;
    float initialDistance;
    Vector2 initialPosition1;
    Vector2 initialPosition2;
    Vector3 initialScale;

    public void Spawn()
    {
        if (currentObject != null)
        {
            spawnedObjects.Add(currentObject);
            currentObject = null;
            planeManager.enabled = false;
            SetAllPlanesState(false);
        }
    }

    public void Cancel()
    {
        Destroy(currentObject);
        currentObject = null;
        planeManager.enabled = false;
        SetAllPlanesState(false);
    }

    public void SetPrefab(GameObject prefab)
    {
        placablePrefab = prefab;
        planeManager.enabled = true;
        SetAllPlanesState(true);
    }

    void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        planeManager.enabled = false;
    }

    bool TryGetTouchPosition(out Vector2 position)
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                position = touch.position;
                return true;
            }
        }
        position = default;
        return false;
    }

    void Update()
    {
        if (planeManager.enabled && TryGetTouchPosition(out Vector2 touchPos))
        {
            if (rayManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                if (currentObject == null)
                    currentObject = Instantiate(placablePrefab, hitPose.position, hitPose.rotation);
                else
                {
                    currentObject.transform.position = hitPose.position;
                    // currentObject.transform.rotation = hitPose.rotation;
                }

                if (Input.touchCount == 2)
                {
                    var touchZero = Input.GetTouch(0);
                    var touchOne = Input.GetTouch(1);

                    if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                        touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
                        {
                            return;
                        }
                    
                    if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
                    {
                        initialDistance = Vector2.Distance(touchZero.position, touchOne.position);

                        initialPosition1 = touchZero.position - touchOne.position;
                        initialPosition2 = touchOne.position - touchZero.position;

                        initialScale = currentObject.transform.localScale;
                        Debug.Log("Initial Distance: " + initialDistance + "GameObject Name: " + currentObject.name);
                    }
                    else
                    {
                        var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
            
                        if (Mathf.Approximately(initialDistance, 0))
                        {
                            return;
                        }

                        var factor = currentDistance / initialDistance;
                        currentObject.transform.localScale = initialScale * factor;
                        
                        var prevDir = initialPosition2 - initialPosition1;      
                        var currDir = touchOne.position - touchZero.position;
                        var angle = Vector2.SignedAngle(prevDir, currDir);
                        currentObject.transform.Rotate(Vector3.up, angle / 2);
                    }
                }
            }
        }
    }

    void SetAllPlanesState(bool active)
    {
        // Maybe create some fade effect?
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(active);
    }
}