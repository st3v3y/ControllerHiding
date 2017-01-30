using System;
using System.Collections.Generic;
using System.Linq;
using ControllerHiding.DTO;
using ControllerHiding.Routing;

namespace ControllerHiding.Repositories
{
    public class PageRepository
    {
        private readonly List<Page> _pages;

        public PageRepository()
        {
            _pages = new List<Page>();
            var home = new Page()
            {
                Name = "Home",
                RouteName = "",
                ParentPage = null,
                ModuleAreas = new List<ModuleArea>
                {
                    new ModuleArea
                    {
                        Name= "TopArea",
                        Modules = new List<Module> { new Module { Controller = "StandardForm", Action = "Index", Identifier = new Guid("A53EF782-C9AB-476C-ABA4-B0946F907512").ToString() } }
                    },
                    new ModuleArea
                    {
                        Name= "BottomAreaLeft",
                        Modules = new List<Module> { new Module { Controller = "HiddenForm", Action = "Index", Identifier = new Guid("02A9152F-FF90-4A4D-9334-98948E60F834").ToString() } }
                    },
                    new ModuleArea
                    {
                        Name= "BottomAreaCenter",
                        Modules = new List<Module> { new Module { Controller = "HiddenForm", Action = "Index", Identifier = "02A9152F-FF90-4A4D-9334-98948E60F834" } }
                    },
                    new ModuleArea
                    {
                        Name= "BottomAreaRight",
                        Modules = new List<Module> { new Module { Controller = "AnotherHiddenForm", Action = "Index", Identifier = "Another-identification-string" } }
                    }
                }
            };
            var blog = new Page()
            {
                Name = "Blog",
                RouteName = "Blog",
                ParentPage = home,
                ModuleAreas = new List<ModuleArea>
                {
                    new ModuleArea
                    {
                        Name = "LeftContent",
                        Modules = new List<Module> { new Module { Controller = "Blog", Action = "BlogEntryList", Identifier = "BlogList"} }
                    },
                    new ModuleArea
                    {
                        Name = "RightContent",
                        Modules = new List<Module> { new Module { Controller = "Comment", Action = "Latest", Identifier = "LatestComments"} }
                    }
                }
            };

            var blogEntry = new Page()
            {
                Name = "Blog Entry",
                RouteName = "BlogEntry",
                ParentPage = blog,
                ModuleAreas = new List<ModuleArea> {
                    new ModuleArea
                    {
                        Name = "LeftContent",
                        Modules = new List<Module>
                        {
                            new Module { Controller = "Blog", Action = "BlogEntry", Identifier = "Entry"},
                            new Module { Controller = "Comment", Action = "CommentList", Identifier = "Entry"}
                        }
                    },
                    new ModuleArea
                    {
                        Name = "RightContent",
                        Modules = new List<Module> { new Module { Controller = "Comment", Action = "Latest", Identifier = "LatestComments"} }
                    }
                }
            };

            var latestEntry = new Page()
            {
                Name = "Latest Entry",
                RouteName = "LatestEntry",
                ParentPage = blog,
                ModuleAreas = new List<ModuleArea> {
                    new ModuleArea
                    {
                        Name = "LeftContent",
                        Modules = new List<Module>
                        {
                            new Module { Controller = "Blog", Action = "BlogEntry", Identifier = "Entry"},
                            new Module { Controller = "Comment", Action = "CommentList", Identifier = "Entry"}
                        }
                    },
                    new ModuleArea
                    {
                        Name = "RightContent",
                        Modules = new List<Module> { new Module { Controller = "Comment", Action = "Latest", Identifier = "LatestComments"} }
                    }
                }
            };

            _pages.AddRange(new []
            {
                home,
                blog,
                blogEntry,
                latestEntry
            });
        }

        public Page GetPageByRoute(string routeName)
        {
            return _pages.FirstOrDefault(x => x.RouteName == routeName);
        }

        public List<Page> GetAllPages()
        {
            return _pages;
        }
    }
}