using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoseDemo : MonoBehaviour
{
    [SerializeField] private GameObject poseContainer;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private GameObject[] exercisePrefabs;
    [SerializeField] private int exerciseIdx;
    private float timer = 5f;
    private int offset = 0;
    private float duration = 0;

    private PilotLogic _pilotLogic;

    private Dictionary<string, int> prefabs = new Dictionary<string, int>()
    {
        // { "Assisted L Sit", 0 },
        { "Bird Dog (L)", 0 },
        { "Bird Dog (R)", 1 },
        { "Elbow Planks", 2 },
        { "Glute Bridge", 3 },
        { "High Planks", 4 },
        { "Pushup Hold", 5 },
        { "Side Plank (L)", 6 },
        { "Side Plank (L) Easy", 7 },
        { "Side Plank (R)", 8 },
        { "Side Plank (R) Easy", 9 },
        { "Single Leg Glute Bridge (L)", 10 },
        { "Single Leg Glute Bridge (R)", 11 },
        { "Static Lunge (L)", 12 },
        { "Static Lunge (R)", 13 },
        { "Straight Bridge", 14 },
        { "Superman Hold", 15 },
        { "Sumo Squat", 16 },
        // { "Wall Sit", 18 }
    };

    private GameObject posePrefab;

    private void updateModels(int idx)
    {
        try
        {
            string pose = LevelSelectDisplay.exerciseList[idx];
            posePrefab = Instantiate(exercisePrefabs[prefabs[pose]]);

            posePrefab.transform.SetParent(poseContainer.transform);
            posePrefab.transform.localPosition = Vector3.zero;
            posePrefab.transform.localRotation = Quaternion.identity;
            posePrefab.transform.localScale = new Vector3(2, 2, 2);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public PoseDemo()
    {
        duration = timer;
    }

    void Start()
    {

        try
        {
            updateModels(exerciseIdx);
            Destroy(poseContainer.transform.GetChild(0).gameObject);
        }
        catch (Exception e)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        // poseContainer.transform.position = Vector3.zero;

        timer -= 1 * Time.deltaTime;

        if (timer == duration / 2)
        {
            offset = 10;
            try
            {
                updateModels(exerciseIdx + offset);
            }
            catch (Exception e)
            {
                
            }
        }

        if (timer <= 0)
        {
            SceneManager.LoadScene("Pilot");
        }

        poseContainer.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}