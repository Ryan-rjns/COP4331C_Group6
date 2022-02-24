using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Unit is an Entity that is involved in the main fighting of the game.
// A Unit has health, damage, armor, a team, and other related stats.
// A Unit can be spawned, switch teams, die when its health reaches 0, and can preform other related operations.
// Specific Unit types that inherit from this class are responsible for providing stats and implementing functions (like the death animation).
public abstract class Unit : Entity
{
    
}
