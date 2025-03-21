using System;

public class CPHInline
{
    public bool Execute() {
        return ApplyActionCooldown();
    }

    ////////////////////////////////////////
    //
    // Action cooldowns
    //

    public bool ApplyActionCooldown()
    {
    	var actionId = args["actionId"].ToString();
        return ApplyCooldown($"actionId-{actionId}", GetGlobal, SetGlobal);
    }

    public bool ApplyUserActionCooldown()
    {
    	var actionId = args["actionId"].ToString();
        return ApplyCooldown($"actionId-{actionId}", GetUserVar, SetUserVar);
    }

    ////////////////////////////////////////
    //
    // Custom cooldowns
    //
    public bool ApplyCooldownGroup()
    {
        return ApplyCooldownGroupInternal(GetGlobal, SetGlobal);
    }
    
    public bool ApplyUserCooldownGroup()
    {
        return ApplyCooldownGroupInternal(GetUserVar, SetUserVar);
    }
    
    private bool ApplyCooldownGroupInternal(Func<string, bool, long> getter,
                                            Action<string, long, bool> setter)
    {
    	if (!CPH.TryGetArg("actionCooldownGroup", out string groupName))
        {
            if (!CPH.TryGetArg("cooldownGroup", out groupName))
            {
                CPH.LogError("SHARED GROUP COOLDOWN: You have attempted to apply a Shared Group Cooldown without providing the group's name in the %actionCooldownGroup% argument.");
                return false;
            }
        }
        return ApplyCooldown($"group-{groupName}", getter, setter);
    }

    // Evaluates and enforces the cooldown for a given key
    private bool ApplyCooldown(string keyName,
                               Func<string, bool, long> getter,
                               Action<string, long, bool> setter)
    {
    	long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    	if (!CPH.TryGetArg("cooldownSecs", out long cooldown))
        {
            // Be backward-compatible with original argument
            if (!CPH.TryGetArg("actionCooldownSecs", out cooldown)) cooldown = 10;
        }

    	// If we have to keep track of cooldowns for an extended time, persist them in case
    	// we get restarted in the meantime.
    	var varName = $"lastExecuted-{keyName}";
    	bool persistCooldown = (cooldown > 300);
    	long lastExecuted = getter(varName, persistCooldown);
        long elapsed = now-lastExecuted;
    	if (elapsed >= cooldown) {
            CPH.SetArgument("cooldownRemaining", 0);
            setter(varName, now, persistCooldown);
            return true;
        }
        CPH.SetArgument("cooldownRemaining", cooldown-elapsed);

    	CPH.TryGetArg("cooldownBehavior", out string behavior);
        switch (behavior) {
            case "continue":
                return true;
            case "break":
            default:
                return false;
        }
    }

    private long GetGlobal(string varName, bool persisted)
    {
    	return CPH.GetGlobalVar<long>(varName, persisted);
    }
    
    private void SetGlobal(string varName, long val, bool persisted)
    {
        CPH.SetGlobalVar(varName, val, persisted);
    }
    

    private long GetUserVar(string varName, bool persisted)
    {
    	if (!CPH.TryGetArg("userName", out string userName)) userName = "nobody";
    	string platform = args["userType"].ToString();
    	switch (platform) {
            case "trovo":   return CPH.GetTrovoUserVar<long>(userName, varName, persisted);
            case "youtube": return CPH.GetYouTubeUserVar<long>(userName, varName, persisted);
            case "twitch":  return CPH.GetTwitchUserVar<long>(userName, varName, persisted);
        }
        return 0;
    }
    private void SetUserVar(string varName, long value, bool persisted)
    {
    	if (!CPH.TryGetArg("userName", out string userName)) userName = "nobody";
    	string platform = args["userType"].ToString();
    	switch (platform) {
            case "trovo":   CPH.SetTrovoUserVar(userName, varName, value, persisted); break;
            case "youtube": CPH.SetYouTubeUserVar(userName, varName, value, persisted); break;
            case "twitch":  CPH.SetTwitchUserVar(userName, varName, value, persisted); break;
        }
    }
}

