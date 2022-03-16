using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class StarLib
{
    // A proper modulo that doesn't give you negative numbers.
    public static float Mod(this float f, float m) => (f % m + m) % m;

    // An easier way to convert a 1D floats into 3D vector for math
    public static Vector3 ToVecX(this float f) => new Vector3(f, 0, 0);
    public static Vector3 ToVecY(this float f) => new Vector3(0, f, 0);
    public static Vector3 ToVecZ(this float f) => new Vector3(0, 0, f);

    // A quick method for clamping the individual axes of a vector
    public static Vector3 ClampX(this Vector3 v, float min, float max) => new Vector3(Mathf.Clamp(v.x, min, max), v.y, v.z);
    public static Vector3 ClampY(this Vector3 v, float min, float max) => new Vector3(v.x, Mathf.Clamp(v.y, min, max), v.z);
    public static Vector3 ClampZ(this Vector3 v, float min, float max) => new Vector3(v.x, v.y, Mathf.Clamp(v.z, min, max));

    // Converts the float into a euler angle in the range (-180,180]
    public static float Euler180(this float f)
    {
        f = f.Mod(360);
        if (f > 180) f -= 360;
        return f;
    }
    // Converts every component of the vector into a euler angle in the range (-180,180]
    public static Vector3 Euler180(this Vector3 v) => new Vector3(v.x.Euler180(), v.y.Euler180(), v.z.Euler180());

    // A safe way to read an index in a List. If the index is invalid, this returns null instead of throwing an exception.
    // If the list contains reference types, use GetC
    // If the list contains non-nullable value types, use GetV.
    public static T GetC<T>(this List<T> list, int index) where T : class
    {
        if (index < 0 || index >= list.Count) return null;
        return list[index];
    }
    public static Nullable<T> GetV<T>(this List<T> list, int index) where T : struct
    {
        if (index < 0 || index >= list.Count) return null;
        return (Nullable<T>)list[index];
    }

    // Recursively search (BFS) the target and all of its children and return all children that match the given name.
    // If exact is true, the names must match exactly. If it's false, this will also return any children whose names include the given string.
    // If count > 0, then this stops after it finds that many results (otherwise count is ignored).
    // The output List and its entries are guarenteed to be non-null
    public static List<GameObject> FindChildren(this GameObject target, string name, bool exact = false, int count = -1)
    {
        var result = new List<GameObject>();

        // Breadth-first search through all of the children.
        var queue = new Queue<GameObject>();
        queue.Enqueue(target);
        while (queue.Count > 0)
        {
            GameObject curr = queue.Dequeue();
            if (curr == null) continue;
            if (exact ? curr.name.Equals(name) : curr.name.Contains(name))
            {
                result.Add(curr);
                count--;
                if (count == 0) break;
            }
            for (int i = 0; i < curr.transform.childCount; i++)
            {
                queue.Enqueue(curr.transform.GetChild(i).gameObject);
            }
        }
        return result;
    }

    // Over time this changes the current value to reach the target value, at the specified rate. Returns the new current value.
    // This function should typically be called from FixedUpdate(), as there must be a (prefereably consistent) Time.deltaTime
    public static Vector3 PID(Vector3 curr, Vector3 target, float rate)
    {
        // If the velocity has reached the target, do nothing
        if (curr == target) return curr;
        // Find the vector that the current velocity needs to travel along to reach the target velocity
        Vector3 velocityDelta = target - curr;
        // Find the amount that the current velocity will change in this update frame. Make sure it doesn't overshoot the target.
        float velocityStep = Mathf.Min(rate * Time.deltaTime, velocityDelta.magnitude);
        // Increment the current velocity
        return curr + velocityStep * velocityDelta.normalized;
    }

    // Do a Physics.RaycastAll and search through all of the hits to find any objects that match the filter
    //
    // Parameters:
    // origin           - The origin of the Raycast (see Physics.RaycastAll)
    // direction        - The direction of the Raycast (see Physics.RaycastAll)
    // maxDistance      - The maximum distance of the Raycast (see Physics.RaycastAll)
    // filter           - Given the current hit and the current object (might be a parent of the hit object if parent looping is enabled),
    //                    this function should return true to keep the hit as a result or false to discard it
    //                    If null: All hits are accepted as results
    // searchMultiple   - If false: The search stops after the first successful result, only returns 1 or 0 results
    // parentsWhitelist - If true: Whitelist: Searches through all parents of the hit object and returns any parent that matches the filter
    //                    If false: Blacklist: Only returns a hit if none of its parents match the filter
    //                    If null: Doesn't search through the hit object's parents at all
    //
    // Returns: A list of all of the results. Each result is a tuple of the hit and gameObject that passed the filter.
    public static List<(RaycastHit hit, GameObject gameObject)> RaycastSearch(Vector3 origin, Vector3 direction, float maxDistance,
        System.Func<RaycastHit, GameObject, bool> filter = null, bool searchMultiple = false, bool? parentsWhitelist = null)
    {
        // Initialize results as an empty list
        var results = new List<(RaycastHit, GameObject)>();
        // Do the actual raycast
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
        // Search through all of the hits
        foreach (var hit in hits)
        {
            // Find the gameObject that was hit
            GameObject hitObject = hit.collider?.gameObject;
            // Search all of its parent gameObjects
            bool filterMatch = false;
            GameObject currObject = hitObject;
            while (currObject != null)
            {
                // If one of the searched gameObjects matches the filter (or if there is not filer)...
                if (filter == null || filter(hit, currObject))
                {
                    filterMatch = true;
                    // If parentsWhitelist is false, then this hit just matched the blacklist, so break.
                    if (parentsWhitelist == false) break;
                    // Otherwise, add the successful object to the list of results
                    results.Add((hit, currObject));
                    // If searchMultiple is false, quit after finding the first result
                    if (!searchMultiple) return results;
                }
                // If parentsWhitelist is null, don't search the parents at all
                if (parentsWhitelist == null) break;
                // Othwerwise, move up to the next parent (the "?.parent" evalueates to null if there is no parent)
                currObject = currObject.transform?.parent?.gameObject;
            }
            // If parentsWhitelist is false, only record a hit if none of the parents matched the blacklist filter
            if (parentsWhitelist == false && filterMatch == false)
            {
                results.Add((hit, hitObject));
                // If searchMultiple is false, quit after finding the first result
                if (!searchMultiple) return results;
            }
        }
        // Return however many results were found. If no results were found then the List is just empty.
        return results;
    }
    
    // An override of RaycastSearch(Vector3 origin, Vector3 direction, float maxDistance,...). See the original for details
    public static List<(RaycastHit hit, GameObject gameObject)> RaycastSearch(Vector3 origin, Vector3 destination,
        System.Func<RaycastHit, GameObject, bool> filter = null, bool searchMultiple = false, bool? parentsWhitelist = null)
        => RaycastSearch(origin, (destination - origin).normalized, (destination - origin).magnitude, filter, searchMultiple, parentsWhitelist);
}



// A logic gate that can be applied to a list of booleans
public enum LogicGate
{
    AND,
    OR,
    NAND,
    NOR
}
public static class LogicGateExtensions
{
    public static bool Apply(this LogicGate gate, List<bool> inputs)
    {
        foreach (bool input in inputs)
        {
            if (gate == LogicGate.AND && !input) return false;
            if (gate == LogicGate.NAND && !input) return true;
            if (gate == LogicGate.OR && input) return true;
            if (gate == LogicGate.NOR && input) return false;
        }
        return gate == LogicGate.AND || gate == LogicGate.NOR;
    }
}

// A Comparison that can be applied to two IComparables
public enum Comparison
{
    
    EQ, // Equal
    LT, // Less Than
    GT, // Greater Than
    NEQ, // Not Equal
    LTEQ, // Less Than or Equal
    GTEQ, // Greater Than or Equal
}
public static class ComparisonExtensions
{
    public static bool Apply(this Comparison c, IComparable a, IComparable b)
    {
        if (a == null || b == null) return false;
        if (c == Comparison.EQ) return a.CompareTo(b) == 0;
        if (c == Comparison.LT) return a.CompareTo(b) < 0;
        if (c == Comparison.GT) return a.CompareTo(b) > 0;
        if (c == Comparison.NEQ) return a.CompareTo(b) != 0;
        if (c == Comparison.LTEQ) return a.CompareTo(b) <= 0;
        if (c == Comparison.GTEQ) return a.CompareTo(b) >= 0;
        // This point should never be reached
        return false;
    }
    public static string Display(this Comparison c)
    {
        if (c == Comparison.EQ) return "=";
        if (c == Comparison.LT) return "<";
        if (c == Comparison.GT) return ">";
        if (c == Comparison.NEQ) return "!=";
        if (c == Comparison.LTEQ) return "<=";
        if (c == Comparison.GTEQ) return ">=";
        // This point should never be reached
        return "???";
    }
}