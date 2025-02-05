using Recorder.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop.Models;
public static class DataLoader
{
    public static Session LoadSessionFromFile(string path)
    {
        using StreamReader reader = new StreamReader(path);
        Session session = JsonSerializer.Deserialize<Session>(reader.ReadToEnd());
        return session;
    }
}
