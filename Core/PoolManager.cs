using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PoolManager : MonoBehaviour
{
    // Start is called before the first frame update

    //public static PoolManager SharedInstance;

   
    private List<GameObject> pooledMeleeEnemyObjects;
    private List<GameObject> pooledRangedEnemyObjects;
    private List<GameObject> pooledProjectileObjects;
    private List<GameObject> pooledExplosionObjects;
    private List<GameObject> pooledElementedObjects;
    private List<GameObject> pooledHitEffectObjects;
    private List<GameObject> pooledUltiObjects;
    private List<GameObject> pooledSummonPortals;
    private List<GameObject> pooledPopUp;
    //public GameObject loadingScreen;
    //public GameObject loadingText;
    private Text loadText;

    private int enemyAmounttoPool = 12;
    private int portalAmounttoPool = 3;
    private int projectileAmounttoPool = 10;
    private int explosionAmounttoPool = 10;
    private int elementedAmounttoPool = 10;
    private int hitEffectAmounttoPool = 20;
    private int popuptopool = 30;
    private void Awake()
    {
        pooledMeleeEnemyObjects = new List<GameObject>();
        pooledRangedEnemyObjects = new List<GameObject>();
        pooledProjectileObjects = new List<GameObject>();
        pooledExplosionObjects = new List<GameObject>();
        pooledElementedObjects = new List<GameObject>();
        pooledHitEffectObjects = new List<GameObject>();
        pooledUltiObjects = new List<GameObject>();
        pooledPopUp = new List<GameObject>();
        StoreUltiHeroes();
        StoreProjectiles();
        StoreMeleeEnemies();
        StoreRangedEnemies();

        StoreExplosions();
        StoreElemented();
        StoreHitEffects();

        StorePopUps();
    }
    void Start()
    {
       
        //loadingScreen.SetActive(true);
       
        //StartCoroutine(StoreCoroutine());

        //loadingScreen.SetActive(false);

    }

    private async void StoreAsync()
    {
        var loadtasks = new List<Task>();
       

        //Task meleeenemies = new Task(StoreMeleeEnemies);
        await Task.Run(() => StoreMeleeEnemies());
        //loadtasks.Add(Task.Run(() => StoreMeleeEnemies()));

        //Task t = Task.WhenAll(loadtasks.ToArray());
        //await t;
        //if (t.Status == TaskStatus.RanToCompletion)
        //    print("All ping attempts succeeded.");
        //else if (t.Status == TaskStatus.Faulted)
        //    print("failed");
        //StoreUltiHeroes();
        //StoreProjectiles();
        //StoreMeleeEnemies();
        //StoreRangedEnemies();

        //StoreExplosions();
        //StoreElemented();
        //StoreHitEffects();

        //StorePopUps();
        loadText.text = "Let's ROCK!";
        //loadingScreen.SetActive(false);


    }
    private void StoreUltiHeroes()
    {
        //loadText.text = "Loading just a little more!";
        var heros = Resources.LoadAll("Ulti", typeof(GameObject));
        foreach (var item in heros)
        {
            GameObject obj = (GameObject)Instantiate(item);
            obj.SetActive(false);
            pooledUltiObjects.Add(obj);
        }

    }
    private void StoreProjectiles()
    {
        //loadText.text = "Loading dream dust!";
        var projectiles = Resources.LoadAll("Projectiles", typeof(GameObject));
        foreach (var item in projectiles)
        {
            for (int i = 0; i < projectileAmounttoPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item);
                obj.SetActive(false);
                pooledProjectileObjects.Add(obj);
            }
        }
    }
    private void StoreMeleeEnemies()
    {
        try
        {
            var meleeenemies = Resources.LoadAll("Enemies/Melee", typeof(GameObject));
            var count = meleeenemies.Length;
           
            foreach (var item in meleeenemies)
            {           
                for (int i = 0; i < enemyAmounttoPool; i++)
                {
                    GameObject obj = (GameObject)Instantiate(item);
                    obj.SetActive(false);
                    pooledMeleeEnemyObjects.Add(obj);
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    private void StoreRangedEnemies()
    {
        var rangedenemies = Resources.LoadAll("Enemies/Ranged", typeof(GameObject));
        var count = rangedenemies.Length;
        int x = 0;
        foreach (var item in rangedenemies)
        {

            x++;
            loadText.text = "Loading Ranged Nightmares (" + x + " /" + count + " )";
            for (int i = 0; i < enemyAmounttoPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item);
                obj.SetActive(false);
                pooledRangedEnemyObjects.Add(obj);
            }
        }
    }
    private void StoreExplosions()
    {
        loadText.text = "Loading Ka-booms";
        var explosions = Resources.LoadAll("Explosions", typeof(GameObject));
        foreach (var item in explosions)
        {
            for (int i = 0; i < explosionAmounttoPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item);
                obj.SetActive(false);
                pooledExplosionObjects.Add(obj);
            }
        }
    }
    private void StoreElemented()
    {
        loadText.text = "Loading Ka-boom thingies!";
        var elemented = Resources.LoadAll("Elemented", typeof(GameObject));
        foreach (var item in elemented)
        {
            for (int i = 0; i < elementedAmounttoPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item);
                obj.SetActive(false);
                pooledElementedObjects.Add(obj);
            }
        }
    }
    private void StoreHitEffects()
    {
        var effect = Resources.LoadAll("HitEffect", typeof(GameObject));
        foreach (var item in effect)
        {
            for (int i = 0; i < hitEffectAmounttoPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item);
                obj.SetActive(false);
                pooledHitEffectObjects.Add(obj);
            }
        }
    }
    private void StorePopUps()
    {
        loadText.text = "Loading some extra wubly bubly thingies!";
        var effect = Resources.LoadAll("Popup", typeof(GameObject));
        foreach (var item in effect)
        {
            for (int i = 0; i < popuptopool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item);
                obj.SetActive(false);
                pooledPopUp.Add(obj);
            }
        }
    }

    public GameObject GetPooledEnemy(bool melee)
    {
        GameObject[] inactives = melee ? pooledMeleeEnemyObjects.Where(a => !a.activeInHierarchy).ToArray() : pooledRangedEnemyObjects.Where(a => !a.activeInHierarchy).ToArray();
        var selected = inactives[UnityEngine.Random.Range(0, inactives.Length)];
        selected.GetComponent<Health>().SetObjectComponents(true);
        return selected;
    }
    public GameObject GetPooledProjectile(ProjectileType ptype)
    {
        var selected = pooledProjectileObjects.Where(a => !a.activeInHierarchy && a.GetComponent<EnumHolder>().projType == ptype).FirstOrDefault();
        if (selected != null)
        {
            selected.SetActive(true);
        }
        return selected;      
    }
    public GameObject GetPooledExplosion(ProjectileType ptype)
    {
        var selected = pooledExplosionObjects.Where(a => !a.activeInHierarchy && a.GetComponent<EnumHolder>().projType == ptype).FirstOrDefault();
        if (selected != null)
        {
            selected.SetActive(true);
        }
        return selected;
    }
    public GameObject GetPooledElemented(ProjectileType ptype)
    {
        var selected = pooledElementedObjects.Where(a => !a.activeInHierarchy && a.GetComponent<EnumHolder>().projType == ptype).FirstOrDefault();
        if (selected != null)
        {
            selected.SetActive(true);
        }
        return selected;
    }
    public GameObject GetPooledHitEffect(ProjectileType ptype)
    {
        var selected = pooledElementedObjects.Where(a => !a.activeInHierarchy && a.GetComponent<EnumHolder>().projType == ptype).FirstOrDefault();
        if (selected != null)
        {
            selected.SetActive(true);
        }
        return selected;
    }
    public GameObject GetPooledPopUp()
    {
        var selected = pooledPopUp.Where(a => !a.activeInHierarchy).FirstOrDefault();
        if (selected != null)
        {
            selected.SetActive(true);
        }
        return selected;
    }
    public List<GameObject> GetHeroes()
    {
        return pooledUltiObjects;
    }




}
