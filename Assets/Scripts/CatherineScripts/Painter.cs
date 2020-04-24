using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
	[SerializeField] private GameObject paintBrush; // stores the paint brush object
	[SerializeField] private GameObject brushContainer; // stores all the brushes in the scene

	[SerializeField] private Camera sceneCam; // camera looking at model
	[SerializeField] private Camera canvasCam; // camera looking at canvas
				
	[SerializeField] private RenderTexture canvasTex; // render texture that views the base texture and paint
	[SerializeField] private Texture clayTexture; // the pot's clay texture

	[SerializeField] private Material baseMat; // material of base texture where the painted texture will be saved

    [SerializeField] private GameObject potBase;

	private float brushSize = 1.0f; // the brush size
	public Color paintColour; // the paint colour

	private int paintCounter = 0; // keeps track of the no. of paint brushes in the scene
	private int maxPaint = 1000; // sets an upper limit for the no. of brushes in the scene

	private bool saving = false; // bool to store if the texture is being saved

    private ContactPoint contact; // stores the contact point of collision

    private bool active = false; //Whether we're painting at the moment

    private Rigidbody rb; // Stores the rigidbody
    private MeshCollider mc; // Stores the mesh collider

    private Vector3 originalPos;

	private void Start()
	{
        InitTexture();

        // Adds SetActive to MovingCam event
        AnimationStateManager.MovingCam += SetActive;

        // Finds the rigidbody and meshcollider of the pot
        rb = GetComponent<Rigidbody>();
        mc = GetComponent<MeshCollider>();

        originalPos = gameObject.transform.parent.position;
	}

    public void InitTexture() {
        // make the base material have the clay texture before any paint
        baseMat.mainTexture = clayTexture;
    }

    private void Update()
    {
        if(active)
        {
            transform.parent.position = new Vector3(-3.103f, 9.535f, 5.6f);
        }
        else
        {
            transform.parent.position = originalPos;
            potBase.SetActive(true);
        }
    }

    /// <summary>
    /// Sets whether painting is active based on where the camera is moving to
    /// </summary>
    /// <param name="e"></param>
    private void SetActive(CamMoveEventArgs e) {
        if(e.MovingTo == Position.painting) {
            StartCoroutine(Activate(e.AnimationLength));
        } else {
            active = false;
        }
    }

    /// <summary>
    /// Waits for animation to finish then activates painting
    /// </summary>
    /// <param name="animationLength"></param>
    /// <returns></returns>
    private IEnumerator Activate(float animationLength) {
        yield return new WaitForSeconds(animationLength);
        active = true;

        // makes the pot kinematic and the mesh collider convex
        rb.isKinematic = true;
        mc.convex = false;

        potBase.SetActive(false);
    }

    private void OnCollisionStay(Collision collision)
	{
        if (!active) {
            return;
        }

        if (collision.collider.CompareTag("Hands"))
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                contact = cp;
            }

            Paint();
        }
	}

	private void Paint()
	{
		// check if it is currently saving
		if(saving)
		{
			return;
		}

        Vector3 uvWorldPos = Vector3.zero;

        if (UVHitPos(ref uvWorldPos))
        {
            GameObject paintObj; // game object to store the paintbrush

            // instanstiates the paint brush with the current colour
            paintObj = Instantiate(paintBrush);
            paintObj.GetComponent<SpriteRenderer>().color = paintColour;

            paintObj.transform.parent = brushContainer.transform; // stores all the paintbrushes in the container
            paintObj.transform.localPosition = uvWorldPos; // moves the position of the brush on the UV map
            paintObj.transform.localScale = Vector3.one * brushSize; // assigns the size of the brush


            paintCounter++; // records the number of brushes

            // if it reaches the the maximum amount of brushes, save the current texture and clear the container
            if (paintCounter >= maxPaint)
            {
                saving = true;
                SaveTexture();
            }
        }
	}

	// returns position on texture map from a raycast hit on the mesh collider
	private bool UVHitPos(ref Vector3 uvWorldPos)
	{
		RaycastHit hit;
        // create a ray from collision point 
		Ray ray = new Ray(contact.point - contact.normal, contact.normal);

        if (Physics.Raycast(ray, out hit))
		{
			Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
			uvWorldPos.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
			uvWorldPos.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
			uvWorldPos.z = 0.0f;

			Debug.Log(uvWorldPos);
			return true;
		}
		else
		{
			return false;
		}
	}

	// Saves the texture to the base material then removes the paint brushes
	public void SaveTexture()
	{
		paintCounter = 0; // resets the paintbrush counter

		RenderTexture.active = canvasTex;
		Texture2D tex = new Texture2D(canvasTex.width, canvasTex.height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, canvasTex.width, canvasTex.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;
		baseMat.mainTexture = tex; //Put the painted texture as the base
		foreach (Transform child in brushContainer.transform)
		{
			//Clear brushes in the container
			Destroy(child.gameObject);
		}

		saving = false;
	}
}
