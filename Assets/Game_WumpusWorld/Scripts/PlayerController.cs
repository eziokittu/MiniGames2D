using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController gm;
    public float rotationSpeed = 90.0f; // Speed of rotation in degrees per second
    public float moveDistance = 2.0f; // Fixed distance to move on each key press
    public Vector2 boundsHorizontal = new Vector2(.5f, 3.5f);
    public Vector2 boundsVertical = new Vector2(-0.5f, 2.5f);
    public GameObject arrowPrefab;
    public float arrowSpeed = 1000f;

    private void Awake() {
        gm = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            RotatePlayer(-rotationSpeed); // Rotate 90 degrees to the left
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            RotatePlayer(rotationSpeed); // Rotate 90 degrees to the right
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            MovePlayer(transform.up); // Move in the local up direction
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (gm.count_arrows>0){
                FireArrow(transform.up); // fire arrow in the direction player is facing
            }
            else{

            }
        }
    }

    public void RotatePlayer(float angle)
    {
        // Rotate the player by the specified angle
        transform.Rotate(Vector3.forward * angle);
        gm.UpdatePlayerDirection((int)transform.rotation.eulerAngles.z);
    }

    public void MovePlayer_ForMobile(){
        MovePlayer(transform.up);
    }

    private void MovePlayer(Vector2 direction)
    {
        // check bounds
        Vector3 newPosition = transform.position + (Vector3)direction * moveDistance;

        // Clamp the new position within the defined boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, boundsHorizontal.x, boundsHorizontal.y);
        newPosition.y = Mathf.Clamp(newPosition.y, boundsVertical.x, boundsVertical.y);

        if (newPosition != transform.position){
            int dir = (int)transform.localEulerAngles.z;
            int pos = gm.GetPlayerPos();
            if (dir == 0){
                pos -= 4;
            }
            else if (dir == 180){
                pos += 4;
            }
            else if (dir == 90){
                pos -= 1;
            }
            else if (dir == 270){
                pos += 1;
            }
            gm.UpdatePlayerPosition(pos); // increment position by 1 relaetd to direction
        }

        // Set the player's position to the clamped position
        transform.position = newPosition;

    }

    private void FireArrow(Vector2 direction){
        // Create a new bullet instance at the firePoint position and rotation.
        GameObject bullet = Instantiate(arrowPrefab, transform.position, transform.rotation);

        // Get the bullet's rigidbody component and set its velocity.
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = (Vector3)direction * arrowSpeed * Time.deltaTime; // Fire in the up direction.

        // Destroy the bullet after a set time if it doesn't hit anything.
        Destroy(bullet, 4.0f);

        // if wumpus in range it dies
        gm.playerCanHitWumpus();

        Debug.Log("Firing arrow");
    }
}

// 0   = up
// 90  = left
// 180 = down
// 270 = right