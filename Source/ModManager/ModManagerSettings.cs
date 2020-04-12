using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModManager
{
    public class ModManagerSettings : ModSettings
    {
        private List<ModAttributes> _saveableModAttributes;
        private List<ButtonAttributes> _saveableButtonAttributes;
        public Dictionary<string, ModAttributes> ModAttributes = new Dictionary<string, ModAttributes>();
        public Dictionary<string, ButtonAttributes> ButtonAttributes = new Dictionary<string, ButtonAttributes>();

        public bool ShowPromotions = true;
        public bool ShowPromotions_NotSubscribed = true;
        public bool ShowPromotions_NotActive = false;
        public bool TrimTags = true;
        public bool TrimVersionStrings = false;
        public bool AddModManagerToNewModLists = true;
        public bool ShowSatisfiedRequirements = false;
        public bool AddExpansionsToNewModLists = true;
        public bool ShowVersionChecksOnSteamMods = false;
        public bool AddHugsLibToNewModLists = false;

        public ModAttributes this[ModMetaData mod]
        {
            get
            {
                if (ModAttributes.ContainsKey(mod.PackageId))
                    return ModAttributes[mod.PackageId];
                var attributes = new ModAttributes(mod);
                ModAttributes.Add(mod.PackageId, attributes);
                return attributes;
            }
        }

        public ButtonAttributes this[ModButton_Installed button]
        {
            get
            {
                if (ButtonAttributes.ContainsKey(button.Name))
                    return ButtonAttributes[button.Name];
                var attributes = new ButtonAttributes(button);
                ButtonAttributes.Add(button.Name, attributes);
                return attributes;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if ( Scribe.mode == LoadSaveMode.Saving )
            {
                _saveableButtonAttributes = ButtonAttributes.Values
                    .Where( a => !a.IsDefault )
                    .ToList();
                _saveableModAttributes = ModAttributes.Values
                    .Where( a => !a.IsDefault )
                    .ToList();
                Debug.Log( $"Writing attributes:" +
                           $"\tButtons: {_saveableButtonAttributes.Count}/{ButtonAttributes.Count}" +
                           $"\tMods: {_saveableModAttributes.Count}/{ModAttributes.Count}" );
            }

            Scribe_Collections.Look( ref _saveableModAttributes, "ModAttributes", LookMode.Deep );
            Scribe_Collections.Look( ref _saveableButtonAttributes, "ButtonAttributes", LookMode.Deep );
            Scribe_Values.Look( ref ShowPromotions, "ShowPromotions", true );
            Scribe_Values.Look( ref ShowPromotions_NotSubscribed, "ShowPromotions_NotSubscribed", true );
            Scribe_Values.Look( ref ShowPromotions_NotActive, "ShowPromotions_NotActive", false );
            Scribe_Values.Look( ref TrimTags, "TrimTags", true );
            Scribe_Values.Look( ref TrimVersionStrings, "TrimVersionStrings", false );
            Scribe_Values.Look( ref AddHugsLibToNewModLists, "AddHugsLibToNewModLists", true );
            Scribe_Values.Look( ref AddModManagerToNewModLists, "AddModManagerToNewModLists", true );
            Scribe_Values.Look( ref AddExpansionsToNewModLists, "AddExpansionsToNewModLists", true );
            Scribe_Values.Look( ref ShowSatisfiedRequirements, "ShowSatisfiedRequirements", false );
            Scribe_Values.Look( ref ShowVersionChecksOnSteamMods, "ShowVersionChecksOnSteamMods", false );

            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
            {
                ModAttributes = new Dictionary<string, ModAttributes>();
                if ( !_saveableModAttributes.NullOrEmpty() )
                    foreach ( var modAttribute in _saveableModAttributes )
                        if ( !modAttribute.IsDefault )
                            ModAttributes.Add( modAttribute.Identifier, modAttribute );

                ButtonAttributes = new Dictionary<string, ButtonAttributes>();
                if ( !_saveableButtonAttributes.NullOrEmpty() )
                    foreach ( var buttonAttribute in _saveableButtonAttributes )
                        if ( !buttonAttribute.IsDefault )
                            ButtonAttributes.Add( buttonAttribute.Name, buttonAttribute );
            }
        }
    }
}