using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DodgeballMod.Content.Projectiles
{
    public class DodgeballProjectile : ModProjectile
    {
        public override string Texture =>
            $"Terraria/Images/Projectile_{ProjectileID.BeachBall}";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 300;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }
    }
}