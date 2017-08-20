using UnityEngine;
using UnityEngine.SceneManagement;

public class levelBounds : MonoBehaviour 
{
	// public variables
    public LayerMask collisionMask;

    // private variables
    private BoxCollider2D _boxCol;
    private Vector3 gizmoBox;

    private void Start()
    {
        _boxCol = GetComponent<BoxCollider2D>();
    }
    // Update is called once per frame
    void Update ()
    {
        Vector2 rayOrigin = transform.position;     // Origin at the players current position
        gizmoBox = _boxCol.bounds.size;

        // Creates a boxcast around the player
        // Creates box cast at origin, the size of the player's collider, no angle, no direction, no distance, and at a specific layer
        RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, gizmoBox, 0, new Vector2(0, 0), 0, collisionMask);

        if (hit)
        {
            if (hit.collider.tag == "Lumi")
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0, 0.5f);

    	Gizmos.DrawCube(this.transform.position, gizmoBox);
	}
}