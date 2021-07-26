using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlManager : MonoBehaviour
{
    public int killTarget;
    public float enemySpeed;
    public float enemyLaserSpeed;
    public float[] enemyLaserRate = new float[2];
    public float[] powerUpSpawnRate = new float[2];
    public float[] asteroidSpawnRate = new float[2];
    public float enemySpawnRate;
    public float ammoSpawnRate;
    public int enemyScore;
    public int enemyLives;
    public bool isEnemyShooting = false;
    public bool autoGeneration = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Main Camera").GetComponent<AudioSource>().time = PlayerPrefs.GetFloat("BkgMusic", 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnApplicationQuit()
    {
        PlayerPrefClear();
    }

    public void PlayerPrefClear()
    {
        PlayerPrefs.DeleteKey("Engine0");
        PlayerPrefs.DeleteKey("Engine1");
        PlayerPrefs.DeleteKey("Lives");
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.DeleteKey("ammo");
        PlayerPrefs.Save();
    }

    public void checkKillCount(int kills)
    {
        PlayerPrefs.SetFloat("BkgMusic", GameObject.Find("Main Camera").GetComponent<AudioSource>().time);
        PlayerPrefs.Save();
        int scene = SceneManager.GetActiveScene().buildIndex;
        if (kills >= killTarget)
            StartCoroutine(LoadNextScene(scene));
    }

    private IEnumerator LoadNextScene(int scene)
    {
        GameObject.Destroy(GameObject.Find("Laser Container"));
        GameObject.Destroy(GameObject.Find("EnemyContainer"));
        GameObject.Destroy(GameObject.Find("SpawnManager"));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(scene + 1);
    }
}
