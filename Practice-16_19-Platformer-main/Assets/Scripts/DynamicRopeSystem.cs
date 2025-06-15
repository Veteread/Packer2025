using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicRopeSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject linkPrefab;
    [SerializeField] private float linkLength = 0.5f;
    [SerializeField] private float spawnDelay = 0.3f;
    [SerializeField] private int maxLinks = 10;

    [Header("References")]
   
    [SerializeField] private BlockSelectionUI blockSelectionUI;
    [SerializeField] private BlockSpawnSystem spawnSystem;
    [SerializeField] private BlockSwingController swingController;
    [SerializeField] private PlayerBlockDropper playerDropper;

    private LinkedList<GameObject> links = new LinkedList<GameObject>();
    private bool isGenerating;
    public Rigidbody2D currentBlockRb;
    private Block currentBlock;
    private GameObject currentBlockPrefab;
    private Transform startAnchor;

    public void StartGeneration()
    {
    	swingController.EnableSwing(true);
        if (!isGenerating && spawnSystem.SelectedPoint != null) 
        {
        	currentBlock = spawnSystem.SelectedPoint.GetComponentInChildren<Block>();
        	if (currentBlock != null)
        	{
        		currentBlockRb = currentBlock.GetComponent<Rigidbody2D>();
        		startAnchor = spawnSystem.SelectedPoint;
        		StartCoroutine(GenerateLinks());
        		swingController.EnableSwing(true);
        	}            
        }
    }

    private IEnumerator GenerateLinks()
    {
        isGenerating = true;
        
        // Первое звено
        GameObject newLink = CreateLink(spawnSystem.SelectedPoint.position, null);
        links.AddFirst(newLink);
        currentBlockRb.isKinematic = false;
        AttachBlock(newLink);

        yield return new WaitForSeconds(spawnDelay);

        // Последующие звенья
        while (links.Count < maxLinks)
        {
            GameObject topLink = CreateLink(startAnchor.position, null);
            
            if (links.Count > 0)
            {
                ReconnectPreviousLinks(topLink);
            }

            links.AddFirst(topLink);
            UpdateBlockPosition();
            
            yield return new WaitForSeconds(spawnDelay);
        }

        isGenerating = false;
    }

    GameObject CreateLink(Vector3 position, Rigidbody2D connectedBody)
    {
        GameObject link = Instantiate(linkPrefab, position, Quaternion.identity);
        Rigidbody2D rb = link.GetComponent<Rigidbody2D>();

        if (connectedBody != null)
        {
            HingeJoint2D joint = link.AddComponent<HingeJoint2D>();
            ConfigureJoint(joint, connectedBody);
        }

        return link;
    }

    void ReconnectPreviousLinks(GameObject newTopLink)
    {
        Rigidbody2D newTopRb = newTopLink.GetComponent<Rigidbody2D>();
        
        foreach (GameObject link in links)
        {
            link.transform.position += Vector3.down * linkLength;
        }

        GameObject oldTop = links.First.Value;
        HingeJoint2D joint = oldTop.GetComponent<HingeJoint2D>();
        joint.enableCollision = true;
        joint.connectedBody = newTopRb;
    }

    void ConfigureJoint(HingeJoint2D joint, Rigidbody2D connectedBody)
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = connectedBody;
        joint.anchor = new Vector2(0, 0.5f);
        joint.connectedAnchor = new Vector2(0, -0.5f);
    }

    void AttachBlock(GameObject firstLink)
    {
    	if (currentBlock == null) return;

    	
        HingeJoint2D blockJoint = currentBlockRb.gameObject.AddComponent<HingeJoint2D>();
        blockJoint.connectedBody = firstLink.GetComponent<Rigidbody2D>();
        blockJoint.anchor = Vector2.zero;
    }

    void UpdateBlockPosition()
    {
        if (currentBlockRb != null && links.Count > 0)
        {
            currentBlockRb.transform.position = links.Last.Value.transform.position + Vector3.down * linkLength;
        }
    }

    public void ResetSystem()
    {
        StopAllCoroutines();
        isGenerating = false;
        swingController.EnableSwing(false);
        playerDropper.SetCurrentBlock(null);
        foreach (GameObject link in links) 
        {
            Destroy(link);
        }
        links.Clear();
        if (currentBlockRb != null)
        {
        	currentBlockRb.transform.SetParent(null);
            currentBlockRb.isKinematic = false;
            currentBlockRb = null;
        }
        currentBlock = null;
    }

    public Rigidbody2D GetCurrentBlock()
    {
    	return currentBlockRb;
    }
}