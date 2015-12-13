# My Words
  - Oh boy! This was definitely interesting and that's why I am writing "My Words" :-)
  - Tried to keep it as simple as it possible.
  - Don't ask me if it's production ready, it's not. But it's in assembly line of production.

# What Manager wants and What I believe
   - [Manager] => wants the WAY of processing customer file asynchronously via web interface
   - [I] => believe this is a long running process that should be done via windows service or scheduled console service. However, I do believe user has every rights to see the update notification via web. So, what manager demanded is definitely prcatical.

# What I did
   - To simplify the solution, I am using TPL and calling "Fire and Forget" task which eventually broadcast the result of the each customer records via SignalR (websocket)
   - Next page ("File Processor") after upload websocket connection is established via SignalR javascript library, and then records of uploaded file will be started processing one by one.
   - To broadcast result to the user that has started the upload, fake user identity via login page is used that uses sessionID as an user name! So, if user open browser window/tab with the same session, he should be able to get the notification.
   - When the process starts, the reference of files being process session will be stored. So, even if the page is refreshed, process won't start again but user will still get the notification.
 
# Limitation of solution (due to time constraints)
- Comments are yet to enter for each method.
- Unit tests are not created.
- Honestly, some of the methods need minor refectoring to make it enable to unit test. 
- Propert exception handling module isn't integrated as this being a demo.

# If not satisfied with the Solution
   - Call me!

# If satisfied
- Do call me!
