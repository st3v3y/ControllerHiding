# ControllerHiding

##What is it for? 
It is an example implementation of a "Parent-Child"-Controller system. 
Imagine we have a single controller ("HomeController") which is called on every page request. But we also want to work with other controllers as parts of the page and use them to display forms for example ("HiddenFormController" and "AnotherHiddenFormController"). 

If we would only want to display a controller action result, it would be easy to accomplish by using <code>@Html.Action("Index", "HiddenForm")</code>. 
But we want to use a form and display action result as child part of the "Home"-Controller again. And that it is actually a bit more tricky.

This is mainly what this implementation solves.

##Example explanation
The example constists of two parts. The first part demonstrates form handling. The second one demonstrates the routing.

### The first part: From handling
The first part is shown on the first page which is split in two sections: 

On top we have a default plain form without any changes. 

![standard controller](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/standard_controller.jpg)

If push the button we end up on a new blank page without the nice design and all "Parent"-Controllers which we loaded on the previous page.

![standard controller result](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/standard_controller_result.jpg)


Instead, the bottom part demonstrates the functionality of this project. 

![hidden child controller](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/hidden_child_controller.jpg)

Once we pushed a button we'll see the following result, staying on the same page:

![hidden child_controller result](https://github.com/st3v3y/Parent-Child-Controller-Routing/blob/master/hidden_child_controller_result.jpg)

### The second part: "Child"-Routing
#### What does "child"-routing mean?
It means that you can seperate your "Parent"/"Home"-Controller by several "Child"-Controllers and you still have the opportunity to give them states by using special url-routes.

*Simple example:*
Your "Home"-Controller has a "Child"-Controller which lists many blog articles and you need a paging-mechanism. You don't want to use the static MVC routing to specify a new route for the page number and you don't want to use bad looking query parameters which you have to extract from the url. So, you can just specify a "Child"-Route and catch automatically the correct page number from a clean URL like "http://mywebsite.com/blog/list/3"

#### How it works
It works! Really! That's it! ;-)

No, it is complicated to explain in a view sentences because many things correlate to each other. For a detailed answer you have to have a look into the code. For a more or less simple answer just keep on reading...

All starts with an URL like the one above (http://mywebsite.com/blog/list/3). The /blog/-Part stands for the current page. It means, we are actually still on our "Home"-Controller but we are displaying a template specially defined for the "Blog"-Page. It is also possible to define Sub-Pages like "http://mywebsite.com/blog/latest". Until this point we just call a defined template which renders some "Child"-Controllers. 

The next part of the URL is a bit more tricky: When you render a "Child"-Action you can specify an identifier. This identifier is needed to tell the route that the following parameter(s) is/are targeting this action. By specifying a "Child-Route" (by simply putting an attribute) like [ChildRoute("{year}/{pageNumber}")] you can call an URL like "http://mywebsite.com/blog/list/2017/2" which means that *all* "Child"-Controller-Actions rendered with the identifier "list" will be called with parameters year = "2017" and pageNumber = 2.
The complete action signature could look like this: 

<code>
*[ChildRoute("{year}/{pageNumber}")]*
public ActionResult BlogEntryList(int? year, int pageNumber = 1)
{
...
}
</code>

The order of multiple "Child-Routes" does not matter. An URL like "http://mywebsite.com/blog/identifier2/my-param/list/2017/2" would also work as the one above.

#### ChildRouting: Defaults
Given you have an URL like "http://mywebsite.com/blog/list/2017/2" the "year"-parameter acts like a filter. You're just displaying blog entries of 2017. In some cases it would be more convenient to use an "default" instead (which is not "-1" or "0" because it would't look very professional). So you wish to have a default like "all" with an URL like: "http://mywebsite.com/blog/list/all/2".

*Important marginal note:*
> Currently it is not possible to just leave this parameter simply away because it would break the validation whether the URL is valid! 

Then this is your action would look like: 

<code>
[ChildRoute("{year}/{pageNumber}")]
*[ChildRouteDefault("year", "all")]*
public ActionResult BlogEntryList(int? year, int pageNumber = 1)
{
...
}
</code>

#### ChildRouting: Constraints
Very annoying: Your colleague played around a little bit and found out he can also call your page with "http://mywebsite.com/blog/list/1955/2" and it displays the same page but with an empty list. He put this link on his website and laughs about you because google has already indexed this URL from his website and it is ranked in the first place. Damn! (Not really realistic scenario but actually what could happen)
So, it's too late to remove the Google index immediately. But next time you can prevent the mistake by simply defining a constraint: 

<code>
[ChildRoute("{year}/{pageNumber}")]
[ChildRouteDefault("year", "all")]
*[ChildRouteBlogEntryYearConstraint("year")]*
public ActionResult BlogEntryList(int? year, int pageNumber = 1)
{
...
}
</code>

<code>
public class ChildRouteBlogEntryYearConstraintAttribute : ChildRouteConstraintAttribute
{
    public ChildRouteBlogEntryYearConstraintAttribute(string routeParamName) 
        : base(routeParamName)
    {
    }

    public override bool IsValidRouteValue(object routeValue)
    {
        var value = routeValue as int?;
        if (value == null)
        {
            return false;
        }
        return new BlogEntryRepository().GetAllBlogEntries().Any(x => x.Date.Year == value.Value);
    }
}
</code>
