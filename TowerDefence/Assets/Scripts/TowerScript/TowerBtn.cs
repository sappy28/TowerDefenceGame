using UnityEngine;

public class TowerBtn : MonoBehaviour {

    [SerializeField]
    private Tower towerButton;
    [SerializeField]
    private Sprite dragSprite;
	[SerializeField] private int towerPrice;

    public Tower TowerObject
    {
        get{
            return towerButton;
            }
    }

    public Sprite DragSprite
    {
        get{
            return dragSprite;
            }
    }

	public int TowerPrice
	{
		get
		{
			return towerPrice;
		}
	}
}
