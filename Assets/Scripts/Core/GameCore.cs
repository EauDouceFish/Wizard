using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq.Expressions;

public class GameCore : Architecture<GameCore>
{
    protected override void Init()
    {
        // ×¢²áModel
        this.RegisterModel(new GameCoreModel());
        this.RegisterModel(new MapModel());
        this.RegisterModel(new MagicInputModel());
        this.RegisterModel(new PlayerModel());
        this.RegisterModel(new GameEntityModel());
        this.RegisterModel(new PropModel());
        this.RegisterModel(new AudioModel());
        this.RegisterModel(new MagicSpellModel());

        // ÓÎÏ·Âß¼­Ïà¹Ø
        // ×¢²áSystem
        this.RegisterSystem(new MapGenerationSystem());
        this.RegisterSystem(new OutlineSystem());
        this.RegisterSystem(new InteractionSystem());
        this.RegisterSystem(new GameFlowSystem());
        this.RegisterSystem(new MagicSpellSystem());
        this.RegisterSystem(new ActorManageSystem());
        this.RegisterSystem(new AIDirectorSystem());
        this.RegisterSystem(new CharacterGrowthSystem());
        this.RegisterSystem(new AudioSystem());
        //this.RegisterSystem(new EffectSystem());
        this.RegisterSystem(new AttributeSystem());
        this.RegisterSystem(new UISystem());

        this.RegisterUtility(new Storage());
    }
}
