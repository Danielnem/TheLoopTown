using System.Collections.Generic;
using UnityEngine;

public class QuadrantManager : MonoBehaviour
{
    public static QuadrantManager Instance;

    [System.Serializable]
    public class QuadrantGroup
    {
        public string name;
        public List<RandomSpriteSelector> spritePoints;
        public EnemySpawner enemySpawner;
    }

    public QuadrantGroup[] quadrants;

    private string lastAC = null;
    private string lastBD = null;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerOppositeQuadrant(string triggeringQuadrant)
    {
        Debug.Log($"[QuadrantManager] Entered quadrant: {triggeringQuadrant}");

        // Determine which pair we’re in
        bool isAC = triggeringQuadrant == "QuadrantA" || triggeringQuadrant == "QuadrantC";
        bool isBD = triggeringQuadrant == "QuadrantB" || triggeringQuadrant == "QuadrantD";

        if (isAC)
        {
            if (lastAC != null && GetOppositeName(lastAC) != triggeringQuadrant)
            {
                Debug.Log($"[QuadrantManager] Can't retrigger A↔C yet. Last: {lastAC}, Expected: {GetOppositeName(lastAC)}");
                return;
            }
        }
        else if (isBD)
        {
            if (lastBD != null && GetOppositeName(lastBD) != triggeringQuadrant)
            {
                Debug.Log($"[QuadrantManager] Can't retrigger B↔D yet. Last: {lastBD}, Expected: {GetOppositeName(lastBD)}");
                return;
            }
        }

        string oppositeName = GetOppositeName(triggeringQuadrant);
        Debug.Log($"[QuadrantManager] Looking for opposite quadrant to trigger: {oppositeName}");

        bool found = false;

        foreach (var q in quadrants)
        {
            Debug.Log($"[QuadrantManager] Checking quadrant group: {q.name}");

            if (q.name == oppositeName)
            {
                found = true;
                Debug.Log($"[QuadrantManager] ✅ MATCH! Triggering quadrant: {q.name}");

                if (q.spritePoints != null && q.spritePoints.Count > 0)
                {
                    foreach (var spritePoint in q.spritePoints)
                    {
                        if (spritePoint != null)
                            spritePoint.RerollSprite();
                        else
                            Debug.LogWarning($"[QuadrantManager] NULL spritePoint in {q.name}");
                    }
                }

                if (q.enemySpawner != null)
                {
                    q.enemySpawner.RerollSpawnPoints();
                }
                else
                {
                    Debug.LogWarning($"[QuadrantManager] Missing EnemySpawner in {q.name}");
                }

                // ✅ Update correct state
                if (isAC) lastAC = triggeringQuadrant;
                if (isBD) lastBD = triggeringQuadrant;

                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"[QuadrantManager] ❌ Opposite quadrant '{oppositeName}' not found!");
        }
    }

    private string GetOppositeName(string name)
    {
        return name switch
        {
            "QuadrantA" => "QuadrantC",
            "QuadrantC" => "QuadrantA",
            "QuadrantB" => "QuadrantD",
            "QuadrantD" => "QuadrantB",
            _ => name
        };
    }
}
