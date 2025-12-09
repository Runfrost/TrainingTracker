using Dynastream.Fit;
using System.Text.Json;

public static class FitToJson
{
    public static string ExtractSessionToJson(Stream fitStream)
    {
        var decode = new Decode();
        var mesgBroadcaster = new MesgBroadcaster();

        // FIT files can contain multiple sessions, so we use a list
        var sessions = new List<Dictionary<string, object>>();

        // Listen for session messages
        mesgBroadcaster.SessionMesgEvent += (sender, e) =>
        {
            Mesg mesg = e.mesg;

            var fields = new Dictionary<string, object>();

            foreach (var field in mesg.Fields)
            {
                fields[field.GetName()] = field.GetValue();
            }

            sessions.Add(fields);
        };

        decode.MesgEvent += mesgBroadcaster.OnMesg;
        decode.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;

        decode.Read(fitStream);

        // Convert the session list to JSON
        return JsonSerializer.Serialize(sessions, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}
