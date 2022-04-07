using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Unit is an Entity that is involved in the main fighting of the game.
// A Unit has health, damage, armor, a team, and other related stats.
// A Unit can be spawned, switch teams, die when its health reaches 0, and can preform other related operations.
// Specific Unit types that inherit from this class are responsible for providing stats and implementing functions (like the death animation).
public abstract class Unit : Entity
{
    float _maxHealth = 20;
    public float MaxHealth
    {
        get => _maxHealth;
        private set => _maxHealth = Mathf.Max(value,1);
    }
    float _healthLost = 0;
    public float HealthLost
    {
        get => _healthLost;
        private set => _healthLost = Mathf.Max(value, 0);
    }
    public float Health
    {
        get => MaxHealth - HealthLost;
    }

    float _armor = 0;
    public float Armor
    {
        get => _armor;
        private set => _armor = value;
    }

    public Team team;

    // Records how many units of each team this unit has killed
    Dictionary<string, int> teamKills = new Dictionary<string, int>();
    public Dictionary<string, int> TeamKills
    {
        get => new Dictionary<string, int>(teamKills);
    }
    // Records how many units with no team this unit has killed
    int nullKills = 0;
    public int NullKills
    {
        get => nullKills;
    }
    // Returns the total number of kills for this unit
    public int Kills
    {
        get
        {
            int kills = nullKills;
            foreach (var teamKill in TeamKills)
            {
                kills += teamKill.Value;
            }
            return kills;
        }
    }
    public void AddKill(string teamName)
    {
        if (team == null)
        {
            nullKills++;
            return;
        }
        int currKills;
        if (teamKills.TryGetValue(teamName, out currKills))
        {
            currKills++;
            teamKills.Remove(teamName);
        }
        else currKills = 1;
        teamKills.Add(teamName, currKills);
    }


    // Called from an opposing unit or projectile that hit this unit with an attack
    public float Damaged(Unit source, float damage)
    {
        // If a neutral relationship is attacked, that relationship becomes hostile
        if (team != null && source.team != null && team != source.team)
        {
            if (team.GetRel(source.team.Name) == Relationship.NEUTRAL)
            {
                team.SetRel(source.team.Name, Relationship.HOSTILE);
            }
        }

        float finalDamage = Mathf.Max(damage - Armor,0);
        HealthLost += finalDamage;
        if (Health < 0) Killed(source);
        return finalDamage;
    }

    // Destroys this unit, called when this unit's health drops below 0
    // If source is an enemy, it gets credit for the kill
    public void Killed(Unit source)
    {
        if (source != null) source.AddKill(team.Name);
        DeathAnimation();
        Destroy(this);
    }
    public GameObject explosionPrefab;
    // Child classes can override this and use it to spawn in a seperate, non-unit death animation.
    public virtual void DeathAnimation() 
    {
        EntityDebug("DEAD");
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        //Destroy(explosionPrefab);

    }
}