﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Scenes indexes
    const int MAIN_MENU = 0;
    const int LEVEL_MENU = 1;
    const int GADGETS_MENU = 2;
    const int SKILLS_MENU = 3;
    const int KARMA_SKILLS_MENU = 4;
    const int LEVEL_SCENE = 5;

    private static GameManager instance;
    public static AudioManager audioManager;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    private void Awake()
    {
        CreateAllChallenges();
        InitializePlayerInfo();
    }

    //Player information
    public int level { get; set; }
    public GadgetTree gadgetTree { get; private set; }
    public SkillsTree skillsTree { get; private set; }
    public KarmaSkillsTree karmaSkillsTree { get; private set; }

    void InitializePlayerInfo()
    {
        level = 1;
        gadgetTree = new GadgetTree();
        skillsTree = new SkillsTree();
        karmaSkillsTree = new KarmaSkillsTree();
    }
    //------------------


    //cl = currentLevel
    public LevelManager cl;

    //TODO meter aqui as três árvores
    List<Challenge> goodDeeds = new List<Challenge>();
    List<Challenge> challenges = new List<Challenge>();
    //Cash to buy gadgets/ammo for gadgets with
    public int money;
    //Points to buy skills with
    public int availableXp;
    public int availableKp;
    //Total points (to show mastery of the level)
    public int totalXp;
    public int totalKp;

    void CreateAllChallenges()
    {
        //TODO ajustar nomes, numero e experiencia de todas estas challenges

        //Numero na lista porque vou dar sort, nome, descrição, check, exp
        challenges.Add(new Challenge(1, "Challenge 1", "Beat the level in under 3 minutes", () => cl.timeElapsed < 3 * 60, 400));
        challenges.Add(new Challenge(2, "Challenge 2", "Beat the level in under 4 minutes", () => cl.timeElapsed < 4 * 60, 350));
        challenges.Add(new Challenge(3, "Challenge 3", "Beat the level in under 5 minutes", () => cl.timeElapsed < 5 * 60, 300));
        challenges.Add(new Challenge(4, "Challenge 9", "Beat the level with at least 6000$", () => cl.cashInInventory >= 6000, 200));
        challenges.Add(new Challenge(5, "Challenge 10", "Beat the level with at least 7000$", () => cl.cashInInventory >= 7000, 250));
        challenges.Add(new Challenge(6, "Challenge 11", "Beat the level with at least 8000$", () => cl.cashInInventory >= 8000, 300));
        challenges.Add(new Challenge(7, "Challenge 12", "Beat the level with at least 9000$", () => cl.cashInInventory >= 9000, 350));
        challenges.Add(new Challenge(8, "Challenge 13", "Beat the level with at least 10000$", () => cl.cashInInventory >= 10000, 400));
        challenges.Add(new Challenge(9, "Challenge 14", "Beat the level without turning on the flashlight", () => !cl.usedFlashlight, 200));
        challenges.Add(new Challenge(10, "Challenge 15", "Clear the level undetected", () => cl.timesDetected < 1, 400));
        challenges.Add(new Challenge(11, "Challenge 17", "Enter the bedroom while it's empty", () => cl.enteredEmptyBedroom, 400));
        challenges.Add(new Challenge(12, "Challenge 18", "Evade the cops with 5 seconds to spare", () => cl.copsCalled && cl.copsTimeLeft >= 5, 200));
        challenges.Add(new Challenge(13, "Challenge 19", "Evade the cops with 7 seconds to spare", () => cl.copsCalled && cl.copsTimeLeft >= 7, 300));
        challenges.Add(new Challenge(14, "Challenge 20", "Evade the cops with 10 seconds to spare", () => cl.copsCalled && cl.copsTimeLeft >= 10, 400));
        challenges.Add(new Challenge(19, "Challenge 32", "Lockpick a door", () => cl.doorsLockpicked >= 1, 50));
        challenges.Add(new Challenge(20, "Challenge 33", "Lockpick a window", () => cl.windowsLockpicked >= 1, 50));
        challenges.Add(new Challenge(15, "Challenge 29", "Hack sucessfully once", () => cl.successfullHacks >= 1, 400));
        challenges.Add(new Challenge(16, "Challenge 30", "Hack sucessfully all hackable devices", () => cl.successfullHacks == 4, 400));
        challenges.Add(new Challenge(17, "Challenge 30", "Hack to make some noise", () => cl.noisyHacks >= 1, 400));
        challenges.Add(new Challenge(18, "Challenge 31", "Hack the safe", () => cl.hackedSafe, 400));
        challenges.Add(new Challenge(21, "Challenge 45", "Use the Lighter to burn a flamable object", () => cl.objectsBurned >= 1, 400));
        challenges.Add(new Challenge(21, "Challenge 45", "Use the Lighter to burn all flamable objects", () => cl.objectsBurned >= 5, 400));
        challenges.Add(new Challenge(22, "Challenge 22", "Steal only from three rooms", () => CheckItemsTags(3, cl.player.inventory), 400));
        challenges.Add(new Challenge(22, "Challenge 24", "Steal only from two rooms", () => CheckItemsTags(2, cl.player.inventory), 400));
        challenges.Add(new Challenge(22, "Challenge 25", "Steal only from one rooms", () => CheckItemsTags(1, cl.player.inventory), 400));

        challenges = challenges.OrderBy(c => c.number).ToList();

        goodDeeds.Add(new Challenge(1, "Don't Sleep With Them. They're Hungry", "Feed the fishes", () => cl.fedFishes, 1));
        goodDeeds.Add(new Challenge(2, "Get That Garbage Outta Here!", "Put the trash in the garbage can", () => cl.trashInCan, 1));
        goodDeeds.Add(new Challenge(3, "And The Award Goes Too...", "Put the statue back up", () => cl.oscarFlipped, 1));

        goodDeeds = goodDeeds.OrderBy(c => c.number).ToList();

    }

    private bool CheckItemsTags(int nRooms, Inventory bag)
    {
        return false;
    }

    public void CheckAllChallenges()
    {
        if (cl.hasEndedSuccessfully)
        {
            foreach (Challenge c in challenges)
            {
                if (c.checkFullfiled())
                {
                    availableXp += c.xp;
                    totalXp += c.xp;
                }
                if (c.fullfilled)
                {
                    availableXp += c.xp;
                    Debug.Log(c.name + ": " + c.description + " - FULLFILLED!");
                }
            }

            foreach (Challenge c in goodDeeds)
            {
                if (c.checkFullfiled())
                {
                    availableKp += c.xp;
                    totalKp += c.xp;
                }
                if (c.fullfilled)
                {
                    availableKp += c.xp;
                    Debug.Log(c.name + ": " + c.description + " - FULLFILLED!");
                }
            }
        }
    }

    public List<Challenge> GetChallenges()
    {
        return challenges;
    }

    public List<Challenge> GetGoodDeeds()
    {
        return goodDeeds;
    }

    public void NewGame()
    {
        // Index defined in project build settings
        SceneManager.LoadScene(LEVEL_MENU);
        //cl = LevelManager.Instance;
    }

    public void StartLevel()
    {
        SceneManager.LoadScene(LEVEL_SCENE);
        //cl.StartGame();
    }

    public void ShowMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void ShowLevelMenu()
    {
        SceneManager.LoadScene(LEVEL_MENU);
    }

    public void ShowGadgetsMenu()
    {
        SceneManager.LoadScene(GADGETS_MENU);
    }

    public void ShowSkillsMenu()
    {
        SceneManager.LoadScene(SKILLS_MENU);
    }

    public void ShowKarmaSkillsMenu()
    {
        SceneManager.LoadScene(KARMA_SKILLS_MENU);
    }

}
