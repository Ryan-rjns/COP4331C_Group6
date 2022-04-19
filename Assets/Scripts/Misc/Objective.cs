using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Each level has an objective that is composed of several tasks
// If all of the WinTasks are true, then the player wins the level
// If ANY of the LooseFlags become true, then the player looses the level
// If the player wins the level, they can recieve extra rewards based on the BonusTasks
public class Objective
{
    private bool enabled = false;
    public string Name { get; set; }
    public string Description { get; set; }
    private List<Flag> winTasks = new List<Flag>();
    private List<Flag> looseTasks = new List<Flag>();
    private List<Flag> bonusTasks = new List<Flag>();
    private List<Flag> AllTasks
    {
        get
        {
            List<Flag> flags = new List<Flag>();
            flags.AddRange(winTasks);
            flags.AddRange(looseTasks);
            flags.AddRange(bonusTasks);
            return flags;
        }
    }

    public Objective(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public Objective Enable()
    {
        Debug.Log($"Objective {Name} is enabled");
        // Enable win/loose updates
        enabled = true;
        // Add CheckTasks to all of the flags
        foreach (Flag f in AllTasks)
        {
            f.AddAction(CheckTasks);
        }
        return this;
    }
    public Objective Disable()
    {
        Debug.Log($"Objective {Name} is disabled");
        // Prevent win/loose updates
        enabled = false;
        // Remove CheckTasks from all of the flags
        foreach (Flag f in AllTasks)
        {
            f.RemoveAction(CheckTasks);
        }
        return this;
    }

    // Adds the given flag as a task for this objective
    // isWinTask==True:  This is a task that is required to win
    // isWinTask==False: If this flag brecomes true, the player immediately looses
    // isWinTask==null:  This is a bonus task and does not count towards winning or loosing
    public Objective Task(Flag task, bool? isWinTask = true)
    {
        if (task == null) return this;
        List<Flag> taskList = isWinTask == true ? winTasks : (isWinTask == false ? looseTasks : bonusTasks);
        taskList.Add(task);
        if(enabled) task.AddAction(CheckTasks);
        Debug.Log($"Objective {Name} added a new task {task}");
        return this;
    }

    private void CheckTasks(WatchVar<bool> source = null)
    {
        Flag sourceFlag = source as Flag;
        if (sourceFlag != null) Debug.Log($"Objective {Name} checking updated task {sourceFlag} with value {sourceFlag.Value}");

        if (!enabled) return;
        // If there is at least one wintask, then the player has the opportunity to win
        bool win = winTasks.Count > 0;
        foreach (Flag wt in winTasks)
        {
            if (wt == null) continue;
            // If any winTask is false, then the player did not win
            if (!wt.Value)
            {
                win = false;
                break;
            }
        }
        // If the player won, don't bother checking the loose conditions (can't win and loose at the same time)
        if (win)
        {
            Debug.Log($"Objective {Name}: Player Wins :)");
            GameManager.Win();
            return;
        }

        foreach (Flag lt in looseTasks)
        {
            if(lt == null) continue;
            // If ANY looseTask is ever true, then the player immediately looses
            if(lt.Value)
            {
                Debug.Log($"Objective {Name}: Player Looses :(");
                GameManager.Lose();
                return;
            }
        }
    }

}

// A LimitFlag is a Flag that tracks a target WatchVar<T> and checks it against a given limit using a Comparison
public class LimitFlag<T> : Flag where T : IEquatable<T>, IComparable
{
    // The variable to track
    private WatchVar<T> var;
    // The limit to check against
    private T limit;
    // The comparison to check with
    Comparison comparison;

    // Constructs a new LimitFlag that is tracking 'var' and checks it agianst 'limit' using 'comparison'
    // This LimitFlag's state is always equal to "var comparison limit" (ex: 5 GT 4 checks 5>4 which is false)
    public LimitFlag(WatchVar<T> var, Comparison comparison, T limit)
    {
        SetLimit(var, limit, comparison);
    }
    private void SetLimit(WatchVar<T> var, T limit, Comparison comparison)
    {
        if (var == null || limit == null) return;
        this.var = var;
        this.limit = limit;
        this.comparison = comparison;
        // track the var and check the limit whenever the var changes
        var.AddAction(CheckLimit);
        // Initialize the limit (is it already true?)
        Value = false;
        CheckLimit();
    }
    // Called when the tracked var changes; evaluates the comparison and checks if it changed
    // (source is ignored, but it must be present to make this a valid delegate)
    private void CheckLimit(WatchVar<T> source = null)
    {
        if (var == null) return;
        // WatchVar handles whether or not the ActionBundle should be called
        Value = comparison.Apply(var.Value, limit);
    }

    public override string GetProgressIndicator()
    {
        return $"({var?.Value?.ToString()??"null"}{comparison.Display()}{limit})";
    }
}

// A LogicFlag is a Flag with a logic gate and a list of other flags.
// The LogicFlag then continuously updates its state to match the result of the logic gate
// This can be used to chain an infinite number of Flags together into one trigger
// i.e. ("Boss 1 Killed" AND ("Boss 2 Killed" OR "Boss 3 Killed"))
public class LogicFlag : Flag
{
    List<Flag> logicFlags = new List<Flag>();
    LogicGate logicGate = LogicGate.AND;

    // Constructs a LogicFlag from the specified LogicGate and a list of other flags
    public LogicFlag(LogicGate gate, params Flag[] flags)
    {
        if (flags == null || flags.Length == 0) return;
        SetLogic(new List<Flag>(flags), gate);
    }
    private void SetLogic(List<Flag> flags, LogicGate gate)
    {
        logicGate = gate;

        foreach (Flag f in flags)
        {
            if (f == null) continue;
            // Prevent infinite loops!
            if (f == this) continue;
            LogicFlag lf = f as LogicFlag;
            if (lf != null)
            {
                if (lf.HasFlag(this)) continue;
            }
            // Add the new flag
            logicFlags.Add(f);
            f.AddAction(CheckLogic);
        }
        // Initialize the logic (is it already true?)
        Value = false;
        CheckLogic();
    }
    // Called when a component falg is updated; evaluates the LogicGate and checks if it changed
    // (source is ignored, but it must be present to make this a valid delegate)
    private void CheckLogic(WatchVar<bool> source = null)
    {
        List<bool> inputs = new List<bool>();
        foreach (var e in logicFlags)
        {
            inputs.Add(e.Value);
        }
        // If the gate did not change, then this Value assignment does nothing
        // If the gate DID change, then WatchVar calls the ActionBundle
        Value = logicGate.Apply(inputs);
    }

    // Returns if the given Flag is being used as an input for this LogicFlag
    public bool HasFlag(Flag flag)
    {
        if (flag == null) return false;
        if (flag == this) return true;
        foreach (Flag f in logicFlags)
        {
            if (flag == f) return true;
            LogicFlag lf = f as LogicFlag;
            if (lf != null)
            {
                if (lf.HasFlag(flag)) return true;
            }
        }
        return false;
    }

    public override string GetProgressIndicator()
    {
        string result = $"({logicGate}:";
        foreach(Flag f in logicFlags)
        {
            if(f==null) continue;
            result += $" {f.GetProgressIndicator()}";
        }
        result += ")";
        return result;
    }
}

// A Flag is a boolean WatchVar that is the basis for objective tasks
// Flags also contain cosmetic information on how the task should be displayed to the screen
public class Flag : WatchVar<bool> 
{
    public Flag()
    {
        AddAction(EnforceLock);
    }

    public string Name { get; set; } = "NoName";

    public string Display() => $"{Name} {GetProgressIndicator()}";
    public virtual string GetProgressIndicator() => "(X)";
    public override string ToString() => Name;

    public bool? Locked { get; private set; }

    // Adds a lock to this flag that prevents it from leaving the True or False state (returns this)
    public Flag Lock(bool targetState)
    {
        Locked = targetState;
        return this;
    }
    // Removes the lock from this flag (returns this)
    public Flag Unlock()
    {
        Locked = null;
        return this;
    }
    private void EnforceLock(WatchVar<bool> ignored)
    {
        if (Locked == null || Value == Locked.Value) return;
        Value = Locked.Value;
    }
}

// A WatchVar can replace any variable, and tracks whenever that variable changes
// When the variable is changed, the WatchVar calls all of its registered functions and gives them a ref to itself
public class WatchVar<T> : ActionBundle<WatchVar<T>> where T : IEquatable<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            // Ignore assignements that don't change the value
            if (_value == null ? value == null : _value.Equals(value)) return;
            _value = value;
            CallBundle(this);
        }
    }
}

// Stores a list of Actions<T> (function delegates that take one parameter of type T)
// These actions are called whenever this bundle is activated
public class ActionBundle<T>
{
    // actions that are to be called when this is triggered
    protected List<Action<T>> bundle = new List<Action<T>>();

    // Registers a new, unique action to be added to this bundle
    // returns true if successful, false if it failed becuase the action is null or is already registered
    public bool AddAction(Action<T> action)
    {
        if (action == null) return false;
        if (bundle.Contains(action)) return false;
        bundle.Add(action);
        return true;
    }
    // Tries to remove the specified action, returns true if successful
    public bool RemoveAction(Action<T> action)
    {
        if (action == null) return false;
        if (!bundle.Contains(action)) return false;
        bundle.Remove(action);
        return true;
    }
    // Returns if the action is already registered
    public bool ContainsAction(Action<T> action)
    {
        if (action == null) return false;
        return bundle.Contains(action);
    }

    // Calls all of the actions in this bundle
    public virtual void CallBundle(T value)
    {
        if (bundle == null) return;
        // If one of the bundle's actions adds or removes actions from this bundle, the foreach loop will crash
        // If this happens, catch the exception move on
        try
        {
            foreach (var action in bundle)
            {
                action(value);
            }
        } 
        catch (InvalidOperationException)
        {
            // do nothing
        }
    }
}