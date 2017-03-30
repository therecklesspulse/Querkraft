using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    GameObject EnemyPrefab;

    [SerializeField]
    float rangeWidth = 20f;
    [SerializeField]
    float rangeHeight = 10f;

    [SerializeField]
    bool xAsRange = true;
    [SerializeField]
    bool yAsRange = false;

    [SerializeField]
    int maxEnemies = 8;
    [SerializeField]
    int enemyPool = -1;
    int enemyCount = 0;

    [SerializeField]
    float spawnRate = 1f;
    float rateCount = 1f;

    [SerializeField]
    bool spawnGrounded = true;
    const float Y_MARGIN = 0.02f;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(rateCount > 0f)
            rateCount -= Time.deltaTime;
        else
        {
            if(enemyCount < maxEnemies && enemyPool != 0 && SomeoneInRange())
            {
                rateCount = spawnRate;
                SpawnEnemy();
            }
        }
	}

    void SpawnEnemy()
    {
        enemyCount++;
        if (enemyPool > 0)
            enemyPool--;
        Vector3 spawnPos = new Vector3(transform.position.x + Random.value * rangeWidth, transform.position.y + Random.value * rangeHeight, 0f);
        if (spawnGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(spawnPos.x, transform.position.y + rangeHeight), Vector2.down, rangeHeight, LayerMask.GetMask("Level"));
            if (hit)
            {
                spawnPos = hit.point;
            }
        }
        Bounds bounds = EnemyPrefab.GetComponent<SpriteRenderer>().sprite.bounds;
        spawnPos.y += (bounds.center.y + bounds.extents.y)*EnemyPrefab.transform.lossyScale.y + Y_MARGIN;
        GameObject spawnedEnemy = Instantiate<GameObject>(EnemyPrefab, spawnPos, Quaternion.identity);
        spawnedEnemy.GetComponent<Enemy>().SetSpawner(this);

    }

    public void SomeoneDied()
    {
        enemyCount--;
    }

    

    bool SomeoneInRange()
    {
        bool someoneInRange = false;
        foreach (Player p in PlayerList.GetPlayerList())
        {
            someoneInRange = ((xAsRange ? InXRange(p.transform.position.x) : true) && (yAsRange ? InYRange(p.transform.position.y) : true));
            if (someoneInRange)
                break;
        }
        return someoneInRange;
    }

    bool InXRange(float x)
    {
        return (x > transform.position.x && x < transform.position.x + rangeWidth);
    }
    bool InYRange(float y)
    {
        return (y > transform.position.y && y < transform.position.y + rangeHeight);
    }
}
