using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : Manager<GameManager>
{
    public EntitiesDatabaseSO entitiesDatabase;

    public Transform team1Parent;
    public Transform team2Parent;

    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<BaseEntity> OnUnitDied;

    List<BaseEntity> team1Entities = new List<BaseEntity>();
    List<BaseEntity> team2Entities = new List<BaseEntity>();

    int unitsPerTeam = 4;

    public void OnEntityBought(EntitiesDatabaseSO.EntityData entityData, int lane = 0)
    {
        BaseEntity newEntity = Instantiate(entityData.prefab, team1Parent);
        newEntity.gameObject.name = entityData.name;
        team1Entities.Add(newEntity);

        newEntity.Setup(Team.Team1, GridManager.Instance.GetFreeNode(Team.Team1, lane));
    }

    public List<BaseEntity> GetEntitiesAgainst(Team against)
    {
        if (against == Team.Team1)
            return team2Entities;
        else
            return team1Entities;
    }

    public void UnitDead(BaseEntity entity)
    {
        team1Entities.Remove(entity);
        team2Entities.Remove(entity);

        OnUnitDied?.Invoke(entity);

        Destroy(entity.gameObject);
    }

    public List<BaseEntity> GetTeamEntities(Team team)
    {
        return team == Team.Team1 ? team1Entities : team2Entities;
    }

    public void SacrificeUnit(BaseEntity entity)
    {
        if (entity == null || entity.Team != Team.Team1)
            return;

        PlayerData.Instance.AddBlood(entity.bloodValue);
        UnitDead(entity);
    }


    public void DebugFight()
    {
        for (int i = 0; i < unitsPerTeam; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, entitiesDatabase.allEntities.Count);
            BaseEntity newEntity = Instantiate(entitiesDatabase.allEntities[randomIndex].prefab, team2Parent);

            team2Entities.Add(newEntity);

            newEntity.Setup(Team.Team2, GridManager.Instance.GetFreeNode(Team.Team2, i % 4));
        }
    }
}

public enum Team
{
    Team1,
    Team2
}
