using UnityEngine;

public class TetrisHold : MonoBehaviour
{
    RaycastHit2D hit;
    public GameObject Player;
    public GameObject Tet1;
    public GameObject Tet2;
    public Transform HoldPoint;

    private PlayerMove playerMove;
    private MaterialVars materialVars;
   
    public bool Hold;
    public bool shoveOn;
    public int maxSplitParts = 4;
    public float Radius;
    public float Distance;
    public float Throw;
    public float distanceFromPlayer = 0.2f;
    private float holdPos;
    private float rotate = 90;
    private int tagTet;
    private int gems;    

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        materialVars = GetComponent<MaterialVars>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            shoveOn = !shoveOn;
            if (shoveOn)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                if (shoveOn == false)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Hold)
            {
                switch (tagTet)
                {
                    case 1:
                        Debug.Log("i cant share");
                        break;
                    case 2:
                        maxSplitParts = 2;
                        CreatePrefabTet();
                        break;
                    case 3:
                        maxSplitParts = 3;
                        CreatePrefabTet();
                        break;
                    case 4:
                        maxSplitParts = 4;
                        CreatePrefabTet();
                        break;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (!Hold)
            {
                Physics2D.queriesStartInColliders = false;
                hit = Physics2D.CircleCast(transform.position, Radius, Vector2.right * transform.localScale.x, Distance);
                if (hit.collider != null && hit.collider.tag == "Tetris")
                {
                    matTet();
                    FreezeTetris();
                }
            }
            else
            {
                Hold = false;
                if (hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    AnFreezeTetris();
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(transform.localScale.x, 1) * Throw;
                }
            }
        }

        if (Hold)
        {
            
            if (holdPos != 0)
            {
                hit.collider.gameObject.transform.position = new Vector2(transform.position.x, transform.position.y + holdPos);
            }
            else if (Hold == true)
            {
                hit.collider.gameObject.transform.position = HoldPoint.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Hold)
            {
                hit.collider.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, rotate);
                rotate += 90;
            }
        }
    }

    public void CreatePrefabTet()
    {       
        Vector3 leftPosition = Player.transform.position + Vector3.left * distanceFromPlayer;
        Vector3 rightPosition = Player.transform.position + Vector3.right * distanceFromPlayer;
        Tet1 = Instantiate(Tet1, leftPosition, Quaternion.identity);
        Tet2 = Instantiate(Tet2, rightPosition, Quaternion.identity);
        Destroy(hit.collider.gameObject);
        Hold = false;
    }

    private void matTet()
    {
        MaterialVars materialVars = hit.transform.GetComponent<MaterialVars>();
        tagTet = materialVars.Parts;
        holdPos = materialVars.HoldPos;
        gems = materialVars.Gems;
        Tet1 = materialVars.SplitPartPrefab;
        Tet2 = materialVars.SplitPartPrefab2;
        Hold = true;
    }

    private void FreezeTetris()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void AnFreezeTetris()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
    }

}
