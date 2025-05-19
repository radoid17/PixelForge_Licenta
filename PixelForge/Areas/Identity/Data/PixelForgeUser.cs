using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PixelForge.Areas.Identity.Data;

// Add profile data for application users by adding properties to the PixelForgeUser class
public class PixelForgeUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; }
    [PersonalData]
    public string SecondName{ get; set; }
}

