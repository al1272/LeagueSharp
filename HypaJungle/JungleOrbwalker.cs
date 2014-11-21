﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace HypaJungle
{
    class JungleOrbwalker
    {


        private static Obj_AI_Hero player = ObjectManager.Player;

        private static int _lastAATick = 0;

        private static Spell _movementPrediction;

        private static int _lastMovement;

        public static void attackMinion(Obj_AI_Base target, Vector3 moveTo)
        {
            if (target != null && CanAttack())
            {
                if (player.IssueOrder(GameObjectOrder.AttackUnit, target))
                    _lastAATick = Environment.TickCount + Game.Ping / 2;
            }
                MoveTo(moveTo);
        }
        public static bool CanAttack()
        {
            if (_lastAATick <= Environment.TickCount)
            {
                return Environment.TickCount + Game.Ping / 2 + 25 >= _lastAATick + player.AttackDelay * 1000+130;
            }
            return false;
        }


        public static float GetAutoAttackRange(Obj_AI_Base source = null, Obj_AI_Base target = null)
        {
            if (source == null)
                source = player;
            var ret = source.AttackRange + player.BoundingRadius;
            if (target != null)
                ret += target.BoundingRadius;
            return ret;
        }

        private static void MoveTo(Vector3 position, float holdAreaRadius = -1)
        {
            var delay = 100;
            if (Environment.TickCount - _lastMovement < delay)
                return;
            _lastMovement = Environment.TickCount;

            if (!CanMove())
                return;
            if (player.Position.Distance(position)>50)
                player.IssueOrder(GameObjectOrder.MoveTo, position);
            return;
            if (holdAreaRadius < 0)
                holdAreaRadius = 20;
            if (player.ServerPosition.Distance(position) < holdAreaRadius)
            {
                if (player.Path.Count() > 1)
                    player.IssueOrder(GameObjectOrder.HoldPosition, player.ServerPosition);
                return;
            }
            if (position.Distance(player.Position) < 200)
                player.IssueOrder(GameObjectOrder.MoveTo, position);
            else
            {
                var point = player.ServerPosition +
                200 * (position.To2D() - player.ServerPosition.To2D()).Normalized().To3D();
                player.IssueOrder(GameObjectOrder.MoveTo, point);
            }

        }

        public static bool CanMove()
        {
            var extraWindup = 70;
            if (_lastAATick <= Environment.TickCount)
                return Environment.TickCount + Game.Ping / 2 >= _lastAATick + player.AttackCastDelay * 1000 + extraWindup;
            return false;
        }
    }
}
