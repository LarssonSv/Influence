using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HazardSystem 
{
    public List<Hazard> _hazards = new List<Hazard>();
    public static HazardSystem HS;

    public void Init()
    {
        HS = this;
    }
    

   public void OnUpdate()
   {
       MoveHazard();
       InfluenceMapper.MP.ResetMap("HazardMap");
       
       foreach (Hazard hazard in _hazards)
       {
           if(!hazard.Modified)
               continue;

           Vector2Int newPos = InfluenceMapper.MP.WorldToGrid(hazard.transform.position);
           InfluenceMapper.MP.DrawCircleOnMap("HazardMap",newPos.x, newPos.y, hazard.Radius, hazard.MapValue);
           hazard.Modified = false;
       }
       InfluenceMapper.MP.CalculateMap();
       
   }

   public void MoveHazard()
   {
       
       foreach (Hazard hazard in _hazards){
           
          //hazard.transform.Translate(Vector3.forward * Time.deltaTime);
          hazard.Modified = true;
       }
      
   }
   
}
