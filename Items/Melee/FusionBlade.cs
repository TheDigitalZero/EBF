﻿using EBF.Extensions;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EBF.Items.Melee
{
    public class FusionBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 64;//Width of the hitbox of the item (usually the item's sprite width)
            Item.height = 64;//Height of the hitbox of the item (usually the item's sprite height)

            Item.damage = 52;//Item's base damage value
            Item.knockBack = 5;//Float, the item's knockback value. How far the enemy is launched when hit
            Item.DamageType = DamageClass.Melee;//Item's damage type, Melee, Ranged, Magic and Summon. Custom damage are also a thing
            Item.useStyle = ItemUseStyleID.Swing;//The animation of the item when used
            Item.useTime = 26;//How fast the item is used
            Item.useAnimation = 26;//How long the animation lasts. For swords it should stay the same as UseTime

            Item.value = Item.sellPrice(copper: 0, silver: 20, gold: 10, platinum: 0);//Item's value when sold
            Item.rare = ItemRarityID.Pink;//Item's name colour, this is hardcoded by the modder and should be based on progression
            Item.UseSound = SoundID.Item1;//The item's sound when it's used
            Item.autoReuse = true;//Boolean, if the item auto reuses if the use button is held
            Item.useTurn = false;//Boolean, if the player's direction can change while using the item

            Item.shoot = ModContent.ProjectileType<FusionBlade_BulletBob>();
            Item.shootSpeed = 1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(amount: 1)
                .AddIngredient(ItemID.HallowedBar, stack: 12)
                .AddIngredient(ItemID.Wire, stack: 40)
                .AddIngredient(ItemID.SoulofFright, stack: 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FusionBlade_BulletBob : ModProjectile
    {
        private NPC target;
        private float direction;
        private const float speed = 15;
        private const int homingRange = 800;
        private const int waitingFrames = 30;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.timeLeft = 60 * 4;

            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
        public override void OnSpawn(IEntitySource source)
        {
            direction = Projectile.velocity.ToRotation();
            Projectile.rotation = direction + MathHelper.PiOver2;
        }
        public override bool PreAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter == waitingFrames)
            {
                //Ignite
                Projectile.velocity *= speed;
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
            }
            
            if (Projectile.frameCounter >= waitingFrames)
            {
                //Animate sprite
                Projectile.frame++;
                if (Projectile.frame > 2)
                {
                    Projectile.frame = 1;
                }

                //Trail
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare);
                Lighting.AddLight(Projectile.Center, TorchID.Orange); //Orange lighting coming from the center of the Projectile.

                //Homing
                if (ProjectileExtensions.ClosestNPC(ref target, homingRange, Projectile.Center))
                {
                    direction = ProjectileExtensions.SlowRotation(direction, (target.Center - Projectile.Center).ToRotation(), 3f);
                    Projectile.velocity = direction.ToRotationVector2() * speed;
                    Projectile.rotation = direction + MathHelper.PiOver2;
                }
            }

            return false;
        }
    }
}
