using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaucerController : MonoBehaviour
{
    [HideInInspector] public int boardX;
    [HideInInspector] public int boardY;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool extracting = false;

    [SerializeField] private GameObject beam;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject crashedSprite;
    [SerializeField] private GameObject explosion;
    
    private Vector3 homePos;
    [SerializeField] private Renderer beamRend;
    [SerializeField] private Renderer smokeRend;
    [SerializeField] private Renderer crashRend;
    [SerializeField] private Renderer explosionRend;
    [SerializeField] private SpriteRenderer saucerRend;
    [SerializeField] private Rigidbody2D rigidBody;
    // initial position of the saucer in the base
    private Vector3 basePos = new Vector3(0.55f, 0.6f, 0f);
    private const string INITIAL_LAYER = "Building"; 
    private const int INITIAL_ORDER_IN_LAYER = 1;
    private const string DEFAULT_LAYER = "Default";

    private Vector3 aboveBasePos = new Vector3(0.55f, 1.125f, 0f);

    private Vector3 resourcePos;
    
    private Vector3 newStartPos;

    private const float ACCELERATION = 300f;
    private const float MARGIN_ERR = 0.05f;
    private const float SPEED_BASE = 0.001f;

    public string state = "none";

    private bool isCollecting = false;

    private float radius = 0.25f;
    private float currAngle = 0f;
    private float DELTA_ANGLE = Mathf.PI / 30; // change in angle in radians
    private int numScans;
    private int MAX_SCANS = 6;
    private int direction;

    private ResourceManager resources = null;
    private ResourceController resourceCont = null;
    private REBotDrag dragCont = null; 

    void Awake() {
        beamRend.enabled = false;

        transform.position = basePos;
        saucerRend.sortingLayerName = INITIAL_LAYER;
        saucerRend.sortingOrder = INITIAL_ORDER_IN_LAYER;
        
    }

    // Gets the board position and generates the location the saucer will travel to initially and its initial angle used for the resource extraction movement
    public void SetResource(int x, int y, ResourceManager resourcesManager, GameObject resource, REBotDrag reBotDrag) {
        resources = resourcesManager;
        dragCont = reBotDrag;
        resourceCont = resource.GetComponent<ResourceController>();
        numScans = (int) (Random.value * MAX_SCANS + 1);
        if(Random.value < 0.5) {
            direction = -1;
        } else {
            direction = 1;
        }
        boardX = x;
        boardY = y;
        currAngle = DELTA_ANGLE * ((int) (Random.value * 100));
        resourcePos = new Vector3(radius * Mathf.Cos(currAngle) + 0.5f + x, 
                                  radius * Mathf.Sin(currAngle) + 0.8f + y, 0);
        state = "leaving base";
    }

    void FixedUpdate() {
        // leaving base
        if(state == "leaving base" && LeaveBase()) {
            state = "moving to resource";
            return;
        }

        // going to resource selected
        if(state == "moving to resource" && MoveFromTo(aboveBasePos, resourcePos)) {
            state = "collecting";
            return;
        }
        // collecting resource
        if(state == "collecting" && Collect()) {
            newStartPos = transform.position;
            state = "moving to base";
            return;
        }
        // going back to base
        if(state == "moving to base" && MoveFromTo(newStartPos, aboveBasePos)) {
            state = "entering base";
            return;
        }
        // entering base
        if(state == "entering base" && EnterBase()) {
            // starts the coroutine that animates the new resource controller
            if(resourceCont == null) {
                print("no resource controller exists!!!");
                return;
            } 
            resources.AddResource(resourceCont.type, 1);
            dragCont.AddBot();
            Instantiate(resourceCont.newResourcePrefab).GetComponent<NewResourceController>().StartDraw(new Vector3(0.5f, 1f, 0f), 1);
            Destroy(this);
            return;
        }
    }

    // called when saucer needs to leave the base
    // returns true when base is left
    private bool LeaveBase() {
        // if the saucer fully left return true
        if(transform.position.y >= aboveBasePos.y) {
            transform.position = aboveBasePos;
            return true;
        }
        Vector3 curr = transform.position;
        curr.y += SPEED_BASE + Time.deltaTime;
        transform.position = curr;
        if(curr.y >= 0.85) {
            saucerRend.sortingLayerName = DEFAULT_LAYER;
            saucerRend.sortingOrder = 0;
        }
        return false;
    }

    // called when saucer needs to be moved from one point to another
    // using the rigid body accelerates and decelerates to that point
    // returns true when close enough to endPos and sets the saucer position to that
    private bool MoveFromTo(Vector3 startPos, Vector3 endPos) {
        // if the margine of error is small enough clear velocity and set the curr pos to end pos
        if(Vector2.Distance(transform.position, endPos) <= MARGIN_ERR) {
            rigidBody.velocity = Vector2.zero;
            transform.position = endPos;
            return true;
        }
        Vector2 direction; 
        // Accelerate if less than half the distance has been covered decelerate otherwise
        if(Vector2.Distance(transform.position, endPos) / Vector2.Distance(startPos, endPos) > 0.48) {
            direction = (endPos - transform.position).normalized;
        } else {
            //direction = (transform.position - endPos).normalized; // away from target
            direction = (-rigidBody.velocity).normalized; // oposite of movement
        }
        
        rigidBody.AddForce(direction * ACCELERATION * Time.deltaTime);
        
        return false;
    }

    // called when saucer needs to enter the base
    // returns true when base is entered
    private bool EnterBase() {
        // if the saucer fully entered return true
        if(transform.position.y <= basePos.y) {
            transform.position = basePos;
            return true;
        }
        Vector3 curr = transform.position;
        curr.y -= SPEED_BASE + Time.deltaTime;
        transform.position = curr;
        if(curr.y <= 0.85) {
            saucerRend.sortingLayerName = INITIAL_LAYER;
            saucerRend.sortingOrder = INITIAL_ORDER_IN_LAYER;
        }
        return false;
    }

    // Does the collecting animation circling and using tractor beam
    // returns true when it is done
    private bool Collect() {
        if(isCollecting) return false;
        if(numScans <= 0) return true;
        if(Random.value < 0.01f) {
            numScans--;
            if(Random.value < 0.5f) {
                direction = -1;
            } else {
                direction = 1;
            }
            isCollecting = true;
            StartCoroutine(TractorAnim());
        }
        currAngle += DELTA_ANGLE * direction;
        transform.position = new Vector3(radius * Mathf.Cos(currAngle) + 0.5f + boardX, 
                                         radius * Mathf.Sin(currAngle) + 0.8f + boardY, 0);
        return false;
    }

    // Coroutine that animates the tractor beam being used
    IEnumerator TractorAnim() {
        Vector3 currPos = transform.position;
        float wobbleDistance = 0.05f;
        // Wobble left and right
        currPos.x -= wobbleDistance;
        transform.position = currPos;
        yield return new WaitForSeconds(0.1f);
        currPos.x += wobbleDistance;
        transform.position = currPos;
        yield return new WaitForSeconds(0.1f);
        currPos.x += wobbleDistance;
        transform.position = currPos;
        yield return new WaitForSeconds(0.1f);
        currPos.x -= wobbleDistance;
        transform.position = currPos;
        yield return new WaitForSeconds(0.1f);
        // flashes and holds tractor beam
        toggleBeam();
        yield return new WaitForSeconds(0.05f);
        toggleBeam();
        yield return new WaitForSeconds(0.05f);
        toggleBeam();
        for(int i = 0; i < 10; i++) {
            yield return new WaitForSeconds(0.1f);
            if(isDead || extracting) break;
        }
        toggleBeam();
        yield return new WaitForSeconds(0.05f);
        isCollecting = false;
    }

    // helper function that toggles the beam
    private void toggleBeam() {
        beamRend.enabled = !beamRend.enabled;
    }

    // Wrapper that starts the movement coroutine 
    public void startMovement(){
        // StartCoroutine(movement());
    }
}
