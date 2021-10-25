using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            position = Input.GetTouch(0).position;
            return true;
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
                    currentObject.transform.rotation = hitPose.rotation;
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