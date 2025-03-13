using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Models;
public class Friend
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string ProfilePictureURL { get; set; }
    public bool IsOnline { get; set; } = false;
}

public class FriendRequest
{
    public string RequestId { get; set; }
    public string UserName { get; set; }
    public string ProfilePictureURL { get; set; }
}
