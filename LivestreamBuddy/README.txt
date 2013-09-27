LivestreamBuddy version 0.5.9 Beta
https://sourceforge.net/projects/livestreambuddy/

Changes:
- Chat will now display emoticon images.  This has the potential to be resource intensive on slower machines.  You can disable it by running LivestreamBuddy with the -noemote flag.

Fixes:
- Stuck on "Joining Chat...".  Twitch changed their IRC authentication process.  LivestreamBuddy now mirrors that change.

--- version 0.5.8 Beta ---

Changes:
- Channel field auto suggest
- Very basic identity manager (replaces the current username and password fields).  This will be expanded in the future.
- View stream ability
- No more soul sucking WinForms RTF.  All replaced with custom GeckoFX (embedded firefox) controls.

--- version 0.5.7 Beta ---

Changes:
- Switched from the built-in .Net WebBrowser control to GeckoFx.  Yields lower memory usage and behaves better with the Twitch Web API.

Fixes:
- Auto suggest memory leak.
- Multibyte characters now work in the chat.  (Switched from system default encoding to UTF-8 for the IRC client)

--- version 0.5.6 Beta ---

Changes:
- Clicking on web links in the chat window will now open a browser for you.

Fixes:
- Error popup when typing in the stream title or game fields.  Auto suggest bug.

--- version 0.5.5 Beta ---

Changes:
- Can now update stream titles/games that you do not own (still have to be an editor)
- Added auto suggest for the stream title and game fields.
- Can now run commercials

--- version 0.5.4 Beta ---

Fixes:
- Closing the application before disconnecting no longer leaves the process running.

Changes:
- Added channel/stream title and game updating ability
- Added Help "system"
- Switched the chat windows to a RTF control.  Pretty colors!

--- version 0.5.3 Beta ---

Fixes:
- Found and fixed a race condition when disconnecting and reconnecting.

Changes:
- New UI for twitch auth
- New UI for channel/stream info

--- version 0.5.2 Beta ---

Changes:
- Connection, login, channel join, and viewer retrieval status notifications
- Improved overall connection stability
- General performance optimizations

--- version 0.5.1 Beta ---

Changes:
- Switched to use a more reliable twitch.tv IRC URL

--- version 0.5.0 Beta ---

This is the initial release of this software.

General features:
- Allows twitch/justin chat interation
- Provides a channel viewer list and count
- Very basic giveaway/random picker feature