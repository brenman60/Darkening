using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static Night Night { get; private set; }
    public static NightSection NightSection { get; private set; }
    public static event Action NightSectionChanged;
    public static event Action EnemyTick;

    private const float baseEnemyTimeInterval = 1;

    [Header("References")]
    [SerializeField] private Nights nights;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private TextMeshProUGUI nightStartText;
    [SerializeField] private Animator nightStartAnimator;
    [SerializeField] private Transform enemyHolder;

    public float nightTime { get; private set; }
    public float sectionTime { get; private set; }

    private List<Task> nightTasks = new List<Task>();
    private List<Task> sectionTasks = new List<Task>();

    private float enemyTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNight();
    }

    private void Update()
    {
        nightTime += Time.deltaTime;
        sectionTime += Time.deltaTime;

        if (sectionTime >= Night.sectionLength && sectionTasks.Count == 0)
        {
            sectionTime = 0;

            switch (NightSection)
            {
                case NightSection.Survival:
                    NightSection = NightSection.Darkening;
                    break;
                case NightSection.Darkening:
                    NightSection = NightSection.Survival;
                    break;
            }

            // Remove previous enemies
            foreach (Transform previousEnemy in enemyHolder)
                Destroy(previousEnemy.gameObject);

            // Change enemies
            foreach (EnemyProperties possibleEnemy in Night.availableEnemies)
            {
                if (possibleEnemy.nightSection != NightSection) continue;

                GameObject enemyObj = Instantiate(possibleEnemy.enemyPrefab, enemyHolder);
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                enemyObj.name = possibleEnemy.name;
                enemy.Init(possibleEnemy);
            }

            NightSectionChanged?.Invoke();

            print("It has now changed to " + NightSection);
            // "Re-roll" the section tasks
        }

        if (nightTime >= Night.nightLength && nightTasks.Count == 0)
        {
            // Beat the night
        }

        enemyTimer += Time.deltaTime;
        if (enemyTimer >= baseEnemyTimeInterval / Night.movementTickReduction)
        {
            enemyTimer = 0f;
            EnemyTick?.Invoke();
        }
    }

    public static void SetNight(Night night)
    {
        Night = night;
    }

    private void StartNight()
    {
        if (Night == null)
            Night = nights.GetNight(1);

        // Start Night animations and stuff
        nightStartText.text = Night.nightStartText;
        nightStartAnimator.SetTrigger("NightStart");

        nightTasks = Night.availableTasks;
    }

    public void CompleteTask(TaskType type, Task task)
    {
        switch (type)
        {
            case TaskType.Night:
                if (nightTasks.Contains(task))
                    nightTasks.Remove(task);
                break;
            case TaskType.Darkening:
                if (sectionTasks.Contains(task))
                    sectionTasks.Remove(task);
                break;
        }
    }
}

public enum NightSection 
{
    Survival,
    Darkening,
}

public enum Task
{

}

public enum TaskType
{
    Night,
    Darkening,
}