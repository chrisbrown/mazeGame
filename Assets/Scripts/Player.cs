using UnityEngine;
using System.Collections;

namespace MazeGame
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MonoBehaviour
    {
        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.




        public float moveTime = 0.1f;    
        private float inverseMoveTime;
        private BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
        private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
        private Animator animator;                  //Used to store a reference to the Player's animator component.
        private int food;                           //Used to store player food points total during level.
        private int speed = 10;
        private int maxSpeed = 10;

        //Start overrides the Start function of MovingObject
        protected void Start()
        {
            //Get a component reference to the Player's animator component
            animator = GetComponent<Animator>();
            animator.SetInteger("state", 5);
            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent<BoxCollider2D>();

            //Get a component reference to this object's Rigidbody2D
            rb2D = GetComponent<Rigidbody2D>();

            inverseMoveTime = 1f / moveTime;
        }


        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {

        }


        private void Update()
        {

            int horizontal = 0;     //Used to store the horizontal move direction.
            int vertical = 0;       //Used to store the vertical move direction.


            //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));

            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
            vertical = (int)(Input.GetAxisRaw("Vertical"));

            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0)
            {
                vertical = 0;
                rb2D.AddForce(new Vector2(speed * horizontal, 0));
                if (rb2D.velocity.x > maxSpeed)
                {
                    rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                }
                if (horizontal == 1)
                {
                    animator.SetInteger("state", 2);
                }
                else if (horizontal == -1)
                {
                    animator.SetInteger("state", 3);
                }

            }

            if (vertical != 0)
            {
                horizontal = 0;
                rb2D.AddForce(new Vector2(0, speed * vertical));
                if (rb2D.velocity.y > maxSpeed)
                {
                    rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
                }
                if (vertical == 1)
                {
                    animator.SetInteger("state", 1);
                }
                else if (vertical == -1)
                {
                    animator.SetInteger("state", 0);
                }
            }
            //Check if we have a non-zero value for horizontal or vertical
            if (horizontal == 0 && vertical == 0)
            {
                rb2D.velocity = new Vector2(0, 0);
                animator.SetInteger("state", 5);
            }
        }


        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        protected IEnumerator SmoothMovement(Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPostion);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
        }

        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "goal")
            {
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                //Invoke("Restart", restartLevelDelay);
                Application.LoadLevel(Application.loadedLevel);
                //Disable the player object since level is over.
                enabled = false;
            }
        }


        //Restart reloads the scene when called.
        private void Restart()
        {
            //Load the last scene loaded, in this case Main, the only scene in the game.
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}