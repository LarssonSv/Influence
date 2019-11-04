using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class NpcController
{
    [FormerlySerializedAs("Npc")] [SerializeField] public List<Npc> Npcs = new List<Npc>();

    public void Init()
    {
        
    }

    public void OnUpdate()
    {
        foreach (Npc npc in Npcs)
        {
            if(npc.Path.Count < 1)
                continue;

            if (Vector3.Distance(npc.transform.position, npc.TargetLocation) < 0.1f)
            {
                npc.Path.RemoveAt(0);
                Debug.Log("Removed Path");
                
                if(npc.Path.Count < 1)
                    continue;

                npc.TargetLocation = InfluenceMapper.MP.DrawPos[0][npc.Path[0].x][npc.Path[0].y];

            }
            
            npc.transform.position =
                Vector3.Lerp(npc.transform.position, npc.TargetLocation, npc.Speed * Time.deltaTime);
        }   
    }
}
