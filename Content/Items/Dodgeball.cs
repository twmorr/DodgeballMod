using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DodgeballMod.Content.Projectiles;

namespace DodgeballMod.Content.Items
{
    public class Dodgeball : ModItem
    {
        // Temporary texture so we don't need to make our own sprite yet.
        public override string Texture =>
            $"Terraria/Images/Item_{ItemID.BeachBall}";

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;

            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 5f;

            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<DodgeballProjectile>();
            Item.shootSpeed = 30f;

            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            /*Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();*/
        }
    }
}