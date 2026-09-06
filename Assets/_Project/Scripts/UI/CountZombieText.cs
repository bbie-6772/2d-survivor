using TMPro;
using UnityEngine;

public class CountZombieText : MonoBehaviour
{
    [SerializeField] private EnemySpawner _spawner;
    [SerializeField] private TMP_Text _countText;
    private int _aliveCount;  

    void OnEnable()
    {
        _spawner.OnSpawn += SetCountText;
        _spawner.OnDespawn += SetCountText;
    }

    void OnDisable()
    {
        _spawner.OnSpawn -= SetCountText;
        _spawner.OnDespawn -= SetCountText;
    }

    void Start()
    {
        SetCountText();
    }

    void SetCountText()
    {
        _countText.text = $"좀비 수: {_spawner.AliveCount}";
    }
}
