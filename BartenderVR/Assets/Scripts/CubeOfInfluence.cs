using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOfInfluence : MonoBehaviour
{
    private static CubeOfInfluence coi;
    public static Collider areaOfInfluence;
    public float TimeTillSnap = 4f;

    static OutOfZoneObject outsideZone;

    private void Start()
    {
        coi = this;
        areaOfInfluence = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (outsideZone != null)
        {
            outsideZone.ManageTime(outsideZone);

            if (outsideZone.timeOutsideAOI > TimeTillSnap)
            {
                try
                {
                    if (outsideZone.gameObjectOutisdeAOI.GetComponent<Interactable>().thisType == Interactable.InteractableType.Glass)
                    {
                        Destroy(outsideZone.gameObjectOutisdeAOI);
                    }
                    else
                    {
                        outsideZone.SnapBack();
                        outsideZone = outsideZone.next;
                    }
                } catch (System.NullReferenceException)
                {
                    outsideZone.SnapBack();
                    outsideZone = outsideZone.next;
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        try
        {
            Interactable interactable = other.GetComponent<Interactable>();
            OutOfZoneObject objOut = new OutOfZoneObject(interactable);
            if (outsideZone == null)
            {
                outsideZone = objOut;
            }
            else
            {
                outsideZone.SetNext(outsideZone, objOut);
            }
            print(objOut.gameObjectOutisdeAOI.name + " has exited the play area.");
        }
        catch (System.NullReferenceException) { } 
    }

}

[System.Serializable]
public class OutOfZoneObject
{
    public Vector3 snapbackTransform;
    public float timeOutsideAOI = 0f;
    public GameObject gameObjectOutisdeAOI;
    public bool deleteIfOutside;
    public OutOfZoneObject next;
    Interactable thisInteractable;

    public OutOfZoneObject(Interactable interactable)
    {
        gameObjectOutisdeAOI = interactable.gameObject;
        snapbackTransform = interactable.SnapVector;
        next = null;
        thisInteractable = interactable;
    }

    public void SetNext(OutOfZoneObject root, OutOfZoneObject set)
    {
        if (root.next != null)
        {
            if (root != set)
            {
                SetNext(root.next, set);
            }
        }
        else
        {
            root.next = set;
        }

    }

    public void ManageTime(OutOfZoneObject root)
    {
        root.timeOutsideAOI += Time.deltaTime;
        if (root.next != null)
        {
            root.next.timeOutsideAOI += Time.deltaTime;
            ManageTime(root.next);
        }
    }

    public void SnapBack()
    {
        Debug.Log("Move bitch");
        thisInteractable.SnapBack();
        //gameObjectOutisdeAOI.transform.position = snapbackTransform;
    }

}
