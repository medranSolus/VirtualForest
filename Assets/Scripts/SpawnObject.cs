using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class SpawnObject : MonoBehaviour
{
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    ARRaycastManager rayManager;
    GameObject currentObject;
    List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField]
    GameObject placablePrefab;

    void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>();
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
        if (TryGetTouchPosition(out Vector2 touchPos))
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

    public void Spawn()
    {
        if (currentObject != null)
        {
            spawnedObjects.Add(currentObject);
            currentObject = null;
        }
    }

    public void SetPrefab(GameObject prefab)
    {
        placablePrefab = prefab;
    }
}