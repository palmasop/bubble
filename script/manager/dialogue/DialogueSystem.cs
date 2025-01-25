using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class DialogueSystem : Singleton<DialogueSystem>
{
    [Export] CanvasLayer dialogueUI;
    [Export] Label conversionLabel;
    [Export] TextureRect charImage;
    [Export] GridContainer optionContainer;
    [Export] TextureRect backgroundImageRect;

    [ExportGroup("Data")]
    [Export] Character[] characters = { new("Player") };
    Dictionary<CharacterT, Character> charactersMap = new();

    Dictionary<string, ConversionGroup> conversionGroups = new();
    Queue<Conversion> queue = new();
    readonly List<Button> optionBts = new(4);
    List<ConversionOption> options = new();
    Dictionary<DialogueData, System.Action> completionCallbacks = new();

    Conversion curConversion;
    bool inConversion;
    bool disabledShowNext;
    bool disablePause;

    #region Typewriter
    string targetText = "";
    string currentDisplayText = "";
    float typeWriterSpeed = .02f;
    float typeWriterTimer = 0;
    bool isTyping = false;
    #endregion

    #region Setup

    public override void _EnterTree()
    {
        base._EnterTree();
        characters.ToList().ForEach(character => charactersMap.Add(character.type, character));
    }

    public override void _Ready()
    {
        base._Ready();
        OptionSetup();
        // CloseConversion();
    }

    public override void _Process(double delta)
    {
        if (!inConversion) return;

        if (!isTyping) return;

        typeWriterTimer += (float)delta;
        if (typeWriterTimer >= typeWriterSpeed)
        {
            typeWriterTimer = 0;
            if (currentDisplayText.Length < targetText.Length)
            {
                currentDisplayText += targetText[currentDisplayText.Length];
                conversionLabel.Text = currentDisplayText;
            }
            else
                isTyping = false;
        }
    }

    void Reset()
    {
        conversionGroups = new();
        inConversion = false;
        curConversion = null;
        queue.Clear();
        resetOption();
        updateBackgroundImage(null, true);
    }

    void OptionSetup()
    {
        optionContainer.Hide();
        if (optionBts.Count != 0)
            return;
        int btIndex = 0;
        foreach (var child in optionContainer.GetChildren())
        {
            if (child is Button bt)
            {
                optionBts.Add(bt);
                int currentIndex = btIndex; // Capture the current value
                bt.Pressed += () => OptionOnClick(currentIndex);
                btIndex++;
            }
        }
    }

    #endregion

    void Pause(bool disablePause)
    {
        ProcessMode = ProcessModeEnum.Always;
        this.disablePause = disablePause;
        if (disablePause) return;
        GetTree().Paused = true;
    }

    void Unpause()
    {
        ProcessMode = ProcessModeEnum.Inherit;
        if (!disablePause)
            GetTree().Paused = false;
        disablePause = false;
    }

    #region Public

    public void AddConversion(DialogueData dialogueData, Action onComplete = null, bool disablePause = false)
    {
        if (inConversion || dialogueData?.conversionGroups == null || dialogueData.conversionGroups.Length == 0) return;
        Pause(disablePause);

        if (onComplete != null)
            completionCallbacks[dialogueData] = onComplete;

        Reset();
        inConversion = true;

        updateBackgroundImage(dialogueData.backgroundImage);
        foreach (var group in dialogueData.conversionGroups)
        {
            if (!conversionGroups.ContainsKey(group.id))
                conversionGroups[group.id] = group;
        }

        dialogueUI.Show();
        JumpToConversionGroup(dialogueData.conversionGroups[0]);
    }

    public void AddConversion(ConversionGroup[] conversionGroupsArray)
    {
        var dialogueData = new DialogueData { conversionGroups = conversionGroupsArray };
        AddConversion(dialogueData);
    }

    public async void CloseConversion()
    {
        var currentDialogue = completionCallbacks.Keys.FirstOrDefault();
        if (currentDialogue != null)
        {
            var callback = completionCallbacks[currentDialogue];
            completionCallbacks.Remove(currentDialogue);
            callback?.Invoke();
        }

        Reset();
        dialogueUI.Hide();

        inConversion = true;
        await Task.Delay(100);
        inConversion = false;
        Unpause();
    }

    #endregion

    #region Conversion Event
    void onConversionEnter(Conversion curConversion)
    {
        switch (curConversion.type)
        {
            case ConversionType.Default:
            case ConversionType.Close:
            case ConversionType.Jump:
                updateLabel();
                break;
            case ConversionType.Option:
                updateOption();
                break;
            case ConversionType.Action:
                updateLabel(); //TODO: handle action
                break;
        }
    }

    bool onConversionExit(Conversion conversion)
    {
        if (conversion == null) return false;

        switch (conversion.type)
        {
            case ConversionType.Jump:
                JumpToConversionGroup(conversion.jumpTo);
                return true;
            case ConversionType.Action:
                break;  //TODO: handle action
            case ConversionType.Option:
                break;
            case ConversionType.Close:
                CloseConversion();
                return true;
        }
        return false;
    }
    #endregion


    #region Option Handler
    void resetOption()
    {
        optionContainer.Hide();
        disabledShowNext = false;
    }

    void updateOption()
    {
        updateLabel();
        if (curConversion.options?.Length < 1)
            return;

        var optionLength = curConversion.options.Length;

        disabledShowNext = true;
        optionContainer.Show();
        optionContainer.Columns = optionLength is 2 or 4 ? 2 : 3;

        options = new();
        optionBts.ForEach(item => item.Hide());
        for (var i = 0; i < Mathf.Min(optionLength, 4); i++)
        {
            var option = curConversion.options[i];
            options.Add(option);
            var optionBt = optionBts[i];
            optionBt.Show();
            optionBt.Text = option.text;
        }
    }

    void OptionOnClick(int btIndex)
    {
        resetOption();

        if (btIndex >= options.Count)
        {
            GD.PrintErr("option index " + btIndex + " is out of range");
            return;
        }

        var option = options[btIndex];
        if (option == null)
        {
            GD.PrintErr("option index " + btIndex + " clicked, but option is not existed");
            return;
        }

        switch (option.type)
        {
            case ConversionOptionType.Jump:
                JumpToConversionGroup(option.jumpTo);
                break;
            case ConversionOptionType.Default:
                DialogueShowNext();
                break;
            case ConversionOptionType.Close:
                CloseConversion();
                break;
            case ConversionOptionType.Action:
                break;
            default:
                DialogueShowNext();
                break;
        }
    }

    #endregion

    #region Handler
    void JumpToConversionGroup(string targetId)
    {
        if (conversionGroups.GetValueOrDefault(targetId, null) == null)
        {
            GD.PrintErr($"Error: No matching conversion with ID: " + targetId + " found.");
            CloseConversion();
            return;
        }
        JumpToConversionGroup(conversionGroups.GetValueOrDefault(targetId, null));
    }

    void JumpToConversionGroup(ConversionGroup conversionGroup)
    {
        if (conversionGroup == null)
        {
            GD.PrintErr($"Error: No matching conversion found.");
            CloseConversion();
            return;
        }

        queue.Clear();
        curConversion = null;
        updateBackgroundImage(conversionGroup.backgroundImage);
        foreach (var conversion in conversionGroup.conversions)
        {
            queue.Enqueue(conversion);
        }
        DialogueShowNext();
    }

    void DialogueShowNext()
    {
        if (onConversionExit(curConversion))
            return;

        if (!queue.Any())
        {
            CloseConversion();
            return;
        }

        curConversion = queue.Dequeue();
        onConversionEnter(curConversion);
    }
    #endregion

    #region Helper
    void updateLabel()
    {
        var character = GetCharacter(curConversion.character);
        var talkingHeader = string.IsNullOrWhiteSpace(character.name)
            ? string.Empty
            : string.Format("{0}: ", character.name);

        targetText = string.Format("{0}{1}", talkingHeader, curConversion.text);
        currentDisplayText = string.Empty;
        isTyping = true;
        typeWriterTimer = 0;
        conversionLabel.Text = currentDisplayText;

        charImage.Texture = character.image;

        if (curConversion.backgroundImage != null)
            updateBackgroundImage(curConversion.backgroundImage);
    }

    public Character GetCharacter(CharacterT type) { return charactersMap.TryGetValue(type, out var character) ? character : new Character("Player"); }

    void updateBackgroundImage(Texture2D image, bool overrideVisible = false)
    {
        if (backgroundImageRect == null || (image == null && !overrideVisible))
            return;
        backgroundImageRect.Texture = image;
        backgroundImageRect.Visible = image != null;
    }
    #endregion

    #region Input Filter
    public override void _Input(InputEvent @event)
    {
        if (!inConversion) return;
        GetViewport().SetInputAsHandled();
        GD.Print("Blocked By Dialogue");
        if (!@event.IsAction("action"))
            return;

        if (isTyping)
        {
            currentDisplayText = targetText;
            conversionLabel.Text = currentDisplayText;
            isTyping = false;
            return;
        }

        if (disabledShowNext) return;
        DialogueShowNext();
    }
    #endregion
}
