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

        //this.RegisterModel(new PlayerModel());
        //this.RegisterModel(new EnemyModel());
        //this.RegisterModel(new BuffModel());
        //this.RegisterModel(new AudioModel());

        // ÓÎÏ·Âß¼­Ïà¹Ø
        // ×¢²áSystem
        this.RegisterSystem(new MapGenerationSystem());
        this.RegisterSystem(new OutlineSystem());
        this.RegisterSystem(new InteractionSystem());
        this.RegisterSystem(new GameFlowSystem());
        this.RegisterSystem(new MagicSpellSystem());
        //this.RegisterSystem(new AudioSystem());
        //this.RegisterSystem(new EffectSystem());
        //this.RegisterSystem(new BuffSystem());
        //this.RegisterSystem(new EnemySystem());
        //this.RegisterSystem(new StateMachineSystem());

        this.RegisterUtility(new Storage());
    }
}
