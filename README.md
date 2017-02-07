# ControllerHiding

##What is it for? 
It is an example implementation of a "Parent-Child"-Controller system. 
Imagine we have a single controller ("HomeController") which is called on every page request. But we also want to work with other controllers as parts of the page and use them to display forms for example ("HiddenFormController" and "AnotherHiddenFormController"). 

If we would only want to display a controller action result, it would be easy to accomplish by using <code>@Html.Action("Index", "HiddenForm")</code>. 
But we want to use a form and display action result as child part of the "Home"-Controller again. And that it is actually a bit more tricky.

This is mainly what this implementation solves.

##Example explanation
The example constists of two parts. The first part is shown on the first page which is split in two sections: 

On top we have a default plain form without any changes. 

![standard controller](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/standard_controller.jpg)

If push the button we end up on a new blank page without the nice design and all "Parent"-Controllers which we loaded on the previous page.

![standard controller result](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/standard_controller_result.jpg)


Instead, the bottom part demonstrates the functionality of this project. 

![hidden child controller](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/hidden_child_controller.jpg)

Once we pushed a button we'll see the following result, staying on the same page:

![hidden child_controller result](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/hidden_child_controller_result.jpg)
