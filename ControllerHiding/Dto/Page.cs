using System.Collections.Generic;

namespace ControllerHiding.DTO
{
    public class Page
    {
        public string Name { get; set; }
        public string RouteName { get; set; }
        public Page ParentPage { get; set; }
        public List<ModuleArea> ModuleAreas { get; set; }
        public string RoutePath
        {
            get
            {
                if (ParentPage == null)
                {
                    return RouteName;
                }
                var parentRoutePath = ParentPage.RoutePath;
                if (!string.IsNullOrEmpty(parentRoutePath))
                {
                    return parentRoutePath + "/" + RouteName;
                }
                return RouteName;
            }
        }
    }
}