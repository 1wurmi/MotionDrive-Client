using MotionDrive.Desktop.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.SecrectsConfig;
public class SecretConfigModel
{
    public string? JWT { get; set; }
    public string? RefreshToken { get; set; }
    public string? ExpiresAt { get; set; }
}
