using UnityEngine;


public class ReactiveGrassSway : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter = null;
    [SerializeField] MeshRenderer meshRenderer = null;
    [SerializeField] LayerMask layerMask = new LayerMask();
    Spring spring = new Spring();  // ERROR

    static readonly string LAYER = "Level";
    static readonly int LAYER_ORDER = 50;
    [SerializeField] float BEND_FACTOR = 0.5f;
    [SerializeField] float BEND_FORCE_ON_EXIT = 0.3f;
    [SerializeField] float colliderHalfWidth = 0.5f;

    float enterOffset = 0;
    float exitOffset = 0;
    bool isBending = false;
    bool isRebounding = false;

    void Awake()
    {
        //meshRenderer.sortingLayerName = LAYER;
        //meshRenderer.sortingOrder = LAYER_ORDER;
    }

    void LateUpdate()
    {
        if (!isRebounding)
            return;

        // apply the spring until its acceleration dies down
        if (Mathf.Abs(spring.acceleration) < Mathf.Epsilon) // ERROR doesnt know .acceleration
        {
            setVertHorizontalOffset(0f);
            isRebounding = false;
        }
        else
            setVertHorizontalOffset(spring.simulate());
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            enterOffset = collider.transform.position.x - transform.position.x;
        }
        if ((layerMask.value & 1 << collider.gameObject.layer) == 0)
            return;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            // Apply a force in the opposite direction that we are currently bending
            if (isBending)
                spring.applyForceStartingAtPosition(BEND_FORCE_ON_EXIT * Mathf.Sign(exitOffset), exitOffset);

            isBending = false;
            isRebounding = true;

        }
        if ((layerMask.value & 1 << collider.gameObject.layer) == 0)
            return;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            float offset = collider.transform.position.x - transform.position.x;

            if (isBending || Mathf.Sign(enterOffset) != Mathf.Sign(offset))
            {
                float radius = colliderHalfWidth + collider.bounds.size.x * 0.5f;
                exitOffset = map(offset, -radius, radius, -1f, 1f);
                setVertHorizontalOffset(exitOffset);

                isRebounding = false;
                isBending = true;
            }
            
        }
        if ((layerMask.value & 1 << collider.gameObject.layer) == 0)
            return;

    }
    void setVertHorizontalOffset(float offset)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;

        vertices[2].x = -0.5f + (offset * BEND_FACTOR / transform.localScale.x);
        vertices[3].x = +0.5f + (offset * BEND_FACTOR / transform.localScale.x);

        meshFilter.mesh.vertices = vertices;
    }

    static float map(float value, float leftMin, float leftMax, float rightMin, float rightMax) => rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
}