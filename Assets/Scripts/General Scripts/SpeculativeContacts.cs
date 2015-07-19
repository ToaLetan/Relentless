using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SpeculativeContacts
{
    public static GameObject CheckObjectContact(GameObject ownerObj, Vector2 directionVector, float distance)
    {
        GameObject contactObject = null;

        Vector3 startPos = ownerObj.transform.position;

        float ownerWidth = ownerObj.GetComponent<SpriteRenderer>().bounds.extents.x;
        float ownerHeight = ownerObj.GetComponent<SpriteRenderer>().bounds.extents.y;

        //Get corner positions.
        Vector3 bottomLeft = new Vector3(ownerObj.transform.position.x + -ownerWidth, ownerObj.transform.position.y + -ownerHeight, ownerObj.transform.position.z);
        Vector3 bottomRight = new Vector3(ownerObj.transform.position.x + ownerWidth, ownerObj.transform.position.y + -ownerHeight, ownerObj.transform.position.z);
        Vector3 topLeft = new Vector3(ownerObj.transform.position.x + -ownerWidth, ownerObj.transform.position.y + ownerHeight, ownerObj.transform.position.z);
        Vector3 topRight = new Vector3(ownerObj.transform.position.x + ownerWidth, ownerObj.transform.position.y + ownerHeight, ownerObj.transform.position.z);

        //Only perform raycast checks with objects on the Environment layer.
        int layerMask = 1 << 2;
        layerMask = ~layerMask;

        List<RaycastHit2D> raycastList = new List<RaycastHit2D>();

        //Check raycasts around the player (center, corners)
        raycastList.Add(Physics2D.Raycast(startPos, directionVector, distance, layerMask) ); //Center
        raycastList.Add(Physics2D.Raycast(bottomLeft, directionVector, distance, layerMask)); //Bottom-Left
        raycastList.Add(Physics2D.Raycast(bottomRight, directionVector, distance, layerMask)); //Bottom-Right
        raycastList.Add(Physics2D.Raycast(topLeft, directionVector, distance, layerMask)); //Top-Left
        raycastList.Add(Physics2D.Raycast(topRight, directionVector, distance, layerMask)); //Top-Right

        for (int i = 0; i < raycastList.Count; i++)
        {
            if (raycastList[i])
            {
                if (raycastList[i].collider.gameObject.tag == "Environment")
                {
                    contactObject = raycastList[i].collider.gameObject;

                    Debug.DrawRay(startPos, directionVector);
                    Debug.DrawRay(bottomLeft, directionVector);
                    Debug.DrawRay(bottomRight, directionVector);
                    Debug.DrawRay(topLeft, directionVector);
                    Debug.DrawRay(topRight, directionVector);
                }
            }
        }

        raycastList.Clear();

        return contactObject;
    }
}
