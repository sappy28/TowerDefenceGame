using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class TowerManager : Singleton<TowerManager> {

    private TowerBtn towerBtnPressed = null;
    private SpriteRenderer spriteRenderer;

	private List<Tower> TowerList = new List<Tower>();
	private List<Collider2D> BuildList= new List<Collider2D>();
	private Collider2D buildTile;

	// Use this for initialization
	void Start () {
	    spriteRenderer = GetComponent<SpriteRenderer>();
		buildTile = GetComponent<Collider2D>();
		spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint,Vector2.zero);

            if(hit.collider.tag == "BuildSite")
            {
				buildTile = hit.collider;
                buildTile.tag = "BuildSitesFull";
				RegisterBuildSites(buildTile);
                placeTower(hit);
				towerBtnPressed = null;
			}
		}
        if(spriteRenderer.enabled)
        {   
            followMouse();
        }
        if(Input.GetMouseButtonDown(1))
        {
            disableDragSprite();
			towerBtnPressed = null;
        }
	}

	public void RegisterBuildSites(Collider2D buildTag)
	{
		BuildList.Add(buildTag);
	}

	public void RegisterTower(Tower tower)
	{
		TowerList.Add(tower);
	}

	public void RenameTagBuildSites()
	{
		foreach(Collider2D buildtag in BuildList)
		{
			buildtag.tag = "BuildSite";
		}
		BuildList.Clear();
	}

	public void DestroyAllTower()
	{
		foreach(Tower tower in TowerList)
		{
			Destroy(tower.gameObject);
		}
		TowerList.Clear();
	}

	public void placeTower (RaycastHit2D hit)
    {
        if(!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
        {
            Tower newTower = Instantiate(towerBtnPressed.TowerObject);
            newTower.transform.position = hit.transform.position;
			buyTower(towerBtnPressed.TowerPrice);
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuild);
			RegisterTower(newTower);
            disableDragSprite();
        }
    }

	public void buyTower(int price)
	{
		GameManager.Instance.subMoney(price);
	}

    public void selectedTower(TowerBtn towerSelected)
    {
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
		{
			towerBtnPressed = towerSelected;
			enableDragSprite(towerBtnPressed.DragSprite);
		}
    }

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x , transform.position.y);
    }

    public void enableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void disableDragSprite()
    {
        spriteRenderer.enabled = false;
    }
}
