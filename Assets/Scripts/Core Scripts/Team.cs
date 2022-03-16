using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    // A static dictionary of all of the teams that have been created, using the team names as keys
    private static Dictionary<string, Team> _teams = new Dictionary<string, Team>();
    public static Dictionary<string,Team> Teams
    {
        // return a copy of the dictionary so that the original cannot be edited
        get => new Dictionary<string, Team>(_teams);
    }
    
    // If a team with the given name already exists, returns that team
    // If there is no team with the given name, and if create is true, a new team with that name is created and returned
    // If there is no team with the gien name, and if create is false, returns null
    public static Team Get(string teamName, bool create = true)
    {
        Team team = null;
        if(Teams.TryGetValue(teamName, out team)) return team;
        if(create) return new Team(teamName);
        return null;
    }


    public Relationship defaultRelationship;
    private Dictionary<string, Relationship> relationships;

    private string _name;
    public string Name
    {
        get => _name;
        private set => _name = value;
    }
    
    private List<Unit> _units;
    public List<Unit> Units
    {
        get => _units;
        private set => _units = value;
    }

    private Team(string name)
    {
        if(_teams.ContainsKey(name))
        {
            throw new ArgumentException(String.Format("Team Constructor: the name {0} is already taken!", name), "name");
        }

        defaultRelationship = Relationship.NEUTRAL;
        relationships = new Dictionary<string, Relationship>();
        Name = name;
        Units = new List<Unit>();
        _teams.Add(Name,this);
    }

    public Team SetRel(string teamName, Relationship relationship)
    {
        if (relationships.ContainsKey(teamName))
        {
            relationships.Remove(teamName);
        }
        relationships.Add(teamName, relationship);
        return this;
    }
    public Relationship GetRel(string teamName)
    {
        if(relationships.ContainsKey(teamName))
        {
            return relationships[teamName];
        }
        return defaultRelationship;
    }
}

public enum Relationship
{
    // Completely ignores the other team no matter what
    NONE,
    // Helps the other team. If the other team is attacked, this team becomes hostile towards the attackers
    FRIENDLY,
    // Ignores the other team, but if this team is attacked by the other team then this relationship becomes Hostile
    NEUTRAL,
    // Attackes the other team on sight
    HOSTILE
}