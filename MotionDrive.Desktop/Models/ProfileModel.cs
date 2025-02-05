using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Models;
public class ProfileModel
{
    public string userName { get; set; }
    public DateTime memberSince { get; set; }
    public string? profilePictureURL { get; set; }
    public string? country { get; set; }
    public string? biography { get; set; }
    public string? favouriteCar { get; set; }
    public string? favouriteTrack { get; set; }
    public string? favouriteGame { get; set; }
    public string? favouriteCarClass { get; set; }
    public bool canEdit { get; set; }
}
