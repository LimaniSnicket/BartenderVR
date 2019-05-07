using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float interactionRadius;
    public bool spawnPerformed;
    public Transform spawnPoint;
    public Interactable.InteractableType thisType;
    public GameObject hand;
    OVRGrabber oVRGrabber;

    protected  KeyCode PickUpKey = KeyCode.M;
    public DefaultOutline defaultOutline;

    List<GameObject> spawned = new List<GameObject>();
    int maxCapacity = 1;

    public virtual void Start()
    {
        //if (XRDevice.isPresent)
        //{
            GameObject spawn = Instantiate(objectToSpawn);
            spawn.transform.position = this.transform.position;
            spawned.Add(spawn);
            print(spawned[0]);
        //}
        spawnPoint = transform;
    }

    private void Update()
    {
        hand = TestHand();
        oVRGrabber = OVRHandInRange();
        if ((hand == null && spawnPerformed) )
        {
            spawnPerformed = false;
        } else if (oVRGrabber == null && spawnPerformed)
        {
            spawnPerformed = false;
        }

    }

    public void Spawn()
    {
        if (XRDevice.isPresent)
        {
            print("Spawn XR edition");
            if (CanSpawn())

                if ((OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0f))
                {
                    StartCoroutine(SpawnObjectInOVRHand());
                }
            {
                
            }
        }
        else
        {
            if (CanSpawn() && Input.GetKeyDown(PickUpKey))
            {
                StartCoroutine(SpawnObjectInTempHand());
            }
        }
    }

    public bool CanSpawn()
    {
        if (spawnPerformed)
        {
            return false;
        }

        if (OVRHandInRange() != null)
        {
            return true;
        }

        if (TestHand() != null)
        {
            return true;
        }

        if (hand!= null)
        {
            return true;
        }
        return false;
    }

    public GameObject TestHand()
    {
        var test = new List<GameObject>(FindObjectsOfType<GameObject>());
        test.Remove(this.gameObject);
        GameObject currentHand = test.Find(delegate (GameObject ovrh) {
            return (transform.position - ovrh.transform.position).magnitude <= interactionRadius;
        });

        if (currentHand != null && currentHand.transform.childCount == 0)
        {
            if (currentHand.tag == "goHand")
            {
                print("Test hand in range " + currentHand.name);
                spawnPoint = currentHand.transform;
                return currentHand;
            }
        }
        return null;
    }

    public OVRGrabber OVRHandInRange()
    {
        var ovrHands = new List<OVRGrabber>(FindObjectsOfType<OVRGrabber>());
        OVRGrabber currentHand = ovrHands.Find(delegate (OVRGrabber ovrh) {
            return (transform.position - ovrh.transform.position).magnitude <= interactionRadius;
        });

        if (currentHand != null)
        {
            print("Hand in Range");
            if (currentHand.grabbedObject == null)
            {
                spawnPoint = currentHand.transform;
                return currentHand;
            }
        }

        return null;

    }

    public IEnumerator SpawnObjectInOVRHand()
    {
        spawnPerformed = true;
        GameObject newObject = Instantiate(objectToSpawn);
        try
        {
            Interactable no = newObject.GetComponentInChildren<Interactable>();
            
        }
        catch (MissingComponentException) { Destroy(newObject);}
        //while (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) != 0)
        //{
        //    yield return new WaitForEndOfFrame();
        //}
        yield return null;
    }

    public IEnumerator SpawnObjectInTempHand()
    {
        spawnPerformed = true;
        GameObject newObject = Instantiate(objectToSpawn);
        Interactable no = newObject.GetComponentInChildren<Interactable>();
        no.HeldByTempHand = true;
        no.TempHandFollow = hand;

        while (no.parent == null)
        {
            yield return new WaitForEndOfFrame();
        }

        no.parent.transform.position = hand.transform.position;
        no.parent.transform.SetParent(hand.transform);
        yield return null;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 1));
        Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
    }

    //public void RemoveItemIfGrabbed()
    //{
    //    if (spawned[0].GetComponent<Interactable>().currentHoldingStatus != Interactable.HoldingStatus.NotHeld)
    //    {
    //        spawned.Remove(spawned[0]);
    //    }

    //    print("Removed Item");

    //}

    public virtual void OnTriggerExit(Collider other)
    {
        if (spawned.Contains(other.gameObject))
        {
            spawned.Remove(other.gameObject);
            print("Removed from trigger");
        }
    }

}
