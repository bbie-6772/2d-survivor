using UnityEngine;

public class HeartSpawner : SpawnerBase
{
    // 스폰 위치를 받아두는 곳 
    private Vector2 _pendingPosition;

    protected override Vector2 GetSpawnPosition()
    {
        return _pendingPosition;
    }

    protected override bool Setup(GameObject obj)
    {
        Heart heart = obj.GetComponent<Heart>();
        if (heart == null)
        {
            Debug.LogError("Prefab에 Heart가 없습니다.", obj);
            return false;
        }

        heart.SetReleaseCallback(Despawn);
        return true;
    }


    public void SpawnHeart(Vector2 pendingPosition)
    {
        _pendingPosition = pendingPosition;
        Spawn();
    }

}
