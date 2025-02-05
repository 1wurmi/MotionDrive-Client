using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Models;
public class LoggedInUserData
{
    public string _name;
    public string Name
    {
        get => _name;
        set => _name = value;
    }
}
