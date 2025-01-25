using Godot;
using System;
using System.Text.Json;
using System.Linq;

[Tool]
[GlobalClass]
public partial class DialogueData : Resource
{
    [Export] public Texture2D backgroundImage;
    [Export] public ConversionGroup[] conversionGroups;

    public string ToJson()
    {
        var data = new DialogueJsonData
        {
            backgroundImagePath = backgroundImage?.ResourcePath ?? "",
            conversionGroups = Array.ConvertAll(conversionGroups, group => new ConversionGroupJson
            {
                id = group.id,
                backgroundImagePath = group.backgroundImage?.ResourcePath ?? "",
                conversions = Array.ConvertAll(group.conversions, conv => new ConversionJson
                {
                    type = conv.type,
                    character = conv.character,
                    text = conv.text,
                    backgroundImagePath = conv.backgroundImage?.ResourcePath ?? "",
                    action = conv.action,
                    jumpTo = conv.jumpTo,
                    options = conv.options != null ? Array.ConvertAll(conv.options, opt => new ConversionOptionJson
                    {
                        type = opt.type,
                        text = opt.text,
                        action = opt.action,
                        jumpTo = opt.jumpTo
                    }) : null
                })
            })
        };

        return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    }

    public static DialogueData FromJson(string json)
    {
        var jsonData = JsonSerializer.Deserialize<DialogueJsonData>(json);
        var dialogueData = new DialogueData
        {
            backgroundImage = string.IsNullOrEmpty(jsonData.backgroundImagePath) ? null : GD.Load<Texture2D>(jsonData.backgroundImagePath),
            conversionGroups = Array.ConvertAll(jsonData.conversionGroups, groupJson => new ConversionGroup
            {
                id = groupJson.id,
                backgroundImage = string.IsNullOrEmpty(groupJson.backgroundImagePath) ? null : GD.Load<Texture2D>(groupJson.backgroundImagePath),
                conversions = Array.ConvertAll(groupJson.conversions, convJson => new Conversion
                {
                    type = convJson.type,
                    character = convJson.character,
                    text = convJson.text,
                    backgroundImage = string.IsNullOrEmpty(convJson.backgroundImagePath) ? null : GD.Load<Texture2D>(convJson.backgroundImagePath),
                    action = convJson.action,
                    jumpTo = convJson.jumpTo,
                    options = convJson.options?.Select(opt => new ConversionOption
                    {
                        type = opt.type,
                        text = opt.text,
                        action = opt.action,
                        jumpTo = opt.jumpTo
                    }).ToArray()
                })
            })
        };
        return dialogueData;
    }
}

// JSON data structure classes
[Serializable]
public class DialogueJsonData
{
    public string backgroundImagePath { get; set; }
    public ConversionGroupJson[] conversionGroups { get; set; }
}

[Serializable]
public class ConversionGroupJson
{
    public string id { get; set; }
    public string backgroundImagePath { get; set; }
    public ConversionJson[] conversions { get; set; }
}

[Serializable]
public class ConversionJson
{
    public ConversionType type { get; set; }
    public CharacterT character { get; set; }
    public string text { get; set; }
    public string backgroundImagePath { get; set; }
    public string action { get; set; }
    public string jumpTo { get; set; }
    public ConversionOptionJson[] options { get; set; }
}

[Serializable]
public class ConversionOptionJson
{
    public ConversionOptionType type { get; set; }
    public string text { get; set; }
    public string action { get; set; }
    public string jumpTo { get; set; }
}
