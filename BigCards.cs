using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed
//C:\Program Files (x86)\Steam\steamapps\workshop\content\1811990\3175889529
namespace BigCards
{
    public partial class BigCardsWFMod : WildfrostMod
    {
        public static BigCardsWFMod instance;

        public BigCardsWFMod(string modDirectory) : base(modDirectory) {
            instance = this;
        }

        public override string GUID => "pheeb.wildfrost.beegcards";

        public override string[] Depends => new string[] {  };

        public override string Title => "Big Cards";

        public override string Description => "Big cards? In my normal sized card pools??";


        public static List<object> assets = new List<object>();
        private List<KeywordDataBuilder> keywords;
        private List<CardUpgradeDataBuilder> cardUpgrades;
        private List<CardDataBuilder> cards;
        private List<CardTypeBuilder> cardTypes;
        private List<StatusEffectDataBuilder> statusEffects;
        private List<TraitDataBuilder> traits;
        private bool preLoaded = false;


        private const string defaultUnitPNG = "default_unit.png";
        private const string defaultBackgroundPNG = "default_background.png";
        private const string defaultClunkerPNG = "default_clunker.png";
        private const string defaultItemPNG = "default_item.png";
        private const string defaultCharmPNG = "default_charm.png";

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
        }

        //Credits to Hopeful for this AddAssets code.
        public override List<T> AddAssets<T, Y>()   //AddAssets is called somewhere inside base.Load(). It is called multiple times, and each time T and Y are different DataFile and DataFileBuilders
        {
            if (assets.OfType<T>().Any())           //Checks if assets has any builders of the corresponding type. 
                Debug.LogWarning($"[{Title}] adding {typeof(Y).Name}s: {assets.OfType<T>().Count()}"); //Debug statement
            return assets.OfType<T>().ToList();     //Return the correct builders.
        }

        private void CreateModAssets()
        {
            keywords = new List<KeywordDataBuilder>();
            cardUpgrades = new List<CardUpgradeDataBuilder>();
            cardTypes = new List<CardTypeBuilder>();
            cards = new List<CardDataBuilder>();
            statusEffects = new List<StatusEffectDataBuilder>();
            traits = new List<TraitDataBuilder>();

            MakeKeywords();
            MakeUpgrades();
            MakeCardTypes();
            MakeCards();
            MakeStatusEffects();
            MakeTraits();

            preLoaded = true;
        }

        // TODO:
        // [] make Titan Charm: adds Titan to a unit (who isn't already large)
        private void MakeUpgrades()
        {
        }

        private void MakeCards()
        {
            assets.Add(
                new CardDataBuilder(this)
                .CreateUnit("giantpet", "Junior")
                .SetSprites(defaultClunkerPNG, defaultBackgroundPNG)
                .SetStats(10, 1, 4)
                .IsPet("", true)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new CardData.StatusEffectStacks[]
                    {
                        SStack("Titanic", 1),
                    };
                })
                );
            assets.Add(
                new CardDataBuilder(this)
                .CreateUnit("giantdweller", "Lumineti")
                .SetSprites("Lumineti.png", "Lumineti_background.png")
                .SetStats(16, 0, 6)
                .WithPools(new string[] { "BasicUnitPool" })
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new CardData.StatusEffectStacks[]
                    {
                        SStack("Titanic", 1),
                        SStack("When Negative Applied To Self Apply Negative To Enemies", 1),
                        SStack("When Positive Applied To Self Apply Positive To Allies", 1),
                    };
                })
                );
            assets.Add(
                new CardDataBuilder(this)
                .CreateUnit("giantclunker", "Giga Mimik")
                .SetSprites(defaultClunkerPNG, defaultBackgroundPNG)
                .SetStats(null, 8, 0)
                .WithCardType("Clunker")
                .WithPools(new string[] { "ClunkUnitPool" })
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.traits = new List<CardData.TraitStacks>() {
                        new CardData.TraitStacks(Get<TraitData>("Explode"), 8),
                    };
                    data.startWithEffects = new CardData.StatusEffectStacks[]
                    {
                        SStack("Scrap", 8),
                        SStack("Titanic", 1),
                        SStack("Trigger When Ally Attacks", 1),
                        SStack("On Card Played Lose Scrap To Self", 1),
                    };
                })
                );
            assets.Add(
                new CardDataBuilder(this)
                .CreateUnit("giantshade", "Shade Smith")
                .SetSprites(defaultClunkerPNG, defaultBackgroundPNG)
                .SetStats(8, null, 4)
                .WithCardType("Friendly")
                .WithPools(new string[] { "MagicUnitPool" })
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new CardData.StatusEffectStacks[]
                    {
                        SStack("Titanic", 1),
                        SStack("MultiHit", 1),
                        SStack("On Turn Summon Assassin", 1),
                    };
                })
                );
            assets.Add(
                new CardDataBuilder(this)
                .CreateUnit("tarBladeSummon", "Tar Blade Assassin")
                .SetSprites(defaultClunkerPNG, defaultBackgroundPNG)
                .SetStats(20, 2, 2)
                .WithCardType("Summoned")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.traits = new List<CardData.TraitStacks>() {
                        new CardData.TraitStacks(Get<TraitData>("Summoned"), 1),
                    };
                    data.startWithEffects = new CardData.StatusEffectStacks[]
                    {
                        SStack("Bonus Damage Equal To Darts In Hand", 1),
                        SStack("Trigger Against Target Hit With Tar Blade", 1),
                    };
                })
                );
        }

        private void MakeCardTypes()
        {
        }

        private void MakeStatusEffects()
        {
            assets.Add(new StatusEffectDataBuilder(this)
                .Create<StatusEffectApplyTitanWhenDrawn>("Titanic")
                .WithCanBeBoosted(false)
                .WithDoesDamage(false)
                .WithOffensive(false)
                .WithMakesOffensive(false)
                .WithIsReaction(false)
                .WithIsStatus(false)
                .WithVisible(true)
                .WithIsKeyword(true)
                .WithStackable(false)
                .WithText("{0}")
                .WithTextInsert($"<keyword={GUID}.titan>")
                );
            assets.Add(
                new StatusEffectDataBuilder(this)
                .Create<StatusEffectApplyXWhenXAppliedTo>("When Negative Applied To Self Apply Negative To Enemies")
                .WithText("When {0}, apply equal to all enemies")
                .WithTextInsert($"<keyword=frost>/<keyword=shroom>/<keyword=demonize>'d")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyXWhenXAppliedTo)data).whenAnyApplied = false;
                    ((StatusEffectApplyXWhenXAppliedTo)data).adjustAmount = false;
                    ((StatusEffectApplyXWhenXAppliedTo)data).applyEqualAmount = true;
                    ((StatusEffectApplyXWhenXAppliedTo)data).whenAppliedTypes = new string[] { "frost", "shroom", "demonize" };
                    ((StatusEffectApplyXWhenXAppliedTo)data).applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
                    ((StatusEffectApplyXWhenXAppliedTo)data).whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;

                })
                );
            assets.Add(
                new StatusEffectDataBuilder(this)
                .Create<StatusEffectApplyXWhenXAppliedTo>("When Positive Applied To Self Apply Positive To Allies")
                .WithText("When {0}, apply equal to all allies")
                .WithTextInsert($"<keyword=shell>/<keyword=spice>/<keyword=block>'d")
                .WithCanBeBoosted(false)
                .WithDoesDamage(false)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyXWhenXAppliedTo)data).whenAnyApplied = false;
                    ((StatusEffectApplyXWhenXAppliedTo)data).adjustAmount = false;
                    ((StatusEffectApplyXWhenXAppliedTo)data).applyEqualAmount = true;
                    ((StatusEffectApplyXWhenXAppliedTo)data).whenAppliedTypes = new string[] { "block", "shell", "spice" };
                    ((StatusEffectApplyXWhenXAppliedTo)data).applyToFlags = StatusEffectApplyX.ApplyToFlags.Allies;
                    ((StatusEffectApplyXWhenXAppliedTo)data).whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;

                })
                );

            assets.Add(
                StatusCopy("Summon Fallow", "Summon Assassin")                        
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)          
                {
                    ((StatusEffectSummon)data).summonCard = TryGet<CardData>($"{GUID}.tarBladeSummon");                                  
                })
                );
            assets.Add(
                StatusCopy("Instant Summon Fallow", "Instant Summon Assassin")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectInstantSummon)data).targetSummon = TryGet<StatusEffectSummon>($"Summon Assassin");
                    ((StatusEffectInstantSummon)data).withEffects = null;
                    ((StatusEffectInstantSummon)data).summonCopy = false;
                    ((StatusEffectInstantSummon)data).summonPosition = StatusEffectInstantSummon.Position.InFrontOf ;
                })
                );
            assets.Add(
                StatusCopy("On Turn Summon Bootleg Copy of RandomEnemy", "On Turn Summon Assassin")
                //StatusCopy("When Deployed Summon Wowee", "On Turn Summon Assassin")
                .WithText("Summon {0}")                                      
                .WithTextInsert($"<card={GUID}.tarBladeSummon>")              
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)             
                {
                    ((StatusEffectApplyXOnTurn)data).effectToApply = TryGet<StatusEffectData>("Instant Summon Assassin");
                    ((StatusEffectApplyXOnTurn)data).applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;

                })
                );

            assets.Add(
                new StatusEffectDataBuilder(this)
                .Create<StatusEffectTriggerWhenEnemyHitByItem>("Trigger Against Target Hit With Tar Blade")
                .WithText("Trigger against anything hit with a {0}")
                .WithTextInsert("<card=Dart>")
                .WithIsReaction(true)
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)   
                {
                    ((StatusEffectTriggerWhenEnemyHitByItem)data).validItems = new string[] { "Dart" };
                    ((StatusEffectTriggerWhenEnemyHitByItem)data).againstTarget = true;
                })
                );
        }

        private void MakeKeywords()
        {
            assets.Add(
                new KeywordDataBuilder(this)
                .Create("titan")
                .WithTitle("Titan")
                .WithDescription("Takes up 2 spaces | top and bottom") //Format is body|note.
                .WithTitleColour(new Color(0.23f, 0.96f, 0.80f))
                .WithNoteColour(new Color(0.90f, 0.52f, 0.22f))
                .WithCanStack(false)
                .WithShow(true)
                .WithShowName(true)
                );
        }
        private void MakeTraits()
        {
            assets.Add(
                new TraitDataBuilder(this)
                .Create("Titan")
                .SubscribeToAfterAllBuildEvent(
                    (trait) =>
                    {
                        trait.keyword = Get<KeywordData>("titan");
                    })
                );
        }

        // HELPERS
        private T TryGet<T>(string name) where T : DataFile
        {
            T data;
            if (typeof(StatusEffectData).IsAssignableFrom(typeof(T)))
                data = base.Get<StatusEffectData>(name) as T;
            else
                data = base.Get<T>(name);

            if (data == null)
                throw new Exception($"TryGet Error: Could not find a [{typeof(T).Name}] with the name [{name}] or [{Extensions.PrefixGUID(name, this)}]");

            return data;
        }
        private  CardData.StatusEffectStacks SStack(string name, int amount) => new CardData.StatusEffectStacks(Get<StatusEffectData>(name), amount);
        private CardData.TraitStacks TStack(string name, int amount) => new CardData.TraitStacks(Get<TraitData>(name), amount);
        private StatusEffectDataBuilder StatusCopy(string oldName, string newName)
        {
            StatusEffectData data = TryGet<StatusEffectData>(oldName).InstantiateKeepName();
            data.name = GUID + "." + newName;
            data.targetConstraints = new TargetConstraint[0];
            StatusEffectDataBuilder builder = data.Edit<StatusEffectData, StatusEffectDataBuilder>();
            builder.Mod = this;
            return builder;
        }
        private CardTypeBuilder CardTypeCopy(string oldName, string newName)
        {
            CardType data = Get<CardType>(oldName).InstantiateKeepName();     //Copies the card type
            data.name = newName;                                              //Changes its name
            CardTypeBuilder builder = data.Edit<CardType, CardTypeBuilder>(); //Wraps it in a builder
            builder.Mod = this;                                               //Gives the builder context.
            return builder;
        }
    }
}
