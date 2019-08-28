using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace ABM_CMS
{
    public class MyViewLocationExpander : IViewLocationExpander  
    {  
  
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,   
            IEnumerable<string> viewLocations)  
        {  
  
            //replace the Views to MyViews..  
            viewLocations = viewLocations.Select(s => s.Replace("Views", "Pages"));  
  
            return viewLocations;  
        }  
  
        public void PopulateValues(ViewLocationExpanderContext context)  
        {  
            //nothing to do here.  
        }  
    } 
}