using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Models;
public class Friend : INotifyPropertyChanged
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string ProfilePictureURL { get; set; }

    private bool _isOnline = false;
    public bool IsOnline
    {
        get => _isOnline;
        set
        {
            if (_isOnline != value)
            {
                _isOnline = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class FriendRequest
{
    public string RequestId { get; set; }
    public string UserName { get; set; }
    public string ProfilePictureURL { get; set; }
}
