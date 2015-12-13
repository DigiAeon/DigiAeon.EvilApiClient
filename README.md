# My Words
  - Oh boy! This was definitely interesting and that's why I am writing "My Words" :-)
  - Tried to keep it as simple as it possible.
  - Don't ask me if it's production ready, it's not. But it's in assembly line.

# What Manager wants and What I believe
   - [Manager] => wants the WAY of processing customer file asynchronously via web interface.
   - [I] => believe this is a long running process that should be done via windows service or scheduled task. However, I do believe user has every rights to see the notifications via web interface.

# What I did
   - To simplify the solution, I am using TPL and calling "Fire and Forget" tasks that eventually broadcast the result of the each customer records via SignalR (websocket)
   - On next page after upload, websocket connection is established via SignalR javascript library, and then processing of customer reords from the uploaded file will be started.
   - To broadcast result to the user that has started the upload, fake user identity via login page is used. FYI, SessionID is used as an user name! So, if user open browser window/tab with the same session, he should be able to get the notification.
   - When the process starts, the reference of files being processed will be stored in session. So, even if the page is refreshed, process won't start again but user will still get the notification.
 
# Limitation of solution (as it's just a demo solution)
- Won't find much comments in code.
- Unit tests are not created.
- No exception handling module is integrated.
- No custom error page.

# If not satisfied
   - Call me!

# If satisfied
- Do call me!
