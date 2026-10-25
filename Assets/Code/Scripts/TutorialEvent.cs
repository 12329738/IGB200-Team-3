public enum TutorialEvent
{
    None,

    PromptClicked,

    MaterialPlaced,

    FinalFormIconClicked,

    HistoryWindowClosed,

    SuccessfulMerge,

    ObjectHovered,

    ActionClicked,

    OptionChosen,

    DecorationMoved,

    GallerySaved,

    WasteCreated,

    WasteRecycled
}


public enum TutorialCondition
{
    Always,

    HasRecycling,

    NoRecycling
}


public enum TutorialPromptLocation
{
    Center,

    Materials,

    FinalForms,

    History,

    Object,

    Choices,

    Gallery,

    Waste
}