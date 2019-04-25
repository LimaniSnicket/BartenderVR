using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaycastDisplay : MonoBehaviour
{
    public TextMeshPro rayCastTMP;
    public float rayCastLength;
    public Interactable focus;

    private void Start()
    {
        rayCastTMP.gameObject.SetActive(false);
        if (rayCastLength <= 0)
        {
            rayCastLength = 5f;
        }
    }

    private void Update()
    {
        RaycastHit CheckFor;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out CheckFor, rayCastLength))
        {
            if (CheckRaycastComponent(CheckFor))
            {
                focus = RaycastedInteractable(CheckFor);
            }
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.red);

        if (focus != null)
        {
            rayCastTMP.gameObject.SetActive(true);
            SetRayCastTMPPosition(focus, 1f);
            SetRayCastTMPString(focus);
            //SetRayCastTMPRotation();
        }
        else
        {
            rayCastTMP.gameObject.SetActive(false);
        }
    }

    void SetRayCastTMPString(Interactable interactable)
    {
        rayCastTMP.text = interactable.name;
    }

    void SetRayCastTMPPosition(Interactable interactable, float offset)
    {
        Transform intParent = (interactable.transform.parent == null) ? interactable.transform : interactable.transform.parent;
        Vector3 tmpPos = new Vector3(intParent.position.x, intParent.position.y + (offset * intParent.lossyScale.y), intParent.position.z);
        rayCastTMP.GetComponent<RectTransform>().position = tmpPos;
    }

    void SetRayCastTMPRotation()
    {
        float turnTowards = transform.rotation.y;

        rayCastTMP.transform.rotation = new Quaternion(0f, turnTowards, 0f, 0f);
    }

    public bool CheckRaycastComponent(RaycastHit outHit)
    {
        try
        {
            outHit.transform.GetComponent<Interactable>();
            return true;
        }
        catch (System.NullReferenceException) { return false; }
    }

    public Interactable RaycastedInteractable(RaycastHit outHit)
    {
        return outHit.transform.GetComponent<Interactable>();
    }

}
