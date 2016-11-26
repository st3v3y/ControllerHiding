# ControllerHiding

##What is it for? 
It is an example implementation of a "Master-Child"-Controller system. 
We have a single controller ("HomeController") which is called everytime. But we also want to work with other controllers, and use them for displaying forms for example ("HiddenFormController"). 
If we only want to display a controller, it's easy to accomplish by using <code>@Html.Action("Index", "HiddenForm")</code>. 
But if we want to use a form and display the "Home"-Controller again it is a bit more complicated.

This is what this implementation solves.
