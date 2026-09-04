using UnityEngine;

public class HeartSpawner : SpawnerBase
{
    // 스폰 위치를 받아두는 곳 
    private Vector2 _pendingPosition;

    protected override Vector2 GetSpawnPosition()
    {
        return _pendingPosition;
    }

    protected override void Setup(GameObject obj)
    {
        
    }


    public void SpawnHeart(Vector2 pendingPosition)
    {
        _pendingPosition = pendingPosition;
        Spawn();
    }

}
