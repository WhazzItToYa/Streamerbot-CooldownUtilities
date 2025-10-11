# Custom Cooldown Utilities for Streamer.bot

Sometimes you want to apply cooldowns to more than just commands, or have more control over exactly what happens when something is on cooldown.  The actions in this import let you apply cooldowns within your own actions:
* Cooldowns on running an action, global and per-user.
* Cooldowns on user-defined labels, global and per-user.
  * For example, if there are multiple actions that you want participating in the same cooldown, then you can give them a common group name that the cooldown will apply to.
* Instead of silently aborting the action, send a "On cooldown" message to chat. 

## Requirements

This version only supports Streamer.bot 1.0.0 or higher. [Click Here](https://github.com/WhazzItToYa/Streamerbot-CooldownUtilities/tree/35ca6e6cfd98a2cac7a0f2dc343df78c93c27570) for the last version which supported 0.2.8.

## Installation

* Click on [ActionCooldown.sb](https://raw.githubusercontent.com/WhazzItToYa/Streamerbot-CooldownUtilities/refs/heads/main/ActionCooldown.sb) and [import it into Streamer.bot](https://docs.streamer.bot/guide/import-export#import) as usual.

## Usage

There are a number of arguments that control the behavior of the cooldown that you'll need to set:
* `cooldownSecs`: (optional) the number of seconds of cooldown.  The default is 10 seconds if not supplied.
* `cooldownGroup`: (required if using one of the Group cooldowns)  The common group name for all the actions that will share the same cooldown.
* `cooldownBehavior`: (optional) Specifies what to do if the action is on cooldown.
  * `break` : (the default) abort the action.
  * `continue` : continue executing the action, and set the argument `cooldownRemaining` to the number of seconds remaining in the cooldown. 0 means that the action was not in cooldown (but it is now).
  * `check` : Only checks if the cooldown is currently in effect, without triggering it. Sets `cooldownRemaining` just as with the "continue" option.

Then use Run Action (run immedately = checked) to run one of the following actions, depending on the type of cooldown you want:
* "Apply Action Cooldown" for a global cooldown on the running action.
* "Apply Action User Cooldown" for a user-specific cooldown on the running action.
* "Apply Shared Group Cooldown" for a global cooldown applying to anything using the same `%cooldownGroup%`
* "Apply User Shared Group Cooldown" for a user-specific cooldown applying to anything using the same `%cooldownGroup%`

## Notes
The default cooldown is 10 seconds.  You can skip the "Set Argument" of "actionCooldownSecs" above if that is your desired cooldown.

It is only accurate down to 1 second.  E.g., if you set the cooldown to 5 seconds, the action will be allowed to run again after 4 - 5 seconds.

This cooldown applies only to the initial action invoked by a trigger.  If you place this in a secondary action invoked by the initial action, the cooldown will only apply to that initial action.

Cooldowns longer than 5 minutes will be persisted, so that if Streamer.bot crashes or is closed, cooldowns started before the shutdown will still be honored.

User cooldowns can only be applied to actions that set the `%userName%` & `%userType%` arguments.  It should work for Twitch, Trovo, Kick, and YouTube, but has only been tested on Twitch.  Please let me know if it does or doesn't work on the other platforms.

