﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EBF.Items.Melee
{
    public class Anarchy : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.DisplayName.WithFormatArgs("Anarchy");//Name of the Item
            base.Tooltip.WithFormatArgs("Reduced defense while held.\nGrows stronger at low health.\n'It's gonna be worth it'");//Tooltip of the item
        }

        public override void SetDefaults()
        {
            Item.width = 112;//Width of the hitbox of the item (usually the item's sprite width)
            Item.height = 116;//Height of the hitbox of the item (usually the item's sprite height)

            Item.damage = 45;//Item's base damage value
            Item.knockBack = 3f;//Float, the item's knockback value. How far the enemy is launched when hit
            Item.DamageType = DamageClass.Melee;//Item's damage type, Melee, Ranged, Magic and Summon. Custom damage are also a thing
            Item.useStyle = ItemUseStyleID.Swing;//The animation of the item when used
            Item.useTime = 30;//How fast the item is used
            Item.useAnimation = 30;//How long the animation lasts. For swords it should stay the same as UseTime

            Item.value = Item.sellPrice(copper: 0, silver: 50, gold: 0, platinum: 0);//Item's value when sold
            Item.rare = ItemRarityID.LightRed;//Item's name colour, this is hardcoded by the modder and should be based on progression
            Item.UseSound = SoundID.Item1;//The item's sound when it's used
            Item.autoReuse = true;//Boolean, if the item auto reuses if the use button is held
            Item.useTurn = false;//Boolean, if the player's direction can change while using the item
        }

        public override void AddRecipes()
        {
            //Recipe that creates this item
            CreateRecipe(amount: 1)
                .AddIngredient(ItemID.SoulofNight, stack: 10)
                .AddIngredient<Avenger>(stack: 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.statLife <= player.statLifeMax / 2)
            {
                if (Main.rand.NextBool(3)) //reduces dust spawn frequency
                {
                    Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Blood);
                }
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            //Boost damage at half health
            if (player.statLife <= player.statLifeMax / 2)
            {
                damage *= 1.5f;
            }
        }

        public override void HoldItem(Player player)
        {
            //75% defense while held
            player.statDefense *= 0.75f;

            //There's no ModifyWeaponAttackSpeed method, so this will have to do...
            if (player.statLife <= player.statLifeMax / 2)
            {
                Item.useTime = 15;
                Item.useAnimation = 15;
            }
            else
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
            }
        }
    }
}
