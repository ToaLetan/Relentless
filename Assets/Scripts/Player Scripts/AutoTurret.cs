using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoTurret : MonoBehaviour 
{
    private const float SHOOT_TIME = 0.5f;
    private const float ROTATION_SPEED = 5.0f;

    private GameManager gameManager = null;

    private GameObject currentTarget = null;
    private GameObject turretTop = null;

    private Timer shootTimer = null;

    private float bulletSpawnPosX = 0.0f;

    private bool isTurretActive = false;

    public GameObject CurrentTarget
    {
        get { return currentTarget; }
        set { currentTarget = value; }
    }

    public bool IsTurretActive
    {
        get { return isTurretActive; }
        set { isTurretActive = value; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        turretTop = gameObject.transform.GetChild(0).gameObject;

        bulletSpawnPosX = turretTop.GetComponent<SpriteRenderer>().sprite.bounds.extents.x;

        shootTimer = new Timer(SHOOT_TIME);

        shootTimer.OnTimerComplete += Shoot;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.CurrentGameState == GameManager.GameState.Running)
        {
            shootTimer.Update();

            if (isTurretActive == true)
            {
                if (currentTarget == null)
                {
                    shootTimer.ResetTimer(false);
                    GetClosestEnemy();
                } 
                else
                    RotateTowardsTarget();
            }
        }
	}

    private void GetClosestEnemy()
    {
        float shortestDistance = 900.0f;
        int indexOfClosestTarget = -1;

        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < potentialTargets.Length; i++)
        {
            float getDistance = SpeculativeContacts.GetDistance(gameObject.transform.position, potentialTargets[i].transform.position);

            if (getDistance < shortestDistance)
            {
                shortestDistance = getDistance;
                indexOfClosestTarget = i;
            }
        }

        if (indexOfClosestTarget > -1)
        {
            currentTarget = potentialTargets[indexOfClosestTarget];
            currentTarget.GetComponent<EnemyBehaviour>().TurretTargetingThis = this;
            shootTimer.ResetTimer(true);
        }
    }

    private void Shoot()
    {
        Vector3 projectileSpawnLocation = turretTop.transform.position + (turretTop.transform.right * bulletSpawnPosX);

        GameObject projectile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player_Projectile"), projectileSpawnLocation, turretTop.transform.rotation) as GameObject;

        projectile.GetComponent<ProjectileScript>().Damage = 1;
        projectile.GetComponent<ProjectileScript>().PierceCount = 1; //Might as well give turrets pierce considering their price.

        if (currentTarget != null)
            shootTimer.ResetTimer(true);
    }

    private void RotateTowardsTarget()
    {
        Vector3 vectorToTarget= Vector3.Normalize(currentTarget.transform.position - turretTop.transform.position);
        float angleToTarget = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion rotationToTarget = Quaternion.AngleAxis(angleToTarget, Vector3.forward);

        turretTop.transform.rotation = Quaternion.Slerp(turretTop.transform.rotation, rotationToTarget, Time.deltaTime * ROTATION_SPEED);
    }
}
