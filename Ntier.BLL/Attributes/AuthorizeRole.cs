using Microsoft.AspNetCore.Mvc;
using Ntier.BLL.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.BLL.Attributes
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeRoleAttribute( string role ):base( typeof(AuthorizationFilter) )
        {
            Arguments = new object[] { role };
        }   
    }
}
