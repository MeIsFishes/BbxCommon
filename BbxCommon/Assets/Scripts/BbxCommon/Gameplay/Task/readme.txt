Task is a lightweight and full-lifecycle function executer, just like a playable script in Timeline. It mainly includes the following features:
  1. Self ticked. That means you can create a Task without hanging it on any manager to tick it.
  2. Context independent. A Task can be initialized externally once and then never relies on other context.
  3. Combining 1 and 2, a Task can manage the whole lifecycle itself, which means you can code some reusable functions as Task and simply create it if you need.
  4. Easily extend. However, interfaces in Task are public for interacting with other modules. There has also been a tool TaskTimeline for helping building heavier pipeline.

TaskTimeline is more like a Timeline of Unity, it processes time-related Task creating requests and holds their references. TaskTimeline can be set a shared context, manage
lifecycle of Tasks it created and interact with them.