using UnityEngine;

/// <Summary>
///	Controls the attributes of the projectiles 
/// </Summary>
public class Lumi_Projectile_Controller : MonoBehaviour
{
	// Public variables
	[HeaderAttribute("Projectile attributes")]
	public PlayerShooterController playerShoot;		// Stores reference to the player shooter controller
	public LumiController _lumi;
	
	public float projectileSpeed;					// How fast * times player x velocity the projectile will fire

	// Private variables
	private Rigidbody2D _rb2d;				// Stores reference to the rigidbody 2d
	

	// Use this for initialization
	void Start () 
	{
		// Gets rigidbody 2d component
		_rb2d = GetComponent<Rigidbody2D>();

	}
	
	void Update()
	{

		// Gives the projectile velocity of the player in the x direction * speed	
		_rb2d.AddForce (new Vector2(_lumi._lumiVelocity.x + projectileSpeed, 0), 0);
	}
	
}
