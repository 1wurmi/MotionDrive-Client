using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Models;
public class ResponseModel
{
    public string? jwt { get; set; }
    public string? refresh { get; set; }
    public bool successFull { get; set; }
    public string? expiresAt { get; set; }
    public string? error { get; set; }

    public IEnumerable<string>? errors { get; set; }
}
