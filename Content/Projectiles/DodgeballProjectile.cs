using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DodgeballMod.Content.Projectiles
{
    public class DodgeballProjectile : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BeachBall}";

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

        public override void AI()
        {
            NPC target = FindClosestEnemy(500f);

            // No enemy nearby. Just continue moving normally.
            if (target == null)
                return;

            // Get a vector pointing from the ball to the enemy.
            Vector2 directionToTarget = target.Center - Projectile.Center;

            // Reduce that vector to direction only.
            directionToTarget.Normalize();

            // This is the velocity we'd ideally like to have.
            Vector2 desiredVelocity = directionToTarget * 12f;

            // Gradually turn our current velocity toward it.
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.08f);
        }

        private NPC FindClosestEnemy(float maxDistance)
        {
            // Start with no target selected.
            NPC closestEnemy = null;

            // Any enemy farther away than this will be ignored.
            float closestDistance = maxDistance;

            // Check every NPC slot in the world.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // Skip NPCs that shouldn't be targeted by homing projectiles.
                if (!npc.CanBeChasedBy())
                    continue;

                // Measure the distance from the dodgeball to this NPC.
                float distance = Vector2.Distance(Projectile.Center, npc.Center);

                // If this NPC is closer than our current best target,
                // remember it as the new closest enemy.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = npc;
                }
            }

            // Returns the closest valid enemy, or null if none were in range.
            return closestEnemy;
        }
    }
}